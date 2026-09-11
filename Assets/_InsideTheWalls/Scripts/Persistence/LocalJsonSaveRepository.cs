using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace InsideTheWalls.Persistence
{
    public interface ISaveFileOperations
    {
        bool FileExists(string path);
        bool DirectoryExists(string path);
        FileAttributes GetAttributes(string path);
        void CreateDirectory(string path);
        string ReadAllText(string path);
        void WriteThrough(string path, string contents);
        void Move(string source, string destination);
        void Replace(string source, string destination, string backup);
    }

    public sealed class SystemSaveFileOperations : ISaveFileOperations
    {
        public bool FileExists(string path) => File.Exists(path);
        public bool DirectoryExists(string path) => Directory.Exists(path);
        public FileAttributes GetAttributes(string path) => File.GetAttributes(path);
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
        public string ReadAllText(string path) => File.ReadAllText(path, Encoding.UTF8);
        public void Move(string source, string destination) => File.Move(source, destination);
        public void Replace(string source, string destination, string backup) => File.Replace(source, destination, backup, true);

        public void WriteThrough(string path, string contents)
        {
            byte[] bytes = new UTF8Encoding(false).GetBytes(contents);
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush(true);
            }
        }
    }

    public sealed class PersistentDataSavePathProvider : ISavePathProvider
    {
        public string RootDirectory => Application.persistentDataPath;
    }

    public sealed class FixedSavePathProvider : ISavePathProvider
    {
        public FixedSavePathProvider(string rootDirectory)
        {
            RootDirectory = string.IsNullOrWhiteSpace(rootDirectory)
                ? throw new ArgumentException("A save root directory is required.", nameof(rootDirectory))
                : Path.GetFullPath(rootDirectory);
        }

        public string RootDirectory { get; }
    }

    public sealed class LocalJsonSaveRepository
    {
        public const int CurrentSchemaVersion = 1;
        public const string SaveFileName = "playable-day.json";
        private const string TemporarySuffix = ".tmp";
        private const string BackupSuffix = ".backup";
        private const int MaximumObjectiveIndex = 7;
        private readonly string rootDirectory;
        private readonly string savePath;
        private readonly string temporaryPath;
        private readonly string backupPath;
        private readonly IUtcClock clock;
        private readonly ISaveFileOperations files;

        public LocalJsonSaveRepository(ISavePathProvider pathProvider, IUtcClock clock = null, ISaveFileOperations files = null)
        {
            if (pathProvider == null) throw new ArgumentNullException(nameof(pathProvider));
            string root = pathProvider.RootDirectory;
            if (string.IsNullOrWhiteSpace(root)) throw new ArgumentException("The save root directory is empty.", nameof(pathProvider));

            string normalizedRoot = Path.GetFullPath(root);
            string volumeRoot = Path.GetPathRoot(normalizedRoot);
            rootDirectory = string.Equals(normalizedRoot, volumeRoot, StringComparison.OrdinalIgnoreCase)
                ? normalizedRoot
                : normalizedRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            savePath = Path.GetFullPath(Path.Combine(rootDirectory, SaveFileName));
            if (!string.Equals(Path.GetDirectoryName(savePath), rootDirectory, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The save path must remain inside its configured root.", nameof(pathProvider));
            temporaryPath = savePath + TemporarySuffix;
            backupPath = savePath + BackupSuffix;
            this.clock = clock ?? new SystemUtcClock();
            this.files = files ?? new SystemSaveFileOperations();
        }

        public string SavePath => savePath;
        public string BackupPath => backupPath;
        public string TemporaryPath => temporaryPath;

        public SaveWriteResult Save(PlayableDaySnapshot snapshot)
        {
            if (!IsValidSnapshot(snapshot, out string validationMessage))
                return new SaveWriteResult(SaveWriteStatus.InvalidSnapshot, validationMessage);

            try
            {
                EnsurePathsAreNotReparsePoints();
                string directory = Path.GetDirectoryName(savePath);
                if (string.IsNullOrWhiteSpace(directory))
                    return new SaveWriteResult(SaveWriteStatus.IoError, "The save directory could not be resolved.");

                files.CreateDirectory(directory);
                var envelope = new SaveEnvelope
                {
                    schemaVersion = CurrentSchemaVersion,
                    savedAtUtc = clock.UtcNow.ToUniversalTime().ToString("O"),
                    snapshot = snapshot,
                    checksum = string.Empty
                };
                envelope.checksum = ComputeChecksum(envelope.schemaVersion, envelope.savedAtUtc, snapshot);
                string json = JsonUtility.ToJson(envelope, true);
                files.WriteThrough(temporaryPath, json);
                SaveLoadResult temporaryValidation = LoadPath(temporaryPath, false);
                if (!temporaryValidation.Succeeded)
                    return new SaveWriteResult(SaveWriteStatus.IoError, "The flushed temporary save failed validation; the previous save was preserved.");

                if (files.FileExists(savePath))
                    files.Replace(temporaryPath, savePath, backupPath);
                else
                    files.Move(temporaryPath, savePath);

                return new SaveWriteResult(SaveWriteStatus.Success, "Playable day saved.");
            }
            catch (UnauthorizedAccessException exception)
            {
                return IoWriteFailure(exception);
            }
            catch (IOException exception)
            {
                return IoWriteFailure(exception);
            }
            catch (SecurityException exception)
            {
                return IoWriteFailure(exception);
            }
            catch (NotSupportedException exception)
            {
                return IoWriteFailure(exception);
            }
        }

        public SaveCompatibilityResult Inspect()
        {
            SaveLoadResult load = Load();
            return new SaveCompatibilityResult(load.Status, load.Message);
        }

        public SaveLoadResult Load()
        {
            try
            {
                EnsurePathsAreNotReparsePoints();
            }
            catch (UnauthorizedAccessException exception)
            {
                return IoLoadFailure(exception);
            }
            catch (IOException exception)
            {
                return IoLoadFailure(exception);
            }
            catch (SecurityException exception)
            {
                return IoLoadFailure(exception);
            }
            catch (NotSupportedException exception)
            {
                return IoLoadFailure(exception);
            }

            SaveLoadResult primary = LoadPath(savePath, true);
            if (primary.Succeeded || primary.Status == SaveLoadStatus.Incompatible) return primary;

            if (primary.Status == SaveLoadStatus.Missing)
            {
                SaveLoadResult interrupted = LoadPath(temporaryPath, true);
                if (interrupted.Succeeded)
                    return new SaveLoadResult(SaveLoadStatus.Recovered, interrupted.Snapshot, "A valid interrupted first-write snapshot is available; the temporary file was left unchanged.");
            }

            SaveLoadResult backup = LoadPath(backupPath, true);
            if (backup.Succeeded)
            {
                return new SaveLoadResult(SaveLoadStatus.Recovered, backup.Snapshot, "The primary save was unavailable or corrupt; a validated backup was loaded without replacing evidence.");
            }

            return primary;
        }

        private SaveLoadResult LoadPath(string path, bool reportMissing)
        {
            if (!files.FileExists(path))
                return new SaveLoadResult(SaveLoadStatus.Missing, null, reportMissing ? "No saved playable day exists." : "Temporary save is missing.");

            string json;
            try
            {
                json = files.ReadAllText(path);
            }
            catch (UnauthorizedAccessException)
            {
                return new SaveLoadResult(SaveLoadStatus.IoError, null, "Save data could not be read because access was denied.");
            }
            catch (IOException)
            {
                return new SaveLoadResult(SaveLoadStatus.IoError, null, "Save data could not be read due to a storage error.");
            }
            catch (SecurityException)
            {
                return new SaveLoadResult(SaveLoadStatus.IoError, null, "Save data could not be read because the storage path is unsafe.");
            }
            catch (NotSupportedException)
            {
                return new SaveLoadResult(SaveLoadStatus.IoError, null, "Save data could not be read on this storage platform.");
            }

            if (string.IsNullOrWhiteSpace(json))
                return new SaveLoadResult(SaveLoadStatus.Corrupt, null, "The save file is empty and was left unchanged.");

            SaveEnvelope envelope;
            try
            {
                envelope = JsonUtility.FromJson<SaveEnvelope>(json);
            }
            catch (ArgumentException)
            {
                return new SaveLoadResult(SaveLoadStatus.Corrupt, null, "The save file is malformed and was left unchanged.");
            }

            if (envelope == null || envelope.snapshot == null)
                return new SaveLoadResult(SaveLoadStatus.Corrupt, null, "The save payload is missing and the file was left unchanged.");
            if (envelope.schemaVersion == 0)
                return new SaveLoadResult(SaveLoadStatus.Corrupt, null, "The save schema marker is missing and the file was left unchanged.");
            if (envelope.schemaVersion != CurrentSchemaVersion)
                return new SaveLoadResult(SaveLoadStatus.Incompatible, null, $"Save schema {envelope.schemaVersion} is incompatible with schema {CurrentSchemaVersion}.");
            if (!DateTime.TryParse(envelope.savedAtUtc, null, System.Globalization.DateTimeStyles.RoundtripKind, out _))
                return new SaveLoadResult(SaveLoadStatus.Corrupt, null, "The save timestamp is invalid and the file was left unchanged.");
            if (!IsValidSnapshot(envelope.snapshot, out string validationMessage))
                return new SaveLoadResult(SaveLoadStatus.Corrupt, null, $"The save payload is invalid and was left unchanged: {validationMessage}");
            if (!FixedTimeEquals(envelope.checksum, ComputeChecksum(envelope.schemaVersion, envelope.savedAtUtc, envelope.snapshot)))
                return new SaveLoadResult(SaveLoadStatus.Corrupt, null, "The save checksum does not match and the file was left unchanged.");

            return new SaveLoadResult(SaveLoadStatus.Success, envelope.snapshot, "Playable day loaded.");
        }

        private static bool IsValidSnapshot(PlayableDaySnapshot snapshot, out string message)
        {
            if (snapshot == null)
            {
                message = "Snapshot is required.";
                return false;
            }

            if (!snapshot.TryGetRole(out _))
            {
                message = "Snapshot role is invalid.";
                return false;
            }

            if (snapshot.objectiveIndex < 0 || snapshot.objectiveIndex > MaximumObjectiveIndex)
            {
                message = $"Objective index must be between 0 and {MaximumObjectiveIndex}.";
                return false;
            }

            if (snapshot.choiceId == null || snapshot.consequencePreview == null)
            {
                message = "Choice and consequence fields cannot be null.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private void EnsurePathsAreNotReparsePoints()
        {
            if (files.DirectoryExists(rootDirectory)
                && (files.GetAttributes(rootDirectory) & FileAttributes.ReparsePoint) != 0)
            {
                throw new SecurityException("The configured save directory cannot be a reparse point.");
            }

            RejectFileReparsePoint(savePath);
            RejectFileReparsePoint(temporaryPath);
            RejectFileReparsePoint(backupPath);
        }

        private void RejectFileReparsePoint(string path)
        {
            if (files.FileExists(path) && (files.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                throw new SecurityException($"Save storage path is a reparse point: {path}");
        }

        private static string ComputeChecksum(int schemaVersion, string savedAtUtc, PlayableDaySnapshot snapshot)
        {
            string canonical = string.Join("\n", schemaVersion.ToString(), savedAtUtc, snapshot.role, snapshot.objectiveIndex.ToString(), snapshot.choiceId, snapshot.consequencePreview);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(canonical));
                var builder = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++) builder.Append(hash[i].ToString("x2"));
                return builder.ToString();
            }
        }

        private static bool FixedTimeEquals(string left, string right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            int difference = 0;
            for (int i = 0; i < left.Length; i++) difference |= left[i] ^ right[i];
            return difference == 0;
        }

        private static SaveWriteResult IoWriteFailure(Exception exception)
        {
            _ = exception;
            return new SaveWriteResult(SaveWriteStatus.IoError, "Save write failed due to a storage or permission error; the previous save was preserved where possible.");
        }

        private static SaveLoadResult IoLoadFailure(Exception exception)
        {
            _ = exception;
            return new SaveLoadResult(SaveLoadStatus.IoError, null, "Save read failed due to a storage, permission, or path-safety error.");
        }
    }
}
