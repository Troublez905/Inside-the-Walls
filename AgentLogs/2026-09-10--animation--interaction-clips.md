# Interaction animation integration

- Added `InteractionAnimationBuilder.EnsureAssets()` for seven supplied FBX animation files. Only derived source copies are configured; original Runtime FBX files and Noah's controller are untouched.
- Output controller: `Assets/_InsideTheWalls/Resources/Characters/InmateTwoAnimations/InmateTwo.controller`.
- Parameters: Speed float (normalized movement speed, normal walk 1), Guard bool, Talk trigger, Reaction trigger. Walk uses Speed as playback multiplier.
- Locomotion uses Unarmed Idle 01 and Catwalk Walk Forward 01. Fighting Idle is a looping upper-body guard. Standing Arguing is an upper-body one-shot. Mask excludes root, legs, foot IK, and hand IK. Shove Reaction is a full-body victim response and automatically returns to the underlying layers. Disappointed and Wheelbarrow Dump are extracted but unbound.
- Derived FBX import uses Humanoid/CreateFromThisModel auto mapping. Root rotation and root Y/XZ are baked into pose. Runtime must disable applyRootMotion. Idle, walk, and guard loop; other clips do not.
- Builder validates exactly one take, valid human avatar, one non-preview humanoid clip, and positive duration per FBX. `INTERACTION_CLIP_OK` logs each actual name, duration, humanoid status, loop setting, and avatar validity. `INTERACTION_ANIMATIONS_OK` marks completed generation.
- Extracted clips, mask, and controller update in place to preserve their GUIDs. Existing derived source copies are reused.
- Source review completed. Unity compilation, import validation, actual durations, and runtime visual checks are delegated to the lead's single Unity batch run and are not claimed here.
- Status: implementation complete; Unity verification pending lead integration.
