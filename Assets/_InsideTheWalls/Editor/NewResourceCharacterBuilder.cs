using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace InsideTheWalls.Editor
{
    public static class NewResourceCharacterBuilder
    {
        private const string SourceRoot = "Assets/_InsideTheWalls/new resources02/";
        private const string Output = "Assets/_InsideTheWalls/Resources/Characters/";
        private const string ImportVersion = "ITW.NewResources02.Humanoid.v1";

        public static void EnsureAssets()
        {
            Directory.CreateDirectory(Output);
            AssetDatabase.Refresh();
            BuildHumanoidPrefab(
                SourceRoot + "officer-lena_Rigged_4727484803/officer-lena_Rigged_4727484803/character_publish.blend",
                "OfficerLenaPlayable");
            BuildHumanoidPrefab(
                SourceRoot + "npc inmate #3_Rigged_4727584817/npc inmate #3_Rigged_4727584817/character_publish.blend",
                "InmateThreePlayable");
            AssetDatabase.SaveAssets();
            Debug.Log("NEW_RESOURCE_CHARACTERS_OK officer-lena and inmate-three runtime prefabs generated");
        }

        private static void BuildHumanoidPrefab(string modelPath, string prefabName)
        {
            if (!File.Exists(modelPath)) throw new FileNotFoundException("Missing supplied rigged character", modelPath);
            var importer = AssetImporter.GetAtPath(modelPath) as ModelImporter;
            if (importer == null) throw new InvalidOperationException("Character ModelImporter unavailable: " + modelPath);
            if (importer.userData != ImportVersion)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.importAnimation = true;
                importer.importCameras = false;
                importer.importLights = false;
                importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
                importer.userData = ImportVersion;
                importer.SaveAndReimport();
            }

            Avatar avatar = AssetDatabase.LoadAllAssetsAtPath(modelPath).OfType<Avatar>().FirstOrDefault();
            if (avatar == null || !avatar.isValid || !avatar.isHuman)
                throw new InvalidOperationException("Supplied rigged character is not a valid Humanoid model: " + modelPath);

            AnimatorController controller = Require<AnimatorController>(InteractionAnimationBuilder.ControllerPath);
            GameObject instance = PrefabUtility.InstantiatePrefab(Require<GameObject>(modelPath)) as GameObject;
            if (instance == null) throw new InvalidOperationException("Could not instantiate supplied character: " + modelPath);
            try
            {
                instance.name = prefabName;
                instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                instance.transform.localScale = Vector3.one;
                Animator animator = instance.GetComponent<Animator>();
                if (animator == null) animator = instance.AddComponent<Animator>();
                animator.avatar = avatar;
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    renderer.receiveShadows = true;
                }
                if (PrefabUtility.SaveAsPrefabAsset(instance, Output + prefabName + ".prefab") == null)
                    throw new IOException("Could not save supplied character prefab: " + prefabName);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        private static T Require<T>(string path) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            return asset != null ? asset : throw new FileNotFoundException("Required character asset missing: " + path);
        }
    }
}
