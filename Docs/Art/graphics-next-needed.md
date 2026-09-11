# Inside the Walls - Next Graphics Needed

This list is based on the current playable code, asset inventory, existing concept boards, and the Noah Mercer source-rig audit. The broad concept-art set is already complete. These requests therefore target individual, production-ready assets that can replace visible gray-box elements in the next builds. Keep generated images as reference unless their rights permit direct use.

## Delivery Status - 2026-09-06

All fourteen request titles now have matching boards in `Docs/Art/` and copies under `Assets/_InsideTheWalls/`. Every board is classified as **reference only**:

- Material boards (#1-#2) show map layouts but are not crop-ready seamless textures.
- Architecture/prop boards (#3-#8) are modeling sheets, not FBX/collision/LOD deliveries.
- UI boards (#9-#10) are composite sheets; #9 leans sci-fi neon and needs a restrained institutional export pass before HUD use.
- Character boards (#11-#14) are visual targets, not retopoed Humanoid FBX clips.

The playable gray-box has been restyled from these references. Production meshes, tileable PBR sets, and animation clips remain outstanding.

## Priority Key

- **P0:** Immediately improves the current offline playable.
- **P1:** Needed after the first environment replacement pass.
- **P2:** Useful polish after the runtime art pipeline is proven.

## Individual Graphics Requests

### 1. Painted Concrete-Block Wall Material

- **Priority:** P0
- **Intended Unity use:** Replaces flat wall colors throughout intake, housing, dining, laundry, and the officer station.
- **Technical delivery:** Seamless 2048x2048 PBR set: Base Color, tangent-space Normal, and URP Mask Map; real-world scale 2 m; no baked lighting; clean and lightly worn variants; PNG/TGA source plus configured URP material.
- **Prompt (529 characters):** Create one seamless painted concrete-block wall material for a stylized-realistic Unity prison simulator. Cool desaturated blue-green paint over believable masonry, subtle roller variation, restrained scuffs near traffic height, worn but maintained, readable mortar at a two-meter viewing scale. Neutral flat lighting, orthographic texture presentation, no perspective, no baked shadows, no stains suggesting neglect, no graffiti, blood, text, logos, or watermark. Supply clean and lightly worn variations with matching PBR maps.

### 2. Institutional Vinyl Floor Material

- **Priority:** P0
- **Intended Unity use:** Gives each interior room a grounded surface while preserving clear navigation and objective readability.
- **Technical delivery:** Seamless 2048x2048 Base Color, Normal, and URP Mask Map; real-world scale 2 m; subtle roughness variation; clean and trafficked variants; configured URP material.
- **Prompt (493 characters):** Create one seamless institutional vinyl floor material for a stylized-realistic Unity correctional setting. Muted gray-green tiles with faint aggregate, soft waxed roughness variation, subtle foot-traffic wear, and clean maintained edges. Designed to remain quiet beneath UI markers and characters at gameplay distance. Neutral flat lighting, no perspective, no baked reflections, no readable markings, dirt piles, blood, logos, or watermark. Include clean and lightly trafficked PBR variants.

### 3. Modular Interior Wall Segment

- **Priority:** P0
- **Intended Unity use:** First production architecture module for replacing primitive room walls and proving scale, snapping, collision, and material reuse.
- **Technical delivery:** FBX, 4 m wide x 3 m high x 0.2 m deep; bottom-center pivot; 1 m grid; UV0 and non-overlapping UV1; simple box collision; LOD0/LOD1/LOD2 targets 2,000/1,000/300 triangles; uses shared wall material.
- **Prompt (489 characters):** Design one production-ready four-meter interior wall module for Inside the Walls, three meters tall, painted concrete block with a durable dark-steel base trim. Grounded stylized realism, believable thickness, simplified readable forms, worn but maintained edges, and no unique damage that reveals tiling. Show orthographic front, side, top, and a clean three-quarter view with pivot and one-meter grid. No door, window, signs, text, graffiti, real branding, tactical detail, or watermark.

### 4. Secure Interior Door

- **Priority:** P0
- **Intended Unity use:** Replaces the primitive controlled-movement gate and becomes the visual target for door/permission interactions.
- **Technical delivery:** Separate frame, hinged leaf, handle, and small vision panel; FBX on 1 m grid; hinge pivot authored; LOD0/LOD1/LOD2 targets 8,000/4,000/1,000 triangles; convex/simple collision; one 2048 PBR texture set; no functional lock internals.
- **Prompt (525 characters):** Design one fictional secure interior door for a stylized-realistic low-security institution. Heavy painted steel leaf, reinforced frame, small safe vision panel, simple institutional handle, restrained faded-yellow identification strip, worn but maintained finish. Show closed and partly open states plus orthographic front, side, and three-quarter views; clearly separate frame, leaf, handle, and hinge pivot for animation. No real manufacturer, readable codes, exposed lock mechanism, tactical diagram, blood, or watermark.

### 5. Intake Property Bin

- **Priority:** P0
- **Intended Unity use:** Replaces the current property-bin block and supplies a close-range objective prop in intake.
- **Technical delivery:** Stackable lidded plastic bin; FBX; 0.6 m x 0.4 m x 0.32 m; separate lid; bottom-center pivot; LOD0/LOD1 3,000/800 triangles; box collision; one 1024 PBR set; six instances share one material.
- **Prompt (454 characters):** Design one stackable intake property bin for Inside the Walls. Durable molded plastic in muted desaturated blue, fitted lid, recessed handholds, rounded safe corners, slight everyday scuffing, and a blank removable label holder. Grounded stylized realism with a strong readable silhouette. Show orthographic front, side, top, open-lid, and three-quarter views with scale. No personal information, readable labels, brands, contraband, blood, or watermark.

### 6. Laundry Cart

- **Priority:** P0
- **Intended Unity use:** Replaces three primitive laundry blocks and becomes the visual anchor for the inmate work objective.
- **Technical delivery:** FBX cart with separate caster wheels and optional cloth load; approximately 1.1 m x 0.7 m x 0.9 m; LOD0/LOD1/LOD2 7,000/3,000/800 triangles; simple collision; one 2048 atlas; interaction anchor at handle.
- **Prompt (476 characters):** Design one institutional laundry cart for Inside the Walls, sized for a single worker. Tubular weathered-steel frame, muted blue washable fabric liner, four caster wheels, ergonomic push handle, restrained use wear, and an optional neatly folded linen load. Stylized realism, clear silhouette from third-person camera distance. Show empty and loaded states with orthographic front, side, top, and three-quarter views. No brands, restraints, weapons, text, blood, or watermark.

### 7. Fixed Dining Table

- **Priority:** P1
- **Intended Unity use:** Replaces primitive dining tables and establishes believable proportions and navigation clearance.
- **Technical delivery:** Four-seat fixed table with rounded stools; FBX; approximately 1.8 m x 1.0 m; LOD0/LOD1/LOD2 7,000/3,000/700 triangles; simple compound collision; one 2048 shared metal material; floor-mount points indicated but nonfunctional.
- **Prompt (486 characters):** Design one four-seat fixed dining table for a humane low-security institution. Brushed stainless tabletop with softly rounded corners, four attached round stools, sturdy simplified floor-mounted frame, subtle clean wear, and generous leg clearance. High-end stylized realism with a readable silhouette and safe proportions. Show orthographic front, side, top, and three-quarter views with a standing adult scale figure. No loose utensils, brands, text, sharp edges, blood, or watermark.

### 8. Yard Exercise Rail

- **Priority:** P1
- **Intended Unity use:** Replaces the current single metal bar and supports a later exercise interaction animation.
- **Technical delivery:** Freestanding low-risk pull/stretch rail; FBX; 3 m wide; LOD0/LOD1 4,000/1,000 triangles; capsule/box collision; one 1024 painted-metal PBR set; interaction grips and foot clearance documented.
- **Prompt (474 characters):** Design one modest outdoor exercise rail for a low-security recreation yard. Powder-coated weathered steel, broad stable posts, rounded connections, faded safety-yellow grip zones, and believable anchored feet. Intended for stretching and basic pull exercises, not an obstacle course. Stylized realism, readable at distance. Show orthographic front, side, top, and three-quarter views with an adult scale figure. No tactical use diagrams, brands, text, weapons, or watermark.

### 9. Objective Interaction Marker

- **Priority:** P0
- **Intended Unity use:** Replaces or complements primitive objective markers for intake, count, meal service, laundry, yard, and report interactions.
- **Technical delivery:** Text-free vector/SVG plus 256px transparent PNG; neutral, available, active, complete, and unavailable visual states; radial 16-frame pulse spritesheet; shape differs by state; readable at 100%, 150%, and 200% UI scale.
- **Prompt (530 characters):** Create one text-free objective interaction marker system for Inside the Walls. Institutional count-board influence: a crisp outer bracket, simple central action dot, and restrained orange progress ring. Show neutral, available, active, complete, and unavailable states using shape as well as color. Calm and utilitarian, readable over light or dark scenes, with a subtle pulse and static reduced-motion option. Flat vector presentation, transparent background, no letters, numbers, logos, gradients that muddy edges, or watermark.

### 10. Schedule Phase Icon - Official Count

- **Priority:** P1
- **Intended Unity use:** Identifies the count phase in HUD schedule updates without relying only on text or color.
- **Technical delivery:** Single SVG plus 128/256/512px transparent PNG exports; filled and outline variants; 2 px minimum stroke at 128 px; monochrome-safe; normal, urgent, and complete state treatments.
- **Prompt (428 characters):** Design one accessible icon for the official-count schedule phase in Inside the Walls. Use an original geometric motif of three aligned person markers inside a squared verification frame, calm institutional document style, strong silhouette, and even visual weight. Show filled and outline versions plus normal, urgent, and complete states. No text, numbers, prison bars, handcuffs, real insignia, faces, gradients, or watermark.

### 11. Optimized Noah Mercer Runtime Body

- **Priority:** P0
- **Intended Unity use:** Replaces the player capsule only after Humanoid avatar and locomotion validation; derived from the preserved source/showcase candidate.
- **Technical delivery:** Retopologized skinned FBX on Unity Humanoid skeleton; LOD0/LOD1/LOD2 targets 45k/22k/8k triangles; maximum four bone influences; one 2048 body/clothing atlas plus 1024 head detail; Base Color, Normal, Mask Map; neutral A-pose; no animation baked into model.
- **Prompt (545 characters):** Create a production-ready runtime interpretation of Noah Mercer using the approved existing character reference, preserving his recognizable face, proportions, issued clothing, restrained demeanor, and grounded stylized-realistic look. Build clean deformation-friendly topology, simplified layered garments, readable hands and footwear, and a silhouette distinct from officers. Neutral A-pose with front, side, back, wireframe, and material views. No redesign, gang symbols, brands, weapons, exaggerated muscles, stereotype coding, or watermark.

### 12. Noah Mercer Idle Animation

- **Priority:** P0
- **Intended Unity use:** First animation gate for the optimized player model and Animator Controller; removes the static mannequin look.
- **Technical delivery:** Humanoid FBX clip; in-place; 30 fps; 4-6 second seamless loop; foot locking; minimal root drift; relaxed and guarded variants; hands clear of body; tested on optimized Noah rig.
- **Prompt (465 characters):** Animate one seamless in-place idle for Noah Mercer on a neutral Unity Humanoid rig. He is alert but trying to appear composed: natural breathing, subtle weight shift, relaxed shoulders with slight guarded tension, small eye and head movement, and hands resting clear of the torso. Grounded everyday acting, stable planted feet, no theatrical fidgeting. Provide front and side motion reference. No aggression, combat stance, restraints, smoking, props, or watermark.

### 13. Noah Mercer Walk Cycle

- **Priority:** P0
- **Intended Unity use:** Hooks directly to the existing third-person movement speed and proves runtime retargeting.
- **Technical delivery:** Humanoid FBX; in-place and root-motion versions; 30 fps; approximately 1 second loop at 1.5 m/s; clean heel/toe contacts; mirrored foot timing; no root yaw; tested at 4.5 m/s blend scaling.
- **Prompt (470 characters):** Animate one natural walk cycle for Noah Mercer on a Unity Humanoid rig. Believable adult pace, compact institutional clothing, moderate stride, clear heel-to-toe contacts, restrained arm swing, attentive posture, and slight guardedness without menace. Seamless loop with stable hips and no foot sliding. Show side and three-quarter motion reference and supply in-place and root-motion intent. No swagger caricature, combat posture, limp, props, restraints, or watermark.

### 14. Probationary Officer Runtime Character

- **Priority:** P1
- **Intended Unity use:** Gives officer role selection and officer objectives a distinct playable silhouette after the inmate pipeline is validated.
- **Technical delivery:** Skinned FBX on the same Unity Humanoid skeleton as Noah; LOD0/LOD1/LOD2 45k/22k/8k triangles; four bone influences maximum; one 2048 uniform atlas plus 1024 head detail; separate radio and key-ring props; no weapon emphasis.
- **Prompt (559 characters):** Create a production-ready probationary correctional officer character based on the approved Lena Ortiz reference. Preserve her thoughtful, composed identity and practical fictional uniform. Grounded stylized realism, believable adult proportions, deformation-friendly topology, restrained radio and key-ring silhouette, readable rank difference, and shared Unity Humanoid compatibility with the inmate rig. Show neutral A-pose, front, side, back, wireframe, and material views. No real patches, action-hero styling, weapon emphasis, stereotypes, or watermark.

## Recommended Production Order

1. Prove the environment pipeline with items 1-6 and replace the largest primitive shapes.
2. Prove the character pipeline with items 11-13 before commissioning additional character meshes.
3. Add items 7-10 to improve objective readability and room identity.
4. Produce item 14 only after the shared Humanoid and LOD gates pass.

## Helpful Non-Graphics Actions From The User

- Confirm whether generated reference images may be used only as inspiration or whether any have documented commercial-use rights.
- Choose a target visual-performance tier: recommended baseline is 1080p/60 fps on a mid-range Windows PC.
- Install Blender 5.x and keep its command-line executable available; the existing Noah export pipeline already uses Blender.
- Learn or arrange retopology and skin-weight cleanup; the current Noah source is about 400,000 triangles and exceeds four bone influences.
- Record simple walk and idle phone videos from front and side in plain clothing; these are useful motion references without requiring specialist equipment.
- Test each Windows build with keyboard/mouse and a controller, especially objective-marker readability and camera obstruction.
- Keep every downloaded asset's store URL, author, license, version, and modification notes in the asset register.
- Avoid buying a large environment pack until one modular wall, door, prop, material, and character have passed the real scene's performance test.

## Existing Graphics Not Requested Again

The repository already contains reference images for the splash, title, menu, loading screen, role portraits, gray-box materials, modular architecture, facility layout, intake, housing, dining, yard/officer station, remaining rooms, principal characters, modular character set, animation reference, gameplay UI, department props, VFX, and future facilities. It also contains three textured inmate source models and a source-quality Noah FBX. These remain useful references, but they are not evidence of production-ready runtime assets.
