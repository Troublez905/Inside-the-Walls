# Gameplay Systems - Controller Refinement

## Assignment

Improve the prototype third-person controller for the generated facility without changing facility, UI, scene, package, or project configuration files. Required work covers mouse/gamepad camera parity, camera obstruction, gravity/grounding, and preservation of current controls.

## Decisions

- Preserved WASD/left-stick movement, right-mouse camera orbit, E/A interaction, movement speed, pitch limits, and camera distance.
- Added right-stick look through the existing `Debug Horizontal` and `Debug Vertical` legacy axes. Those axes are already declared for joystick axes 5/6, so the controller does not query undefined axis names that would log every frame.
- Applied a dead zone and clamp to gamepad look, with frame-rate-independent angular speed.
- Kept mouse input as per-frame pointer delta rather than multiplying it by delta time.
- Replaced `CharacterController.SimpleMove` with explicit `Move`, grounded downward bias, gravity accumulation, and collision-flag grounding correction.
- Added a non-allocating sphere cast from the player focus toward the desired camera location. The nearest non-player obstruction pulls the camera forward with padding.
- Reacquires `Camera.main` when needed, preserving compatibility with the runtime-created camera order.
- Exposes `SetMovementSuppressed` so modal choices can own the horizontal axis without moving the capsule.
- Ignores runtime objective-marker colliders during camera obstruction and falls back to a complete allocating cast only if the reusable 12-hit buffer saturates.

## Reviewer Feedback And Response

- Review noted that joystick axes 5/6 are device-layout-dependent. The implementation uses the declared axes safely, but right-stick parity remains conditional until Xbox and PlayStation-style layouts are exercised; the Input Actions migration remains the long-term fix.
- Review found movement and choice selection shared the horizontal axis. The controller now exposes explicit movement suppression, and the lead approved a bounded presentation edit that enables it while a choice owns the axis and clears it after confirmation/completion.
- Review found the default collision mask could include orange objective markers. The controller explicitly ignores those generated marker colliders. Architecture still uses default collision layers because facility-layer configuration was outside scope.
- Review flagged fixed-buffer saturation. Dense-hit saturation now takes a rare `SphereCastAll` fallback rather than using an incomplete result.
- Review noted hardcoded look speed, inversion, and dead zone. These are explicitly outstanding settings work, not claimed complete.

## Files

- `Assets/_InsideTheWalls/Scripts/Characters/PrototypePlayerController.cs`
- `AgentLogs/2026-09-06--gameplay-systems--controller-refinement.md`

## Verification

- Confirmed `Horizontal`, `Vertical`, `Mouse X`, `Mouse Y`, `Debug Horizontal`, and `Debug Vertical` exist in the current legacy Input Manager before referencing them.
- Camera collision uses a fixed reusable hit buffer and introduces no per-frame collection allocation.
- Dense collision overlap can allocate only when the fixed buffer saturates; this favors correct obstruction over an incomplete hit set.
- Static diff validation completed. Unity compile and physical-controller playtesting remain for the coordinated integration pass.

## Status

Implementation complete and compile-ready by inspection. Final acceptance requires Unity compilation, camera checks at every objective, and keyboard/mouse plus Xbox/PlayStation-style controller playtesting. Look sensitivity, inversion, and dead-zone settings remain outstanding.
