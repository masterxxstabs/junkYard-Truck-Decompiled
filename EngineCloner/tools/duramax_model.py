"""The 6.6L Duramax's model: a GM-style V8 turbo-diesel, modeled from scratch to the
game's own V8, so it sits in the same engine bays and every part slot lines up.

The engine comes apart like the game's others: each piece here is the look of one
of the V8's parts (its own mesh, in that part's frame), so the block, heads,
covers, intake, turbo, manifolds, pumps, belt... still come off, wear out and go
back on the game's way. The V8's gasoline parts get diesel looks:

  v8_block           deep-skirt block, oil cooler, A/C compressor, power steering
                     pump, idler, engine mounts, bellhousing flange, dipstick tube
  v8_headD_e/P_e     aluminium heads, exhaust ports, glow plugs
  v8_valvecoverD_e/P_e  rocker housings and black injector covers, "DURAMAX"
  oilcap             the oil filler cap (on the driver's cover)
  v8_intakeman_e     valley manifold, Y-bridge with "6.6L", EGR, charge pipe
  v8_distributor_e   the turbo (VGT), in the valley at the back
  v8_wires_e         fuel rails, injector lines, glow plug harness
  v8_carb_e          the turbo's air inlet elbow and MAF housing
  v8_airbox_e        the air filter
  v8_fuelpump_e      the CP3 high-pressure fuel pump
  v8_exhaustmanD_e/P_e  cast log manifolds and up-pipes to the turbo
  v8_timingcover_e, v8_crankpulley_e, v8_waterpump_e, v8_fanwclutch_e,
  v8_alternator_e, altfan, v8_fanbelt_e, v8_hosethermostat_e, v8_hose_e,
  v8_oilfilter_e, v8_starter_e, v8_oilpan_e

Textures are baked per part, like the game's engines: each part is unwrapped and
gets its own color map (shading in the corners, grime, worn edges, cast grain),
normal map and metal/smoothness map.

Frames come from the F100's V8 (f1003.blend, made by JunkyardRanger/tools/
import_prefab.py from the game's F100 package). Run:

  python3 duramax_model.py f1003.blend <out_dir> [render]

Everything in <out_dir> except the previews goes in UserData/EngineCloner/models/.
"""
import sys, os, math
import bpy, bmesh
from mathutils import Vector, Matrix, Quaternion

F100_BLEND, OUT = (os.path.abspath(a) for a in sys.argv[1:3])
RENDER = len(sys.argv) > 3 and sys.argv[3] == "render"
QUICK = os.environ.get("QUICK") == "1"     # small textures, few samples (iterating)
os.makedirs(OUT, exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=F100_BLEND)
scene = bpy.context.scene

# Part slot -> its path in the F100's V8, and its texture size.
SLOTS = {
    "v8_block": ("v8_block", 2048),
    "v8_headD_e": ("v8_block/v8_headD_e", 1024),
    "v8_headP_e": ("v8_block/v8_headP_e", 1024),
    "v8_valvecoverD_e": ("v8_block/v8_headD_e/v8_valvecoverD_e", 1024),
    "v8_valvecoverP_e": ("v8_block/v8_headP_e/v8_valvecoverP_e", 1024),
    "oilcap": ("v8_block/v8_headD_e/v8_valvecoverD_e/oilcap", 256),
    "v8_intakeman_e": ("v8_block/v8_intakeman_e", 1024),
    "v8_distributor_e": ("v8_block/v8_intakeman_e/v8_distributor_e", 1024),
    "v8_wires_e": ("v8_block/v8_intakeman_e/v8_distributor_e/v8_wires_e", 512),
    "v8_carb_e": ("v8_block/v8_intakeman_e/v8_carb_e", 512),
    "v8_airbox_e": ("v8_block/v8_intakeman_e/v8_carb_e/v8_airbox_e", 512),
    "v8_fuelpump_e": ("v8_block/v8_fuelpump_e", 512),
    "v8_timingcover_e": ("v8_block/v8_cam_e/v8_timinggear_e/v8_timingchain_e/v8_timingcover_e", 1024),
    "v8_oilpan_e": ("v8_block/v8_oilpan_e", 1024),
    "v8_exhaustmanD_e": ("v8_block/v8_headD_e/v8_exhaustmanD_e", 1024),
    "v8_exhaustmanP_e": ("v8_block/v8_headP_e/v8_exhaustmanP_e", 1024),
    "v8_crankpulley_e": ("v8_block/v8_crank_e/v8_timinggearsmall_e/v8_crankpulley_e", 512),
    "v8_oilfilter_e": ("v8_block/v8_oilfilter_e", 256),
    "v8_starter_e": ("v8_block/v8_starter_e", 512),
    "v8_waterpump_e": ("v8_block/v8_waterpump_e", 512),
    "v8_fanwclutch_e": ("v8_block/v8_waterpump_e/v8_fanwclutch_e", 512),
    "v8_alternator_e": ("v8_block/v8_headP_e/v8_alternator_e", 512),
    "altfan": ("v8_block/v8_headP_e/v8_alternator_e/altfan", 256),
    "v8_fanbelt_e": ("v8_block/v8_headP_e/v8_alternator_e/v8_fanbelt_e", 256),
    "v8_hosethermostat_e": ("v8_block/v8_intakeman_e/v8_hosethermostat_e", 512),
    "v8_hose_e": ("v8_block/v8_waterpump_e/v8_hose_e", 256),
}
by_path = {o.get("path", o.name): o for o in scene.objects}
frames = {slot: by_path[path].matrix_world.copy() for slot, (path, _) in SLOTS.items()}
for o in list(scene.objects):
    bpy.data.objects.remove(o)

# --- Dimensions, from the game's V8 (Blender frame: x to the passenger's side,
# y forward, z up) ------------------------------------------------------------
C = 0.163               # the crank's x
CRANK_Z = -1.668
REAR, FRONT = 1.645, 2.305
BELT_Y = 2.44           # the front accessory drive's plane
BANK = math.radians(45)  # each bank leans 45 degrees out
L = FRONT - REAR
MID = (FRONT + REAR) / 2
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


ORIGIN = Vector((C, MID, -1.60))     # where the banks' bore axes meet

# --- Surfaces -----------------------------------------------------------------
# What each piece is made of. The bake turns these into the part's textures:
# color, how worn the edges get (and to what), how much grime collects, how
# glossy and metallic it is, and how coarse the casting is.
SURF = {
    #            base color             metal rough  worn edge             wear grime bump
    "Iron":     ((0.095, 0.10, 0.105),  0.15, 0.72, (0.30, 0.29, 0.27),   0.55, 1.00, 1.0),
    "Alu":      ((0.27, 0.27, 0.26),    0.70, 0.58, (0.55, 0.55, 0.53),   0.45, 1.00, 0.8),
    "AluMach":  ((0.48, 0.48, 0.48),    0.90, 0.35, (0.70, 0.70, 0.70),   0.20, 0.60, 0.1),
    "Pulley":   ((0.20, 0.20, 0.21),    0.85, 0.48, (0.55, 0.55, 0.56),   0.50, 0.80, 0.3),
    "Plastic":  ((0.032, 0.032, 0.035), 0.00, 0.42, (0.09, 0.09, 0.095),  0.35, 0.40, 0.25),
    "Rubber":   ((0.022, 0.022, 0.024), 0.00, 0.82, (0.06, 0.06, 0.06),   0.25, 0.35, 0.2),
    "Steel":    ((0.38, 0.38, 0.40),    1.00, 0.40, (0.65, 0.65, 0.65),   0.30, 0.70, 0.1),
    "DarkSteel": ((0.10, 0.10, 0.11),   0.60, 0.50, (0.40, 0.40, 0.40),   0.50, 0.60, 0.3),
    "Zinc":     ((0.56, 0.53, 0.42),    1.00, 0.32, (0.72, 0.70, 0.62),   0.30, 0.50, 0.1),
    "Exhaust":  ((0.12, 0.08, 0.06),    0.35, 0.88, (0.26, 0.21, 0.17),   0.40, 0.60, 1.3),
    "Filter":   ((0.03, 0.075, 0.24),   0.30, 0.35, (0.55, 0.55, 0.57),   0.40, 0.50, 0.1),
    "Yellow":   ((0.85, 0.58, 0.03),    0.00, 0.40, (0.95, 0.80, 0.30),   0.20, 0.40, 0.1),
    "Copper":   ((0.60, 0.34, 0.20),    1.00, 0.35, (0.80, 0.55, 0.40),   0.30, 0.40, 0.1),
    "Badge":    ((0.78, 0.78, 0.80),    1.00, 0.22, (0.90, 0.90, 0.90),   0.10, 0.20, 0.0),
    "FilterMedia": ((0.55, 0.12, 0.10), 0.00, 0.80, (0.70, 0.30, 0.25),   0.10, 0.30, 0.6),
}

pieces = []
_names = [0]


def uniq(prefix):
    _names[0] += 1
    return "%s%d" % (prefix, _names[0])


def new_object(name, slot, bm, surf, sharp=35):
    me = bpy.data.meshes.new(name)
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    bm.to_mesh(me)
    bm.free()
    return add_mesh(me, name, slot, surf, sharp)


def add_mesh(me, name, slot, surf, sharp=35):
    o = bpy.data.objects.new(name, me)
    scene.collection.objects.link(o)
    o["slot"] = slot
    o["surf"] = surf
    o["sharp"] = sharp
    pieces.append(o)
    return o


def apply_modifier(o, kind, **props):
    m = o.modifiers.new("m", kind)
    for k, v in props.items():
        setattr(m, k, v)
    dg = bpy.context.evaluated_depsgraph_get()
    me = bpy.data.meshes.new_from_object(o.evaluated_get(dg))
    o.modifiers.clear()
    old = o.data
    o.data = me
    bpy.data.meshes.remove(old)
    return o


def bevel(o, width, segments=3, angle=30):
    if width <= 0:
        return o
    return apply_modifier(o, "BEVEL", width=width, segments=segments, limit_method="ANGLE",
                          angle_limit=math.radians(angle), harden_normals=False, use_clamp_overlap=True)


def hull(points, name, slot, surf, bev=0.0, segments=3):
    bm = bmesh.new()
    vs = [bm.verts.new(p) for p in points]
    bmesh.ops.convex_hull(bm, input=vs)
    for v in [v for v in bm.verts if not v.link_faces]:
        bm.verts.remove(v)
    bmesh.ops.dissolve_limit(bm, angle_limit=math.radians(1), verts=bm.verts, edges=bm.edges)
    o = new_object(name, slot, bm, surf)
    return bevel(o, bev, segments)


def box(center, axes, half, name, slot, surf, bev=0.0, segments=3):
    """A box from its center, three axis vectors and half sizes along them."""
    c = Vector(center)
    pts = [c + axes[0] * half[0] * sx + axes[1] * half[1] * sy + axes[2] * half[2] * sz
           for sx in (-1, 1) for sy in (-1, 1) for sz in (-1, 1)]
    bev = min(bev, min(half) * 0.95) if bev else 0.0
    return hull(pts, name, slot, surf, bev, segments)


def frame_of(axis):
    axis = Vector(axis).normalized()
    up = Vector((0, 0, 1)) if abs(axis.z) < 0.9 else Vector((1, 0, 0))
    u = axis.cross(up).normalized()
    v = axis.cross(u)
    return axis, u, v


def revolve(profile, center, axis, name, slot, surf, seg=32, rmod=None, sharp=35, phase=0.0):
    """A solid of revolution: profile is (radius, distance along the axis) from
    one end to the other; a radius of 0 closes that end. rmod(angle, index)
    multiplies a ring's radius (for flutes, pleats, ribs)."""
    axis, u, v = frame_of(axis)
    c = Vector(center)
    bm = bmesh.new()
    rings = []
    for i, (r, h) in enumerate(profile):
        if r < 1e-6:
            rings.append([bm.verts.new(c + axis * h)])
            continue
        ring = []
        for k in range(seg):
            a = 2 * math.pi * k / seg + phase
            rr = r * (rmod(a, i) if rmod else 1.0)
            ring.append(bm.verts.new(c + axis * h + (u * math.cos(a) + v * math.sin(a)) * rr))
        rings.append(ring)
    for i in range(len(rings) - 1):
        a_, b_ = rings[i], rings[i + 1]
        if len(a_) == 1 and len(b_) == 1:
            continue
        if len(a_) == 1:
            for k in range(seg):
                bm.faces.new((a_[0], b_[(k + 1) % seg], b_[k]))
        elif len(b_) == 1:
            for k in range(seg):
                bm.faces.new((a_[k], a_[(k + 1) % seg], b_[0]))
        else:
            for k in range(seg):
                kk = (k + 1) % seg
                bm.faces.new((a_[k], a_[kk], b_[kk], b_[k]))
    if len(rings[0]) > 1:
        bm.faces.new(list(reversed(rings[0])))
    if len(rings[-1]) > 1:
        bm.faces.new(rings[-1])
    return new_object(name, slot, bm, surf, sharp)


def cylinder(center, axis, r, length, name, slot, surf, seg=32, r2=None, bev=0.0, sharp=35):
    """A capped cylinder (or cone), its ends softly rounded when bev > 0."""
    r2 = r if r2 is None else r2
    h = length / 2
    if bev > 0:
        b = min(bev, r * 0.5, r2 * 0.5, h * 0.9)
        prof = [(0, -h), (r - b, -h)]
        for k in range(1, 4):
            a = math.pi / 2 * k / 4
            prof.append((r - b + b * math.sin(a), -h + b - b * math.cos(a)))
        prof += [(r, -h + b), (r2, h - b)]
        for k in range(1, 4):
            a = math.pi / 2 * k / 4
            prof.append((r2 - b + b * math.cos(a), h - b + b * math.sin(a)))
        prof += [(r2 - b, h), (0, h)]
    else:
        prof = [(0, -h), (r, -h), (r2, h), (0, h)]
    return revolve(prof, center, axis, name, slot, surf, seg, sharp=sharp)


def extrude(poly, origin, u, v, w, length, name, slot, surf, bev=0.0, segments=3, angle=30):
    """A flat outline (u, v points) pushed out along w, centered on origin."""
    o_ = Vector(origin)
    bm = bmesh.new()
    lo = [bm.verts.new(o_ + u * a + v * b - w * length / 2) for a, b in poly]
    hi = [bm.verts.new(o_ + u * a + v * b + w * length / 2) for a, b in poly]
    bm.faces.new(list(reversed(lo)))
    bm.faces.new(hi)
    n = len(poly)
    for i in range(n):
        j = (i + 1) % n
        bm.faces.new((lo[i], lo[j], hi[j], hi[i]))
    o = new_object(name, slot, bm, surf)
    return bevel(o, bev, segments, angle)


META_K = 0.57   # a metaball's surface sits at about this fraction of its radius


def meta(elements, name, slot, surf, res=0.005, tris=6000):
    """A smooth casting from blended shapes: ("ball", center, r),
    ("capsule", a, b, r), ("box", center, (u, v, w) axes, half sizes, r)."""
    mb = bpy.data.metaballs.new(uniq("MB"))
    mb.resolution = res
    mb.render_resolution = res
    mb.threshold = 0.6
    for e in elements:
        kind = e[0]
        if kind == "ball":
            el = mb.elements.new(type="BALL")
            el.co = Vector(e[1])
            el.radius = e[2] / META_K
        elif kind == "capsule":
            a, b, r = Vector(e[1]), Vector(e[2]), e[3]
            el = mb.elements.new(type="CAPSULE")
            el.co = (a + b) / 2
            el.radius = r / META_K
            el.size_x = max((b - a).length / 2, 1e-4)
            el.rotation = Vector((1, 0, 0)).rotation_difference((b - a).normalized())
        elif kind == "box":
            c, axes, half, r = Vector(e[1]), e[2], e[3], e[4]
            el = mb.elements.new(type="CUBE")
            el.co = c
            el.radius = r / META_K
            el.size_x, el.size_y, el.size_z = (max(h - r, 1e-4) for h in half)
            m = Matrix((axes[0], axes[1], axes[2])).transposed()
            el.rotation = m.to_quaternion()
        el.stiffness = 2.0
    ob = bpy.data.objects.new(mb.name, mb)
    scene.collection.objects.link(ob)
    dg = bpy.context.evaluated_depsgraph_get()
    me = bpy.data.meshes.new_from_object(ob.evaluated_get(dg))
    bpy.data.objects.remove(ob)
    bpy.data.metaballs.remove(mb)
    o = add_mesh(me, name, slot, surf, 180)
    have = len(me.polygons) * 2
    if have > tris:
        apply_modifier(o, "DECIMATE", ratio=tris / have)
    return o


def smooth_path(points, passes):
    pts = [Vector(p) for p in points]
    for _ in range(passes):
        q = [pts[0]]
        for a, b in zip(pts, pts[1:]):
            q += [a.lerp(b, 0.25), a.lerp(b, 0.75)]
        q.append(pts[-1])
        pts = q
    return pts


def tube(points, r, name, slot, surf, seg=16, passes=2, radii=None, caps=True, sharp=180):
    """A tube along a path (smoothed), radius r, or radii along the path."""
    pts = smooth_path(points, passes)
    if radii is not None:
        rr = smooth_path([Vector((x, 0, 0)) for x in radii], passes)
        rr = [p.x for p in rr]
    else:
        rr = [r] * len(pts)
    bm = bmesh.new()
    rings = []
    prev = None
    for i, p in enumerate(pts):
        t = (pts[min(i + 1, len(pts) - 1)] - pts[max(i - 1, 0)]).normalized()
        if prev is None:
            n = (Vector((0, 0, 1)) if abs(t.z) < 0.9 else Vector((1, 0, 0))).cross(t).normalized()
        else:
            n = (prev - t * prev.dot(t)).normalized()
        prev = n
        b = t.cross(n)
        rings.append([bm.verts.new(p + (n * math.cos(a) + b * math.sin(a)) * rr[i]) for a in (2 * math.pi * k / seg for k in range(seg))])
    for i in range(len(rings) - 1):
        for k in range(seg):
            kk = (k + 1) % seg
            bm.faces.new((rings[i][k], rings[i][kk], rings[i + 1][kk], rings[i + 1][k]))
    if caps:
        bm.faces.new(list(reversed(rings[0])))
        bm.faces.new(rings[-1])
    return new_object(name, slot, bm, surf, sharp)


def bolt(center, axis, slot, size=0.0065, washer=True, surf="Steel"):
    """A hex bolt head (with a flange) standing out along axis."""
    axis = Vector(axis).normalized()
    c = Vector(center)
    h = size * 0.75
    revolve([(0, 0), (size * 1.15, 0), (size * 1.15, size * 0.18), (size, size * 0.2), (size, h), (size * 0.8, h + size * 0.08), (0, h + size * 0.08)],
            c, axis, "Bolt", slot, surf, 6, sharp=40, phase=math.pi / 6)


def clamp(center, axis, r, slot, width=0.009):
    """A hose clamp band with its screw box."""
    axis, u, v = frame_of(axis)
    c = Vector(center)
    revolve([(r * 0.98, -width / 2), (r + 0.0015, -width / 2), (r + 0.0015, width / 2), (r * 0.98, width / 2)], c, axis, "Clamp", slot, "Steel", 28)
    box(c + u * (r + 0.004), (u, axis, v), (0.0045, width * 0.55, 0.006), "ClampBox", slot, "Steel", 0.0015, 2)


def text(body, size, center, normal, up, name, slot, surf, depth=0.0025):
    """Raised lettering facing out along `normal`, its top toward `up`."""
    cu = bpy.data.curves.new(name, "FONT")
    cu.body = body
    cu.size = size
    cu.align_x = "CENTER"
    cu.align_y = "CENTER"
    cu.extrude = depth
    cu.bevel_depth = depth * 0.3
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
    return add_mesh(me, name, slot, surf, 30)


def pulley(center, r, slot, name="Pulley", ribs=6, surf="Pulley", depth=0.026, hub=True):
    """A serpentine pulley: ribbed rim, flanges, web and hub, axis along y."""
    w = depth / 2
    prof = [(0, -w - 0.004), (r * 0.42, -w - 0.004), (r * 0.42, -w), (r + 0.003, -w), (r + 0.003, -w + 0.002)]
    n = max(1, ribs * 2)
    for k in range(n + 1):
        h = -w + 0.002 + (depth - 0.004) * k / n
        prof.append((r - (0.0018 if k % 2 else 0.0), h))
    prof += [(r + 0.003, w - 0.002), (r + 0.003, w), (r * 0.88, w), (r * 0.85, w - 0.004), (r * 0.40, w - 0.004)]
    if hub:
        prof += [(r * 0.38, w + 0.006), (r * 0.22, w + 0.006)]
    prof.append((0, w + (0.006 if hub else -0.004)))
    revolve(prof, center, Y, name, slot, surf, 40, sharp=40)


# =============================================================================
# The block
# =============================================================================
def block():
    s_ = "v8_block"
    pts = []
    for side in (-1, 1):
        out, across = bank_axes(side)
        deck = ORIGIN + out * 0.19
        pts.append((side, deck + across * 0.118, deck - across * 0.118))
    (dl, dlo, dli), (pr, pro, pri) = pts
    # Cross-section: rail, skirt, the banks flaring out to the decks, the valley.
    sec = [(C - 0.138, -1.705), (C - 0.146, -1.615), (dlo.x, dlo.z), (dli.x, dli.z),
           (C - 0.03, -1.452), (C + 0.03, -1.452), (pri.x, pri.z), (pro.x, pro.z),
           (C + 0.146, -1.615), (C + 0.138, -1.705)]
    poly = [(x - C, z) for x, z in sec]
    extrude(poly, (C, MID, 0), X, Z, Y, L, "Block", s_, "Iron", 0.012, 3)
    for side in (-1, 1):
        out, across = bank_axes(side)
        deck = ORIGIN + out * 0.19
        lo = Vector((C + side * 0.146, 0, -1.615))
        hi = deck + across * 0.118
        hi.y = 0
        wall = (hi - lo)
        wn = wall.normalized()
        # Cylinder barrels pushing out of the bank wall.
        for i in range(4):
            p = lo.lerp(hi, 0.55) + Y * cyl_y(i) + across * 0.004
            cylinder(p, wn, 0.042, wall.length * 0.9, "Barrel", s_, "Iron", 28, bev=0.012)
        # Deck rail: bolt bosses along the deck's outer edge.
        for i in range(5):
            y = REAR + 0.05 + i * (L - 0.10) / 4
            cylinder(hi + Y * y - out * 0.012 + across * 0.004, out, 0.011, 0.024, "HeadBoss", s_, "Iron", 16, bev=0.003)
        # Skirt ribs between the barrels, and freeze plugs.
        for i in range(5):
            y = REAR + 0.04 + i * (L - 0.08) / 4
            box(Vector((C + side * 0.146, y, -1.655)), (X, Y, Z), (0.008, 0.007, 0.045), "Rib", s_, "Iron", 0.005)
        for i in range(3):
            cylinder(Vector((C + side * 0.149, REAR + 0.15 + i * 0.19, -1.645)), X, 0.018, 0.008, "FreezePlug", s_, "Steel", 24, bev=0.002)
        # Engine mounts: boss, rubber, bracket, bolts.
        mb = Vector((C + side * 0.155, MID + 0.08, -1.64))
        box(mb, (X, Y, Z), (0.018, 0.045, 0.035), "MountBoss", s_, "Iron", 0.012)
        cylinder(mb + X * side * 0.04 - Z * 0.02, Z, 0.03, 0.034, "MountRubber", s_, "Rubber", 24, bev=0.008)
        box(mb + X * side * 0.05 - Z * 0.05, (X, Y, Z), (0.035, 0.05, 0.005), "MountBracket", s_, "DarkSteel", 0.003)
        for dy in (-0.03, 0.03):
            bolt(mb + X * side * 0.07 - Z * 0.045 + Y * dy, Z, s_)
    # The oil cooler housing on the passenger's side, with its coolant pipes.
    oc = Vector((C + 0.165, 1.96, -1.585))
    meta([("box", oc, (X, Y, Z), (0.022, 0.13, 0.034), 0.014)], "OilCooler", s_, "Iron", 0.004, 3000)
    for k in range(9):
        bolt(oc + X * 0.022 + Y * (-0.12 + k * 0.03) + Z * (0.028 if k % 2 else -0.028), X, s_, 0.005)
    for dy in (-0.09, 0.09):
        tube([oc + Y * dy + X * 0.02, oc + Y * dy + X * 0.05, oc + Y * dy * 1.3 + X * 0.06 + Z * 0.04], 0.012, "CoolerPipe", s_, "Rubber", 16, 2)
    # Bellhousing flange at the back, with its bolts.
    bell = [(-0.21, -1.76), (0.0, -1.79), (0.21, -1.76), (0.285, -1.64), (0.27, -1.52), (0.21, -1.44), (-0.21, -1.44), (-0.27, -1.52), (-0.285, -1.64)]
    extrude(bell, (C, REAR - 0.002, 0), X, Z, Y, 0.024, "Bellhousing", s_, "Iron", 0.008, 3)
    for a in range(12):
        t = a / 12 * 2 * math.pi
        bolt(Vector((C + 0.245 * math.cos(t), REAR - 0.015, -1.605 + 0.15 * math.sin(t))), -Y, s_, 0.0075)
    # Dipstick tube up to the game's dipstick handle (front, passenger's side).
    tube([(C + 0.148, 2.18, -1.66), (C + 0.17, 2.22, -1.56), (C + 0.19, 2.27, -1.46), (0.365, 2.293, -1.39)], 0.0055, "DipTube", s_, "Steel", 10)
    # Front accessories on the block: A/C compressor (driver's side, low), power
    # steering pump (driver's side, high), idler.
    AC = Vector((C - 0.19, BELT_Y, -1.58))
    revolve([(0, -0.20), (0.05, -0.20), (0.058, -0.19), (0.06, -0.12), (0.064, -0.11), (0.064, -0.10), (0.06, -0.09), (0.06, -0.05), (0.045, -0.04), (0, -0.04)],
            AC, Y, "ACCompressor", s_, "Alu", 40)
    for k in range(4):
        revolve([(0.059, -0.17 + k * 0.02), (0.066, -0.168 + k * 0.02), (0.066, -0.162 + k * 0.02), (0.059, -0.16 + k * 0.02)], AC, Y, "ACFin", s_, "Alu", 40)
    box(AC - Y * 0.12 + Z * 0.065, (X, Y, Z), (0.022, 0.03, 0.012), "ACManifold", s_, "Alu", 0.006)
    for dx in (-0.012, 0.012):
        cylinder(AC - Y * 0.12 + Z * 0.085 + X * dx, Z, 0.006, 0.02, "ACPort", s_, "Steel", 12)
    pulley(AC, 0.058, s_, "ACPulley")
    cylinder(AC + Y * 0.02, Y, 0.028, 0.008, "ACClutch", s_, "DarkSteel", 28, bev=0.002)
    box(Vector((C - 0.165, BELT_Y - 0.085, -1.555)), (X, Y, Z), (0.034, 0.05, 0.045), "ACBracket", s_, "Iron", 0.012)
    PS = Vector((C - 0.17, BELT_Y, -1.36))
    revolve([(0, -0.14), (0.04, -0.14), (0.046, -0.13), (0.046, -0.04), (0.035, -0.03), (0, -0.03)], PS, Y, "PSPump", s_, "Alu", 32)
    res = PS - Y * 0.09 + Z * 0.065
    cylinder(res, Z, 0.033, 0.05, "PSReservoir", s_, "Plastic", 28, bev=0.008)
    cylinder(res + Z * 0.03, Z, 0.022, 0.012, "PSCap", s_, "Plastic", 6 * 4, bev=0.003)
    tube([PS - Y * 0.1 + X * 0.04, PS - Y * 0.14 + X * 0.08 - Z * 0.02, PS - Y * 0.2 + X * 0.08 - Z * 0.08], 0.008, "PSHose", s_, "Rubber", 12)
    pulley(PS, 0.05, s_, "PSPulley")
    IDLER = Vector((C + 0.13, BELT_Y, -1.53))
    pulley(IDLER, 0.035, s_, "Idler", ribs=0, surf="AluMach")
    box(IDLER - Y * 0.045 - X * 0.02, (X, Y, Z), (0.035, 0.02, 0.014), "IdlerArm", s_, "Iron", 0.008)
    bolt(IDLER + Y * 0.02, Y, s_, 0.008)
    # Oil filter adapter above the filter, driver's side.
    box(Vector((-0.04, 1.743, -1.62)), (X, Y, Z), (0.04, 0.05, 0.028), "FilterAdapter", s_, "Alu", 0.012)
    # Drain cocks and sensors.
    bolt(Vector((C - 0.149, 1.80, -1.69)), -X, s_, 0.008)
    bolt(Vector((C + 0.149, 1.80, -1.69)), X, s_, 0.008)
    cylinder(Vector((C - 0.149, 2.08, -1.62)), -X, 0.01, 0.02, "Sensor", s_, "Plastic", 16)


# =============================================================================
# Heads, valve covers, oil cap
# =============================================================================
def head(side, s_):
    out, across = bank_axes(side)
    hc = ORIGIN + out * 0.245
    # Section in (across, out): wider at the deck, with the cover rail on top.
    sec = [(-0.128, -0.055), (0.128, -0.055), (0.13, -0.035), (0.122, 0.035), (0.118, 0.042), (0.118, 0.055),
           (-0.118, 0.055), (-0.118, 0.042), (-0.122, 0.035), (-0.13, -0.035)]
    extrude(sec, hc, across, out, Y, L - 0.012, "Head", s_, "Alu", 0.008, 3)
    for i in range(4):
        y = cyl_y(i) - MID
        # Exhaust port faces, glow plug bosses with plugs.
        box(hc + across * 0.133 + Y * y - out * 0.005, (across, Y, out), (0.008, 0.034, 0.028), "PortFace", s_, "Alu", 0.006)
        g = hc + across * 0.128 + Y * (y + 0.05) + out * 0.03
        cylinder(g, across, 0.009, 0.02, "GlowBoss", s_, "Alu", 16, bev=0.003)
        cylinder(g + across * 0.015, across, 0.0045, 0.02, "GlowPlug", s_, "Steel", 6)
        cylinder(g + across * 0.026, across, 0.003, 0.008, "GlowTip", s_, "Copper", 10)
        # Intake side bosses for the manifold.
        box(hc - across * 0.13 + Y * y + out * 0.015, (across, Y, out), (0.006, 0.028, 0.02), "IntakeFace", s_, "Alu", 0.005)
    # Head bolts along the outer edge, and coolant crossover bosses at the front.
    for i in range(5):
        y = -L / 2 + 0.04 + i * (L - 0.08) / 4
        bolt(hc + across * 0.122 + out * 0.058 + Y * y, out, s_, 0.0065)
    cylinder(hc + Y * (L / 2 - 0.004) + out * 0.02, Y, 0.022, 0.012, "CoolantBoss", s_, "Alu", 24, bev=0.004)
    # Lifting eye at the back of the head.
    eye = hc + out * 0.055 + Y * (-(L / 2) + 0.03) + across * 0.06
    tube([eye, eye + out * 0.035 - Y * 0.01, eye + out * 0.045 - Y * 0.035, eye + out * 0.025 - Y * 0.055, eye - Y * 0.045], 0.007, "LiftingEye", s_, "Iron", 10, 1)


def covers(side, s_):
    out, across = bank_axes(side)
    vc = ORIGIN + out * 0.33
    # The aluminium rocker housing: a cast frame, ribbed on its outside face.
    sec = [(-0.117, -0.03), (0.117, -0.03), (0.114, 0.012), (0.105, 0.02), (-0.105, 0.02), (-0.114, 0.012)]
    extrude(sec, vc, across, out, Y, L - 0.02, "RockerHousing", s_, "Alu", 0.008, 3)
    for k in range(3):
        box(vc + across * 0.117 - out * (0.022 - k * 0.012), (across, Y, out), (0.004, L / 2 - 0.04, 0.0025), "HousingRib", s_, "Alu", 0.002, 2)
    for i in range(6):
        y = -L / 2 + 0.05 + i * (L - 0.10) / 5
        for a in (-0.108, 0.108):
            cylinder(vc + across * a + Y * y + out * 0.016, out, 0.009, 0.014, "BoltBoss", s_, "Alu", 16, bev=0.003)
            bolt(vc + across * a + Y * y + out * 0.023, out, s_, 0.0055)
    # The black injector cover: a smooth rounded hump, a raised plateau with
    # "DURAMAX", ribs at both ends.
    ic = vc + out * 0.02
    hump = [("box", ic + out * 0.016, (across, Y, out), (0.096, L / 2 - 0.035, 0.022), 0.02),
            ("box", ic + out * 0.034, (across, Y, out), (0.07, L / 2 - 0.07, 0.016), 0.014)]
    meta(hump, "InjectorCover", s_, "Plastic", 0.004, 6000)
    top = ic + out * 0.052
    for sgn in (-1, 1):
        for k in range(5):
            y_ = sgn * (L / 2 - 0.055 - k * 0.011)
            box(top - out * 0.012 + Y * y_, (across, Y, out), (0.055, 0.0028, 0.006), "CoverRib", s_, "Plastic", 0.0022, 2)
    text("DURAMAX", 0.038, top, out, X * -side, "Badge", s_, "Badge")
    if side < 0:
        # Oil filler neck up to the cap (see oil_cap()).
        cap = Vector((-0.125, 1.84, -1.21))
        base = cap - out * 0.05
        foot = base - out * (base - vc).dot(out) + out * 0.03
        cylinder((foot + base) / 2, out, 0.026, (base - foot).length + 0.004, "FillerNeck", s_, "Plastic", 28, bev=0.004)
        cylinder(foot, out, 0.034, 0.012, "NeckFoot", s_, "Plastic", 28, bev=0.004)
    else:
        # Crankcase breather off the back of the passenger's cover.
        bp = vc + out * 0.04 - Y * (L / 2 - 0.06) - across * 0.06
        cylinder(bp, out, 0.016, 0.03, "BreatherPort", s_, "Plastic", 20, bev=0.004)
        tube([bp + out * 0.012, bp + out * 0.04 - Y * 0.01, bp + out * 0.04 - Y * 0.06 - X * 0.05, bp - Y * 0.09 - X * 0.12 + out * 0.01], 0.013, "Breather", s_, "Rubber", 16)
        clamp(bp + out * 0.016, out, 0.0135, s_)


def oil_cap():
    s_ = "oilcap"
    out, _ = bank_axes(-1)
    cap = Vector((-0.125, 1.84, -1.21))
    revolve([(0, -0.025), (0.028, -0.025), (0.03, -0.022), (0.03, -0.012), (0.033, -0.009), (0.033, 0.006), (0.028, 0.012), (0.02, 0.014), (0, 0.014)],
            cap, out, "OilCap", s_, "Yellow", 48, rmod=lambda a, i: 1.0 + (0.045 * max(0, math.cos(a * 12)) if 4 <= i <= 5 else 0.0))
    box(cap + out * 0.016, (bank_axes(-1)[1], Y, out), (0.022, 0.004, 0.006), "CapGrip", s_, "Yellow", 0.003, 2)


# =============================================================================
# Intake, turbo, inlet, air filter
# =============================================================================
TURBO = Vector((C, 1.72, -1.215))
COMP_OUT = TURBO + X * 0.055 + Y * 0.07 + Z * 0.05     # compressor outlet (forward)
COMP_IN = TURBO + X * 0.11                              # compressor inlet (to the side)
TURB_IN = TURBO - X * 0.06 - Y * 0.06 - Z * 0.03        # turbine inlet (the up-pipes)


def intake():
    s_ = "v8_intakeman_e"
    els = []
    for side in (-1, 1):
        px = C + side * 0.058
        els.append(("capsule", (px, REAR + 0.07, -1.335), (px, FRONT - 0.12, -1.335), 0.028))
        for i in range(4):
            els.append(("capsule", (px, cyl_y(i), -1.336), (C + side * 0.088, cyl_y(i), -1.336), 0.022))
    els.append(("capsule", (C - 0.05, FRONT - 0.13, -1.33), (C + 0.05, FRONT - 0.13, -1.33), 0.032))
    els.append(("capsule", (C, FRONT - 0.13, -1.33), (C, FRONT - 0.20, -1.32), 0.034))
    meta(els, "Manifold", s_, "Alu", 0.005, 9000)
    for side in (-1, 1):
        for i in range(4):
            bolt(Vector((C + side * 0.07, cyl_y(i) + 0.04, -1.30)), Z, s_, 0.005)
    # Y-bridge rising at the front to the intercooler pipe, with "6.6L".
    yb = [Vector((C, FRONT - 0.17, -1.31)), Vector((C, FRONT - 0.11, -1.285)), Vector((C, FRONT - 0.07, -1.24)), Vector((C, FRONT - 0.04, -1.205)), Vector((C, FRONT + 0.0, -1.195))]
    tube(yb, 0.05, "YBridge", s_, "Alu", 32, 3, radii=[0.052, 0.05, 0.047, 0.045, 0.045])
    revolve([(0.03, 0.0), (0.052, 0.0), (0.054, 0.004), (0.054, 0.012), (0.03, 0.012)], Vector((C, FRONT + 0.0, -1.195)), Y, "InletFlange", s_, "Alu", 40)
    tube([(C, FRONT + 0.01, -1.195), (C, FRONT + 0.06, -1.195)], 0.047, "Coupler", s_, "Rubber", 32, 0)
    for k in (0.02, 0.05):
        clamp(Vector((C, FRONT + k, -1.195)), Y, 0.048, s_, 0.008)
    text("6.6L", 0.036, Vector((C, FRONT - 0.085, -1.247)), Vector((0, -0.45, 1)), Y, "Displacement", s_, "Badge")
    # EGR valve on top of the bridge, and the EGR cooler along the passenger's side.
    egr = Vector((C - 0.045, FRONT - 0.13, -1.255))
    cylinder(egr, Z, 0.024, 0.05, "EGRValve", s_, "Alu", 28, bev=0.006)
    cylinder(egr + Z * 0.033, Z, 0.02, 0.018, "EGRMotor", s_, "Plastic", 28, bev=0.004)
    box(egr + Z * 0.036 - X * 0.024, (X, Y, Z), (0.01, 0.007, 0.006), "EGRPlug", s_, "Plastic", 0.002, 2)
    eg = [(C + 0.088, FRONT - 0.15, -1.275), (C + 0.09, 1.95, -1.27), (C + 0.088, 1.84, -1.272)]
    tube(eg, 0.026, "EGRCooler", s_, "Steel", 24, 1)
    for k in range(7):
        revolve([(0.026, -0.002), (0.029, -0.0015), (0.029, 0.0015), (0.026, 0.002)], Vector((C + 0.089, 1.87 + k * 0.035, -1.272)), Y, "EGRFin", s_, "Steel", 24)
    # Wiring harness along the driver's side of the valley, with clips.
    tube([(C - 0.09, FRONT - 0.04, -1.29), (C - 0.095, 2.05, -1.283), (C - 0.09, 1.85, -1.286), (C - 0.08, REAR + 0.06, -1.29)], 0.009, "Harness", s_, "Rubber", 12)
    for k in range(4):
        box(Vector((C - 0.093, REAR + 0.15 + k * 0.13, -1.285)), (X, Y, Z), (0.012, 0.008, 0.012), "HarnessClip", s_, "Plastic", 0.003, 2)
    # Charge pipe: turbo outlet forward to the bridge.
    tube([COMP_OUT + Y * 0.01, COMP_OUT + Y * 0.06 + Z * 0.01, Vector((C + 0.03, 1.95, -1.17)), Vector((C + 0.01, 2.05, -1.19)), Vector((C, 2.12, -1.24)), Vector((C, FRONT - 0.15, -1.29))],
         0.03, "ChargePipe", s_, "Alu", 28, 2)
    clamp(COMP_OUT + Y * 0.03 + Z * 0.004, Y, 0.031, s_)


def turbo():
    """The turbo in the valley: compressor (aluminium scroll) to the passenger's
    side, turbine (iron) to the driver's, the center section between, VGT
    actuator, heat shield, oil lines."""
    s_ = "v8_distributor_e"
    T = TURBO
    # Compressor: back plate, scroll, inlet bell with its wheel.
    revolve([(0, 0.0), (0.06, 0.0), (0.066, 0.006), (0.066, 0.022), (0.05, 0.03), (0, 0.03)], T + X * 0.03, X, "CompBack", s_, "Alu", 48)
    scroll = []
    radii = []
    n = 24
    for k in range(n + 1):
        f = k / n
        a = -math.pi * 0.35 + f * 2 * math.pi * 0.9
        rc = 0.05 + 0.02 * f
        scroll.append(T + X * 0.05 + (Y * math.cos(a) + Z * math.sin(a)) * rc)
        radii.append(0.014 + 0.016 * f)
    # Out of the scroll, forward to the outlet.
    scroll += [scroll[-1] + Y * 0.02, COMP_OUT]
    radii += [0.03, 0.03]
    tube(scroll, 0.02, "CompScroll", s_, "Alu", 24, 1, radii=radii)
    revolve([(0.034, 0.0), (0.044, 0.0), (0.048, 0.02), (0.05, 0.05), (0.052, 0.058), (0.044, 0.06), (0.038, 0.055), (0.036, 0.0)], T + X * 0.05, X, "CompInlet", s_, "AluMach", 48)
    cylinder(T + X * 0.07, X, 0.034, 0.04, "CompBore", s_, "DarkSteel", 32)
    cylinder(T + X * 0.09, X, 0.009, 0.012, "WheelNut", s_, "Steel", 6)
    for k in range(7):
        a = k / 7 * 2 * math.pi
        d = Y * math.cos(a) + Z * math.sin(a)
        t_ = Y * -math.sin(a) + Z * math.cos(a)
        box(T + X * 0.085 + d * 0.02, (d, X, t_), (0.012, 0.008, 0.0012), "CompBlade", s_, "AluMach", 0.0005, 1)
    revolve([(0.046, -0.004), (0.052, -0.004), (0.052, 0.006), (0.046, 0.006)], T + X * 0.104, X, "InletClamp", s_, "Steel", 40)
    # Center section, with the oil feed and drain.
    revolve([(0, -0.03), (0.028, -0.03), (0.036, -0.022), (0.036, 0.018), (0.03, 0.03), (0, 0.03)], T, X, "CenterHousing", s_, "Iron", 32)
    tube([T + Z * 0.035, T + Z * 0.06 + Y * 0.02, T + Z * 0.07 + Y * 0.08, Vector((C - 0.02, 1.88, -1.17))], 0.004, "OilFeed", s_, "Steel", 10)
    tube([T - Z * 0.035, T - Z * 0.06, T - Z * 0.09 + Y * 0.01], 0.012, "OilDrain", s_, "Rubber", 14)
    # Turbine: a cast scroll housing round the back plate, outlet, inlet flange.
    els = [("capsule", T - X * 0.035, T - X * 0.06, 0.05)]
    for k in range(n + 1):
        f = k / n
        a = math.pi * 0.6 + f * 2 * math.pi * 0.85
        rc = 0.05 + 0.018 * (1 - f)
        els.append(("ball", T - X * 0.048 + (Y * math.cos(a) + Z * math.sin(a)) * rc, 0.03 - 0.014 * f))
    els.append(("capsule", TURB_IN + Z * 0.004, T - X * 0.05 + (Y * math.cos(math.pi * 0.6) + Z * math.sin(math.pi * 0.6)) * 0.068, 0.028))
    meta(els, "TurbHousing", s_, "Exhaust", 0.003, 7000)
    revolve([(0.03, 0.0), (0.046, 0.0), (0.046, -0.022), (0.052, -0.026), (0.052, -0.036), (0.034, -0.036)], T - X * 0.075, X, "TurbOutlet", s_, "Exhaust", 48)
    cylinder(T - X * 0.1, X, 0.031, 0.02, "OutletBore", s_, "DarkSteel", 32)
    box(TURB_IN - Z * 0.004, (X, Y, Z), (0.036, 0.03, 0.006), "TurbInletFlange", s_, "Exhaust", 0.004)
    for dx in (-0.026, 0.026):
        bolt(TURB_IN - Z * 0.01 + X * dx, -Z, s_, 0.005)
    act = T - X * 0.035 + Z * 0.075 + Y * 0.035
    box(act, (X, Y, Z), (0.03, 0.028, 0.02), "VGTActuator", s_, "Plastic", 0.01)
    box(act + Y * 0.035 + Z * 0.004, (X, Y, Z), (0.01, 0.009, 0.007), "VGTPlug", s_, "Plastic", 0.003, 2)
    for dx in (-0.02, 0.02):
        bolt(act + X * dx + Z * 0.02 - Y * 0.02, Z, s_, 0.0045)
    box(T - Z * 0.07, (X, Y, Z), (0.045, 0.04, 0.03), "Pedestal", s_, "Iron", 0.014)
    for dx in (-0.03, 0.03):
        bolt(T - Z * 0.07 + X * dx + Y * 0.04, Y, s_, 0.006)


def inlet():
    """The turbo's air inlet: a rubber elbow up from the compressor and the MAF
    housing it leads to."""
    s_ = "v8_carb_e"
    path = [COMP_IN + X * 0.005, COMP_IN + X * 0.04, COMP_IN + X * 0.065 + Z * 0.03, Vector((COMP_IN.x + 0.07, 1.76, -1.13)), Vector((COMP_IN.x + 0.065, 1.82, -1.095)), Vector((COMP_IN.x + 0.06, 1.90, -1.085))]
    tube(path, 0.045, "InletElbow", s_, "Rubber", 32, 3, radii=[0.05, 0.05, 0.05, 0.05, 0.052, 0.052])
    for k, p in ((0, path[0] + X * 0.012), (1, path[-1] - Y * 0.012)):
        clamp(p, X if k == 0 else Y, 0.052, s_, 0.01)
    # Two moulded ridges on the elbow.
    bends = smooth_path(path, 3)
    for k in (len(bends) // 3, 2 * len(bends) // 3):
        t = (bends[k + 1] - bends[k - 1]).normalized()
        revolve([(0.05, -0.005), (0.055, -0.002), (0.055, 0.002), (0.05, 0.005)], bends[k], t, "Ridge", s_, "Rubber", 32)
    maf = Vector((COMP_IN.x + 0.06, 1.95, -1.085))
    cylinder(maf, Y, 0.054, 0.07, "MAFHousing", s_, "Plastic", 40, bev=0.006)
    box(maf + Z * 0.06, (X, Y, Z), (0.018, 0.022, 0.012), "MAFSensor", s_, "Plastic", 0.004)
    box(maf + Z * 0.062 + X * 0.024, (X, Y, Z), (0.009, 0.008, 0.007), "MAFPlug", s_, "Plastic", 0.002, 2)
    for dy in (-0.012, 0.012):
        bolt(maf + Z * 0.073 + Y * dy, Z, s_, 0.0035)


AIRF = Vector((COMP_IN.x + 0.06, 2.10, -1.085))


def air_filter():
    s_ = "v8_airbox_e"
    # A pleated cone filter on the MAF housing, rubber base, end cap.
    pleats = 40
    prof = [(0, -0.12), (0.06, -0.12), (0.062, -0.115), (0.062, -0.10), (0.058, -0.095), (0.056, -0.09), (0.05, 0.03), (0.048, 0.035), (0, 0.035)]
    revolve(prof, AIRF + Y * 0.08, Y, "FilterBody", s_, "FilterMedia", pleats * 4,
            rmod=lambda a, i: 1.0 + (0.09 * abs(math.cos(a * pleats / 2)) if 5 <= i <= 6 else 0.0), sharp=50)
    revolve([(0.054, -0.012), (0.062, -0.012), (0.064, -0.006), (0.064, 0.006), (0.062, 0.012), (0.054, 0.012)], AIRF - Y * 0.035, Y, "FilterBase", s_, "Rubber", 48)
    clamp(AIRF - Y * 0.036, Y, 0.064, s_, 0.01)
    revolve([(0, 0.0), (0.052, 0.0), (0.054, 0.006), (0.05, 0.012), (0.025, 0.016), (0, 0.016)], AIRF + Y * 0.115, Y, "FilterCap", s_, "DarkSteel", 48)


# =============================================================================
# Fuel system: rails, injector lines, glow plug harness; CP3 pump
# =============================================================================
CP3 = Vector((0.40, 2.27, -1.575))


def fuel_lines():
    s_ = "v8_wires_e"
    for side in (-1, 1):
        out, across = bank_axes(side)
        hc = ORIGIN + out * 0.245
        vc = ORIGIN + out * 0.33
        rail = hc + across * 0.142 + out * 0.03
        tube([rail + Y * (-L / 2 + 0.04), rail + Y * (L / 2 - 0.02)], 0.011, "FuelRail", s_, "Steel", 20, 0)
        for e in (-L / 2 + 0.04, L / 2 - 0.02):
            cylinder(rail + Y * e, Y, 0.012, 0.012, "RailPlug", s_, "Steel", 6)
        cylinder(rail + Y * (-L / 2 + 0.14) - across * 0.012, across, 0.009, 0.03, "PressureSensor", s_, "Plastic", 16, bev=0.002)
        for i in range(4):
            y = cyl_y(i) - MID
            a = rail + Y * y
            b = vc + across * 0.112 - out * 0.018 + Y * (y - 0.02)
            mid = a.lerp(b, 0.5) + out * 0.03 + across * 0.02
            tube([a + out * 0.008, a + out * 0.03, mid, b + across * 0.012, b], 0.0042, "InjLine", s_, "Steel", 10, 2)
            cylinder(a + out * 0.012, out, 0.0075, 0.012, "LineNut", s_, "Steel", 6)
            cylinder(b + across * 0.006, across, 0.0075, 0.012, "LineNut", s_, "Steel", 6)
        # Glow plug harness: a loom along the head with a lead to each plug.
        loom = [hc + across * 0.16 + out * 0.01 + Y * (-L / 2 + 0.02), hc + across * 0.165 + out * 0.012 + Y * (L / 2 - 0.02)]
        tube(loom, 0.0065, "GlowLoom", s_, "Rubber", 12, 0)
        for i in range(4):
            g = hc + across * 0.128 + Y * (cyl_y(i) - MID + 0.05) + out * 0.03
            p = hc + across * 0.163 + out * 0.011 + Y * (cyl_y(i) - MID + 0.05)
            tube([p, p.lerp(g + across * 0.03, 0.5) + out * 0.01, g + across * 0.03], 0.0028, "GlowLead", s_, "Rubber", 8, 1)
            cylinder(g + across * 0.03, across, 0.005, 0.012, "GlowBoot", s_, "Rubber", 12)
    # Feed lines from the CP3 to both rails' front ends.
    outD, acD = bank_axes(-1)
    outP, acP = bank_axes(1)
    railD = ORIGIN + outD * 0.245 + acD * 0.142 + outD * 0.03 + Y * (L / 2 - 0.02)
    railP = ORIGIN + outP * 0.245 + acP * 0.142 + outP * 0.03 + Y * (L / 2 - 0.02)
    src = CP3 + Z * 0.05
    tube([src, src + Z * 0.03, railP + Y * 0.05 + Z * 0.02, railP + Y * 0.012], 0.0045, "FeedLine", s_, "Steel", 10, 2)
    tube([src - X * 0.02, src - X * 0.02 + Z * 0.04, Vector((C + 0.1, FRONT + 0.03, -1.40)), Vector((C - 0.08, FRONT + 0.035, -1.40)), railD + Y * 0.06 + Z * 0.02, railD + Y * 0.012], 0.0045, "FeedLine", s_, "Steel", 10, 2)


def cp3():
    s_ = "v8_fuelpump_e"
    P = CP3
    meta([("box", P, (X, Y, Z), (0.035, 0.032, 0.042), 0.014), ("capsule", P + Z * 0.035 - X * 0.02, P + Z * 0.035 + X * 0.02, 0.022)], "CP3Body", s_, "Alu", 0.003, 3000)
    revolve([(0, -0.012), (0.042, -0.012), (0.045, -0.008), (0.045, 0.004), (0.036, 0.008), (0, 0.008)], P + Y * 0.04, Y, "CP3Flange", s_, "Alu", 40)
    for k in range(3):
        a = k / 3 * 2 * math.pi + 0.5
        bolt(P + Y * 0.05 + (X * math.cos(a) + Z * math.sin(a)) * 0.036, Y, s_, 0.005)
    cylinder(P + Z * 0.06 + X * 0.012, Z, 0.008, 0.02, "HPOutlet", s_, "Steel", 6)
    cylinder(P + Z * 0.06 - X * 0.02, Z, 0.008, 0.02, "HPOutlet", s_, "Steel", 6)
    reg = P + X * 0.045 - Y * 0.01
    cylinder(reg, X, 0.012, 0.03, "Regulator", s_, "Plastic", 24, bev=0.003)
    box(reg + X * 0.02 + Z * 0.008, (X, Y, Z), (0.008, 0.009, 0.006), "RegPlug", s_, "Plastic", 0.002, 2)
    tube([P - X * 0.035 + Z * 0.01, P - X * 0.06 + Z * 0.01, P - X * 0.07 - Z * 0.04 - Y * 0.02], 0.006, "SupplyHose", s_, "Rubber", 12)


# =============================================================================
# Exhaust manifolds and up-pipes
# =============================================================================
def manifold(side, s_):
    out, across = bank_axes(side)
    hc = ORIGIN + out * 0.245
    log = hc + across * 0.178 - out * 0.005
    els = [("capsule", log + Y * (L / 2 - 0.05), log - Y * (L / 2 - 0.04), 0.026)]
    for i in range(4):
        y = cyl_y(i) - MID
        els.append(("capsule", hc + across * 0.14 + Y * y, log + Y * (y - 0.02 * side), 0.022))
        els.append(("box", hc + across * 0.143 + Y * y - out * 0.005, (across, Y, out), (0.007, 0.036, 0.03), 0.006))
    meta(els, "Manifold", s_, "Exhaust", 0.004, 8000)
    for i in range(4):
        y = cyl_y(i) - MID
        for dy in (-0.026, 0.026):
            bolt(hc + across * 0.152 + Y * (y + dy) + out * 0.022, across, s_, 0.0055)
    # Up-pipe from the manifold's back end round to the turbine inlet.
    start = log - Y * (L / 2 - 0.04)
    p1 = start - Y * 0.04 + out * 0.01
    p2 = Vector((start.x * 0.6 + C * 0.4, REAR - 0.04, -1.36))
    p3 = Vector((TURB_IN.x + side * 0.06, REAR - 0.02, TURB_IN.z - 0.08))
    p4 = TURB_IN - Z * 0.05 + X * side * 0.02
    tube([start, p1, p2, p3, p4, TURB_IN - Z * 0.012], 0.023, "UpPipe", s_, "Exhaust", 20, 2)
    # A braided flex section on each up-pipe.
    pts = smooth_path([start, p1, p2, p3, p4, TURB_IN - Z * 0.012], 3)
    run = [0.0]
    for a, b in zip(pts, pts[1:]):
        run.append(run[-1] + (b - a).length)
    mid = run[-1] * 0.45
    k = 0
    for d in [mid - 0.035 + j * 0.007 for j in range(11)]:
        while k < len(run) - 2 and run[k + 1] < d:
            k += 1
        f = (d - run[k]) / max(1e-6, run[k + 1] - run[k])
        p = pts[k].lerp(pts[k + 1], f)
        t = (pts[k + 1] - pts[k]).normalized()
        revolve([(0.022, -0.0035), (0.0265, -0.0012), (0.0265, 0.0012), (0.022, 0.0035)], p, t, "Flex", s_, "Steel", 24)
    for d in (mid - 0.042, mid + 0.042):
        k = max(0, min(len(run) - 2, next((j for j in range(len(run) - 1) if run[j + 1] >= d), len(run) - 2)))
        t = (pts[k + 1] - pts[k]).normalized()
        revolve([(0.022, -0.004), (0.027, -0.004), (0.027, 0.004), (0.022, 0.004)], pts[k], t, "FlexCollar", s_, "Steel", 24)
    revolve([(0.022, -0.006), (0.034, -0.006), (0.034, 0.006), (0.022, 0.006)], start - Y * 0.004, Y, "PipeFlange", s_, "Exhaust", 24)


# =============================================================================
# Front of the engine
# =============================================================================
def timing_cover():
    s_ = "v8_timingcover_e"
    outline = [(-0.13, -1.75), (-0.06, -1.77), (0.06, -1.77), (0.13, -1.75), (0.155, -1.68), (0.15, -1.60), (0.125, -1.42), (0.07, -1.375),
               (-0.07, -1.375), (-0.125, -1.42), (-0.15, -1.60), (-0.155, -1.68)]
    extrude([(x, z) for x, z in outline], (C, FRONT + 0.022, 0), X, Z, Y, 0.032, "TimingCover", s_, "Alu", 0.012, 4)
    # Raised center: the crank seal boss and the pump drive bulge.
    cylinder(Vector((C, FRONT + 0.04, CRANK_Z)), Y, 0.06, 0.016, "SealBoss", s_, "Alu", 40, bev=0.006)
    cylinder(Vector((C, FRONT + 0.05, CRANK_Z)), Y, 0.042, 0.008, "CrankSeal", s_, "Rubber", 40)
    meta([("capsule", (C - 0.08, FRONT + 0.035, -1.48), (C + 0.08, FRONT + 0.035, -1.48), 0.03)], "CoverBulge", s_, "Alu", 0.004, 2500)
    for a in range(16):
        t = a / 16 * 2 * math.pi
        p = Vector((C + 0.142 * math.cos(t), FRONT + 0.04, -1.57 + 0.18 * math.sin(t)))
        cylinder(p, Y, 0.009, 0.01, "BoltBoss", s_, "Alu", 16)
        bolt(p + Y * 0.005, Y, s_, 0.0055)
    cylinder(Vector((C - 0.09, FRONT + 0.045, -1.70)), Y, 0.012, 0.02, "CrankSensor", s_, "Plastic", 20, bev=0.003)


def damper():
    s_ = "v8_crankpulley_e"
    cp = Vector((C, BELT_Y - 0.03, CRANK_Z))
    revolve([(0, -0.045), (0.05, -0.045), (0.05, -0.03), (0.088, -0.03), (0.098, -0.026), (0.1, -0.012), (0.098, 0.0), (0.05, 0.002), (0.048, 0.01), (0, 0.01)], cp, Y, "Damper", s_, "Iron", 48)
    revolve([(0.099, -0.028), (0.101, -0.026), (0.101, -0.012), (0.099, -0.01)], cp, Y, "DamperRubber", s_, "Rubber", 48)
    pulley(cp + Y * 0.03, 0.094, s_, "BeltPulley", hub=False)
    cylinder(cp + Y * 0.052, Y, 0.032, 0.012, "DamperHub", s_, "Steel", 32, bev=0.003)
    bolt(cp + Y * 0.058, Y, s_, 0.015)
    for k in range(4):
        a = k / 4 * 2 * math.pi + 0.4
        cylinder(cp + Y * 0.012 + (X * math.cos(a) + Z * math.sin(a)) * 0.07, Y, 0.007, 0.006, "BalanceHole", s_, "DarkSteel", 12)


WP = Vector((C, BELT_Y, -1.43))
FAN = WP + Y * 0.08


def water_pump():
    s_ = "v8_waterpump_e"
    els = [("ball", WP - Y * 0.07, 0.07), ("capsule", WP - Y * 0.08, WP - Y * 0.08 + X * 0.14 - Z * 0.02, 0.03),
           ("capsule", WP - Y * 0.08, WP - Y * 0.08 - X * 0.14 - Z * 0.02, 0.03), ("capsule", WP - Y * 0.08, WP - Y * 0.03, 0.04)]
    meta(els, "PumpHousing", s_, "Alu", 0.004, 5000)
    for sx in (-1, 1):
        for dz in (-0.02, 0.025):
            bolt(WP - Y * 0.05 + X * sx * 0.12 + Z * dz, Y, s_, 0.006)
    revolve([(0, -0.03), (0.025, -0.03), (0.022, 0.0), (0, 0.0)], WP, Y, "PumpShaft", s_, "Steel", 24)
    pulley(WP, 0.07, s_, "PumpPulley", ribs=0)
    # The pump's inlet for the lower hose, to the passenger's side.
    tube([WP - Y * 0.07 + X * 0.06 - Z * 0.07, WP - Y * 0.05 + X * 0.15 - Z * 0.08, WP - Y * 0.04 + X * 0.19 - Z * 0.09], 0.02, "PumpInlet", s_, "Alu", 24, 1)


def fan():
    s_ = "v8_fanwclutch_e"
    revolve([(0, -0.07), (0.04, -0.07), (0.058, -0.06), (0.066, -0.045), (0.066, -0.015), (0.058, 0.0), (0.03, 0.006), (0, 0.006)], FAN, Y, "FanClutch", s_, "Alu", 48)
    for k in range(18):
        t = k / 18 * 2 * math.pi
        d = X * math.cos(t) + Z * math.sin(t)
        tg = X * -math.sin(t) + Z * math.cos(t)
        box(FAN - Y * 0.055 + d * 0.05, (d, Y, tg), (0.014, 0.012, 0.0018), "ClutchFin", s_, "Alu", 0.001, 1)
    cylinder(FAN + Y * 0.004, Y, 0.016, 0.006, "Bimetal", s_, "Steel", 24)
    revolve([(0.04, -0.004), (0.072, -0.004), (0.072, 0.004), (0.04, 0.004)], FAN - Y * 0.01, Y, "Spider", s_, "DarkSteel", 48)
    # Nine broad, curved, pitched blades with real thickness and round tips.
    blades = 9
    for k in range(blades):
        t0 = k / blades * 2 * math.pi
        bm = bmesh.new()
        rows = []
        J = 10
        for j in range(J + 1):
            f = j / J
            rr = 0.07 + f * 0.135
            th = t0 + 0.12 * f
            tip = math.sqrt(max(0.0, 1 - ((f - 0.8) / 0.2) ** 2)) if f > 0.8 else 1.0
            chord = (0.05 + 0.03 * f) * max(0.15, tip)
            pitch = math.radians(30 - 8 * f)
            thick = 0.0028
            rad = X * math.cos(th) + Z * math.sin(th)
            tang = X * -math.sin(th) + Z * math.cos(th)
            cv = tang * math.cos(pitch) + Y * math.sin(pitch)
            nv = -tang * math.sin(pitch) + Y * math.cos(pitch)
            row = []
            m = 7
            for side_ in (1, -1):
                for q in (range(m) if side_ > 0 else range(m - 1, -1, -1)):
                    u = -0.5 + q / (m - 1)
                    camber = 0.006 * (1 - 4 * u * u)
                    row.append(FAN - Y * 0.012 + rad * rr + cv * chord * u + nv * (camber + side_ * thick * math.sqrt(max(0.0, 1 - 4 * u * u)) + 0.0006 * side_))
            rows.append([bm.verts.new(p) for p in row])
        cnt = len(rows[0])
        for j in range(len(rows) - 1):
            for q in range(cnt):
                qq = (q + 1) % cnt
                bm.faces.new((rows[j][q], rows[j][qq], rows[j + 1][qq], rows[j + 1][q]))
        bm.faces.new(list(reversed(rows[0])))
        bm.faces.new(rows[-1])
        bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=1e-5)
        new_object("FanBlade", s_, bm, "Plastic", 60)
    revolve([(0.05, -0.016), (0.074, -0.016), (0.074, -0.004), (0.05, -0.004)], FAN, Y, "BladeHub", s_, "Plastic", 48)
    for k in range(4):
        a = k / 4 * 2 * math.pi + 0.3
        bolt(FAN - Y * 0.004 + (X * math.cos(a) + Z * math.sin(a)) * 0.05, Y, s_, 0.005)


ALT = Vector((0.37, BELT_Y, -1.29))


def alternator():
    s_ = "v8_alternator_e"
    A = ALT
    # Front and rear housings with cooling slots, stator band between.
    revolve([(0.02, -0.025), (0.06, -0.025), (0.07, -0.032), (0.072, -0.06)], A, Y, "FrontHousing", s_, "Alu", 48,
            rmod=lambda a, i: 1.0 - (0.06 if (i == 1 and math.cos(a * 10) > 0.3) else 0.0))
    revolve([(0.075, -0.058), (0.077, -0.064), (0.077, -0.096), (0.075, -0.102)], A, Y, "Stator", s_, "DarkSteel", 48,
            rmod=lambda a, i: 1.0 + (0.012 if math.cos(a * 36) > 0 else 0.0))
    revolve([(0.072, -0.1), (0.07, -0.13), (0.06, -0.15), (0.03, -0.158), (0, -0.158)], A, Y, "RearHousing", s_, "Alu", 48)
    for k in range(10):
        a = k / 10 * 2 * math.pi
        d = X * math.cos(a) + Z * math.sin(a)
        box(A - Y * 0.035 + d * 0.069, (d, Y, X * -math.sin(a) + Z * math.cos(a)), (0.004, 0.009, 0.006), "Slot", s_, "DarkSteel", 0.0015, 1)
    cylinder(A - Y * 0.162 + X * 0.03, Y, 0.007, 0.012, "Terminal", s_, "Copper", 6)
    box(A - Y * 0.155 - X * 0.035 + Z * 0.02, (X, Y, Z), (0.014, 0.008, 0.01), "Plug", s_, "Plastic", 0.003, 2)
    for sx in (-1, 1):
        box(A - Y * 0.08 + X * sx * 0.082 - Z * 0.03, (X, Y, Z), (0.012, 0.06, 0.012), "Ear", s_, "Alu", 0.006)
    box(A - Y * 0.08 - X * 0.1 - Z * 0.05, (X, Y, Z), (0.03, 0.05, 0.008), "Bracket", s_, "Iron", 0.004)


def alt_fan():
    s_ = "altfan"
    A = ALT
    pulley(A + Y * 0.004, 0.032, s_, "AltPulley")
    revolve([(0.015, -0.012), (0.066, -0.012), (0.066, -0.008), (0.015, -0.008)], A, Y, "FanDisc", s_, "DarkSteel", 40)
    for k in range(12):
        a = k / 12 * 2 * math.pi
        d = X * math.cos(a) + Z * math.sin(a)
        tg = X * -math.sin(a) + Z * math.cos(a)
        box(A - Y * 0.017 + d * 0.045 + tg * 0.004, (d, Y, tg), (0.02, 0.008, 0.0012), "FanVane", s_, "DarkSteel", 0.0006, 1)
    cylinder(A + Y * 0.022, Y, 0.009, 0.008, "PulleyNut", s_, "Steel", 6)


def belt():
    s_ = "v8_fanbelt_e"
    AC = (C - 0.19, -1.58)
    PS = (C - 0.17, -1.36)
    IDLER = (C + 0.13, -1.53)
    pulleys = [((C, CRANK_Z), 0.094), (AC, 0.058), (PS, 0.05), ((WP.x, WP.z), 0.07), ((ALT.x, ALT.z), 0.032), (IDLER, 0.035)]
    pts2 = []
    for (cx, cz), r in pulleys:
        for k in range(72):
            t = k / 72 * 2 * math.pi
            pts2.append((cx + (r + 0.0035) * math.cos(t), cz + (r + 0.0035) * math.sin(t)))

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

    # Resample the loop evenly so the ribs follow it smoothly.
    loop = [Vector((x, 0, z)) for x, z in convex(pts2)]
    total = sum((loop[(i + 1) % len(loop)] - loop[i]).length for i in range(len(loop)))
    step = 0.006
    even = []
    i, acc, target = 0, 0.0, 0.0
    while target < total and i < len(loop) * 2:
        a = loop[i % len(loop)]
        b = loop[(i + 1) % len(loop)]
        seg = (b - a).length
        if acc + seg >= target:
            even.append(a.lerp(b, (target - acc) / seg if seg else 0))
            target += step
        else:
            acc += seg
            i += 1
    # Cross-section: flat back outside, six V ribs inside.
    prof = [(0.0025, -0.011), (0.0025, 0.011)]
    for k in range(13):
        y = 0.011 - k * 0.022 / 12
        prof.append((-0.0015 if k % 2 == 0 else -0.0035, y))
    bm = bmesh.new()
    rings = []
    n_ = len(even)
    for i in range(n_):
        t = (even[(i + 1) % n_] - even[i - 1]).normalized()
        nrm = Vector((t.z, 0, -t.x))
        rings.append([bm.verts.new(Vector((even[i].x, BELT_Y, even[i].z)) + nrm * dn + Y * dy) for dn, dy in prof])
    m = len(prof)
    for i in range(n_):
        j = (i + 1) % n_
        for k in range(m):
            kk = (k + 1) % m
            bm.faces.new((rings[i][k], rings[i][kk], rings[j][kk], rings[j][k]))
    new_object("Belt", s_, bm, "Rubber", 50)


def thermostat():
    s_ = "v8_hosethermostat_e"
    TH = Vector((C - 0.07, FRONT - 0.02, -1.285))
    meta([("ball", TH, 0.03), ("capsule", TH, TH + Y * 0.05 + Z * 0.02, 0.022), ("box", TH - Z * 0.02, (X, Y, Z), (0.045, 0.03, 0.008), 0.006)], "ThermoHousing", s_, "Alu", 0.003, 3000)
    for dx in (-0.035, 0.035):
        bolt(TH - Z * 0.012 + X * dx, Z, s_, 0.005)
    cylinder(TH + Z * 0.02 - X * 0.03, X, 0.006, 0.016, "TempSender", s_, "Plastic", 16, bev=0.002)
    path = [TH + Y * 0.045 + Z * 0.018, TH + Y * 0.09 + Z * 0.04, TH + Y * 0.16 + Z * 0.075 - X * 0.01, TH + Y * 0.24 + Z * 0.09 - X * 0.02, TH + Y * 0.30 + Z * 0.09 - X * 0.02]
    tube(path, 0.024, "UpperHose", s_, "Rubber", 28, 3, radii=[0.023, 0.024, 0.024, 0.024, 0.026])
    clamp(path[0] + Y * 0.012 + Z * 0.006, Y + Z * 0.4, 0.025, s_)
    clamp(path[-1] - Y * 0.015, Y, 0.027, s_)


def lower_hose():
    s_ = "v8_hose_e"
    start = WP - Y * 0.04 + X * 0.19 - Z * 0.09
    path = [start, start + X * 0.03, Vector((0.47, 2.46, -1.56)), Vector((0.49, 2.54, -1.60)), Vector((0.50, 2.62, -1.615))]
    tube(path, 0.022, "LowerHose", s_, "Rubber", 28, 3, radii=[0.022, 0.023, 0.023, 0.023, 0.024])
    clamp(start + X * 0.015, X, 0.023, s_)
    clamp(path[-1] - Y * 0.015, Y, 0.025, s_)


# =============================================================================
# Underneath and the sides
# =============================================================================
def oil_filter():
    s_ = "v8_oilfilter_e"
    OF = Vector((-0.052, 1.743, -1.71))
    revolve([(0, 0.065), (0.03, 0.065), (0.043, 0.06), (0.047, 0.05), (0.047, -0.045), (0.049, -0.05), (0.049, -0.058), (0.045, -0.062), (0.03, -0.064), (0, -0.064)],
            OF, -Z, "OilFilter", s_, "Filter", 48, rmod=lambda a, i: 1.0 + (0.035 * max(0, math.cos(a * 16)) if i in (2, 3) else 0.0))
    revolve([(0.03, 0.063), (0.036, 0.063), (0.036, 0.07), (0.03, 0.07)], OF, -Z, "Gasket", s_, "Rubber", 32)


def starter():
    s_ = "v8_starter_e"
    ST = Vector((0.37, 1.735, -1.70))
    revolve([(0, 0.075), (0.035, 0.075), (0.048, 0.07), (0.05, 0.06), (0.05, -0.02), (0.046, -0.025), (0.046, -0.04), (0.04, -0.06), (0.03, -0.09), (0.02, -0.1), (0, -0.1)],
            ST, -Y, "Starter", s_, "DarkSteel", 40)
    sol = ST + Z * 0.062 + Y * 0.02
    revolve([(0, -0.045), (0.024, -0.045), (0.026, -0.04), (0.026, 0.035), (0.02, 0.045), (0, 0.045)], sol, Y, "Solenoid", s_, "Zinc", 32)
    for dx in (-0.01, 0.01):
        cylinder(sol - Y * 0.05 + X * dx, Y, 0.005, 0.014, "SolTerminal", s_, "Copper", 6)
    for k in range(2):
        bolt(ST - Y * 0.1 + X * (0.035 if k else -0.035), -Y, s_, 0.007)


def oil_pan():
    s_ = "v8_oilpan_e"
    # Side profile (y, z): shallow at the back, a deep sump at the front.
    side = [(REAR + 0.01, -1.705), (FRONT - 0.01, -1.705), (FRONT - 0.02, -1.83), (FRONT - 0.06, -1.862), (MID + 0.05, -1.862), (MID - 0.02, -1.79), (REAR + 0.03, -1.775), (REAR + 0.012, -1.74)]
    extrude([(y, z) for y, z in side], (C, 0, 0), Y, Z, X, 0.236, "OilPan", s_, "Iron", 0.016, 4)
    extrude([(REAR + 0.004, -1.698), (FRONT - 0.004, -1.698), (FRONT - 0.004, -1.71), (REAR + 0.004, -1.71)], (C, 0, 0), Y, Z, X, 0.27, "PanRail", s_, "Iron", 0.004, 2)
    for k in range(6):
        box(Vector((C, MID + 0.04 + k * 0.045, -1.866)), (X, Y, Z), (0.1, 0.004, 0.005), "PanRib", s_, "Iron", 0.003, 2)
    for k in range(12):
        sx = 1 if k % 2 else -1
        bolt(Vector((C + sx * 0.128, REAR + 0.05 + (k // 2) * 0.115, -1.714)), -Z, s_, 0.005)
    bolt(Vector((C + 0.05, FRONT - 0.08, -1.866)), -Z, s_, 0.011)
    cylinder(Vector((C - 0.118, MID + 0.12, -1.80)), -X, 0.012, 0.02, "LevelSensor", s_, "Plastic", 16, bev=0.003)


# =============================================================================
block()
for side, h, c, m in ((-1, "v8_headD_e", "v8_valvecoverD_e", "v8_exhaustmanD_e"), (1, "v8_headP_e", "v8_valvecoverP_e", "v8_exhaustmanP_e")):
    head(side, h)
    covers(side, c)
    manifold(side, m)
oil_cap()
intake()
turbo()
inlet()
air_filter()
fuel_lines()
cp3()
timing_cover()
damper()
water_pump()
fan()
alternator()
alt_fan()
belt()
thermostat()
lower_hose()
oil_filter()
starter()
oil_pan()
print("modeled", len(pieces), "pieces")

# =============================================================================
# Bake: each part becomes one object, unwrapped, with its own textures
# =============================================================================
from PIL import Image

SAMPLES = 8 if QUICK else 64
GRIME = (0.045, 0.040, 0.034)
RUST = (0.30, 0.12, 0.05)


def bake_material(surf):
    base, metal, rough, edge, wear, grime, bump = SURF[surf]
    m = bpy.data.materials.new("S_" + surf)
    m.use_nodes = True
    nt = m.node_tree
    N, Lk = nt.nodes, nt.links
    for n in list(N):
        N.remove(n)
    out = N.new("ShaderNodeOutputMaterial")
    tc = N.new("ShaderNodeTexCoord")
    geo = N.new("ShaderNodeNewGeometry")

    def link(v, sock):
        if isinstance(v, (int, float)):
            sock.default_value = v
        elif isinstance(v, tuple):
            sock.default_value = (*v, 1.0) if len(v) == 3 else v
        else:
            Lk.new(v, sock)

    def noise(scale, detail=4.0, r=0.55):
        n = N.new("ShaderNodeTexNoise")
        n.inputs["Scale"].default_value = scale
        n.inputs["Detail"].default_value = detail
        n.inputs["Roughness"].default_value = r
        Lk.new(tc.outputs["Object"], n.inputs["Vector"])
        return n.outputs["Fac"]

    def op(o, a, b=0.0, clamp=False):
        n = N.new("ShaderNodeMath")
        n.operation = o
        n.use_clamp = clamp
        link(a, n.inputs[0])
        link(b, n.inputs[1])
        return n.outputs[0]

    def remap(v, a, b, c=0.0, d=1.0):
        n = N.new("ShaderNodeMapRange")
        n.clamp = True
        link(v, n.inputs["Value"])
        n.inputs["From Min"].default_value = a
        n.inputs["From Max"].default_value = b
        n.inputs["To Min"].default_value = c
        n.inputs["To Max"].default_value = d
        return n.outputs["Result"]

    def mix(f, a, b, blend="MIX"):
        n = N.new("ShaderNodeMix")
        n.data_type = "RGBA"
        n.blend_type = blend
        n.clamp_result = True
        ins = [s for s in n.inputs if s.type == "RGBA"]
        link(f, n.inputs[0])
        link(a, ins[0])
        link(b, ins[1])
        return [s for s in n.outputs if s.type == "RGBA"][0]

    nL = noise(4.0, 3.0)
    nM = noise(22.0, 6.0)
    nF = noise(150.0, 8.0, 0.6)
    grain = noise(600.0, 2.0)
    ao = N.new("ShaderNodeAmbientOcclusion")
    ao.samples = 16
    ao.only_local = True
    ao.inputs["Distance"].default_value = 0.035
    aov = ao.outputs["AO"]
    bev = N.new("ShaderNodeBevel")
    bev.samples = 8
    bev.inputs["Radius"].default_value = 0.0035
    dot = N.new("ShaderNodeVectorMath")
    dot.operation = "DOT_PRODUCT"
    Lk.new(bev.outputs["Normal"], dot.inputs[0])
    Lk.new(geo.outputs["Normal"], dot.inputs[1])
    edge_ = remap(dot.outputs["Value"], 0.985, 0.90)
    sepz = N.new("ShaderNodeSeparateXYZ")
    Lk.new(geo.outputs["Position"], sepz.inputs[0])

    wear_ = op("MULTIPLY", op("MULTIPLY", edge_, remap(nM, 0.40, 0.62)), wear, True)
    cav = remap(aov, 0.98, 0.55)
    low = op("MULTIPLY", remap(sepz.outputs["Z"], -1.45, -1.86), nL)
    film = op("ADD", 0.12, op("MULTIPLY", remap(nL, 0.5, 0.75), 0.35))
    dirt = op("MULTIPLY", op("ADD", op("ADD", cav, op("MULTIPLY", low, 0.6)), film), remap(nM, 0.25, 0.75, 0.55, 1.0), True)
    grime_ = op("MULTIPLY", dirt, grime, True)

    col = mix(remap(nL, 0.3, 0.7), tuple(c * 0.84 for c in base), tuple(min(1.0, c * 1.16) for c in base))
    if surf == "Exhaust":
        col = mix(op("MULTIPLY", remap(nM, 0.45, 0.68), 0.75), col, RUST)
    col = mix(remap(nF, 0.35, 0.65, 0.0, 0.25), col, tuple(c * 0.75 for c in base))
    col = mix(wear_, col, edge)
    col = mix(op("MULTIPLY", grime_, 0.9), col, GRIME)
    shade = op("ADD", op("MULTIPLY", aov, 0.65), 0.35)
    sh = N.new("ShaderNodeCombineColor")
    for k in ("Red", "Green", "Blue"):
        Lk.new(shade, sh.inputs[k])
    albedo = mix(1.0, col, sh.outputs[0], "MULTIPLY")

    metal_ = op("ADD", op("MULTIPLY", op("SUBTRACT", 1.0, op("MULTIPLY", grime_, 0.7)), metal), op("MULTIPLY", wear_, 0.25), True)
    smooth_ = op("ADD", op("MULTIPLY", op("MULTIPLY", op("SUBTRACT", 1.0, op("MULTIPLY", grime_, 0.6)), 1.0 - rough), remap(nF, 0.3, 0.7, 0.85, 1.12)), op("MULTIPLY", wear_, 0.12), True)
    ms = N.new("ShaderNodeCombineColor")
    Lk.new(metal_, ms.inputs["Red"])
    Lk.new(smooth_, ms.inputs["Green"])

    height = op("ADD", grain, op("MULTIPLY", nF, 0.5))
    bump_ = N.new("ShaderNodeBump")
    bump_.inputs["Strength"].default_value = min(1.0, 0.22 * bump)
    bump_.inputs["Distance"].default_value = 0.0004
    Lk.new(height, bump_.inputs["Height"])
    bev2 = N.new("ShaderNodeBevel")
    bev2.samples = 8
    bev2.inputs["Radius"].default_value = 0.0025
    Lk.new(bump_.outputs["Normal"], bev2.inputs["Normal"])
    pr = N.new("ShaderNodeBsdfPrincipled")
    Lk.new(bev2.outputs["Normal"], pr.inputs["Normal"])

    em_alb = N.new("ShaderNodeEmission")
    Lk.new(albedo, em_alb.inputs["Color"])
    em_ms = N.new("ShaderNodeEmission")
    Lk.new(ms.outputs[0], em_ms.inputs["Color"])
    target = N.new("ShaderNodeTexImage")
    target.name = "BakeTarget"
    N.active = target
    m["passes"] = True
    m["_alb"] = em_alb.name
    m["_ms"] = em_ms.name
    m["_pr"] = pr.name
    return m


MATS = {s: bake_material(s) for s in SURF}


def use_pass(mat, which):
    nt = mat.node_tree
    out = [n for n in nt.nodes if n.type == "OUTPUT_MATERIAL"][0]
    for l in list(out.inputs["Surface"].links):
        nt.links.remove(l)
    node = nt.nodes[mat["_" + which]]
    nt.links.new(node.outputs[0], out.inputs["Surface"])


scene.render.engine = "CYCLES"
scene.cycles.device = "CPU"
scene.cycles.samples = SAMPLES
scene.render.bake.margin = 6
scene.view_settings.view_transform = "Standard"

parts = {}
stats = {}
by_slot = {}
for o in pieces:
    by_slot.setdefault(o["slot"], []).append(o)
for slot, (_, size) in SLOTS.items():
    objs = by_slot.get(slot, [])
    if not objs:
        continue
    if QUICK:
        size = max(128, size // 4)
    for o in objs:
        me = o.data
        me.materials.clear()
        me.materials.append(MATS[o["surf"]])
        me.shade_smooth()
        if o["sharp"] < 180:
            me.set_sharp_from_angle(angle=math.radians(o["sharp"]))
    surfs = {}
    for o in objs:
        surfs[o["surf"]] = surfs.get(o["surf"], 0) + 1
    bpy.ops.object.select_all(action="DESELECT")
    for o in objs:
        o.select_set(True)
    bpy.context.view_layer.objects.active = objs[0]
    if len(objs) > 1:
        bpy.ops.object.join()
    J = bpy.context.view_layer.objects.active
    J.name = slot
    J["surfs"] = ",".join(surfs)
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.uv.smart_project(angle_limit=math.radians(55), island_margin=max(0.003, 4.0 / size), area_weight=0.0, scale_to_bounds=False)
    bpy.ops.object.mode_set(mode="OBJECT")
    imgs = {}
    for kind, data in (("alb", False), ("ms", True), ("nrm", True)):
        img = bpy.data.images.new("%s_%s" % (slot, kind), size, size, alpha=False)
        if data:
            img.colorspace_settings.name = "Non-Color"
        imgs[kind] = img
    for kind in ("alb", "ms", "nrm"):
        for mat in J.data.materials:
            node = mat.node_tree.nodes["BakeTarget"]
            node.image = imgs[kind]
            mat.node_tree.nodes.active = node
            use_pass(mat, "pr" if kind == "nrm" else kind)
        if kind == "nrm":
            bpy.ops.object.bake(type="NORMAL", normal_space="TANGENT", margin=6)
        else:
            bpy.ops.object.bake(type="EMIT", margin=6)
        path = os.path.join(OUT, "duramax_%s_%s.png" % (slot, kind))
        imgs[kind].filepath_raw = path
        imgs[kind].file_format = "PNG"
        imgs[kind].save()
    # Final files: color as JPEG, metal (R) + smoothness (A) as one PNG.
    alb = os.path.join(OUT, "duramax_%s_alb.png" % slot)
    Image.open(alb).convert("RGB").save(os.path.join(OUT, "duramax_%s.jpg" % slot), quality=92)
    os.remove(alb)
    msp = os.path.join(OUT, "duramax_%s_ms.png" % slot)
    msi = Image.open(msp).convert("RGB")
    r, g, _ = msi.split()
    Image.merge("RGBA", (r, r, r, g)).save(os.path.join(OUT, "duramax_%s_m.png" % slot))
    os.remove(msp)
    os.replace(os.path.join(OUT, "duramax_%s_nrm.png" % slot), os.path.join(OUT, "duramax_%s_n.png" % slot))
    parts[slot] = J
    me = J.data
    me.calc_loop_triangles()
    stats[slot] = len(me.loop_triangles)
    print("baked", slot, size, stats[slot], "tris", flush=True)

# =============================================================================
# Export
# =============================================================================
# Each slot's frame: the game's (Unity) transform of that part. The import made
# Blender objects with x, y, z = Unity x, z, y; positions go out in Unity terms,
# with x mirrored like the mods' other OBJ files (their parser mirrors it back).
SWAP = Matrix(((1, 0, 0, 0), (0, 0, 1, 0), (0, 1, 0, 0), (0, 0, 0, 1)))
lines = ["# 6.6L Duramax for Engine Cloner: one group per V8 part it re-skins, in that part's frame.",
         "mtllib duramax.mtl"]
mtl = ["# Materials of duramax.obj: one per part, with its baked textures (Standard shader in game).", ""]
vb = tb = nb = 1
for slot, J in parts.items():
    U = SWAP @ frames[slot] @ SWAP
    Ui = U.inverted()
    Nm = Ui.to_3x3().inverted().transposed()
    me = J.data
    me.calc_loop_triangles()
    normals = me.corner_normals
    lines.append("o " + slot)
    for v in me.vertices:
        p = Ui @ (SWAP @ v.co.to_4d())
        lines.append("v %.5f %.5f %.5f" % (-p.x, p.y, p.z))
    uvl = me.uv_layers.active
    n_index, n_lines, t_index, t_lines, faces = {}, [], {}, [], []
    for tri in me.loop_triangles:
        corner = []
        for li in tri.loops:
            vi = me.loops[li].vertex_index
            nu = (Nm @ (SWAP.to_3x3() @ normals[li].vector)).normalized()
            key = (round(-nu.x, 3), round(nu.y, 3), round(nu.z, 3))
            if key not in n_index:
                n_index[key] = len(n_lines)
                n_lines.append("vn %.3f %.3f %.3f" % key)
            uvk = tuple(round(c, 5) for c in uvl.data[li].uv)
            if uvk not in t_index:
                t_index[uvk] = len(t_lines)
                t_lines.append("vt %.5f %.5f" % uvk)
            corner.append("%d/%d/%d" % (vb + vi, tb + t_index[uvk], nb + n_index[key]))
        # The mirrored x flips the winding: the parser flips it back.
        faces.append("f " + " ".join(corner))
    lines += t_lines + n_lines
    lines.append("usemtl D_" + slot)
    lines += faces
    vb += len(me.vertices)
    tb += len(t_lines)
    nb += len(n_lines)
    surfs = J["surfs"].split(",")
    metal = sum(SURF[s][1] for s in surfs) / len(surfs)
    rough = sum(SURF[s][2] for s in surfs) / len(surfs)
    mtl += ["newmtl D_" + slot, "Kd 1.0000 1.0000 1.0000", "d 1.000", "Pm %.3f" % metal, "Pr %.3f" % rough,
            "map_Kd duramax_%s.jpg" % slot, "map_Bump duramax_%s_n.png" % slot, "map_Pm duramax_%s_m.png" % slot, ""]
open(os.path.join(OUT, "duramax.obj"), "w").write("\n".join(lines) + "\n")
open(os.path.join(OUT, "duramax.mtl"), "w").write("\n".join(mtl))
print("exported %d parts, %d triangles" % (len(parts), sum(stats.values())))
for s, n in stats.items():
    print("  %-22s %6d" % (s, n))

# =============================================================================
# Preview, with the baked textures
# =============================================================================
if RENDER:
    for slot, J in parts.items():
        m = bpy.data.materials.new("P_" + slot)
        m.use_nodes = True
        nt = m.node_tree
        b = nt.nodes["Principled BSDF"]
        ta = nt.nodes.new("ShaderNodeTexImage")
        ta.image = bpy.data.images.load(os.path.join(OUT, "duramax_%s.jpg" % slot))
        nt.links.new(ta.outputs["Color"], b.inputs["Base Color"])
        tm = nt.nodes.new("ShaderNodeTexImage")
        tm.image = bpy.data.images.load(os.path.join(OUT, "duramax_%s_m.png" % slot))
        tm.image.colorspace_settings.name = "Non-Color"
        sp = nt.nodes.new("ShaderNodeSeparateColor")
        nt.links.new(tm.outputs["Color"], sp.inputs[0])
        nt.links.new(sp.outputs["Red"], b.inputs["Metallic"])
        inv = nt.nodes.new("ShaderNodeMath")
        inv.operation = "SUBTRACT"
        inv.inputs[0].default_value = 1.0
        nt.links.new(tm.outputs["Alpha"], inv.inputs[1])
        nt.links.new(inv.outputs[0], b.inputs["Roughness"])
        tn = nt.nodes.new("ShaderNodeTexImage")
        tn.image = bpy.data.images.load(os.path.join(OUT, "duramax_%s_n.png" % slot))
        tn.image.colorspace_settings.name = "Non-Color"
        nm = nt.nodes.new("ShaderNodeNormalMap")
        nt.links.new(tn.outputs["Color"], nm.inputs["Color"])
        nt.links.new(nm.outputs["Normal"], b.inputs["Normal"])
        J.data.materials.clear()
        J.data.materials.append(m)
    sc = scene
    sc.cycles.samples = int(os.environ.get("SAMPLES", "48"))
    sc.render.resolution_x, sc.render.resolution_y = 1200, 800
    w = bpy.data.worlds.new("w")
    sc.world = w
    w.use_nodes = True
    w.node_tree.nodes["Background"].inputs[0].default_value = (0.55, 0.6, 0.66, 1)
    w.node_tree.nodes["Background"].inputs[1].default_value = 0.8
    sun = bpy.data.objects.new("sun", bpy.data.lights.new("sun", "SUN"))
    sun.data.energy = 3.5
    sun.rotation_euler = (math.radians(40), math.radians(15), math.radians(150))
    sc.collection.objects.link(sun)
    cam = bpy.data.objects.new("cam", bpy.data.cameras.new("cam"))
    sc.collection.objects.link(cam)
    sc.camera = cam
    target = Vector((C, MID, -1.45))
    views = os.environ.get("VIEWS", "front34,side,top,rear34,front").split(",")
    spots = {"front34": (C + 0.95, FRONT + 0.85, -0.95), "side": (C - 1.25, MID, -1.35), "top": (C + 0.35, MID + 0.45, -0.55),
             "rear34": (C - 0.9, REAR - 0.85, -0.85), "front": (C + 0.25, FRONT + 1.2, -1.30), "pside": (C + 1.2, MID + 0.2, -1.2),
             "turbo": (C - 0.35, REAR - 0.45, -0.95), "fan": (C + 0.45, FRONT + 0.6, -1.25), "under": (C + 0.8, MID + 0.3, -1.95)}
    aims = {"turbo": Vector((C, 1.72, -1.22)), "fan": Vector((C, BELT_Y, -1.45))}
    for name in views:
        loc = spots[name]
        cam.location = loc
        cam.rotation_euler = (aims.get(name, target) - Vector(loc)).to_track_quat("-Z", "Y").to_euler()
        sc.render.filepath = os.path.join(OUT, "preview_" + name + ".png")
        bpy.ops.render.render(write_still=True)
        print("rendered", name, flush=True)
