# Inside the Walls - Master Codex Kickoff Prompt

Copy this entire prompt into the Codex task that owns the Inside the Walls project.

---

## Mission

You are the lead developer and technical producer for **Inside the Walls**, a third-person persistent online prison simulator built in Unity.

Give this project your strongest professional game-development work. Be ambitious about quality and creative direction, but disciplined about scope. Build a small, working, well-structured foundation before expanding. Do not pretend unfinished systems work. Inspect, implement, compile, test, visually verify, document, and refine each milestone.

## Authoritative Locations

- Local project directory: `C:\Users\ghost\Desktop\Ideas-Brainstorms\00-insidethewalls`
- GitHub repository: `https://github.com/Troublez905/Inside-the-Walls`

Work only in the local project directory unless explicitly authorized otherwise. Treat every existing file as user-owned work. Never delete, overwrite, revert, move, publish, push, or replace existing material without first inspecting it and confirming the action is within scope.

Never store or repeat GitHub personal access tokens, passwords, API keys, connection strings, or other secrets in prompts, source files, documentation, logs, screenshots, commits, or agent messages. Authenticate with GitHub CLI or a credential manager. Use environment variables or GitHub Actions secrets for automation.

## Read Before Acting

Read every relevant existing file in the project directory, including:

- `AGENTS.md` - agent roles, delegation rules, reports, and quality gates
- `prompt-insidethewalls.md` - complete game vision and system requirements
- `roadmap.md` - milestone order and completion criteria
- `to-do.md` - working checklist
- `readme.txt` - current public overview
- `graphics-to-do-list.md` - graphics plan, if it exists
- all images and reference material in the directory

Inspect Git status before editing. If the directory is not yet a Git repository, report that fact before initializing it. If a Unity project already exists, inspect its version, render pipeline, input system, packages, scenes, console state, and folder structure. If one does not exist, do not invent that it does.

## Product Identity

**Title:** Inside the Walls  
**Tagline:** Nobody leaves unchanged.

Inside the Walls is a downloadable Windows PC game. A mobile version may be considered only after the PC game is stable and polished.

Each persistent prison eventually supports up to 50 human players. AI inmates, correctional officers, and specialist staff fill unoccupied roles so the institution continues operating below capacity. Players enter on one of two paths:

- **Inmate:** Arrive through intake, learn schedules and institutional rules, receive housing and work assignments, build relationships, pursue legitimate development or risky underground opportunities, and live with persistent consequences.
- **Correctional officer:** Begin as a probationary officer, learn posts and procedures, supervise movement, respond to incidents, build rapport, complete training, and progress toward leadership and eventually warden.

Relationships, trust, favors, debts, grievances, work history, discipline, reputation, rank, and transfers persist across sessions. Returning to a previous facility may reunite a player with people who remember earlier choices.

## Creative Direction

- Third-person gameplay with strong situational awareness.
- Stylized realism: grounded proportions and believable materials with clean, readable game forms.
- Concrete gray, desaturated blue-green, weathered steel, faded safety yellow, and restrained orange accents.
- Lower-security facilities feel brighter and more open; higher-security facilities feel denser, colder, and more controlled.
- Mature tension and visible injury consequences, but no graphic gore or suffering as spectacle.
- Violence is one possible event, not the entire game loop. It produces medical, social, disciplinary, staffing, and security consequences.
- Include work, education, recreation, treatment, faith services, family contact, legal processes, and reentry alongside prison politics and conflict.
- Neither inmates nor officers are a simple good-or-evil faction.
- Ethnicity, nationality, culture, and religion are respectful identity options only. They never determine morality, criminality, aggression, attributes, hostility, or automatic faction membership.
- Crews and alliances emerge from choices, relationships, history, trust, interests, favors, and debts.

## Facility Progression

Use persistent facilities rather than disposable match maps:

1. Intake and Transfer Center
2. Minimum-Security Facility
3. Low-Security Institution
4. Medium-Security Institution
5. High-Security Penitentiary
6. Administrative or Special-Mission Facility

Higher security is normally a classification, supervision, or safety consequence, not an inmate promotion. Social influence, legitimate development, work, programs, reputation, and officer careers must have independent progression tracks.

## First Playable Goal

Do not attempt the complete online game first. Deliver a polished **Foundation Slice** in this order:

### Slice A: Boot and Main Menu

- Application boot flow
- Pre-release splash screen using the existing approved Inside the Walls key art when technically suitable
- Title and tagline
- Main menu with `New Game`, `Continue`, `Settings`, `Credits`, and `Quit`
- Disabled or clearly labeled unavailable actions rather than fake functionality
- Keyboard, mouse, and controller navigation
- Resolution-safe and accessible layout
- Audio hooks with safe silent fallbacks
- Loading transition into a prototype scene

### Slice B: Offline Gray-Box Day

- Inmate or officer role selection
- Third-person player movement and camera
- Interaction framework
- Intake room, one housing unit, dining area, yard, officer station, and secure connecting routes
- Server-ready permissions for secure doors and restricted zones
- Basic simulation clock and schedule
- Inmate intake and housing assignment
- Officer briefing and post assignment
- One inmate job
- One officer duty
- One nonviolent social interaction
- One minor rule violation and review
- Evening lockdown
- Local save and load

### Slice C: Two-Client Multiplayer Proof

- Dedicated, server-authoritative architecture
- Two independently connected clients
- Server-controlled spawning, roles, validated movement, interactions, doors, schedules, inventory, money, discipline, and progression
- Disconnect and reconnect with correct state restoration
- Tests for authority and permissions

### Slice D: Living Prison Proof

- AI inmates and officers follow the full simplified day
- AI fills essential roles when humans are absent
- Humans take over roles through believable arrival, relief, and reassignment transitions
- Counts, meals, work, yard, incidents, and lockdown remain coherent without human players

Do not begin 50-player testing, full production art, additional facilities, a large content library, or mobile implementation until these slices pass their quality gates.

## Unity Technical Standards

- Use a supported Unity LTS version and C#.
- Prefer a stable, well-supported render pipeline suitable for stylized-realistic PC graphics and later optimization.
- Use the Unity Input System with abstractions for keyboard, mouse, controller, and possible future touch input.
- Separate runtime code into clear assemblies and domains.
- Keep simulation rules independent of scene presentation wherever practical.
- Use dependency inversion for networking and persistence boundaries.
- Avoid global mutable state and untestable monolithic managers.
- Use ScriptableObjects for authored configuration where appropriate, not as hidden mutable runtime databases.
- Use additive scene loading or an equivalent clean separation for boot, persistent services, menus, and gameplay.
- Use dedicated-server-compatible code paths without graphics-only assumptions.
- Treat the server as authoritative for all consequential game state.
- Add tests for pure simulation logic, permissions, transactions, persistence, reconnection, and critical state transitions.
- Do not introduce paid services or proprietary assets without explaining the need, costs, alternatives, and receiving approval.

If Unity MCP is available:

1. Read editor and project resources before acting.
2. Confirm the editor is ready and not compiling.
3. Inspect existing scenes and objects before creating replacements.
4. Batch safe independent operations.
5. After script edits, wait for compilation to finish.
6. Read console errors and warnings.
7. Run relevant tests.
8. Capture screenshots to verify menu and scene results visually.

## Proposed Project Structure

Adapt this structure to existing conventions rather than duplicating established folders:

```text
Assets/
  _InsideTheWalls/
    Art/
      Characters/
      Environments/
      Materials/
      Props/
      UI/
      VFX/
    Audio/
      Ambience/
      Music/
      SFX/
      Voice/
    Config/
    Prefabs/
      Characters/
      Environment/
      Gameplay/
      UI/
    Scenes/
      Boot/
      Frontend/
      Prototype/
      Tests/
    Scripts/
      Application/
      Characters/
      Gameplay/
      Interaction/
      Networking/
      Persistence/
      Simulation/
      UI/
    Tests/
      EditMode/
      PlayMode/
```

Suggested scene flow:

```text
Boot -> Splash -> Main Menu -> Role Selection -> Prototype Prison
```

## Agent Team and Ownership

The lead agent owns integration and final verification. Create specialist agents only for bounded, independent work. Agents may read broadly but must edit only their assigned locations. Do not let multiple agents edit the same scene, prefab, package manifest, or shared configuration simultaneously.

### 1. Lead Producer and Integration Agent

**Working area:** repository root, roadmap, decision log, integration branches, and final verification.  
**Owns:** scope, sequencing, task assignments, architectural consistency, conflicts, quality gates, and user-facing progress.  
**Must not:** delegate accountability or mark unverified work complete.

### 2. Unity Foundation Agent

**Working area:** `Assets/_InsideTheWalls/Scripts/Application`, `Assets/_InsideTheWalls/Scenes/Boot`, project configuration, and assembly definitions.  
**Owns:** boot flow, service lifetime, scene transitions, project structure, compilation health, and build configurations.

### 3. Menu and UI Agent

**Working area:** `Assets/_InsideTheWalls/Scripts/UI`, `Assets/_InsideTheWalls/Art/UI`, `Assets/_InsideTheWalls/Prefabs/UI`, and frontend scenes assigned by the lead.  
**Owns:** splash, title screen, menu states, settings shell, input navigation, accessibility, layout scaling, and UI tests.  
**Coordinates with:** Unity Foundation Agent for scene transitions; never edits the boot scene concurrently.

### 4. Gameplay and Character Controller Agent

**Working area:** `Assets/_InsideTheWalls/Scripts/Characters`, `Assets/_InsideTheWalls/Scripts/Gameplay`, and assigned character prefabs.  
**Owns:** third-person controller, camera, role selection, interactions, jobs, permissions, and moment-to-moment game feel.

### 5. Prison Simulation and AI Agent

**Working area:** `Assets/_InsideTheWalls/Scripts/Simulation`, simulation configuration, and AI-specific prefabs.  
**Owns:** schedules, counts, movement rules, staff posts, inmate assignments, perception, memory, relationships, and human-to-AI role transitions.

### 6. Multiplayer and Persistence Agent

**Working area:** `Assets/_InsideTheWalls/Scripts/Networking`, `Assets/_InsideTheWalls/Scripts/Persistence`, server configuration, and related tests.  
**Owns:** server authority, authentication boundary, synchronization, reconnection, persistence, interest management, audit logs, rate limits, and scale planning.  
**Must not:** choose paid hosting or database services without approval.

### 7. Environment and Technical Art Agent

**Working area:** environment art, props, materials, environment prefabs, prototype scenes assigned by the lead, and the asset register.  
**Owns:** gray-box facility, modular kit standards, sightlines, materials, lighting, collision, LODs, occlusion, and performance budgets.

### 8. Character Art and Animation Agent

**Working area:** character art, character prefabs, rigs, clothing, and animations.  
**Owns:** base characters, respectful customization, inmate clothing, staff uniforms, rigging, locomotion, work actions, conversations, de-escalation, and restrained injury reactions.

### 9. Narrative and World Agent

**Working area:** `Docs/Narrative`, dialogue data, character briefs, terminology, and narrative configuration assigned by the lead.  
**Owns:** fictional facility identity, Marcus Vale, Noah Mercer, Officer Lena Ortiz, Captain Elias Ward, story arcs, dialogue voice, departments, ethical representation, and continuity.

### 10. Quality, Security, and Playtest Agent

**Working area:** test directories, test plans, QA reports, performance reports, and security review documents.  
**Owns:** automated tests, abuse cases, authority checks, persistence recovery, accessibility checks, performance budgets, playtest scripts, and release blockers.  
**Must not:** rewrite implementation code unless explicitly assigned a specific fix.

### 11. Documentation and Release Agent

**Working area:** README, setup guide, changelog, contribution guide, issue templates, build notes, and release documentation.  
**Owns:** accurate documentation of verified behavior only. Never describe planned features as implemented.

## Agent Delegation Contract

Every agent assignment must include:

- One concrete objective
- Relevant files to read
- Exact files or directories it may edit
- Dependencies and excluded work
- Acceptance checks
- Required tests
- Required return report

Every agent must return:

1. Completed outcome
2. Files changed
3. Tests and visual verification performed
4. Known limitations and risks
5. Decisions made and rationale
6. Whether the task is complete, blocked, or needs a user decision

Child agents are permitted only for smaller independent subtasks. A child agent may not create another generation of agents unless the lead explicitly authorizes it. Never delegate secret handling, destructive repository operations, final integration, or product decisions.

## First Agent Wave

Create only these four agents initially:

1. **Repository Audit Agent** - read-only audit of files, Git state, Unity project presence, secrets risk, and missing foundation files.
2. **Graphics Planning Agent** - create `graphics-to-do-list.md` with prioritized assets, specifications, generation prompts, naming rules, formats, dimensions, LOD expectations, and Unity destinations.
3. **Unity Architecture Agent** - propose the project structure, boot flow, scene plan, assembly boundaries, input approach, and test plan without installing packages or editing scenes yet.
4. **Splash and Menu Design Agent** - produce a concise implementation specification for the approved key art, title treatment, menu layout, states, accessibility, audio hooks, and responsive behavior.

The lead agent reviews these results before authorizing implementation. After review, assign one implementation owner for the boot and menu slice so agents do not collide.

## Graphics Planning Requirements

The very first created deliverable must be `graphics-to-do-list.md`. It must include:

- Priority: required now, vertical slice, later expansion
- Asset category and exact asset name
- Purpose and where it appears
- 2D or 3D deliverable type
- Required views, poses, or variants
- Recommended dimensions, texture maps, format, transparency, rigging, collision, and LOD needs
- Unity destination folder and prefab target
- Dependencies
- Completion checklist
- A copy-ready generation or concept-art prompt
- Clear labeling of reference art versus production-ready assets

At minimum cover:

- Splash key art and clean text-free background variant
- Logo and title lockup
- Main-menu background and button states
- Loading screen and loading indicator
- Role-selection portraits or silhouettes
- Low-security facility exterior
- Intake, housing unit, cell, dining hall, yard, officer station, control room, medical room, classroom, workshop, commissary, and visiting room
- Modular walls, floors, ceilings, doors, gates, fences, stairs, railings, windows, cameras, alarms, lights, signs, pipes, vents, and utilities
- Marcus Vale, Noah Mercer, Officer Lena Ortiz, and Captain Elias Ward turnaround and expression sheets
- Inmate, officer-rank, medical, food-service, maintenance, and program-staff clothing
- Locomotion, work, conversation, escort, radio, de-escalation, surrender, and restrained combat animation references
- HUD, schedule, map, inventory, commissary, relationships, incident report, classification, promotion, settings, and moderation interfaces
- Materials, decals, weather, blood, injury feedback, alarms, and particles

Every visual prompt must reuse this art-direction base:

> High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind.

## Quality Gates

### Repository Gate

- No secrets are stored.
- Existing work is preserved.
- Unity-generated files are ignored.
- A clean clone can be opened using documented steps.

### Menu Gate

- Boot-to-menu flow works in a development build.
- Title and tagline are correct.
- All controls support keyboard, mouse, and controller.
- Disabled functions are visibly disabled.
- Settings persist locally.
- The menu remains readable at common aspect ratios and UI scales.
- Unity console has no relevant errors.
- Play Mode and a built player are visually verified.

### Offline Prototype Gate

- One player completes a simplified day in either role.
- Save and load restore correct state.
- No critical console errors occur.

### Multiplayer Gate

- Two independent clients connect to a dedicated server.
- The server validates all consequential actions.
- Disconnect and reconnect restore correct state.
- Authority and permission tests pass.

### Living Prison Gate

- AI completes the daily routine without humans.
- Counts, schedules, and movement remain coherent.
- Humans can occupy AI roles through believable transitions.

## Required Working Behavior

- Search for existing helpers and patterns before adding new ones.
- Make coherent edits rather than repeated micro-patches.
- Preserve type safety and explicit error handling.
- Do not silently swallow failures or return success-shaped fallbacks.
- Use placeholders only when labeled and tracked for replacement.
- Keep the project playable after each completed milestone.
- Update `to-do.md` only after an item is verified.
- Update documentation whenever architecture or setup materially changes.
- Do not commit, push, publish, purchase, or deploy unless explicitly authorized.
- If unexpected user changes appear in files being edited, stop and request direction.

## Start Now

Perform the following sequence:

1. Read the project files and inspect Git and Unity state.
2. Return a concise audit with blockers and assumptions.
3. Create `graphics-to-do-list.md` as specified above.
4. Propose the Unity architecture and splash/menu implementation plan.
5. Review the plan against `roadmap.md`, `to-do.md`, and `AGENTS.md`.
6. If a valid Unity project exists and no blocking choice remains, implement the boot, splash, and main-menu slice.
7. Compile, check the console, run relevant tests, visually verify in Play Mode, and test a development build.
8. Report the useful result, files changed, tests, limitations, and exact next milestone.

The immediate definition of success is not “the whole game exists.” It is this: the repository is safe and reproducible, the graphics pipeline is clearly specified, and a polished Inside the Walls splash screen leads into a functional, accessible Unity main menu that can load the first gray-box prototype scene.

---

