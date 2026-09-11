using InsideTheWalls.Simulation;
using UnityEngine;

namespace InsideTheWalls.Characters
{
    public sealed class PrisonNpc : MonoBehaviour
    {
        private CharacterController body;
        private Transform leftArm, rightArm, leftLeg, rightLeg, label;
        private Vector3 start, end;
        private bool returning;
        private float pauseUntil, recoilUntil;
        private float gestureUntil;
        private bool calmingGesture;
        private GameObject interactionHalo;
        private CharacterInteractionAnimator animationDriver;
        private static readonly int Speed = Animator.StringToHash("Speed");
        public string DisplayName { get; private set; }
        public bool Officer { get; private set; }
        public NpcEncounterState Encounter { get; } = new NpcEncounterState();

        public void Configure(string displayName, bool officer, Vector3 position, Vector3 destination,
            Material uniform, Material skin, Material shoes)
        {
            Configure(displayName, officer, position, destination, uniform, skin, shoes, null);
        }

        public void Configure(string displayName, bool officer, Vector3 position, Vector3 destination,
            Material uniform, Material skin, Material shoes, string visualResource)
        {
            DisplayName = displayName;
            Officer = officer;
            transform.position = start = position;
            end = destination;
            body = gameObject.AddComponent<CharacterController>();
            body.height = 1.8f; body.radius = 0.28f; body.center = Vector3.up * 0.9f;
            body.stepOffset = 0.2f; body.skinWidth = 0.035f;
            if (!TryBuildAnimatedVisual(visualResource))
            {
                Part("Torso", transform, new Vector3(0, 1.18f, 0), new Vector3(.48f,.6f,.28f), uniform);
                Part("Head", transform, new Vector3(0, 1.65f, 0), new Vector3(.26f,.32f,.25f), skin);
                Part("Hair", transform, new Vector3(0, 1.81f, -.015f), new Vector3(.27f,.08f,.25f), shoes);
                Part("Face", transform, new Vector3(0,1.67f,.13f), new Vector3(.16f,.035f,.025f), shoes);
                leftArm = Limb("Left arm", new Vector3(-.33f,1.43f,0), .52f, uniform, skin);
                rightArm = Limb("Right arm", new Vector3(.33f,1.43f,0), .52f, uniform, skin);
                leftLeg = Limb("Left leg", new Vector3(-.14f,.9f,0), .74f, uniform, shoes);
                rightLeg = Limb("Right leg", new Vector3(.14f,.9f,0), .74f, uniform, shoes);
            }
            label = new GameObject("Name and role").transform;
            label.SetParent(transform, false); label.localPosition = new Vector3(0,2.12f,0);
            TextMesh text = label.gameObject.AddComponent<TextMesh>();
            text.text = displayName + "\n" + (officer ? "OFFICER" : "INMATE");
            text.fontSize = 40; text.characterSize = .055f; text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center; text.color = officer ? new Color(.65f,.84f,1f) : new Color(1f,.82f,.55f);
            interactionHalo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            interactionHalo.name = "Interaction focus";
            interactionHalo.transform.SetParent(transform, false);
            interactionHalo.transform.localPosition = Vector3.up * .025f;
            interactionHalo.transform.localScale = new Vector3(.85f, .012f, .85f);
            interactionHalo.GetComponent<Collider>().enabled = false;
            Destroy(interactionHalo.GetComponent<Collider>());
            interactionHalo.GetComponent<Renderer>().sharedMaterial = uniform;
            interactionHalo.SetActive(false);
        }

        public void SetHighlighted(bool highlighted) => interactionHalo.SetActive(highlighted);

        public string GreetingFor(bool playerIsOfficer)
        {
            if (Officer) return playerIsOfficer
                ? "Keep the walkway clear. We can check the movement board together."
                : "Tell me where you need to go. We'll check your assignment before movement.";
            switch (DisplayName)
            {
                case "Eli": return "Count comes first. I keep my bunk ready so mornings start quietly.";
                case "Mateo": return "This bit of yard is the quiet corner. A short walk helps me reset.";
                default: return "Laundry goes faster when we sort carefully. Fresh sheets make a difference.";
            }
        }

        public void Gesture(bool calm)
        {
            calmingGesture = calm;
            gestureUntil = Time.time + 1.2f;
            if (!calm && animationDriver != null) animationDriver.PlayTalk();
        }

        public void React(Transform player, bool shoved)
        {
            pauseUntil = Time.time + 3f;
            if (shoved)
            {
                recoilUntil = Time.time + .3f;
                if (animationDriver != null) animationDriver.PlayReaction();
            }
            Vector3 direction = player.position - transform.position; direction.y = 0;
            if (direction.sqrMagnitude > .001f) transform.rotation = Quaternion.LookRotation(direction);
        }

        public void Tick(bool paused, bool nearby)
        {
            if (body == null) return;
            float stride = 0;
            float normalizedSpeed = 0;
            if (!paused)
            {
                Vector3 movement = Vector3.zero;
                if (Time.time < recoilUntil) movement = -transform.forward * .6f;
                else if (!nearby && Time.time >= pauseUntil)
                {
                    Vector3 offset = (returning ? start : end) - transform.position; offset.y = 0;
                    if (offset.magnitude < .2f) { returning = !returning; pauseUntil = Time.time + 1.4f; }
                    else
                    {
                        movement = offset.normalized * .65f;
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(offset), Time.deltaTime * 4f);
                    }
                }
                Vector3 before = transform.position;
                CollisionFlags flags = body.Move((movement + Vector3.down * 3f) * Time.deltaTime);
                if ((flags & CollisionFlags.Sides) != 0) { returning = !returning; pauseUntil = Time.time + 1f; }
                Vector3 traveled = transform.position - before; traveled.y = 0;
                if (Time.deltaTime > 0f && Time.time >= recoilUntil)
                    normalizedSpeed = Mathf.Clamp01(traveled.magnitude / (1.8f * Time.deltaTime));
                if (traveled.sqrMagnitude > .000001f) stride = Mathf.Sin(Time.time * 6f) * 22f;
            }
            bool guard = Encounter.Tension > 0 && Time.time < pauseUntil;
            if (animationDriver != null)
            {
                animationDriver.Animator.SetFloat(Speed, normalizedSpeed);
                animationDriver.SetGuard(!paused && guard);
            }
            else if (leftArm != null && rightArm != null && leftLeg != null && rightLeg != null)
            {
            leftArm.localRotation = Quaternion.Euler(guard ? -65 : stride, 0, guard ? -20 : 0);
            rightArm.localRotation = Quaternion.Euler(guard ? -65 : -stride, 0, guard ? 20 : 0);
            if (!guard && Time.time < gestureUntil)
            {
                float weight = Mathf.Sin(Mathf.Clamp01((gestureUntil - Time.time) / 1.2f) * Mathf.PI);
                rightArm.localRotation = Quaternion.Euler(-35f * weight, 0, 12f * weight);
                if (calmingGesture) leftArm.localRotation = Quaternion.Euler(-35f * weight, 0, -12f * weight);
            }
            leftLeg.localRotation = Quaternion.Euler(-stride,0,0);
            rightLeg.localRotation = Quaternion.Euler(stride,0,0);
            }
            if (Camera.main != null) label.rotation = Camera.main.transform.rotation;
        }

        private bool TryBuildAnimatedVisual(string visualResource)
        {
            string resource = string.IsNullOrWhiteSpace(visualResource) ? "Characters/InmateTwoPlayable" : visualResource;
            GameObject prefab = Resources.Load<GameObject>(resource);
            if (prefab == null) return false;
            GameObject visual = Instantiate(prefab, transform);
            visual.name = "Animated character visual";
            visual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            animationDriver = CharacterInteractionAnimator.Attach(transform);
            if (animationDriver != null) return true;
            visual.SetActive(false);
            Destroy(visual);
            Debug.LogWarning("NPC is using its lightweight placeholder until the supplied character asset can animate: " + resource, this);
            return false;
        }

        private Transform Limb(string title, Vector3 joint, float length, Material clothing, Material tip)
        {
            Transform pivot = new GameObject(title).transform; pivot.SetParent(transform, false); pivot.localPosition = joint;
            Part(title + " segment", pivot, new Vector3(0,-length/2,0), new Vector3(.17f,length,.19f), clothing);
            Part(title + " end", pivot, new Vector3(0,-length,.035f), new Vector3(.18f,.13f,.25f), tip);
            return pivot;
        }

        private static void Part(string title, Transform parent, Vector3 position, Vector3 size, Material material)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube); part.name = title;
            part.transform.SetParent(parent, false); part.transform.localPosition = position; part.transform.localScale = size;
            Collider collider = part.GetComponent<Collider>(); collider.enabled = false; Destroy(collider);
            part.GetComponent<Renderer>().sharedMaterial = material;
        }
    }
}
