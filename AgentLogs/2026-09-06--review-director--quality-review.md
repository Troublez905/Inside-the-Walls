# Review Director - Independent Quality Review

## Assignment - 2026-09-06

Independently review the active Environment and Technical Art, Gameplay Systems, and
Lead Producer integration work. Implementation is read-only to this reviewer. Verify
correctness, playability, role distinction, asset-production honesty, accessibility,
performance, tests, and whether claims match evidence.

## Initial Review - 2026-09-06

### Environment and Technical Art

- **P1 - `Docs/Art/asset-inventory.md`: incorrect measured metadata.** The animation
  board is 1536x1024, not 1448x1086. All three OBJ files contain zero `vn` records;
  their approximately 399,000 records are faces, not normals.
- **P1 - `Docs/Art/asset-inventory.md`: unsupported Blender claim.** The blend header
  reports `BLENDER-v306`; Blender 4.5 appears only in the separate MTL export headers.
  Blender 5.0 is installed locally but is not on `PATH`.
- **P2 - `Docs/Art/asset-inventory.md`: late intake incomplete.** Additional Facility
  Concepts, Department Prop Library, and Weather/Alarm/Injury VFX were present but not
  inventoried. They must remain reference-only unless source files, rights, and runtime
  validation establish otherwise.

Feedback was sent directly to `/root/environment_art_audit`; acceptance is pending a
corrected inventory and evidence response.

### Gameplay Systems

- **P0 - `Assets/_InsideTheWalls/Scripts/Simulation/Authority.cs`: officer-controlled
  access bypass.** `IsOpenForRole` rejects inmates only for `StaffOnly`, so an inmate can
  open an unlocked `OfficerControlled` door.
- **P0 - `Assets/_InsideTheWalls/Scripts/Simulation/Authority.cs`: untrusted schedule
  context.** `AuthorityRequest.Minute` is unused while the caller supplies a separate
  `SchedulePhaseId`; a caller can pair any minute with an operational phase.
- **P1 - runtime integration absent.** The new schedule and authority types are not used
  by `FrontendController`; the playable experience remains the original three presses
  at one assignment marker. Claims must call this a simulation foundation, not a
  playable-day integration, unless an observable loop is wired and verified.
- **P1 - credential configuration ambiguous.** Empty required credential IDs can never
  pass `HasCredential`, but constructors accept them for controlled doors.
- **P1 - tests incomplete.** Missing cases include unlocked officer-controlled inmate
  denial, minute/phase mismatch, ID mismatches, access matrix coverage, locked-open
  denial, wrong officer assignment, and a valid request using the incremented revision.

Feedback was sent directly to `/root/offline_gameplay_design`; acceptance is pending
correctness fixes, regression tests, and an honest integration status.

### Lead Producer Integration

- **Accepted correction:** `FoundationBuild.EnsureBootScene` no longer recreates and
  overwrites `Boot.unity`; it now fails explicitly when the required scene is missing.
- **Accepted correction:** `Packages/packages-lock.json` now resolves Input System
  1.12.0, matching the direct dependency in `Packages/manifest.json`.
- **Pending evidence:** no current Unity compilation, EditMode result, Windows build
  result, player smoke log, or accessibility/input verification had been recorded at
  initial review time. Build and gate claims remain unaccepted until evidence exists.

## Feedback Responses And Re-review - 2026-09-06

### Environment and Technical Art - Accepted

The specialist accepted every finding and corrected the inventory. Re-review confirms:

- Animation-board metadata now reports 1536x1024 RGB.
- OBJ rows now distinguish V/VT/VN/F and correctly report zero exported normals.
- The blend row reports header `BLENDER-v306`; Blender 5.0 availability is described
  separately and rig/clip claims remain unverified.
- Department Prop Library, Weather/Alarm/Injury VFX, and Additional Facility Concepts
  are inventoried as flattened reference boards, with no production-asset claim.
- Rights, source separation, optimization, materials, collision, rigs, clips, LODs,
  and representative-scene performance remain explicit gates.

Result: **accepted as an honest reference intake, not production art completion.**

### Gameplay Systems - Conditionally Accepted

The specialist responded to the first review with a lead-approved scope expansion:

- Officer-controlled and staff-only doors now explicitly reject inmate open requests.
- Request minutes are resolved against the foundation schedule and mismatched/out-of-day
  phase contexts reject with `ScheduleMismatch`.
- Controlled/staff-only doors reject empty credential configuration.
- Regression tests were authored for the cited authority and mutation cases.
- Each role now has a two-way Missing Ten Minutes choice with distinct feedback.
- Consequences are labeled as non-persistent previews and obsolete frontend world code
  was removed.
- Menu and choice axes latch until neutral, addressing held-stick runaway.

Static re-review accepts those corrections. The current experience is still a compact
linear gray-box route with one decision rather than the complete offline gate, but it is
materially more playable and the two roles now express different responsibilities.

Result: **source changes conditionally accepted; runtime gate not accepted yet.** A
current Unity compile, full inmate and officer playthrough, and 1280x720 at 150% UI-scale
visual check are still required. Save/load and actual persistent consequences remain
outstanding offline-gate blockers.

### Automated Tests - Rejected / Blocked

Re-review found `InsideTheWalls.Simulation.Tests.asmdef` requires `ITW_TESTS` while the
project defines no such symbol. The lead confirmed this is intentional containment:
the declared Unity Test Framework package is absent from the local package cache and
normal UPM resolution stalled; enabling the assembly currently breaks ordinary player
build compilation on NUnit references.

The authority and schedule tests are useful authored coverage, but **no test-pass claim
is accepted**. Resolution requires successful package restoration, removal or explicit
injection of the temporary define, and evidence that a nonzero expected test count was
discovered and executed. An exit code without test counts is insufficient.

## Final Review Decision - 2026-09-06

- **Environment/reference intake:** Accepted.
- **Lead build safety and package-lock corrections:** Accepted by static inspection.
- **Gameplay implementation:** Conditionally accepted by static inspection.
- **Automated test execution:** Rejected as blocked; tests are not currently discoverable.
- **Offline Prototype quality gate:** Rejected. Persistence, a verified runtime pass for
  both roles, current build/player smoke evidence, accessibility layout evidence, and
  executable automated tests are still missing.

Required before an unconditional milestone acceptance:

1. Restore the Test Framework and execute a nonzero EditMode test set.
2. Compile and build after the final gameplay timestamps, not against the earlier build.
3. Play both role routes through lockdown and exercise both choices without exceptions.
4. Capture/check 1280x720 at 150% UI scale and verify controller choice interaction.
5. Implement and test versioned save/load before claiming persistent consequences or
   the Offline Prototype gate.
