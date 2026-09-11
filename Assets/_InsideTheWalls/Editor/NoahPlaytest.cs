using System;
using System.IO;
using InsideTheWalls.UI;
using UnityEditor;
using UnityEngine;

namespace InsideTheWalls.Editor
{
    [InitializeOnLoad]
    public static class NoahPlaytest
    {
        private const string MenuPath = "Inside the Walls/Playtest/Animated Inmate Check (No Save)";
        private const string PopulationMenuPath = "Inside the Walls/Playtest/Population Preview (No Save)";
        private const string PendingPathKey = "ITW.NoahPlaytest.PendingPath";
        private const string PreviewKey = "ITW.NoahPlaytest.PopulationPreview";
        private static double deadline;

        static NoahPlaytest()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        [MenuItem(MenuPath)]
        public static void Start() => StartSession(false);

        [MenuItem(PopulationMenuPath)]
        public static void StartPopulation() => StartSession(true);

        private static void StartSession(bool populationPreview)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                throw new InvalidOperationException("Stop Play Mode and finish compilation before starting the Noah playtest.");

            string project = Path.GetDirectoryName(UnityEngine.Application.dataPath);
            string output = Path.Combine(project, "Docs", "Screenshots", "Noah-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff"));
            SessionState.SetString(PendingPathKey, output);
            SessionState.SetBool(PreviewKey, populationPreview);
            EditorApplication.isPlaying = true;
        }

        [MenuItem(MenuPath, true)]
        [MenuItem(PopulationMenuPath, true)]
        private static bool CanStart() => !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling;

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode && !string.IsNullOrEmpty(SessionState.GetString(PendingPathKey, string.Empty)))
            {
                deadline = EditorApplication.timeSinceStartup + 30d;
                EditorApplication.update += StartWhenReady;
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.update -= StartWhenReady;
                SessionState.EraseString(PendingPathKey);
                SessionState.EraseBool(PreviewKey);
            }
        }

        private static void StartWhenReady()
        {
            FrontendController frontend = UnityEngine.Object.FindFirstObjectByType<FrontendController>();
            if (frontend == null && EditorApplication.timeSinceStartup < deadline) return;

            EditorApplication.update -= StartWhenReady;
            string output = SessionState.GetString(PendingPathKey, string.Empty);
            bool populationPreview = SessionState.GetBool(PreviewKey, false);
            SessionState.EraseString(PendingPathKey);
            SessionState.EraseBool(PreviewKey);
            if (frontend == null)
            {
                Debug.LogError("Noah playtest could not find the application frontend after entering Play Mode.");
                return;
            }

            EditorApplication.ExecuteMenuItem("Window/General/Game");
            if (populationPreview) frontend.BeginPopulationPreview();
            else
            {
                frontend.BeginNoahVerification(output);
                Debug.Log("NOAH_PLAYTEST_STARTED output=" + output + " (saves disabled)");
            }
        }
    }
}
