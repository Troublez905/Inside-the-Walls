using System;
using System.Collections.Generic;

namespace InsideTheWalls.Simulation
{
    public enum PlayerRole
    {
        Inmate,
        Officer
    }

    public enum AuthorityAction
    {
        OpenDoor,
        UnlockDoor
    }

    public enum DoorAccess
    {
        Public,
        AssignmentRestricted,
        OfficerControlled,
        StaffOnly
    }

    public enum AuthorityRejection
    {
        None,
        StaleRevision,
        ScheduleMismatch,
        WrongRole,
        WrongAssignment,
        OutsideScheduleWindow,
        MissingCredential,
        DoorLocked
    }

    public sealed class ActorState
    {
        private readonly HashSet<string> credentials;

        public ActorState(string actorId, PlayerRole role, string assignmentId, IEnumerable<string> credentials = null)
        {
            ActorId = string.IsNullOrWhiteSpace(actorId) ? throw new ArgumentException("Actor ID is required.", nameof(actorId)) : actorId;
            Role = role;
            AssignmentId = assignmentId ?? string.Empty;
            this.credentials = credentials == null
                ? new HashSet<string>(StringComparer.Ordinal)
                : new HashSet<string>(credentials, StringComparer.Ordinal);
        }

        public string ActorId { get; }
        public PlayerRole Role { get; }
        public string AssignmentId { get; }
        public bool HasCredential(string credentialId) => !string.IsNullOrEmpty(credentialId) && credentials.Contains(credentialId);
    }

    public sealed class DoorState
    {
        public DoorState(string doorId, DoorAccess access, bool isLocked, string requiredAssignmentId = "", string requiredCredentialId = "")
        {
            DoorId = string.IsNullOrWhiteSpace(doorId) ? throw new ArgumentException("Door ID is required.", nameof(doorId)) : doorId;
            if ((access == DoorAccess.OfficerControlled || access == DoorAccess.StaffOnly)
                && string.IsNullOrWhiteSpace(requiredCredentialId))
            {
                throw new ArgumentException("Controlled and staff-only doors require a credential ID.", nameof(requiredCredentialId));
            }

            Access = access;
            IsLocked = isLocked;
            RequiredAssignmentId = requiredAssignmentId ?? string.Empty;
            RequiredCredentialId = requiredCredentialId ?? string.Empty;
        }

        public string DoorId { get; }
        public DoorAccess Access { get; }
        public bool IsLocked { get; private set; }
        public string RequiredAssignmentId { get; }
        public string RequiredCredentialId { get; }
        internal void Unlock() => IsLocked = false;
    }

    public readonly struct AuthorityRequest
    {
        public AuthorityRequest(string actorId, AuthorityAction action, string targetId, int minute, int expectedRevision)
        {
            ActorId = actorId ?? string.Empty;
            Action = action;
            TargetId = targetId ?? string.Empty;
            Minute = minute;
            ExpectedRevision = expectedRevision;
        }

        public string ActorId { get; }
        public AuthorityAction Action { get; }
        public string TargetId { get; }
        public int Minute { get; }
        public int ExpectedRevision { get; }
    }

    public readonly struct AuthorityDecision
    {
        private AuthorityDecision(bool accepted, AuthorityRejection rejection)
        {
            Accepted = accepted;
            Rejection = rejection;
        }

        public bool Accepted { get; }
        public AuthorityRejection Rejection { get; }
        public static AuthorityDecision Allow() => new AuthorityDecision(true, AuthorityRejection.None);
        public static AuthorityDecision Reject(AuthorityRejection reason) => new AuthorityDecision(false, reason);
    }

    public interface IActionValidator
    {
        AuthorityDecision Validate(AuthorityRequest request, ActorState actor, DoorState door, SchedulePhaseId phase, int currentRevision);
    }

    public sealed class DoorActionValidator : IActionValidator
    {
        public AuthorityDecision Validate(AuthorityRequest request, ActorState actor, DoorState door, SchedulePhaseId phase, int currentRevision)
        {
            if (request.ExpectedRevision != currentRevision) return AuthorityDecision.Reject(AuthorityRejection.StaleRevision);
            SchedulePhaseId authoritativePhase;
            try
            {
                authoritativePhase = DaySchedule.CreateFoundationDay().PhaseAt(request.Minute).Id;
            }
            catch (ArgumentOutOfRangeException)
            {
                return AuthorityDecision.Reject(AuthorityRejection.ScheduleMismatch);
            }

            if (phase != authoritativePhase) return AuthorityDecision.Reject(AuthorityRejection.ScheduleMismatch);
            if (!string.Equals(request.ActorId, actor.ActorId, StringComparison.Ordinal)) return AuthorityDecision.Reject(AuthorityRejection.WrongRole);
            if (!string.Equals(request.TargetId, door.DoorId, StringComparison.Ordinal)) return AuthorityDecision.Reject(AuthorityRejection.DoorLocked);

            if (request.Action == AuthorityAction.UnlockDoor)
            {
                if (actor.Role != PlayerRole.Officer) return AuthorityDecision.Reject(AuthorityRejection.WrongRole);
                if (door.Access != DoorAccess.OfficerControlled && door.Access != DoorAccess.StaffOnly)
                    return AuthorityDecision.Reject(AuthorityRejection.WrongRole);
                if (!actor.HasCredential(door.RequiredCredentialId)) return AuthorityDecision.Reject(AuthorityRejection.MissingCredential);
                if (!HasMovementPost(actor, door)) return AuthorityDecision.Reject(AuthorityRejection.WrongAssignment);
                if (!IsOperationalPhase(phase)) return AuthorityDecision.Reject(AuthorityRejection.OutsideScheduleWindow);
                return AuthorityDecision.Allow();
            }

            if (door.IsLocked) return AuthorityDecision.Reject(AuthorityRejection.DoorLocked);
            if (!IsOpenForRole(actor, door)) return AuthorityDecision.Reject(AuthorityRejection.WrongRole);
            if (door.Access == DoorAccess.AssignmentRestricted && !HasAssignment(actor, door))
                return AuthorityDecision.Reject(AuthorityRejection.WrongAssignment);
            if (!IsOperationalPhase(phase)) return AuthorityDecision.Reject(AuthorityRejection.OutsideScheduleWindow);
            return AuthorityDecision.Allow();
        }

        private static bool IsOpenForRole(ActorState actor, DoorState door)
        {
            switch (door.Access)
            {
                case DoorAccess.Public:
                case DoorAccess.AssignmentRestricted:
                    return true;
                case DoorAccess.OfficerControlled:
                case DoorAccess.StaffOnly:
                    return actor.Role == PlayerRole.Officer;
                default:
                    return false;
            }
        }

        private static bool HasAssignment(ActorState actor, DoorState door)
        {
            return string.Equals(actor.AssignmentId, door.RequiredAssignmentId, StringComparison.Ordinal);
        }

        private static bool HasMovementPost(ActorState actor, DoorState door)
        {
            return string.IsNullOrEmpty(door.RequiredAssignmentId) || HasAssignment(actor, door);
        }

        private static bool IsOperationalPhase(SchedulePhaseId phase)
        {
            return phase != SchedulePhaseId.Onboarding && phase != SchedulePhaseId.Lockdown;
        }
    }

    public sealed class OfflineAuthorityGateway
    {
        private readonly IActionValidator validator;

        public OfflineAuthorityGateway(IActionValidator validator)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public int Revision { get; private set; }

        public AuthorityDecision Execute(AuthorityRequest request, ActorState actor, DoorState door, SchedulePhaseId phase)
        {
            AuthorityDecision decision = validator.Validate(request, actor, door, phase, Revision);
            if (!decision.Accepted) return decision;

            if (request.Action == AuthorityAction.UnlockDoor) door.Unlock();
            Revision++;
            return decision;
        }
    }
}
