# Review Director - Persistence And Character Pipeline Review

## Assignment - 2026-09-06

Independently review persistence and character-pipeline work while it develops. All
implementation and source assets are read-only to this reviewer.

## Acceptance Gates

### Persistence

1. Save data has an explicit schema version and compatibility policy. Unsupported newer
   data is distinguished from corrupt data; missing data is not reported as corruption.
2. Writes are atomic or recoverable: temporary write, flush/close, validated promotion,
   and a defined backup/recovery path. Failed writes cannot destroy the last valid save.
3. Serialization, storage, clock/path, and file operations are injectable enough for
   deterministic tests; tests do not depend on the real player profile.
4. The save root is application-owned. Slot names/IDs cannot escape it through rooted
   paths, separators, traversal, invalid names, symlinks/reparse points, or unsafe
   normalization.
5. Load results distinguish success, missing, incompatible, corrupt, and I/O failure.
   No broad catch or success-shaped fallback hides a failure; diagnostics avoid secrets
   and excessive personal paths.
6. Continue consumes a small, sane metadata/result contract. It enables only for a
   validated compatible save and can explain missing, corrupt, incompatible, and I/O
   states without loading gameplay as a side effect.
7. Required tests cover round-trip, boundaries/defaults, older migration if supported,
   newer rejection, truncation/malformed data, interrupted replacement, backup recovery,
   denied/locked I/O, and path attacks. Test discovery count must be nonzero.

### Character Pipeline

1. Every original source hash remains unchanged. Output is written only to a bounded,
   explicit destination and never overwrites source files.
2. The Blender command is deterministic, non-interactive, time-bounded, fails nonzero,
   uses an explicit Blender executable/source/script/output, and validates paths before
   deleting or replacing any prior generated output.
3. The report records Blender version, source hash, object/mesh/armature/bone/action
   inventory, topology, dimensions/transforms, materials/textures, modifiers, and
   warnings. Unknown facts remain unknown.
4. FBX export is Unity-oriented and recorded: selected mesh/armature only, applied unit
   policy, axis policy, deform-bone policy, leaf bones disabled, animation bake/ranges
   explicit, no texture embedding surprise, and reproducible settings.
5. Scale, orientation, feet/pivot/root, humanoid mapping, skin weights, materials, and
   clips are not claimed valid until independently verified. A generated FBX is a
   candidate, not a production-ready character.
6. Missing armature/actions/material links, empty/invalid FBX, oversized topology, or
   partial command failure blocks completion and is surfaced explicitly.

Initial status: gates defined; agent outputs pending inspection.

## Initial Persistence Review - 2026-09-06

- **P0 - backup recovery absent:** `File.Replace` creates `.backup`, but `Load()` did
  not validate or recover it when the primary was corrupt.
- **P0 - incomplete promotion contract:** first-save promotion moved the temporary file
  without validating its serialized contents, and interrupted/stale temporary handling
  was undefined.
- **P1 - insufficient failure injection:** only the root path was injected; clock and
  file write/replace/read operations were static, preventing deterministic interrupted
  replacement and I/O-failure tests.
- **P1 - privacy:** result messages included raw `exception.Message`, which can expose
  absolute profile paths and system details to UI consumers.
- **P1 - schema/integrity ambiguity:** absent schema defaults to zero and was classified
  as incompatible; checksum omitted schema and timestamp, and timestamp validity was not
  checked.
- **P1 - tests incomplete:** initial tests covered basic round-trip/missing/corruption,
  but not backup recovery, interrupted promotion, injected I/O failures, truncation,
  role/null boundaries, schema-zero policy, or nonzero discovery.

Positive evidence: fixed save filename avoids slot traversal, temp is same-directory and
uses `Flush(true)`, result states distinguish core outcomes, snapshot validation is
explicit, and compatibility inspection has no gameplay side effect.

Findings were sent directly to `/root/offline_gameplay_design`; re-review pending.

## Initial Character Pipeline Review - 2026-09-06

- **P0 - valid prior outputs could be destroyed:** the wrapper exported directly to the
  canonical FBX/report paths and deleted them on timeout, nonzero exit, or source-hash
  mismatch, even if those files predated the failed run.
- **P0 - material evidence incomplete:** embedded textures were enabled without proving
  every image path resolved or recording node socket/use, colorspace, and deterministic
  external Unity handoff.
- **P1 - rig evidence insufficient:** bone names/hierarchy, roots, mesh parenting,
  weights/influences, unweighted vertices, shape keys, and world-space bounds were absent.
- **P1 - unbounded action/selection semantics:** every global action and every mesh and
  armature were exported without establishing a linked expected character graph.
- **P1 - scale/orientation evidence incomplete:** FBX `-Z/Y` flags were recorded, but
  feet/pivot, object location, world bounds, transforms, and meter equivalence were not
  validated.
- **P1 - reproducibility gaps:** timeout input was not bounded; stdout/stderr and script
  hash/settings version were not captured; output containment was implicit rather than
  validated before cleanup/promotion.

Positive evidence: factory startup/background mode, explicit executable/source/script,
timeout and nonzero handling, before/after source hash, deform-only/leaf-bone settings,
and candidate-only Unity status provide a sound starting point.

Findings were sent directly to `/root/environment_art_audit`; material and staging fixes
are required before re-review.

## Persistence Kernel Re-review - 2026-09-06

The revised persistence kernel is **accepted by static review, conditional on test
execution**. It now has explicit `Success`, `Recovered`, `Missing`, `Corrupt`,
`Incompatible`, and `IoError` outcomes; schema-zero is corrupt while unsupported
nonzero schemas are incompatible. The checksum covers schema, UTC timestamp, and the
snapshot. Temporary output is flushed, closed, re-read, and fully validated before
promotion. Valid backup and interrupted-first-write temporary saves can be returned as
`Recovered` without silently overwriting evidence. Clock and file operations are
injectable, UI-safe errors omit exception/path details, and fixed-root/fixed-name plus
reparse checks materially constrain path risk.

Tests are authored for round trip, missing/empty/malformed/tampered data, role and
objective boundaries, schema zero/future, backup/temp recovery, replacement failure,
and invalid root. They were not executed: the test assembly remains intentionally gated
by `ITW_TESTS` because the local Unity Test Framework package is unavailable/stalled.
No automated pass is accepted or claimed. Residual warning: the injected path provider
is a trusted boundary and ancestor reparse points are not exhaustively walked.

## Runtime Save/Continue Integration Review - 2026-09-06

- **P1 - save errors are invisible in gameplay:** `SaveCurrentProgress()` writes a
  failure only to `prototypeMessage`, while `DrawPrototypeHud()` renders
  `playableDay.StatusMessage` whenever the controller exists. Because the failed
  objective remains different from `lastSavedObjective`, the write and error log also
  repeat every frame. A dedicated visible persistence error and a retry/latch policy
  are required.
- **P1 - Continue mutates recovered evidence immediately:** both New Game and Continue
  call `SaveCurrentProgress()` after setup. Continue therefore rewrites timestamp and
  checksum without player progress and implicitly promotes backup/temp recovery by
  overwriting the canonical path. Continue should initialize `lastSavedObjective`
  without saving until the next real checkpoint, or use an explicit, tested recovery
  promotion contract.

Positive evidence: runtime references the persistence assembly; Continue is enabled
only by `CanContinue`; missing/corrupt/incompatible/I/O messages remain distinct;
snapshot role and objective bounds are validated; objective transitions save after the
index changes; and `objectiveIndex == objectives.Length` restores through `CompleteDay`
without indexing past the array. `Logs/save-continue-build.log` reports
`FOUNDATION_BUILD_OK`, size 141361862 and warnings 8, proving compilation/build but not
runtime persistence UX. Findings were sent to `/root`; re-review pending.

## Character Pipeline Re-review - 2026-09-06

The wrapper now uses a unique staging directory, bounds timeout to 30-300 seconds,
preserves prior canonical outputs on Blender/validation failure, compares source hashes,
checks staged containment, validates a nonempty artifact/report, records a bounded log,
and promotes only after validation. The script now restricts export to exactly one
armature and its linked meshes, records bones/hierarchy, weights, transforms, world
bounds, action linkage, material socket/color-space evidence, script/source/output
hashes, and truthfully labels Unity readiness as unverified. The original source report
hash is `7317AB5D...36BC4B`; the generated FBX is nonempty (18,877,676 bytes).

Character acceptance remains **rejected** due to one material blocker and readiness
limits:

- **P0 - textures overwrite each other:** all three packed images are saved as
  `Textures/NoahMercer_BaseColor.png`. The report gives base color, roughness, and normal
  the identical handoff path, while disk contains only one PNG. The material handoff is
  incomplete and potentially the last-written map masquerades as base color.
- **P1 - promoted audit is stale:** `export.path`, image `filepath`, and `handoff_path`
  still name the deleted `.staging-*` directory, so the canonical audit does not describe
  the canonical files.
- **P1 - runtime suitability is not established:** the single mesh is 399,998 triangles
  with up to seven skin influences. There are no actions, and facing, Humanoid avatar,
  skin-weight import behavior, rendering, and animation remain unverified. This is only
  a source/showcase candidate, not a playable character.

These findings were sent directly to `/root/environment_art_audit`; regeneration and
re-review are required.

## Runtime Integration Re-review - 2026-09-06

The two semantic findings were corrected in source: Continue now restores without an
immediate write, New Game alone creates the initial save, and a failed checkpoint is
latched so it does not retry/log every frame. A dedicated `persistenceStatus` is rendered
in the HUD. One P1 presentation issue remains: the gameplay message occupies y=124..174,
the persistence line y=154..184, and progress begins at y=182, so the new status overlaps
adjacent text and may still be unreadable. The current build log predates this source
revision (log 11:10:55; controller 11:13:11), so post-fix compilation is also unverified.
The overlap and stale build evidence were reported to `/root`.
