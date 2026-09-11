# Animated Noah - Local Alpha

## Verification status - 2026-09-08

The Windows-NoahAlpha build compiled successfully with zero build warnings (196,254,583 bytes reported by Unity). It is **not yet animation-verified**. The opt-in hidden-player test passed avatar, skin, URP shader and idle-state checks, but measured zero leg motion and captured a black frame. This may involve hidden-window rendering/culling; the actual on-screen result must be checked before calling the update complete. Unity MCP setup is the next prerequisite for direct editor testing. Existing save and backup hashes remained unchanged.

## Play

Open `Builds/Windows-NoahAlpha/InsideTheWalls.exe`, then choose **New Game > Inmate**.
WASD / left stick moves; hold right mouse button / right stick to orbit; E / controller A interacts.
Noah uses a 1.8 m/s walk. Officer mode keeps the previous prototype character and movement.
The previous build in `Builds/Windows` is retained. Version remains v2.05 - Playable Alpha; nothing is published automatically.

## Asset integration

- `CharacterAssetBuilder` makes a separate runtime FBX copy, maps its Humanoid skeleton and caps imported skin weights at four.
- The original Blender file and source FBX are preserved.
- The generated prefab owns the URP material, normal-map import, valid Avatar and idle/walk controller.
- Idle and walking use local derivatives of the installed RPG Animations Pack clips. The raised weapon-arm curves are replaced with relaxed arms and a gentle swing; the original pack files are untouched.
- Controller and animation asset GUIDs survive regeneration.
- Noah replaces the capsule only when the visual passes rig/controller/skinned-mesh checks. Movement drives animation from collision-resolved displacement, including interaction suppression and wall stops.

## Rebuild and verify

The recorded build used Unity 6000.3.0f1. The user subsequently opened/upgraded the project in 6000.4.0f1; use the current ProjectVersion.txt for subsequent work. Keep ordinary package resolution enabled. Do **not** use `-noUpm`; that made package shaders unavailable in the previous attempt.

Batch entry point: `InsideTheWalls.Editor.FoundationBuild.ValidateAndBuildNoah`.
The regular `ValidateAndBuild` entry point still targets the older canonical `Builds/Windows` directory.

The development player accepts `-verifyNoah "<absolute evidence directory>"`. This starts the actual inmate flow with saving disabled, exercises real movement, checks retargeted bone motion, stopping, interaction suppression and wall collision, captures gameplay images, and exits with 0 on success or 2 on failure. The input override and verifier are excluded from non-development builds.

## Remaining scope

This is a single-character animated prototype, not a crowd-ready final character. The source-derived mesh is still approximately 400,000 triangles; production retopology/LODs and skinning polish remain. Officer animation, facial animation, hand props and dedicated interaction clips are separate work.

The RPG animation source remains covered by its Asset Store license; do not publish the raw package as open-source project assets.

## API reference

Clip adaptation uses Unity's [AnimationUtility.SetEditorCurve](https://docs.unity3d.com/ScriptReference/AnimationUtility.SetEditorCurve.html) editor API.
