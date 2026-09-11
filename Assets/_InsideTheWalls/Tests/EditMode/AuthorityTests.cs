using NUnit.Framework;

namespace InsideTheWalls.Simulation.Tests
{
    public sealed class AuthorityTests
    {
        private const string MovementPost = "UnitA-MovementPost";
        private const string KeyRing = "UnitA-KeyRing";

        [Test]
        public void AssignedInmate_CanOpenUnlockedWorkDoorDuringWork()
        {
            var actor = new ActorState("inmate", PlayerRole.Inmate, "Laundry-Worker");
            var door = new DoorState("laundry", DoorAccess.AssignmentRestricted, false, "Laundry-Worker");
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            AuthorityDecision result = gateway.Execute(Request(actor, door, AuthorityAction.OpenDoor), actor, door, SchedulePhaseId.WorkAndPostDuty);

            Assert.That(result.Accepted, Is.True);
            Assert.That(gateway.Revision, Is.EqualTo(1));
        }

        [Test]
        public void UnassignedInmate_IsRejectedWithoutMutation()
        {
            var actor = new ActorState("inmate", PlayerRole.Inmate, "Kitchen-Worker");
            var door = new DoorState("laundry", DoorAccess.AssignmentRestricted, false, "Laundry-Worker");
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            AuthorityDecision result = gateway.Execute(Request(actor, door, AuthorityAction.OpenDoor), actor, door, SchedulePhaseId.WorkAndPostDuty);

            Assert.That(result.Accepted, Is.False);
            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.WrongAssignment));
            Assert.That(door.IsLocked, Is.False);
            Assert.That(gateway.Revision, Is.Zero);
        }

        [Test]
        public void AssignedOfficer_WithCredentialCanUnlockControlledDoor()
        {
            var actor = new ActorState("officer", PlayerRole.Officer, MovementPost, new[] { KeyRing });
            var door = new DoorState("route", DoorAccess.OfficerControlled, true, MovementPost, KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            AuthorityDecision result = gateway.Execute(Request(actor, door, AuthorityAction.UnlockDoor), actor, door, SchedulePhaseId.WorkMovement);

            Assert.That(result.Accepted, Is.True);
            Assert.That(door.IsLocked, Is.False);
            Assert.That(gateway.Revision, Is.EqualTo(1));
        }

        [Test]
        public void OfficerWithoutCredential_CannotUnlockAndDoorRemainsLocked()
        {
            var actor = new ActorState("officer", PlayerRole.Officer, MovementPost);
            var door = new DoorState("route", DoorAccess.OfficerControlled, true, MovementPost, KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            AuthorityDecision result = gateway.Execute(Request(actor, door, AuthorityAction.UnlockDoor), actor, door, SchedulePhaseId.WorkMovement);

            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.MissingCredential));
            Assert.That(door.IsLocked, Is.True);
            Assert.That(gateway.Revision, Is.Zero);
        }

        [Test]
        public void Inmate_CannotUnlockOfficerControlledDoor()
        {
            var actor = new ActorState("inmate", PlayerRole.Inmate, "Laundry-Worker", new[] { KeyRing });
            var door = new DoorState("route", DoorAccess.OfficerControlled, true, MovementPost, KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            AuthorityDecision result = gateway.Execute(Request(actor, door, AuthorityAction.UnlockDoor), actor, door, SchedulePhaseId.WorkMovement);

            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.WrongRole));
            Assert.That(door.IsLocked, Is.True);
            Assert.That(gateway.Revision, Is.Zero);
        }

        [Test]
        public void Inmate_CannotOpenUnlockedOfficerControlledDoor()
        {
            var actor = new ActorState("inmate", PlayerRole.Inmate, "Laundry-Worker");
            var door = new DoorState("route", DoorAccess.OfficerControlled, false, MovementPost, KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            AuthorityDecision result = gateway.Execute(Request(actor, door, AuthorityAction.OpenDoor), actor, door, SchedulePhaseId.WorkMovement);

            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.WrongRole));
            Assert.That(gateway.Revision, Is.Zero);
        }

        [Test]
        public void RequestMinute_MustMatchSuppliedPhase()
        {
            var actor = new ActorState("officer", PlayerRole.Officer, MovementPost, new[] { KeyRing });
            var door = new DoorState("route", DoorAccess.OfficerControlled, true, MovementPost, KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());
            var request = new AuthorityRequest(actor.ActorId, AuthorityAction.UnlockDoor, door.DoorId, 590, 0);

            AuthorityDecision result = gateway.Execute(request, actor, door, SchedulePhaseId.WorkMovement);

            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.ScheduleMismatch));
            Assert.That(door.IsLocked, Is.True);
            Assert.That(gateway.Revision, Is.Zero);
        }

        [TestCase(439)]
        [TestCase(600)]
        public void RequestMinute_OutsidePlayableDayIsRejected(int minute)
        {
            var actor = new ActorState("officer", PlayerRole.Officer, MovementPost, new[] { KeyRing });
            var door = new DoorState("route", DoorAccess.OfficerControlled, true, MovementPost, KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());
            var request = new AuthorityRequest(actor.ActorId, AuthorityAction.UnlockDoor, door.DoorId, minute, 0);

            AuthorityDecision result = gateway.Execute(request, actor, door, SchedulePhaseId.WorkMovement);

            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.ScheduleMismatch));
            Assert.That(gateway.Revision, Is.Zero);
        }

        [Test]
        public void ControlledDoor_RequiresConfiguredCredential()
        {
            Assert.Throws<System.ArgumentException>(() => new DoorState("route", DoorAccess.OfficerControlled, true));
            Assert.Throws<System.ArgumentException>(() => new DoorState("staff", DoorAccess.StaffOnly, true));
        }

        [Test]
        public void PublicDoor_CanBeOpenedByBothRoles()
        {
            var inmate = new ActorState("inmate", PlayerRole.Inmate, string.Empty);
            var officer = new ActorState("officer", PlayerRole.Officer, string.Empty);
            var firstDoor = new DoorState("public-one", DoorAccess.Public, false);
            var secondDoor = new DoorState("public-two", DoorAccess.Public, false);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            Assert.That(gateway.Execute(Request(inmate, firstDoor, AuthorityAction.OpenDoor), inmate, firstDoor, SchedulePhaseId.WorkMovement).Accepted, Is.True);
            var currentRequest = new AuthorityRequest(officer.ActorId, AuthorityAction.OpenDoor, secondDoor.DoorId, 485, gateway.Revision);
            Assert.That(gateway.Execute(currentRequest, officer, secondDoor, SchedulePhaseId.WorkMovement).Accepted, Is.True);
            Assert.That(gateway.Revision, Is.EqualTo(2));
        }

        [Test]
        public void StaffDoor_IsOfficerOnly()
        {
            var inmate = new ActorState("inmate", PlayerRole.Inmate, string.Empty);
            var door = new DoorState("staff", DoorAccess.StaffOnly, false, requiredCredentialId: KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            AuthorityDecision result = gateway.Execute(Request(inmate, door, AuthorityAction.OpenDoor), inmate, door, SchedulePhaseId.WorkMovement);

            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.WrongRole));
            Assert.That(gateway.Revision, Is.Zero);
        }

        [Test]
        public void ActorAndTargetMismatch_AreRejectedWithoutMutation()
        {
            var actor = new ActorState("officer", PlayerRole.Officer, MovementPost, new[] { KeyRing });
            var door = new DoorState("route", DoorAccess.OfficerControlled, true, MovementPost, KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            var wrongActor = new AuthorityRequest("someone-else", AuthorityAction.UnlockDoor, door.DoorId, 485, 0);
            Assert.That(gateway.Execute(wrongActor, actor, door, SchedulePhaseId.WorkMovement).Rejection, Is.EqualTo(AuthorityRejection.WrongRole));
            var wrongTarget = new AuthorityRequest(actor.ActorId, AuthorityAction.UnlockDoor, "other-door", 485, 0);
            Assert.That(gateway.Execute(wrongTarget, actor, door, SchedulePhaseId.WorkMovement).Rejection, Is.EqualTo(AuthorityRejection.DoorLocked));
            Assert.That(door.IsLocked, Is.True);
            Assert.That(gateway.Revision, Is.Zero);
        }

        [Test]
        public void OfficerWithWrongAssignment_CannotUnlockDoor()
        {
            var actor = new ActorState("officer", PlayerRole.Officer, "Yard-Post", new[] { KeyRing });
            var door = new DoorState("route", DoorAccess.OfficerControlled, true, MovementPost, KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            AuthorityDecision result = gateway.Execute(Request(actor, door, AuthorityAction.UnlockDoor), actor, door, SchedulePhaseId.WorkMovement);

            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.WrongAssignment));
            Assert.That(door.IsLocked, Is.True);
            Assert.That(gateway.Revision, Is.Zero);
        }

        [Test]
        public void LockedDoor_CannotBeOpened()
        {
            var actor = new ActorState("inmate", PlayerRole.Inmate, string.Empty);
            var door = new DoorState("public", DoorAccess.Public, true);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            AuthorityDecision result = gateway.Execute(Request(actor, door, AuthorityAction.OpenDoor), actor, door, SchedulePhaseId.WorkMovement);

            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.DoorLocked));
            Assert.That(gateway.Revision, Is.Zero);
        }

        [Test]
        public void ValidCredential_CannotBypassLockdownSchedule()
        {
            var actor = new ActorState("officer", PlayerRole.Officer, MovementPost, new[] { KeyRing });
            var door = new DoorState("route", DoorAccess.OfficerControlled, true, MovementPost, KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());

            var request = new AuthorityRequest(actor.ActorId, AuthorityAction.UnlockDoor, door.DoorId, 594, 0);
            AuthorityDecision result = gateway.Execute(request, actor, door, SchedulePhaseId.Lockdown);

            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.OutsideScheduleWindow));
            Assert.That(door.IsLocked, Is.True);
            Assert.That(gateway.Revision, Is.Zero);
        }

        [Test]
        public void StaleRequest_IsRejectedWithoutMutation()
        {
            var actor = new ActorState("officer", PlayerRole.Officer, MovementPost, new[] { KeyRing });
            var firstDoor = new DoorState("first", DoorAccess.OfficerControlled, true, MovementPost, KeyRing);
            var secondDoor = new DoorState("second", DoorAccess.OfficerControlled, true, MovementPost, KeyRing);
            var gateway = new OfflineAuthorityGateway(new DoorActionValidator());
            gateway.Execute(Request(actor, firstDoor, AuthorityAction.UnlockDoor), actor, firstDoor, SchedulePhaseId.WorkMovement);

            var stale = new AuthorityRequest(actor.ActorId, AuthorityAction.UnlockDoor, secondDoor.DoorId, 485, 0);
            AuthorityDecision result = gateway.Execute(stale, actor, secondDoor, SchedulePhaseId.WorkMovement);

            Assert.That(result.Rejection, Is.EqualTo(AuthorityRejection.StaleRevision));
            Assert.That(secondDoor.IsLocked, Is.True);
            Assert.That(gateway.Revision, Is.EqualTo(1));
        }

        private static AuthorityRequest Request(ActorState actor, DoorState door, AuthorityAction action)
        {
            return new AuthorityRequest(actor.ActorId, action, door.DoorId, 485, 0);
        }
    }
}
