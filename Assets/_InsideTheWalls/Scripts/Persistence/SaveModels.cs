using System;
using InsideTheWalls.Simulation;

namespace InsideTheWalls.Persistence
{
    [Serializable]
    public sealed class PlayableDaySnapshot
    {
        public string role;
        public int objectiveIndex;
        public string choiceId;
        public string consequencePreview;

        public static PlayableDaySnapshot Create(PlayerRole role, int objectiveIndex, string choiceId, string consequencePreview)
        {
            return new PlayableDaySnapshot
            {
                role = role.ToString(),
                objectiveIndex = objectiveIndex,
                choiceId = choiceId ?? string.Empty,
                consequencePreview = consequencePreview ?? string.Empty
            };
        }

        public bool TryGetRole(out PlayerRole parsedRole)
        {
            return Enum.TryParse(role, false, out parsedRole) && Enum.IsDefined(typeof(PlayerRole), parsedRole);
        }
    }

    [Serializable]
    internal sealed class SaveEnvelope
    {
        public int schemaVersion;
        public string savedAtUtc;
        public PlayableDaySnapshot snapshot;
        public string checksum;
    }

    public enum SaveWriteStatus
    {
        Success,
        InvalidSnapshot,
        IoError
    }

    public enum SaveLoadStatus
    {
        Success,
        Recovered,
        Missing,
        Corrupt,
        Incompatible,
        IoError
    }

    public readonly struct SaveWriteResult
    {
        public SaveWriteResult(SaveWriteStatus status, string message)
        {
            Status = status;
            Message = message ?? string.Empty;
        }

        public SaveWriteStatus Status { get; }
        public string Message { get; }
        public bool Succeeded => Status == SaveWriteStatus.Success;
    }

    public readonly struct SaveLoadResult
    {
        public SaveLoadResult(SaveLoadStatus status, PlayableDaySnapshot snapshot, string message)
        {
            Status = status;
            Snapshot = snapshot;
            Message = message ?? string.Empty;
        }

        public SaveLoadStatus Status { get; }
        public PlayableDaySnapshot Snapshot { get; }
        public string Message { get; }
        public bool Succeeded => Status == SaveLoadStatus.Success || Status == SaveLoadStatus.Recovered;
        public bool IsCompatible => Succeeded;
    }

    public readonly struct SaveCompatibilityResult
    {
        public SaveCompatibilityResult(SaveLoadStatus status, string message)
        {
            Status = status;
            Message = message ?? string.Empty;
        }

        public SaveLoadStatus Status { get; }
        public string Message { get; }
        public bool CanContinue => Status == SaveLoadStatus.Success || Status == SaveLoadStatus.Recovered;
    }

    public interface ISavePathProvider
    {
        string RootDirectory { get; }
    }

    public interface IUtcClock
    {
        DateTime UtcNow { get; }
    }

    public sealed class SystemUtcClock : IUtcClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
