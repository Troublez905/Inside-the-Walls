# Inside the Walls - To-Do List

Use this checklist with `roadmap.md` and `prompt-insidethewalls.md`. Complete and verify each milestone before expanding the game's scope.

## 1. Secure the Project

- [ ] Revoke the GitHub personal access token previously exposed in chat.
- [ ] Create a replacement token only if GitHub CLI authentication is insufficient.
- [ ] Authenticate locally with `gh auth login` or a credential manager.
- [ ] Confirm no token, password, API key, or secret exists in any project file.
- [ ] Confirm the canonical repository is `https://github.com/Troublez905/Inside-the-Walls`.
- [ ] Confirm the local project directory is `C:\Users\ghost\Desktop\Ideas-Brainstorms\00-insidethewalls`.

## 2. Prepare the GitHub Repository

- [ ] Initialize Git in the local project directory if it is not already initialized.
- [ ] Connect the local repository to the canonical GitHub repository.
- [ ] Add a Unity-specific `.gitignore` before importing generated Unity files.
- [ ] Add `README.md`, `roadmap.md`, `prompt-insidethewalls.md`, and this checklist.
- [ ] Decide on a license and add `LICENSE` when ready.
- [ ] Add `CONTRIBUTING.md` after the development workflow is established.
- [ ] Add issue templates for bugs, features, art tasks, and technical tasks.
- [ ] Protect the main branch after the first stable project commit.
- [ ] Use feature branches and pull requests for reviewed changes.

## 3. Lock the Preproduction Scope

- [ ] Write a one-page game vision.
- [ ] Define the inmate core gameplay loop.
- [ ] Define the correctional-officer core gameplay loop.
- [ ] Write a clear list of features excluded from the first vertical slice.
- [ ] Define the first playable session from login through save and disconnect.
- [ ] Define success criteria for a 30-minute test session.
- [ ] Create a terminology guide for roles, ranks, departments, and facilities.
- [ ] Create a decision log for major design and technical choices.

## 4. Design the First Facility

- [ ] Choose the fictional facility name and location.
- [ ] Draw the low-security campus layout.
- [ ] Draw the intake and processing layout.
- [ ] Draw the first housing unit and cell layout.
- [ ] Place the dining hall, kitchen, yard, medical room, classroom, workshop, commissary, visiting room, officer station, and control room.
- [ ] Mark public, supervised, restricted, staff-only, and emergency-access zones.
- [ ] Check sightlines, navigation routes, secure doors, and gameplay bottlenecks.
- [ ] Create a complete in-game daily schedule.
- [ ] Define how the schedule changes during shortages, searches, emergencies, and lockdowns.

## 5. Finish the Art Bible

- [ ] Add the approved splash art and title treatment.
- [ ] Lock the stylized-realism visual rules.
- [ ] Lock the core color palette and lighting direction.
- [ ] Define environment dimensions and modular grid standards.
- [ ] Define material, texture-resolution, polygon, LOD, and collision budgets.
- [ ] Create prompts for the exterior, intake, housing unit, cell, yard, and control room.
- [ ] Create an asset register with status, owner, budget, prefab path, and review date.
- [ ] Confirm generated references are inspiration only unless their licenses permit direct use.

## 6. Finish the Character Bible

- [ ] Complete Marcus Vale's history, goals, relationships, secrets, and story branches.
- [ ] Complete Noah Mercer's history, goals, relationships, secrets, and story branches.
- [ ] Complete Officer Lena Ortiz's history, goals, relationships, secrets, and story branches.
- [ ] Complete Captain Elias Ward's history, goals, relationships, secrets, and story branches.
- [ ] Define each character's speech style without relying on stereotypes.
- [ ] Create a consistent turnaround and expression-sheet prompt for each character.
- [ ] Define inmate, officer-rank, medical, food-service, maintenance, and program-staff clothing.
- [ ] Define respectful character-creation options independent of morality, statistics, and affiliations.

## 7. Create the Unity Project

- [ ] Select and document a supported Unity LTS version.
- [ ] Select and document the render pipeline.
- [ ] Configure the Unity Input System.
- [ ] Create the project folder structure for code, scenes, prefabs, art, audio, UI, tests, and configuration.
- [ ] Add assembly definitions to keep major systems separated.
- [ ] Create development, test, client, and dedicated-server configurations.
- [ ] Confirm a clean clone opens and compiles without errors.
- [ ] Commit the empty working foundation.

## 8. Build the Offline Gray-Box Prototype

- [ ] Implement third-person movement.
- [ ] Implement the third-person camera and collision handling.
- [ ] Implement the interaction system.
- [ ] Add inmate and officer role selection.
- [ ] Gray-box intake, one housing unit, dining, yard, and officer station.
- [ ] Implement secure doors, keys, permissions, and restricted zones.
- [ ] Implement the simulation clock and basic schedule.
- [ ] Implement inmate intake and housing assignment.
- [ ] Implement officer shift briefing and post assignment.
- [ ] Implement one inmate job.
- [ ] Implement one officer duty.
- [ ] Implement one nonviolent social interaction.
- [ ] Save and load one local character.
- [ ] Play through one complete simplified day without errors.

## 9. Prove the Multiplayer Foundation

- [ ] Compare suitable Unity networking approaches and document the decision.
- [ ] Create a dedicated, server-authoritative host.
- [ ] Implement player authentication and persistent player identity.
- [ ] Synchronize spawning and role assignment.
- [ ] Synchronize validated movement.
- [ ] Synchronize doors, interactions, schedules, and permissions.
- [ ] Keep inventory, money, damage, discipline, rank, and progression server-owned.
- [ ] Implement disconnect and reconnect handling.
- [ ] Add audit logging for important administrative actions.
- [ ] Connect two clients and complete the daily loop.
- [ ] Add automated tests for authority, permissions, and reconnect behavior.

## 10. Build the Living Prison

- [ ] Implement shared role definitions for human and AI characters.
- [ ] Implement AI schedules, needs, navigation, posts, and basic perception.
- [ ] Add AI inmates, officers, and essential specialist staff.
- [ ] Add believable AI-to-human shift relief and reassignment.
- [ ] Implement counts, meals, work, programs, yard, medical call-outs, and lockdown.
- [ ] Implement relationship memory, trust, respect, gratitude, debts, and grievances.
- [ ] Implement separate reputation categories rather than one morality score.
- [ ] Verify the prison can complete a full day with no human players online.

## 11. Add Consequences and Progression

- [ ] Implement official accounts, work pay, commissary, inventory, and approved property.
- [ ] Implement incident evidence and witness accounts.
- [ ] Implement officer reports and supervisory review.
- [ ] Implement temporary safety measures, findings, sanctions, and appeals.
- [ ] Implement inmate classification reviews and transfer eligibility.
- [ ] Implement officer evaluations, training, certifications, and promotions.
- [ ] Make violence produce medical, disciplinary, staffing, security, and social consequences.
- [ ] Test one incident from occurrence through persistent resolution.

## 12. Produce Vertical-Slice Art

- [ ] Model the modular architectural kit.
- [ ] Model secure doors, gates, fences, stairs, railings, windows, and security fixtures.
- [ ] Model furniture and department props.
- [ ] Create character base bodies and customization options.
- [ ] Create inmate clothing, officer ranks, and specialist uniforms.
- [ ] Rig the character bases.
- [ ] Create locomotion, interaction, work, conversation, de-escalation, and restrained combat animations.
- [ ] Create role, schedule, map, inventory, relationship, report, classification, promotion, settings, and moderation UI.
- [ ] Create lighting, materials, decals, weather, alarms, particles, blood, and injury feedback.
- [ ] Create LODs, collision meshes, occlusion settings, and optimized prefabs.
- [ ] Review all assets against the art bible and performance budget.

## 13. Complete and Test the Vertical Slice

- [ ] Polish inmate intake and officer onboarding.
- [ ] Complete one full day and night cycle for both roles.
- [ ] Add two inmate jobs and two officer posts.
- [ ] Add education, commissary, visitation, medical care, and recreation.
- [ ] Add one minor and one serious incident path.
- [ ] Test saving, disconnecting, reconnecting, and state restoration.
- [ ] Test with 8-12 players or simulated clients.
- [ ] Run accessibility, usability, moderation, and recovery checks.
- [ ] Fix all release-blocking defects.
- [ ] Confirm testers can play for 30 minutes without developer intervention.

## 14. Scale Toward 50 Players

- [ ] Add network interest management.
- [ ] Add distance- and relevance-based AI simulation tiers.
- [ ] Profile server CPU, memory, bandwidth, physics, and database load.
- [ ] Test with 25 clients before testing with 50.
- [ ] Add rate limiting, anti-cheat validation, monitoring, backups, and rollback procedures.
- [ ] Document performance targets and test results.
- [ ] Pass a stable 50-client test before expanding to additional facilities.

## 15. Later Expansion

- [ ] Add minimum-, medium-, and high-security facilities.
- [ ] Add administrative and special-purpose facilities where narratively appropriate.
- [ ] Add transfers, reentry, additional careers, departments, programs, and stories.
- [ ] Expand accessibility, settings, community moderation, and account recovery.
- [ ] Prepare store materials, closed testing, crash reporting, deployment, and support workflows.
- [ ] Research a mobile adaptation only after the PC version is stable and polished.

## Current Priority

- [ ] Complete Sections 1 through 3.
- [ ] Do not begin full production art or 50-player networking until the offline gray-box loop works.

