# Environment Art Graphical Pass

## 1. Completed

- Expanded the runtime facility from flat gray-box volumes into readable institutional zones with wall bands, front beams, backed signs, path striping, laundry machine faces, movement-board lines, exercise rail supports, and perimeter rhythm.
- Added five emissive ceiling fixtures and two shadowless security point lights for restrained warm/cool contrast.
- Kept the original gameplay layout, objective coordinates, open room fronts, and 3.2 m controlled-movement opening unchanged.
- Removed colliders from every new decorative object. The movement opening contains no decorative bars, avoiding a visually closed but traversable gate.
- Replaced per-color materials with one cached runtime surface material and per-renderer `MaterialPropertyBlock` values. Shader lookup is URP Lit, URP Unlit, then Sprites/Default, with an explicit error if none exists.

## 2. Files changed

- `Assets/_InsideTheWalls/Scripts/Presentation/FacilityVisualBuilder.cs`
- `AgentLogs/2026-09-06--environment-art--graphical-pass.md`

## 3. Verification

- `git diff --check` passed for the implementation file.
- Static review confirms all new route markings, signage panels, bands, beams, machine faces, board details, fixtures, rails, and perimeter details use `DecorativeBlock`/`DecorativeDisc`, which destroy their primitive colliders.
- Material creation is outside all frame loops and is bounded to one facility surface material per selected shader. Color/metal/emission variation uses property blocks.
- Runtime additions are fixed loops and fixed arrays. Static inspection counts 130 generated environment GameObjects, including 128 renderer-bearing objects and two lights. These are conservative draw/object proxies rather than measured frame draw calls.
- A fresh Unity 6000.3.0f1 `-noUpm` Windows development build passed with `FOUNDATION_BUILD_OK size=141366798 warnings=0` and process return code 0. Log: `Logs/environment-graphical-pass-noupm-build.log`.

## 4. Limitations and risks

- An in-game visual/collision walkthrough remains unverified; the successful build verifies compilation but not route traversal by a player.
- Draw calls and realtime-light cost are bounded by design but have not been measured in the Unity Profiler.
- The environment remains primitive-based placeholder art; it improves hierarchy and readability without replacing the future modular asset pipeline.

## 5. Decisions

- Used shared primitive meshes and a single property-block-driven material to keep this runtime prototype deterministic and package-free.
- Kept decorative geometry collision-free to protect navigation and interaction reachability.
- Used safety orange/yellow sparingly for paths, sign accents, and the perimeter rail while keeping the primary palette worn concrete, dark metal, muted blue, and wood.

## 6. Status

Implementation and compile/build verification are complete. Play-mode visual, route-traversal, and Profiler verification remain recommended.
