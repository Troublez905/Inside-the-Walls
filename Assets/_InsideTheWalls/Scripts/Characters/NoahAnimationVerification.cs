#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace InsideTheWalls.Characters
{
    // Opt-in development smoke test. The frontend must disable saves before calling Begin.
    public sealed class NoahAnimationVerification : MonoBehaviour
    {
        private const int MotionSamples = 12;
        private readonly StringBuilder diagnostics = new StringBuilder();
        private bool failed;
        private bool previousRunInBackground;
        private bool hasSpeedParameter;
        private float deadline;
        private PrototypePlayerController player;
        private Animator animator;
        private SkinnedMeshRenderer skin;
        private GameObject blocker;

        public bool IsRunning { get; private set; }
        public bool IsComplete { get; private set; }
        public bool Succeeded { get; private set; }
        public string CurrentStep { get; private set; } = "Not started";
        public string Result { get; private set; } = string.Empty;
        public string FailureReason { get; private set; } = string.Empty;
        public string OutputDirectory { get; private set; } = string.Empty;
        public string Diagnostics => diagnostics.ToString();

        public void Begin(string directory)
        {
            if (IsRunning || IsComplete) throw new InvalidOperationException("Create a new Noah verification component for each run.");
            if (!UnityEngine.Application.isPlaying) throw new InvalidOperationException("Noah verification requires play mode.");
            if (string.IsNullOrWhiteSpace(directory)) throw new ArgumentException("A capture directory is required.", nameof(directory));
            OutputDirectory = Path.GetFullPath(directory);
            Directory.CreateDirectory(OutputDirectory);
            // Replace any old terminal result before polling begins.
            File.WriteAllText(Path.Combine(OutputDirectory, "result.txt"), "NOAH_SMOKE_RUNNING");
            previousRunInBackground = UnityEngine.Application.runInBackground;
            UnityEngine.Application.runInBackground = true;
            IsRunning = true;
            CurrentStep = "Starting";
            UnityEngine.Application.logMessageReceived += OnLog;
            deadline = Time.realtimeSinceStartup + 90f;
            StartCoroutine(RunVerification());
        }

        private void Update()
        {
            if (!IsRunning) return;
            if (!failed && Time.realtimeSinceStartup > deadline)
                Check(false, "timeout during " + CurrentStep);
            if (failed) Complete();
        }

        private void OnLog(string message, string trace, LogType type)
        {
            if (!IsRunning || (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)) return;
            failed = true;
            if (string.IsNullOrEmpty(FailureReason)) FailureReason = message;
            diagnostics.AppendLine($"{type}: {message}\n{trace}");
        }

        private void OnDisable()
        {
            if (!IsRunning) return;
            Check(false, "verification interrupted during " + CurrentStep);
            Complete();
        }

        private void OnDestroy() => UnityEngine.Application.logMessageReceived -= OnLog;

        private bool Check(bool condition, string message)
        {
            string entry = (condition ? "NOAH_CHECK_OK " : "NOAH_CHECK_FAILED ") + message;
            if (condition) Record(entry);
            else
            {
                failed = true;
                if (string.IsNullOrEmpty(FailureReason)) FailureReason = message;
                Debug.LogError(entry);
            }
            return condition;
        }

        private void Record(string message)
        {
            diagnostics.AppendLine(message);
            Debug.Log(message);
        }

        private void RecordAnimation(string label)
        {
            Camera camera = Camera.main;
            string state = "unavailable";
            if (animator != null && animator.isInitialized && animator.layerCount > 0)
            {
                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                state = $"hash:{info.fullPathHash} idle:{info.IsName("Idle")} walk:{info.IsName("Walk")} normalizedTime:{info.normalizedTime:F4} stateSpeed:{info.speed:F3} multiplier:{info.speedMultiplier:F3} transition:{animator.IsInTransition(0)}";
            }
            string animation = animator == null ? "missing" : $"enabled:{animator.enabled} active:{animator.gameObject.activeInHierarchy} initialized:{animator.isInitialized} culling:{animator.cullingMode} updateMode:{animator.updateMode} animatorSpeed:{animator.speed:F3} Speed:{(hasSpeedParameter ? animator.GetFloat("Speed").ToString("F3") : "unavailable")} state:({state})";
            string visibility = skin == null ? "missing" : $"enabled:{skin.enabled} active:{skin.gameObject.activeInHierarchy} isVisibleAnyCamera:{skin.isVisible} forceRenderingOff:{skin.forceRenderingOff} updateWhenOffscreen:{skin.updateWhenOffscreen} bounds:{skin.bounds}";
            string view = camera == null ? "missing" : $"name:{camera.name} enabled:{camera.enabled} active:{camera.gameObject.activeInHierarchy} pixels:{camera.pixelWidth}x{camera.pixelHeight} mask:{camera.cullingMask} position:{camera.transform.position}";
            if (camera != null && skin != null)
                view += $" skinInFrustum:{GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), skin.bounds)} skinLayerIncluded:{(camera.cullingMask & (1 << skin.gameObject.layer)) != 0} skinViewport:{camera.WorldToViewportPoint(skin.bounds.center)}";
            Record($"NOAH_DIAGNOSTIC {label} realtime:{Time.realtimeSinceStartup:F3} timeScale:{Time.timeScale:F3} animator=({animation}) renderer=({visibility}) camera=({view})");
        }

        private bool Capture(string name)
        {
            Camera camera = Camera.main;
            RecordAnimation("before capture " + name);
            if (!Check(camera != null && camera.isActiveAndEnabled, "active capture camera")) return false;
            RenderTexture previous = RenderTexture.active;
            RenderTexture target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Texture2D capture = null;
            try
            {
                var request = new RenderPipeline.StandardRequest { destination = target };
                if (!Check(RenderPipeline.SupportsRenderRequest(camera, request), "pipeline supports StandardRequest")) return false;
                // An explicit request renders with URP even when a hidden Game view never reaches WaitForEndOfFrame.
                RenderTexture.active = target;
                GL.Clear(true, true, Color.black);
                RenderTexture.active = previous;
                RenderPipeline.SubmitRenderRequest(camera, request);
                RenderTexture.active = target;
                capture = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
                capture.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
                capture.Apply();
                Color32[] pixels = capture.GetPixels32();
                int litPixels = 0;
                double luminance = 0;
                for (int i = 0; i < pixels.Length; i++)
                {
                    Color32 pixel = pixels[i];
                    if (pixel.r > 8 || pixel.g > 8 || pixel.b > 8) litPixels++;
                    luminance += (0.2126 * pixel.r + 0.7152 * pixel.g + 0.0722 * pixel.b) / 255.0;
                }
                float litFraction = (float)litPixels / pixels.Length;
                File.WriteAllBytes(Path.Combine(OutputDirectory, name + ".png"), capture.EncodeToPNG());
                Record($"NOAH_CAPTURE {name} size:{target.width}x{target.height} litFraction:{litFraction:F6} meanLuminance:{luminance / pixels.Length:F6} source:URP-camera-request (IMGUI HUD excluded)");
                return Check(litFraction > 0.001f, name + " is not black");
            }
            finally
            {
                RenderTexture.active = previous;
                if (capture != null) Destroy(capture);
                RenderTexture.ReleaseTemporary(target);
            }
        }

        private IEnumerator RunVerification()
        {
            yield return Verify();
            if (IsRunning) Complete();
        }

        private IEnumerator Verify()
        {
            yield return null;
            CurrentStep = "Character setup";
            player = FindFirstObjectByType<PrototypePlayerController>();
            if (!Check(player != null, "player spawned")) yield break;
            player.VerificationMoveInput = Vector2.zero;
            animator = player.GetComponentInChildren<Animator>();
            skin = player.GetComponentInChildren<SkinnedMeshRenderer>();
            RecordAnimation("setup");
            if (!Check(animator != null && animator.enabled && animator.avatar != null && animator.isHuman && animator.avatar.isValid && animator.runtimeAnimatorController != null, "valid enabled humanoid animator and controller")) yield break;
            foreach (AnimatorControllerParameter parameter in animator.parameters)
                if (parameter.name == "Speed" && parameter.type == AnimatorControllerParameterType.Float) hasSpeedParameter = true;
            if (!Check(hasSpeedParameter, "float Speed parameter")) yield break;
            MeshRenderer capsule = player.GetComponent<MeshRenderer>();
            if (!Check(capsule != null && !capsule.enabled, "capsule replaced by Noah")) yield break;
            if (!Check(skin != null && skin.enabled && skin.sharedMesh != null && skin.sharedMaterial != null && skin.sharedMaterial.shader != null && skin.sharedMaterial.shader.isSupported && skin.sharedMaterial.shader.name == "Universal Render Pipeline/Lit", "supported URP skin material")) yield break;
            Record($"NOAH_MESH vertices={skin.sharedMesh.vertexCount} bones={skin.bones.Length} bounds={skin.bounds}");
            player.TeleportTo(new Vector3(0f, 1f, -5f), 0f);
            CurrentStep = "Idle";
            yield return new WaitForSecondsRealtime(0.7f);
            RecordAnimation("idle");
            if (!Check(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"), "idle state")) yield break;
            if (!Capture("noah-idle-gameplay")) yield break;

            CurrentStep = "Walking samples";
            Transform[] legs = { animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg), animator.GetBoneTransform(HumanBodyBones.RightLowerLeg) };
            if (!Check(legs[0] != null && legs[1] != null, "retargeted lower-leg bones")) yield break;
            Vector3 start = player.transform.position;
            player.VerificationMoveInput = Vector2.up;
            yield return new WaitForSecondsRealtime(0.4f);
            var rotations = new Quaternion[MotionSamples, legs.Length];
            float maximumLegAngle = 0f;
            int walkSamples = 0;
            // Sample without extra renders or Animator.Update calls so culling remains observable.
            for (int sample = 0; sample < MotionSamples; sample++)
            {
                yield return new WaitForSecondsRealtime(0.09f + sample % 3 * 0.037f);
                for (int bone = 0; bone < legs.Length; bone++)
                {
                    rotations[sample, bone] = legs[bone].localRotation;
                    for (int prior = 0; prior < sample; prior++)
                        maximumLegAngle = Mathf.Max(maximumLegAngle, Quaternion.Angle(rotations[prior, bone], rotations[sample, bone]));
                }
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) walkSamples++;
                RecordAnimation($"walk sample {sample + 1}/{MotionSamples} maxLegAngle:{maximumLegAngle:F3}");
            }
            // Keep the visual evidence even when the motion assertion fails.
            if (!Capture("noah-walking-gameplay")) yield break;
            if (!Check(maximumLegAngle > 3f, $"retargeted leg motion across {MotionSamples} samples ({maximumLegAngle:F2} degrees)")) yield break;
            if (!Check(walkSamples >= MotionSamples / 2, $"walk state observed ({walkSamples}/{MotionSamples} samples)")) yield break;
            Vector3 travel = player.transform.position - start;
            travel.y = 0f;
            if (!Check(travel.magnitude > 1f, $"controller moved ({travel.magnitude:F2}m)")) yield break;

            CurrentStep = "Stop and interaction suppression";
            player.VerificationMoveInput = Vector2.zero;
            yield return new WaitForSecondsRealtime(0.5f);
            RecordAnimation("stopped");
            if (!Check(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && animator.GetFloat("Speed") < 0.05f, "stop returns to idle")) yield break;
            player.VerificationMoveInput = Vector2.up;
            player.SetMovementSuppressed(true);
            start = player.transform.position;
            yield return new WaitForSecondsRealtime(0.35f);
            if (!Check(Vector3.Distance(start, player.transform.position) < 0.05f && animator.GetFloat("Speed") < 0.05f, "interaction suppression stops motion")) yield break;
            player.SetMovementSuppressed(false);

            CurrentStep = "Wall collision";
            blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blocker.name = "Noah verification blocker";
            blocker.transform.position = player.transform.position + Vector3.forward * 1.2f;
            blocker.transform.localScale = new Vector3(4f, 3f, 0.3f);
            blocker.GetComponent<Renderer>().enabled = false;
            yield return new WaitForSecondsRealtime(1.2f);
            RecordAnimation("wall collision");
            if (!Check(animator.GetFloat("Speed") < 0.05f && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"), "wall collision returns to idle")) yield break;
            RemoveBlocker();
            player.VerificationMoveInput = Vector2.zero;
            player.SetMovementSuppressed(true);
            player.enabled = false;
            CurrentStep = "Front detail";
            Camera camera = Camera.main;
            if (!Check(camera != null, "front detail camera")) yield break;
            camera.transform.position = skin.bounds.center + new Vector3(2.5f, 0.6f, 3.6f);
            camera.transform.LookAt(skin.bounds.center);
            yield return new WaitForSecondsRealtime(0.5f);
            Capture("noah-front-detail");
        }

        private void RemoveBlocker()
        {
            if (blocker == null) return;
            blocker.SetActive(false);
            Destroy(blocker);
            blocker = null;
        }

        private void Complete()
        {
            if (!IsRunning) return;
            RecordAnimation("completion during " + CurrentStep);
            IsRunning = false;
            StopAllCoroutines();
            UnityEngine.Application.logMessageReceived -= OnLog;
            if (player != null)
            {
                player.VerificationMoveInput = Vector2.zero;
                player.SetMovementSuppressed(true);
            }
            RemoveBlocker();
            UnityEngine.Application.runInBackground = previousRunInBackground;
            Succeeded = !failed;
            Result = Succeeded
                ? "NOAH_SMOKE_OK: humanoid, URP camera captures, idle/walk/stop, sampled retargeted bones, movement, suppression, wall collision. Saves disabled by frontend."
                : $"NOAH_SMOKE_FAILED during {CurrentStep}: {FailureReason}. See diagnostics.txt and player/editor log.";
            try
            {
                File.WriteAllText(Path.Combine(OutputDirectory, "diagnostics.txt"), Diagnostics);
                File.WriteAllText(Path.Combine(OutputDirectory, "result.txt"), Result);
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            {
                Succeeded = false;
                FailureReason = "Could not write verification results: " + exception.Message;
                Result = "NOAH_SMOKE_FAILED: " + FailureReason;
                diagnostics.AppendLine(exception.ToString());
                Debug.LogException(exception);
            }
            CurrentStep = Succeeded ? "Complete" : "Failed";
            IsComplete = true;
            if (Succeeded) Debug.Log(Result);
            else Debug.LogError(Result);
            enabled = false;
#if !UNITY_EDITOR
            UnityEngine.Application.Quit(Succeeded ? 0 : 2);
#endif
        }
    }
}
#endif
