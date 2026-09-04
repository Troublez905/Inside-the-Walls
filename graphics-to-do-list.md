# Inside the Walls - Graphics Production Register

## Purpose and scope

This is the source-of-truth art register for the Foundation Slice and the later vertical slice. It deliberately separates concept/reference work from assets that can ship in Unity. No generated image or raw DCC export is production-ready until it passes the completion checks in this document.

Priority labels:

- **P0 - Required now:** needed for Slice A or to establish the gray-box art language.
- **P1 - Vertical slice:** needed for the 8-12 player low-security slice after the offline gray-box gate passes.
- **P2 - Later expansion:** useful after the vertical slice is coherent and on budget.

Readiness labels:

- **REF:** reference/concept only; never imported as final gameplay art without rights, cleanup, and technical review.
- **PROD:** production-ready deliverable target; must satisfy geometry, texture, naming, collision, LOD, and Unity validation checks.
- **PLACEHOLDER:** deliberately temporary and visibly tracked for replacement.

## Art-direction base (mandatory in every prompt)

> High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind.

## Existing references

| File | Label | Use | Technical decision |
|---|---|---|---|
| `exec-5a330b2e-ae91-4376-a4a8-ad126ac3ce45.png` | REF / candidate approved key art | Mood, palette, title hierarchy, wet-yard lighting | Suitable as pre-release splash only after ownership/license and resolution are confirmed. It includes baked text and a real-country flag motif; create a fictionalized, text-free master before production use. |
| `riverbend-05.jpg` | REF only | Dining furniture density, ceiling grid, durable floor palette | Photographic reference, not a texture or shipping backdrop. Do not reproduce identifiable facility signage or layout one-to-one. |

## Naming, scale, and delivery rules

- Unity units are meters; model at 1:1 scale, Y-up, +Z forward. Apply transforms before export and place interaction pivots at hinges, handles, feet, or floor contact as appropriate.
- Source files: `SRC_<Category>_<Name>_v###.blend`. Runtime meshes: `SM_` static, `SK_` skinned, `AN_` animation, `T_` texture, `M_` material, `MI_` material instance, `UI_` sprite, `VFX_` effect, and `PF_` prefab.
- Authoring handoff is `.blend`; Unity delivery is `.fbx` until the project pipeline is locked. Keep clean `.glb` exports available for validation/portability. Do not ship raw generator meshes or DCC-native dependencies.
- PBR textures use PNG/TGA masters: Base Color (sRGB), Normal (linear), and packed Mask `R=metallic, G=AO, B=detail/unused, A=smoothness`. UI uses PNG or SVG where supported; transparency is straight alpha with clean edge padding.
- Reuse trim sheets, tileables, atlases, and material instances. Texture figures below are maxima, not targets. Avoid unique 4K maps for repeated props.
- Static architecture has simple authored collision; interactive doors use separate primitive colliders. Never use detailed render meshes as MeshColliders on repeated assets.
- LOD0/1/2 targets are approximately 100/50/20 percent of listed triangle budget; large hero structures may add impostor or cull tiers. LOD transitions require visual review in third-person camera conditions.
- Character basis: one humanoid-compatible shared skeleton, consistent bone names, root at ground, separate clothing meshes, blendshape-ready face, and no identity-linked gameplay attributes.

## Practical performance budgets

| Class | LOD0 guideline | Textures | Collision / batching |
|---|---:|---|---|
| Modular architecture piece | 0.2-15k triangles | 1-2K shared trim/tileable | Primitive/low-poly collision; static batch compatible |
| Small/medium prop | 0.3-8k triangles | 512-1K atlas/shared | Primitive or convex proxy |
| Hero prop/door/security fixture | 5-20k triangles | 1-2K | Separate interaction collider and pivot |
| Environment landmark/building set | 80-250k visible LOD0 set | 2K shared sets; rare 4K atlas | Sectorized collision and occlusion-ready hierarchy |
| Character including clothing/hair | 45-70k triangles | 2K body/head + 1-2K clothing | Capsule gameplay collider; ragdoll only if approved later |
| UI screen | vector/9-slice preferred | 1x/2x sprites, 2048 atlas max | No physics; safe-area and scaling tests |

## P0 - Required now

### ITW-UI-001 - Splash key art and clean background

- **Label/type:** REF master plus PROD 2D raster derivatives.
- **Purpose/location:** boot splash and marketing reference; clean background also feeds the main menu.
- **Variants/views:** 16:9 composed key art with title; identical text-free background; 21:9 and 4:3 crops; dark accessibility-safe overlay plate.
- **Specification:** 7680x4320 lossless master, 3840x2160 and 2560x1440 PNG delivery; opaque; sRGB; no rig/collision/LOD. Preserve a central and left-side UI-safe zone. Target compressed runtime texture <= 8 MB per displayed variant.
- **Unity target:** `Assets/_InsideTheWalls/Art/UI/Frontend/KeyArt/`; `PF_UI_Splash`.
- **Dependencies:** rights review of existing PNG; final fictional facility identity; logo asset; menu safe-area spec.
- **Completion checklist:** [ ] source/licence recorded [ ] title-free master exists [ ] no real logos/flags [ ] crop set reviewed at 16:9, 21:9, 4:3 [ ] text remains live UI, not baked [ ] Unity compression and color reviewed [ ] controller prompt area remains clear.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Cinematic low-security institution courtyard at blue hour after light rain, open but controlled architecture, perimeter fence and modest observation tower, warm interior windows against cool clouds, a few distant neutral human silhouettes, calm institutional tension rather than menace, strong depth layers, wide 16:9 composition, generous clean negative space in the upper center and left for a separately rendered title, no text, no flags, no identifiable real facility, no weapons in focus. Produce concept reference plus a matching clean background plate.

### ITW-UI-002 - Logo and title lockup

- **Label/type:** REF typography exploration leading to PROD 2D vector.
- **Purpose/location:** splash, main menu, loading, store placeholders, documentation covers.
- **Variants/views:** stacked and horizontal; title only; title plus `Nobody leaves unchanged.`; one-color, light, dark, and restrained-orange accent variants.
- **Specification:** SVG source with outlined delivery copy; 4096px transparent PNG fallback; straight alpha; no rig/collision/LOD. Typography must stay readable at 320px width and avoid distressed micro-noise.
- **Unity target:** `Assets/_InsideTheWalls/Art/UI/Brand/`; `PF_UI_TitleLockup`.
- **Dependencies:** original typeface license or custom lettering; accessibility contrast review.
- **Completion checklist:** [ ] title and tagline exact [ ] type rights documented [ ] silhouette reads at small size [ ] light/dark/mono variants [ ] transparent edges clean [ ] no real insignia [ ] SVG and PNG import tested.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Graphic-design exploration for the original game title INSIDE THE WALLS and tagline Nobody leaves unchanged. Bold condensed institutional lettering, measured weathering, subtle concrete and stamped-document influence, restrained orange divider accent, authoritative but human rather than horror, clean vector-like edges, stacked and horizontal lockups, monochrome and reversed versions on a neutral presentation sheet. No copied type treatments, no prison-brand logos, no extra words.

### ITW-UI-003 - Main-menu background and controls

- **Label/type:** PROD 2D background, vector/9-slice UI controls, REF layout sheet.
- **Purpose/location:** Slice A main menu with New Game, Continue, Settings, Credits, Quit.
- **Variants/views:** background with day-to-blue-hour subtle loop option; buttons default, hover, pressed, focused, disabled; keyboard/controller focus ring; 16:9, 21:9, 4:3 framing.
- **Specification:** background shares ITW-UI-001 clean 4K master; optional loop 8-12 seconds, 1440p H.264/WebM only after runtime test. Buttons SVG plus 512x128 PNG fallbacks; straight alpha; 48px minimum effective control height at 1080p.
- **Unity target:** `Assets/_InsideTheWalls/Art/UI/Frontend/Menu/`; `PF_UI_MainMenu`.
- **Dependencies:** logo, font licensing, input glyph set, UI implementation spec.
- **Completion checklist:** [ ] all five states distinct without color alone [ ] disabled state unmistakable [ ] WCAG-inspired contrast check [ ] 200% UI scale [ ] ultrawide and 4:3 crops [ ] mouse/keyboard/controller focus parity [ ] no baked menu text.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Main-menu UI concept over a text-free low-security courtyard background, quiet blue-hour atmosphere, left-aligned navigation safe zone with strong legibility, thin weathered-steel panels and faded safety-yellow focus line, demonstrate default, hover, pressed, keyboard-controller focused, and visibly disabled button states. Keep typography and labels on a separate presentation layer; background deliverable contains no text or logos.

### ITW-UI-004 - Loading screen and indicator

- **Label/type:** PROD 2D illustration/crop and UI animation sprites or vector.
- **Purpose/location:** transitions from menu to role selection and prototype scene.
- **Variants/views:** one neutral facility crop; indicator idle/loading/error; reduced-motion variant.
- **Specification:** 3840x2160 opaque PNG master, 2560x1440 runtime derivative; 256x256 indicator SVG or 16-frame transparent PNG atlas; no rig/collision/LOD.
- **Unity target:** `Assets/_InsideTheWalls/Art/UI/Frontend/Loading/`; `PF_UI_LoadingScreen`.
- **Dependencies:** key-art background, loading-state API, accessibility motion setting.
- **Completion checklist:** [ ] readable status-text safe area [ ] indicator communicates activity without color [ ] reduced-motion still frame [ ] no fake progress implication [ ] all aspect ratios tested.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Minimal loading-screen concept using a quiet secure corridor leading toward daylight, broad uncluttered dark area for live status text, abstract indicator inspired by a rotating institutional count-board marker, restrained motion frames, include reduced-motion static state and error state, no words or numbers baked into art.

### ITW-UI-005 - Role-selection portraits

- **Label/type:** REF character portrait concepts; PROD 2D portrait renders only after approved character models exist.
- **Purpose/location:** inmate versus probationary officer selection.
- **Variants/views:** two neutral waist-up silhouettes/portraits; selected, unselected, unavailable; male/female presentation diversity without gameplay implications.
- **Specification:** 2048x2048 transparent PNG per portrait; alpha; no rig/collision/LOD for UI render. Maintain head-and-shoulder safe crop.
- **Unity target:** `Assets/_InsideTheWalls/Art/UI/Frontend/RoleSelection/`; `PF_UI_RoleSelection`.
- **Dependencies:** shared character bible, inmate clothing, officer uniform, accessibility review.
- **Completion checklist:** [ ] equal visual dignity and agency [ ] no faction morality coding [ ] readable silhouettes [ ] selected state not color-only [ ] diverse options independent of stats [ ] transparent edge review.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Paired role-selection portrait sheet: newly arrived adult inmate in plain issued clothing and probationary correctional officer in fictional practical uniform, both alert, composed, equally human, three-quarter waist-up pose, neutral background, consistent lens and lighting, selected and unselected framing treatments, no aggression, no heroic-versus-villain visual coding, no text.

### ITW-ENV-001 - Foundation gray-box kit

- **Label/type:** PROD 3D placeholder modular kit.
- **Exact assets:** `SM_Grid_Wall_1m/2m/4m`, `SM_Grid_Floor_2m/4m`, `SM_Grid_Ceiling_2m/4m`, `SM_Grid_DoorFrame`, `SM_Grid_Door_Secure`, `SM_Grid_Gate`, `SM_Grid_Fence_2m/4m`, `SM_Grid_Stair_Straight`, `SM_Grid_Railing_1m/2m`, `SM_Grid_Window`, `SM_Grid_Camera`, `SM_Grid_Alarm`, `SM_Grid_Light`, `SM_Grid_Sign`, `SM_Grid_Pipe`, `SM_Grid_Vent`, `SM_Grid_UtilityPanel`.
- **Purpose/location:** blocks every required prototype room and proves scale, sightlines, navigation, door permissions, and interaction pivots before production art.
- **Variants/views:** inside/outside corners; solid/window/door wall bays; left/right doors; fence corner/end; stair 3m rise; lit/unlit light; alarm idle/active; blank signage plates.
- **Specification:** 1m grid, 3m floor-to-floor; 0.2-15k triangles per piece; one 1K neutral atlas; FBX plus validation GLB; opaque except fence/window alpha where needed; no rig. Primitive/low-poly collision on every traversable piece. LOD0 only because placeholder, but production-replacement LOD slots must be preserved.
- **Unity target:** `Assets/_InsideTheWalls/Art/Environments/Graybox/`; `Assets/_InsideTheWalls/Prefabs/Environment/Graybox/PF_Grid_*`.
- **Dependencies:** Unity project scale, player capsule, navigation width, door-permission interface.
- **Completion checklist:** [ ] exact 1m snapping [ ] transforms applied [ ] pivots consistent [ ] no z-fighting seams [ ] stairs/corridors pass controller test [ ] doors expose hinge and interaction anchor [ ] collision layer assigned [ ] material shared [ ] placeholder label visible in asset metadata.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Orthographic modular-kit design sheet for a low-security institutional gray box: one-meter-grid walls, floors, ceilings, secure doors, gates, chain-link fence, straight stairs, railings, windows, cameras, alarms, practical lights, blank signs, pipes, vents, and utility panels. Show dimensions, consistent pivots, inside and outside corners, clean silhouette hierarchy, simple collision-proxy diagrams, neutral prototype materials, no identifiable real-world security blueprint.

### ITW-ENV-002 - Foundation room blockouts

- **Label/type:** PROD 3D PLACEHOLDER room assemblies.
- **Exact assets:** `PF_Blockout_Intake`, `PF_Blockout_HousingUnit`, `PF_Blockout_Cell`, `PF_Blockout_DiningHall`, `PF_Blockout_Yard`, `PF_Blockout_OfficerStation`, `PF_Blockout_ControlRoom`, `PF_Blockout_MedicalRoom`, `PF_Blockout_Classroom`, `PF_Blockout_Workshop`, `PF_Blockout_Commissary`, `PF_Blockout_VisitingRoom`; connecting secure service route included in the scene assembly.
- **Purpose/location:** offline gray-box day and later replacement guide.
- **Variants/views:** playable room assembly plus top-down diagram and two eye-level composition captures; housing and dining low/high occupancy dressing toggles.
- **Specification:** built only from ITW-ENV-001 plus primitive furniture; no unique textures; FBX only for any custom blockout prop; static primitive collision; no LOD beyond replacement slots.
- **Unity target:** `Assets/_InsideTheWalls/Prefabs/Environment/Graybox/Rooms/`; room prefabs named above.
- **Dependencies:** facility flow diagram, simplified daily schedule, interaction list, accessibility clearance.
- **Completion checklist:** [ ] all 12 named spaces exist [ ] gameplay routes work [ ] secure/service routes readable [ ] sightlines support both roles [ ] no real facility copied [ ] replacement boundaries documented [ ] room budget counters captured.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Fictional compact low-security facility gray-box plan and room vignette sheet covering intake, one housing unit, one cell or cubicle, dining hall, yard, officer station, control room, medical room, classroom, workshop, commissary, visiting room, and secure service connections. Prioritize readable circulation, believable adjacency, accessible clearances, varied social spaces, and enjoyable third-person sightlines. Conceptual gameplay layout only, not a real facility blueprint and not operational security guidance.

## P1 - Vertical slice

### ITW-ENV-010 - Low-security exterior and production room set

- **Label/type:** REF environment concepts followed by PROD 3D modular environment and room prefabs.
- **Exact assets:** `PF_LowSec_Exterior`, `PF_Intake`, `PF_HousingUnit`, `PF_Cell`, `PF_DiningHall`, `PF_Yard`, `PF_OfficerStation`, `PF_ControlRoom`, `PF_MedicalRoom`, `PF_Classroom`, `PF_Workshop`, `PF_Commissary`, `PF_VisitingRoom`.
- **Purpose/location:** replace the validated gray-box without changing gameplay metrics.
- **Variants/views:** exterior day/overcast/wet; each room clean baseline and lived-in dressing set; cells/cubicles occupied/unoccupied; top-down reference and eye-level hero view.
- **Specification:** sectorized FBX/glTF validation export; 80-250k visible triangles per major view, 2K shared trims/tileables, rare 4K exterior atlas only if profiled; glass transparency only where required; no rig. Authored simple collision; LOD0/1/2 for repeated props and exterior modules; occlusion-ready hierarchy.
- **Unity target:** `Assets/_InsideTheWalls/Art/Environments/LowSecurity/`; `Assets/_InsideTheWalls/Prefabs/Environment/LowSecurity/`.
- **Dependencies:** approved gray-box, modular production kit, materials, signage, props, lighting profiles.
- **Completion checklist:** [ ] gray-box metrics unchanged [ ] every named room covered [ ] day route remains legible [ ] interiors feel maintained and humane [ ] collision/navigation retested [ ] LOD transitions reviewed [ ] material/draw-call budget captured [ ] exterior fictionalized.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Cohesive production environment sheet for a small fictional low-security institution: welcoming-but-controlled exterior plus intake, housing unit, cell or cubicle, dining hall, yard, officer station, control room, medical room, classroom, workshop, commissary, and visiting room. Show one hero view and one orthographic material-and-module breakdown per space, bright natural daylight in lower-security areas, clear gameplay routes and distinct departmental identities, worn but maintained, no real facility layout, no operational security annotations.

### ITW-ENV-011 - Production modular architecture and security kit

- **Label/type:** PROD 3D.
- **Exact assets:** production replacements for all `SM_Grid_*` items: walls, floors, ceilings, doors, gates, fences, stairs, railings, windows, cameras, alarms, lights, signs, pipes, vents, utilities.
- **Purpose/location:** reusable construction kit for all vertical-slice spaces.
- **Variants/views:** 1m/2m/4m modules; corners/caps; secure/non-secure door readability; clean/worn material instances; damaged state only for authored incidents.
- **Specification:** FBX runtime plus GLB validation; 0.2-20k triangles; 2K trim sheets and 1K fixture atlases; glass/fence transparency minimized; doors/gates animate around correct pivots. Simple collision proxies; LOD0/1/2 on fixtures over 5k triangles and repeated complex modules.
- **Unity target:** `Assets/_InsideTheWalls/Art/Environments/Modular/`; `Assets/_InsideTheWalls/Prefabs/Environment/Modular/PF_*`.
- **Dependencies:** approved gray-box dimensions, shared material library, door gameplay contract.
- **Completion checklist:** [ ] one-to-one gray-box replacement [ ] snapping/pivots validated [ ] door states readable [ ] animated pieces separated [ ] colliders do not snag controller [ ] LODs preserve silhouette [ ] prefab variants use shared materials [ ] naming audit passes.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Detailed modular 3D asset sheet for walls, floors, ceilings, doors, gates, fences, stairs, railings, windows, cameras, alarms, lights, blank directional signs, pipes, vents, and utility boxes. One-meter grid, consistent edge language, limited reusable materials, clear secure-versus-public readability, exploded pivot diagrams for doors and gates, simple LOD silhouette examples and collision proxy overlays, no text labels on the models.

### ITW-CHAR-001 - Principal character sheets

- **Label/type:** REF concept sheets leading to PROD 3D characters.
- **Exact assets:** `SK_MarcusVale`, `SK_NoahMercer`, `SK_OfficerLenaOrtiz`, `SK_CaptainEliasWard`.
- **Purpose/location:** narrative encounters, role onboarding, relationship and officer-career scenes.
- **Variants/views:** front/back/left/right/three-quarter turnarounds; neutral A-pose; expression sheet with neutral, attentive, concerned, guarded, relieved, firm, tired, and restrained pain; close head study; clothing callouts.
- **Specification:** each production character 45-70k triangles including clothing/hair; 2K head/body plus 2K clothing, Base Color/Normal/Mask; opaque with hair cards if required; shared humanoid rig, facial blendshapes, capsule collider; LOD0/1/2 plus distant crowd LOD3.
- **Unity target:** `Assets/_InsideTheWalls/Art/Characters/Principals/<Name>/`; `Assets/_InsideTheWalls/Prefabs/Characters/PF_<Name>`.
- **Dependencies:** finalized biographies; shared character base/skeleton; clothing kit; animation retarget test.
- **Completion checklist:** [ ] age and role read without caricature [ ] all turnaround angles align [ ] eight expressions [ ] anatomy/clothing continuity [ ] no stereotype-coded features [ ] shared skeleton retargets [ ] blendshapes tested [ ] LODs/collider/prefab validated.
- **Copy-ready prompt - Marcus Vale:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Marcus Vale, 41-year-old inmate and former warehouse supervisor, calm, useful, observant, carrying the weight of misplaced loyalty and family strain. Full-body model sheet with front, back, side, three-quarter, neutral A-pose, consistent proportions, issued clothing, practical shoes, close head study, and expressions: neutral, attentive, concerned, guarded, relieved, firm, tired, restrained pain. Dignified natural appearance, no gang symbolism, no stereotype cues, plain neutral background.
- **Copy-ready prompt - Noah Mercer:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Noah Mercer, 24-year-old first-time inmate and former construction worker, frightened beneath a defensive posture, physically capable but inexperienced, with believable potential for education, reentry, or risky choices. Full-body model sheet with front, back, side, three-quarter, neutral A-pose, issued clothing, close head study, and expressions: neutral, attentive, concerned, guarded, relieved, firm, tired, restrained pain. Human and nuanced, no gang symbolism, no stereotype cues, plain neutral background.
- **Copy-ready prompt - Officer Lena Ortiz:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Officer Lena Ortiz, 28-year-old probationary correctional officer with overnight-security experience, alert and professional, believes communication prevents unnecessary conflict while learning institutional boundaries. Full-body model sheet with front, back, side, three-quarter, neutral A-pose, fictional practical uniform and duty belt without brand marks, close head study, and expressions: neutral, attentive, concerned, guarded, relieved, firm, tired, restrained pain. Competent without action-hero styling, no stereotype cues, plain neutral background.
- **Copy-ready prompt - Captain Elias Ward:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Captain Elias Ward, 49-year-old security commander with two decades of experience, steady and watchful, shaped by a past disturbance where caution saved lives, evaluates rookies by judgment under pressure. Full-body model sheet with front, back, side, three-quarter, neutral A-pose, fictional captain uniform with restrained rank distinction, close head study, and expressions: neutral, attentive, concerned, guarded, relieved, firm, tired, restrained pain. Seasoned without villain coding, no stereotype cues, plain neutral background.

### ITW-CHAR-002 - Shared bodies, clothing, and hair

- **Label/type:** REF design matrix plus PROD 3D modular characters.
- **Exact assets:** `SK_Body_A/B/C`, `SK_HeadSet_01`, `SM_HairSet_01`, `SK_Cloth_Inmate`, `SK_Cloth_Officer_Probationary/Officer/Senior/Lieutenant/Captain/Leadership`, `SK_Cloth_Medical`, `SK_Cloth_FoodService`, `SK_Cloth_Maintenance`, `SK_Cloth_ProgramStaff`.
- **Purpose/location:** player customization and AI population with readable role/rank silhouettes.
- **Variants/views:** inclusive adult body/head options; practical hair; inmate sizes; officer rank identifiers; medical, food-service, maintenance, education/counseling/program variants; clean and work-worn material instances.
- **Specification:** 45-70k triangles assembled LOD0; 2K body/head and 1-2K clothing atlases; opaque/hair cards; shared humanoid rig and corrective blendshapes; capsule collider; LOD0/1/2/3. Clothing must not clip core locomotion range.
- **Unity target:** `Assets/_InsideTheWalls/Art/Characters/Modular/`; `Assets/_InsideTheWalls/Prefabs/Characters/Modular/PF_CharacterBase`.
- **Dependencies:** shared skeleton, customization data model, respectful representation review, animation set.
- **Completion checklist:** [ ] roles readable at 15m [ ] rank not color-only [ ] identity choices independent of stats/factions [ ] fit matrix tested [ ] skin/hair lighting reviewed [ ] locomotion clipping pass [ ] atlas/material reuse [ ] LOD and rig validation.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Inclusive modular adult character and clothing design matrix using one shared humanoid skeleton: varied believable bodies, faces, skin tones, practical hairstyles, issued inmate clothing, probationary through leadership officer uniforms, medical scrubs, food-service garments, maintenance workwear, and education, counseling, treatment, recreation, and faith-program staff attire. Front/back/side views, garment separation, rank-readable shape details, limited shared material palette, no identity-linked moral or faction coding, no real agency patches.

### ITW-ANIM-001 - Core animation reference and production clips

- **Label/type:** REF motion boards/video followed by PROD humanoid clips.
- **Exact assets:** `AN_Locomotion`, `AN_Work`, `AN_Conversation`, `AN_Escort`, `AN_Radio`, `AN_Deescalation`, `AN_Surrender`, `AN_Combat_Restrained` plus sitting, sleeping, eating, exercising, searching, and treatment support clips.
- **Purpose/location:** movement, daily work, social play, officer duties, and consequence-focused incidents.
- **Variants/views:** body-size neutral; left/right starts; unarmed; calm/urgent locomotion; work loops; conversation listening/speaking; cooperative escort; radio stand/walk; open-hand de-escalation; surrender kneel/stand; restrained shove/block/grapple break and non-graphic injury reactions.
- **Specification:** FBX clips on shared humanoid rig, 30/60 fps source as appropriate, root-motion and in-place variants where gameplay needs both, clean loops, foot contacts and event markers; no textures; character capsule collision remains authoritative; animation LOD via animator/culling policy.
- **Unity target:** `Assets/_InsideTheWalls/Art/Characters/Animations/`; character animator prefabs/controllers assigned later.
- **Dependencies:** shared skeleton, controller motion contract, interaction timing, gameplay authority design.
- **Completion checklist:** [ ] all eight mandated groups covered [ ] loop seams clean [ ] foot sliding acceptable [ ] root and in-place versions named [ ] hand/contact markers present [ ] retarget on body set [ ] no glorified/excessive violence [ ] network-important events are not animation-authoritative.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Professional animation reference board for a shared humanoid rig, clear side and three-quarter key poses for locomotion, institutional work tasks, listening and conversation, cooperative escort, standing and walking radio use, open-hand de-escalation, standing and kneeling surrender, and restrained non-graphic defensive combat including block, controlled separation, and recovery. Add sitting, sleeping, eating, exercising, searching, and medical-treatment support poses. Natural weight, readable hands, calm-to-urgent variations, no cinematic brutality, no tactical instruction labels.

### ITW-UI-010 - Gameplay and administration UI system

- **Label/type:** REF UX boards leading to PROD vector/9-slice UI.
- **Exact assets:** `PF_UI_HUD`, `PF_UI_Schedule`, `PF_UI_Map`, `PF_UI_Inventory`, `PF_UI_Commissary`, `PF_UI_Relationships`, `PF_UI_IncidentReport`, `PF_UI_Classification`, `PF_UI_Promotion`, `PF_UI_Settings`, `PF_UI_Moderation`.
- **Purpose/location:** all mandated player and staff information surfaces.
- **Variants/views:** inmate/officer context; empty/loading/error/disabled/success states; keyboard-mouse/controller prompts; 16:9/21:9/4:3; 100/150/200% UI scales; color-vision-safe status variants.
- **Specification:** SVG/PNG icons at 64/128/256, 2048 max atlases, straight alpha; 9-slice panels; no rig/collision/LOD. HUD reserves center view and supports subtitle/message safe areas.
- **Unity target:** `Assets/_InsideTheWalls/Art/UI/Gameplay/`; exact prefab names above in `Assets/_InsideTheWalls/Prefabs/UI/`.
- **Dependencies:** terminology, data schemas, input glyphs, localization and accessibility rules.
- **Completion checklist:** [ ] all 11 surfaces present [ ] role permissions reflected [ ] complete state matrix [ ] navigation order defined [ ] text never baked [ ] contrast and non-color cues [ ] UI scaling/aspect tests [ ] destructive/moderation actions require clear confirmation.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Complete diegetic-inspired but highly readable game UI design system: low-chrome HUD, daily schedule, facility map, inventory, commissary, relationships and memories, incident report, classification review, officer promotion, settings, and moderation interfaces. Use clean document-grid structure, restrained orange focus, desaturated blue-green panels, off-white text, distinct focus and disabled states, keyboard and controller navigation cues, empty/loading/error examples, no text baked into reusable art, accessible contrast, no fake real-agency branding.

### ITW-MAT-001 - Shared material and decal library

- **Label/type:** REF swatch board plus PROD PBR materials/decals.
- **Exact assets:** `M_Concrete`, `M_PaintedBlock`, `M_WeatheredSteel`, `M_StainlessSteel`, `M_VinylFloor`, `M_SafetyPaint`, `M_Glass`, `M_Fence`, `T_Decal_Wayfinding`, `T_Decal_Wear`, `T_Decal_Maintenance`, `T_Decal_Incident`.
- **Purpose/location:** coherent surface language and low-cost environmental variation.
- **Variants/views:** clean/standard/worn; dry/wet; faded yellow/restrained orange accents; fictional blank or icon-based signage.
- **Specification:** 2K tileable Base Color/Normal/Mask masters, 1K decal atlases, PNG/TGA source; decals with straight alpha; no rig/collision; shader LOD/fallback planned. Reuse materials across modules.
- **Unity target:** `Assets/_InsideTheWalls/Art/Materials/` and `Assets/_InsideTheWalls/Art/Environments/Decals/`; material presets, no standalone prefab except decal projectors.
- **Dependencies:** render pipeline choice, modular UV/texel-density standard, fictional signage vocabulary.
- **Completion checklist:** [ ] PBR ranges plausible [ ] shared texel density [ ] tiling hidden at play distance [ ] wetness is parameterized [ ] decals fictional and respectful [ ] atlas padding clean [ ] material variants do not duplicate textures [ ] profiler review.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. PBR material and decal reference board with concrete, painted masonry, weathered and stainless steel, durable vinyl flooring, faded safety paint, reinforced glass, chain-link fence, fictional wayfinding, maintenance marks, restrained wear, and generic incident-cleanup traces. Show clean, standard, worn, dry, and wet swatches under neutral lighting, believable roughness and scale, limited reusable atlas plan, no readable real-world logos.

### ITW-VFX-001 - Weather, alarms, particles, blood, and injury feedback

- **Label/type:** REF effect boards plus PROD VFX graphs, flipbooks, decals, and UI feedback.
- **Exact assets:** `VFX_Rain`, `VFX_WetSurface`, `VFX_DustMotes`, `VFX_Steam`, `VFX_Sparks`, `VFX_AlarmBeacon`, `VFX_AlarmUI`, `VFX_Blood_Minor`, `VFX_InjuryFeedback`.
- **Purpose/location:** atmosphere, readable emergencies, maintenance failures, and restrained consequence feedback.
- **Variants/views:** clear/overcast/rain; indoor/outdoor; alarm idle/active; minor blood decal small/medium; injury low/medium with reduced-effects accessibility option.
- **Specification:** 1024-2048 flipbook/atlas PNG or TGA with alpha; particle max counts and overdraw budgets set during profiling; no rig/collision except optional rain volumes; distance/culling LOD and mobile fallback mandatory.
- **Unity target:** `Assets/_InsideTheWalls/Art/VFX/`; `Assets/_InsideTheWalls/Prefabs/Gameplay/VFX/PF_VFX_*`.
- **Dependencies:** render pipeline, lighting profiles, incident/injury states, accessibility setting.
- **Completion checklist:** [ ] alarm readable without color/audio alone [ ] no graphic gore [ ] effects communicate consequence [ ] reduced-effects mode [ ] overdraw/particle budget profiled [ ] indoor occlusion and culling [ ] decal lifetime/pooling [ ] weather does not obscure objectives.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Restrained real-time effects reference sheet: light rain and wet concrete, dust motes, utility steam, brief electrical sparks, amber-red alarm beacon with shape and pulse cues, subtle UI alarm edge treatment, very small non-graphic blood marks, bruising and guarded-movement injury feedback. Include normal and reduced-effects accessibility versions, transparent flipbook planning, clear silhouettes, low overdraw, no suffering as spectacle.

### ITW-PROP-001 - Department furniture and interaction props

- **Label/type:** REF prop sheets followed by PROD 3D props.
- **Exact assets:** intake desk/property bins, bunk/cubicle/locker, dining table/bench/tray, yard bench/exercise station, officer desk/chair/console, control monitors, exam bed/medical cart, classroom desk/chair/board, workshop bench/tool silhouettes, commissary counter/shelf/package set, visiting table/chair/partition, laundry cart, waste bin.
- **Purpose/location:** gives each mandated room a readable function and supports the first inmate job, officer duty, and social interaction.
- **Variants/views:** clean/used material instances; occupied/empty storage; interactive/open/closed variants where gameplay requires; no branded consumables.
- **Specification:** 0.3-12k triangles each; 512-1K shared category atlases, 2K hero console atlas maximum; FBX/GLB validation; alpha only for screens/mesh perforation if necessary; no rig unless a simple hinge; primitive/convex collision; LOD0/1/2 on repeated assets over 3k triangles.
- **Unity target:** `Assets/_InsideTheWalls/Art/Props/Departments/`; `Assets/_InsideTheWalls/Prefabs/Environment/Props/PF_*`.
- **Dependencies:** approved room blockouts, interaction anchors, material library.
- **Completion checklist:** [ ] every room has minimum function set [ ] interaction pivots/anchors [ ] no real brands [ ] collision supports navigation [ ] shared atlases/materials [ ] repeated props have LODs [ ] prop density profiled [ ] no loose operational detail that enables wrongdoing.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Modular department prop sheet for intake, housing, dining, yard, officer station, control room, medical room, classroom, workshop, commissary, visiting, laundry, and service routes. Durable furniture and benign interaction objects, front/side/three-quarter views, shared-material groups, hinge and interaction-anchor callouts, simple collision silhouettes, clean and work-worn variants, fictional generic packaging and interfaces, no tactical security detail.

## P2 - Later expansion

### ITW-EXP-001 - Additional facilities, climate, population, and presentation

- **Label/type:** REF concepts only until vertical-slice gate passes; later PROD 3D/2D.
- **Exact assets:** intake-transfer campus, minimum-security campus, medium-security institution, high-security institution, administrative/medical special-mission facility; seasonal weather sets; expanded hairstyles/clothing/props; store capsule and promotional crops.
- **Purpose/location:** future facility progression and content breadth, not the first playable.
- **Variants/views:** exterior identity, representative public/common interior, day/night/weather, security-density progression without moral value coding.
- **Specification:** concept at 4K; production budgets must be derived from vertical-slice profiler data rather than assumed now; all production assets require collision/LOD/material reuse plans.
- **Unity target:** `Assets/_InsideTheWalls/Art/Expansion/<Facility>/`; prefab targets assigned only when milestone is authorized.
- **Dependencies:** vertical-slice art/performance gate, fictional world bible, facility gameplay requirements, platform plan.
- **Completion checklist:** [ ] vertical slice has passed [ ] concept does not duplicate real site [ ] new kit reuse quantified [ ] profiler-based budgets approved [ ] scope owner assigned [ ] production authorization recorded.
- **Copy-ready prompt:** High-end stylized-realistic 3D game art for Inside the Walls. Grounded fictional North American correctional setting, believable proportions, simplified readable forms, strong gameplay silhouettes, concrete gray and desaturated blue-green palette, weathered steel, faded safety yellow, restrained orange accents, worn but maintained surfaces, controlled cinematic lighting, no real logos, no watermark, no stereotype-based features, no exaggerated horror, and no graphic gore. Designed as reference for an optimized Unity Windows PC game with future mobile adaptation in mind. Future facility progression concept board showing fictional intake-transfer, minimum-security, medium-security, high-security, and administrative medical campuses. Communicate increasing movement control through density, circulation, material temperature, and lighting while keeping every setting believable and humane. Include day, night, rain, snow, and heat-haze mood thumbnails, identify reusable visual motifs without operational diagrams, no real facilities or agency insignia.

## Production order

1. Verify rights and technical suitability of the existing key art; derive a text-free composition and lock the brand system.
2. Produce the responsive splash, menu, loading, and role-selection reference package.
3. Build and playtest the P0 gray-box kit and all 12 room assemblies; freeze gameplay dimensions only after the offline loop works.
4. Approve the shared material, modular production kit, character skeleton, UI tokens, and VFX accessibility rules.
5. Replace gray-box modules by risk and reuse: doors/routes first, shared architecture second, required furniture third, character and animation integration fourth, room dressing last.
6. Capture profiler evidence and visual reviews before authorizing P2 work.

## Global definition of done for every production asset

- [ ] Register entry has owner, status, source/license, dependency state, performance budget, prefab path, LOD status, and last review date.
- [ ] Exact name, scale, transforms, pivot, hierarchy, and material slots follow this document.
- [ ] Source is preserved; runtime export imports without errors; no generator watermark, hidden geometry, real logos, or secret metadata.
- [ ] Texture color space, alpha, compression, mipmaps, and maximum size are explicitly reviewed.
- [ ] Collision and interaction anchors are tested with the player controller; navigation remains valid.
- [ ] Required LODs/culling are present and transitions pass third-person visual review.
- [ ] Asset is reviewed in representative neutral, warm-interior, and cool-exterior lighting.
- [ ] Performance is measured in the target scene; budgets are adjusted from evidence and recorded.
- [ ] Representation and content review confirms dignity, fictional branding, restrained injury treatment, and no stereotype-based design.
- [ ] Prefab is validated in Unity and reference-only inputs remain clearly separated from shipping assets.

## Register maintenance fields

When production begins, append these fields to each asset's tracking ticket or asset database row: `Owner`, `Status (Brief/REF/Blockout/Production/Review/Approved)`, `Source and license`, `Triangle count by LOD`, `Texture memory`, `Material count`, `Collision status`, `Rig status`, `Unity prefab path`, `Dependencies`, `Last review date`, `Reviewer`, and `Known issues`.
