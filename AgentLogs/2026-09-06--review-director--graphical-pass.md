# Graphical Pass Review

Date: 2026-09-06
Role: Visual Quality and Performance Reviewer
Status: Conditional acceptance - visual-state defect corrected; runtime performance and final build evidence pending

## Scope

Independent review of the playable facility, UI, runtime-created materials, lighting, shader compatibility, accessibility, visual cohesion, and the graphics request catalog. This reviewer makes no production changes outside this report.

## Acceptance Gates

1. **Collision and traversal:** Every objective remains reachable with the existing character controller and 2.6 m interaction range. Decorative geometry must not close routes, trap the player, or obstruct the follow camera. Controlled gates and room openings must remain visibly and physically coherent.
2. **Material discipline:** Runtime materials are shared and bounded, never created per frame. The facility pass should remain at or below 12 shared environment materials unless a measured exception is documented. Repeated props must not use renderer `.material` mutations that silently clone materials.
3. **URP compatibility:** All runtime scene surfaces resolve to URP-compatible shaders in the configured Unity 6 / URP 17 pipeline. Missing shaders produce an explicit error; the pass must not depend on Built-in `Standard`.
4. **Rendering cost:** Decorative additions must avoid transparent-layer overdraw, realtime point/spot-light proliferation, and needless unique meshes/materials. Static non-interactive environment should be eligible for batching. Approval requires a representative runtime measurement or, if tooling is unavailable, an explicit unmeasured-risk note.
5. **HUD and menu readability:** Core objective, location, time, interaction prompt, save state, and choice controls remain readable at 16:9 and resizable-window layouts. Text must not overlap, clip, or rely on color alone. Selected, disabled, warning, and objective states require shape, label, or icon support.
6. **Visual hierarchy:** Gameplay landmarks and active objectives must read before ambient decoration. Orange and safety yellow remain restrained navigational accents rather than broad surface colors.
7. **Cohesion:** The environment consistently communicates a fictional low-security institution that is worn but maintained, with concrete gray, desaturated blue-green, dark metal, muted wood, and controlled warm accents. No real facility, agency, or brand is reproduced.
8. **Regression gate:** Unity compiles with zero errors, the Windows player build succeeds, and a fresh new game plus Continue flow completes without a visual or traversal regression.

## Baseline Findings

- The project correctly targets Universal Render Pipeline 17.0.1 and assigns separate PC and Mobile URP assets.
- The existing facility builder caches materials by shader and RGBA value, avoiding one material per object in the current static build. Its shader fallback chain stays inside URP before a sprite fallback; the final fallback is unsuitable for lit 3D production and should remain an emergency/error path only.
- Current geometry is runtime primitive-heavy. This is acceptable for a prototype, but draw calls, batches, triangles, shadow casters, and runtime material count are not yet captured in a representative playable frame.
- The existing room shells omit front walls and use simple cube colliders. Any decorative pass must protect entrances, objective marker visibility, and camera clearance rather than treating the concept art as a literal layout.
- PC quality currently uses two pixel lights, 40 m shadow distance, two cascades, four skin weights, and no MSAA. Added local lights or fine high-contrast edges could expose aliasing and should be justified visually and measured.
- Texture streaming is disabled. Large concept/source textures must not be loaded into the playable scene as environment surfaces without import-size and memory review.

## Pending Review

- Audit the environment pass diff and test evidence.
- Verify material/shader counts and check for hidden material instancing.
- Check facility traversal/camera clearance and UI at representative resolutions.
- Review `Docs/Art/graphics-next-needed.md` for duplicate requests, individually listed entries, and prompts shorter than 600 characters.

## Implementation Review

### Findings

1. **P1 - Material palette exceeds the review target.** Static inspection identifies approximately 16 distinct cached runtime materials: seven named palette colors plus ground, walk, five room/yard surface colors, movement-board lines, and machine-disc color. Sharing is correctly implemented with `sharedMaterial`, so this is bounded rather than a leak, but it exceeds the 12-material prototype target and has no runtime batch/draw-call measurement. Consolidate the near-duplicate green/gray surface colors or capture a representative frame proving the exception is harmless.
2. **P1 - Final build evidence is incomplete.** `Logs/environment-graphical-pass-build.log` stops during Package Manager registration and contains no successful build completion. This log does not prove compilation or player-build success for the graphical pass.
3. **P2 - Rendering cost remains unmeasured.** The pass adds many separate primitive renderers, five emissive fixture meshes, and two shadowless realtime point lights. No transparent materials were introduced and local-light shadows are disabled, but draw calls, batches, SetPass calls, triangles, and frame time need capture on the target 1080p PC tier.
4. **P2 - Static batching is not declared.** Non-interactive decorations are runtime-created as separate objects and are not marked static. This is acceptable for the small prototype only until renderer statistics are captured; production replacements should use shared meshes/materials and explicit batching/instancing strategy.
5. **Resolved during review - false gate state.** The first implementation placed six non-colliding bars across the controlled-movement opening. This allowed the player to walk through a visually closed gate. The bars were removed after review feedback, preserving the existing route and avoiding a misleading permission state.

### Accepted Qualities

- Decorative blocks and laundry-machine discs explicitly remove primitive colliders, while existing structural blocks retain their collision. Existing gameplay coordinates and the 2.6 m interaction contract were not changed.
- Runtime color materials remain cached and assigned with `sharedMaterial`; no renderer `.material` clone pattern or per-frame material construction was introduced.
- The shader chain resolves URP Lit, then URP Unlit, with an explicit logged failure if no shader is found. The sprite fallback remains an emergency compatibility risk and should not become the production 3D path.
- The visual language is cohesive: concrete gray, desaturated blue-green, dark metal, muted wood, and restrained orange/safety-yellow route accents. Sign panels, wall bands, fixtures, equipment details, and route markings improve room identity without importing unlicensed art.
- No transparent surfaces, real brands, real facility identifiers, graphic imagery, or tactical operational details were introduced.

## Graphics Request Catalog Review

- `Docs/Art/graphics-next-needed.md` contains 14 individually scoped production requests rather than grouped batches.
- Automated independent counting confirms every declared prompt length matches its actual paragraph length. Counts range from 428 to 559 characters, all strictly below 600.
- No duplicate request titles or duplicate intended uses were found. The catalog correctly avoids re-requesting the broad concept boards already present and instead asks for production-ready materials, modules, props, markers, optimized characters, and animations.
- Each request includes priority, intended Unity use, technical delivery, and a separate prompt. The catalog is accepted without changes.

## Verification Performed

- Inspected `FacilityVisualBuilder.cs`, URP package/version configuration, Graphics Settings, PC/Mobile Quality Settings, playable-day coordinates, HUD implementation, asset inventory, and the graphics request catalog.
- Searched the facility implementation for material construction, `.material` use, shader selection, primitive/collider creation, lights, shadows, and static declarations.
- Independently counted all 14 catalog prompt paragraphs and checked declared values against actual character counts.
- Confirmed the post-feedback source no longer contains the misleading gate-bar geometry.
- Reviewed the latest graphical build log; it does not contain a completion result and therefore cannot satisfy the regression gate.

## Decision

The graphical code pass is **conditionally accepted for prototype integration** because its major visual-state defect was corrected and its material allocation is bounded and shared. It is **not release-gate accepted** until a successful post-change Windows build is recorded, the player is traversed through both role routes, and representative runtime rendering statistics justify or reduce the approximately 16-material palette and added lights/renderers.
