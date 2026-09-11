using System;
using System.Collections.Generic;

namespace InsideTheWalls.Simulation
{
    public enum SchedulePhaseId
    {
        Onboarding,
        MorningCount,
        MealMovement,
        WorkMovement,
        WorkAndPostDuty,
        Yard,
        Review,
        Lockdown
    }

    public readonly struct SchedulePhase
    {
        public SchedulePhase(SchedulePhaseId id, int startsAtMinute, int endsAtMinute)
        {
            if (startsAtMinute < 0 || endsAtMinute <= startsAtMinute)
            {
                throw new ArgumentOutOfRangeException(nameof(startsAtMinute), "A phase requires a non-empty, forward-moving time range.");
            }

            Id = id;
            StartsAtMinute = startsAtMinute;
            EndsAtMinute = endsAtMinute;
        }

        public SchedulePhaseId Id { get; }
        public int StartsAtMinute { get; }
        public int EndsAtMinute { get; }
        public bool Contains(int minute) => minute >= StartsAtMinute && minute < EndsAtMinute;
    }

    public sealed class DaySchedule
    {
        private readonly SchedulePhase[] phases;

        public DaySchedule(IEnumerable<SchedulePhase> phases)
        {
            if (phases == null) throw new ArgumentNullException(nameof(phases));
            this.phases = new List<SchedulePhase>(phases).ToArray();
            if (this.phases.Length == 0) throw new ArgumentException("A day schedule requires at least one phase.", nameof(phases));

            for (int i = 1; i < this.phases.Length; i++)
            {
                if (this.phases[i - 1].EndsAtMinute != this.phases[i].StartsAtMinute)
                {
                    throw new ArgumentException("Schedule phases must be ordered and contiguous.", nameof(phases));
                }
            }
        }

        public IReadOnlyList<SchedulePhase> Phases => phases;

        public SchedulePhase PhaseAt(int minute)
        {
            for (int i = 0; i < phases.Length; i++)
            {
                if (phases[i].Contains(minute)) return phases[i];
            }

            throw new ArgumentOutOfRangeException(nameof(minute), minute, "Minute falls outside the playable day.");
        }

        public static DaySchedule CreateFoundationDay()
        {
            return new DaySchedule(new[]
            {
                new SchedulePhase(SchedulePhaseId.Onboarding, 440, 450),
                new SchedulePhase(SchedulePhaseId.MorningCount, 450, 460),
                new SchedulePhase(SchedulePhaseId.MealMovement, 460, 480),
                new SchedulePhase(SchedulePhaseId.WorkMovement, 480, 500),
                new SchedulePhase(SchedulePhaseId.WorkAndPostDuty, 500, 550),
                new SchedulePhase(SchedulePhaseId.Yard, 550, 575),
                new SchedulePhase(SchedulePhaseId.Review, 575, 590),
                new SchedulePhase(SchedulePhaseId.Lockdown, 590, 600)
            });
        }
    }
}
