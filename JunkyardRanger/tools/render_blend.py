import sys, os, math
import bpy, bmesh
from mathutils import Vector
blend, outprefix = sys.argv[1], sys.argv[2]
views = sys.argv[3].split(",") if len(sys.argv) > 3 else ["front34", "rear34", "side"]
samples = int(sys.argv[4]) if len(sys.argv) > 4 else 32
bpy.ops.wm.open_mainfile(filepath=blend)
sc = bpy.context.scene
vis = [o for o in sc.objects if o.type == "MESH" and not o.hide_render]
mn = Vector((1e9,) * 3); mx = Vector((-1e9,) * 3)
for o in vis:
    for c in o.bound_box:
        w = o.matrix_world @ Vector(c)
        mn = Vector(map(min, mn, w)); mx = Vector(map(max, mx, w))
ctr = (mn + mx) / 2
size = mx - mn
print("bounds", tuple(round(v, 3) for v in mn), tuple(round(v, 3) for v in mx))
sc.render.engine = "CYCLES"; sc.cycles.samples = samples; sc.cycles.device = "CPU"
sc.render.resolution_x = 1200; sc.render.resolution_y = 750
sc.view_settings.view_transform = "Standard"
w = bpy.data.worlds.new("w"); sc.world = w; w.use_nodes = True
w.node_tree.nodes["Background"].inputs[0].default_value = (0.62, 0.72, 0.85, 1)
w.node_tree.nodes["Background"].inputs[1].default_value = 0.9
g = bpy.data.meshes.new("ground"); bm = bmesh.new(); bmesh.ops.create_grid(bm, x_segments=1, y_segments=1, size=40); bm.to_mesh(g)
go = bpy.data.objects.new("ground", g); go.location = (ctr.x, ctr.y, mn.z)
gm = bpy.data.materials.new("groundm"); gm.use_nodes = True
gm.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (0.32, 0.30, 0.27, 1)
g.materials.append(gm); sc.collection.objects.link(go)
sun = bpy.data.lights.new("sun", "SUN"); sun.energy = 4.0; sun.angle = math.radians(8)
so = bpy.data.objects.new("sun", sun); so.rotation_euler = (math.radians(50), math.radians(10), math.radians(140)); sc.collection.objects.link(so)
cam = bpy.data.cameras.new("cam"); cam.lens = 50
co = bpy.data.objects.new("cam", cam); sc.collection.objects.link(co); sc.camera = co
L = max(size.x, size.y) 
V = {
    "front34": (Vector((1.15, 1.1, 0.42)) * L, Vector((0, 0.05 * L, -0.05))),
    "rear34": (Vector((-1.05, -1.2, 0.48)) * L, Vector((0, -0.05 * L, -0.05))),
    "side": (Vector((-1.75, 0.0, 0.18)) * L, Vector((0, 0, -0.05))),
    "front": (Vector((0, 1.7, 0.2)) * L, Vector((0, 0, 0))),
    "top": (Vector((0.0, -0.001, 1.9)) * L, Vector((0, 0, 0))),
}
for name in views:
    off, tgt = V[name]
    target = ctr + tgt
    co.location = target + off
    co.rotation_euler = (target - co.location).to_track_quat("-Z", "Y").to_euler()
    sc.render.filepath = outprefix + name + ".png"
    bpy.ops.render.render(write_still=True)
    print("rendered", name)
