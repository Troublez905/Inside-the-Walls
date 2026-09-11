# Inside the Walls Asset Inventory

Audit date: 2026-09-06  
Unity target: 6000.3.0f1, URP 17.0.1  
Status: Reference intake only. Ownership and production rights are not yet recorded.

## Classification Rules

- **Reference:** flattened concept or character sheet; do not ship or use as a texture source.
- **Placeholder:** may be used temporarily in the alpha after rights confirmation and a visual check.
- **Production candidate:** source data exists, but it still requires import, optimization, material, rig, and performance validation.
- Recommended paths are destinations for a later controlled organization pass. This audit did not move or rename anything.

## Next-Needed Graphics Intake (2026-09-06)

Fourteen boards matching `graphics-next-needed.md` were delivered under `Docs/Art/` and copied to `Assets/_InsideTheWalls/` for Unity visibility. **All remain reference boards.** They are not separable tileable PBR maps, FBX modules, animation clips, or production UI sprites.

| Current file | Dimensions / format | Maps to request | Classification | Recommended path |
|---|---:|---|---|---|
| `Painted Concrete-Block Wall Material.png` | 1448x1086 PNG RGB | #1 wall material | Reference | `Art/Materials/Environment/Reference/REF_PaintedConcreteBlockWall.png` |
| `Institutional Vinyl Floor Material.png` | 1448x1086 PNG RGB | #2 floor material | Reference | `Art/Materials/Environment/Reference/REF_InstitutionalVinylFloor.png` |
| `Modular Interior Wall Segment.png` | 1448x1086 PNG RGB | #3 wall module | Reference | `Art/Environments/Modular/Reference/REF_InteriorWallSegment.png` |
| `Secure Interior Door.png` | 1448x1086 PNG RGB | #4 secure door | Reference | `Art/Environments/Doors/Reference/REF_SecureInteriorDoor.png` |
| `Intake Property Bin.png` | 1448x1086 PNG RGB | #5 property bin | Reference | `Art/Props/Intake/Reference/REF_PropertyBin.png` |
| `Laundry Cart.png` | 1536x1024 PNG RGB | #6 laundry cart | Reference | `Art/Props/Laundry/Reference/REF_LaundryCart.png` |
| `Fixed Dining Table.png` | 1448x1086 PNG RGB | #7 dining table | Reference | `Art/Props/Dining/Reference/REF_FixedDiningTable.png` |
| `Yard Exercise Rail.png` | 1536x1024 PNG RGB | #8 exercise rail | Reference | `Art/Props/Yard/Reference/REF_ExerciseRail.png` |
| `Objective Interaction Marker.png` | 2172x724 PNG RGBA | #9 objective marker | Reference | `Art/UI/Gameplay/Reference/REF_ObjectiveMarkerStates.png` |
| `Schedule Phase Icon Official Count.png` | 1448x1086 PNG RGB | #10 count icon | Reference | `Art/UI/Schedule/Reference/REF_CountPhaseIcon.png` |
| `Optimized Noah Mercer Runtime Body.png` | 1448x1086 PNG RGB | #11 Noah runtime body | Reference | `Art/Characters/Principals/NoahMercer/Reference/REF_OptimizedRuntimeBody.png` |
| `Noah Mercer Idle Animation.png` | 1536x1024 PNG RGB | #12 idle animation | Reference | `Art/Characters/Principals/NoahMercer/Reference/REF_IdleAnimation.png` |
| `Noah Mercer Walk Cycle.png` | 1672x941 PNG RGB | #13 walk cycle | Reference | `Art/Characters/Principals/NoahMercer/Reference/REF_WalkCycle.png` |
| `Probationary Officer Runtime Character.png` | 1448x1086 PNG RGB | #14 officer runtime | Reference | `Art/Characters/Principals/LenaOrtiz/Reference/REF_ProbationaryRuntime.png` |

### Runtime response already applied

- `FacilityVisualBuilder` now samples the board palette and proportions: blue-green walls with dark base trim, vinyl floor color, stacked property bins, laundry carts with liner/load, stainless dining tables with stools, secure door leaf with safety stripe/vision panel, and yard rail with yellow grips.
- Objective markers use a procedural bracket/ring silhouette inspired by request #9 without cropping the composite sheet.
- True production delivery still requires separated maps/meshes/clips, rights records, and scene performance validation.

## Image Inventory

| Current file under `Assets/_InsideTheWalls/` | Dimensions / format | Classification and register mapping | Recommended path | Notes |
|---|---:|---|---|---|
| `Alpha Gray-Box Material Set.png` | 1448x1086 PNG RGB | Reference; #6 gray-box materials | `Art/Materials/Graybox/Reference/REF_GrayboxMaterialBoard.png` | Not tileable PBR maps; all labels and swatches are baked together. |
| `Core Animation Reference Set.png` | 1536x1024 PNG RGB | Reference; #19 animation | `Art/Characters/Animations/Reference/REF_CoreAnimationBoard.png` | Strong coverage of locomotion, work, conversation, escort, radio, de-escalation, surrender, defensive, rest, eating, exercise, search, treatment, daily life, and transitions. It is poses only, not animation clips. |
| `Department Prop Library.png` | 1536x1024 PNG RGB | Reference; #21 department props | `Art/Props/Departments/Reference/REF_DepartmentPropLibrary.png` | Concept-completes the department coverage and includes scale, state, collision, and interaction-anchor examples. It contains no separable meshes, textures, collision, or prefabs. |
| `Weather, Alarm, And Injury VFX.png` | 1536x1024 PNG RGB | Reference; #22 VFX | `Art/VFX/Reference/REF_WeatherAlarmInjuryVFX.png` | Concept-completes normal and reduced-effects variants. The pictured flipbooks and decals are baked into an opaque sheet, not transparent production sequences or Unity prefabs. |
| `Additional Facility Concepts.png` | 1536x1024 PNG RGB | Reference; #23 expansion facilities | `Art/Expansion/Reference/REF_AdditionalFacilityConcepts.png` | Concept-completes intake/transfer, minimum, medium, high, and administrative/medical studies. Reference only until the vertical-slice performance gate passes. |
| `Dining Hall And Kitchen Service Edge.png` | 1536x1024 PNG RGB | Reference; #11 and #21 | `Art/Environments/Rooms/Dining/Reference/REF_DiningKitchen.png` | Useful furniture vocabulary; no scale-certified meshes or clean prop cutouts. |
| `Housing Unit And Cell.png` | 1536x1024 PNG RGB | Reference; #10 and #21 | `Art/Environments/Rooms/Housing/Reference/REF_HousingCell.png` | Useful dayroom/cell composition; signage and all prop views are baked. |
| `Intake And Processing Room.png` | 1536x1024 PNG RGB | Reference; #9 and #21 | `Art/Environments/Rooms/Intake/Reference/REF_IntakeProcessing.png` | Good P0 room/prop reference; rebuild dimensions on the 1 m grid. |
| `Loading Screen And Indicator.png` | 1672x941 PNG RGB | Reference/placeholder; #4 | `Art/UI/Loading/Reference/REF_LoadingScreenBoard.png` | Indicator frames cannot be used independently without source exports. |
| `Logo And Title Lockup.png` | 1536x1024 PNG RGBA | Reference/source candidate; #1 | `Art/UI/Brand/Source/SRC_LogoTitleBoard.png` | Contains alpha but remains a composite board. Obtain separate SVG/transparent variants before production use. |
| `Low-Security Facility Layout Concept.png` | 1672x941 PNG RGB | Reference; #8 | `Art/Environments/Facility/Concept/REF_LowSecurityCampus.png` | Scope is larger than the first slice; use landmarks and zone language, not the depicted layout as a construction plan. |
| `Main Menu UI Kit.png` | 1536x1024 PNG RGB | Reference; #2 | `Art/UI/Frontend/Reference/REF_MainMenuKit.png` | No actual 9-slices, icons, or scalable controls; baked text and color-only state risks. |
| `Modular Character And Uniform Set` | 1536x1024 PNG data without extension | Invalid duplicate | N/A | Byte-identical to the `.png` file (SHA-256 `611D24A2...E3AB58`). Do not import both. |
| `Modular Character And Uniform Set.png` | 1536x1024 PNG RGB | Reference; #18 | `Art/Characters/Modular/Reference/REF_CharacterUniformMatrix.png` | Broad role coverage; not meshes, shared rig, garments, materials, or LODs. |
| `modular environment set.png` | 1536x1024 PNG RGB | Reference; #7 | `Art/Environments/Modular/Reference/REF_ModularEnvironmentDimensions.png` | Best P0 modular construction reference because it specifies 1/2/4 m bays, 3 m height, collisions, and pivots. |
| `Modular Prison Architecture Kit.png` | 1448x1086 PNG RGB | Reference; #7 | `Art/Environments/Modular/Reference/REF_ArchitectureKitVisuals.png` | Complements the dimensioned board visually; redundant subject matter and no production geometry. |
| `Principal Character - Noah Mercer.png` | 1536x1024 PNG RGB | Reference; #15 | `Art/Characters/Principals/NoahMercer/Reference/REF_NoahMercerSheet.png` | Best-defined inmate identity; not the requested 4K master or a model. |
| `Principal Character - Officer Lena Ortiz.png` | 1536x1024 PNG RGB | Reference; #16 | `Art/Characters/Principals/LenaOrtiz/Reference/REF_LenaOrtizSheet.png` | Best-defined playable officer identity; no corresponding 3D model yet. |
| `Remaining Facility Rooms.png` | 1536x1024 PNG RGB | Reference; #13 and #21 | `Art/Environments/Rooms/Shared/Reference/REF_RemainingRooms.png` | Useful P1 scope board; each room still needs its own layout and prop source. |
| `Role Selection Portraits.png` | 1254x1254 PNG RGB | Placeholder; #5 | `Art/UI/Frontend/RoleSelection/Reference/REF_RolePortraitStates.png` | Opaque 2x2 composite with baked borders; identities do not match the approved principal sheets consistently. |
| `Text-Free Splash Background.png` | 1672x941 PNG RGB | Placeholder candidate; #1/#3 | `Art/UI/Frontend/Backgrounds/TEX_Splash_LowSecurity_Dusk.png` | Best immediately usable UI image after rights confirmation, but below 4K and tonally darker than the low-security brief. |
| `Yard And Officer Station.png` | 1536x1024 PNG RGB | Reference; #12 and #21 | `Art/Environments/Rooms/Yard_OfficerStation/Reference/REF_YardOfficerStation.png` | Strong P0 sightline and prop board; rebuild rather than crop assets from it. |
| `captain-elias-ward-sheet-01.png` | 1448x1086 PNG RGB | Reference; #17 | `Art/Characters/Principals/EliasWard/Reference/REF_EliasWardSheet.png` | Complete concept coverage, but below 4K and stylistically needs alignment with Lena. |
| `character lineup.png` | 1536x1024 PNG RGB | Reference; #18 | `Art/Characters/Modular/Reference/REF_RoleLineup.png` | Useful scale/role overview; overlaps other modular character boards. |
| `character-concepts.png` | 1536x1024 PNG RGB | Reference; #18 | `Art/Characters/Modular/Reference/REF_CharacterConcepts.png` | Exploration only; officer equipment and character continuity vary. |
| `character-inmate-01-T.png` | 1024x1536 PNG RGBA | Reference; #15/#18 | `Art/Characters/Principals/NoahMercer/Reference/REF_Noah_TPoseFront.png` | Matches Noah most closely; raster T-pose is not rig/model data. |
| `character-inmate-02-T.png` | 1024x1536 PNG RGBA | Reference; #18 | `Art/Characters/Modular/Reference/REF_InmateBroad_TPose.png` | Best visual match for textured model 02; identity is unassigned. |
| `character-inmate-03-T.png` | 1024x1536 PNG RGBA | Reference; #18 | `Art/Characters/Modular/Reference/REF_InmateAthletic_TPose.png` | Best visual match for textured model 03; identity is unassigned. |
| `character-sheet-inmate01.png` | 1448x1086 PNG RGB | Reference; #15 | `Art/Characters/Principals/NoahMercer/Reference/REF_NoahTurnaround.png` | Redundant with the stronger named Noah sheet but useful for modeling views. |
| `character-sheet-inmate02.png` | 1448x1086 PNG RGB | Reference; #18/#19 | `Art/Characters/Modular/Reference/REF_InmateBroad_CompliancePose.png` | Pose-specific sheet, not a neutral modeling turnaround. |

## Character Model Inventory

| Current source | Contents / measured complexity | Readiness | Recommended path | Required work |
|---|---|---|---|---|
| `Character Models/character-inmate-01-T_Textured_4652083415/` | OBJ 26.9 MB; V 200,000; VT 217,053; VN 0; F 399,998; one MTL; one 2048x2048 RGB diffuse texture | Production candidate, **not runtime-ready** | `Art/Characters/Principals/NoahMercer/Source/High/` | Best identity candidate for Noah. Retopologize, generate normals/tangents, audit UVs/materials, create normal/mask maps, rig/skin, add a facial solution, LODs and collider, then benchmark. |
| `Character Models/character-inmate-02-T_Textured_4652183416/` | OBJ 27.0 MB; V 199,540; VT 226,099; VN 0; F 399,131; one MTL; one 2048x2048 RGB diffuse texture | Production candidate, **not runtime-ready** | `Art/Characters/Modular/Inmates/InmateBroad/Source/High/` | Highest immediate value as an NPC body reference. Same retopo/normal/rig/material/LOD work; user must assign identity. |
| `Character Models/character-inmate-03-T_Textured_4652283417/` | OBJ 27.1 MB; V 199,828; VT 228,244; VN 0; F 399,778; one MTL; one 2048x2048 RGB diffuse texture | Production candidate, **not runtime-ready** | `Art/Characters/Modular/Inmates/InmateAthletic/Source/High/` | Same optimization pipeline; visually too close to model 02 to count as broad modular diversity by itself. |
| `Character Models/character-inmate-01-T_Rigged_4652083419/.../character_publish.blend` | Blender file header `BLENDER-v306`, 64.7 MB | Best technical source candidate, unverified | `Art/Characters/Principals/NoahMercer/Source/Rig/character_publish.blend` | Preferred starting point because it may preserve skeleton/skin data. Blender 5.0 is installed outside PATH, but an attempted read did not complete within the audit window; bones, weights, clips, scale, topology, and materials remain unverified. The Blender 4.5 comment belongs to the separate MTL exports and does not establish this file's version. |
| `Character Models/character-inmate-01-T_Rigged_4652083419/.../character.usdz` | USDZ, 43.3 MB | Interchange candidate, Unity import unverified | `Art/Characters/Principals/NoahMercer/Source/Interchange/character.usdz` | Convert through an approved DCC to FBX/glTF; do not rely on direct Unity USDZ support. Validate axis, meters, blendshapes, skeleton, and textures. |
| `Character Models/character-inmate-01-T_Rigged_4652083419/.../fscharacterpublish_animated.usdz` | Animated USDZ, 43.4 MB | Animation source candidate, Unity import unverified | `Art/Characters/Principals/NoahMercer/Source/Interchange/fscharacterpublish_animated.usdz` | Determine clip names/ranges and licensing, then export humanoid FBX clips. It does not replace the #19 animation set without motion verification. |

The three OBJ assets are approximately 400,000 polygon faces each and are far beyond a sensible repeated-character budget for an 8-12 player slice, let alone the 50-player target plus AI. They also provide only a baked diffuse map: no normal, mask, roughness, AO, separate eye/hair materials, authored collision, LODs, or verified skeleton.

## Best Current Candidates

1. **P0 environment construction:** `modular environment set.png`, supported by `Modular Prison Architecture Kit.png`.
2. **P0 room loop:** intake, housing, dining, and yard/officer-station boards in that order.
3. **P0 character source experiment:** Noah/model 01 `character_publish.blend`, isolated in a test scene after DCC inspection; never drop the raw OBJ into the main scene.
4. **P1 frontend:** `Text-Free Splash Background.png`; use only as a temporary background after rights confirmation.
5. **P1 identity references:** named Noah, Lena, and Elias sheets. Lena currently lacks a model; Marcus Vale is still absent/unresolved.
6. **P1 animation planning:** `Core Animation Reference Set.png`; select only locomotion, interaction, conversation, radio, work, and sit clips for the first pass.

## Integration Shortlist

### P0: playable gray-box

1. Rebuild a metric 1 m modular kit with primitive URP materials; do not texture from the reference-sheet pixels.
2. Assemble intake -> secure corridor -> housing/dayroom -> dining -> yard and compact officer station.
3. Add simple collision, role-gated doors, count/assignment markers, interaction anchors, and navigation surfaces.
4. Validate camera clearance, movement widths, officer sightlines, both role objectives, and performance before detail art.
5. Inspect model 01 in Blender, export one optimized humanoid FBX, and test it against the existing player controller in an isolated scene.

### P1: vertical-slice presentation

1. Create separate brand/UI exports, 9-slices, icons, focus states, and responsive role portraits.
2. Author reusable room props with shared materials and simple collision.
3. Produce a minimal animation pack from the reference board with in-place locomotion and clean loops.
4. Add optimized Lena and supporting NPC models only after the shared skeleton and performance budget are proven.
5. Produce true tileable PBR material sets and representative lighting; then profile in the playable scene.

## Missing Or Invalid Deliverables

The graphics register is now **concept-complete for items #1-#23**: every requested group has at least one visual reference board. This does not mean production-complete. The final three additions close the concept mapping for department props, VFX, and expansion facilities only.

- No separated SVG/transparent logo family, favicon, real 9-slice UI kit, or independent role portraits.
- No tileable Base Color/Normal/Mask material sets.
- No production modular environment meshes, prefabs, collision, UVs, or LODs.
- No optimized gameplay character, verified Unity humanoid avatar, shared skeleton, blendshapes, or character LODs.
- No 3D Lena Ortiz, Elias Ward, or clearly identified Marcus Vale. Do not silently rename an unnamed inmate as Marcus.
- No Unity-ready animation clips despite the new reference board/animated USDZ.
- No production gameplay UI system (#20), department prop meshes/prefabs (#21), transparent VFX assets/prefabs (#22), or production expansion assets (#23). Their reference concepts are present.
- The extensionless `Modular Character And Uniform Set` is an invalid duplicate and should remain excluded from import.

## Import And Rights Risks

- Provenance, commercial rights, generative-model terms, and source licenses are unknown for every new asset.
- Concept sheets contain baked text and inconsistent slogans; they are not reusable UI or decal sources.
- Model scale, orientation, pivots, mesh integrity, UV overlap, bone hierarchy, weights, animation clips, and material links remain unverified.
- Importing the raw 400k-face OBJs into Unity will increase repository/import time and can cause severe runtime cost if instantiated.
- Keep high-poly DCC sources outside runtime-loaded folders; never place them under `Resources`.
- Generate `.meta` files only in one controlled Unity import pass after final paths are approved.

## Verification Performed

- Enumerated the complete current `Assets/_InsideTheWalls` tree and classified all new art files.
- Visually inspected every concept/reference PNG, including the late-added animation board.
- Verified image dimensions, RGB/RGBA formats, file sizes, and the exact duplicate hash.
- Parsed OBJ records to measure positions, UVs, normals, and polygon faces.
- Read the MTLs and verified each references one `BakedTexture.png`; each texture is 2048x2048 RGB.
- Rechecked OBJ record types directly: all three have zero `vn` records; the approximately 399k values are polygon-face counts.
- Verified `character_publish.blend` begins with `BLENDER-v306`. Blender 5.0 is installed at `C:/Program Files/Blender Foundation/Blender 5.0/blender.exe` but is not on PATH; the attempted background inspection did not complete within the audit window.
- Verified the final three boards are 1536x1024, 24-bit RGB PNG files and recorded their SHA-256 hashes in the final-intake log.
- Did not move, rename, delete, import, commit, or modify any supplied asset.
