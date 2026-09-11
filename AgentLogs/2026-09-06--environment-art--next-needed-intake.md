# Environment Art - Next-Needed Graphics Intake

## Assignment

Intake the fourteen boards delivered for `Docs/Art/graphics-next-needed.md`, classify them honestly, mirror them into the Unity asset root for visibility, and restyle the runtime gray-box from their proportions and palette without treating flattened sheets as production textures or meshes.

## Completed

- Confirmed all 14 requested titles exist under `Docs/Art/` and measured dimensions/formats.
- Copied the boards into `Assets/_InsideTheWalls/` beside the earlier concept set.
- Updated `Docs/Art/asset-inventory.md` with a dedicated next-needed intake table.
- Marked delivery status in `Docs/Art/graphics-next-needed.md`.
- Restyled `FacilityVisualBuilder` toward the new references: blue-green walls, dark base trim, vinyl floors, property-bin stacks, laundry carts, dining tables with stools, secure door leaf, and yellow-grip exercise rail.
- Updated objective markers in `PlayableDayController` to a procedural bracket/ring silhouette.

## Files Created Or Changed

- `Assets/_InsideTheWalls/*.png` (14 copied reference boards)
- `Docs/Art/asset-inventory.md`
- `Docs/Art/graphics-next-needed.md`
- `Assets/_InsideTheWalls/Scripts/Presentation/FacilityVisualBuilder.cs`
- `Assets/_InsideTheWalls/Scripts/Presentation/PlayableDayController.cs`
- `AgentLogs/2026-09-06--environment-art--next-needed-intake.md`

## Verification

- Measured every new PNG with System.Drawing.
- Visually inspected material, door, prop, and marker boards.
- Confirmed no FBX, SVG, spritesheet, or separated PBR map was present.
- Preserved objective coordinates and the controlled-movement opening for traversal.

## Limitations And Risks

- These boards do not satisfy the technical delivery rows in `graphics-next-needed.md`.
- Objective marker sheet style conflicts with the restrained institutional brief; runtime markers are procedural stand-ins.
- Unity batch build/package resolution was still running during intake; post-change compile evidence is pending.
- Rights/provenance remain unrecorded.

## Decisions

- Keep every board reference-only until separable production assets and rights exist.
- Prefer restyling primitives from measured proportions over cropping composite sheets into textures.
- Continue prioritizing true modular wall/door/prop FBX and one optimized Humanoid before more concept boards.

## Status

Complete for reference intake and gray-box restyle. Production art pipeline remains open.
