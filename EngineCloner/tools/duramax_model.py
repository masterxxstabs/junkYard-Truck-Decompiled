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
BANK = math.radians(45)  # each bank leans 45 degrees out

# --- Materials --------------------------------------------------------------
def mat(name, rgb, metal=0.0, rough=0.5):
    m = bpy.data.materials.new(name)
    m.use_nodes = True
    b = m.node_tree.nodes["Principled BSDF"]
    b.inputs["Base Color"].default_value = (*rgb, 1)
    b.inputs["Metallic"].default_value = metal
    b.inputs["Roughness"].default_value = rough
    return m

IRON = mat("D_Iron", (0.12, 0.125, 0.13), 0.5, 0.65)
ALU = mat("D_Alu", (0.58, 0.59, 0.61), 0.85, 0.42)
BLACK = mat("D_Black", (0.025, 0.025, 0.028), 0.1, 0.5)
GLOSS = mat("D_Gloss", (0.015, 0.015, 0.017), 0.2, 0.18)
COIL = mat("D_Coil", (0.04, 0.04, 0.045), 0.0, 0.35)
BADGE = mat("D_Badge", (0.86, 0.86, 0.88), 1.0, 0.15)
RED = mat("D_Red", (0.62, 0.05, 0.04), 0.1, 0.35)

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


def bevel(o, width, segments=2, angle=40):
    m = o.modifiers.new("bv", "BEVEL")
    m.width = width
    m.segments = segments
    m.limit_method = "ANGLE"
    m.angle_limit = math.radians(angle)
    dg = bpy.context.evaluated_depsgraph_get()
    me = bpy.data.meshes.new_from_object(o.evaluated_get(dg))
    o.modifiers.clear()
    o.data = me


def hull(points, name, slot, material, bev=0.0):
    bm = bmesh.new()
    vs = [bm.verts.new(p) for p in points]
    bmesh.ops.convex_hull(bm, input=vs)
    for v in [v for v in bm.verts if not v.link_faces]:
        bm.verts.remove(v)
    o = new_object(name, slot, bm, material)
    if bev:
        bevel(o, bev)
    o.data.shade_smooth()
    return o


def box(center, axes, half, name, slot, material, bev=0.0):
    """A box from its center, three axis vectors and half sizes along them."""
    c = Vector(center)
    pts = []
    for sx in (-1, 1):
        for sy in (-1, 1):
            for sz in (-1, 1):
                pts.append(c + axes[0] * half[0] * sx + axes[1] * half[1] * sy + axes[2] * half[2] * sz)
    return hull(pts, name, slot, material, bev)


def cylinder(center, axis, r, length, name, slot, material, seg=24, r2=None):
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
    return hull(pts, name, slot, material)


def tube(points, r, name, slot, material, seg=12):
    pts = [Vector(p) for p in points]
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
    o.data.shade_smooth()
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
    """side -1: driver's bank (leans to -x), +1: passenger's. Returns (out, across):
    out along the cylinder bores (up and away from the middle), across the bank."""
    out = Vector((side * math.sin(BANK), 0, math.cos(BANK)))
    across = Vector((math.cos(BANK), 0, -side * math.sin(BANK))) * side
    return out, across

# --- Block --------------------------------------------------------------------
L = (FRONT - REAR)
MID = (FRONT + REAR) / 2
# Crankcase and deep skirt.
box((C, MID, -1.655), (X, Y, Z), (0.135, L / 2, 0.085), "Crankcase", "v8_block", IRON, 0.012)
for side in (-1, 1):
    out, across = bank_axes(side)
    # The bank: from the crank up to the deck, leaning out.
    base = Vector((C, MID, -1.60)) + out * 0.10
    box(base, (across, Y, out), (0.105, L / 2 - 0.005, 0.095), "Bank", "v8_block", IRON, 0.01)
    # Ribs down the side of the bank and skirt.
    for i in range(6):
        y = REAR + 0.07 + i * (L - 0.14) / 5
        p = Vector((C, y, -1.62)) + out * 0.10 + across * (-0.105 * side * 0 + 0.0) + Vector((side * 0.13, 0, -0.02))
        box(p, (X, Y, Z), (0.008, 0.008, 0.06), "Rib", "v8_block", IRON, 0.003)
    # Freeze plugs.
    for i in range(3):
        y = REAR + 0.13 + i * 0.2
        cylinder((C + side * 0.14, y, -1.585), X, 0.018, 0.012, "FreezePlug", "v8_block", ALU, 16)
    # Engine mount boss.
    box((C + side * 0.15, MID + 0.06, -1.64), (X, Y, Z), (0.025, 0.045, 0.035), "MountBoss", "v8_block", IRON, 0.008)
# Bellhousing flange at the back.
hull([(C + x, REAR - 0.012 + dy, z) for x, z in ((-0.20, -1.75), (0.20, -1.75), (0.27, -1.62), (0.24, -1.44), (-0.24, -1.44), (-0.27, -1.62)) for dy in (0, 0.02)],
     "Bellhousing", "v8_block", IRON, 0.008)
# Valley cover between the banks (under the intake).
box((C, MID, -1.43), (X, Y, Z), (0.06, L / 2 - 0.03, 0.012), "Valley", "v8_block", IRON, 0.005)

# --- Heads --------------------------------------------------------------------
for side, slot in ((-1, "v8_headD_e"), (1, "v8_headP_e")):
    out, across = bank_axes(side)
    hc = Vector((C, MID, -1.60)) + out * 0.245
    box(hc, (across, Y, out), (0.13, L / 2 - 0.01, 0.055), "Head", slot, ALU, 0.01)
    # Exhaust ports along the outer side, 4 per bank.
    for i in range(4):
        y = REAR + 0.11 + i * (L - 0.22) / 3
        # "across" points to the bank's outer, lower side: where the exhaust is.
        p = hc + Vector((0, y - MID, 0)) + across * 0.135
        cylinder(p, across, 0.022, 0.03, "ExhaustPort", slot, ALU, 16)

# --- Valve covers -----------------------------------------------------------
# A diesel: no coil packs. Each cover carries a raised black injector cover
# (the textured plastic shroud over the injectors and their lines) with
# DURAMAX on it.
for side, slot in ((-1, "v8_valvecoverD_e"), (1, "v8_valvecoverP_e")):
    out, across = bank_axes(side)
    vc = Vector((C, MID, -1.60)) + out * 0.33
    box(vc, (across, Y, out), (0.105, L / 2 - 0.025, 0.035), "Cover", slot, IRON, 0.02)
    # The cover's lip.
    box(vc - out * 0.03, (across, Y, out), (0.115, L / 2 - 0.018, 0.008), "CoverLip", slot, IRON, 0.004)
    # Injector shroud: tapered, with ribs at each end.
    ic = vc + out * 0.05
    hull([ic + across * a_ + Y * y_ + out * h for a_, h in ((-0.095, -0.016), (0.095, -0.016), (0.075, 0.018), (-0.075, 0.018))
          for y_ in (-(L / 2 - 0.045), L / 2 - 0.045)], "InjectorCover", slot, BLACK, 0.012)
    for y_ in (-(L / 2 - 0.07), L / 2 - 0.07):
        box(ic + Y * y_ + out * 0.02, (across, Y, out), (0.07, 0.006, 0.004), "CoverRib", slot, GLOSS, 0.002)
    # Fuel lines peeking out below the shroud on the inner side, one per injector.
    for i in range(4):
        y = REAR + 0.11 + i * (L - 0.22) / 3
        p = vc + out * 0.035 + Vector((0, y - MID, 0)) + across * (-0.095 if side > 0 else 0.095)
        tube([p, p + Vector((-side * 0.03, 0, -0.01)), p + Vector((-side * 0.05, 0.0, -0.04))], 0.005, "FuelLine", slot, ALU, 8)
    # DURAMAX on the shroud, read from that side of the truck.
    tc = ic + out * 0.0185
    text("DURAMAX", 0.04, tc, out, X * -side, "Badge", slot, BADGE)
    # The oil filler cap on the driver's cover, at the front.
    if side < 0:
        cylinder(vc + out * 0.04 + Vector((0, L / 2 - 0.05, 0)) + across * 0.06, out, 0.022, 0.02, "OilCap", slot, GLOSS, 20)

# --- Intake and turbo ---------------------------------------------------------
# A low manifold along the valley feeding both banks, the "Y-bridge" at the
# front where the charge air comes in, and the turbocharger sitting in the
# valley at the back, its compressor outlet running forward to the Y-bridge.
box((C, MID + 0.02, -1.33), (X, Y, Z), (0.09, L / 2 - 0.05, 0.03), "Manifold", "v8_intakeman_e", ALU, 0.015)
for side in (-1, 1):
    for i in range(4):
        y = REAR + 0.11 + i * (L - 0.22) / 3
        box((C + side * 0.095, y, -1.335), (X, Y, Z), (0.025, 0.028, 0.022), "Port", "v8_intakeman_e", ALU, 0.008)
# Y-bridge: a cast elbow rising at the front, then the intake horn forward.
hull([(C + x, y, z) for x in (-0.06, 0.06) for y, z in ((2.08, -1.31), (2.24, -1.31), (2.24, -1.22), (2.13, -1.24))], "YBridge", "v8_intakeman_e", ALU, 0.02)
tube([(C, 2.20, -1.235), (C, 2.26, -1.215), (C, 2.31, -1.21)], 0.045, "Horn", "v8_intakeman_e", BLACK, 20)
cylinder((C, 2.315, -1.21), Y, 0.05, 0.02, "HornFlange", "v8_intakeman_e", ALU, 24)
text("6.6L", 0.042, (C, 2.165, -1.2335), Vector((0, 0.35, 1)), -Y, "Displacement", "v8_intakeman_e", BADGE)
# Turbo in the valley at the back: compressor (aluminium) toward the
# passenger's side, turbine (iron) toward the driver's, the center section
# between them.
TURBO = Vector((C, 1.74, -1.215))
cylinder(TURBO + X * 0.05, X, 0.072, 0.055, "Compressor", "v8_intakeman_e", ALU, 28)
cylinder(TURBO + X * 0.085, X, 0.04, 0.025, "CompressorInlet", "v8_intakeman_e", BLACK, 24)
cylinder(TURBO, X, 0.04, 0.05, "CenterSection", "v8_intakeman_e", IRON, 20)
cylinder(TURBO - X * 0.055, X, 0.066, 0.05, "Turbine", "v8_intakeman_e", IRON, 28)
# Compressor outlet forward along the valley to the Y-bridge.
tube([TURBO + X * 0.05 + Y * 0.06 + Z * 0.02, (C + 0.03, 1.92, -1.20), (C, 2.06, -1.22), (C, 2.13, -1.26)], 0.03, "ChargePipe", "v8_intakeman_e", BLACK, 16)
# Turbo mounting pedestal down to the valley.
box(TURBO - Z * 0.06, (X, Y, Z), (0.04, 0.035, 0.03), "Pedestal", "v8_intakeman_e", IRON, 0.008)

# --- Exhaust manifolds ------------------------------------------------------
# Log manifolds on the outer side of each head, and the up-pipe from the back
# of each, up over the end of the valve cover, into the turbo's turbine.
for side, slot in ((-1, "v8_exhaustmanD_e"), (1, "v8_exhaustmanP_e")):
    out, across = bank_axes(side)
    hc = Vector((C, MID, -1.60)) + out * 0.245
    edge = hc + across * 0.135
    log = edge + across * 0.035
    tube([log + Y * (L / 2 - 0.05), log, log - Y * (L / 2 - 0.06)], 0.026, "Log", slot, IRON, 16)
    for i in range(4):
        y = REAR + 0.11 + i * (L - 0.22) / 3
        box(log - across * 0.02 + Y * (y - MID), (across, Y, out), (0.022, 0.024, 0.026), "Runner", slot, IRON, 0.008)
    start = log - Y * (L / 2 - 0.06)
    turbine = TURBO - X * 0.055
    # Straight up behind the head, then in over the end of the cover.
    p1 = start - Y * 0.035 + Z * 0.07
    p2 = Vector((start.x, REAR - 0.03, -1.27))
    p3 = Vector(((start.x + C + side * 0.12) / 2, REAR - 0.035, -1.215))
    p4 = Vector((C + side * 0.11, REAR + 0.0, -1.205))
    tube([start, p1, p2, p3, p4, turbine + Vector((side * 0.03, -0.04, 0))], 0.024, "UpPipe", slot, IRON, 14)

# --- Front damper ---------------------------------------------------------------
cp = Vector((C, 2.40, CRANK_Z))
cylinder(cp, Y, 0.098, 0.04, "Damper", "v8_crankpulley_e", IRON, 36)
for k in range(5):
    cylinder(cp + Y * (0.022 + k * 0.006), Y, 0.09, 0.003, "BeltGroove", "v8_crankpulley_e", BLACK, 36)
cylinder(cp + Y * 0.024, Y, 0.035, 0.012, "DamperHub", "v8_crankpulley_e", ALU, 24)

# --- Timing cover ---------------------------------------------------------------
hull([(C + x, FRONT + dy, z) for x, z in ((-0.13, -1.75), (0.13, -1.75), (0.15, -1.62), (0.12, -1.40), (0.03, -1.37), (-0.03, -1.37), (-0.12, -1.40), (-0.15, -1.62)) for dy in (0.005, 0.045)],
     "TimingCover", "v8_timingcover_e", IRON, 0.012)
cylinder((C, FRONT + 0.05, CRANK_Z), Y, 0.045, 0.012, "CrankSeal", "v8_timingcover_e", BLACK, 24)

# --- Oil pan ----------------------------------------------------------------
hull([(C + x, y, z) for x, z in ((-0.125, -1.70), (0.125, -1.70), (0.10, -1.79), (-0.10, -1.79)) for y in (REAR + 0.005, FRONT - 0.005)]
     + [(C + x, y, -1.86) for x in (-0.08, 0.08) for y in (MID + 0.02, FRONT - 0.03)],
     "OilPan", "v8_oilpan_e", BLACK, 0.014)
cylinder((C + 0.04, FRONT - 0.09, -1.862), Z, 0.012, 0.015, "DrainPlug", "v8_oilpan_e", ALU, 12)

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
        n_index = {}
        n_lines = []
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
                corner.append("%d/%d/%d" % (vb + vi, tb, nb + n_index[key]))
            faces.setdefault(me.materials[0].name, []).append("f " + " ".join(corner))
        lines.append("vt 0 0")
        lines += n_lines
        for m, fs in faces.items():
            if m not in used:
                used.append(m)
            lines.append("usemtl " + m)
            lines += fs
        vb += len(me.vertices)
        tb += 1
        nb += len(n_lines)
open(os.path.join(OUT, "duramax.obj"), "w").write("\n".join(lines) + "\n")
mtl = ["# Materials of duramax.obj (made on the game's Standard shader in game).", ""]
for name in used:
    b = bpy.data.materials[name].node_tree.nodes["Principled BSDF"]
    col = tuple(b.inputs["Base Color"].default_value[:3])
    mtl += ["newmtl " + name, "Kd %.4f %.4f %.4f" % col, "d 1.000", "Pm %.3f" % b.inputs["Metallic"].default_value,
            "Pr %.3f" % b.inputs["Roughness"].default_value, ""]
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
    for name, loc in (("front34", (C + 0.95, FRONT + 0.85, -0.95)), ("side", (C - 1.25, MID, -1.35)), ("top", (C + 0.35, MID + 0.45, -0.55)), ("rear34", (C - 0.9, REAR - 0.85, -0.85))):
        cam.location = loc
        cam.rotation_euler = (target - Vector(loc)).to_track_quat("-Z", "Y").to_euler()
        sc.render.filepath = os.path.join(OUT, "duramax_" + name + ".png")
        bpy.ops.render.render(write_still=True)
        print("rendered", name)
