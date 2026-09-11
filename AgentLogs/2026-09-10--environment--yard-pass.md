# Environment Yard Pass - 2026-09-10

Status: Implementation complete; Unity compile and visual verification delegated to lead integration.

## Completed

- Added two reusable, deterministic 64 x 64 mipmapped runtime textures: subtle concrete joints/grain for walls and walks, and asphalt aggregate for the existing yard slab.
- Reused the shared material and property-block flow; texture tiling follows surface dimensions. Texture generation uses a local System.Random so gameplay random state is unchanged.
- Added a compact recreation boundary and three exercise station marks in the unobstructed northern portion of the yard.
- Added back-wall coping and paired emissive bulkhead fixtures to each room. These are decorative primitives; no new Light components or blocking colliders.
- New textures are destroyed and their cache cleared alongside the existing material reset.

## Files

- Assets/_InsideTheWalls/Scripts/Presentation/FacilityVisualBuilder.cs
- AgentLogs/2026-09-10--environment--yard-pass.md

## Verification And Limits

- Read AGENTS.md and the complete builder before editing.
- git diff --check passed for the builder (only the repository line-ending notice).
- Source inspection confirms all existing Block coordinates and sizes are preserved, with only the yard surface color changed. Additions use DecorativeBlock, which removes its primitive collider.
- No Unity, Blender, image generation, packages, character files, or external assets used.
- No runtime or screenshot result claimed. Lead must check URP texture/property-block appearance, paint visibility, and compilation. Cube UVs reuse coordinates on all faces, so the narrow edge grain can stretch; broad faces are the intended viewing surfaces.
- The working file already contained edits before this assignment; the git diff statistics include those changes.

## Suggested Views

- From (0, 2, -0.5), look south into the yard: asphalt, recreation paint, benches and exercise rail.
- From (-13, 2, 3), look north into Housing A: concrete grain, rear coping, and paired wall fixtures.
- From (10, 2, 0), look west along the main walk: repeated surface scale and uninterrupted routes.

## Decisions

- Kept surface breakup restrained and neutral for an institutional facility; safety yellow remains reserved for existing movement guidance.
- Used small generated textures and existing emissive material behavior to improve surface readability without adding realtime lighting cost.

## New Resources 01 Integration

- Audited all eight OBJ folders, including triangle/UV counts, bounds and SHA256. The duplicate wall OBJ is identical and remains intact. Character blend/USDZ files were inventoried by name only.
- Created Tools/EnvironmentPipeline/import_new_resources.py and seven derived FBX meshes, copied original base-color textures, source/runtime JSON audits and 512px silhouette previews under Art/Environment/NewResources01.
- Blender batch completed with exit 0. Runtime triangles: Bench 7200, WallLight 7199, OutdoorTileA 1152, OutdoorTileB 1152, SecureDoor 4319, YardFence 4320, WallPanel 4320. All meshes retain UVs.
- Inspected all seven previews: bench, light, door and panels stand upright; supplied upright tile assets are rotated 90 degrees onto the ground. Uniform dimension normalization and bottom-center pivots preserve proportions. Bench runtime scale 0.62 keeps its 0.641m depth within the original 0.65m collision footprint.
- Added EnvironmentAssetBuilder.EnsureAssets() to import meshes without colliders/lights/animation, create owned URP Lit materials and seven Resources prefabs. The lead invokes this in the Unity build; this agent did not launch Unity.
- Wired imported benches and bulkhead fixtures in place of matching primitives. Added a four-tile apron, back-wall panel details, fixed officer-station service-door dressing, and north-perimeter rail details. Existing collision objects and controlled door remain unchanged.
- Tiles are visually sunk into the existing slab with their tops at 0.12m; no additional collision geometry. Their scanned edges remain irregular. The low-resolution fence is rail-like and is decorative in front of the existing secure perimeter, not a replacement for its blocking surface.
- First Blender startup loaded installed add-ons before factory reset, producing unrelated add-on warnings. The batch itself succeeded; documented repeat invocation uses --factory-startup to avoid loading those add-ons.
- Unity import/material/prefab verification and in-game screenshots remain with the lead. No claim of an animated imported door or a full architecture replacement.
