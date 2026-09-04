using System;
using InsideTheWalls.Application;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace InsideTheWalls.Editor
{
    public static class FoundationBuild
    {
        public static void ValidateAndBuild()
        {
            ValidateCoreRules();
            string bootScene = EnsureBootScene();

            PlayerSettings.companyName = "Troublez905";
            PlayerSettings.productName = "Inside the Walls";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.fullScreenMode = UnityEngine.FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;

            string[] scenes = { bootScene };
            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = "Builds/Windows/InsideTheWalls.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"Windows build failed: {report.summary.result}, {report.summary.totalErrors} errors");
            }

            UnityEngine.Debug.Log($"FOUNDATION_BUILD_OK size={report.summary.totalSize} warnings={report.summary.totalWarnings}");
        }

        private static string EnsureBootScene()
        {
            const string folder = "Assets/_InsideTheWalls/Scenes/Boot";
            const string path = folder + "/Boot.unity";
            System.IO.Directory.CreateDirectory(folder);
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, path);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(path, true) };
            return path;
        }

        private static void ValidateCoreRules()
        {
            if (MenuAvailability.CanContinue(false, true))
            {
                throw new InvalidOperationException("Continue must remain disabled without a save.");
            }

            if (!MenuAvailability.CanContinue(true, true))
            {
                throw new InvalidOperationException("Continue must be enabled for a compatible save.");
            }

            if (MenuAvailability.ContinueReason(false, false) != "No saved session")
            {
                throw new InvalidOperationException("Missing-save reason is not explicit.");
            }
        }
    }
}
