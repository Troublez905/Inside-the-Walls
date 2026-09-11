using System.Collections.Generic;
using InsideTheWalls.Simulation;
using UnityEngine;

namespace InsideTheWalls.Characters
{
    public sealed class PrisonPopulation : MonoBehaviour
    {
        private readonly List<PrisonNpc> people = new List<PrisonNpc>();
        private readonly List<Material> materials = new List<Material>();
        private PrototypePlayerController player;
        private CharacterInteractionAnimator playerAnimation;
        private bool playerOfficer;
        private bool inputIsBlocked = true;
        private bool objectivePriority;
        private float messageUntil = 6f;
        public IReadOnlyList<PrisonNpc> Npcs => people;
        public string ProximityPrompt { get; private set; } = "";
        public string StatusMessage { get; private set; } = "NPC rapport and tension last for this session only; not saved.";
        public bool IsGuarding { get; private set; }

        public void Configure(PrototypePlayerController target, bool playerIsOfficer)
        {
            if (player != null) { Debug.LogError("Population is already configured.", this); return; }
            player = target; playerOfficer = playerIsOfficer;
            playerAnimation = player.GetComponent<CharacterInteractionAnimator>();
            messageUntil = Time.time + 6f;
            Material inmate = Surface("Inmate uniform", new Color(.63f,.35f,.16f));
            Material officer = Surface("Officer uniform", new Color(.12f,.22f,.31f));
            Material skin = Surface("Skin", new Color(.60f,.40f,.28f));
            Material shoes = Surface("Shoes and hair", new Color(.09f,.08f,.07f));
            Spawn("Eli", false, new Vector3(-13,.12f,4), new Vector3(-13,.12f,6), inmate, skin, shoes, "Characters/InmateTwoPlayable");
            Spawn("Mateo", false, new Vector3(-2,.12f,-2.5f), new Vector3(-3.5f,.12f,-2.5f), inmate, skin, shoes, "Characters/InmateThreePlayable");
            Spawn("Sam", false, new Vector3(15.7f,.12f,7.1f), new Vector3(14,.12f,7.1f), inmate, skin, shoes, "Characters/InmateThreePlayable");
            Spawn("Officer Reed", true, new Vector3(6,.12f,-1.7f), new Vector3(8,.12f,-1.7f), officer, skin, shoes, "Characters/OfficerLenaPlayable");
            Spawn("Officer Chen", true, new Vector3(11,.12f,-9), new Vector3(13,.12f,-9), officer, skin, shoes, "Characters/OfficerLenaPlayable");
        }

        public void Tick(bool inputBlocked, bool objectiveHasPriority)
        {
            ProximityPrompt = ""; IsGuarding = false;
            if (Time.time > messageUntil) StatusMessage = "";
            if (player == null) return;
            bool blocked = inputBlocked || player.MovementSuppressed;
            inputIsBlocked = blocked;
            objectivePriority = objectiveHasPriority;
            PrisonNpc nearest = null; float best = 2.2f;
            foreach (PrisonNpc person in people)
            {
                float distance = Vector3.Distance(person.transform.position, player.transform.position);
                person.Tick(blocked, distance < 2.6f);
                if (distance < best && Visible(person)) { nearest = person; best = distance; }
            }
            foreach (PrisonNpc person in people) person.SetHighlighted(!blocked && person == nearest);
            if (blocked)
            {
                if (playerAnimation != null) playerAnimation.SetGuard(false);
                return;
            }
            IsGuarding = Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.JoystickButton4);
            if (playerAnimation != null) playerAnimation.SetGuard(IsGuarding);
            if (nearest == null) return;
            ProximityPrompt = nearest.DisplayName + (objectiveHasPriority ? "" : "  |  E / A: TALK") + "  |  R / Y: CALM";
            NpcAction? action = null;
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton3)) action = NpcAction.Calm;
            else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton2)) action = NpcAction.Shove;
            else if (player.InteractionPressed && !objectiveHasPriority) action = NpcAction.Talk;
            if (!action.HasValue) return;
            TryAction(nearest, action.Value);
        }

        public bool TryTalk(PrisonNpc person) => TryAction(person, NpcAction.Talk);
        public bool TryCalm(PrisonNpc person) => TryAction(person, NpcAction.Calm);
        public bool TryShove(PrisonNpc person) => TryAction(person, NpcAction.Shove);

        private bool TryAction(PrisonNpc person, NpcAction action)
        {
            return TryAct(person, action, out _);
        }

        public bool TryAct(PrisonNpc person, NpcAction action, out string response)
        {
            messageUntil = Time.time + 6f;
            if (player == null || person == null || !people.Contains(person))
            { response = StatusMessage = "No available conversation partner."; return false; }
            if (IsGuarding && action == NpcAction.Shove)
            { response = StatusMessage = "Guard held: release G / LB before shoving. R / Y to calm."; return false; }
            bool accepted = person.Encounter.TryAct(action, Time.timeAsDouble,
                inputIsBlocked || player.MovementSuppressed || !isActiveAndEnabled, objectivePriority,
                Vector3.Distance(person.transform.position, player.transform.position) < 2.2f,
                Visible(person), out string reply);
            if (accepted)
            {
                person.React(player.transform, action == NpcAction.Shove);
                if (action != NpcAction.Shove)
                {
                    person.Gesture(action == NpcAction.Calm);
                    // The supplied arguing gesture is used for conversation, not mislabeled as calming.
                    if (action == NpcAction.Talk && playerAnimation != null) playerAnimation.PlayTalk();
                }
                if (action == NpcAction.Talk && person.Encounter.Tension == 0) reply = person.GreetingFor(playerOfficer);
            }
            string accountability = accepted && action == NpcAction.Shove && playerOfficer ? " As an officer, use de-escalation." : "";
            StatusMessage = person.DisplayName + ": " + reply + accountability + "  Rapport " + person.Encounter.Rapport + " / tension " + person.Encounter.Tension + " (not saved)";
            response = StatusMessage;
            return accepted;
        }

        private bool Visible(PrisonNpc person)
        {
            Vector3 origin = player.transform.position + Vector3.up * .65f;
            Vector3 delta = person.transform.position + Vector3.up * 1.2f - origin;
            foreach (RaycastHit hit in Physics.RaycastAll(origin, delta.normalized, delta.magnitude, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform == player.transform || hit.transform.IsChildOf(player.transform)
                    || hit.transform == person.transform || hit.transform.IsChildOf(person.transform)) continue;
                return false;
            }
            return true;
        }

        private void Spawn(string title, bool officer, Vector3 start, Vector3 end, Material uniform, Material skin, Material shoes)
        {
            Spawn(title, officer, start, end, uniform, skin, shoes, null);
        }

        private void Spawn(string title, bool officer, Vector3 start, Vector3 end, Material uniform, Material skin, Material shoes, string visualResource)
        {
            GameObject actor = new GameObject(title); actor.transform.SetParent(transform, false);
            PrisonNpc npc = actor.AddComponent<PrisonNpc>();
            npc.Configure(title, officer, start, end, uniform, skin, shoes, visualResource);
            people.Add(npc);
        }

        private Material Surface(string title, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new System.InvalidOperationException("NPCs require the project's URP Lit shader.");
            Material material = new Material(shader) { name = title }; material.SetColor("_BaseColor", color);
            materials.Add(material); return material;
        }

        private void OnDisable()
        {
            inputIsBlocked = true; IsGuarding = false; ProximityPrompt = "";
            if (playerAnimation != null) playerAnimation.SetGuard(false);
        }
        private void OnDestroy() { foreach (Material material in materials) if (material != null) Destroy(material); }
    }
}
