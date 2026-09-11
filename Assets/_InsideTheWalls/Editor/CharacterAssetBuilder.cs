using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace InsideTheWalls.Editor
{
    public static class CharacterAssetBuilder
    {
        private const string ModelPath = "Assets/_InsideTheWalls/Art/Characters/Principals/NoahMercer/Source/Interchange/NoahMercer_SourceRig.fbx";
        private const string BaseColorPath = "Assets/_InsideTheWalls/Art/Characters/Principals/NoahMercer/Source/Interchange/Textures/NoahMercer_BaseColor.png";
        private const string NormalPath = "Assets/_InsideTheWalls/Art/Characters/Principals/NoahMercer/Source/Interchange/Textures/NoahMercer_Normal.png";
        private const string IdlePath = "Assets/DoubleL/Demo/Anim/OneHand_Up_Idle.anim";
        private const string WalkPath = "Assets/DoubleL/Demo/Anim/OneHand_Up_Walk_F_InPlace.anim";
        private const string OutputDirectory = "Assets/_InsideTheWalls/Resources/Characters";
        private const string MaterialPath = OutputDirectory + "/NoahMercerRuntime.mat";
        private const string ControllerPath = OutputDirectory + "/NoahMercerRuntime.controller";
        private const string PrefabPath = OutputDirectory + "/NoahMercerPlayable.prefab";
        private const string RuntimeRigPath = "Assets/_InsideTheWalls/Art/Characters/Principals/NoahMercer/Runtime/NoahMercerRig.fbx";

        public static void EnsurePlayableCharacterAssets()
        {
            Directory.CreateDirectory(OutputDirectory);
            AssetDatabase.Refresh();

            GameObject model = BuildHumanoidRig();
            AnimationClip idle = BuildLocomotionClip(IdlePath, "NoahIdle", false);
            AnimationClip walk = BuildLocomotionClip(WalkPath, "NoahWalk", true);
            Material material = BuildMaterial();
            AnimatorController controller = BuildController(idle, walk);

            GameObject instance = PrefabUtility.InstantiatePrefab(model) as GameObject;
            if (instance == null) throw new InvalidOperationException("Noah model could not be instantiated.");

            try
            {
                instance.name = "NoahMercerPlayable";
                instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                instance.transform.localScale = Vector3.one;
                foreach (Collider collider in instance.GetComponentsInChildren<Collider>(true)) UnityEngine.Object.DestroyImmediate(collider);
                foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true)) renderer.sharedMaterial = material;

                Animator animator = instance.GetComponent<Animator>();
                if (animator == null) animator = instance.AddComponent<Animator>();
                animator.avatar = AssetDatabase.LoadAllAssetsAtPath(RuntimeRigPath).OfType<Avatar>().FirstOrDefault();
                if (animator.avatar == null || !animator.avatar.isValid || !animator.avatar.isHuman)
                    throw new InvalidOperationException("Noah requires a valid Humanoid avatar before a playable prefab can be saved.");
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                PrefabUtility.SaveAsPrefabAsset(instance, PrefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("CHARACTER_ASSETS_OK NoahMercerPlayable with RPG idle/walk controller");
        }

        private static GameObject BuildHumanoidRig()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(RuntimeRigPath));
            AssetDatabase.Refresh();
            if (!File.Exists(RuntimeRigPath) && !AssetDatabase.CopyAsset(ModelPath, RuntimeRigPath))
                throw new IOException("Unable to create Noah's runtime rig copy.");
            var importer = AssetImporter.GetAtPath(RuntimeRigPath) as ModelImporter;
            if (importer == null) throw new InvalidOperationException("Noah runtime FBX importer is unavailable.");
            if (importer.animationType != ModelImporterAnimationType.Human || importer.userData != "ITW.Noah.Humanoid.v1")
            {
                GameObject model = RequireAsset<GameObject>(RuntimeRigPath);
                string[] boneNames = { "Hips", "Spine", "Spine1", "Spine2", "Neck", "Head", "LeftShoulder", "LeftArm", "LeftForeArm", "LeftHand", "RightShoulder", "RightArm", "RightForeArm", "RightHand", "LeftUpLeg", "LeftLeg", "LeftFoot", "LeftToeBase", "RightUpLeg", "RightLeg", "RightFoot", "RightToeBase" };
                string[] humanNames = { "Hips", "Spine", "Chest", "UpperChest", "Neck", "Head", "LeftShoulder", "LeftUpperArm", "LeftLowerArm", "LeftHand", "RightShoulder", "RightUpperArm", "RightLowerArm", "RightHand", "LeftUpperLeg", "LeftLowerLeg", "LeftFoot", "LeftToes", "RightUpperLeg", "RightLowerLeg", "RightFoot", "RightToes" };
                HumanDescription description = importer.humanDescription;
                description.human = boneNames.Select((bone, index) => new HumanBone
                {
                    boneName = bone, humanName = humanNames[index], limit = new HumanLimit { useDefaultValues = true }
                }).ToArray();
                description.skeleton = model.GetComponentsInChildren<Transform>(true).Select(bone => new SkeletonBone
                {
                    name = bone.name, position = bone.localPosition, rotation = bone.localRotation, scale = bone.localScale
                }).ToArray();
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.humanDescription = description;
                importer.skinWeights = ModelImporterSkinWeights.Custom;
                importer.maxBonesPerVertex = 4;
                importer.importCameras = false;
                importer.importLights = false;
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                importer.userData = "ITW.Noah.Humanoid.v1";
                importer.SaveAndReimport();
            }
            return RequireAsset<GameObject>(RuntimeRigPath);
        }

        private static AnimationClip BuildLocomotionClip(string sourcePath, string name, bool walking)
        {
            AnimationClip source = RequireAsset<AnimationClip>(sourcePath);
            if (!source.humanMotion) throw new InvalidOperationException($"Expected humanoid motion in {sourcePath}");
            string path = OutputDirectory + "/" + name + ".anim";
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
            {
                clip = UnityEngine.Object.Instantiate(source);
                AssetDatabase.CreateAsset(clip, path);
            }
            else EditorUtility.CopySerialized(source, clip);
            clip.name = name;
            // Preserve the pack's leg cycle, but replace its raised weapon-arm pose.
            foreach (string side in new[] { "Left", "Right" })
            {
                SetMuscle(clip, side + " Shoulder Down-Up", -0.15f);
                SetMuscle(clip, side + " Shoulder Front-Back", 0f);
                SetMuscle(clip, side + " Arm Down-Up", -0.78f);
                SetMuscle(clip, side + " Arm Twist In-Out", 0f);
                SetMuscle(clip, side + " Forearm Stretch", 0.7f);
                SetMuscle(clip, side + " Forearm Twist In-Out", 0f);
                SetMuscle(clip, side + " Hand Down-Up", 0f);
                SetMuscle(clip, side + " Hand In-Out", 0f);
                var swing = new AnimationCurve();
                for (int i = 0; i <= 32; i++)
                {
                    float phase = i / 32f;
                    float amplitude = walking ? 0.16f : 0.015f;
                    float sign = side == "Left" ? 1f : -1f;
                    swing.AddKey(phase * source.length, sign * amplitude * Mathf.Sin(phase * Mathf.PI * 2f));
                }
                AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve("", typeof(Animator), side + " Arm Front-Back"), swing);
            }
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            settings.loopBlend = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            EditorUtility.SetDirty(clip);
            return clip;
        }

        private static void SetMuscle(AnimationClip clip, string muscle, float value)
        {
            if (!HumanTrait.MuscleName.Contains(muscle)) throw new InvalidOperationException($"Unknown muscle: {muscle}");
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve("", typeof(Animator), muscle), AnimationCurve.Constant(0f, clip.length, value));
        }

        private static Material BuildMaterial()
        {
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(
                "Packages/com.unity.render-pipelines.universal/Shaders/Lit.shader");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
            var normalImporter = AssetImporter.GetAtPath(NormalPath) as TextureImporter;
            if (normalImporter == null) throw new InvalidOperationException("Noah normal-map importer is unavailable.");
            if (normalImporter.textureType != TextureImporterType.NormalMap)
            {
                normalImporter.textureType = TextureImporterType.NormalMap;
                normalImporter.SaveAndReimport();
            }
            Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material == null)
            {
                material = new Material(shader) { name = "Noah Mercer Runtime" };
                AssetDatabase.CreateAsset(material, MaterialPath);
            }
            else
            {
                material.shader = shader;
            }

            material.SetTexture("_BaseMap", RequireAsset<Texture2D>(BaseColorPath));
            material.SetTexture("_BumpMap", RequireAsset<Texture2D>(NormalPath));
            material.EnableKeyword("_NORMALMAP");
            material.SetFloat("_Smoothness", 0.18f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static AnimatorController BuildController(AnimationClip idle, AnimationClip walk)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            if (controller.layers.Length == 0) controller.AddLayer("Base Layer");
            controller.parameters = Array.Empty<AnimatorControllerParameter>();
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            AnimatorStateMachine machine = controller.layers[0].stateMachine;
            foreach (ChildAnimatorState state in machine.states) machine.RemoveState(state.state);
            AnimatorState idleState = machine.AddState("Idle");
            idleState.motion = idle;
            AnimatorState walkState = machine.AddState("Walk");
            walkState.motion = walk;
            walkState.speed = 1.2f;
            machine.defaultState = idleState;

            AnimatorStateTransition toWalk = idleState.AddTransition(walkState);
            toWalk.hasExitTime = false;
            toWalk.duration = 0.14f;
            toWalk.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
            AnimatorStateTransition toIdle = walkState.AddTransition(idleState);
            toIdle.hasExitTime = false;
            toIdle.duration = 0.18f;
            toIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
            EditorUtility.SetDirty(controller);
            return controller;
        }

        private static T RequireAsset<T>(string path) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            return asset != null ? asset : throw new FileNotFoundException($"Required character asset is missing: {path}");
        }
    }
}
