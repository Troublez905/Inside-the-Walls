# Next Graphics: People, Interactions And Areas

Priority order for the next playable prototype. Each numbered item is one asset or one animation, not a bundled concept sheet. Every prompt below is under 600 characters. These are production targets, not claims that a generator will deliver a usable rig automatically.

## Best File Types

- **Characters and animations: FBX.** Include the actual skeleton and skin weights. An OBJ or PNG cannot supply a working animated character by itself. Keep the editable `.blend` separately.
- **Static props and architecture: FBX preferred; OBJ is acceptable.** For OBJ, also include its `.mtl` and texture files. Doors need separate frame and leaf meshes, even when delivered in one FBX.
- **Textures: separate PNG maps.** BaseColor, Normal (OpenGL), Roughness, and Metallic if needed. Do not bake lighting or cast shadows into BaseColor. Unity materials are configured separately.
- Deliver one asset per clearly named folder under `Docs/Art/Incoming/`, with source files and a brief license/source note. Do not overwrite the existing characters or runtime prefabs.
- Use real meters, applied transforms, feet on the ground for characters, useful pivots for props, UVs, and no cameras/lights. Re-import each exported file to check scale, texture paths, bones and animation.

Unity supports FBX and OBJ and recommends exported FBX for production handoffs: [Unity model formats](https://docs.unity3d.com/6000.0/Documentation/Manual/3D-formats.html). FBX supports skeletal and animation data: [Blender FBX manual](https://docs.blender.org/manual/en/latest/files/import_export/fbx.html).

## Characters And Movement

### 1. Inmate #2 - Production Rig

Format: FBX with mesh, Humanoid skeleton and skin weights; editable BLEND alongside it. Highest priority. Target 20,000-35,000 triangles, maximum four weights per vertex, 2K maps.

Prompt:
> Create a game-ready 3D character matching my inmate #2 reference: adult man, short hair and beard, orange short-sleeve prison uniform, white undershirt, gray slip-on shoes. Preserve his identity and proportions. Full body, neutral expression, clean T-pose for rigging, separated fingers, feet flat, 1.8 m tall. Clean deformation topology at shoulders, elbows, hips and knees; UV unwrapped, PBR textures, no background or props. Deliver a skinned Humanoid FBX and editable Blender source.

### 2. Correctional Officer Lena Ortiz

Format: rigged FBX plus BLEND. Target 20,000-30,000 triangles, 2K maps. Match the existing Lena reference rather than creating an unrelated face.

Prompt:
> Create one game-ready adult female correctional officer matching the supplied Lena Ortiz reference. Practical navy uniform, black boots, simple duty belt and clipped radio, no drawn weapon or real agency insignia. Professional neutral expression and realistic proportions. Full body in clean T-pose, separated hands and fingers, feet flat at origin, 1.72 m tall. Clean joint topology, UVs, PBR textures and a fully weighted Humanoid rig. Export FBX with editable Blender source, no environment.

### 3. Second Correctional Officer

Format: rigged FBX plus BLEND. Target 15,000-25,000 triangles, 2K maps.

Prompt:
> Create one adult male correctional officer for a grounded prison simulation: middle-aged, average build, close-cropped hair, composed expression. Navy short-sleeve uniform, black duty belt, boots and clipped radio; no weapon in hand, badges or real-world logos. Full body T-pose, feet flat, realistic 1.8 m height, separated fingers. Optimize for animation with clean shoulder and knee loops, UVs, PBR maps and four-weight Humanoid skinning. Export rigged FBX and Blender source on an empty scene.

### 4. Inmate #3 - Background Character

Format: rigged FBX. Target 10,000-20,000 triangles, 1K-2K maps; ideally also a 5,000-triangle LOD.

Prompt:
> Turn my inmate #3 reference into one optimized full-body 3D game character. Retain the face, orange uniform and footwear from the reference. Neutral expression, clean T-pose, no props or environment, feet flat at origin. Use realistic human scale, clean topology around joints and a single UV atlas. Supply a Humanoid skeleton with tested skin weights, maximum four influences per vertex. Export FBX with base color, normal and roughness maps; include a lower-detail mesh for distant NPCs.

### 5. Relaxed Idle Animation

Format: animation FBX on the same Humanoid rig. This is an animation request, not an image/model prompt. A T-pose belongs in the bind pose; the gameplay idle must lower the arms.

Prompt:
> Animate the supplied rigged character standing calmly for a prison-simulation idle. Both arms hang naturally beside the torso, elbows slightly bent, hands open and relaxed near the outer thighs, never clasped in front. Add subtle breathing, small weight shifts and occasional head movement. Feet stay planted, no root translation, no weapon pose. Four-second seamless loop at 30 fps. Preserve the provided skeleton and scale; export a baked Humanoid animation FBX.

### 6. Natural Walk Animation

Format: animation FBX on the same rig, in place. Target approximately 1.8 m/s at playback speed 1.

Prompt:
> Animate a relaxed natural walk on the supplied Humanoid rig, suitable for an adult walking through a prison dayroom. Arms swing gently at the sides, hands open, shoulders relaxed, no weapon stance or hands held in front. Clear heel contact and toe-off, stable hips, no foot sliding. In-place seamless loop at 30 fps, designed for 1.8 meters per second movement in Unity. Preserve the skeleton and scale; export one baked animation FBX, no mesh changes or camera.

## Interaction And Combat Animation

### 7. Conversation Gesture

Format: animation FBX, one clip, matching the supplied rig.

Prompt:
> Animate one restrained conversational gesture on the supplied Humanoid character: begin with hands relaxed at sides, briefly lift one open hand to explain a point, nod once, then return smoothly to the same idle. Respectful everyday tone, no pointing in someone's face, no exaggerated acting. Feet planted, no root motion, duration 2.5 seconds at 30 fps. Keep the original skeleton and scale. Export one baked FBX animation clip with a clean idle-compatible start and end.

### 8. De-escalation Gesture

Format: animation FBX, one clip.

Prompt:
> Animate a calm de-escalation gesture on the supplied Humanoid rig. From relaxed idle, lift both open palms to lower-chest height, shoulders down, give a small reassuring nod, then lower hands to the sides. Convey 'let's slow down' without threatening or touching anyone. Feet remain planted; no root displacement. Three seconds at 30 fps, smooth idle-compatible start and end, no props. Preserve bones and scale; export a single baked FBX animation.

### 9. Short Shove

Format: animation FBX, one clip. Non-graphic prototype action; no weapon or injury mesh.

Prompt:
> Animate one brief non-graphic two-hand shove on the supplied Humanoid rig. Start from relaxed standing, make a readable short anticipation, extend both open palms forward at chest height, then recover to hands at sides. Keep the motion restrained and balanced, no punch, weapon, blood or knockdown. Feet largely planted, no root translation, about 0.8 seconds at 30 fps. Preserve the existing skeleton and scale; export one baked FBX animation with a clear contact frame.

### 10. Defensive Guard

Format: animation FBX, one seamless loop. A later transition clip can be added separately.

Prompt:
> Animate a non-aggressive defensive guard loop on the supplied Humanoid rig: forearms protect the upper torso, open hands, elbows close, chin slightly tucked and knees soft. Add subtle breathing and a small stable weight shift. No weapon, clenched punching motion or forward lunge. Feet planted and no root motion. Two-second seamless loop at 30 fps, realistic restrained movement. Preserve the supplied skeleton and proportions; export one baked FBX animation clip.

### 11. Shove Reaction

Format: animation FBX, one clip. Displacement will be controlled by gameplay, not baked into the root.

Prompt:
> Animate one mild shove reaction on the supplied Humanoid rig: torso recoils briefly, arms open to regain balance, one small corrective step is suggested, then return to a stable relaxed stance with hands at sides. No falling, injury, blood or exaggerated impact. Keep the root stationary so Unity controls displacement. Duration 1.0 second at 30 fps, readable contact at the beginning and smooth recovery. Keep the original bones and scale; export one baked FBX animation.

## Area Models

### 12. Concrete Wall Segment

Format: FBX preferred; OBJ plus MTL acceptable. One module, 3 m wide x 3 m tall x 0.2 m thick, under 2,000 triangles.

Prompt:
> Model one modular institutional concrete-block wall panel, exactly 3 m wide, 3 m tall and 0.2 m thick. Muted warm-gray painted masonry, darker scuffed base strip, subtle age and believable scale, no graffiti or signage. Straight seamless edges for grid snapping, thickness visible, origin at bottom-left corner. Optimized clean topology, UV unwrapped with tileable PBR material, no baked lighting, room or props. Export one FBX or OBJ with separate texture maps.

### 13. Secure Interior Door

Format: FBX with separate frame and leaf. Leaf origin on hinge axis; roughly 1 m x 2.1 m clear opening.

Prompt:
> Create one grounded institutional steel door with its frame. Muted blue-gray painted metal, small reinforced observation window, recessed handle and modest edge wear. Door opening 1 m wide by 2.1 m high. Frame and door leaf must be separate named meshes; pivot the leaf precisely on its hinge axis so it can animate. Clean UVs, simple collision-friendly shapes, PBR textures, no wall, floor, people or logos. Export FBX with transforms applied and the leaf closed.

### 14. Yard Floor Tile

Format: FBX or OBJ, 4 m x 4 m, under 200 triangles. Most surface detail belongs in textures.

Prompt:
> Model one flat 4 m by 4 m modular outdoor prison-yard ground tile. Aged gray concrete with restrained aggregate, hairline wear and subtle discoloration, no deep cracks, objects, grass or painted markings. Edges must join seamlessly on a meter grid. Keep the surface flat for gameplay and geometry minimal. UVs use a repeatable PBR material; provide base color, normal and roughness without baked shadows. Export FBX or OBJ with origin at one corner and real-meter scale.

### 15. Yard Fence Panel

Format: FBX preferred; OBJ acceptable. One 3 m wide x 3 m tall module, under 3,000 triangles.

Prompt:
> Create one 3 m wide by 3 m tall institutional yard-fence module with two galvanized posts and a simple top rail. Fine chain-link mesh should use efficient geometry or a supplied opacity texture rather than dense wire geometry. Weathered but maintained, no razor wire, signs, gates or environment. Snap-compatible straight edges, bottom-corner origin, realistic thickness. Clean UVs and separate PBR/opacity maps. Export optimized FBX or OBJ at meter scale.

### 16. Yard Bench

Format: FBX or OBJ, one static model, under 3,000 triangles.

Prompt:
> Model one fixed outdoor institutional bench, 1.8 m long, 0.45 m seat height, simple dark powder-coated steel supports and weathered muted composite slats. Practical, sturdy, restrained wear, no cushions, graffiti or loose objects. Ground-contact feet, centered ground-level pivot, clean silhouette and simple collision shape. Optimized topology with UVs and PBR textures, no background or people. Export FBX or OBJ in real meters with all textures supplied separately.

### 17. Wall-Mounted Light Fixture

Format: FBX or OBJ, one fixture. Separate emissive cover mesh; no baked light or Unity light required.

Prompt:
> Model one vandal-resistant institutional wall light, approximately 0.6 m long and 0.18 m high. Rounded frosted cover inside a dark metal protective housing, subtle fasteners, believable industrial construction and light wear. Separate the luminous cover from the opaque housing so emission can be controlled in Unity. Rear mounting plane as pivot, low-poly optimized mesh, UVs and PBR maps. No wall, wires extending away, people or baked glow. Export one FBX or OBJ.

## Blender Collaboration

Blender is already installed locally and is useful for correcting pivots, scale, UVs, topology, weights and animation before export. Keep an editable BLEND source outside runtime assets and export FBX for Unity. For animation, send the existing rig with the request: a text/image generator cannot guarantee compatible bone names or skinning.

Blender MCP could support interactive scene inspection and scripted edits. The community [Blender MCP project](https://github.com/ahujasid/blender-mcp) exposes Python execution inside Blender, so treat it as a trusted local development bridge, not a public service. It is not installed or verified by this asset-list task. We can configure it in a separate setup pass; no paid service is required for the local modeling pipeline.

Most helpful next handoff: the officer model, the relaxed idle clip and the natural walk clip. Preserve inmate #1 while repairing it; put the repair in a new versioned folder rather than replacing its working source in place.
