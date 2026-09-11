using System;
using NUnit.Framework;

namespace InsideTheWalls.Simulation.Tests
{
    public sealed class ScheduleTests
    {
        [TestCase(440, SchedulePhaseId.Onboarding)]
        [TestCase(449, SchedulePhaseId.Onboarding)]
        [TestCase(450, SchedulePhaseId.MorningCount)]
        [TestCase(459, SchedulePhaseId.MorningCount)]
        [TestCase(460, SchedulePhaseId.MealMovement)]
        [TestCase(479, SchedulePhaseId.MealMovement)]
        [TestCase(480, SchedulePhaseId.WorkMovement)]
        [TestCase(499, SchedulePhaseId.WorkMovement)]
        [TestCase(500, SchedulePhaseId.WorkAndPostDuty)]
        [TestCase(549, SchedulePhaseId.WorkAndPostDuty)]
        [TestCase(550, SchedulePhaseId.Yard)]
        [TestCase(574, SchedulePhaseId.Yard)]
        [TestCase(575, SchedulePhaseId.Review)]
        [TestCase(589, SchedulePhaseId.Review)]
        [TestCase(590, SchedulePhaseId.Lockdown)]
        [TestCase(599, SchedulePhaseId.Lockdown)]
        public void FoundationDay_MapsEveryBoundaryMinuteToOnePhase(int minute, SchedulePhaseId expected)
        {
            Assert.That(DaySchedule.CreateFoundationDay().PhaseAt(minute).Id, Is.EqualTo(expected));
        }

        [TestCase(439)]
        [TestCase(600)]
        public void FoundationDay_RejectsMinutesOutsidePlayableRange(int minute)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DaySchedule.CreateFoundationDay().PhaseAt(minute));
        }

        [Test]
        public void Constructor_RejectsScheduleGaps()
        {
            Assert.Throws<ArgumentException>(() => new DaySchedule(new[]
            {
                new SchedulePhase(SchedulePhaseId.Onboarding, 440, 450),
                new SchedulePhase(SchedulePhaseId.MorningCount, 451, 460)
            }));
        }
    }
}
