# Offline NPC interactions

- Added three inmates and two officers using articulated cube figures, shared URP materials, names and role labels.
- Added short local patrols using CharacterController collision, mild shove recoil, defensive arm pose and spoken responses.
- Input is sampled only through the population Tick: E/A talk, F/X shove, G/LB guard, R/Y calm. Objectives own E/A. Blocked states clear guard and discard actions.
- Short range and line of sight gate encounters. Pure encounter rules enforce cooldown and bounded rapport/tension; state is explicitly transient and never added to saves.
- NUnit coverage added for blocked/occluded/out-of-range actions, objective priority, cooldown and de-escalation bounds.
- Pure rules compiled with PowerShell Add-Type; shove, cooldown rejection and calm-down checks passed. NUnit coverage is authored; Unity test execution belongs to the lead.
- Public Npcs collection and TryAct(PrisonNpc, NpcAction, out string), TryTalk, TryCalm, TryShove share the keyboard range/visibility/state rules for integrated verification.
- Guard suppresses shoving with clear feedback. It is a defensive social stance, not a damage-blocking combat system. Status responses expire after six seconds.
- Source inspection completed. Lead owns Unity compile, build and visual smoke checks. No networking, injury, weapons, pathfinding or persistent relationships are claimed.
- Complete implementation; visual quality and controller interaction require the integrated Unity smoke test.
