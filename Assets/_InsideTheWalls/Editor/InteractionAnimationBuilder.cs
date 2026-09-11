using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace InsideTheWalls.Editor
{
    public static class InteractionAnimationBuilder
    {
        private const string CharacterRoot = "Assets/_InsideTheWalls/Art/Characters/InmateTwo/";
        private const string Output = "Assets/_InsideTheWalls/Resources/Characters/InmateTwoAnimations/";
        public const string ControllerPath = Output + "InmateTwo.controller";
        private const string ImportVersion = "ITW.Interaction.Humanoid.v1";

        public static void EnsureAssets()
        {
            Directory.CreateDirectory(CharacterRoot + "Animations/Source");
            Directory.CreateDirectory(Output);
            AssetDatabase.Refresh();
            var clips = new Dictionary<string, AnimationClip>();
            foreach (string name in new[] { "Catwalk Walk Forward 01", "Disappointed", "Fighting Idle", "Shove Reaction", "Standing Arguing", "Unarmed Idle 01", "Wheelbarrow Dump" })
                clips.Add(name, ImportClip(name, name == "Catwalk Walk Forward 01" || name == "Fighting Idle" || name == "Unarmed Idle 01"));
            BuildController(clips);
            AssetDatabase.SaveAssets();
            Debug.Log("INTERACTION_ANIMATIONS_OK seven validated humanoid clips; Speed/Guard/Talk/Reaction controller");
        }

        private static AnimationClip ImportClip(string name, bool loop)
        {
            string sourcePath = CharacterRoot + "Runtime/" + name + ".fbx";
            string derivedPath = CharacterRoot + "Animations/Source/" + name + ".fbx";
            if (!File.Exists(sourcePath)) throw new FileNotFoundException("Missing supplied animation", sourcePath);
            if (!File.Exists(derivedPath) && !AssetDatabase.CopyAsset(sourcePath, derivedPath))
                throw new IOException("Could not copy animation source: " + sourcePath);
            var importer = AssetImporter.GetAtPath(derivedPath) as ModelImporter;
            if (importer == null) throw new InvalidOperationException("Animation ModelImporter unavailable: " + derivedPath);
            if (importer.userData != ImportVersion)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.sourceAvatar = null;
                importer.importAnimation = true;
                importer.importCameras = false;
                importer.importLights = false;
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                // CreateFromThisModel lets Unity auto-map each supplied animation skeleton.
                importer.SaveAndReimport();
                ModelImporterClipAnimation[] takes = importer.defaultClipAnimations;
                if (takes.Length != 1) throw new InvalidOperationException($"Expected one take in {name}, found {takes.Length}.");
                takes[0].name = name;
                takes[0].loopTime = loop;
                takes[0].loopPose = loop;
                takes[0].lockRootRotation = true;
                takes[0].lockRootHeightY = true;
                takes[0].lockRootPositionXZ = true;
                importer.clipAnimations = takes;
                importer.SaveAndReimport();
            }
            Avatar avatar = AssetDatabase.LoadAllAssetsAtPath(derivedPath).OfType<Avatar>().FirstOrDefault();
            if (avatar == null || !avatar.isValid || !avatar.isHuman)
                throw new InvalidOperationException("Invalid humanoid avatar: " + derivedPath);
            AnimationClip[] imported = AssetDatabase.LoadAllAssetsAtPath(derivedPath).OfType<AnimationClip>()
                .Where(clip => !clip.name.StartsWith("__preview__", StringComparison.Ordinal)).ToArray();
            if (imported.Length != 1 || !imported[0].humanMotion || imported[0].length <= 0f)
                throw new InvalidOperationException("Expected one non-empty humanoid animation: " + derivedPath);
            string outputPath = Output + name + ".anim";
            AnimationClip result = AssetDatabase.LoadAssetAtPath<AnimationClip>(outputPath);
            if (result == null)
            {
                result = UnityEngine.Object.Instantiate(imported[0]);
                AssetDatabase.CreateAsset(result, outputPath);
            }
            else EditorUtility.CopySerialized(imported[0], result);
            result.name = name;
            EditorUtility.SetDirty(result);
            importer.userData = ImportVersion;
            AssetDatabase.WriteImportSettingsIfDirty(derivedPath);
            Debug.Log($"INTERACTION_CLIP_OK name={name} seconds={result.length:F3} humanMotion={result.humanMotion} loop={loop} avatarValid={avatar.isValid}");
            return result;
        }

        private static void BuildController(Dictionary<string, AnimationClip> clips)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            controller.parameters = Array.Empty<AnimatorControllerParameter>();
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Guard", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Talk", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Reaction", AnimatorControllerParameterType.Trigger);
            while (controller.layers.Length < 3) controller.AddLayer(controller.layers.Length == 1 ? "Upper Body" : "Reaction");
            AnimatorControllerLayer[] layers = controller.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                AnimatorStateMachine machine = layers[i].stateMachine;
                foreach (AnimatorStateTransition transition in machine.anyStateTransitions) machine.RemoveAnyStateTransition(transition);
                foreach (ChildAnimatorState state in machine.states) machine.RemoveState(state.state);
                layers[i].defaultWeight = 1f;
                layers[i].blendingMode = AnimatorLayerBlendingMode.Override;
            }
            layers[0].name = "Locomotion";
            layers[1].name = "Upper Body";
            layers[1].avatarMask = BuildUpperBodyMask();
            layers[2].name = "Reaction";
            layers[2].avatarMask = null;
            controller.layers = layers;

            AnimatorStateMachine locomotion = layers[0].stateMachine;
            AnimatorState idle = State(locomotion, "Idle", clips["Unarmed Idle 01"]);
            AnimatorState walk = State(locomotion, "Walk", clips["Catwalk Walk Forward 01"]);
            walk.speedParameter = "Speed";
            walk.speedParameterActive = true;
            walk.speed = 1f;
            locomotion.defaultState = idle;
            Transition(idle, walk).AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
            Transition(walk, idle).AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");

            AnimatorStateMachine upper = layers[1].stateMachine;
            AnimatorState empty = State(upper, "Empty", null);
            AnimatorState guard = State(upper, "Guard", clips["Fighting Idle"]);
            AnimatorState talk = State(upper, "Talk", clips["Standing Arguing"]);
            upper.defaultState = empty;
            Transition(empty, guard).AddCondition(AnimatorConditionMode.If, 0f, "Guard");
            Transition(guard, empty).AddCondition(AnimatorConditionMode.IfNot, 0f, "Guard");
            Transition(empty, talk).AddCondition(AnimatorConditionMode.If, 0f, "Talk");
            Transition(guard, talk).AddCondition(AnimatorConditionMode.If, 0f, "Talk");
            Transition(talk, empty, true);

            AnimatorStateMachine reactions = layers[2].stateMachine;
            AnimatorState noReaction = State(reactions, "Empty", null);
            AnimatorState reaction = State(reactions, "Shove Reaction", clips["Shove Reaction"]);
            reactions.defaultState = noReaction;
            Transition(noReaction, reaction).AddCondition(AnimatorConditionMode.If, 0f, "Reaction");
            Transition(reaction, noReaction, true);
            EditorUtility.SetDirty(controller);
        }

        private static AvatarMask BuildUpperBodyMask()
        {
            string path = Output + "UpperBody.mask";
            AvatarMask mask = AssetDatabase.LoadAssetAtPath<AvatarMask>(path);
            if (mask == null)
            {
                mask = new AvatarMask { name = "InmateTwo Upper Body" };
                AssetDatabase.CreateAsset(mask, path);
            }
            for (int i = 0; i < (int)AvatarMaskBodyPart.LastBodyPart; i++) mask.SetHumanoidBodyPartActive((AvatarMaskBodyPart)i, false);
            foreach (AvatarMaskBodyPart part in new[] { AvatarMaskBodyPart.Body, AvatarMaskBodyPart.Head, AvatarMaskBodyPart.LeftArm, AvatarMaskBodyPart.RightArm, AvatarMaskBodyPart.LeftFingers, AvatarMaskBodyPart.RightFingers })
                mask.SetHumanoidBodyPartActive(part, true);
            EditorUtility.SetDirty(mask);
            return mask;
        }

        private static AnimatorState State(AnimatorStateMachine machine, string name, Motion motion)
        {
            AnimatorState state = machine.AddState(name);
            state.motion = motion;
            state.writeDefaultValues = false;
            return state;
        }

        private static AnimatorStateTransition Transition(AnimatorState from, AnimatorState to, bool exit = false)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            transition.hasExitTime = exit;
            transition.exitTime = 0.95f;
            transition.hasFixedDuration = true;
            transition.duration = 0.15f;
            return transition;
        }
    }
}
