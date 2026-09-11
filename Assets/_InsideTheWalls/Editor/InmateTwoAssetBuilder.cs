using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace InsideTheWalls.Editor
{
    public static class InmateTwoAssetBuilder
    {
        private const string RigPath = "Assets/_InsideTheWalls/Art/Characters/InmateTwo/Runtime/InmateTwoRig.fbx";
        private const string TexturePath = "Assets/_InsideTheWalls/Art/Characters/InmateTwo/Runtime/InmateTwo_BaseColor.png";
        private const string Output = "Assets/_InsideTheWalls/Resources/Characters/";

        public static void EnsurePlayableCharacterAssets()
        {
            Directory.CreateDirectory(Output);
            AssetDatabase.Refresh();
            var importer = AssetImporter.GetAtPath(RigPath) as ModelImporter;
            if (importer == null) throw new FileNotFoundException("Run inmate_two_export.py before building inmate #2.");
            if (importer.userData != "ITW.InmateTwo.Humanoid.v1")
            {
                GameObject source = Require<GameObject>(RigPath);
                string[] bones = { "Hips", "Spine", "Spine1", "Spine2", "Neck", "Head", "LeftShoulder", "LeftArm", "LeftForeArm", "LeftHand", "RightShoulder", "RightArm", "RightForeArm", "RightHand", "LeftUpLeg", "LeftLeg", "LeftFoot", "LeftToeBase", "RightUpLeg", "RightLeg", "RightFoot", "RightToeBase" };
                string[] human = { "Hips", "Spine", "Chest", "UpperChest", "Neck", "Head", "LeftShoulder", "LeftUpperArm", "LeftLowerArm", "LeftHand", "RightShoulder", "RightUpperArm", "RightLowerArm", "RightHand", "LeftUpperLeg", "LeftLowerLeg", "LeftFoot", "LeftToes", "RightUpperLeg", "RightLowerLeg", "RightFoot", "RightToes" };
                HumanDescription description = importer.humanDescription;
                description.human = bones.Select((bone, i) => new HumanBone { boneName = bone, humanName = human[i], limit = new HumanLimit { useDefaultValues = true } }).ToArray();
                description.skeleton = source.GetComponentsInChildren<Transform>(true).Select(bone => new SkeletonBone { name = bone.name, position = bone.localPosition, rotation = bone.localRotation, scale = bone.localScale }).ToArray();
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.humanDescription = description;
                importer.skinWeights = ModelImporterSkinWeights.Custom;
                importer.maxBonesPerVertex = 4;
                importer.importCameras = false;
                importer.importLights = false;
                importer.importAnimation = false;
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                importer.userData = "ITW.InmateTwo.Humanoid.v1";
                importer.SaveAndReimport();
            }
            Avatar avatar = AssetDatabase.LoadAllAssetsAtPath(RigPath).OfType<Avatar>().FirstOrDefault();
            if (avatar == null || !avatar.isHuman || !avatar.isValid) throw new InvalidOperationException("Inmate #2 requires a valid Humanoid avatar.");
            InteractionAnimationBuilder.EnsureAssets();
            AnimatorController controller = Require<AnimatorController>(Output + "InmateTwoAnimations/InmateTwo.controller");
            if (!controller.parameters.Any(p => p.name == "Speed" && p.type == AnimatorControllerParameterType.Float))
                throw new InvalidOperationException("The shared locomotion controller requires a Speed float.");
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
            Material material = AssetDatabase.LoadAssetAtPath<Material>(Output + "InmateTwoRuntime.mat");
            if (material == null)
            {
                material = new Material(shader) { name = "Inmate Two Runtime" };
                AssetDatabase.CreateAsset(material, Output + "InmateTwoRuntime.mat");
            }
            material.shader = shader;
            material.SetTexture("_BaseMap", Require<Texture2D>(TexturePath));
            material.SetFloat("_Smoothness", 0.18f);
            EditorUtility.SetDirty(material);
            GameObject instance = PrefabUtility.InstantiatePrefab(Require<GameObject>(RigPath)) as GameObject;
            if (instance == null) throw new InvalidOperationException("Inmate #2 model could not be instantiated.");
            try
            {
                instance.name = "InmateTwoPlayable";
                instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                instance.transform.localScale = Vector3.one;
                foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true)) renderer.sharedMaterial = material;
                Animator animator = instance.GetComponent<Animator>();
                if (animator == null) animator = instance.AddComponent<Animator>();
                animator.avatar = avatar;
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                if (PrefabUtility.SaveAsPrefabAsset(instance, Output + "InmateTwoPlayable.prefab") == null)
                    throw new IOException("Could not save inmate #2 prefab.");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
            AssetDatabase.SaveAssets();
            Debug.Log("INMATE_TWO_ASSETS_OK valid humanoid using supplied interaction and locomotion clips");
        }

        private static T Require<T>(string path) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            return asset != null ? asset : throw new FileNotFoundException("Required character asset missing: " + path);
        }
    }
}
