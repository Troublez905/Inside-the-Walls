"""Build a derived inmate #2 rig without modifying either original character."""

import argparse
import hashlib
import json
import math
import shutil
from pathlib import Path

import bpy
from mathutils import Vector, Matrix


ROOT = Path(__file__).resolve().parents[2]
DONOR = ROOT / "Assets/_InsideTheWalls/Character Models/character-inmate-01-T_Rigged_4652083419/character-inmate-01-T_Rigged_4652083419/character_publish.blend"
SOURCE = ROOT / "Assets/_InsideTheWalls/Character Models/character-inmate-02-T_Textured_4652183416/mesh.obj"
OUTPUT = ROOT / "Assets/_InsideTheWalls/Art/Characters/InmateTwo/Runtime"


def sha256(path):
    digest = hashlib.sha256()
    with Path(path).open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest().upper()


def bounds(obj):
    points = [obj.matrix_world @ vertex.co for vertex in obj.data.vertices]
    return {
        "min": [min(point[i] for point in points) for i in range(3)],
        "max": [max(point[i] for point in points) for i in range(3)],
    }


def import_sources():
    bpy.ops.wm.open_mainfile(filepath=str(DONOR), load_ui=False, use_scripts=False)
    rigs = [obj for obj in bpy.data.objects if obj.type == "ARMATURE"]
    meshes = [obj for obj in bpy.data.objects if obj.type == "MESH"]
    if len(rigs) != 1 or len(meshes) != 1:
        raise RuntimeError("Expected one donor rig and one donor mesh.")
    rig, donor = rigs[0], meshes[0]
    rig.animation_data_clear()
    rig.data.pose_position = "REST"
    for bone in rig.pose.bones:
        bone.matrix_basis.identity()
        for constraint in list(bone.constraints):
            bone.constraints.remove(constraint)
    bpy.ops.object.select_all(action="DESELECT")
    bpy.ops.wm.obj_import(filepath=str(SOURCE), forward_axis="NEGATIVE_Z", up_axis="Y")
    targets = [obj for obj in bpy.context.selected_objects if obj.type == "MESH"]
    if len(targets) != 1:
        raise RuntimeError("Expected one inmate #2 mesh.")
    target = targets[0]
    bpy.context.view_layer.objects.active = target
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return rig, donor, target


def main():
    import sys
    parser = argparse.ArgumentParser()
    parser.add_argument("--inspect", action="store_true")
    args = parser.parse_args(sys.argv[sys.argv.index("--") + 1 :])
    protected = [DONOR, SOURCE, SOURCE.with_suffix('.mtl'), SOURCE.parent / 'BakedTexture.png']
    hashes = {str(path.relative_to(ROOT)): sha256(path) for path in protected}
    rig, donor, target = import_sources()
    report = {
        "donor_bounds": bounds(donor),
        "target_bounds": bounds(target),
        "target_vertices": len(target.data.vertices),
        "target_triangles": sum(len(poly.vertices) - 2 for poly in target.data.polygons),
        "bones": [{"name": b.name, "head": list(rig.matrix_world @ b.head_local), "tail": list(rig.matrix_world @ b.tail_local)} for b in rig.data.bones],
    }
    print("INMATE_TWO_INSPECTION " + json.dumps(report))
    if args.inspect:
        return
    OUTPUT.mkdir(parents=True, exist_ok=True)
    donor_height = report['donor_bounds']['max'][2] - report['donor_bounds']['min'][2]
    target_height = report['target_bounds']['max'][2] - report['target_bounds']['min'][2]
    target.scale *= donor_height / target_height
    bpy.context.view_layer.objects.active = target
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    decimate = target.modifiers.new('Runtime triangle budget', 'DECIMATE')
    decimate.ratio = 32000 / report['target_triangles']
    bpy.ops.object.modifier_apply(modifier=decimate.name)
    for group in donor.vertex_groups:
        target.vertex_groups.new(name=group.name)
    transfer = target.modifiers.new('Donor surface skin transfer', 'DATA_TRANSFER')
    transfer.object = donor
    transfer.use_vert_data = True
    transfer.data_types_verts = {'VGROUP_WEIGHTS'}
    transfer.vert_mapping = 'POLYINTERP_NEAREST'
    transfer.layers_vgroup_select_src = 'ALL'
    transfer.layers_vgroup_select_dst = 'NAME'
    bpy.ops.object.modifier_apply(modifier=transfer.name)
    bpy.ops.object.vertex_group_limit_total(limit=4)
    bpy.ops.object.vertex_group_normalize_all(lock_active=False)
    influences = [sum(g.weight > 0 for g in v.groups) for v in target.data.vertices]
    triangles = sum(len(p.vertices) - 2 for p in target.data.polygons)
    if min(influences) == 0 or max(influences) > 4 or triangles > 35000:
        raise RuntimeError('Runtime skin/triangle validation failed')
    # Bake identical world-space normalization into the independent mesh and rig.
    reduced_bounds = bounds(target)
    reduced_height = reduced_bounds['max'][2] - reduced_bounds['min'][2]
    normalization = Matrix.Scale(1.8 / reduced_height, 4) @ Matrix.Translation(Vector((0, 0, -reduced_bounds['min'][2])))
    target.data.transform(normalization @ target.matrix_world)
    target.matrix_world = Matrix.Identity(4)
    rig.data.transform(normalization @ rig.matrix_world)
    rig.matrix_world = Matrix.Identity(4)
    rig.data.pose_position = 'POSE'
    target.parent = rig
    armature = target.modifiers.new('InmateTwo skin', 'ARMATURE')
    armature.object = rig
    target.name = 'InmateTwoBody'
    rig.name = 'InmateTwoRig'
    texture = OUTPUT / 'InmateTwo_BaseColor.png'
    shutil.copy2(SOURCE.parent / 'BakedTexture.png', texture)
    mat = bpy.data.materials.new('InmateTwoRuntime')
    mat.use_nodes = True
    tex = mat.node_tree.nodes.new('ShaderNodeTexImage')
    tex.image = bpy.data.images.load(str(texture))
    mat.node_tree.links.new(tex.outputs['Color'], mat.node_tree.nodes.get('Principled BSDF').inputs['Base Color'])
    target.data.materials.clear()
    target.data.materials.append(mat)
    bpy.ops.object.select_all(action='DESELECT')
    target.select_set(True)
    rig.select_set(True)
    bpy.context.view_layer.objects.active = rig
    output = OUTPUT / 'InmateTwoRig.fbx'
    bpy.ops.export_scene.fbx(filepath=str(output), use_selection=True, object_types={'ARMATURE', 'MESH'}, axis_forward='-Z', axis_up='Y', apply_unit_scale=True, apply_scale_options='FBX_SCALE_UNITS', add_leaf_bones=False, use_armature_deform_only=True, bake_anim=False, path_mode='RELATIVE')
    after = {str(path.relative_to(ROOT)): sha256(path) for path in protected}
    if hashes != after:
        raise RuntimeError('Protected input hash mismatch')
    report.update({'triangles': triangles, 'vertices': len(target.data.vertices), 'max_influences': max(influences), 'unweighted_vertices': influences.count(0), 'runtime_bounds': bounds(target), 'protected_source_hashes': hashes, 'sources_unchanged': True, 'output_sha256': sha256(output), 'rig_method': 'Nearest donor polygon surface interpolation after height alignment, max four normalized influences', 'unity_validation': 'Pending humanoid import and animation visual review'})
    (OUTPUT / 'InmateTwoExportReport.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    print('INMATE_TWO_EXPORT_OK ' + json.dumps({'triangles': triangles, 'max_influences': max(influences), 'bounds': bounds(target)}))


if __name__ == "__main__":
    main()
