"""Convert the door speaker (Speaker.blend) and the D4S amp (d4s.usdz) for
Truck Parts QOL.

Usage:  python3 convert_audio_models.py Speaker.blend d4s.usdz out_dir
Needs:  pip install bpy   (Blender as a Python module; it reads .blend and USD)

Writes speaker.obj/.mtl, amp.obj/.mtl and the amp's textures to out_dir. The
mod loads them from UserData/TruckPartsQOL/models/. Both models come out in
meters, centered on their bounding box, with the side you look at once fitted
toward +Z (the speaker's cone, the amp's top) and their mounting face toward -Z.
"""
import os, sys, math
import bpy, bmesh, mathutils

SPEAKER_DIAMETER = 0.17   # 6.5" door speaker, frame outside diameter (m)
SPEAKER_KEEP = 0.35       # decimate to this share of its 44k triangles
AMP_LENGTH = 0.38         # D4S JP23.4-sized 4-channel amp (m)

speaker_blend, amp_usdz, out = sys.argv[-3:]
os.makedirs(out, exist_ok=True)


def mesh_objects():
    return [o for o in bpy.context.scene.objects if o.type == 'MESH']


def bounds(objs):
    dg = bpy.context.evaluated_depsgraph_get()
    mn = mathutils.Vector((1e18,) * 3); mx = -mn
    for o in objs:
        m = o.evaluated_get(dg).to_mesh()
        for v in m.vertices:
            w = o.matrix_world @ v.co
            mn = mathutils.Vector(map(min, mn, w)); mx = mathutils.Vector(map(max, mx, w))
        o.evaluated_get(dg).to_mesh_clear()
    return mn, mx


def bake_to_single(objs, name):
    """Apply modifiers and transforms, join into one object."""
    bpy.ops.object.select_all(action='DESELECT')
    for o in objs:
        o.select_set(True)
    bpy.context.view_layer.objects.active = objs[0]
    for o in objs:
        bpy.context.view_layer.objects.active = o
        for mod in list(o.modifiers):
            bpy.ops.object.modifier_apply(modifier=mod.name)
    bpy.context.view_layer.objects.active = objs[0]
    # USD imports hang meshes under transform-only parents: bake those in too.
    bpy.ops.object.parent_clear(type='CLEAR_KEEP_TRANSFORM')
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    if len(objs) > 1:
        bpy.ops.object.join()
    o = bpy.context.view_layer.objects.active
    o.name = name
    return o


def recenter_and_scale(o, factor):
    mn, mx = bounds([o])
    c = (mn + mx) / 2
    o.data.transform(mathutils.Matrix.Scale(factor, 4) @ mathutils.Matrix.Translation(-c))
    o.data.update()


def export(o, path, forward, up):
    bpy.ops.object.select_all(action='DESELECT')
    o.select_set(True)
    bpy.context.view_layer.objects.active = o
    bpy.ops.wm.obj_export(filepath=path, export_selected_objects=True, apply_modifiers=True,
                          forward_axis=forward, up_axis=up, export_normals=True, export_uv=True,
                          export_materials=True, export_triangulated_mesh=True, export_pbr_extensions=True, path_mode='STRIP')


# --- Speaker: one mesh (geometry nodes), cone toward Blender -Y. ---
bpy.ops.wm.open_mainfile(filepath=speaker_blend)
spk = bake_to_single([bpy.data.objects['Circle']], 'Speaker')
mn, mx = bounds([spk])
dia = max(mx.x - mn.x, mx.z - mn.z)
dec = spk.modifiers.new('decimate', 'DECIMATE'); dec.ratio = SPEAKER_KEEP
bpy.context.view_layer.objects.active = spk; bpy.ops.object.modifier_apply(modifier=dec.name)
bpy.ops.object.shade_auto_smooth(angle=math.radians(35)) if hasattr(bpy.ops.object, 'shade_auto_smooth') else None
recenter_and_scale(spk, SPEAKER_DIAMETER / dia)
for m in spk.data.materials:
    bsdf = [n for n in m.node_tree.nodes if n.type == 'BSDF_PRINCIPLED'] if m.node_tree else []
    if bsdf:
        bsdf[0].inputs['Metallic'].default_value = 0.9 if m.name == 'Copper' else 0.1
        bsdf[0].inputs['Roughness'].default_value = 0.35 if m.name == 'Copper' else 0.7
# Blender (x, y, z) -> OBJ (x, z, -y): the cone (-Y) faces OBJ +Z.
export(spk, os.path.join(out, 'speaker.obj'), 'NEGATIVE_Z', 'Y')
print('speaker: %d triangles, %.3f m across' % (sum(len(p.vertices) - 2 for p in spk.data.polygons), SPEAKER_DIAMETER))

# --- Amp: 88 USD meshes, 88 materials (21 different), top toward Blender +Z. ---
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.wm.usd_import(filepath=amp_usdz)
objs = mesh_objects()
# Textures: saved next to the OBJ under plain names ('#' starts an OBJ comment).
tex_names = {}
for i, img in enumerate(bpy.data.images):
    if img.size[0] == 0:
        continue
    name = 'amp_tex%02d.jpg' % i
    img.file_format = 'JPEG'
    img.filepath_raw = os.path.join(out, name)
    img.save()
    img.name = name
    tex_names[img] = name
# One material per texture (+ base color): 88 -> 21 draw calls.
unique = {}
for o in objs:
    for slot in o.material_slots:
        m = slot.material
        if m is None or m.node_tree is None:
            continue
        imgs = [n.image for n in m.node_tree.nodes if n.type == 'TEX_IMAGE' and n.image]
        bsdf = [n for n in m.node_tree.nodes if n.type == 'BSDF_PRINCIPLED']
        col = tuple(round(c, 3) for c in bsdf[0].inputs['Base Color'].default_value) if bsdf else (1, 1, 1, 1)
        key = (imgs[0].name if imgs else '', col)
        if key not in unique:
            unique[key] = m
            m.name = 'amp_%02d' % len(unique)
            if bsdf:
                bsdf[0].inputs['Alpha'].default_value = 1.0  # the frosted bottom plate: solid
                bsdf[0].inputs['Metallic'].default_value = 0.6  # anodized aluminium
                bsdf[0].inputs['Roughness'].default_value = 0.45
        slot.material = unique[key]
amp = bake_to_single(objs, 'Amp')
mn, mx = bounds([amp])
recenter_and_scale(amp, AMP_LENGTH / max(mx.x - mn.x, mx.y - mn.y))
# Blender axes kept: top (+Z) faces OBJ +Z, length along OBJ Y.
export(amp, os.path.join(out, 'amp.obj'), 'Y', 'Z')
mn, mx = bounds([amp])
print('amp: %d triangles, %d materials, %.3f x %.3f x %.3f m' % (sum(len(p.vertices) - 2 for p in amp.data.polygons), len(unique), *(mx - mn)))
