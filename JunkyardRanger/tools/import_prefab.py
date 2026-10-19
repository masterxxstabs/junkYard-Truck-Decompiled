"""Builds a Blender scene from an AssetRipper prefab: every renderer as an object
named by its path, in the prefab root's frame converted to Blender (X right,
Y forward, Z up), with materials and textures.

Run: python3 import_prefab.py <raw dir> <prefab path in package> <out.blend>
"""
import sys, os, math, json
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import bpy, bmesh
from mathutils import Vector, Matrix
from unitypkg import Package, Prefab

raw, prefab_path, out_blend = sys.argv[1], sys.argv[2], sys.argv[3]
for o in list(bpy.data.objects):
    bpy.data.objects.remove(o)

pkg = Package(raw)
pf = Prefab(pkg, pkg.by_path[prefab_path])
scene = bpy.context.scene
col_on = bpy.data.collections.new("visible")
col_off = bpy.data.collections.new("hidden_parts")
scene.collection.children.link(col_on)
scene.collection.children.link(col_off)

images = {}


def image(tex):
    f = tex.get("file")
    if not f:
        return None
    if f in images:
        return images[f]
    try:
        ext = os.path.splitext(tex.get("path") or "")[1] or ".png"
        linkdir = os.path.join(os.path.dirname(os.path.abspath(out_blend)), "tex_links")
        os.makedirs(linkdir, exist_ok=True)
        link = os.path.join(linkdir, tex["guid"] + ext)
        if not os.path.exists(link):
            os.symlink(os.path.abspath(f), link)
        img = bpy.data.images.load(link)
        img.name = os.path.basename(tex.get("path") or "tex")
    except Exception:
        img = None
    images[f] = img
    return img


materials = {}


def material(ref):
    guid = ref.get("guid")
    if guid in materials:
        return materials[guid]
    m = pkg.material(ref)
    name = (m or {}).get("name") or "missing"
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    nt = mat.node_tree
    bsdf = nt.nodes["Principled BSDF"]
    if m:
        c = m["colors"].get("_Color") or {"r": 1, "g": 1, "b": 1, "a": 1}
        base = (c.get("r", 1), c.get("g", 1), c.get("b", 1), 1)
        bsdf.inputs["Base Color"].default_value = base
        bsdf.inputs["Metallic"].default_value = float(m["floats"].get("_Metallic", 0))
        bsdf.inputs["Roughness"].default_value = 1 - float(m["floats"].get("_Glossiness", m["floats"].get("_Smoothness", 0.4)))
        mode = float(m["floats"].get("_Mode", 0))
        main = m["tex"].get("_MainTex") or m["tex"].get("_BaseMap") or m["tex"].get("_Albedo")
        if main and image(main):
            tn = nt.nodes.new("ShaderNodeTexImage")
            tn.image = image(main)
            sc = main.get("scale") or {}
            if sc and (sc.get("x", 1) != 1 or sc.get("y", 1) != 1):
                mp = nt.nodes.new("ShaderNodeMapping")
                tc = nt.nodes.new("ShaderNodeTexCoord")
                mp.inputs["Scale"].default_value = (sc.get("x", 1), sc.get("y", 1), 1)
                off = main.get("offset") or {}
                mp.inputs["Location"].default_value = (off.get("x", 0), off.get("y", 0), 0)
                nt.links.new(tc.outputs["UV"], mp.inputs["Vector"])
                nt.links.new(mp.outputs["Vector"], tn.inputs["Vector"])
            mix = nt.nodes.new("ShaderNodeMix")
            mix.data_type = "RGBA"
            mix.blend_type = "MULTIPLY"
            mix.inputs["Factor"].default_value = 1.0
            mix.inputs[6].default_value = base
            nt.links.new(tn.outputs["Color"], mix.inputs[7])
            nt.links.new(mix.outputs[2], bsdf.inputs["Base Color"])
            if mode >= 1:
                nt.links.new(tn.outputs["Alpha"], bsdf.inputs["Alpha"])
        elif mode >= 2 and c.get("a", 1) < 1:
            bsdf.inputs["Alpha"].default_value = c.get("a", 1)
        bump = m["tex"].get("_BumpMap")
        if bump and image(bump):
            tn = nt.nodes.new("ShaderNodeTexImage")
            tn.image = image(bump)
            tn.image.colorspace_settings.name = "Non-Color"
            nm = nt.nodes.new("ShaderNodeNormalMap")
            nm.inputs["Strength"].default_value = float(m["floats"].get("_BumpScale", 1))
            nt.links.new(tn.outputs["Color"], nm.inputs["Color"])
            nt.links.new(nm.outputs["Normal"], bsdf.inputs["Normal"])
        mat["unity"] = json.dumps({"shader": m["shader"][0], "colors": m["colors"], "floats": m["floats"], "tex": {k: v.get("path") for k, v in m["tex"].items()}, "keywords": m["keywords"]})
    materials[guid] = mat
    return mat


def to_blender(p):
    return (p[0], p[2], p[1])


def xform(w, v):
    return (w[0][0] * v[0] + w[0][1] * v[1] + w[0][2] * v[2] + w[0][3],
            w[1][0] * v[0] + w[1][1] * v[1] + w[1][2] * v[2] + w[1][3],
            w[2][0] * v[0] + w[2][1] * v[1] + w[2][2] * v[2] + w[2][3])


def xform_dir(w, v):
    # inverse-transpose of the 3x3 for normals
    m = Matrix(((w[0][0], w[0][1], w[0][2]), (w[1][0], w[1][1], w[1][2]), (w[2][0], w[2][1], w[2][2])))
    try:
        n = m.inverted().transposed() @ Vector(v)
    except ValueError:
        n = Vector(v)
    return n.normalized() if n.length > 0 else n


count = 0
info = []
for n in pf.nodes:
    comps = pf.components(n["go"])
    mesh_ref = None
    rend = None
    for fid, kind, f in comps:
        if kind == "MeshFilter":
            mesh_ref = f.get("m_Mesh") or {}
        elif kind == "SkinnedMeshRenderer":
            rend = f
            mesh_ref = f.get("m_Mesh") or {}
        elif kind == "MeshRenderer":
            rend = f
    if rend is None or not mesh_ref:
        continue
    md = pkg.mesh(mesh_ref)
    if not md or md.get("empty") or "builtin" in md:
        continue
    w = n["world"]
    me = bpy.data.meshes.new(md["name"])
    verts = [to_blender(xform(w, p)) for p in md["positions"]]
    faces = []
    face_mat = []
    for si, tris in enumerate(md["submeshes"]):
        for i in range(0, len(tris) - 2, 3):
            # Unity -> right-handed: reverse the winding.
            faces.append((tris[i], tris[i + 2], tris[i + 1]))
            face_mat.append(si)
    me.from_pydata(verts, [], faces)
    me.validate(clean_customdata=False)
    if md.get("uvs") and len(md["uvs"]) == len(verts):
        uvl = me.uv_layers.new(name="UVMap")
        for poly in me.polygons:
            for li in poly.loop_indices:
                vi = me.loops[li].vertex_index
                u = md["uvs"][vi]
                uvl.data[li].uv = (u[0], u[1])
    for poly, mi in zip(me.polygons, face_mat):
        poly.material_index = mi
    if md.get("normals") and len(md["normals"]) == len(verts):
        ns = [tuple(to_blender(xform_dir(w, nn))) for nn in md["normals"]]
        me.shade_smooth()
        try:
            me.normals_split_custom_set_from_vertices(ns)
        except Exception:
            pass
    for ref in rend.get("m_Materials") or []:
        me.materials.append(material(ref))
    ob = bpy.data.objects.new(n["path"] or n["name"], me)
    ob["path"] = n["path"]
    ob["mesh"] = md["name"]
    on = n["inHierarchy"] and bool(rend.get("m_Enabled", 1))
    (col_on if on else col_off).objects.link(ob)
    if not on:
        ob.hide_render = True
        ob.hide_set(True) if False else None
    count += 1
    info.append((n["path"], md["name"], len(verts), on))

bpy.ops.wm.save_as_mainfile(filepath=out_blend)
print("objects:", count, "materials:", len(materials), "images:", len(images))
