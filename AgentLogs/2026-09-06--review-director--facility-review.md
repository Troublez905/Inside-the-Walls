# Review Director - Facility Runtime Review

## Assignment - 2026-09-06

Independently review the planned runtime `FacilityVisualBuilder` work and
`PrototypePlayerController` refinement while they develop. Implementation is read-only
to this reviewer. Verify URP shader safety, traversal and obstruction, objective
coordinate readability, camera clipping, keyboard/controller parity, accessibility,
runtime object/performance budget, and complete integration with the playable-day loop.

## Controller First Review - 2026-09-06

- **P1 - controller parity remains unverified.** The new look path reads legacy
  `Debug Horizontal` and `Debug Vertical`, mapped to joystick axes 5 and 6. Those axis
  numbers are device/layout-dependent; existence in `InputManager.asset` is not proof
  of Xbox- and PlayStation-style right-stick operation.
- **P1 - choice input also moved the player.** The choice UI and locomotion both read
  `Horizontal`, so choosing an outcome translated the capsule.
- **P1 - camera cast included non-obstructions.** Casting against all default layers
  allowed objective-marker colliders and future decorative props to pull the camera in.
- **P2 - cast saturation and settings gaps.** The initial 12-hit non-allocating cast did
  not handle saturation. Look sensitivity, dead zone, and invert-Y remain hardcoded.

Positive findings: explicit gravity/grounding improves movement predictability; mouse
delta is no longer incorrectly multiplied by frame time; the sphere-cast direction,
self filtering, trigger exclusion, nearest-hit choice, and padding are structurally
sound.

Feedback was sent directly to `/root/offline_gameplay_design`.

## Controller Response And Re-review - 2026-09-06

The specialist added a movement-suppression contract and wires it on choice entry,
confirmation, and completion. Objective-marker camera hits are ignored, and saturated
non-allocating casts fall back to a complete `SphereCastAll` query. Static inspection
accepts these corrections.

Result: **conditionally accepted by static inspection.** Final acceptance still
requires a current Unity compile, approach tests at every objective and wall, both-axis
tests on representative controllers, and keyboard/mouse playthrough. Name-based marker
filtering is acceptable for this generated prototype but should become an explicit
camera-collision layer before content production. Sensitivity, invert-Y, and dead-zone
settings remain required accessibility work.

## Facility Builder Status - 2026-09-06

At this checkpoint no `FacilityVisualBuilder` file or implementation was present in the
shared tree. Repeated handoff/status requests were sent to the Environment and Technical
Art specialist and escalated to the lead. Facility integration therefore remains
**unreviewed and unaccepted** until implementation evidence appears.

## Facility Builder First Review - 2026-09-06

After the builder landed, inspection found:

- **P0 - objective/prop overlaps:** Housing, Dining, Laundry, and Officer Station
  objective markers intersected or touched tables/desks, reducing marker readability
  and creating avoidable approach/camera collisions.
- **P1 - room footprint overlap:** Housing and Dining overlapped along their adjacent
  footprints and side walls, producing an unintended seam/sliver.
- **P1 - material budget/lifecycle:** every primitive allocated a separate material
  despite a small shared palette; `sharedMaterial` did not make those allocations shared.
- **P1 - camera collision:** every low walk slab and prop used a default-layer collider,
  so non-structural geometry could pump the camera.
- **P1 - evidence timing:** earlier build evidence predates the builder and cannot verify
  this integration.
- **P2 - shader/sign risk:** the URP Lit/Unlit fallback is sensible, but runtime
  `Shader.Find` remains stripping-sensitive and `Sprites/Default` is a weak 3D fallback.
  TextMesh facing and contrast require visual verification.

The findings were sent immediately to the lead and Environment/Technical Art agent.

## Facility Corrections And Re-review - 2026-09-06

The lead moved the recurring Housing, Dining, Laundry, and Officer Station props away
from their objective coordinates; shifted and narrowed Dining to remove its overlap with
Housing; introduced a shared color/shader material cache with explicit reset; and made
camera obstruction ignore low surfaces and generated objective markers.

Static coordinate recheck confirms every objective now has a clear approach direction:

- Housing table-to-marker collider gap is approximately 0.85 m.
- Dining has a clear southern approach; its nearest rear table edge is approximately
  0.45 m beyond the marker collider.
- Laundry table-to-marker gap is approximately 0.9 m.
- Officer desk-to-marker gap is approximately 0.45 m with a clear southern approach.
- Housing ends at x=-7.5 and Dining begins at x=-6.5, leaving a 1 m footprint gap.
- The controlled gate opening and yard objective remain unobstructed.

The builder creates roughly fifty facility primitives plus six non-collider signs. Its
small cached palette is reasonable for this prototype; raw source art and high-density
character models are not loaded. Integration is present through
`PlayableDayController.BuildWorld -> FacilityVisualBuilder.Build`.

## Final Acceptance - 2026-09-06

- **Facility source implementation:** Accepted by static inspection.
- **Controller source implementation:** Conditionally accepted by static inspection.
- **Compilation and Windows build:** Accepted. `Logs/facility-pass-build-2.log` was
  written after the final builder/controller edits and records `Build Finished, Result:
  Success`, `FOUNDATION_BUILD_OK size=141319874 warnings=0`, and a successful batch exit
  with return code 0.
- **Interactive runtime gate:** Conditionally accepted. Both-role traversal, camera
  behavior at every objective, sign facing/contrast, shader appearance, 1280x720 at
  150% UI scale, and representative controller axes still require interactive evidence.

No remaining P0/P1 source defect was identified in the reviewed facility builder. Before
production art, replace height/name heuristics with explicit camera-collision layers,
replace runtime shader lookup with retained/serialized materials, and expose camera
look settings. For this gray-box milestone, those are tracked follow-up risks rather
than source-acceptance blockers.
