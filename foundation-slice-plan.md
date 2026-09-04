# Inside the Walls - Foundation Slice Plan

## Product promise

**Title:** Inside the Walls  
**Tagline:** Nobody leaves unchanged.

The first playable proves that a prison can be tense and absorbing without making combat the main activity. The player enters a living institution, reads its routines, makes small consequential choices, and sees people remember what happened.

## Distinctive play thesis

The core fantasy is **pressure through procedure**. Every useful action consumes time, attention, access, or trust. Routine creates predictability; relationships create exceptions; consequences alter tomorrow's routine.

The Foundation Slice should make four verbs satisfying:

1. **Read** the schedule, room state, people, and risks.
2. **Choose** whether to follow procedure, negotiate, help, delay, or bend a rule.
3. **Act** through movement, conversation, work, reporting, and controlled access.
4. **Live with it** as trust, duty performance, favors, incidents, and assignments persist.

This creates two complementary roles without reducing either to a faction:

- **Inmate:** build a survivable day by balancing official progress, relationships, limited time, and personal commitments.
- **Officer:** maintain a coherent day by balancing procedure, discretion, safety, staffing pressure, and rapport.

## Thirty-minute playable arc

The first gray-box day uses one shared sequence viewed from either role:

1. Intake or shift briefing introduces identity, assignment, schedule, and one immediate relationship.
2. Movement period teaches navigation, restricted zones, doors, and visible institutional rules.
3. Meal creates a social choice: keep a promise, assist someone, or protect personal time.
4. Work period gives each role one readable task with quality and timing feedback.
5. Yard creates the slice's main dilemma through conflicting requests and incomplete information.
6. A minor violation triggers an officer observation and review, not instant omniscient punishment.
7. Evening count and lockdown resolve the day, update relationships and records, and save.
8. A short next-day preview shows one concrete consequence, giving the player a reason to return.

### Signature scenario: The Missing Ten Minutes

During movement to work, an inmate asks for help delivering a permitted personal message. The detour is harmless but risks lateness. An officer simultaneously has a post obligation and must decide whether to investigate, redirect, document, or use discretion. AI characters only know what they saw or were told.

This small event exercises schedule pressure, access, conversation, witness knowledge, rapport, discretion, reporting, review, and persistent consequences. It is intentionally nonviolent and replayable from both roles.

## Slice order

### Slice A - Boot and frontend

Scene flow: `Boot -> Splash -> MainMenu -> RoleSelect -> LowSecurityPrototype`.

- Use the supplied title art as provisional splash/menu reference pending rights, resolution, and fictional-setting approval.
- Present the menu as an **after-hours intake board**: restrained motion, precise typography, faded-orange focus markers, and a dark lower-left rail that preserves the art.
- Actions: New Game, Continue, Settings, Credits, Quit.
- Continue is enabled only for valid save metadata. Missing or incompatible content has an adjacent reason.
- New Game is enabled only when its destination is present in Build Settings.
- Keyboard, mouse, and controller share one explicit focus model. Modals trap and restore focus.
- Settings cover volume, display mode, resolution, UI/text scale, reduced motion, screen shake, subtitles, and input-remapping status.
- Missing audio clips produce silence without errors or behavior changes.

### Slice B - Offline gray-box day

- One intake room, housing unit, dining area, yard, officer station, and secure connecting routes.
- Third-person movement and camera, interaction prompts, schedule clock, role and post assignment.
- One inmate work task, one officer duty, one social choice, one minor violation and review, evening lockdown.
- Local versioned save/load restores role, assignment, schedule, checkpoint, permissions, task state, relationships, and incident state.
- All consequential actions travel through an authority request and validator, even offline.

## Technical foundation

Create the project from the latest patched **Unity 6.3 LTS Universal 3D/URP** template available in Unity Hub, then record and pin the exact editor and resolved package versions. Do not select a networking transport in Slices A or B.

### Runtime boundaries

- `InsideTheWalls.Core`: identifiers, results, clock/random abstractions; no Unity dependencies.
- `InsideTheWalls.Simulation`: roles, schedule, assignments, permissions, incidents, day state; Core only.
- `InsideTheWalls.Application`: state machine, use cases, persistence/scene/network ports; Core and Simulation.
- `InsideTheWalls.Persistence`: versioned local save adapter and migrations.
- `InsideTheWalls.Interaction`: requests, prompts, doors, and zones.
- `InsideTheWalls.Characters`: input intent, motor, camera, avatar presentation.
- `InsideTheWalls.UI`: views and presenters; no direct Persistence or Networking reference.
- `InsideTheWalls.Presentation`: Unity composition roots and scene/audio/visual adapters.
- `InsideTheWalls.Networking.Abstractions`: authority context and command contracts only.
- `InsideTheWalls.Networking`: deferred until the multiplayer proof.

`Boot` is the only composition root. It owns persistent application lifetime and additive scene transitions. Simulation code must not depend on cameras, renderers, UI, audio, animation, or input devices so the same rules can run headlessly later.

### Input

Use one Input System actions asset with `UI` and `Player` maps. Devices generate intent; they do not mutate simulation state directly. Include keyboard/mouse and controller bindings from the first build, persist rebinding overrides, and expose sensitivity, dead-zone, and invert-Y settings.

## Verification gates

### Repository gate

- Previously exposed GitHub credentials are confirmed revoked.
- Unity `.gitignore` exists before generated files are tracked.
- Exact Unity version and package lock are committed only after approval.
- A clean clone opens with documented steps and contains no secrets.

### Menu gate

- Boot-to-menu works in Play Mode and a Windows development build with no relevant console errors.
- Navigation works with keyboard, mouse, and a virtual/physical gamepad.
- Focus is always visible; disabled actions cannot invoke and explain why.
- Settings persist; missing/corrupt saves and failed scene loads surface explicit errors.
- Layout passes 1280x720, 16:9, 16:10, ultrawide, 4:3, and 75-150% UI scale checks.

### Offline gate

- Both roles can complete the simplified day and reach lockdown.
- Schedule boundaries and permission matrices pass deterministic EditMode tests.
- Unauthorized door/zone actions fail and authorized actions succeed.
- Save round-trip, schema migration, and corrupt-file handling pass.
- The Missing Ten Minutes resolves through observation, review, and a persistent next-day consequence.

## Deliberate exclusions

No 50-player load testing, production character art, large content library, full AI population, final backend, paid service, additional facility, or mobile build begins before the offline gate passes. Violence is not required to prove the loop.

## Decisions required before implementation

1. Confirm the previously exposed GitHub token has been revoked.
2. Approve creating the project with the latest patched Unity 6.3 LTS Universal 3D/URP template.
3. Confirm whether the key art's `Northridge` name, U.S. flag, and U.S. setting are canonical or provisional.
4. Confirm that the supplied images are owned or licensed for project use; until then they remain reference-only.

