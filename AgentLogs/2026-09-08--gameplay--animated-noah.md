# Animated Noah: Runtime Gameplay Integration

## Conversation

- Lead Producer -> Gameplay specialist: Finish runtime integration in PrototypePlayerController.cs and PlayableDayController.cs only. Drive animation from actual horizontal movement, use approximately 1.8 m/s for animated Noah, preserve the officer's movement, and retain the capsule when Noah's rig is invalid. Do not launch Unity or change version/build assets.
- Gameplay specialist -> Lead Producer: Confirmed the scope. Unity integration guidance read. The lead owns compilation and rendered verification; no reviewer or child agents are involved.
- Gameplay specialist -> Lead Producer: Implemented the runtime changes below. Await the lead's compilation, collision, and visual verification before treating the update as a verified playable build.
- Lead Producer -> Gameplay specialist: Add an internal nullable Vector2 movement override in editor/development builds so the standalone smoke test can exercise the actual movement, collision, and animation path without OS input.
- Gameplay specialist -> Lead Producer: Added VerificationMoveInput behind UNITY_EDITOR || DEVELOPMENT_BUILD. Null preserves normal input; override input is still clamped and obeys interaction suppression. The override is absent from release builds.

## Changes

- Actual post-CharacterController.Move horizontal displacement supplies the Animator's normalized Speed value; a blocked player returns to idle instead of walking against a wall.
- Interaction suppression and teleporting reset Speed to idle immediately.
- A valid animated visual selects 1.8 m/s walking. The officer and missing/invalid-character fallback preserve 4.5 m/s movement.
- The capsule remains visible unless Noah has an enabled Animator, valid Humanoid Avatar, runtime controller, and an enabled skinned mesh with bones. Warnings explain what needs rebuilding.
- Noah's feet anchor matches the CharacterController's bottom. Camera, objectives, authority, save/load state, and resource paths are unchanged.
- Editor/development-only VerificationMoveInput supports deterministic smoke-test movement through the existing CharacterController and animation logic.

## Verification And Limits

- Scoped source inspection and whitespace checks performed; Unity compilation and live movement/rendering checks are assigned to the lead.
- Runtime integration assumes the generated controller provides its existing float Speed parameter and the prefab's origin is at its feet. The lead's character builder owns those asset contracts.
- No Unity editor launch, package change, version bump, Git commit, or GitHub push performed.
