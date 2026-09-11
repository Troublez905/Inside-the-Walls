# v5.o - Pre-Beta Release

Date: 2026-09-11

## Playable Focus

- Updated the in-game and menu version label to `v5.o - Pre-Beta Release`.
- Integrated the new `Behind The Walls` ensemble splash into the splash screen.
- Added supplied interaction and combat animation clips to the shared Inmate #2 controller.
- Added runtime prefabs for Officer Lena and Inmate #3 from `Assets/_InsideTheWalls/new resources02`.
- Updated NPC spawning so inmates and officers can use animated supplied character prefabs, with procedural placeholders still available as fallbacks.
- Kept local save data out of verification and build automation.

## Verification

- Unity batch build: `InsideTheWalls.Editor.FoundationBuild.ValidateAndBuildPopulation`
- Result: `FOUNDATION_BUILD_OK size=315231334 warnings=491`
- Build output: `Builds/Windows-PopulationAlpha/InsideTheWalls.exe`
- Character prefab check: `InsideTheWalls.Editor.NewResourceCharacterBuilder.EnsureAssets`
- Result: `NEW_RESOURCE_CHARACTERS_OK officer-lena and inmate-three runtime prefabs generated`

## Known Limits

- NPC interaction is still prototype-level: talk, calm, guard stance, and shove reaction are wired, but this is not a full combat system yet.
- The two officer NPCs currently share the Officer Lena runtime prefab until more distinct rigged officer models are selected.
- Unity still reports existing warnings from packages and older editor helper APIs; no compile errors were present in the verified pass.
- Build outputs remain local and are intentionally ignored by Git.
