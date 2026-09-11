#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace InsideTheWalls.Characters
{
    // Enter only through FrontendController's no-save population preview.
    public sealed class PopulationVerification : MonoBehaviour
    {
        private readonly StringBuilder evidence = new StringBuilder();
        private string output;
        private string failure;
        private float deadline;
        private bool running;
        private bool previousBackground;
        private PrototypePlayerController player;
        private GameObject blocker;

        public void Begin(string directory)
        {
            output = Path.GetFullPath(directory);
            Directory.CreateDirectory(output);
            File.WriteAllText(Path.Combine(output, "result.txt"), "POPULATION_SMOKE_RUNNING");
            previousBackground = UnityEngine.Application.runInBackground;
            UnityEngine.Application.runInBackground = true;
            UnityEngine.Application.logMessageReceived += OnLog;
            running = true;
            deadline = Time.realtimeSinceStartup + 90f;
            StartCoroutine(Run());
        }

        private void OnLog(string message, string stack, LogType type)
        {
            if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
            if (failure == null) failure = message;
            evidence.AppendLine(message + "\n" + stack);
        }

        private void Update()
        {
            if (!running) return;
            if (Time.realtimeSinceStartup > deadline && failure == null) failure = "Verification timeout.";
            if (failure != null) Finish();
        }

        private void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("POPULATION_CHECK_FAILED " + message);
            evidence.AppendLine("PASS " + message);
        }

        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(0.7f);
            player = FindFirstObjectByType<PrototypePlayerController>();
            PrisonPopulation population = FindFirstObjectByType<PrisonPopulation>();
            Require(player != null && population != null, "player and population spawned");
            Require(player.transform.Find("Inmate #2 Visual") != null, "inmate #2 selected, not fallback");
            Require(population.Npcs.Count == 5 && population.Npcs.Count(n => n.Officer) == 2, "three inmates and two officers");
            Require(population.Npcs.All(n => n.GetComponent<CharacterController>().enabled), "NPC collision controllers enabled");
            Require(FindObjectsByType<Renderer>(FindObjectsSortMode.None).All(r => r.sharedMaterials.All(m => m != null && m.shader != null && m.shader.isSupported)), "scene materials have supported shaders");

            player.VerificationMoveInput = Vector2.zero;
            PrisonNpc partner = population.Npcs.First(n => n.DisplayName == "Mateo");
            player.TeleportTo(partner.transform.position + new Vector3(0f, 1f, -1.4f), 0f);
            yield return new WaitForSecondsRealtime(0.25f);
            population.Tick(false, false);
            Require(population.TryTalk(partner) && partner.Encounter.Rapport == 1, "nearby conversation changes rapport");
            Require(!population.TryShove(partner), "cooldown rejects immediate shove");
            yield return new WaitForSecondsRealtime(1.3f);
            Require(population.TryShove(partner) && partner.Encounter.Tension == 2, "shove creates temporary tension");
            yield return new WaitForSecondsRealtime(0.12f);
            player.enabled = false;
            Camera camera = Camera.main;
            camera.transform.position = partner.transform.position + new Vector3(-3.5f, 2.2f, -4.5f);
            camera.transform.LookAt(partner.transform.position + Vector3.up);
            Capture(camera, "population-encounter");
            yield return new WaitForSecondsRealtime(1.3f);
            Require(population.TryCalm(partner) && partner.Encounter.Tension == 0, "de-escalation lowers tension");
            yield return new WaitForSecondsRealtime(1.3f);
            population.Tick(true, false);
            Require(!population.TryShove(partner), "blocked input rejects action");
            population.Tick(false, true);
            Require(!population.TryTalk(partner), "objective retains interaction priority");
            population.Tick(false, false);

            blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blocker.name = "Population verification occluder";
            blocker.transform.position = (partner.transform.position + player.transform.position) * 0.5f + Vector3.up * 0.5f;
            blocker.transform.localScale = new Vector3(2f, 3f, 0.18f);
            blocker.GetComponent<Renderer>().enabled = false;
            Physics.SyncTransforms();
            Require(!population.TryTalk(partner), "wall blocks conversation");
            blocker.SetActive(false);
            Destroy(blocker);
            blocker = null;
            player.TeleportTo(new Vector3(-8f, 1f, -11f), 0f);
            Require(!population.TryTalk(partner), "out-of-range action rejected");

            camera.transform.position = new Vector3(2f, 14f, -18f);
            camera.transform.LookAt(new Vector3(0f, 0f, 2f));
            Capture(camera, "population-facility");
            var day = GetComponent<Presentation.PlayableDayController>();
            Require(day != null && day.ObjectiveIndex == 0, "NPC actions do not advance objective save checkpoints");
            Finish();
        }

        private void Capture(Camera camera, string name)
        {
            Require(camera != null, "capture camera available");
            RenderTexture previous = RenderTexture.active;
            RenderTexture target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Texture2D image = null;
            try
            {
                var request = new RenderPipeline.StandardRequest { destination = target };
                Require(RenderPipeline.SupportsRenderRequest(camera, request), "URP camera request supported");
                RenderPipeline.SubmitRenderRequest(camera, request);
                RenderTexture.active = target;
                image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
                image.Apply();
                File.WriteAllBytes(Path.Combine(output, name + ".png"), image.EncodeToPNG());
                int lit = image.GetPixels32().Count(p => p.r > 8 || p.g > 8 || p.b > 8);
                Require(lit > 1280 * 720 / 1000, name + " is not black (camera-only, HUD excluded)");
            }
            finally
            {
                RenderTexture.active = previous;
                if (image != null) Destroy(image);
                RenderTexture.ReleaseTemporary(target);
            }
        }

        private void Finish()
        {
            if (!running) return;
            running = false;
            StopAllCoroutines();
            UnityEngine.Application.logMessageReceived -= OnLog;
            UnityEngine.Application.runInBackground = previousBackground;
            if (blocker != null) { blocker.SetActive(false); Destroy(blocker); }
            if (player != null) { player.VerificationMoveInput = Vector2.zero; player.SetMovementSuppressed(true); }
            string result = failure == null ? "POPULATION_SMOKE_OK" : "POPULATION_SMOKE_FAILED: " + failure;
            File.WriteAllText(Path.Combine(output, "diagnostics.txt"), evidence.ToString());
            File.WriteAllText(Path.Combine(output, "result.txt"), result);
            Debug.Log(result);
#if !UNITY_EDITOR
            UnityEngine.Application.Quit(failure == null ? 0 : 2);
#endif
            enabled = false;
        }

        private void OnDisable()
        {
            if (!running) return;
            failure = "Verification interrupted.";
            Finish();
        }
    }
}
#endif
