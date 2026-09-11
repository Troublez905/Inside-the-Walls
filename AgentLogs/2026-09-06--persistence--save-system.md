# Persistence - Local Save System

## Assignment

Implement a versioned local JSON repository for the playable-day prototype. It must preserve role, objective progress, choice, and consequence-preview state; expose explicit compatibility/load results; write atomically with recoverable backups; support injected test storage; and never silently overwrite corrupt or incompatible evidence.

## Decisions

- Schema version 1 stores role as a stable name rather than an enum ordinal, objective index 0-7, choice ID, consequence preview, and a UTC round-trip timestamp.
- The envelope checksum covers schema version, timestamp, and every snapshot field. It detects accidental truncation or modification; it is integrity checking, not tamper-proof authentication.
- The production path provider uses `Application.persistentDataPath`. Tests inject a normalized fixed root.
- Save filenames are internal constants; no caller-provided slot or filename reaches path combination.
- Save writes use a same-directory temporary file, UTF-8 without BOM, and `FileStream.Flush(true)`. The bytes are read and fully validated before promotion.
- Existing saves use `File.Replace` with a backup. First saves use same-volume `File.Move` only after flushed-temp validation.
- Valid backup or interrupted-first-write temp data returns the distinct `Recovered` status. Recovery loads but never promotes or overwrites the damaged/missing primary automatically.
- Missing, corrupt, incompatible, recovered, and I/O outcomes are distinct. UI-facing errors are stable and omit raw exception/path details.
- `Inspect` returns compatibility only and does not expose the gameplay snapshot.
- `ISaveFileOperations` and `IUtcClock` permit deterministic failure and timestamp tests without packages.
- Existing root and file reparse points are rejected. `ISavePathProvider` is trusted infrastructure; fixed slot naming prevents traversal. Future caller-supplied slot IDs must reject rooted paths, separators, and traversal before combination.
- Schema 0 means the version marker is absent and is treated as corrupt/unknown, while nonzero unsupported schemas are incompatible.

## Review Feedback And Response

- Review found backup creation without recovery. Load now validates the primary, then a backup when primary is corrupt/unavailable, and returns `Recovered` without replacing evidence.
- Review found first-save promotion insufficiently checked. The flushed temporary file now passes complete schema, timestamp, snapshot, and checksum validation before move.
- Review requested interruption and I/O testability. Clock and all filesystem operations are injectable; tests inject replace failure deterministically.
- Review found raw exception messages privacy-sensitive. Public results no longer include exception details or absolute paths.
- Review requested stronger integrity metadata. Schema and validated UTC timestamp are included in the checksum.
- Review requested path/reparse policy. The root is normalized, the fixed save remains inside it, and existing root/target/temp/backup reparse points are rejected.
- Review requested broad persistence coverage. Tests cover round trip, missing, empty, malformed/truncated, checksum mismatch, valid/invalid role and objective boundaries, schema 0 and future schema, interrupted first write, backup recovery, replace failure, and invalid storage root.
- The lead authorized one exception to add `InsideTheWalls.Persistence` to the existing EditMode test assembly reference list. Its existing `ITW_TESTS` gate and all other fields were preserved.
- Review Director conditionally accepted the corrected persistence kernel by static inspection. Conditions are nonzero test execution once the framework is available and later UI/Continue integration. Ancestor reparse points are not walked, so the injected path provider remains trusted infrastructure as documented.

## Files

- `Assets/_InsideTheWalls/Scripts/Persistence.meta`
- `Assets/_InsideTheWalls/Scripts/Persistence/InsideTheWalls.Persistence.asmdef`
- `Assets/_InsideTheWalls/Scripts/Persistence/InsideTheWalls.Persistence.asmdef.meta`
- `Assets/_InsideTheWalls/Scripts/Persistence/SaveModels.cs`
- `Assets/_InsideTheWalls/Scripts/Persistence/SaveModels.cs.meta`
- `Assets/_InsideTheWalls/Scripts/Persistence/LocalJsonSaveRepository.cs`
- `Assets/_InsideTheWalls/Scripts/Persistence/LocalJsonSaveRepository.cs.meta`
- `Assets/_InsideTheWalls/Tests/EditMode/PersistenceTests.cs`
- `Assets/_InsideTheWalls/Tests/EditMode/PersistenceTests.cs.meta`
- `Assets/_InsideTheWalls/Tests/EditMode/InsideTheWalls.Simulation.Tests.asmdef` (authorized reference only)
- `AgentLogs/2026-09-06--persistence--save-system.md`

## Verification

- Assembly JSON parses and Persistence references only Simulation plus Unity's built-in runtime.
- Static checks confirm all save paths use the fixed filename under the normalized injected root.
- Static checks confirm no public result contains raw exception messages.
- `git diff --check` completed without patch errors.
- Tests are authored but not claimed executed. The repository's test assembly remains intentionally gated by `ITW_TESTS` while the local Unity Test Framework package is unavailable.

## Limitations And Risks

- The repository is not yet wired into `PlayableDayController` or Continue UI because those files were outside assignment ownership.
- `File.Replace` support is platform/filesystem dependent. A failure returns I/O error and preserves the prior primary where the platform honors replacement semantics; the validated temp is retained for diagnosis/recovery.
- Reparse checks cover the configured root and concrete save files, but `ISavePathProvider` remains trusted infrastructure rather than a sandbox against a malicious provider.
- Checksum is designed for corruption detection, not adversarial save signing.

## Status

Implementation and tests are complete by static inspection. Unity compilation and nonzero test discovery remain blocked by the known local Test Framework resolution issue; no passing test claim is made.
