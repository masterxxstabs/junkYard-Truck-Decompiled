"""The 6.6L Duramax's model: a GM-style V8 turbo-diesel, modeled from scratch to the
game's own V8 (so it sits in the same engine bays, and every part slot lines up).

Each piece replaces the look of one of the V8's parts, and is exported in that
part's frame:

  v8_block           the block: deep-skirt crankcase, both banks, bellhousing
                     flange, ribs, freeze plugs, engine mount bosses
  v8_headD_e/P_e     aluminium heads with their exhaust ports
  v8_valvecoverD_e/P_e  covers with black injector shrouds and fuel lines, "DURAMAX"
  v8_intakeman_e     the valley manifold, the Y-bridge with its intake horn and
                     "6.6L", and the turbo sitting in the valley at the back
  v8_exhaustmanD_e/P_e  log manifolds with up-pipes to the turbo
  v8_crankpulley_e   the big front damper
  v8_timingcover_e   the front cover
  v8_oilpan_e        a deep black oil pan with its drain plug

Frames come from the F100's V8 (f1003.blend, made by JunkyardRanger/tools/
import_prefab.py from the game's F100 package). Run:

  python3 duramax_model.py f1003.blend <out_dir> [render]

<out_dir>/duramax.obj and duramax.mtl go in UserData/EngineCloner/models/.
"""
import sys, os, math
import bpy, bmesh
from mathutils import Vector, Matrix

F100_BLEND, OUT = (os.path.abspath(a) for a in sys.argv[1:3])
RENDER = len(sys.argv) > 3 and sys.argv[3] == "render"
os.makedirs(OUT, exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=F100_BLEND)
scene = bpy.context.scene

SLOTS = {
    "v8_block": "v8_block",
    "v8_headD_e": "v8_block/v8_headD_e",
    "v8_headP_e": "v8_block/v8_headP_e",
    "v8_valvecoverD_e": "v8_block/v8_headD_e/v8_valvecoverD_e",
    "v8_valvecoverP_e": "v8_block/v8_headP_e/v8_valvecoverP_e",
    "v8_intakeman_e": "v8_block/v8_intakeman_e",
    "v8_timingcover_e": "v8_block/v8_cam_e/v8_timinggear_e/v8_timingchain_e/v8_timingcover_e",
    "v8_oilpan_e": "v8_block/v8_oilpan_e",
    "v8_exhaustmanD_e": "v8_block/v8_headD_e/v8_exhaustmanD_e",
    "v8_exhaustmanP_e": "v8_block/v8_headP_e/v8_exhaustmanP_e",
    "v8_crankpulley_e": "v8_block/v8_crank_e/v8_timinggearsmall_e/v8_crankpulley_e",
    "v8_oilfilter_e": "v8_block/v8_oilfilter_e",
    "v8_starter_e": "v8_block/v8_starter_e",
    "v8_waterpump_e": "v8_block/v8_waterpump_e",
    "v8_fanwclutch_e": "v8_block/v8_waterpump_e/v8_fanwclutch_e",
    "v8_alternator_e": "v8_block/v8_headP_e/v8_alternator_e",
    "v8_fanbelt_e": "v8_block/v8_headP_e/v8_alternator_e/v8_fanbelt_e",
    "v8_hosethermostat_e": "v8_block/v8_intakeman_e/v8_hosethermostat_e",
}
by_path = {o.get("path", o.name): o for o in scene.objects}
frames = {}
for slot, path in SLOTS.items():
    frames[slot] = by_path[path].matrix_world.copy()
# The game's V8, kept for the preview (hidden in renders of the Duramax alone).
v8_objects = [o for o in scene.objects if str(o.get("path", o.name)).startswith("v8_block")]
for o in list(scene.objects):
    if o not in v8_objects:
        bpy.data.objects.remove(o)
for o in v8_objects:
    o.hide_render = True

# --- Dimensions, from the game's V8 (Blender frame: y forward, z up) ----------
C = 0.163               # the crank's x
CRANK_Z = -1.668
REAR, FRONT = 1.645, 2.305
BELT_Y = 2.44           # the front accessory drive's plane
BANK = math.radians(45)  # each bank leans 45 degrees out
L = FRONT - REAR
MID = (FRONT + REAR) / 2

# --- Textures ---------------------------------------------------------------
# Tileable, made here: albedo and a normal map for each surface (cast iron,
# sand-cast aluminium, textured plastic, rubber), so the castings have grain.
import numpy as np
from PIL import Image


def noise(size, scales, seed):
    rng = np.random.default_rng(seed)
    out = np.zeros((size, size))
    for cells, amp in scales:
        g = rng.random((cells, cells))
        # Tileable smooth upsampling (bicubic on a wrapped grid).
        img = Image.fromarray((np.tile(g, (3, 3)) * 255).astype(np.uint8)).resize((size * 3, size * 3), Image.BICUBIC)
        a = np.asarray(img, dtype=float)[size:2 * size, size:2 * size] / 255.0
        out += a * amp
    return out / sum(a for _, a in scales)


def normal_map(height, strength):
    gx = (np.roll(height, -1, 1) - np.roll(height, 1, 1)) * strength
    gy = (np.roll(height, -1, 0) - np.roll(height, 1, 0)) * strength
    n = np.dstack((-gx, -gy, np.ones_like(height)))
    n /= np.linalg.norm(n, axis=2, keepdims=True)
    return Image.fromarray(((n * 0.5 + 0.5) * 255).astype(np.uint8))


def surface(name, base, spread, scales, strength, seed, speckle=0.0):
    h = noise(512, scales, seed)
    if speckle:
        rng = np.random.default_rng(seed + 7)
        h = h - (rng.random((512, 512)) < speckle) * 0.25
    col = np.clip(np.array(base)[None, None, :] * (1 - spread / 2 + spread * h[:, :, None]), 0, 1)
    Image.fromarray((col * 255).astype(np.uint8)).save(os.path.join(OUT, name + ".png"))
    normal_map(h, strength).save(os.path.join(OUT, name + "_n.png"))
    return name + ".png", name + "_n.png"

TEX = {
    # (base color, how much it varies, noise scales, bump strength)
    "D_Iron": surface("duramax_iron", (0.20, 0.202, 0.208), 0.4, ((512, 0.55), (128, 0.25), (16, 0.2)), 1.6, 1, 0.01),
    "D_Alu": surface("duramax_alu", (0.62, 0.63, 0.65), 0.22, ((512, 0.6), (128, 0.25), (12, 0.15)), 0.9, 2, 0.004),
    "D_Black": surface("duramax_plastic", (0.05, 0.05, 0.055), 0.35, ((512, 0.8), (64, 0.2)), 0.8, 3),
    "D_Rubber": surface("duramax_rubber", (0.035, 0.035, 0.037), 0.25, ((256, 0.6), (32, 0.4)), 0.5, 4),
}

# --- Materials --------------------------------------------------------------
def mat(name, rgb, metal=0.0, rough=0.5):
    m = bpy.data.materials.new(name)
    m.use_nodes = True
    nt = m.node_tree
    b = nt.nodes["Principled BSDF"]
    b.inputs["Base Color"].default_value = (*rgb, 1)
    b.inputs["Metallic"].default_value = metal
    b.inputs["Roughness"].default_value = rough
    if name in TEX:
        alb, nrm = TEX[name]
        t = nt.nodes.new("ShaderNodeTexImage")
        t.image = bpy.data.images.load(os.path.join(OUT, alb))
        nt.links.new(t.outputs["Color"], b.inputs["Base Color"])
        tn = nt.nodes.new("ShaderNodeTexImage")
        tn.image = bpy.data.images.load(os.path.join(OUT, nrm))
        tn.image.colorspace_settings.name = "Non-Color"
        nm = nt.nodes.new("ShaderNodeNormalMap")
        nt.links.new(tn.outputs["Color"], nm.inputs["Color"])
        nt.links.new(nm.outputs["Normal"], b.inputs["Normal"])
    return m

IRON = mat("D_Iron", (0.15, 0.152, 0.158), 0.5, 0.7)
ALU = mat("D_Alu", (0.60, 0.61, 0.63), 0.8, 0.45)
BLACK = mat("D_Black", (0.045, 0.045, 0.05), 0.05, 0.6)
GLOSS = mat("D_Gloss", (0.015, 0.015, 0.017), 0.2, 0.18)
RUBBER = mat("D_Rubber", (0.03, 0.03, 0.032), 0.0, 0.85)
STEEL = mat("D_Steel", (0.72, 0.72, 0.74), 1.0, 0.25)
BADGE = mat("D_Badge", (0.86, 0.86, 0.88), 1.0, 0.15)
FILTER = mat("D_Filter", (0.05, 0.16, 0.42), 0.3, 0.3)
YELLOW = mat("D_Yellow", (0.95, 0.72, 0.05), 0.0, 0.35)
COPPER = mat("D_Copper", (0.72, 0.42, 0.22), 1.0, 0.3)

pieces = []


def new_object(name, slot, bm, material):
    me = bpy.data.meshes.new(name)
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    bm.to_mesh(me)
    bm.free()
    me.materials.append(material)
    o = bpy.data.objects.new(name, me)
    scene.collection.objects.link(o)
    o["slot"] = slot
    pieces.append(o)
    return o


def bevel(o, width, segments=4, angle=30):
    m = o.modifiers.new("bv", "BEVEL")
    m.width = width
    m.segments = segments
    m.limit_method = "ANGLE"
    m.angle_limit = math.radians(angle)
    dg = bpy.context.evaluated_depsgraph_get()
    me = bpy.data.meshes.new_from_object(o.evaluated_get(dg))
    o.modifiers.clear()
    o.data = me


def hull(points, name, slot, material, bev=0.0, segments=4):
    bm = bmesh.new()
    vs = [bm.verts.new(p) for p in points]
    bmesh.ops.convex_hull(bm, input=vs)
    for v in [v for v in bm.verts if not v.link_faces]:
        bm.verts.remove(v)
    o = new_object(name, slot, bm, material)
    if bev:
        bevel(o, bev, segments)
    return o


def box(center, axes, half, name, slot, material, bev=0.0, segments=4):
    """A box from its center, three axis vectors and half sizes along them."""
    c = Vector(center)
    pts = []
    for sx in (-1, 1):
        for sy in (-1, 1):
            for sz in (-1, 1):
                pts.append(c + axes[0] * half[0] * sx + axes[1] * half[1] * sy + axes[2] * half[2] * sz)
    # Rounded: never more than a bit under the smallest half size.
    bev = min(bev, min(half) * 0.95) if bev else 0.0
    return hull(pts, name, slot, material, bev, segments)


def cylinder(center, axis, r, length, name, slot, material, seg=32, r2=None, bev=0.0):
    axis = Vector(axis).normalized()
    up = Vector((0, 0, 1)) if abs(axis.z) < 0.9 else Vector((1, 0, 0))
    u = axis.cross(up).normalized()
    v = axis.cross(u)
    c = Vector(center)
    pts = []
    for end, rr in ((-0.5, r), (0.5, r2 if r2 is not None else r)):
        for i in range(seg):
            a = 2 * math.pi * i / seg
            pts.append(c + axis * length * end + (u * math.cos(a) + v * math.sin(a)) * rr)
    bev = min(bev, r * 0.5, length * 0.45) if bev else 0.0
    return hull(pts, name, slot, material, bev, 3)


def bolt_head(center, axis, name, slot, r=0.007):
    return cylinder(center, axis, r, 0.006, name, slot, STEEL, 6)


def tube(points, r, name, slot, material, seg=16, smooth_passes=2):
    pts = [Vector(p) for p in points]
    for _ in range(smooth_passes):
        q = [pts[0]]
        for a, b in zip(pts, pts[1:]):
            q += [a.lerp(b, 0.25), a.lerp(b, 0.75)]
        q.append(pts[-1])
        pts = q
    bm = bmesh.new()
    rings = []
    prev = None
    for i, p in enumerate(pts):
        t = (pts[min(i + 1, len(pts) - 1)] - pts[max(i - 1, 0)]).normalized()
        n = (Vector((0, 0, 1)) if abs(t.z) < 0.9 else Vector((1, 0, 0))).cross(t).normalized() if prev is None else (prev - t * prev.dot(t)).normalized()
        prev = n
        b = t.cross(n)
        rings.append([bm.verts.new(p + (n * math.cos(a) + b * math.sin(a)) * r) for a in (2 * math.pi * k / seg for k in range(seg))])
    for i in range(len(rings) - 1):
        for k in range(seg):
            kk = (k + 1) % seg
            bm.faces.new((rings[i][k], rings[i][kk], rings[i + 1][kk], rings[i + 1][k]))
    bm.faces.new(list(reversed(rings[0])))
    bm.faces.new(rings[-1])
    o = new_object(name, slot, bm, material)
    o["smooth"] = True
    return o


def text(body, size, center, normal, up, name, slot, material, depth=0.002):
    """Raised lettering facing out along `normal`, its top toward `up`."""
    cu = bpy.data.curves.new(name, "FONT")
    cu.body = body
    cu.size = size
    cu.align_x = "CENTER"
    cu.align_y = "CENTER"
    cu.extrude = depth
    tmp = bpy.data.objects.new(name, cu)
    scene.collection.objects.link(tmp)
    out = Vector(normal).normalized()
    up = Vector(up)
    up = (up - out * up.dot(out)).normalized()
    right = up.cross(out)
    tmp.matrix_world = Matrix((
        (right.x, up.x, out.x, center[0]),
        (right.y, up.y, out.y, center[1]),
        (right.z, up.z, out.z, center[2]),
        (0, 0, 0, 1)))
    dg = bpy.context.evaluated_depsgraph_get()
    me = bpy.data.meshes.new_from_object(tmp.evaluated_get(dg))
    me.transform(tmp.matrix_world)
    bpy.data.objects.remove(tmp)
    me.materials.clear()
    me.materials.append(material)
    o = bpy.data.objects.new(name, me)
    scene.collection.objects.link(o)
    o["slot"] = slot
    pieces.append(o)
    return o

X = Vector((1, 0, 0))
Y = Vector((0, 1, 0))
Z = Vector((0, 0, 1))


def bank_axes(side):
    """side -1: driver's bank (leans to -x), +1: passenger's. out: along the bores
    (up and away from the middle); across: toward the bank's outer, lower side."""
    out = Vector((side * math.sin(BANK), 0, math.cos(BANK)))
    across = Vector((math.cos(BANK), 0, -side * math.sin(BANK))) * side
    return out, across


def cyl_y(i):
    return REAR + 0.11 + i * (L - 0.22) / 3

# --- Block ---------------------------------------------------------------------
box((C, MID, -1.655), (X, Y, Z), (0.135, L / 2, 0.085), "Crankcase", "v8_block", IRON, 0.035)
for side in (-1, 1):
    out, across = bank_axes(side)
    base = Vector((C, MID, -1.60)) + out * 0.10
    box(base, (across, Y, out), (0.105, L / 2 - 0.005, 0.095), "Bank", "v8_block", IRON, 0.03)
    # Water jacket bulges along each bank, one per cylinder.
    for i in range(4):
        p = base + across * 0.1 + Y * (cyl_y(i) - MID) + out * 0.01
        cylinder(p, out, 0.055, 0.15, "Jacket", "v8_block", IRON, 24, bev=0.02)
    # Ribs down the skirt.
    for i in range(6):
        y = REAR + 0.07 + i * (L - 0.14) / 5
        box((C + side * 0.133, y, -1.64), (X, Y, Z), (0.008, 0.009, 0.055), "Rib", "v8_block", IRON, 0.006)
    for i in range(3):
        cylinder((C + side * 0.142, REAR + 0.15 + i * 0.19, -1.60), X, 0.019, 0.01, "FreezePlug", "v8_block", STEEL, 20, bev=0.003)
    # Engine mounts: boss, rubber, bracket.
    box((C + side * 0.155, MID + 0.08, -1.63), (X, Y, Z), (0.022, 0.045, 0.04), "MountBoss", "v8_block", IRON, 0.015)
    cylinder((C + side * 0.19, MID + 0.08, -1.66), Z, 0.03, 0.03, "MountRubber", "v8_block", RUBBER, 20, bev=0.006)
    box((C + side * 0.205, MID + 0.08, -1.69), (X, Y, Z), (0.03, 0.05, 0.006), "MountBracket", "v8_block", STEEL, 0.004)
# Bellhousing flange, with its bolts.
ring = [(-0.20, -1.75), (0.20, -1.75), (0.27, -1.62), (0.24, -1.44), (-0.24, -1.44), (-0.27, -1.62)]
hull([(C + x, REAR - 0.012 + dy, z) for x, z in ring for dy in (0, 0.022)], "Bellhousing", "v8_block", IRON, 0.012)
for a in range(10):
    t = a / 10 * 2 * math.pi
    bolt_head((C + 0.235 * math.cos(t), REAR - 0.016, -1.595 + 0.14 * math.sin(t)), Y, "BellBolt", "v8_block")
# Valley cover under the intake.
box((C, MID, -1.43), (X, Y, Z), (0.06, L / 2 - 0.03, 0.012), "Valley", "v8_block", IRON, 0.01)
# Dipstick: tube up the driver's side, yellow loop handle.
tube([(C - 0.135, MID + 0.12, -1.66), (C - 0.15, MID + 0.13, -1.50), (C - 0.21, MID + 0.15, -1.32), (C - 0.22, MID + 0.16, -1.22)], 0.006, "DipTube", "v8_block", STEEL, 10)
tube([(C - 0.22, MID + 0.16, -1.215), (C - 0.235, MID + 0.16, -1.19), (C - 0.22, MID + 0.16, -1.17), (C - 0.205, MID + 0.16, -1.19), (C - 0.22, MID + 0.16, -1.215)], 0.006, "DipHandle", "v8_block", YELLOW, 10, 1)
# Front accessory drive on the block: the A/C compressor (driver's side, low),
# the power steering pump (driver's side, high) and an idler.
AC = Vector((C - 0.19, BELT_Y, -1.58))
cylinder(AC - Y * 0.11, Y, 0.06, 0.18, "ACCompressor", "v8_block", ALU, 32, bev=0.012)
for k in range(5):
    cylinder(AC - Y * (0.03 + k * 0.03), Y, 0.063, 0.008, "ACFin", "v8_block", ALU, 32)
cylinder(AC, Y, 0.06, 0.03, "ACPulley", "v8_block", STEEL, 32, bev=0.004)
cylinder(AC + Y * 0.016, Y, 0.03, 0.006, "ACClutch", "v8_block", ALU, 24)
box((C - 0.17, BELT_Y - 0.07, -1.55), (X, Y, Z), (0.03, 0.05, 0.04), "ACBracket", "v8_block", IRON, 0.012)
PS = Vector((C - 0.17, BELT_Y, -1.36))
cylinder(PS - Y * 0.07, Y, 0.045, 0.11, "PSPump", "v8_block", ALU, 28, bev=0.01)
cylinder(PS - Y * 0.10 + Z * 0.06, Z, 0.032, 0.05, "PSReservoir", "v8_block", BLACK, 24, bev=0.008)
cylinder(PS, Y, 0.05, 0.025, "PSPulley", "v8_block", STEEL, 32, bev=0.004)
IDLER = Vector((C + 0.13, BELT_Y, -1.53))
cylinder(IDLER, Y, 0.035, 0.03, "Idler", "v8_block", STEEL, 28, bev=0.005)
cylinder(IDLER - Y * 0.03, Y, 0.02, 0.03, "IdlerArm", "v8_block", IRON, 16)
# The high-pressure fuel pump, in the valley at the front, with its lines.
cylinder((C, FRONT - 0.07, -1.40), Y, 0.04, 0.09, "InjectionPump", "v8_block", ALU, 28, bev=0.01)
for side in (-1, 1):
    tube([(C + side * 0.02, FRONT - 0.06, -1.36), (C + side * 0.06, FRONT - 0.09, -1.33), (C + side * 0.09, FRONT - 0.15, -1.32), (C + side * 0.10, FRONT - 0.30, -1.31)], 0.006, "RailLine", "v8_block", STEEL, 10)

# --- Heads --------------------------------------------------------------------
for side, slot in ((-1, "v8_headD_e"), (1, "v8_headP_e")):
    out, across = bank_axes(side)
    hc = Vector((C, MID, -1.60)) + out * 0.245
    box(hc, (across, Y, out), (0.13, L / 2 - 0.01, 0.055), "Head", slot, ALU, 0.025)
    for i in range(4):
        p = hc + Y * (cyl_y(i) - MID) + across * 0.135
        cylinder(p, across, 0.024, 0.03, "ExhaustPort", slot, ALU, 24, bev=0.006)
        # Glow plugs, below the ports.
        g = hc + Y * (cyl_y(i) - MID + 0.03) + across * 0.12 - out * 0.035
        cylinder(g, across, 0.006, 0.03, "GlowPlug", slot, STEEL, 10)
        cylinder(g + across * 0.017, across, 0.008, 0.008, "GlowPlugTip", slot, COPPER, 10)
    # Lifting eye at the back of the head.
    eye = hc + out * 0.06 + Y * (-(L / 2) + 0.03)
    tube([eye - Y * 0.0, eye + out * 0.03 - Y * 0.01, eye + out * 0.04 - Y * 0.035, eye + out * 0.02 - Y * 0.05, eye - Y * 0.04], 0.008, "LiftingEye", slot, IRON, 10, 1)

# --- Valve covers ---------------------------------------------------------------
for side, slot in ((-1, "v8_valvecoverD_e"), (1, "v8_valvecoverP_e")):
    out, across = bank_axes(side)
    vc = Vector((C, MID, -1.60)) + out * 0.33
    box(vc, (across, Y, out), (0.105, L / 2 - 0.025, 0.035), "Cover", slot, ALU, 0.03)
    box(vc - out * 0.03, (across, Y, out), (0.115, L / 2 - 0.018, 0.008), "CoverLip", slot, ALU, 0.006)
    # Cover bolts around the edge.
    for i in range(6):
        y = REAR + 0.06 + i * (L - 0.12) / 5
        for a in (-0.108, 0.108):
            bolt_head(vc - out * 0.022 + across * a + Y * (y - MID), out, "CoverBolt", slot)
    # Injector shroud: a rounded plastic cap, ribbed at the ends.
    ic = vc + out * 0.05
    hull([ic + across * a_ + Y * y_ + out * h for a_, h in ((-0.095, -0.016), (0.095, -0.016), (0.07, 0.02), (-0.07, 0.02))
          for y_ in (-(L / 2 - 0.045), L / 2 - 0.045)], "InjectorCover", slot, BLACK, 0.018, 4)
    for k in range(4):
        y_ = (L / 2 - 0.065 - k * 0.012)
        for sgn in (-1, 1):
            box(ic + Y * (sgn * y_) + out * 0.021, (across, Y, out), (0.06, 0.003, 0.004), "CoverRib", slot, GLOSS, 0.002, 2)
    for i in range(4):
        p = vc + out * 0.035 + Y * (cyl_y(i) - MID) - across * 0.095
        tube([p, p - across * 0.025 - out * 0.01, p - across * 0.045 - out * 0.045], 0.005, "FuelLine", slot, STEEL, 10)
    tc = ic + out * 0.021
    text("DURAMAX", 0.04, tc, out, X * -side, "Badge", slot, BADGE)
    if side < 0:
        oc = vc + out * 0.04 + Y * (L / 2 - 0.05) - across * 0.06
        cylinder(oc, out, 0.024, 0.022, "OilCap", slot, GLOSS, 28, bev=0.006)
    else:
        # Crankcase breather tube off the back of the passenger's cover.
        bp = vc + out * 0.03 - Y * (L / 2 - 0.06) - across * 0.07
        tube([bp, bp + out * 0.03, bp + out * 0.03 - Y * 0.05 - X * 0.05, bp - Y * 0.08 - X * 0.12], 0.012, "Breather", slot, RUBBER, 14)

# --- Intake and turbo ---------------------------------------------------------
box((C, MID + 0.02, -1.33), (X, Y, Z), (0.09, L / 2 - 0.05, 0.03), "Manifold", "v8_intakeman_e", ALU, 0.025)
for side in (-1, 1):
    for i in range(4):
        box((C + side * 0.095, cyl_y(i), -1.335), (X, Y, Z), (0.026, 0.03, 0.022), "Port", "v8_intakeman_e", ALU, 0.015)
# Y-bridge: a rounded cast elbow rising at the front.
tube([(C, 2.10, -1.31), (C, 2.17, -1.29), (C, 2.21, -1.255), (C, 2.235, -1.225)], 0.055, "YBridge", "v8_intakeman_e", ALU, 24)
tube([(C, 2.21, -1.23), (C, 2.26, -1.215), (C, 2.30, -1.21)], 0.047, "Horn", "v8_intakeman_e", RUBBER, 24, 1)
for k in range(3):
    cylinder((C, 2.245 + k * 0.016, -1.217), Y, 0.051, 0.006, "HornRib", "v8_intakeman_e", RUBBER, 24)
cylinder((C, 2.306, -1.21), Y, 0.05, 0.016, "HornClamp", "v8_intakeman_e", STEEL, 28)
text("6.6L", 0.04, (C, 2.155, -1.2425), Vector((0, 0.45, 1)), -Y, "Displacement", "v8_intakeman_e", BADGE)
# EGR cooler along the passenger's side of the valley.
egr = [(C + 0.085, 2.12, -1.265), (C + 0.09, 1.95, -1.262), (C + 0.09, 1.86, -1.265)]
tube(egr, 0.028, "EGRCooler", "v8_intakeman_e", STEEL, 20, 1)
for k in range(6):
    cylinder((C + 0.089, 1.885 + k * 0.04, -1.264), Y, 0.031, 0.004, "EGRFin", "v8_intakeman_e", STEEL, 20)
# Wiring harness along the driver's side of the valley, with clips.
tube([(C - 0.09, FRONT - 0.04, -1.29), (C - 0.095, 2.05, -1.282), (C - 0.09, 1.85, -1.285), (C - 0.08, REAR + 0.06, -1.29)], 0.01, "Harness", "v8_intakeman_e", RUBBER, 12)
for k in range(4):
    box((C - 0.093, REAR + 0.15 + k * 0.13, -1.285), (X, Y, Z), (0.013, 0.008, 0.013), "HarnessClip", "v8_intakeman_e", BLACK, 0.004, 2)
# Turbo in the valley at the back.
TURBO = Vector((C, 1.74, -1.215))
cylinder(TURBO + X * 0.052, X, 0.074, 0.06, "Compressor", "v8_intakeman_e", ALU, 40, bev=0.022)
cylinder(TURBO + X * 0.088, X, 0.045, 0.02, "CompressorInlet", "v8_intakeman_e", ALU, 32, bev=0.005)
cylinder(TURBO + X * 0.098, X, 0.036, 0.012, "CompressorBore", "v8_intakeman_e", BLACK, 32)
cylinder(TURBO, X, 0.04, 0.05, "CenterSection", "v8_intakeman_e", IRON, 28, bev=0.01)
cylinder(TURBO - X * 0.055, X, 0.068, 0.055, "Turbine", "v8_intakeman_e", IRON, 40, bev=0.022)
# Heat shield over the turbine, and the VGT actuator.
hull([TURBO - X * 0.055 + Vector((dx, dy, dz)) for dx in (-0.04, 0.035) for dy in (-0.075, 0.075) for dz in (0.055, 0.08)], "HeatShield", "v8_intakeman_e", STEEL, 0.01)
cylinder(TURBO + Vector((-0.02, 0.07, 0.04)), Z, 0.022, 0.03, "VGTActuator", "v8_intakeman_e", BLACK, 20, bev=0.005)
tube([TURBO + X * 0.052 + Y * 0.06 + Z * 0.02, (C + 0.035, 1.86, -1.20), (C + 0.02, 1.98, -1.205), (C, 2.06, -1.25), (C, 2.11, -1.30)], 0.031, "ChargePipe", "v8_intakeman_e", ALU, 20)
box(TURBO - Z * 0.065, (X, Y, Z), (0.04, 0.035, 0.03), "Pedestal", "v8_intakeman_e", IRON, 0.015)

# --- Thermostat housing and the upper radiator hose ---------------------------
TH = Vector((C - 0.07, FRONT - 0.02, -1.29))
cylinder(TH, Z, 0.035, 0.05, "Thermostat", "v8_hosethermostat_e", ALU, 28, bev=0.01)
tube([TH + Z * 0.02, TH + Y * 0.06 + Z * 0.05, TH + Y * 0.16 + Z * 0.08 - X * 0.01, TH + Y * 0.28 + Z * 0.10 - X * 0.02], 0.025, "UpperHose", "v8_hosethermostat_e", RUBBER, 20)
for k in (0.04, 0.26):
    cylinder(TH + Y * k + Z * (0.04 + k * 0.22), Y, 0.028, 0.008, "HoseClamp", "v8_hosethermostat_e", STEEL, 20)

# --- Exhaust manifolds ------------------------------------------------------
for side, slot in ((-1, "v8_exhaustmanD_e"), (1, "v8_exhaustmanP_e")):
    out, across = bank_axes(side)
    hc = Vector((C, MID, -1.60)) + out * 0.245
    log = hc + across * 0.17
    tube([log + Y * (L / 2 - 0.05), log, log - Y * (L / 2 - 0.06)], 0.028, "Log", slot, IRON, 24, 0)
    for i in range(4):
        box(log - across * 0.022 + Y * (cyl_y(i) - MID), (across, Y, out), (0.022, 0.026, 0.03), "Runner", slot, IRON, 0.012)
        bolt_head(log - across * 0.04 + Y * (cyl_y(i) - MID + 0.035) + out * 0.03, across, "ManifoldBolt", slot)
    start = log - Y * (L / 2 - 0.06)
    turbine = TURBO - X * 0.055
    p1 = start - Y * 0.035 + Z * 0.07
    p2 = Vector((start.x, REAR - 0.03, -1.27))
    p3 = Vector(((start.x + C + side * 0.12) / 2, REAR - 0.035, -1.215))
    p4 = Vector((C + side * 0.11, REAR, -1.205))
    tube([start, p1, p2, p3, p4, turbine + Vector((side * 0.03, -0.04, 0))], 0.024, "UpPipe", slot, IRON, 18)
    # A braided flex joint on the up-pipe.
    for k in range(4):
        cylinder(p2 + Vector((0, -0.002 * k, 0.012 * k - 0.02)), Z, 0.027, 0.006, "Flex", slot, STEEL, 18)

# --- Timing cover -------------------------------------------------------------
hull([(C + x, FRONT + dy, z) for x, z in ((-0.13, -1.75), (0.13, -1.75), (0.15, -1.62), (0.12, -1.40), (0.03, -1.37), (-0.03, -1.37), (-0.12, -1.40), (-0.15, -1.62)) for dy in (0.005, 0.045)],
     "TimingCover", "v8_timingcover_e", IRON, 0.02)
cylinder((C, FRONT + 0.05, CRANK_Z), Y, 0.045, 0.012, "CrankSeal", "v8_timingcover_e", RUBBER, 32)
for a in range(12):
    t = a / 12 * 2 * math.pi
    bolt_head((C + 0.13 * math.cos(t), FRONT + 0.048, -1.56 + 0.17 * math.sin(t)), Y, "CoverBolt", "v8_timingcover_e")

# --- Water pump and fan -------------------------------------------------------
WP = Vector((C, BELT_Y, -1.43))
cylinder(WP - Y * 0.06, Y, 0.07, 0.07, "WaterPump", "v8_waterpump_e", ALU, 32, bev=0.015)
cylinder(WP, Y, 0.07, 0.025, "WPPulley", "v8_waterpump_e", STEEL, 36, bev=0.004)
FAN = WP + Y * 0.08
cylinder(FAN - Y * 0.02, Y, 0.06, 0.05, "FanClutch", "v8_fanwclutch_e", ALU, 36, bev=0.012)
for k in range(10):
    t = k / 10 * 2 * math.pi
    box(FAN - Y * 0.02 + Vector((math.cos(t) * 0.058, 0, math.sin(t) * 0.058)), (Vector((math.cos(t), 0, math.sin(t))), Y, Vector((-math.sin(t), 0, math.cos(t)))), (0.008, 0.02, 0.002), "ClutchFin", "v8_fanwclutch_e", ALU, 0.001, 2)
# Seven curved, pitched blades, wide at the root and narrowing to the tip.
for k in range(7):
    t0 = k / 7 * 2 * math.pi
    bm = bmesh.new()
    rows = []
    for j in range(7):
        f = j / 6
        rr = 0.06 + f * 0.135
        t = t0 + 0.25 * f * f                      # swept back toward the tip
        half = math.radians(13 - 6 * f)           # narrower toward the tip
        pitch = 0.022 * (1 - 0.4 * f)
        row = []
        for sgn in (-1, 1):
            a = t + sgn * half
            for th in (-0.0025, 0.0025):
                row.append(bm.verts.new(FAN + Vector((math.cos(a) * rr, sgn * pitch + th, math.sin(a) * rr))))
        rows.append(row)
    for j in range(6):
        a_, b_ = rows[j], rows[j + 1]
        for q in ((0, 2, 3, 1), (0, 1, 1, 0)):
            pass
        bm.faces.new((a_[0], a_[2], b_[2], b_[0]))
        bm.faces.new((a_[1], b_[1], b_[3], a_[3]))
        bm.faces.new((a_[0], b_[0], b_[1], a_[1]))
        bm.faces.new((a_[2], a_[3], b_[3], b_[2]))
    bm.faces.new((rows[0][0], rows[0][1], rows[0][3], rows[0][2]))
    bm.faces.new((rows[-1][0], rows[-1][2], rows[-1][3], rows[-1][1]))
    o = new_object("FanBlade", "v8_fanwclutch_e", bm, BLACK)
cylinder(FAN, Y, 0.065, 0.012, "FanSpider", "v8_fanwclutch_e", STEEL, 36)

# --- Alternator ---------------------------------------------------------------
ALT = Vector((0.33, BELT_Y, -1.33))
cylinder(ALT - Y * 0.10, Y, 0.072, 0.13, "Alternator", "v8_alternator_e", ALU, 36, bev=0.015)
for k in range(8):
    t = k / 8 * 2 * math.pi
    box(ALT - Y * 0.04 + Vector((math.cos(t) * 0.06, 0, math.sin(t) * 0.06)), (Vector((math.cos(t), 0, math.sin(t))), Y, Vector((-math.sin(t), 0, math.cos(t)))), (0.004, 0.012, 0.012), "AltVent", "v8_alternator_e", BLACK, 0.002, 2)
cylinder(ALT - Y * 0.17, Y, 0.035, 0.03, "AltBack", "v8_alternator_e", BLACK, 24, bev=0.006)
cylinder(ALT, Y, 0.038, 0.03, "AltPulley", "v8_alternator_e", STEEL, 28, bev=0.004)
box(ALT - Y * 0.1 + Vector((-0.085, 0, -0.02)), (X, Y, Z), (0.02, 0.05, 0.012), "AltBracket", "v8_alternator_e", IRON, 0.008)

# --- Serpentine belt round all the pulleys ----------------------------------
CRANK = Vector((C, BELT_Y, CRANK_Z))
PULLEYS = [(CRANK, 0.098), (AC, 0.06), (PS, 0.05), (WP, 0.07), (ALT, 0.038), (IDLER, 0.035)]
pts2 = []
for c, r in PULLEYS:
    for k in range(64):
        t = k / 64 * 2 * math.pi
        pts2.append((c.x + (r + 0.004) * math.cos(t), c.z + (r + 0.004) * math.sin(t)))


def convex(points):
    pts = sorted(set(points))
    def cross(o, a, b):
        return (a[0] - o[0]) * (b[1] - o[1]) - (a[1] - o[1]) * (b[0] - o[0])
    lower, upper = [], []
    for p in pts:
        while len(lower) >= 2 and cross(lower[-2], lower[-1], p) <= 0:
            lower.pop()
        lower.append(p)
    for p in reversed(pts):
        while len(upper) >= 2 and cross(upper[-2], upper[-1], p) <= 0:
            upper.pop()
        upper.append(p)
    return lower[:-1] + upper[:-1]

loop = convex(pts2)
bm = bmesh.new()
rings = []
for i, (x, z) in enumerate(loop):
    a = Vector((x, 0, z))
    b = Vector((loop[(i + 1) % len(loop)][0], 0, loop[(i + 1) % len(loop)][1]))
    c0 = Vector((loop[i - 1][0], 0, loop[i - 1][1]))
    t = (b - c0).normalized()
    n = Vector((t.z, 0, -t.x))
    rings.append([bm.verts.new((x + n.x * dn, BELT_Y + dy, z + n.z * dn)) for dn, dy in ((-0.003, -0.012), (0.003, -0.012), (0.003, 0.012), (-0.003, 0.012))])
for i in range(len(rings)):
    j = (i + 1) % len(rings)
    for k in range(4):
        kk = (k + 1) % 4
        bm.faces.new((rings[i][k], rings[i][kk], rings[j][kk], rings[j][k]))
new_object("Belt", "v8_fanbelt_e", bm, RUBBER)

# --- Oil filter, starter ---------------------------------------------------------
OF = Vector((-0.052, 1.743, -1.71))
cylinder(OF, Z, 0.046, 0.12, "OilFilter", "v8_oilfilter_e", FILTER, 36, bev=0.012)
cylinder(OF - Z * 0.066, Z, 0.044, 0.012, "OilFilterBase", "v8_oilfilter_e", ALU, 36)
for k in range(8):
    t = k / 8 * 2 * math.pi
    box(OF + Z * 0.05 + Vector((math.cos(t) * 0.045, math.sin(t) * 0.045, 0)), (X, Y, Z), (0.004, 0.004, 0.008), "FilterFlute", "v8_oilfilter_e", FILTER, 0.002, 1)
ST = Vector((0.37, 1.735, -1.70))
cylinder(ST, Y, 0.05, 0.15, "Starter", "v8_starter_e", BLACK, 32, bev=0.012)
cylinder(ST + Z * 0.06 + Y * 0.02, Y, 0.025, 0.08, "Solenoid", "v8_starter_e", ALU, 24, bev=0.006)
cylinder(ST - Y * 0.09, Y, 0.03, 0.04, "StarterNose", "v8_starter_e", ALU, 24, bev=0.006)

# --- Oil pan ----------------------------------------------------------------
hull([(C + x, y, z) for x, z in ((-0.125, -1.70), (0.125, -1.70), (0.10, -1.79), (-0.10, -1.79)) for y in (REAR + 0.005, FRONT - 0.005)]
     + [(C + x, y, -1.86) for x in (-0.08, 0.08) for y in (MID + 0.02, FRONT - 0.03)],
     "OilPan", "v8_oilpan_e", IRON, 0.03)
for k in range(5):
    box((C, REAR + 0.1 + k * 0.11, -1.795), (X, Y, Z), (0.098, 0.005, 0.006), "PanRib", "v8_oilpan_e", IRON, 0.004, 2)
for k in range(8):
    bolt_head((C + (0.126 if k % 2 else -0.126), REAR + 0.07 + (k // 2) * 0.17, -1.705), Vector((1 if k % 2 else -1, 0, 0)), "PanBolt", "v8_oilpan_e")
cylinder((C + 0.04, FRONT - 0.09, -1.862), Z, 0.012, 0.015, "DrainPlug", "v8_oilpan_e", STEEL, 6)

# --- Front damper ---------------------------------------------------------------
cp = Vector((C, BELT_Y - 0.03, CRANK_Z))
cylinder(cp, Y, 0.098, 0.04, "Damper", "v8_crankpulley_e", IRON, 48, bev=0.008)
cylinder(cp - Y * 0.012, Y, 0.1, 0.012, "DamperRing", "v8_crankpulley_e", RUBBER, 48)
for k in range(5):
    cylinder(cp + Y * (0.022 + k * 0.006), Y, 0.09, 0.003, "BeltGroove", "v8_crankpulley_e", BLACK, 48)
cylinder(cp + Y * 0.025, Y, 0.035, 0.014, "DamperHub", "v8_crankpulley_e", STEEL, 32, bev=0.004)
bolt_head(cp + Y * 0.034, Y, "CrankBolt", "v8_crankpulley_e", 0.014)

# --- Texture coordinates: box-projected from the frame, 5 repeats a meter -------
for o in pieces:
    me = o.data
    uv = me.uv_layers.new(name="UVMap") if not me.uv_layers else me.uv_layers[0]
    for poly in me.polygons:
        n = poly.normal
        ax = max(range(3), key=lambda i: abs(n[i]))
        for li in poly.loop_indices:
            c = me.vertices[me.loops[li].vertex_index].co
            a_, b_ = [(c.y, c.z), (c.x, c.z), (c.x, c.y)][ax]
            uv.data[li].uv = (a_ * 5.0, b_ * 5.0)
    # Smooth where it's curved, sharp at real edges.
    me.shade_smooth()
    me.set_sharp_from_angle(angle=math.radians(38))

# --- Export --------------------------------------------------------------
# Each slot's frame: the game's (Unity) transform of that part. The import made
# Blender objects with x, y, z = Unity x, z, y; positions go out in Unity terms,
# with x mirrored like the mods' other OBJ files (their parser mirrors it back).
SWAP = Matrix(((1, 0, 0, 0), (0, 0, 1, 0), (0, 1, 0, 0), (0, 0, 0, 1)))
lines = ["# 6.6L Duramax for Engine Cloner: one group per V8 part it re-skins, in that part's frame.",
         "mtllib duramax.mtl"]
vb = tb = nb = 1
used = []
for slot in SLOTS:
    objs = [o for o in pieces if o.get("slot") == slot]
    if not objs:
        continue
    # Unity world of the slot, and its inverse.
    U = SWAP @ frames[slot] @ SWAP
    Ui = U.inverted()
    N = Ui.to_3x3().inverted().transposed()
    lines.append("o " + slot)
    for o in objs:
        me = o.data
        me.calc_loop_triangles()
        normals = me.corner_normals
        for v in me.vertices:
            p = Ui @ (SWAP @ v.co.to_4d())
            lines.append("v %.5f %.5f %.5f" % (-p.x, p.y, p.z))
        uvl = me.uv_layers.active
        n_index = {}
        n_lines = []
        t_index = {}
        t_lines = []
        faces = {}
        for tri in me.loop_triangles:
            corner = []
            for li in tri.loops:
                vi = me.loops[li].vertex_index
                nv = normals[li].vector
                nu = (N @ (SWAP.to_3x3() @ nv)).normalized()
                key = (round(-nu.x, 3), round(nu.y, 3), round(nu.z, 3))
                if key not in n_index:
                    n_index[key] = len(n_lines)
                    n_lines.append("vn %.3f %.3f %.3f" % key)
                uvk = tuple(round(c, 4) for c in uvl.data[li].uv) if uvl else (0.0, 0.0)
                if uvk not in t_index:
                    t_index[uvk] = len(t_lines)
                    t_lines.append("vt %.4f %.4f" % uvk)
                corner.append("%d/%d/%d" % (vb + vi, tb + t_index[uvk], nb + n_index[key]))
            faces.setdefault(me.materials[0].name, []).append("f " + " ".join(corner))
        lines += t_lines
        lines += n_lines
        for m, fs in faces.items():
            if m not in used:
                used.append(m)
            lines.append("usemtl " + m)
            lines += fs
        vb += len(me.vertices)
        tb += len(t_lines)
        nb += len(n_lines)
open(os.path.join(OUT, "duramax.obj"), "w").write("\n".join(lines) + "\n")
mtl = ["# Materials of duramax.obj (made on the game's Standard shader in game).", ""]
for name in used:
    b = bpy.data.materials[name].node_tree.nodes["Principled BSDF"]
    col = tuple(b.inputs["Base Color"].default_value[:3])
    mtl += ["newmtl " + name, "Kd %.4f %.4f %.4f" % col, "d 1.000", "Pm %.3f" % b.inputs["Metallic"].default_value,
            "Pr %.3f" % b.inputs["Roughness"].default_value]
    if name in TEX:
        # The texture carries the color: Kd stays as the tint (white-ish).
        mtl[-4] = "Kd 1.0000 1.0000 1.0000"
        mtl += ["map_Kd " + TEX[name][0], "map_Bump " + TEX[name][1]]
    mtl.append("")
open(os.path.join(OUT, "duramax.mtl"), "w").write("\n".join(mtl))
print("exported", len(pieces), "pieces in", len({o["slot"] for o in pieces}), "parts; materials", used)

# --- Preview ------------------------------------------------------------------
if RENDER:
    sc = scene
    sc.render.engine = "CYCLES"
    sc.cycles.samples = int(os.environ.get("SAMPLES", "32"))
    sc.cycles.device = "CPU"
    sc.render.resolution_x, sc.render.resolution_y = 1200, 800
    sc.view_settings.view_transform = "Standard"
    w = bpy.data.worlds.new("w")
    sc.world = w
    w.use_nodes = True
    w.node_tree.nodes["Background"].inputs[0].default_value = (0.55, 0.6, 0.66, 1)
    sun = bpy.data.objects.new("sun", bpy.data.lights.new("sun", "SUN"))
    sun.data.energy = 4
    sun.rotation_euler = (math.radians(40), math.radians(15), math.radians(150))
    sc.collection.objects.link(sun)
    cam = bpy.data.objects.new("cam", bpy.data.cameras.new("cam"))
    sc.collection.objects.link(cam)
    sc.camera = cam
    target = Vector((C, MID, -1.50))
    for name, loc in (("front34", (C + 0.95, FRONT + 0.85, -0.95)), ("side", (C - 1.25, MID, -1.35)), ("top", (C + 0.35, MID + 0.45, -0.55)), ("rear34", (C - 0.9, REAR - 0.85, -0.85)), ("front", (C + 0.25, FRONT + 1.2, -1.30))):
        cam.location = loc
        cam.rotation_euler = (target - Vector(loc)).to_track_quat("-Z", "Y").to_euler()
        sc.render.filepath = os.path.join(OUT, "duramax_" + name + ".png")
        bpy.ops.render.render(write_still=True)
        print("rendered", name)
