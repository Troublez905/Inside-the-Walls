using System;
using System.IO;
using InsideTheWalls.Persistence;
using NUnit.Framework;

namespace InsideTheWalls.Simulation.Tests
{
    public sealed class PersistenceTests
    {
        private string root;
        private LocalJsonSaveRepository repository;

        [SetUp]
        public void SetUp()
        {
            root = Path.Combine(Path.GetTempPath(), "InsideTheWalls.PersistenceTests", Guid.NewGuid().ToString("N"));
            repository = new LocalJsonSaveRepository(new FixedSavePathProvider(root));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }

        [Test]
        public void MissingSave_IsDistinctAndCannotContinue()
        {
            SaveLoadResult load = repository.Load();
            SaveCompatibilityResult compatibility = repository.Inspect();

            Assert.That(load.Status, Is.EqualTo(SaveLoadStatus.Missing));
            Assert.That(load.Snapshot, Is.Null);
            Assert.That(compatibility.CanContinue, Is.False);
        }

        [Test]
        public void NewGameSnapshot_RoundTripsAllPlayableState()
        {
            PlayableDaySnapshot snapshot = PlayableDaySnapshot.Create(PlayerRole.Inmate, 4, "deliver-message", "Noah trust preview");

            SaveWriteResult write = repository.Save(snapshot);
            SaveLoadResult load = repository.Load();

            Assert.That(write.Succeeded, Is.True, write.Message);
            Assert.That(load.Succeeded, Is.True, load.Message);
            Assert.That(load.Snapshot.role, Is.EqualTo("Inmate"));
            Assert.That(load.Snapshot.objectiveIndex, Is.EqualTo(4));
            Assert.That(load.Snapshot.choiceId, Is.EqualTo("deliver-message"));
            Assert.That(load.Snapshot.consequencePreview, Is.EqualTo("Noah trust preview"));
            Assert.That(repository.Inspect().CanContinue, Is.True);
        }

        [Test]
        public void SecondSave_CreatesBackupOfPreviousValidSave()
        {
            repository.Save(PlayableDaySnapshot.Create(PlayerRole.Inmate, 2, string.Empty, string.Empty));
            repository.Save(PlayableDaySnapshot.Create(PlayerRole.Officer, 5, "use-discretion", "Ward follow-up preview"));

            Assert.That(File.Exists(repository.BackupPath), Is.True);
            Assert.That(File.ReadAllText(repository.BackupPath), Does.Contain("Inmate"));
            Assert.That(repository.Load().Snapshot.role, Is.EqualTo("Officer"));
        }

        [Test]
        public void CorruptSave_IsReportedAndLeftUnchanged()
        {
            Directory.CreateDirectory(root);
            const string corrupt = "{ definitely-not-json";
            File.WriteAllText(repository.SavePath, corrupt);

            SaveLoadResult result = repository.Load();

            Assert.That(result.Status, Is.EqualTo(SaveLoadStatus.Corrupt));
            Assert.That(File.ReadAllText(repository.SavePath), Is.EqualTo(corrupt));
        }

        [Test]
        public void EmptySave_IsCorruptAndLeftUnchanged()
        {
            Directory.CreateDirectory(root);
            File.WriteAllText(repository.SavePath, string.Empty);

            Assert.That(repository.Load().Status, Is.EqualTo(SaveLoadStatus.Corrupt));
            Assert.That(File.ReadAllText(repository.SavePath), Is.Empty);
        }

        [Test]
        public void CorruptPrimary_LoadsValidatedBackupWithoutOverwritingEvidence()
        {
            repository.Save(PlayableDaySnapshot.Create(PlayerRole.Inmate, 2, "keep-route", "first"));
            repository.Save(PlayableDaySnapshot.Create(PlayerRole.Officer, 5, "use-discretion", "second"));
            const string corruptPrimary = "{ truncated";
            File.WriteAllText(repository.SavePath, corruptPrimary);

            SaveLoadResult result = repository.Load();

            Assert.That(result.Status, Is.EqualTo(SaveLoadStatus.Recovered));
            Assert.That(result.Snapshot.role, Is.EqualTo("Inmate"));
            Assert.That(File.ReadAllText(repository.SavePath), Is.EqualTo(corruptPrimary));
        }

        [Test]
        public void InterruptedFirstWrite_LoadsValidatedTemporaryFileWithoutPromotingIt()
        {
            repository.Save(PlayableDaySnapshot.Create(PlayerRole.Inmate, 3, "deliver-message", "preview"));
            File.Move(repository.SavePath, repository.TemporaryPath);

            SaveLoadResult result = repository.Load();

            Assert.That(result.Status, Is.EqualTo(SaveLoadStatus.Recovered));
            Assert.That(result.Snapshot.objectiveIndex, Is.EqualTo(3));
            Assert.That(File.Exists(repository.SavePath), Is.False);
            Assert.That(File.Exists(repository.TemporaryPath), Is.True);
        }

        [Test]
        public void IncompatibleSchema_IsDistinctAndLeftUnchanged()
        {
            Directory.CreateDirectory(root);
            const string incompatible = "{\"schemaVersion\":99,\"savedAtUtc\":\"2026-09-06T00:00:00Z\",\"snapshot\":{\"role\":\"Inmate\",\"objectiveIndex\":0,\"choiceId\":\"\",\"consequencePreview\":\"\"},\"checksum\":\"unused\"}";
            File.WriteAllText(repository.SavePath, incompatible);

            SaveLoadResult result = repository.Load();

            Assert.That(result.Status, Is.EqualTo(SaveLoadStatus.Incompatible));
            Assert.That(repository.Inspect().CanContinue, Is.False);
            Assert.That(File.ReadAllText(repository.SavePath), Is.EqualTo(incompatible));
        }

        [Test]
        public void MissingSchemaMarker_IsCorruptRatherThanCompatible()
        {
            Directory.CreateDirectory(root);
            const string schemaZero = "{\"snapshot\":{\"role\":\"Inmate\",\"objectiveIndex\":0,\"choiceId\":\"\",\"consequencePreview\":\"\"}}";
            File.WriteAllText(repository.SavePath, schemaZero);

            Assert.That(repository.Load().Status, Is.EqualTo(SaveLoadStatus.Corrupt));
        }

        [Test]
        public void TamperedPayload_IsCorruptAndNotReturned()
        {
            repository.Save(PlayableDaySnapshot.Create(PlayerRole.Inmate, 1, string.Empty, string.Empty));
            string json = File.ReadAllText(repository.SavePath).Replace("\"objectiveIndex\": 1", "\"objectiveIndex\": 2");
            File.WriteAllText(repository.SavePath, json);

            SaveLoadResult result = repository.Load();

            Assert.That(result.Status, Is.EqualTo(SaveLoadStatus.Corrupt));
            Assert.That(result.Snapshot, Is.Null);
        }

        [TestCase(-1)]
        [TestCase(8)]
        public void InvalidObjectiveIndex_IsRejectedBeforeWriting(int objectiveIndex)
        {
            SaveWriteResult result = repository.Save(PlayableDaySnapshot.Create(PlayerRole.Inmate, objectiveIndex, string.Empty, string.Empty));

            Assert.That(result.Status, Is.EqualTo(SaveWriteStatus.InvalidSnapshot));
            Assert.That(File.Exists(repository.SavePath), Is.False);
        }

        [TestCase(0)]
        [TestCase(7)]
        public void ObjectiveBoundary_IsValid(int objectiveIndex)
        {
            Assert.That(repository.Save(PlayableDaySnapshot.Create(PlayerRole.Inmate, objectiveIndex, string.Empty, string.Empty)).Succeeded, Is.True);
        }

        [Test]
        public void InvalidRole_IsRejectedBeforeWriting()
        {
            var snapshot = new PlayableDaySnapshot
            {
                role = "Administrator",
                objectiveIndex = 0,
                choiceId = string.Empty,
                consequencePreview = string.Empty
            };

            Assert.That(repository.Save(snapshot).Status, Is.EqualTo(SaveWriteStatus.InvalidSnapshot));
            Assert.That(File.Exists(repository.SavePath), Is.False);
        }

        [Test]
        public void ReplaceFailure_PreservesPreviousPrimaryAndReturnsIoError()
        {
            repository.Save(PlayableDaySnapshot.Create(PlayerRole.Inmate, 1, string.Empty, "original"));
            string original = File.ReadAllText(repository.SavePath);
            var failingRepository = new LocalJsonSaveRepository(
                new FixedSavePathProvider(root),
                new FixedClock(),
                new ReplaceFailingFileOperations());

            SaveWriteResult result = failingRepository.Save(PlayableDaySnapshot.Create(PlayerRole.Officer, 4, "document", "replacement"));

            Assert.That(result.Status, Is.EqualTo(SaveWriteStatus.IoError));
            Assert.That(File.ReadAllText(repository.SavePath), Is.EqualTo(original));
        }

        [Test]
        public void RootThatIsAFile_ReturnsIoError()
        {
            string fileRoot = Path.Combine(root, "not-a-directory");
            Directory.CreateDirectory(root);
            File.WriteAllText(fileRoot, "occupied");
            var fileRepository = new LocalJsonSaveRepository(new FixedSavePathProvider(fileRoot));

            SaveWriteResult result = fileRepository.Save(PlayableDaySnapshot.Create(PlayerRole.Officer, 0, string.Empty, string.Empty));

            Assert.That(result.Status, Is.EqualTo(SaveWriteStatus.IoError));
        }

        private sealed class FixedClock : IUtcClock
        {
            public DateTime UtcNow => new DateTime(2026, 9, 6, 12, 0, 0, DateTimeKind.Utc);
        }

        private sealed class ReplaceFailingFileOperations : ISaveFileOperations
        {
            private readonly SystemSaveFileOperations inner = new SystemSaveFileOperations();
            public bool FileExists(string path) => inner.FileExists(path);
            public bool DirectoryExists(string path) => inner.DirectoryExists(path);
            public FileAttributes GetAttributes(string path) => inner.GetAttributes(path);
            public void CreateDirectory(string path) => inner.CreateDirectory(path);
            public string ReadAllText(string path) => inner.ReadAllText(path);
            public void WriteThrough(string path, string contents) => inner.WriteThrough(path, contents);
            public void Move(string source, string destination) => inner.Move(source, destination);
            public void Replace(string source, string destination, string backup) => throw new IOException("Injected replace failure.");
        }
    }
}
