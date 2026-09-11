import bpy
import hashlib
import json
import os
import sys
import shutil
from mathutils import Vector

SETTINGS_VERSION = "1.1"


def sha256(path):
    digest = hashlib.sha256()
    with open(path, "rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest().upper()


def main():
    args = sys.argv[sys.argv.index("--") + 1 :]
    if len(args) != 3:
        raise RuntimeError("Expected source.blend, output.fbx, report.json")

    source, output, report_path = map(os.path.abspath, args)
    bpy.ops.wm.open_mainfile(filepath=source, load_ui=False, use_scripts=False)

    armature_objects = sorted((o for o in bpy.data.objects if o.type == "ARMATURE"), key=lambda o: o.name)
    mesh_objects = sorted((o for o in bpy.data.objects if o.type == "MESH"), key=lambda o: o.name)
    if len(armature_objects) != 1 or not mesh_objects:
        raise RuntimeError(f"Export refused: expected one armature and at least one mesh, found {len(armature_objects)} and {len(mesh_objects)}")
    armature_object = armature_objects[0]
    unrelated = [o.name for o in mesh_objects if o.parent != armature_object and not any(m.type == "ARMATURE" and m.object == armature_object for m in o.modifiers)]
    if unrelated:
        raise RuntimeError(f"Export refused: meshes are not linked to the sole armature: {unrelated}")

    meshes = []
    world_corners = []
    for obj in mesh_objects:
        mesh = obj.data
        triangles = sum(max(0, len(poly.vertices) - 2) for poly in mesh.polygons)
        weights = [sum(1 for membership in vertex.groups if membership.weight > 0.0) for vertex in mesh.vertices]
        corners = [obj.matrix_world @ Vector(corner) for corner in obj.bound_box]
        world_corners.extend(corners)
        meshes.append(
            {
                "name": obj.name,
                "vertices": len(mesh.vertices),
                "polygons": len(mesh.polygons),
                "triangles": triangles,
                "materials": [slot.material.name if slot.material else None for slot in obj.material_slots],
                "dimensions_m": [round(float(v), 6) for v in obj.dimensions],
                "scale": [round(float(v), 6) for v in obj.scale],
                "rotation_euler_degrees": [round(float(v) * 57.295779513, 4) for v in obj.rotation_euler],
                "location": [round(float(v), 6) for v in obj.location],
                "parent": obj.parent.name if obj.parent else None,
                "vertex_groups": len(obj.vertex_groups),
                "unweighted_vertices": sum(1 for count in weights if count == 0),
                "max_influences": max(weights, default=0),
                "shape_keys": [key.name for key in mesh.shape_keys.key_blocks] if mesh.shape_keys else [],
                "modifiers": [{"name": m.name, "type": m.type, "object": m.object.name if hasattr(m, "object") and m.object else None} for m in obj.modifiers],
                "armature_modifiers": [m.object.name for m in obj.modifiers if m.type == "ARMATURE" and m.object],
            }
        )

    armatures = [
        {
            "name": obj.name,
            "bones": len(obj.data.bones),
            "deform_bones": sum(1 for bone in obj.data.bones if bone.use_deform),
            "scale": [round(float(v), 6) for v in obj.scale],
            "rotation_euler_degrees": [round(float(v) * 57.295779513, 4) for v in obj.rotation_euler],
        }
        for obj in armature_objects
    ]
    for record, obj in zip(armatures, armature_objects):
        record["location"] = [round(float(v), 6) for v in obj.location]
        record["dimensions_m"] = [round(float(v), 6) for v in obj.dimensions]
        record["bones_ordered"] = [{"name": b.name, "parent": b.parent.name if b.parent else None, "deform": b.use_deform} for b in obj.data.bones]
    actions = [
        {
            "name": action.name,
            "frame_start": float(action.frame_range[0]),
            "frame_end": float(action.frame_range[1]),
            "fcurves": len(action.fcurves),
        }
        for action in sorted(bpy.data.actions, key=lambda a: a.name)
    ]
    materials = []
    original_image_paths = {image.name: bpy.path.abspath(image.filepath, library=image.library) for image in bpy.data.images}
    texture_manifest = []
    texture_dir = os.path.join(os.path.dirname(output), "Textures")
    os.makedirs(texture_dir, exist_ok=True)
    for mat in sorted(bpy.data.materials, key=lambda m: m.name):
        image_nodes = []
        if mat.use_nodes:
            for node in (n for n in mat.node_tree.nodes if n.type == "TEX_IMAGE"):
                image = node.image
                resolved = bpy.path.abspath(image.filepath, library=image.library) if image else ""
                sockets = [{"node": link.to_node.name, "socket": link.to_socket.name} for output_socket in node.outputs for link in output_socket.links]
                entry = {"node": node.name, "image": image.name if image else None, "resolved_path": resolved, "exists": bool(resolved and os.path.isfile(resolved)), "packed": bool(image and image.packed_file), "size": list(image.size) if image else None, "colorspace": image.colorspace_settings.name if image else None, "usage": sockets}
                if image and image.packed_file:
                    normalized = image.name.lower()
                    semantic = "Normal" if "normal" in normalized else "Roughness" if "rough" in normalized else "BaseColor"
                    extension = os.path.splitext(image.name)[1] or ".png"
                    target_name = "NoahMercer_" + semantic + extension
                    target = os.path.join(texture_dir, target_name)
                    image.save(filepath=target)
                    image.filepath = target
                    entry["handoff_path"] = "Textures/" + target_name
                    entry["handoff_sha256"] = sha256(target)
                    texture_manifest.append({"semantic": semantic, "path": entry["handoff_path"], "bytes": os.path.getsize(target), "sha256": entry["handoff_sha256"]})
                elif image and entry["exists"]:
                    target = os.path.join(texture_dir, os.path.basename(resolved))
                    shutil.copy2(resolved, target)
                    image.filepath = target
                    entry["handoff_path"] = "Textures/" + os.path.basename(target)
                    entry["handoff_sha256"] = sha256(target)
                    texture_manifest.append({"semantic": "Source", "path": entry["handoff_path"], "bytes": os.path.getsize(target), "sha256": entry["handoff_sha256"]})
                elif image:
                    raise RuntimeError(f"Material image is neither packed nor present: {mat.name}/{node.name}/{resolved}")
                image_nodes.append(entry)
        materials.append({"name": mat.name, "nodes": bool(mat.use_nodes), "image_nodes": image_nodes})
    images = [
        {
            "name": image.name,
            "size": list(image.size),
            "packed": image.packed_file is not None,
            "source_filepath": original_image_paths.get(image.name, ""),
            "handoff_path": next((entry["path"] for entry in texture_manifest if os.path.basename(entry["path"]) == os.path.basename(image.filepath)), None),
        }
        for image in sorted(bpy.data.images, key=lambda i: i.name)
        if image.name != "Render Result"
    ]

    bpy.ops.object.select_all(action="DESELECT")
    export_objects = mesh_objects + armature_objects
    for obj in export_objects:
        obj.select_set(True)
    bpy.context.view_layer.objects.active = next(o for o in export_objects if o.type == "ARMATURE")

    os.makedirs(os.path.dirname(output), exist_ok=True)
    bpy.ops.export_scene.fbx(
        filepath=output,
        use_selection=True,
        object_types={"ARMATURE", "MESH"},
        axis_forward="-Z",
        axis_up="Y",
        apply_unit_scale=True,
        apply_scale_options="FBX_SCALE_UNITS",
        add_leaf_bones=False,
        use_armature_deform_only=True,
        bake_anim=False,
        bake_anim_use_all_actions=False,
        bake_anim_use_nla_strips=False,
        bake_anim_simplify_factor=0.0,
        path_mode="COPY",
        embed_textures=False,
    )

    if not os.path.isfile(output) or os.path.getsize(output) == 0:
        raise RuntimeError("FBX exporter returned without a non-empty output")

    report = {
        "source": source,
        "source_sha256": sha256(source),
        "blender_version": bpy.app.version_string,
        "scene_unit_system": bpy.context.scene.unit_settings.system,
        "scene_unit_scale": bpy.context.scene.unit_settings.scale_length,
        "scene_fps": bpy.context.scene.render.fps / bpy.context.scene.render.fps_base,
        "world_bounds": {"min": [round(min(v[i] for v in world_corners), 6) for i in range(3)], "max": [round(max(v[i] for v in world_corners), 6) for i in range(3)]},
        "feet_minimum_y": round(min(v.y for v in world_corners), 6),
        "feet_minimum_z": round(min(v.z for v in world_corners), 6),
        "forward_facing": "Not proven automatically; requires visual Unity/DCC review.",
        "meshes": meshes,
        "armatures": armatures,
        "actions": actions,
        "animation_linkage": {"active_action": armature_object.animation_data.action.name if armature_object.animation_data and armature_object.animation_data.action else None, "nla_tracks": [track.name for track in armature_object.animation_data.nla_tracks] if armature_object.animation_data else [], "exported": False, "reason": "Source actions require compatibility/root-motion review before bounded clip export."},
        "materials": materials,
        "images": images,
        "texture_manifest": texture_manifest,
        "export": {
            "path": "NoahMercer_SourceRig.fbx",
            "bytes": os.path.getsize(output),
            "sha256": sha256(output),
            "axis_forward": "-Z",
            "axis_up": "Y",
            "apply_unit_scale": True,
            "scale_mode": "FBX_SCALE_UNITS",
            "deform_bones_only": True,
            "add_leaf_bones": False,
            "all_actions": bool(actions),
            "animations_exported": False,
            "embedded_textures": False,
            "settings_version": SETTINGS_VERSION,
            "script_sha256": sha256(os.path.abspath(__file__)),
            "runtime_classification": "Showcase/source-only: 399,998 triangles and up to 7 skin influences are not runtime-ready.",
            "intended_unity_skin_weights": "Reduce and validate to maximum 4 influences per vertex before runtime integration.",
        },
        "unity_status": "Interchange export only; Humanoid avatar and runtime readiness require Unity import validation.",
    }
    with open(report_path, "w", encoding="utf-8") as stream:
        json.dump(report, stream, indent=2)


if __name__ == "__main__":
    main()
