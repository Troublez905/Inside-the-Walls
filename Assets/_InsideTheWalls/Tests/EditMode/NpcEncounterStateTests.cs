#if ITW_TESTS
using InsideTheWalls.Simulation;
using NUnit.Framework;

namespace InsideTheWalls.Tests
{
    public sealed class NpcEncounterStateTests
    {
        [Test]
        public void BlockedOrOccludedActionsDoNotMutateState()
        {
            var state = new NpcEncounterState();
            Assert.IsFalse(state.TryAct(NpcAction.Shove, 10, true, false, true, true, out _));
            Assert.IsFalse(state.TryAct(NpcAction.Shove, 10, false, false, true, false, out _));
            Assert.IsFalse(state.TryAct(NpcAction.Shove, 10, false, false, false, true, out _));
            Assert.AreEqual(0, state.Tension);
        }
        [Test]
        public void ObjectiveOwnsTalkAndDoesNotConsumeCooldown()
        {
            var state = new NpcEncounterState();
            Assert.IsFalse(state.TryAct(NpcAction.Talk, 10, false, true, true, true, out _));
            Assert.IsTrue(state.TryAct(NpcAction.Talk, 10, false, false, true, true, out _));
        }
        [Test]
        public void ShoveCooldownAndCalmHaveBoundedConsequences()
        {
            var state = new NpcEncounterState();
            Assert.IsTrue(state.TryAct(NpcAction.Shove, 10, false, false, true, true, out _));
            Assert.AreEqual(2, state.Tension);
            Assert.IsFalse(state.TryAct(NpcAction.Shove, 10.1, false, false, true, true, out _));
            Assert.AreEqual(2, state.Tension);
            Assert.IsTrue(state.TryAct(NpcAction.Calm, 12, false, false, true, true, out _));
            Assert.AreEqual(0, state.Tension);
            for (int i = 0; i < 20; i++) state.TryAct(NpcAction.Shove, 14 + i * 2, false, false, true, true, out _);
            Assert.AreEqual(5, state.Tension); Assert.AreEqual(-5, state.Rapport);
        }
    }
}
#endif
