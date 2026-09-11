# Art Director - Next Graphics Audit

## Completed

Audited the playable presentation code, the complete graphics checklist, the asset inventory, available source graphics, and the Noah Mercer export audit. Produced a prioritized list of individual production graphics for the next playable builds. Each request includes a separate prompt under 600 characters, technical delivery requirements, priority, and intended Unity use.

## Files Created

- `Docs/Art/graphics-next-needed.md`
- `AgentLogs/2026-09-06--art-director--next-graphics.md`

## Verification

- Confirmed the current facility is generated primarily from colored cube primitives in `FacilityVisualBuilder.cs`.
- Confirmed the repository already contains the broad concept/reference boards from the existing 23-item checklist.
- Confirmed Noah's source FBX is not runtime-ready: roughly 400,000 triangles, up to seven source influences, and no animation actions.
- Kept every listed generation/artist prompt under 600 characters.
- Kept all requests individual rather than combining rooms, prop libraries, material libraries, animation libraries, or UI screens.
- Made no edits to gameplay code, Unity assets, package manifests, scenes, or project settings.

## Limitations And Risks

- Concept art and image generation cannot directly produce validated production meshes, rigs, collisions, LODs, or Unity materials; those require modeling and technical-art work.
- Asset rights and provenance are not established by file presence and must be recorded before shipping.
- Character integration remains dependent on retopology, four-weight skinning, Humanoid validation, and animation testing.
- Exact performance budgets should be revised after profiling the representative playable scene on the chosen minimum-spec PC.

## Decisions

- Prioritized a small environment replacement set because it changes the largest amount of visible gameplay with the least pipeline risk.
- Prioritized one optimized inmate plus idle and walk before more characters because a shared Humanoid pipeline must be proven first.
- Did not request new versions of existing broad reference boards; the new list asks for production-ready derivatives or narrowly scoped missing assets.
- Separated every material, prop, UI symbol, character, and motion clip into its own request to make delivery and acceptance unambiguous.

## Status

Complete. The next decision belongs to production: commission or build items 1-6 first, then validate them in the actual Unity scene before expanding the art set.
