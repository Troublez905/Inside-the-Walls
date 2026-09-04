# Inside the Walls Development Roadmap

## Milestone 0: Project Setup

- Initialize the Unity project and Git repository.
- Add `.gitignore`, contribution rules, directory conventions, and GitHub issue templates.
- Select the Unity LTS version, render pipeline, input system, and coding standards.
- Create an asset register and decision log.

**Done when:** The project opens without errors, a clean clone can run it, and no secrets or generated build files are tracked.

## Milestone 1: Game and Technical Design

- Finish the one-page vision, role loops, system boundaries, and scope exclusions.
- Create the first facility floor plan and department map.
- Define player, AI, schedule, relationship, reputation, incident, inventory, and persistence data models.
- Choose the networking, backend, database, and hosting approach through a small technical comparison.

**Done when:** Every vertical-slice feature has an owner, dependency, test, and acceptance condition.

## Milestone 2: Offline Gray-Box Prototype

- Build third-person movement, camera, interaction, and animation hooks.
- Gray-box intake, one housing unit, dining, yard, officer station, and secure routes.
- Add role selection, doors, keys, permissions, a schedule clock, and local saving.
- Implement one inmate task and one officer duty.

**Done when:** One player can complete a full simplified day and restore saved progress.

## Milestone 3: Multiplayer Foundation

- Create a dedicated, server-authoritative host.
- Synchronize spawning, movement, interactions, doors, schedules, and role state.
- Validate commands on the server.
- Add authentication, disconnect/reconnect, logs, and two-client automated tests.

**Done when:** Two computers can complete the daily loop and reconnect to correct state.

## Milestone 4: Living Prison

- Add AI inmates, officers, and specialist staff.
- Implement schedules, needs, posts, perception, navigation, and relationship memory.
- Add counts, meals, jobs, programs, recreation, medical call-outs, and lockdowns.
- Allow humans to relieve AI from roles through believable shift transitions.

**Done when:** The facility runs coherently for a full simulated day without human players.

## Milestone 5: Consequence Systems

- Add reputation, trust, favors, debts, grievances, and witnessed-event memory.
- Add server-owned inventory, official money, commissary, pay, and property.
- Add reporting, evidence, investigations, hearings, discipline, classification review, and appeals.
- Add officer evaluations, training, certifications, and promotions.

**Done when:** One incident produces persistent and explainable consequences for both roles.

## Milestone 6: Art Production

- Replace gray boxes with a modular low-security environment kit.
- Produce character bases, uniforms, hairstyles, props, UI, animation, lighting, decals, and restrained VFX.
- Build LODs, collision, occlusion, texture budgets, and reusable prefabs.
- Review every asset against the art guide and asset register.

**Done when:** The entire vertical slice has one coherent visual language and meets its performance budget.

## Milestone 7: Vertical Slice

- Polish intake and officer onboarding.
- Complete one day/night cycle for both roles.
- Add two inmate jobs, two officer posts, education, commissary, visitation, medical care, and minor and serious incidents.
- Test persistence, accessibility, moderation, and recovery.

**Done when:** 8-12 testers can play for at least 30 minutes in either role without developer intervention.

## Milestone 8: Scale Testing

- Increase tests to 25 and then 50 clients.
- Profile bandwidth, server CPU, memory, AI frequency, physics, and database load.
- Add interest management, AI simulation tiers, rate limiting, anti-cheat checks, and operational dashboards.

**Done when:** A 50-client test meets documented stability and performance targets.

## Milestone 9: Expansion and Release Preparation

- Add additional security levels, transfers, reentry, departments, careers, and stories.
- Complete tutorials, accessibility, settings, account recovery, moderation, privacy, and community policies.
- Prepare store materials, closed testing, crash reporting, deployment, backups, and rollback procedures.
- Research mobile controls and performance only after the PC build is stable.

**Done when:** The release candidate passes gameplay, security, performance, accessibility, and operational checks.

## Art Production Checklist

- Modular walls, floors, ceilings, cells, doors, gates, fences, stairs, railings, windows, and roofs
- Intake, control, housing, dining, kitchen, medical, education, workshop, commissary, visiting, yard, laundry, chapel, and maintenance props
- Character bodies, heads, hair, inmate clothing, officer ranks, and specialist uniforms
- Locomotion, sitting, sleeping, eating, working, exercising, searching, escorting, radio, treatment, conversation, de-escalation, surrender, and restrained combat animation
- Role selection, character creation, schedule, map, permissions, inventory, commissary, relationships, reports, classification, promotion, settings, and moderation UI
- Lighting profiles, material library, signage, decals, weather, blood, injury feedback, alarms, particles, audio hooks, LODs, and collision

## Character Foundations

- **Marcus Vale:** A 41-year-old inmate and former warehouse supervisor whose misplaced loyalty and financial decisions brought him into the system. Calm and useful, he guides new arrivals while balancing old debts and a damaged family relationship.
- **Noah Mercer:** A 24-year-old first-time inmate and former construction worker. Frightened but defensive, he can pursue education and release preparation, prison influence, or an unstable mixture of both.
- **Officer Lena Ortiz:** A 28-year-old probationary officer who previously worked overnight security. She believes communication prevents unnecessary conflict, but institutional pressure tests her judgment and boundaries.
- **Captain Elias Ward:** A 49-year-old security commander with two decades of experience. A past disturbance saved lives but made him wary of rapid change; he evaluates rookies by their decisions under pressure.

