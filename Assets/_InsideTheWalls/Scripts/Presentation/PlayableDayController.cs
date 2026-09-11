using System;
using System.Collections.Generic;
using InsideTheWalls.Characters;
using InsideTheWalls.Persistence;
using InsideTheWalls.Simulation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InsideTheWalls.Presentation
{
    public sealed class PlayableDayController : MonoBehaviour
    {
        private readonly struct Objective
        {
            public Objective(string title, string location, Vector3 position, int minute, string prompt, string result, DoorMoment doorMoment = DoorMoment.None)
            {
                Title = title;
                Location = location;
                Position = position;
                Minute = minute;
                Prompt = prompt;
                Result = result;
                DoorMoment = doorMoment;
            }

            public string Title { get; }
            public string Location { get; }
            public Vector3 Position { get; }
            public int Minute { get; }
            public string Prompt { get; }
            public string Result { get; }
            public DoorMoment DoorMoment { get; }
        }

        private enum DoorMoment
        {
            None,
            OpenLaundry,
            UnlockMovementGate
        }

        private const float InteractionRange = 2.6f;
        private readonly List<GameObject> markers = new List<GameObject>();
        private DaySchedule schedule;
        private Objective[] objectives;
        private PrototypePlayerController player;
        private PlayerRole role;
        private int objectiveIndex;
        private ActorState actor;
        private DoorState laundryDoor;
        private DoorState movementGate;
        private OfflineAuthorityGateway authority;
        private bool awaitingChoice;
        private bool choiceAxisArmed = true;
        private int selectedChoice;
        private string consequencePreview;
        private string choiceId;
        private Sprite objectiveMarkerSprite;
        private PrisonPopulation population;
        private bool enablePopulation;

        public string ObjectiveTitle => IsComplete ? "LOCKDOWN COMPLETE" : objectives[objectiveIndex].Title;
        public string LocationLabel => IsComplete ? "HOUSING A" : objectives[objectiveIndex].Location;
        public string StatusMessage { get; private set; }
        public string ProximityPrompt { get; private set; }
        public bool IsComplete { get; private set; }
        public string Summary { get; private set; }
        public string ProgressLabel => $"OBJECTIVE {Math.Min(objectiveIndex + 1, objectives.Length)} / {objectives.Length}";
        public int ObjectiveIndex => objectiveIndex;
        public PlayerRole Role => role;
        public PrisonPopulation Population => population;
        public string PopulationMessage => population != null ? population.StatusMessage : string.Empty;
        public bool IsGuarding => population != null && population.IsGuarding;

        public PlayableDaySnapshot CreateSnapshot()
        {
            return PlayableDaySnapshot.Create(role, objectiveIndex, choiceId, consequencePreview);
        }

        public string TimeLabel
        {
            get
            {
                int minute = IsComplete ? 590 : objectives[objectiveIndex].Minute;
                return $"{minute / 60:00}:{minute % 60:00}";
            }
        }

        public void Configure(PlayerRole selectedRole, bool includePopulation = true)
        {
            role = selectedRole;
            enablePopulation = includePopulation;
            schedule = DaySchedule.CreateFoundationDay();
            objectives = role == PlayerRole.Inmate ? CreateInmateObjectives() : CreateOfficerObjectives();
            actor = role == PlayerRole.Inmate
                ? new ActorState("local-inmate", role, "Laundry-Worker")
                : new ActorState("local-officer", role, "UnitA-MovementPost", new[] { "UnitA-KeyRing" });
            laundryDoor = new DoorState("laundry-door", DoorAccess.AssignmentRestricted, false, "Laundry-Worker");
            movementGate = new DoorState("movement-gate", DoorAccess.OfficerControlled, true, "UnitA-MovementPost", "UnitA-KeyRing");
            authority = new OfflineAuthorityGateway(new DoorActionValidator());
            StatusMessage = role == PlayerRole.Inmate
                ? "Start at intake. Your housing and laundry assignments are waiting."
                : "Report for briefing. Your post controls the morning movement gate.";
            BuildWorld();
            RefreshMarkers();
        }

        public bool Restore(PlayableDaySnapshot snapshot)
        {
            if (snapshot == null || !snapshot.TryGetRole(out PlayerRole savedRole) || savedRole != role
                || snapshot.objectiveIndex < 0 || snapshot.objectiveIndex > objectives.Length)
            {
                return false;
            }

            objectiveIndex = snapshot.objectiveIndex;
            choiceId = snapshot.choiceId ?? string.Empty;
            consequencePreview = snapshot.consequencePreview ?? string.Empty;
            if (!ApplyProgressedAuthority())
            {
                return false;
            }

            StatusMessage = objectiveIndex == 0
                ? StatusMessage
                : "Saved session restored. Report to your highlighted objective.";
            if (objectiveIndex >= objectives.Length) CompleteDay();
            else
            {
                PlacePlayerNearCurrentObjective();
                RefreshMarkers();
            }

            return true;
        }

        private bool ApplyProgressedAuthority()
        {
            for (int i = 0; i < objectiveIndex && i < objectives.Length; i++)
            {
                Objective completed = objectives[i];
                if (completed.DoorMoment == DoorMoment.None) continue;
                if (!ResolveDoorMoment(completed)) return false;
            }

            return true;
        }

        private void PlacePlayerNearCurrentObjective()
        {
            if (player == null || objectiveIndex < 0 || objectiveIndex >= objectives.Length) return;

            Vector3 target = objectives[objectiveIndex].Position;
            Vector3 spawn = target + new Vector3(0f, 0f, -2.4f);
            spawn.y = 1f;
            Vector3 facing = target - spawn;
            float yaw = facing.sqrMagnitude > 0.01f
                ? Quaternion.LookRotation(new Vector3(facing.x, 0f, facing.z)).eulerAngles.y
                : 0f;
            player.TeleportTo(spawn, yaw);
        }

        public void Tick()
        {
            if (player == null) return;
            float distance = IsComplete ? float.PositiveInfinity
                : Vector3.Distance(player.transform.position, objectives[objectiveIndex].Position);
            bool objectiveHasPriority = distance <= InteractionRange;
            if (population != null)
                population.Tick(awaitingChoice || IsComplete || player.MovementSuppressed, objectiveHasPriority);
            if (IsComplete) return;

            if (awaitingChoice)
            {
                TickChoice();
                return;
            }

            Objective objective = objectives[objectiveIndex];
            ProximityPrompt = distance <= InteractionRange
                ? $"E / A  {objective.Prompt}"
                : population != null && !string.IsNullOrEmpty(population.ProximityPrompt)
                    ? population.ProximityPrompt
                    : $"{distance:0}m TO {objective.Location}";

            if (!player.InteractionPressed || distance > InteractionRange) return;

            if (IsChoiceObjective())
            {
                awaitingChoice = true;
                player.SetMovementSuppressed(true);
                selectedChoice = 0;
                UpdateChoicePrompt();
                return;
            }

            if (!ResolveDoorMoment(objective)) return;

            StatusMessage = objective.Result;
            objectiveIndex++;
            if (objectiveIndex >= objectives.Length)
            {
                CompleteDay();
                return;
            }

            RefreshMarkers();
        }

        private void TickChoice()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(horizontal) < 0.3f) choiceAxisArmed = true;
            bool moveChoice = choiceAxisArmed && Mathf.Abs(horizontal) > 0.7f;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || moveChoice)
            {
                selectedChoice = 1 - selectedChoice;
                choiceAxisArmed = false;
                UpdateChoicePrompt();
            }

            if (!player.InteractionPressed) return;

            awaitingChoice = false;
            player.SetMovementSuppressed(false);
            if (role == PlayerRole.Inmate)
            {
                if (selectedChoice == 0)
                {
                    choiceId = "deliver-message";
                    StatusMessage = "You deliver the permitted message. A receipt supports your account, but you arrive late.";
                    consequencePreview = "Preview: Noah may trust you more; laundry may begin with a lateness review.";
                }
                else
                {
                    choiceId = "keep-work-route";
                    StatusMessage = "You keep to the direct route. Laundry starts on time, but Noah sees the promise go unkept.";
                    consequencePreview = "Preview: your work record may benefit; Noah may be less willing to trust you.";
                }
            }
            else
            {
                if (selectedChoice == 0)
                {
                    choiceId = "document-incident";
                    StatusMessage = "You document the observed lateness and attach the inmate's explanation for review.";
                    consequencePreview = "Preview: Ward may value accuracy; the inmate may face a counseling review.";
                }
                else
                {
                    choiceId = "verify-use-discretion";
                    StatusMessage = "You verify the permitted delivery and use discretion, recording a redirect without an incident.";
                    consequencePreview = "Preview: rapport may improve; Ward may ask you to justify the exception.";
                }
            }

            objectiveIndex++;
            if (objectiveIndex >= objectives.Length) CompleteDay();
            else RefreshMarkers();
        }

        private bool IsChoiceObjective()
        {
            return (role == PlayerRole.Inmate && objectiveIndex == 3)
                || (role == PlayerRole.Officer && objectiveIndex == 4);
        }

        private void UpdateChoicePrompt()
        {
            string first = role == PlayerRole.Inmate ? "DELIVER MESSAGE" : "DOCUMENT INCIDENT";
            string second = role == PlayerRole.Inmate ? "KEEP TO WORK ROUTE" : "VERIFY AND USE DISCRETION";
            string left = selectedChoice == 0 ? $"[{first}]" : first;
            string right = selectedChoice == 1 ? $"[{second}]" : second;
            ProximityPrompt = $"LEFT/RIGHT: {left}  /  {right}    E / A: CONFIRM";
        }

        private bool ResolveDoorMoment(Objective objective)
        {
            DoorState door;
            AuthorityAction action;
            if (objective.DoorMoment == DoorMoment.OpenLaundry)
            {
                door = laundryDoor;
                action = AuthorityAction.OpenDoor;
            }
            else if (objective.DoorMoment == DoorMoment.UnlockMovementGate)
            {
                door = movementGate;
                action = AuthorityAction.UnlockDoor;
            }
            else
            {
                return true;
            }

            SchedulePhaseId phase = schedule.PhaseAt(objective.Minute).Id;
            var request = new AuthorityRequest(actor.ActorId, action, door.DoorId, objective.Minute, authority.Revision);
            AuthorityDecision decision = authority.Execute(request, actor, door, phase);
            if (decision.Accepted) return true;

            StatusMessage = $"ACCESS DENIED: {DescribeRejection(decision.Rejection)}. Check your assignment and return during movement.";
            return false;
        }

        private void CompleteDay()
        {
            if (player != null) player.SetMovementSuppressed(false);
            IsComplete = true;
            ProximityPrompt = string.Empty;
            StatusMessage = "Final count reconciled. The unit settles into lockdown.";
            Summary = role == PlayerRole.Inmate
                ? $"Laundry assignment and final count completed. {consequencePreview}\n\nYour completed day is saved locally."
                : $"Movement post and final count completed. {consequencePreview}\n\nYour completed day is saved locally.";
            RefreshMarkers();
        }

        private void BuildWorld()
        {
            foreach (GameObject existing in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (existing != gameObject) Destroy(existing);
            }

            RenderSettings.ambientLight = new Color(0.38f, 0.43f, 0.42f);
            var lightObject = new GameObject("Morning Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            light.color = new Color(1f, 0.88f, 0.7f);
            lightObject.transform.rotation = Quaternion.Euler(42f, -34f, 0f);

            FacilityVisualBuilder.Build();

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.12f, 0.14f);
            camera.fieldOfView = 62f;

            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObject.name = role == PlayerRole.Inmate ? "Inmate Player" : "Officer Player";
            playerObject.transform.position = new Vector3(-8f, 1f, -11f);
            Destroy(playerObject.GetComponent<CapsuleCollider>());
            playerObject.AddComponent<CharacterController>();
            Renderer capsuleRenderer = playerObject.GetComponent<Renderer>();
            ApplyUrpMaterial(capsuleRenderer, role == PlayerRole.Inmate
                ? new Color(0.82f, 0.34f, 0.08f)
                : new Color(0.15f, 0.28f, 0.38f));
            if (role == PlayerRole.Inmate && TryAttachInmateVisual(playerObject))
            {
                capsuleRenderer.enabled = false;
            }
            player = playerObject.AddComponent<PrototypePlayerController>();

            Texture2D markerTexture = Resources.Load<Texture2D>("UI/ObjectiveMarkerActive");
            if (markerTexture != null)
            {
                objectiveMarkerSprite = Sprite.Create(
                    markerTexture,
                    new Rect(0f, 0f, markerTexture.width, markerTexture.height),
                    new Vector2(0.5f, 0.5f),
                    markerTexture.width);
            }

            for (int i = 0; i < objectives.Length; i++)
            {
                markers.Add(CreateObjectiveMarker($"Objective {i + 1}: {objectives[i].Title}", objectives[i].Position));
            }
            if (enablePopulation)
            {
                population = gameObject.AddComponent<PrisonPopulation>();
                population.Configure(player, role == PlayerRole.Officer);
            }
        }

        private static bool TryAttachInmateVisual(GameObject playerObject)
        {
            if (TryAttachCharacter(playerObject, "Characters/InmateTwoPlayable", "Inmate #2 Visual")) return true;
            Debug.LogWarning("Inmate #2 is not ready; using the preserved Noah character while its rig is prepared.");
            return TryAttachCharacter(playerObject, "Characters/NoahMercerPlayable", "Noah Mercer Visual");
        }

        private static bool TryAttachCharacter(GameObject playerObject, string resourcePath, string visualName)
        {
            GameObject characterPrefab = Resources.Load<GameObject>(resourcePath);
            if (characterPrefab == null)
            {
                return false;
            }

            Animator animator = characterPrefab.GetComponentInChildren<Animator>();
            if (animator == null || !animator.enabled || animator.avatar == null
                || !animator.avatar.isValid || !animator.avatar.isHuman
                || animator.runtimeAnimatorController == null)
            {
                Debug.LogWarning($"{resourcePath} requires an enabled Animator, a valid Humanoid Avatar, and a runtime controller.");
                return false;
            }

            bool hasSkinnedMesh = false;
            foreach (SkinnedMeshRenderer renderer in characterPrefab.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (renderer.enabled && renderer.sharedMesh != null && renderer.bones.Length > 0)
                {
                    hasSkinnedMesh = true;
                    break;
                }
            }
            if (!hasSkinnedMesh)
            {
                Debug.LogWarning($"{resourcePath} has no enabled, rigged skinned mesh.");
                return false;
            }

            GameObject character = Instantiate(characterPrefab, playerObject.transform);
            character.name = visualName;
            CharacterController body = playerObject.GetComponent<CharacterController>();
            Vector3 feetPosition = body.center - Vector3.up * (body.height * 0.5f);
            character.transform.SetLocalPositionAndRotation(feetPosition, Quaternion.identity);
            Debug.Log("PLAYER_CHARACTER_READY " + visualName);
            return true;
        }

        private GameObject CreateObjectiveMarker(string name, Vector3 position)
        {
            if (objectiveMarkerSprite != null)
            {
                var spriteMarker = new GameObject(name);
                spriteMarker.transform.position = position + Vector3.up * 1.15f;
                spriteMarker.transform.localScale = Vector3.one * 1.35f;
                var spriteRenderer = spriteMarker.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = objectiveMarkerSprite;
                spriteRenderer.sortingOrder = 20;
                spriteMarker.AddComponent<ObjectiveMarkerBillboard>();
                return spriteMarker;
            }

            var root = new GameObject(name);
            root.transform.SetPositionAndRotation(position, Quaternion.identity);

            GameObject core = CreateBlock($"{name} Core", position, new Vector3(0.55f, 1.15f, 0.55f), new Color(0.9f, 0.38f, 0.06f));
            core.transform.SetParent(root.transform, true);

            CreateMarkerBracket($"{name} Bracket NW", position + new Vector3(-0.55f, 0.85f, -0.55f), new Vector3(0.28f, 0.06f, 0.06f), root.transform);
            CreateMarkerBracket($"{name} Bracket NE", position + new Vector3(0.55f, 0.85f, -0.55f), new Vector3(0.28f, 0.06f, 0.06f), root.transform);
            CreateMarkerBracket($"{name} Bracket SW", position + new Vector3(-0.55f, 0.85f, 0.55f), new Vector3(0.28f, 0.06f, 0.06f), root.transform);
            CreateMarkerBracket($"{name} Bracket SE", position + new Vector3(0.55f, 0.85f, 0.55f), new Vector3(0.28f, 0.06f, 0.06f), root.transform);
            CreateMarkerBracket($"{name} Ring", position + new Vector3(0f, 1.35f, 0f), new Vector3(0.95f, 0.06f, 0.95f), root.transform);

            var collider = root.AddComponent<BoxCollider>();
            collider.center = new Vector3(0f, 0.6f, 0f);
            collider.size = new Vector3(1.1f, 1.4f, 1.1f);
            return root;
        }

        private static void CreateMarkerBracket(string name, Vector3 position, Vector3 scale, Transform parent)
        {
            GameObject bracket = CreateBlock(name, position, scale, new Color(0.95f, 0.55f, 0.12f));
            bracket.transform.SetParent(parent, true);
            UnityEngine.Object.Destroy(bracket.GetComponent<Collider>());
        }

        private void CreatePerimeter()
        {
            Color fence = new Color(0.16f, 0.19f, 0.19f);
            CreateBlock("Fence North", new Vector3(0f, 2f, 15f), new Vector3(40f, 4f, 0.3f), fence);
            CreateBlock("Fence South", new Vector3(0f, 2f, -15f), new Vector3(40f, 4f, 0.3f), fence);
            CreateBlock("Fence East", new Vector3(20f, 2f, 0f), new Vector3(0.3f, 4f, 30f), fence);
            CreateBlock("Fence West", new Vector3(-20f, 2f, 0f), new Vector3(0.3f, 4f, 30f), fence);
        }

        private static void CreateLocation(string name, Vector3 position, Color color)
        {
            CreateBlock(name, position, new Vector3(5f, 0.2f, 4f), color);
        }

        private void RefreshMarkers()
        {
            for (int i = 0; i < markers.Count; i++) markers[i].SetActive(!IsComplete && i == objectiveIndex);
        }

        private static GameObject CreateBlock(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetPositionAndRotation(position, Quaternion.identity);
            block.transform.localScale = scale;
            ApplyUrpMaterial(block.GetComponent<Renderer>(), color);
            return block;
        }

        private static void ApplyUrpMaterial(Renderer target, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Sprites/Default");
            if (shader == null)
            {
                Debug.LogError("No supported runtime shader is available for the gray-box material.");
                return;
            }

            var material = new Material(shader)
            {
                name = $"Graybox {ColorUtility.ToHtmlStringRGB(color)}"
            };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            target.sharedMaterial = material;
        }

        private static Objective[] CreateInmateObjectives()
        {
            return new[]
            {
                new Objective("RECEIVE ASSIGNMENTS", "INTAKE", new Vector3(-14f, 0.75f, -9f), 442, "CHECK IN AT PROPERTY DESK", "Housing A, bunk 04. Laundry detail. The schedule is now yours to keep."),
                new Objective("MORNING COUNT", "HOUSING A", new Vector3(-13f, 0.75f, 7f), 452, "REPORT PRESENT FOR COUNT", "Count clears. Movement opens toward dining."),
                new Objective("KEEP A SMALL PROMISE", "DINING", new Vector3(-4f, 0.75f, 9f), 468, "SAVE MARCUS A SEAT", "Marcus notices. A small promise costs a little time."),
                new Objective("THE MISSING TEN MINUTES", "YARD WALK", new Vector3(0f, 0.75f, -3f), 486, "DECIDE HOW TO RESPOND TO NOAH", "Noah's request forces a choice between a promise and the clock."),
                new Objective("REPORT TO LAUNDRY", "LAUNDRY", new Vector3(13f, 0.75f, 8f), 512, "PRESENT WORK ASSIGNMENT", "Access validated. Six laundry bags sorted; your arrival time is recorded.", DoorMoment.OpenLaundry),
                new Objective("GIVE YOUR ACCOUNT", "OFFICER STATION", new Vector3(13f, 0.75f, -7f), 582, "STATE WHAT HAPPENED", "Your account is attached to the available timestamps and records."),
                new Objective("RETURN FOR LOCKDOWN", "HOUSING A", new Vector3(-13f, 0.75f, 7f), 594, "CONFIRM FINAL COUNT", "Lockdown confirmed. A next-day preview reflects today's choices.")
            };
        }

        private static Objective[] CreateOfficerObjectives()
        {
            return new[]
            {
                new Objective("SHIFT BRIEFING", "OFFICER STATION", new Vector3(13f, 0.75f, -7f), 442, "RECEIVE POST AND KEY RING", "Unit A movement post assigned. Observe first; record only what you know."),
                new Objective("VERIFY MORNING COUNT", "HOUSING A", new Vector3(-13f, 0.75f, 7f), 452, "CONFIRM HOUSING COUNT", "Count reconciled. Meal movement may begin."),
                new Objective("OPEN WORK MOVEMENT", "MOVEMENT GATE", new Vector3(3f, 0.75f, 1f), 484, "VALIDATE AND UNLOCK GATE", "Authority accepted your assignment, credential, schedule, and request revision.", DoorMoment.UnlockMovementGate),
                new Objective("PROCESS MOVEMENT REQUESTS", "OFFICER STATION", new Vector3(13f, 0.75f, -7f), 520, "REVIEW THREE DESTINATIONS", "Two requests approved. One wrong assignment redirected without punishment."),
                new Objective("INTERVIEW, DON'T ASSUME", "YARD", new Vector3(0f, 0.75f, -3f), 560, "ASK ABOUT THE LATE ARRIVAL", "The inmate reports a permitted delivery. You record it as a statement, not a fact."),
                new Objective("RECORD YOUR DECISION", "INTAKE DESK", new Vector3(-14f, 0.75f, -9f), 582, "SUBMIT OBSERVATIONS AND OUTCOME", "Timestamps, your observation, and the chosen disposition enter the shift record."),
                new Objective("SECURE THE UNIT", "HOUSING A", new Vector3(-13f, 0.75f, 7f), 594, "CONFIRM COUNT AND LOCKDOWN", "The unit is secure. Your measured response is part of tomorrow's briefing.")
            };
        }

        private static string DescribeRejection(AuthorityRejection rejection)
        {
            return rejection switch
            {
                AuthorityRejection.WrongRole => "role not authorized",
                AuthorityRejection.WrongAssignment => "wrong assignment",
                AuthorityRejection.OutsideScheduleWindow => "outside schedule window",
                AuthorityRejection.MissingCredential => "required credential missing",
                AuthorityRejection.StaleRevision => "request state changed",
                AuthorityRejection.ScheduleMismatch => "schedule state mismatch",
                AuthorityRejection.DoorLocked => "door remains locked",
                _ => "request rejected"
            };
        }
    }
}
