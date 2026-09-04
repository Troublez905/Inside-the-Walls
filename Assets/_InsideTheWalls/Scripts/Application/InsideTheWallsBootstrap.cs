using UnityEngine;

namespace InsideTheWalls.Application
{
    public static class InsideTheWallsBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StartApplication()
        {
            if (Object.FindFirstObjectByType<UI.FrontendController>() != null)
            {
                return;
            }

            var root = new GameObject("InsideTheWalls.Application");
            Object.DontDestroyOnLoad(root);
            root.AddComponent<UI.FrontendController>();
        }
    }
}
