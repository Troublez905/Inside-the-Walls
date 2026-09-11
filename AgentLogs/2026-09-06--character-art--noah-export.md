# Character Art - Noah Source-Rig Handoff

## Outcome

The rigged Inmate 01 source was inspected through a bounded Blender 5.0 headless pipeline. The original `.blend` remained unchanged (SHA-256 `7317AB5D9B938860DF09072C441EC3FA70C918D8C4F4EC68240E9E13FA36BC4B`). A reproducible source-quality FBX and three distinct packed texture maps were exported and imported by Unity without build warnings.

## Files

- `Tools/CharacterPipeline/noah_export.py`
- `Tools/CharacterPipeline/Invoke-NoahExport.ps1`
- `Assets/_InsideTheWalls/Art/Characters/Principals/NoahMercer/Source/Interchange/NoahMercer_SourceRig.fbx`
- `Assets/_InsideTheWalls/Art/Characters/Principals/NoahMercer/Source/Interchange/NoahMercer_SourceRig.audit.json`
- `Assets/_InsideTheWalls/Art/Characters/Principals/NoahMercer/Source/Interchange/Textures/NoahMercer_BaseColor.png`
- `Assets/_InsideTheWalls/Art/Characters/Principals/NoahMercer/Source/Interchange/Textures/NoahMercer_Normal.png`
- `Assets/_InsideTheWalls/Art/Characters/Principals/NoahMercer/Source/Interchange/Textures/NoahMercer_Roughness.png`

## Verification

- One 1.89 m skinned mesh, 26 deform bones, no unweighted vertices.
- Three texture outputs have distinct paths and SHA-256 hashes.
- Unity imported the FBX and textures during `Logs/save-continue-build-final.log`.
- Final Windows build completed with zero warnings.

## Quality Decision

This export is accepted only as a preserved source/showcase candidate. It is not integrated into runtime: 399,998 triangles, up to seven source skin influences, no animation actions, and an unproven forward-facing orientation fail the runtime character gate. The next character pass must retopologize, cap weights at four, produce LODs, validate a Humanoid avatar, and supply locomotion clips before replacing the alpha capsule.

## Status

Source handoff complete. Runtime character conversion remains intentionally blocked on optimization and animation work.
