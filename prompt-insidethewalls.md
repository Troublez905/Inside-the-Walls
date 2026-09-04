# Codex Build Prompt: Inside the Walls

## Project Locations

- Local repository directory: `C:\Users\ghost\Desktop\Ideas-Brainstorms\00-insidethewalls`
- GitHub repository: `https://github.com/Troublez905/Inside-the-Walls`

Never place GitHub access tokens, passwords, API keys, or other secrets in source files, prompts, logs, commits, or documentation. Use `gh auth login`, the operating-system credential manager, environment variables, or GitHub Actions secrets.

## Role

Act as a senior Unity multiplayer engineer, systems designer, AI programmer, and technical producer. Build the project incrementally, preserve unrelated work, follow existing repository conventions, and verify each milestone before continuing.

## Game Vision

**Inside the Walls** is a downloadable, third-person, persistent online prison simulator for Windows PC. A mobile version may be considered after the PC release is stable and polished.

**Tagline:** *Nobody leaves unchanged.*

Each persistent prison supports up to 50 human players. AI-controlled inmates, correctional officers, and specialist staff fill empty roles so routines continue below capacity. Players begin either as newly arrived inmates or probationary correctional officers. Their relationships, decisions, work history, reputation, discipline, rank, and transfers persist across sessions.

Use a fictional North American correctional system inspired by real institutional structures. Accuracy should create believable gameplay, not reproduce a real facility or provide practical instructions for wrongdoing.

## Design Principles

- Stylized realism with readable characters, grounded architecture, and restrained gore.
- Violence has medical, disciplinary, security, staffing, and social consequences.
- Include work, education, treatment, recreation, faith services, family contact, legal processes, and reentry alongside conflict.
- Ethnicity, culture, nationality, and religion are respectful identity choices only. They never determine criminality, morality, aggression, statistics, hostility, or faction membership.
- Crews and alliances form through choices, history, shared interests, trust, favors, and debts.
- Neither inmates nor officers are a simple good-or-evil faction.
- Progress comes from responsibility, competence, relationships, and consequential decisions.

## Facility Structure

1. **Intake and Transfer Center:** Processing, screening, property, orientation, classification, temporary housing, and transport.
2. **Minimum Security:** Dormitory housing, open movement, work, programs, and release preparation.
3. **Low Security:** Fenced boundaries, structured movement, larger departments, and developing social politics.
4. **Medium Security:** Cell housing, reinforced perimeters, tighter movement, more staff, and greater internal control.
5. **High Security:** Closely controlled movement, intensive supervision, complex safety risks, and coordinated emergency response.
6. **Administrative Facilities:** Medical, transfer, protective, restricted, and other special missions; these are not merely a higher gameplay level.

Higher security is normally a classification or safety consequence, not an inmate promotion. Social influence, institutional status, education, work, rehabilitation, and officer careers use separate progression tracks.

## Inmate Progression

- New arrival
- Oriented resident
- Stable unit resident
- Trusted worker or program participant
- Senior worker, peer mentor, or respected community member
- Transfer-eligible or reentry-focused inmate

Track personal, housing-unit, staff, work, program, conduct, and underground reputations separately. Classification should consider conduct, supervision needs, safety and separation needs, escape risk, health, programming, sentence status, and work eligibility.

## Officer Progression

1. Probationary Officer
2. Correctional Officer
3. Senior Officer or Field Training Officer
4. Lieutenant
5. Captain
6. Associate Warden
7. Warden
8. Optional complex or regional leadership endgame

Promotions require training, certifications, accurate reports, sound judgment, de-escalation, ethical conduct, leadership, attendance, and safe completion of duties. Abuse, corruption, negligence, unnecessary force, and falsified reports create investigations and career consequences.

## Prison Organization

Model interacting departments for the warden, security, unit management, intake and records, medical, psychology, education, vocational training, recreation, food service, commissary, religious services, visiting, maintenance, laundry, intelligence, transportation, and reentry.

Each housing unit has a unit manager, case manager, counselor, and unit officer. Departments need schedules, jobs, access permissions, resources, AI roles, dependencies, and believable failure states.

## Daily Simulation

Support wake-up, meals, medication and medical call-outs, work, school, official counts, programs, appointments, recreation, visitation, evening unit time, final count, and lockdown. Schedules react to staff shortages, emergencies, searches, maintenance failures, weather, court transfers, and overcrowding.

## Core Systems

- Relationship memories: familiarity, trust, respect, fear, gratitude, debt, grievance, professional confidence, and witnessed events.
- Economy: official account, work pay, commissary, approved property, favors, confiscation, and restitution.
- Incidents: evidence, witnesses, reports, temporary safety measures, review, findings, proportionate consequences, and appeals.
- Emergency escalation: medical events, missing persons, fights, fires, disturbances, attempted escapes, and infrastructure failures.
- AI schedules, needs, roles, perception, memory, and decision-making compatible with human-controlled roles.

## Technical Requirements

- Use a supported Unity LTS release and C#.
- Use dedicated, server-authoritative networking suitable for approximately 50 players.
- The server owns movement validation, inventory, currency, damage, discipline, permissions, rank, and persistence.
- Separate simulation rules, networking, persistence, UI, and presentation behind clear interfaces.
- Support secure authentication, reconnection, audit logs, moderation, and state recovery.
- Use interest management and budgeted AI updates for scale.
- Keep controls, UI scaling, assets, and performance portable enough for possible future mobile work.
- Do not add paid services without presenting costs and receiving approval.

## First Playable Vertical Slice

Build one small low-security institution for 8-12 test clients, architected to scale toward 50. Include intake, one housing unit, cells or cubicles, officer station, control room, dining hall, kitchen, medical room, classroom, workshop, commissary, visiting room, yard, segregation area, and secure service routes.

Implement one complete day with role selection, inmate intake, officer briefing, housing and post assignments, count, meal movement, one inmate job, one officer duty, yard, one social interaction, one minor violation, report and review, lockdown, saving, disconnecting, reconnecting, and correct state restoration.

## Art Direction

Use grounded stylized-realistic 3D art: believable proportions, simplified readable forms, concrete gray, desaturated blue-green, weathered steel, faded safety yellow, and restrained orange accents. Lower-security spaces are brighter and more open; higher-security spaces are denser and more controlled. Avoid grimdark horror, stereotypes, excessive grime, graphic gore, logos, and copied real-world branding.

Required art groups include modular architecture, security fixtures, department props, character bodies, inmate and staff clothing, hairstyles, animation, UI, decals, lighting, VFX, injury states, LODs, and collision meshes. Maintain an asset register with owner, status, performance budget, prefab path, LOD status, and last review date.

## Working Method

1. Inspect the project, Unity version, render pipeline, input system, packages, scenes, repository status, and compile errors.
2. Propose the architecture, data models, networking approach, persistence schema, AI model, prefab structure, and milestone plan.
3. Identify only genuinely blocking decisions; otherwise choose sensible defaults and proceed.
4. Implement small, testable stages beginning with an offline gray-box loop.
5. Add two-client networking before content expansion.
6. Compile, inspect console errors, run relevant tests, and verify in Play Mode after each major stage.
7. Record changed files, tests, limitations, and the next playable objective.
8. Do not attempt the complete game in one pass.

Begin by creating the project architecture and the smallest offline gray-box vertical slice described in `roadmap.md`.
