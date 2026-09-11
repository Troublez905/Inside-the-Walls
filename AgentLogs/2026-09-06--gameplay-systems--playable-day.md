# Gameplay Systems - Playable Day Integration

## Assignment

Integrate the deterministic schedule and offline authority foundation into the runtime-generated gray-box prototype. Provide distinct inmate and officer routes, multiple spatial objectives, proximity guidance, a controlled-door authority moment, recoverable completion, and a lockdown summary without changing scenes, packages, build tooling, or user art.

## Decisions

- Kept the runtime-generated environment for this iteration and divided it into seven readable locations.
- Added seven objectives per role with different institutional responsibilities and route order.
- Used the simulation's authoritative day schedule for displayed phase times.
- Routed the inmate laundry-door request and officer movement-gate unlock through `OfflineAuthorityGateway`.
- Added explicit two-way choices at The Missing Ten Minutes moment. Inmates choose between delivering the message or keeping to the work route; officers choose between documenting the incident or verifying the delivery and using discretion.
- Treated consequences as previews only. No persistence or relationship-state claim is made because save/load is outside this slice.
- Kept keyboard/controller interaction parity through E/A and choice parity through horizontal input plus E/A.
- Added axis latching for menu and choice navigation so a held stick cannot race through options.

## Review Feedback And Response

- Review found inmates could open unlocked officer-controlled doors. The lead expanded ownership; explicit door-role permissions now reject this without mutation and a regression test covers it.
- Review found caller-supplied phase could disagree with request time. Validation now resolves the foundation schedule from request time and rejects mismatches and out-of-range times.
- Review found empty controlled-door credentials were ambiguous. Controlled and staff-only doors now reject empty credential configuration.
- Review requested broader authority coverage. Tests now cover actor/target mismatch, public and staff access, locked doors, wrong officer assignment, current-revision success, time boundaries, and rejected-action immutability.
- Review found the first runtime pass was still linear cube traversal. Each role now receives an explicit, controller-accessible dilemma with distinct immediate feedback and summary preview.
- Review found summary text overstated persistence. Summary copy now labels outcomes as next-day previews and states that persistence arrives in the save/load slice.
- Review noted visual reliance on orange markers. Distance, location, objective, progress, and interaction text remain available independently of marker color; completion suppresses the proximity panel to prevent overlay collision.
- Review requested dead frontend cleanup. The unused original world builder and stale progress/player fields were removed.
- Review found the test assembly currently requires undefined `ITW_TESTS`. This file is outside the expanded edit permission and was escalated to the lead; no test-discovery success is claimed until a nonzero run is observed.

## Files

- `Assets/_InsideTheWalls/Scripts/UI/FrontendController.cs`
- `Assets/_InsideTheWalls/Scripts/Presentation/PlayableDayController.cs`
- `Assets/_InsideTheWalls/Scripts/Presentation/PlayableDayController.cs.meta`
- `Assets/_InsideTheWalls/Scripts/Presentation.meta`
- `Assets/_InsideTheWalls/Scripts/InsideTheWalls.Runtime.asmdef`
- `Assets/_InsideTheWalls/Scripts/Simulation/Authority.cs` (lead-approved review fix)
- `Assets/_InsideTheWalls/Tests/EditMode/AuthorityTests.cs` (lead-approved regression coverage)
- `AgentLogs/2026-09-06--gameplay-systems--playable-day.md`

## Verification

- Assembly JSON parses and runtime assembly references `InsideTheWalls.Simulation`.
- `git diff --check` reports no patch whitespace errors in assigned files.
- Static inspection confirms obsolete `BuildPrototypeWorld`, `dutyProgress`, `stationPosition`, and frontend player fields are removed.
- Runtime objectives use foundation-day minutes matching the supplied schedule phases.
- Full Unity compile, EditMode discovery, visual layout, and role playthrough remain pending coordinated verification by the lead because another agent is active in the same Unity workspace and the current test asmdef excludes tests behind undefined `ITW_TESTS`.

## Status

Implementation is compile-ready by inspection. Final gate remains blocked on a coordinated Unity compile/playthrough and correction of the test-discovery define by its owner.
