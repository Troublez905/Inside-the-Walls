using System;

namespace InsideTheWalls.Simulation
{
    public enum NpcAction { Talk, Shove, Calm }

    // Deliberately transient: this state is not included in the day save snapshot.
    public sealed class NpcEncounterState
    {
        private double readyAt;
        public int Rapport { get; private set; }
        public int Tension { get; private set; }
        public bool TryAct(NpcAction action, double now, bool blocked, bool objectivePriority,
            bool inRange, bool visible, out string response)
        {
            if (blocked || !inRange || !visible || (objectivePriority && action == NpcAction.Talk))
            { response = "Move closer with a clear view to interact."; return false; }
            if (double.IsNaN(now) || double.IsInfinity(now) || now < readyAt)
            { response = "Give them a moment before acting again."; return false; }
            switch (action)
            {
                case NpcAction.Talk:
                    Rapport = Math.Min(5, Rapport + 1);
                    response = Tension > 0 ? "I need some space. Let's keep this calm." : "Morning. Keep to your route and we'll get through the day.";
                    break;
                case NpcAction.Shove:
                    Tension = Math.Min(5, Tension + 2);
                    Rapport = Math.Max(-5, Rapport - 2);
                    response = "Step back. We can talk without pushing.";
                    break;
                case NpcAction.Calm:
                    Tension = Math.Max(0, Tension - 2);
                    Rapport = Math.Min(5, Rapport + 1);
                    response = "Okay. Let's give each other space.";
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(action));
            }
            readyAt = now + 1.2;
            return true;
        }
    }
}
