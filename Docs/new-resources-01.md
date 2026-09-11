# New Resources 01 Intake

## Scope

User-provided folder: `Assets/_InsideTheWalls/New Resources 01`. Originals stay unchanged. Optimized runtime derivatives and import reports belong under `Assets/_InsideTheWalls/Art/Environment/NewResources01`; Unity-ready prefabs belong under `Resources/Environment/NewResources01`.

- Bench, wall light, and both outdoor tiles: source meshes are about 400,000 triangles each; reduce before repeated runtime placement.
- Secure steel door, tall yard fence, and wall panel: lower-detail source geometry is suitable for a bounded import pass after dimensions/orientation checks.
- Duplicate `wall-panel-01_Textured_4723584732 (1)` has identical OBJ content; keep both originals, generate one runtime wall asset.
- New character #1 BLEND and USDZ files are preserved as source handoffs. Their presence is not proof of improved locomotion; do not replace the user's original rig or inmate #2 automatically.

## Splash

The attached ensemble artwork was copied unchanged to `Assets/_InsideTheWalls/Resources/UI/InsideTheWalls_EnsembleSplash.png`. It is used only for the splash, fitted without cropping. The existing menu art remains available.

SHA256: `5319816EAC8DBA91E4FE4994B04F220E6D560E3DD5FBFE7E3F8525B136F2E038`.

The artwork says **Inside The Bars**, while the project remains **Inside the Walls**. No product, repository or build rename is implied; title confirmation was requested from the user. No image editing or logo replacement was performed.

Update: the user then requested **Behind The Walls**. A separate edited image, `BehindTheWalls_EnsembleSplash.png`, now supplies the splash title; the original is retained. The edit prompt and inspection are recorded in `Docs/Art/behind-the-walls-splash-prompt.md`. Repository and save paths remain unchanged.

## Verification

Runtime import, geometry budgets, material checks and visible placement are recorded after the integration build. Until those checks run, these are source assets plus integration work, not a verified new release.

The outstanding asset requests are individually listed in `Docs/Art/graphics-npcs-combat-areas.md`; each of its 17 prompts is under 600 characters (longest: 497).
