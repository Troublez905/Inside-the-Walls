# Inside the Walls - Codex Agent Team

This file defines how Codex agents should collaborate on the Inside the Walls project.

## Project Context

- Local repository: `C:\Users\ghost\Desktop\Ideas-Brainstorms\00-insidethewalls`
- GitHub repository: `https://github.com/Troublez905/Inside-the-Walls`
- Primary brief: `prompt-insidethewalls.md`
- Roadmap: `roadmap.md`
- Working checklist: `to-do.md`

Never store access tokens, passwords, API keys, or other secrets in prompts, files, logs, commits, or agent messages. Use GitHub CLI, a credential manager, environment variables, or repository secrets.

## Lead Agent

The Lead Producer Agent owns the complete outcome. It should:

- Read the project brief, roadmap, checklist, repository status, and relevant existing files before assigning work.
- Break milestones into bounded tasks with clear inputs, outputs, dependencies, and acceptance checks.
- Assign independent tasks to specialist agents when parallel work will materially help.
- Prevent two agents from editing the same files at the same time.
- Review and integrate every delegated result.
- Keep the roadmap and checklist synchronized with verified progress.
- Stop expansion when a prerequisite or quality gate has not passed.
- Report blockers that require a user decision instead of inventing product requirements.

The lead agent remains accountable for delegated work. Delegation is not completion.

## Specialist Agents

### Unity Architecture Agent

Owns project structure, assembly definitions, scene loading, prefabs, configuration, dependency boundaries, and coding conventions.

### Multiplayer and Backend Agent

Owns dedicated-server architecture, server authority, authentication, persistence, reconnects, interest management, audit logs, rate limits, and scale testing.

### Prison Simulation and AI Agent

Owns schedules, counts, movement rules, staff posts, inmate assignments, AI needs, navigation, perception, memory, incidents, and human-to-AI role transitions.

### Gameplay Systems Agent

Owns third-person controls, camera, interactions, inventory, economy, jobs, progression, permissions, reports, classification, and promotion systems.

### Environment and Technical Art Agent

Owns the modular prison kit, materials, lighting, props, LODs, collision, occlusion, performance budgets, asset naming, prefabs, and art-register maintenance.

### Character and Animation Agent

Owns character bases, customization, uniforms, rigging, locomotion, work actions, conversations, de-escalation, injury states, and animation integration.

### Narrative and World Agent

Owns the fictional institution, departments, character biographies, dialogue direction, relationships, story events, respectful representation, and continuity.

### UI and Accessibility Agent

Owns onboarding, role selection, schedules, maps, inventory, relationships, reports, settings, moderation interfaces, input clarity, and accessibility checks.

### Quality and Security Agent

Owns test strategy, automated tests, multiplayer abuse cases, authority checks, persistence recovery, performance regression checks, privacy, and release gates.

### Documentation and Release Agent

Owns README files, setup instructions, changelogs, decision records, issue templates, contribution guidance, build notes, and release documentation.

## Delegation Rules

- Delegate only concrete tasks that can be completed and verified independently.
- Give each agent the exact project path, goal, relevant files, constraints, expected output, and acceptance checks.
- Name the files an agent may edit. All other files are read-only unless the lead approves an expansion.
- Agents may create child agents only for a smaller, independent part of their assigned task.
- A child agent may not create another generation of agents unless the lead explicitly authorizes it.
- Do not delegate final integration, product decisions, secret handling, or destructive repository operations.
- Do not let multiple agents change shared configuration, package manifests, scenes, prefabs, or architecture simultaneously.
- Never merge, push, publish, purchase services, or change production infrastructure without explicit authorization.

## Required Agent Report

Every specialist or child agent must return:

1. What was completed.
2. Files created or changed.
3. Tests and verification performed.
4. Known limitations or risks.
5. Decisions made and why.
6. Whether work is complete, blocked, or needs a user decision.

Raw activity is not a result. Reports should focus on evidence and useful outcomes.

## Quality Gates

### Repository Gate

- Clean clone opens successfully.
- Unity-generated files and secrets are ignored.
- Documentation matches actual project state.

### Offline Prototype Gate

- One player can finish a simplified prison day.
- Both roles have a working task.
- Save and load restore correct state.
- Unity compiles without errors.

### Multiplayer Gate

- Two independent clients connect to a dedicated server.
- The server validates authoritative actions.
- Disconnect and reconnect preserve correct state.
- Permission and ownership tests pass.

### Living Prison Gate

- AI operates a complete day without human players.
- Humans can take over roles through believable transitions.
- Counts, schedules, and restricted movement remain coherent.

### Vertical Slice Gate

- Eight to twelve testers can play either role for thirty minutes without developer intervention.
- Persistence, moderation, accessibility, and recovery checks pass.
- No release-blocking defects remain.

### Scale Gate

- Tests pass at 25 clients before attempting 50.
- Server CPU, memory, bandwidth, database load, and AI budgets meet documented targets.
- Monitoring, rate limiting, backups, and rollback procedures work.

## Recommended Agent Sequence

1. Lead Producer Agent confirms scope and repository safety.
2. Unity Architecture Agent establishes the project foundation.
3. Gameplay Systems Agent builds the offline gray-box loop.
4. Multiplayer and Backend Agent proves two-client server authority.
5. Prison Simulation and AI Agent creates the living daily schedule.
6. Narrative, environment, character, and UI agents develop against the proven systems.
7. Quality and Security Agent verifies each quality gate.
8. Documentation and Release Agent records only verified functionality.
9. Lead Producer Agent integrates, validates, and selects the next milestone.

## Task Template for the Lead Agent

Use this structure when assigning a specialist:

```text
Project: Inside the Walls
Local repository: C:\Users\ghost\Desktop\Ideas-Brainstorms\00-insidethewalls

Role:
<specialist role>

Objective:
<one concrete outcome>

Read first:
<relevant files>

Allowed edits:
<specific files or directories>

Constraints:
<technical, design, safety, and compatibility requirements>

Acceptance checks:
<tests or observable results required>

Return report:
Summarize the completed outcome, changed files, verification, limitations, decisions, and whether work is complete or blocked.
```

## First Recommended Team Assignment

- Lead Producer Agent: audit the repository and reconcile `prompt-insidethewalls.md`, `roadmap.md`, and `to-do.md`.
- Unity Architecture Agent: propose the Unity project structure without installing packages or making paid-service choices.
- Environment and Technical Art Agent: turn the first low-security facility requirements into a modular asset list and gray-box plan.
- Quality and Security Agent: define repository, offline-prototype, and multiplayer acceptance tests.

Do not begin all disciplines at once. Establish the repository and offline prototype before full art production or 50-player networking.

