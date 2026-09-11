"""Audit with Python; derive using Blender --factory-startup --background --python this_file -- --derive."""
import hashlib
import json
from pathlib import Path
import sys
import shutil
import math

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / 'Assets/_InsideTheWalls/New Resources 01'
OUTPUT = ROOT / 'Assets/_InsideTheWalls/Art/Environment/NewResources01'
ASSETS = {
    'BENCH_Textured_4722484705': ('Bench', 7500, 2.7),
    'wall-light_Textured_4722584707': ('WallLight', 7500, 0.6),
    'outdoor-tile_Textured_4722684710': ('OutdoorTileA', 1200, 2.0),
    'outdoor-tile_Textured_4722684721': ('OutdoorTileB', 1200, 2.0),
    'Secure Steel Door_Textured_4717884613': ('SecureDoor', 4500, 2.5),
    'Tall-Yard-Fence_Textured_4717784612': ('YardFence', 4500, 3.0),
    'wall-panel-01_Textured_4723584732': ('WallPanel', 4500, 2.8),
}


def audit():
    records = []
    hashes = {}
    for path in sorted(SOURCE.glob('*/mesh.obj')):
        bounds = [[float('inf')] * 3, [float('-inf')] * 3]
        vertices = faces = triangles = uv = 0
        with path.open(encoding='utf-8') as stream:
            for line in stream:
                if line.startswith('v '):
                    vertices += 1
                    for axis, value in enumerate(map(float, line.split()[1:4])):
                        bounds[0][axis] = min(bounds[0][axis], value)
                        bounds[1][axis] = max(bounds[1][axis], value)
                elif line.startswith('vt '):
                    uv += 1
                elif line.startswith('f '):
                    faces += 1
                    triangles += len(line.split()) - 3
        digest = hashlib.sha256(path.read_bytes()).hexdigest()
        record = dict(source=path.parent.name, vertices=vertices, faces=faces,
                      triangles=triangles, uv_count=uv, bounds=bounds, sha256=digest,
                      duplicate_of=hashes.get(digest))
        hashes.setdefault(digest, path.parent.name)
        records.append(record)
    OUTPUT.mkdir(parents=True, exist_ok=True)
    report = dict(objects=records, character_source_names=[p.name for p in SOURCE.glob('character*/*')])
    (OUTPUT / 'source-audit.json').write_text(json.dumps(report, indent=2) + '\n')
    print(json.dumps(report, indent=2), flush=True)
    return records


def derive():
    import bpy
    from mathutils import Vector
    reports = []
    for folder, (name, budget, meters) in ASSETS.items():
        source_hash = hashlib.sha256((SOURCE / folder / 'mesh.obj').read_bytes()).hexdigest()
        bpy.ops.wm.read_factory_settings(use_empty=True)
        bpy.ops.wm.obj_import(filepath=str(SOURCE / folder / 'mesh.obj'), forward_axis='NEGATIVE_Z', up_axis='Y')
        meshes = [obj for obj in bpy.context.scene.objects if obj.type == 'MESH']
        bpy.ops.object.select_all(action='DESELECT')
        for obj in meshes:
            obj.select_set(True)
        bpy.context.view_layer.objects.active = meshes[0]
        bpy.ops.object.join()
        obj = bpy.context.object
        obj.name = name
        # OBJ Y-up becomes Blender Z-up. Tiles were supplied standing on edge.
        if name.startswith('OutdoorTile'):
            obj.rotation_euler.x += math.pi / 2
        bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)
        dimensions = obj.dimensions.copy()
        reference = dimensions.z if name == 'SecureDoor' else dimensions.x
        obj.scale = (meters / reference,) * 3
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        bounds = [obj.matrix_world @ Vector(corner) for corner in obj.bound_box]
        offset = Vector(((min(v.x for v in bounds) + max(v.x for v in bounds)) / 2,
                         (min(v.y for v in bounds) + max(v.y for v in bounds)) / 2,
                         min(v.z for v in bounds)))
        for vertex in obj.data.vertices:
            vertex.co -= offset
        source_triangles = sum(len(p.vertices) - 2 for p in obj.data.polygons)
        modifier = obj.modifiers.new('Runtime triangle budget', 'DECIMATE')
        modifier.ratio = min(1, budget / source_triangles * 0.96)
        modifier.use_collapse_triangulate = True
        bpy.ops.object.modifier_apply(modifier=modifier.name)
        triangles = sum(len(p.vertices) - 2 for p in obj.data.polygons)
        if triangles > budget or not obj.data.uv_layers:
            raise RuntimeError(f'{name}: triangle budget or UV validation failed')
        texture = OUTPUT / (name + '_BaseColor.png')
        shutil.copy2(SOURCE / folder / 'BakedTexture.png', texture)
        bpy.ops.export_scene.fbx(filepath=str(OUTPUT / (name + '.fbx')), use_selection=True,
                                 object_types={'MESH'}, axis_forward='-Z', axis_up='Y',
                                 bake_anim=False, add_leaf_bones=False, path_mode='STRIP')
        reports.append(dict(name=name, source=folder, source_triangles=source_triangles,
                            runtime_triangles=triangles, dimensions_blender=list(obj.dimensions),
                            pivot='bottom center', uniform_scale=meters / reference,
                            tile_rotation_degrees=90 if name.startswith('OutdoorTile') else 0))
        # Small orthographic workbench preview for orientation/silhouette inspection.
        scene = bpy.context.scene
        scene.render.engine = 'BLENDER_WORKBENCH'
        scene.display.shading.light = 'STUDIO'
        scene.display.shading.color_type = 'MATERIAL'
        scene.display.shading.show_shadows = True
        scene.display.shading.show_cavity = True
        scene.render.resolution_x = 512
        scene.render.resolution_y = 512
        scene.render.resolution_percentage = 100
        center = Vector((0, 0, obj.dimensions.z / 2))
        bpy.ops.object.camera_add(location=center + Vector((3, -5, 3)))
        camera = bpy.context.object
        camera.rotation_euler = (center - camera.location).to_track_quat('-Z', 'Y').to_euler()
        camera.data.type = 'ORTHO'
        camera.data.ortho_scale = max(obj.dimensions) * 1.5
        scene.camera = camera
        scene.render.filepath = str(OUTPUT / (name + '_preview.png'))
        bpy.ops.render.render(write_still=True)
        if hashlib.sha256((SOURCE / folder / 'mesh.obj').read_bytes()).hexdigest() != source_hash:
            raise RuntimeError('Source changed during export: ' + folder)
        print(json.dumps(reports[-1]), flush=True)
    (OUTPUT / 'runtime-audit.json').write_text(json.dumps(reports, indent=2) + '\n')


if __name__ == '__main__':
    audit()
    if '--derive' in sys.argv:
        derive()
