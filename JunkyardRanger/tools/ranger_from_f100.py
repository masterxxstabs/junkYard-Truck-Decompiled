"""Junkyard Ranger: a 2004 Ford Ranger body made from the game's own F100
(f1003), fitted to the Diamondback's chassis.

Steps:
  1. The F100's body panels (main body, hood, doors, tailgate, glass, fuel cap).
  2. The 1977 front end (grille, headlight bezels, bumper) comes off.
  3. Everything is remapped into the Diamondback's frame: 92% width, 87.5%
     height, and the length remapped section by section (front overhang, front
     arch, cab, bed, rear arch, tailgate) so both axles land on the Diamondback's
     and its cab, seats and dash fit inside.
  4. Ranger styling: the hood and fenders slope down to the nose and the hood
     gets its raised middle; the front corners round off in plan view, and a
     2001-04 front end is swept round that rounded outline: a chunky
     body-colored grille surround with a honeycomb insert and the oval,
     headlights under the hood's edge that wrap round the corners (the
     Diamondback's own headlight lens, two round lamps a side, and its corner
     lamp), and a big rounded bumper with the plate, a black intake and fog
     lights. The Diamondback's firewall and inner fenders go in the engine bay.
  5. The two-tone: the body, doors and tailgate cut at TWO_TONE_Z into lower
     groups that take the second color.
  6. Accessories the mod shows in slots, each in the frame of the part its slot
     hangs on: the bull bar (the Diamondback's brush guard slot), rain guards
     (the doors) and a sun visor (the cab).

Blender frame: X right, Y forward, Z up, the Diamondback root's origin.

Run: python3 ranger_from_f100.py f1003.blend dpt.blend <out_dir> [render[:views]]
"""
import sys, os, math
import bpy, bmesh
from mathutils import Vector

F100_BLEND, DPT_BLEND, OUT = (os.path.abspath(a) for a in sys.argv[1:4])
RENDER = sys.argv[4] if len(sys.argv) > 4 else ""
os.makedirs(OUT, exist_ok=True)

bpy.ops.wm.open_mainfile(filepath=F100_BLEND)
scene = bpy.context.scene

# ------------------------------------------------------------------ 1. F100 parts
KEEP = {
    "fmainbody": "Body",
    "fhood": "Hood",
    "f100_ddoor": "DoorD",
    "f100_ddoor/000-0-0": "DoorD",
    "f100_pdoor": "DoorP",
    "f100_pdoor/002-0-2": "DoorP",
    "ftailgate": "Tailgate",
    "016-0-16": "Glass",
    "FuelCapF": "GasDoor",
}
for o in list(scene.objects):
    if o.get("path", o.name) not in KEEP:
        bpy.data.objects.remove(o)
for o in scene.objects:
    o["group"] = KEEP[o["path"]]
    o.hide_render = False
# The F100's hood has only the rust and mud layers; give it the body's base
# layer too (same texture atlas), so it takes the paint like everything else.
hood_me = bpy.data.objects["fhood"].data
hood_me.materials[0] = bpy.data.materials["Tex_RGB_23"]
# The fuel cap alone is 11k vertices: a tenth is plenty.
cap_obj = bpy.data.objects["FuelCapF"]
dec = cap_obj.modifiers.new("dec", "DECIMATE")
dec.ratio = 0.1
dg = bpy.context.evaluated_depsgraph_get()
cap_obj.data = bpy.data.meshes.new_from_object(cap_obj.evaluated_get(dg))
cap_obj.modifiers.clear()

# ------------------------------------------------------------------ 2. 1977 front end off
def components(bm):
    comp = {}
    cid = 0
    for v in bm.verts:
        if v.index in comp:
            continue
        stack = [v]
        comp[v.index] = cid
        while stack:
            x = stack.pop()
            for e in x.link_edges:
                y = e.other_vert(x)
                if y.index not in comp:
                    comp[y.index] = cid
                    stack.append(y)
        cid += 1
    return comp, cid

PATCH = (0.630, 0.224, 0.698, 0.291)   # a clean, fully painted patch of the F100 atlas


def paint_pieces(obj, rule, what):
    """Map the loose pieces of obj for which rule(lo, hi) is true onto the painted
    patch of the atlas, so they're body-colored (F100 frame)."""
    bm = bmesh.new()
    bm.from_mesh(obj.data)
    bm.verts.ensure_lookup_table()
    comp, ncomp = components(bm)
    lo = [[1e9] * 3 for _ in range(ncomp)]
    hi = [[-1e9] * 3 for _ in range(ncomp)]
    for v in bm.verts:
        c = comp[v.index]
        for i in range(3):
            lo[c][i] = min(lo[c][i], v.co[i])
            hi[c][i] = max(hi[c][i], v.co[i])
    hit = {c for c in range(ncomp) if rule(Vector(lo[c]), Vector(hi[c]))}
    uv = bm.loops.layers.uv.active
    cu, cv = (PATCH[0] + PATCH[2]) / 2, (PATCH[1] + PATCH[3]) / 2
    for f in bm.faces:
        if comp[f.verts[0].index] in hit:
            for l in f.loops:
                l[uv].uv = (cu + 0.02 * math.sin(l.vert.co.y * 3.0), cv + 0.02 * math.sin(l.vert.co.z * 9.0))
    bm.to_mesh(obj.data)
    bm.free()
    print(what + ": painted", len(hit), "of", ncomp, "pieces of", obj.name)


def strip_pieces(obj, rule, what):
    """Delete the loose pieces of obj for which rule(lo, hi) is true (F100 frame)."""
    bm = bmesh.new()
    bm.from_mesh(obj.data)
    bm.verts.ensure_lookup_table()
    comp, ncomp = components(bm)
    lo = [[1e9] * 3 for _ in range(ncomp)]
    hi = [[-1e9] * 3 for _ in range(ncomp)]
    for v in bm.verts:
        c = comp[v.index]
        for i in range(3):
            lo[c][i] = min(lo[c][i], v.co[i])
            hi[c][i] = max(hi[c][i], v.co[i])
    drop = {c for c in range(ncomp) if rule(Vector(lo[c]), Vector(hi[c]))}
    bmesh.ops.delete(bm, geom=[v for v in bm.verts if comp[v.index] in drop], context="VERTS")
    bm.to_mesh(obj.data)
    bm.free()
    print(what + ": removed", len(drop), "of", ncomp, "pieces of", obj.name)

CX = 0.118  # the F100's centerline


def out_from_center(lo, hi):
    """How far the piece's inner edge is from the centerline."""
    if lo.x >= CX:
        return lo.x - CX
    if hi.x <= CX:
        return CX - hi.x
    return 0.0

body = bpy.data.objects["fmainbody"]
# The 1977 grille, headlight bezels and front bumper.
strip_pieces(body, lambda lo, hi: lo.y > 2.39, "front end")
# The rear bumper (the Diamondback's step bumper stays on its frame).
strip_pieces(body, lambda lo, hi: lo.y < -2.19 and hi.z < -1.45, "rear bumper")
# The full-length chrome beltline molding (it covers a panel seam, so it
# stays, painted body color: the Ranger's side crease).
def molding(lo, hi):
    return hi.z - lo.z < 0.10 and hi.y - lo.y > 0.6 and out_from_center(lo, hi) > 0.66 and lo.z > -1.25 and hi.z < -1.05
paint_pieces(body, molding, "molding")
channel_uv = []

for door in ("f100_ddoor", "f100_pdoor"):
    # The chrome mirror and its arm, and the vent window's divider bar.
    strip_pieces(bpy.data.objects[door], lambda lo, hi: out_from_center(lo, hi) > 0.84 and lo.y > 0.89 and hi.y < 1.05 and lo.z > -1.14 and hi.z < -0.73, "mirror")
    paint_pieces(bpy.data.objects[door], molding, "molding")
    strip_pieces(bpy.data.objects[door], lambda lo, hi: lo.y > 0.90 and hi.y < 1.16 and hi.y - lo.y < 0.25 and hi.z - lo.z > 0.3 and lo.z > -0.97 and hi.z < -0.48 and out_from_center(lo, hi) > 0.6, "vent bar")

# ------------------------------------------------------------------ 3. remap
SX, SZ = 0.92, 0.875
X0, Z0, ZT = 0.118, -1.904, 0.705
# F100 y -> Diamondback y, keeping the arches round (0.92 around each axle).
YCP = [
    (-2.45, -2.58),
    (-2.285, -2.385),   # tailgate
    (-1.561, -1.855),   # rear arch, back edge
    (-1.041, -1.375),   # rear axle
    (-0.521, -0.895),   # rear arch, front edge
    (0.12, -0.40),      # back of the cab
    (1.356, 0.93),      # windshield base
    (1.404, 0.974),     # front arch, back edge
    (1.904, 1.434),     # front axle
    (2.404, 1.894),     # front arch, front edge
    (2.62, 2.12),
]


def pchip(points):
    xs = [p[0] for p in points]
    ys = [p[1] for p in points]
    n = len(xs)
    h = [xs[i + 1] - xs[i] for i in range(n - 1)]
    d = [(ys[i + 1] - ys[i]) / h[i] for i in range(n - 1)]
    m = [0.0] * n
    m[0], m[-1] = d[0], d[-1]
    for i in range(1, n - 1):
        if d[i - 1] * d[i] <= 0:
            m[i] = 0
        else:
            w1, w2 = 2 * h[i] + h[i - 1], h[i] + 2 * h[i - 1]
            m[i] = (w1 + w2) / (w1 / d[i - 1] + w2 / d[i])

    def f(x):
        if x <= xs[0]:
            return ys[0] + m[0] * (x - xs[0])
        if x >= xs[-1]:
            return ys[-1] + m[-1] * (x - xs[-1])
        i = max(k for k in range(n - 1) if xs[k] <= x)
        t = (x - xs[i]) / h[i]
        h00 = 2 * t ** 3 - 3 * t ** 2 + 1
        h10 = t ** 3 - 2 * t ** 2 + t
        h01 = -2 * t ** 3 + 3 * t ** 2
        h11 = t ** 3 - t ** 2
        return h00 * ys[i] + h10 * h[i] * m[i] + h01 * ys[i + 1] + h11 * h[i] * m[i + 1]
    return f

ymap = pchip(YCP)


def smooth(t):
    t = min(1.0, max(0.0, t))
    return t * t * (3 - 2 * t)


HOOD_DROP = 0.10
# The nose, seen from above: the fenders end at y 2.013 (measured), 0.846 out;
# the front corners round off on a 0.17 m radius, blended in over the last 0.32 m
# (above the bumper; below it the bumper wraps round the corners instead).
NOSE_Y, NOSE_HW, NOSE_R, NOSE_L = 2.013, 0.852, 0.17, 0.32


def corner_pullback(ax, r=NOSE_R, hw=NOSE_HW):
    """How far the nose's outline at |x| = ax lies behind its middle."""
    d = ax - (hw - r)
    if d <= 0:
        return 0.0
    d = min(d, r)
    return r - math.sqrt(r * r - d * d)


def remap(co):
    x = SX * (co.x - X0)
    y = ymap(co.y)
    z = ZT + SZ * (co.z - Z0)
    # Ranger nose: hood and fender tops slope down toward the front...
    if y > 1.0:
        z -= HOOD_DROP * smooth((y - 1.0) / 1.15) * smooth((z - 1.10) / 0.28)
    # ...and the front corners round off in plan view (keeping their height).
    if y > NOSE_Y - NOSE_L:
        blend = smooth((y - (NOSE_Y - NOSE_L)) / NOSE_L)
        y -= corner_pullback(abs(x)) * blend * smooth((z - 1.0) / 0.15)
    return Vector((x, y, z))


for o in scene.objects:
    me = o.data
    for v in me.vertices:
        v.co = remap(o.matrix_world @ v.co)
    o.matrix_world.identity()
    me.update()

# The 2001-up hood's raised middle ("power dome"), wide and running its length.
for v in bpy.data.objects["fhood"].data.vertices:
    c = v.co
    w = max(0.0, 1.0 - (c.x / 0.42) ** 2)
    v.co.z += 0.03 * w * w * (0.4 + 0.6 * smooth((c.y - 1.15) / 0.5)) * (1.0 - 0.5 * smooth((c.y - 1.9) / 0.15))
bpy.data.objects["fhood"].data.update()

# The fuel cap stays in the F100's filler recess, on the left like a Ranger's;
# the mod moves the game's fuel inlet there.
cap = bpy.data.objects["FuelCapF"]
FUEL_CAP = sum((v.co for v in cap.data.vertices), Vector()) / len(cap.data.vertices)
print("fuel cap at", tuple(round(c, 3) for c in FUEL_CAP))

# ------------------------------------------------------------------ 4. Ranger front end
def mat(name, rgb, metal=0.0, rough=0.5, alpha=1.0):
    m = bpy.data.materials.get(name)
    if m is None:
        m = bpy.data.materials.new(name)
        m.use_nodes = True
        b = m.node_tree.nodes["Principled BSDF"]
        b.inputs["Base Color"].default_value = (*rgb, 1)
        b.inputs["Metallic"].default_value = metal
        b.inputs["Roughness"].default_value = rough
        if alpha < 1:
            b.inputs["Alpha"].default_value = alpha
    return m

# New parts use the game's own materials by name; the mod finds them in game.
PAINT = bpy.data.materials["Tex_RGB_23"]          # the F100's body (tinted with the paint color in game)
# (R_* materials are made in game from the game's Standard shader; see the MTL.)
BLACK = mat("R_Black", (0.035, 0.035, 0.037), 0.0, 0.62)
GRILLE = mat("R_Grille", (0.012, 0.012, 0.013), 0.25, 0.45)
CHROME = mat("R_Chrome", (0.86, 0.86, 0.88), 1.0, 0.1)
BLUE = mat("R_Oval", (0.02, 0.09, 0.42), 0.4, 0.15)
# The Diamondback's own lamp materials (found by name in game): grey, with the
# lens pattern in their normal maps.
HEAD = mat("frontLight", (0.65, 0.65, 0.65), 0.3, 0.2)
TURN = mat("frontTurn", (0.9, 0.9, 0.9), 0.2, 0.2)


def lamp_look(m, guid):
    """Preview only: the lens pattern (the game's normal map) on a lamp material."""
    nt = m.node_tree
    if any(n.type == "NORMAL_MAP" for n in nt.nodes):
        return
    img = os.path.join(os.path.dirname(os.path.abspath(DPT_BLEND)), "dpt_raw", guid, "asset")
    if not os.path.exists(img):
        return
    tex = nt.nodes.new("ShaderNodeTexImage")
    tex.image = bpy.data.images.load(img)
    tex.image.colorspace_settings.name = "Non-Color"
    nm = nt.nodes.new("ShaderNodeNormalMap")
    nt.links.new(tex.outputs["Color"], nm.inputs["Color"])
    nt.links.new(nm.outputs["Normal"], nt.nodes["Principled BSDF"].inputs["Normal"])

lamp_look(HEAD, "f49e6881ff0200744b5238834c46aa8d")
lamp_look(TURN, "d00375479840fcb428d8759f19ebd6ee")

new_parts = []


def new_object(name, group, bm, material):
    me = bpy.data.meshes.new(name)
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    bm.to_mesh(me)
    bm.free()
    me.materials.append(material)
    o = bpy.data.objects.new(name, me)
    scene.collection.objects.link(o)
    o["group"] = group
    o["path"] = name
    new_parts.append(o)
    return o


def hull(points, name, group, material, bevel=0.0):
    bm = bmesh.new()
    vs = [bm.verts.new(p) for p in points]
    bmesh.ops.convex_hull(bm, input=vs)
    for v in [v for v in bm.verts if not v.link_faces]:
        bm.verts.remove(v)
    o = new_object(name, group, bm, material)
    if bevel:
        apply_bevel(o, bevel)
    return o


def box(x0, x1, y0, y1, z0, z1, name, group, material, bevel=0.0):
    return hull([(x, y, z) for x in (x0, x1) for y in (y0, y1) for z in (z0, z1)], name, group, material, bevel)


def apply_bevel(o, width, segments=2):
    m = o.modifiers.new("bv", "BEVEL")
    m.width = width
    m.segments = segments
    m.limit_method = "ANGLE"
    m.angle_limit = math.radians(35)
    dg = bpy.context.evaluated_depsgraph_get()
    me = bpy.data.meshes.new_from_object(o.evaluated_get(dg))
    o.modifiers.clear()
    o.data = me


def cylinder_y(cx, cz, rx, rz, y0, y1, name, group, material, seg=40):
    bm = bmesh.new()
    a = [bm.verts.new((cx + rx * math.cos(2 * math.pi * i / seg), y0, cz + rz * math.sin(2 * math.pi * i / seg))) for i in range(seg)]
    b = [bm.verts.new((v.co.x, y1, v.co.z)) for v in a]
    bm.faces.new(a)
    bm.faces.new(b)
    for i in range(seg):
        j = (i + 1) % seg
        bm.faces.new((a[i], a[j], b[j], b[i]))
    return new_object(name, group, bm, material)


# Measure the front of the remapped fenders and hood so the new front end meets them.
def bounds(objs, pred=lambda v: True):
    lo = Vector((1e9,) * 3)
    hi = Vector((-1e9,) * 3)
    for o in objs:
        for v in o.data.vertices:
            if pred(v.co):
                lo = Vector(map(min, lo, v.co))
                hi = Vector(map(max, hi, v.co))
    return lo, hi

import numpy as np
hood_obj = bpy.data.objects["fhood"]
hlo, hhi = bounds([hood_obj])
HOOD_FRONT_Z = max(v.co.z for v in hood_obj.data.vertices if v.co.y > hhi.y - 0.06 and abs(v.co.x) < 0.3) - 0.02
HOOD_FRONT_Y = max(v.co.y for v in hood_obj.data.vertices if abs(v.co.x) < 0.3)
print("hood front edge z", round(HOOD_FRONT_Z, 3), "y", round(HOOD_FRONT_Y, 3))

FACE = NOSE_Y + 0.035               # front of the headlights, in the middle
HW = NOSE_HW + 0.004
BAND = (HOOD_FRONT_Z - 0.275, HOOD_FRONT_Z - 0.008)   # grille and headlights
BUMPER = (BAND[0] - 0.245, BAND[0] - 0.008)

# Paint UVs: new painted parts sample a clean, fully painted patch of the F100's
# atlas (so they take the paint and the game's grime like the rest of the body).
PATCH = (0.630, 0.224, 0.698, 0.291)


def paint_uvs(o):
    me = o.data
    uv = me.uv_layers.new(name="UVMap")
    for poly in me.polygons:
        n = poly.normal
        ax = max(range(3), key=lambda i: abs(n[i]))
        for li in poly.loop_indices:
            c = me.vertices[me.loops[li].vertex_index].co
            a_, b_ = [(c.y, c.z), (c.x, c.z), (c.x, c.y)][ax]
            u = PATCH[0] + (PATCH[2] - PATCH[0]) * (0.5 + 0.5 * math.sin(a_ * 1.7))
            v = PATCH[1] + (PATCH[3] - PATCH[1]) * (0.5 + 0.5 * math.sin(b_ * 1.7))
            uv.data[li].uv = (u, v)


class Outline:
    """The nose's outline from above (right half; mirrored for the left): a gently
    bowed front, a rounded corner, then back along the side. Points by arc length
    s from the middle (negative s = left side), with the outward normal."""

    def __init__(self, face, hw, r, bow, side):
        pts = []
        straight = hw - r
        for i in range(25):
            x = straight * i / 24
            y = face + bow * (1 - (x / straight) ** 2) if straight > 0 else face
            pts.append((x, y))
        cx, cy = straight, face - r
        for i in range(1, 25):
            t = math.radians(90 - 90 * i / 24)
            pts.append((cx + r * math.cos(t), cy + r * math.sin(t)))
        for i in range(1, 9):
            pts.append((hw, cy - side * i / 8))
        self.pts = [Vector(p) for p in pts]
        self.s = [0.0]
        for i in range(1, len(self.pts)):
            self.s.append(self.s[-1] + (self.pts[i] - self.pts[i - 1]).length)
        self.total = self.s[-1]
        self.end_front = self.s[24]          # where the corner starts
        self.end_corner = self.s[48]         # where the side starts

    def at(self, s):
        sign = -1.0 if s < 0 else 1.0
        s = min(abs(s), self.total)
        i = 1
        while i < len(self.s) - 1 and self.s[i] < s:
            i += 1
        a, b = self.pts[i - 1], self.pts[i]
        t = (s - self.s[i - 1]) / max(1e-9, self.s[i] - self.s[i - 1])
        p = a.lerp(b, t)
        tan = (b - a).normalized()
        n = Vector((-tan.y, tan.x))       # outward (left of the direction of travel)
        return Vector((sign * p.x, p.y)), Vector((sign * n.x, n.y))

    def s_at_x(self, x):
        """Arc length where the outline reaches |x| (front and corner)."""
        lo, hi = 0.0, self.end_corner
        for _ in range(40):
            mid = (lo + hi) / 2
            if self.at(mid)[0].x < abs(x):
                lo = mid
            else:
                hi = mid
        return (lo + hi) / 2 * (1 if x >= 0 else -1)


def set_uvs(o, uvs):
    """UVs by vertex (in the order the vertices were made)."""
    me = o.data
    layer = me.uv_layers.new(name="UVMap")
    for li, loop in enumerate(me.loops):
        layer.data[li].uv = uvs[loop.vertex_index]


def sweep_outline(name, group, material, outline, ss, profile, cap=True, uv=None):
    """Loft a profile (list of (d, z): d out along the outline's normal) along it.
    uv(s, z) gives a texture coordinate for each point, if wanted."""
    bm = bmesh.new()
    rings = []
    uvs = []
    for s_ in ss:
        p, n = outline.at(s_)
        rings.append([bm.verts.new((p.x + n.x * d, p.y + n.y * d, z)) for (d, z) in profile(s_)])
        if uv:
            uvs.extend(uv(s_, z) for (d, z) in profile(s_))
    k = len(rings[0])
    for i in range(len(rings) - 1):
        for j in range(k):
            jj = (j + 1) % k
            bm.faces.new((rings[i][j], rings[i][jj], rings[i + 1][jj], rings[i + 1][j]))
    if cap:
        bm.faces.new(rings[0])
        bm.faces.new(list(reversed(rings[-1])))
    o = new_object(name, group, bm, material)
    o.data.shade_smooth()
    if material is PAINT:
        paint_uvs(o)
    elif uv:
        set_uvs(o, uvs)
    return o


def span(a, b, n):
    return [a + (b - a) * i / (n - 1) for i in range(n)]


def disc(name, group, material, outline, s_, z, r_out, d0, d1, scale_z=1.0, seg=32):
    """A round part facing out along the outline's normal (lamps, fog lights)."""
    p, n = outline.at(s_)
    side = Vector((-n.y, n.x))
    bm = bmesh.new()
    rings = []
    for d in (d0, d1):
        ring = []
        for i in range(seg):
            a = 2 * math.pi * i / seg
            u, w = math.cos(a) * r_out, math.sin(a) * r_out * scale_z
            ring.append(bm.verts.new((p.x + n.x * d + side.x * u, p.y + n.y * d + side.y * u, z + w)))
        rings.append(ring)
    bm.faces.new(rings[0])
    bm.faces.new(list(reversed(rings[1])))
    for i in range(seg):
        j = (i + 1) % seg
        bm.faces.new((rings[0][i], rings[0][j], rings[1][j], rings[1][i]))
    o = new_object(name, group, bm, material)
    o.data.shade_smooth()
    # The lens texture's circle on the face.
    set_uvs(o, [(0.5 + 0.5 * math.cos(2 * math.pi * i / seg), 0.5 + 0.5 * math.sin(2 * math.pi * i / seg)) for _ in (d0, d1) for i in range(seg)])
    return o


NOSE = Outline(FACE, HW, NOSE_R, 0.025, 0.14)

# Front panel: closes the nose behind the lights and grille, under the hood.
sweep_outline("FrontPanel", "Fascia", PAINT, NOSE, span(-NOSE.end_corner, NOSE.end_corner, 61), lambda s_: [
    (-0.16, BAND[0] - 0.02), (-0.09, BAND[0] - 0.02), (-0.09, HOOD_FRONT_Z - 0.006), (-0.16, HOOD_FRONT_Z - 0.006)])

# Grille: a chunky body-colored surround standing proud of the headlights, its
# edges rounded, around a black honeycomb with the oval.
GW = 0.36
gz0, gz1 = BAND[0] - 0.005, BAND[1] + 0.004
LEAN = 0.12   # the face leans back towards the top


def surround_profile(s_):
    x = abs(NOSE.at(s_)[0].x)
    k = smooth((x - (GW + 0.035)) / 0.035)   # round the ends
    f = 0.04 - 0.05 * k
    pts = [(-0.07, gz0), (f - 0.02, gz0), (f - 0.004, gz0 + 0.012), (f, gz0 + 0.03),
           (f, gz1 - 0.03), (f - 0.004, gz1 - 0.012), (f - 0.02, gz1), (-0.07, gz1)]
    return [(d - LEAN * (z - gz0), z) for d, z in pts]

g0, g1 = NOSE.s_at_x(-GW - 0.07), NOSE.s_at_x(GW + 0.07)
ring = sweep_outline("GrilleSurround", "Fascia", PAINT, NOSE, span(g0, g1, 41), surround_profile)


def rounded_rect(hw, z0, z1, r, n=6):
    pts = []
    for cx, cz, a0 in ((hw - r, z1 - r, 0), (-hw + r, z1 - r, 90), (-hw + r, z0 + r, 180), (hw - r, z0 + r, 270)):
        for i in range(n + 1):
            a = math.radians(a0 + 90 * i / n)
            pts.append((cx + r * math.cos(a), cz + r * math.sin(a)))
    return pts

bm = bmesh.new()
opening = rounded_rect(GW, gz0 + 0.045, gz1 - 0.045, 0.035)
front_ = [bm.verts.new((x, FACE + 0.3, z)) for x, z in opening]
back_ = [bm.verts.new((x, FACE - 0.3, z)) for x, z in opening]
bm.faces.new(front_)
bm.faces.new(list(reversed(back_)))
for i in range(len(opening)):
    j = (i + 1) % len(opening)
    bm.faces.new((front_[i], back_[i], back_[j], front_[j]))
cut = new_object("cut", "Fascia", bm, PAINT)
mod = ring.modifiers.new("b", "BOOLEAN")
mod.operation = "DIFFERENCE"
mod.solver = "EXACT"
mod.object = cut
dg = bpy.context.evaluated_depsgraph_get()
ring.data = bpy.data.meshes.new_from_object(ring.evaluated_get(dg))
ring.modifiers.clear()
bpy.data.objects.remove(cut)
paint_uvs(ring)


def face_y(x):
    return NOSE.at(NOSE.s_at_x(x))[0].y

bm = bmesh.new()
cell = 0.03
zc = gz0 + 0.045 + cell * 0.5
r = 0
while zc < gz1 - 0.045 - cell * 0.45:
    x = -GW + cell * (0.5 + 0.5 * (r % 2))
    while x < GW - cell * 0.45:
        yf = face_y(x) - 0.02 - LEAN * (zc - gz0)
        o6 = [bm.verts.new((x + cell * 0.5 * math.cos(math.radians(30 + 60 * k)), yf, zc + cell * 0.5 * math.sin(math.radians(30 + 60 * k)))) for k in range(6)]
        i6 = [bm.verts.new((x + cell * 0.36 * math.cos(math.radians(30 + 60 * k)), yf, zc + cell * 0.36 * math.sin(math.radians(30 + 60 * k)))) for k in range(6)]
        b6 = [bm.verts.new((v.co.x, yf - 0.03, v.co.z)) for v in i6]
        for k in range(6):
            j = (k + 1) % 6
            bm.faces.new((o6[k], o6[j], i6[j], i6[k]))
            bm.faces.new((i6[k], i6[j], b6[j], b6[k]))
        x += cell
    zc += cell * 0.86
    r += 1
new_object("Honeycomb", "Fascia", bm, GRILLE)
sweep_outline("GrilleBack", "Fascia", GRILLE, NOSE, span(NOSE.s_at_x(-GW), NOSE.s_at_x(GW), 9), lambda s_: [
    (-0.075, gz0 + 0.04), (-0.06, gz0 + 0.04), (-0.06 - LEAN * (gz1 - gz0 - 0.08), gz1 - 0.04), (-0.075 - LEAN * (gz1 - gz0 - 0.08), gz1 - 0.04)])
oz = (gz0 + gz1) / 2
yo = face_y(0)
yo -= LEAN * (oz - gz0)
cylinder_y(0, oz, 0.09, 0.037, yo + 0.01, yo + 0.03, "OvalRim", "Fascia", CHROME)
cylinder_y(0, oz, 0.08, 0.028, yo + 0.02, yo + 0.036, "Oval", "Fascia", BLUE)

# Headlights: from beside the grille, round the corner to the front of the
# fender; the top runs just under the hood's front edge (which drops towards the
# corners), so they get narrower outward. Clear lens over a chrome housing with
# two round lamps; amber where it wraps round the corner.
from mathutils.bvhtree import BVHTree
HOOD_BVH = BVHTree.FromObject(hood_obj, bpy.context.evaluated_depsgraph_get())


def hood_edge_z(x):
    """Height of the hood's front edge at x (its last point seen from above)."""
    x = min(abs(x), 0.83)
    z = HOOD_FRONT_Z
    for i in range(150):
        hit = HOOD_BVH.ray_cast(Vector((x, 1.6 + 0.004 * i, 3.0)), Vector((0, 0, -1)))
        if hit[0] is not None:
            z = hit[0].z
    return z

L0 = NOSE.s_at_x(GW + 0.075)
L1 = NOSE.end_corner + 0.05
AMB = NOSE.end_corner - 0.06
zb = BAND[0] + 0.012
for sgn in (1, -1):
    ss = [sgn * v for v in span(L0, L1, 33)]
    if sgn < 0:
        ss = list(reversed(ss))
    raw = [hood_edge_z(NOSE.at(s_)[0].x) - 0.01 for s_ in ss]
    # Smooth, and never rising towards the outside.
    order = sorted(range(len(ss)), key=lambda i: abs(ss[i]))
    sm = []
    for k, i in enumerate(order):
        window = [raw[order[j]] for j in range(max(0, k - 2), min(len(order), k + 3))]
        v = sum(window) / len(window)
        sm.append(min(v, sm[-1]) if sm else v)
    tops = {ss[i]: max(sm[k], zb + 0.10) for k, i in enumerate(order)}

    def lens(s_, d_front=0.006, d_back=-0.075, inset=0.0):
        top = tops[min(tops, key=lambda k: abs(k - s_))] - inset
        # The bottom rises a little as it wraps round the corner.
        bot = zb + inset + 0.03 * smooth((abs(s_) - (NOSE.end_corner - 0.14)) / 0.18)
        pts = [(d_back, bot), (d_front - 0.01, bot), (d_front, bot + 0.012), (d_front, top - 0.012), (d_front - 0.01, top), (d_back, top)]
        return [(d - LEAN * (z - zb), z) for d, z in pts]

    clear = [v for v in ss if abs(v) <= AMB + 1e-6]
    amber = [v for v in ss if abs(v) >= AMB - 1e-6]

    def across(s_, z, s0, s1, reps):
        """Texture coordinates over a lamp: reps lens circles across, one high."""
        top = tops[min(tops, key=lambda k: abs(k - s_))]
        return ((abs(s_) - s0) / (s1 - s0) * reps, (z - zb) / max(0.05, top - zb))

    # The headlight: the Diamondback's headlight lens, two of its round lamps
    # side by side; round the corner its turn signal.
    sweep_outline("HeadLens", "Lights", HEAD, NOSE, clear, lens, uv=lambda s_, z: across(s_, z, L0, AMB, 2))
    sweep_outline("Signal", "Lights", TURN, NOSE, amber, lens, uv=lambda s_, z: (
        0.05 + 0.65 * (abs(s_) - AMB) / (L1 - AMB), 0.1 + 0.8 * (z - zb) / max(0.05, tops[min(tops, key=lambda k: abs(k - s_))] - zb)))
    # A dark back to the whole lamp.
    sweep_outline("LampBack", "Lights", BLACK, NOSE, ss, lambda s_: lens(s_, -0.076, -0.09, -0.004))

# Bumper: big, body-colored, its top rounded, wrapping round the corners back
# towards the wheels; the upper face carries the plate, and below a step the
# lower lip has the intake and the fog lights.
BUMP = Outline(FACE + 0.075, HW + 0.03, 0.30, 0.04, 0.16)
z0, z1 = BUMPER
zm = z0 + 0.105


def bumper_profile(s_):
    # The ends tuck in against the fenders, in front of the wheels.
    tuck = 0.035 * smooth((abs(s_) - (BUMP.total - 0.07)) / 0.07)
    return [(d - tuck, z) for d, z in [(-0.24, z1 - 0.005), (-0.12, z1 + 0.004), (-0.05, z1 - 0.002), (-0.015, z1 - 0.022), (0.0, z1 - 0.06),
            (0.002, zm + 0.03), (-0.008, zm + 0.008), (-0.035, zm - 0.004), (-0.04, zm - 0.03),
            (-0.032, z0 + 0.04), (-0.04, z0 + 0.012), (-0.07, z0), (-0.24, z0)]]

sweep_outline("FrontBumper", "Bumper", PAINT, BUMP, span(-BUMP.total, BUMP.total, 81), bumper_profile)
# Intake: a black slot across the lower lip.
sweep_outline("Intake", "Bumper", GRILLE, BUMP, span(BUMP.s_at_x(-0.42), BUMP.s_at_x(0.42), 17), lambda s_: [
    (-0.07, z0 + 0.022), (-0.03, z0 + 0.022), (-0.034, zm - 0.022), (-0.07, zm - 0.022)])
for sgn in (1, -1):
    s_ = BUMP.s_at_x(sgn * 0.60)
    zf = (z0 + zm) / 2 - 0.004
    disc("FogPocket", "Bumper", GRILLE, BUMP, s_, zf, 0.058, -0.06, -0.024, 0.85)
    disc("Fog", "Bumper", HEAD, BUMP, s_, zf, 0.044, -0.03, -0.014, 0.85)
yp = BUMP.at(0)[0].y
box(-0.16, 0.16, yp - 0.004, yp + 0.008, zm + 0.03, zm + 0.112, "Plate", "Bumper", CHROME)
sweep_outline("Valance", "Bumper", BLACK, BUMP, span(BUMP.s_at_x(-0.72), BUMP.s_at_x(0.72), 31), lambda s_: [
    (-0.20, z0 - 0.045), (-0.08, z0 - 0.045), (-0.06, z0 + 0.004), (-0.20, z0 + 0.004)])

# Engine bay: the F100 body has no firewall or inner fenders, and the
# Diamondback's (part of its cab, which the mod hides) don't show. Take them
# from the Diamondback's cab: what's ahead of the cab, inside the fenders and
# under the hood, painted like the body.
with bpy.data.libraries.load(DPT_BLEND) as (src, dst):
    dst.objects = ["cabin_low"]
cab = dst.objects[0]
bm = bmesh.new()
bm.from_mesh(cab.data)
bm.transform(cab.matrix_world)
drop = []
for f in bm.faces:
    c = f.calc_center_median()
    inside = abs(c.x) < 0.62 or (abs(c.x) < 0.76 and c.z > 1.15)   # clear of the wheel wells
    if not (0.78 < c.y < 1.95 and inside and c.z < min(1.40, HOOD_FRONT_Z + 0.03 + 0.08 * (1.95 - c.y))):
        drop.append(f)
bmesh.ops.delete(bm, geom=drop, context="FACES")
bmesh.ops.delete(bm, geom=[v for v in bm.verts if not v.link_faces], context="VERTS")
print("engine bay:", len(bm.faces), "faces from the Diamondback's cab")
bay = new_object("EngineBay", "Body", bm, PAINT)
paint_uvs(bay)
bpy.data.objects.remove(cab)

# Bull bar (an accessory: the mod puts it in the Diamondback's brush guard slot
# on Rangers, so it pops on and bolts down like the game's own parts). Black
# steel: two upright plates bolted to the frame horns (where the game's brush
# guard bolts are), a center hoop over the grille with a bar across the bumper,
# and three tubes each side wrapping round the headlights.
BULLBAR = mat("R_BullBar", (0.025, 0.025, 0.027), 0.35, 0.42)


def tube(points, r, name, seg=12, caps=True):
    """A round tube along a polyline (points in the Ranger's frame)."""
    pts = [Vector(p) for p in points]
    bm = bmesh.new()
    rings = []
    prev_n = None
    for i, p in enumerate(pts):
        t = (pts[min(i + 1, len(pts) - 1)] - pts[max(i - 1, 0)]).normalized()
        if prev_n is None:
            up = Vector((0, 0, 1)) if abs(t.z) < 0.9 else Vector((1, 0, 0))
            n = t.cross(up).normalized()
        else:
            n = (prev_n - t * prev_n.dot(t)).normalized()
        prev_n = n
        b = t.cross(n)
        rings.append([bm.verts.new(p + (n * math.cos(a) + b * math.sin(a)) * r)
                      for a in (2 * math.pi * k / seg for k in range(seg))])
    for i in range(len(rings) - 1):
        for k in range(seg):
            kk = (k + 1) % seg
            bm.faces.new((rings[i][k], rings[i][kk], rings[i + 1][kk], rings[i + 1][k]))
    if caps:
        bm.faces.new(list(reversed(rings[0])))
        bm.faces.new(rings[-1])
    o = new_object(name, "BullBar", bm, BULLBAR)
    o.data.shade_smooth()
    return o


def plate(path, x, half_w, half_t, name):
    """A flat bar standing in the y-z plane at x: path is (y, z) points, the plate
    half_w wide across the path and half_t thick in x."""
    bm = bmesh.new()
    rings = []
    P = [Vector((0, y, z)) for y, z in path]
    for i, p in enumerate(P):
        t = (P[min(i + 1, len(P) - 1)] - P[max(i - 1, 0)]).normalized()
        n = Vector((0, -t.z, t.y))          # across the path, in the y-z plane
        c = Vector((x, p.y, p.z))
        rings.append([bm.verts.new(c + n * a + Vector((b, 0, 0))) for a, b in ((-half_w, -half_t), (half_w, -half_t), (half_w, half_t), (-half_w, half_t))])
    for i in range(len(rings) - 1):
        for k in range(4):
            kk = (k + 1) % 4
            bm.faces.new((rings[i][k], rings[i][kk], rings[i + 1][kk], rings[i + 1][k]))
    bm.faces.new(list(reversed(rings[0])))
    bm.faces.new(rings[-1])
    return new_object(name, "BullBar", bm, BULLBAR)


BB_X = 0.33                       # the game's brush guard bolts: x +-0.33, y 2.153, z 0.74-0.77
BB_TOP = BAND[1] + 0.02           # level with the hood's front edge
BB_MID = BUMPER[1] + 0.03         # just over the bumper
BB_LOW = BUMPER[0] + 0.035        # across the bumper's lower lip
UPRIGHT = [(2.150, 0.700), (2.165, 0.790), (2.255, 0.850), (2.275, BB_LOW), (2.280, BB_MID), (2.262, BB_TOP - 0.02), (2.245, BB_TOP + 0.015)]


def upright_y(z):
    """Where the upright's front is at height z."""
    for (y0, z0_), (y1, z1_) in zip(UPRIGHT, UPRIGHT[1:]):
        if z0_ <= z <= z1_:
            return y0 + (y1 - y0) * (z - z0_) / max(1e-6, z1_ - z0_)
    return UPRIGHT[-1][0]

for sgn in (1, -1):
    plate(UPRIGHT, sgn * BB_X, 0.045, 0.011, "BullUpright")
    # The foot: a bracket against the frame horn, through which the bolts go.
    box(sgn * BB_X - 0.012, sgn * BB_X + 0.012, 2.11, 2.20, 0.70, 0.80, "BullFoot", "BullBar", BULLBAR)
# Center: the top hoop and two bars across, between the uprights.
for z_, bow in ((BB_TOP, 0.02), (BB_MID, 0.012), (BB_LOW, 0.008)):
    y_ = upright_y(z_) + 0.012
    tube([(BB_X * (2 * i / 16 - 1), y_ + bow * (1 - (2 * i / 16 - 1) ** 2), z_) for i in range(17)], 0.021, "BullBar")
# Wings: three tubes round each headlight, ending near the fender, joined at
# the end by an upright tube.
for sgn in (1, -1):
    ends = []
    for k, z_ in enumerate((BB_TOP - 0.01, (BB_TOP + zb) / 2, zb + 0.03)):
        s0 = NOSE.s_at_x(BB_X + 0.02)
        s1 = NOSE.end_corner - 0.02
        pts = []
        y_up = upright_y(z_)
        d0 = y_up - NOSE.at(s0)[0].y + 0.01
        for i in range(25):
            s_ = s0 + (s1 - s0) * i / 24
            p, n = NOSE.at(s_)
            d = d0 + (0.06 - d0) * smooth(i / 24 * 1.3 - 0.3)
            pts.append((sgn * (p.x + n.x * d), p.y + n.y * d, z_))
        pts.insert(0, (sgn * (BB_X + 0.005), y_up + 0.012, z_))
        tube(pts, 0.017, "BullWing")
        ends.append(pts[-1])
    tube([ends[0], ends[1], ends[2]], 0.017, "BullWingEnd")

# Two-tone paint (like the 2001-04 Edge): the body below a line along the sides
# (through the wheel arches) is its own paint group, "Lower", with the bumper.
# The cab and bed, doors and tailgate are cut there and their lower parts become
# groups of their own (DoorDLower follows the door, and so on).
TWO_TONE_Z = 1.12
for src, lower in (("fmainbody", "BodyLower"), ("f100_ddoor", "DoorDLower"), ("f100_pdoor", "DoorPLower"), ("ftailgate", "TailgateLower")):
    o = bpy.data.objects[src]
    bm = bmesh.new()
    bm.from_mesh(o.data)
    geom = bm.verts[:] + bm.edges[:] + bm.faces[:]
    bmesh.ops.bisect_plane(bm, geom=geom, plane_co=(0, 0, TWO_TONE_Z), plane_no=(0, 0, 1))
    below = [f for f in bm.faces if f.calc_center_median().z < TWO_TONE_Z]
    above = [f for f in bm.faces if f not in set(below)]
    bm_low = bm.copy()
    # Keep the faces below in the copy, the rest in the original.
    low_drop = [f for f in bm_low.faces if f.calc_center_median().z >= TWO_TONE_Z]
    bmesh.ops.delete(bm_low, geom=low_drop, context="FACES")
    bmesh.ops.delete(bm, geom=below, context="FACES")
    bm.to_mesh(o.data)
    bm.free()
    me = bpy.data.meshes.new(lower)
    bm_low.to_mesh(me)
    bm_low.free()
    for m in o.data.materials:
        me.materials.append(m)
    lo = bpy.data.objects.new(lower, me)
    scene.collection.objects.link(lo)
    lo["group"] = lower
    lo["path"] = lower
    print("two-tone:", src, len(me.polygons), "faces below", TWO_TONE_Z)

# Rain guards (smoked window visors) along the top of each door window and down
# its front edge, on the outside of the frame. Accessories: the mod shows them
# in slots on the doors, so they snap on with the game's own snap.
VISOR = mat("R_Visor", (0.018, 0.018, 0.02), 0.0, 0.12)
RAIN_PATH = [(0.665, 1.660), (0.600, 1.765), (0.530, 1.870), (0.470, 1.940), (0.400, 1.962), (0.250, 1.970), (0.050, 1.970), (-0.080, 1.962)]
# (o out from the window, u along the frame's outward normal: negative, so the
# guard hangs out and down over the top of the window, the way a rain guard does.)
RAIN_PROFILE = [(o * 0.75, -u * 0.75 + 0.012) for o, u in ((0.0, -0.004), (0.008, 0.030), (0.034, 0.052), (0.066, 0.058), (0.070, 0.052), (0.038, 0.046), (0.013, 0.027), (0.004, -0.004))]


def window_x(z):
    """The door window's outer face (x) at height z."""
    return 0.722 - 0.24 * (z - 1.6) + 0.045


def rain_guard(sgn, group):
    P = [Vector((0, y, z)) for y, z in RAIN_PATH]
    # A smoother path: two passes of corner cutting.
    for _ in range(2):
        Q = [P[0]]
        for a, b in zip(P, P[1:]):
            Q += [a.lerp(b, 0.25), a.lerp(b, 0.75)]
        Q.append(P[-1])
        P = Q
    bm = bmesh.new()
    rings = []
    for i, p in enumerate(P):
        t = (P[min(i + 1, len(P) - 1)] - P[max(i - 1, 0)]).normalized()
        n = Vector((0, -t.z, t.y))
        if n.y + n.z < 0:
            n = -n
        base = Vector((sgn * window_x(p.z), p.y, p.z))
        end_taper = smooth(i / 4) * smooth((len(P) - 1 - i) / 4)
        rings.append([bm.verts.new(base + n * (u * end_taper) + Vector((sgn * o * end_taper, 0, 0))) for o, u in RAIN_PROFILE])
    k = len(RAIN_PROFILE)
    for i in range(len(rings) - 1):
        for j in range(k):
            jj = (j + 1) % k
            bm.faces.new((rings[i][j], rings[i][jj], rings[i + 1][jj], rings[i + 1][j]))
    bm.faces.new(list(reversed(rings[0])))
    bm.faces.new(rings[-1])
    o = new_object("RainGuard", group, bm, VISOR)
    o.data.shade_smooth()
    return o

rain_guard(-1, "RainGuardD")
rain_guard(1, "RainGuardP")

# Sun visor: a shell over the top of the windshield, bolted to the front of the
# roof, with two air scoops; body colored (it takes the body's paint).
SV_TOP = [(0.42, 2.040), (0.55, 2.052), (0.66, 2.056), (0.78, 2.045), (0.87, 2.016), (0.915, 1.984)]
SV_BOT = [(0.905, 1.976), (0.83, 2.002), (0.70, 2.028), (0.58, 2.036), (0.44, 2.032)]
SV_HW = 0.74
SV_DEPTH = 0.68          # front to back: overhangs the windshield by about 12 cm
bm = bmesh.new()
rings = []
xs = [SV_HW * (2 * i / 40 - 1) for i in range(41)]
for x in xs:
    e = abs(x) / SV_HW
    # The ends round off: shallower and drawn back toward the roof.
    f = 1.0 - smooth((e - 0.82) / 0.18) * 0.85
    ring = []
    for y, z in SV_TOP + SV_BOT:
        yy = 0.42 + (y - 0.42) * f * SV_DEPTH
        zz = 2.032 + (z - 2.032) * (0.4 + 0.6 * f)
        ring.append(bm.verts.new((x, yy, zz - 0.012 * e * e)))
    rings.append(ring)
k = len(SV_TOP) + len(SV_BOT)
for i in range(len(rings) - 1):
    for j in range(k):
        jj = (j + 1) % k
        bm.faces.new((rings[i][j], rings[i][jj], rings[i + 1][jj], rings[i + 1][j]))
bm.faces.new(list(reversed(rings[0])))
bm.faces.new(rings[-1])
sv = new_object("SunVisor", "SunVisor", bm, PAINT)
sv.data.shade_smooth()
paint_uvs(sv)
for sx in (-0.30, 0.30):
    pts = []
    for y0, h, w in ((0.60, 0.002, 0.07), (0.70, 0.024, 0.08), (0.80, 0.020, 0.075), (0.84, 0.002, 0.07)):
        y = 0.42 + (y0 - 0.42) * SV_DEPTH
        for side in (-1, 1):
            z_top = 2.056 - 0.3 * (y - 0.66) ** 2
            pts += [(sx + side * w, y, z_top - 0.004), (sx + side * w * 0.8, y, z_top + h)]
    scoop = hull(pts, "SunVisorScoop", "SunVisor", PAINT, 0.006)
    paint_uvs(scoop)
SUN_VISOR_BOLTS = [(sx, 0.47, 2.047) for sx in (-0.6, -0.25, 0.25, 0.6)]

# Mirrors: black housings at the front of each door window, like the 2001-04 Ranger.
door_top = max(v.co.z for v in bpy.data.objects["f100_ddoor"].data.vertices if abs(v.co.x) > 0.80 and v.co.z < 1.6)
print("door belt line z", round(door_top, 3))
for s_, group in ((-1, "DoorD"), (1, "DoorP")):
    zb = door_top - 0.01
    pts = []
    for (x, y, z) in ((0.87, 0.78, zb), (1.03, 0.76, zb + 0.015), (1.05, 0.765, zb + 0.20), (0.885, 0.78, zb + 0.215),
                      (0.87, 0.90, zb + 0.01), (1.01, 0.88, zb + 0.03), (1.025, 0.88, zb + 0.19), (0.885, 0.90, zb + 0.20)):
        pts.append((s_ * x, y, z))
    hull(pts, "Mirror", group, BLACK, 0.02)
    glass_pts = [(s_ * x, 0.757, z) for (x, z) in ((0.905, zb + 0.03), (1.025, zb + 0.035), (1.03, zb + 0.185), (0.91, zb + 0.19))]
    hull(glass_pts + [(x, 0.759, z) for (x, _, z) in glass_pts], "MirrorGlass", group, CHROME)

# ------------------------------------------------------------------ paint layer texture
# The game's paint mask (the F100's rust layer alpha), thin gaps closed, plus the
# molding channel; clean near-white paint, tinted with the paint color in game.
from PIL import Image, ImageFilter, ImageDraw
import numpy as np
MODELS = os.path.dirname(DPT_BLEND)
rust_img = [i for i in bpy.data.images if os.path.basename(i.filepath).split(".")[0] and "rust" in i.name.lower() and "23" in i.name]
rust_path = bpy.path.abspath(rust_img[0].filepath)
rust = Image.open(rust_path).convert("RGBA")
W, H = rust.size
alpha = Image.fromarray(np.array(rust)[..., 3])
alpha = alpha.filter(ImageFilter.MaxFilter(15)).filter(ImageFilter.MinFilter(15))
draw = ImageDraw.Draw(alpha)
for tri in channel_uv:
    draw.polygon([(u * W, (1 - v) * H) for u, v in tri], fill=255)
alpha = alpha.filter(ImageFilter.MaxFilter(5))
rng = np.random.default_rng(7)
noise = Image.fromarray((rng.normal(0, 1, (H // 8, W // 8)) * 40 + 128).clip(0, 255).astype(np.uint8)).resize((W, H), Image.BICUBIC)
v = (226 + (np.array(noise).astype(np.float32) - 128) * 0.05).clip(0, 255)
paint_tex = np.dstack([v, v, v, np.array(alpha)]).astype(np.uint8)
os.makedirs(os.path.join(OUT, "assets"), exist_ok=True)
Image.fromarray(paint_tex, "RGBA").save(os.path.join(OUT, "assets", "ranger_paint.png"), optimize=True)
Image.fromarray(paint_tex, "RGBA").save(os.path.join(MODELS, "ranger_assets", "ranger_paint.png"), optimize=True)
print("paint texture from", os.path.basename(rust_path))

# ------------------------------------------------------------------ export
# OBJ in the mods' convention: the Diamondback root's frame in Unity terms
# (x right, y up, z forward), X mirrored for OBJ. Parts that move with one of
# the Diamondback's own parts (doors, hood, tailgate) are written in that part's
# frame (closed), so in game they're simply parented to it.
import json
import numpy as np
FOLLOW = {
    "DoorD": "cabin_low/cardoor_d",
    "DoorP": "cabin_low/cardoor_p",
    "Hood": "cabin_low/hood_low1",
    "Tailgate": "body_low/tailgate",
    "BullBar": "brushguard_e",
    "DoorDLower": "cabin_low/cardoor_d",
    "DoorPLower": "cabin_low/cardoor_p",
    "TailgateLower": "body_low/tailgate",
    "RainGuardD": "cabin_low/cardoor_d",
    "RainGuardP": "cabin_low/cardoor_p",
    "SunVisor": "cabin_low",
}
ORDER = ["Body", "BodyLower", "Fascia", "Bumper", "Lights", "Glass", "GasDoor", "Hood", "DoorD", "DoorDLower", "DoorP", "DoorPLower", "Tailgate", "TailgateLower",
         "BullBar", "RainGuardD", "RainGuardP", "SunVisor"]
anchors = json.load(open(os.path.join(MODELS, "dpt_anchors.json")))


def export_obj(obj_path, mtl_path):
    out = ["# Junkyard Ranger body: the game's F100 made into a 2004 Ford Ranger, on the Diamondback's chassis.",
           "# Frame: the Diamondback's root (Unity x right, y up, z forward), X mirrored. Groups that follow",
           "# one of the Diamondback's parts are in that part's frame; see ranger.json.",
           "mtllib " + os.path.basename(mtl_path)]
    vb = tb = nb = 1
    used = []
    for group in ORDER:
        objs = [o for o in scene.objects if o.type == "MESH" and o.get("group") == group]
        if not objs:
            continue
        M = np.array(anchors[FOLLOW[group]]["world"]) if group in FOLLOW else np.identity(4)
        Minv = np.linalg.inv(M)
        Nm = np.linalg.inv(Minv[:3, :3]).T
        out.append("o " + group)
        for o in objs:
            me = o.data
            if not me.has_custom_normals:
                me.set_sharp_from_angle(angle=math.radians(34))
            me.calc_loop_triangles()
            normals = me.corner_normals
            uvl = me.uv_layers.active
            mats = [m.name if m else "R_Black" for m in me.materials] or ["R_Black"]
            for tri_mat in {mats[min(p.material_index, len(mats) - 1)] for p in me.polygons}:
                if tri_mat not in used:
                    used.append(tri_mat)
            # positions
            P = np.array([[v.co.x, v.co.z, v.co.y, 1.0] for v in me.vertices]) @ Minv.T
            for x, y, z, _ in P:
                out.append("v %.5f %.5f %.5f" % (-x, y, z))
            uv_index = {}
            n_index = {}
            uv_lines = []
            n_lines = []
            faces_by_mat = {}
            for tri in me.loop_triangles:
                corner = []
                for li in tri.loops:
                    vi = me.loops[li].vertex_index
                    if uvl is not None:
                        u = tuple(round(c, 5) for c in uvl.data[li].uv)
                    else:
                        u = (0.0, 0.0)
                    if u not in uv_index:
                        uv_index[u] = len(uv_lines)
                        uv_lines.append("vt %.5f %.5f" % u)
                    nv = normals[li].vector
                    nu = Nm @ np.array([nv.x, nv.z, nv.y])
                    nl = np.linalg.norm(nu)
                    nu = nu / nl if nl > 0 else nu
                    nk = (round(-nu[0], 3), round(nu[1], 3), round(nu[2], 3))
                    if nk not in n_index:
                        n_index[nk] = len(n_lines)
                        n_lines.append("vn %.3f %.3f %.3f" % nk)
                    corner.append("%d/%d/%d" % (vb + vi, tb + uv_index[u], nb + n_index[nk]))
                faces_by_mat.setdefault(mats[min(tri.material_index, len(mats) - 1)], []).append("f " + " ".join(corner))
            out += uv_lines
            out += n_lines
            for m, fs in faces_by_mat.items():
                out.append("usemtl " + m)
                out += fs
            vb += len(me.vertices)
            tb += len(uv_lines)
            nb += len(n_lines)
    with open(obj_path, "w") as fh:
        fh.write("\n".join(out) + "\n")
    mtl = ["# Materials of ranger.obj. In game the mod uses the game's own material of",
           "# the same name when there is one (F100 / Diamondback); otherwise these values.",
           "# Tex_RGB_23 (the F100 body) is drawn in layers: Tex_RGB_23, R_Paint (ranger_paint.png,",
           "# tinted with the paint color) and Tex_RGB_23Mud, like the F100's own body.", ""]
    for name in used:
        m = bpy.data.materials.get(name)
        col, metal, rough, alpha = (0.6, 0.6, 0.6), 0.0, 0.5, 1.0
        if m and m.use_nodes and "Principled BSDF" in m.node_tree.nodes:
            b = m.node_tree.nodes["Principled BSDF"]
            col = tuple(b.inputs["Base Color"].default_value[:3])
            metal = b.inputs["Metallic"].default_value
            rough = b.inputs["Roughness"].default_value
            alpha = b.inputs["Alpha"].default_value
        mtl += ["newmtl " + name, "Kd %.4f %.4f %.4f" % col, "d %.3f" % alpha, "Pm %.3f" % metal, "Pr %.3f" % rough, ""]
    with open(mtl_path, "w") as fh:
        fh.write("\n".join(mtl))
    print("exported", os.path.basename(obj_path), "materials:", used)
    return used

used = export_obj(os.path.join(OUT, "assets", "ranger.obj"), os.path.join(OUT, "assets", "ranger.mtl"))
meta = {
    "version": 1,
    "base": "dirt pickup truck",
    "follow": FOLLOW,
    "fuelCap": [round(FUEL_CAP.x, 4), round(FUEL_CAP.z, 4), round(FUEL_CAP.y, 4)],
    "layers": {"Tex_RGB_23": ["Tex_RGB_23", "R_Paint", "Tex_RGB_23Mud"]},
    "paintTexture": "ranger_paint.png",
    "hide": ["body_low", "cabin_low"],
    "twoTone": TWO_TONE_Z,
    "accessories": {"SunVisor": {"bolts": [[round(x, 4), round(z, 4), round(y, 4)] for x, y, z in SUN_VISOR_BOLTS]}},
}
json.dump(meta, open(os.path.join(OUT, "assets", "ranger.json"), "w"), indent=1)

# ------------------------------------------------------------------ save
bpy.ops.wm.save_as_mainfile(filepath=os.path.join(OUT, "ranger.blend"))
print("parts:", sorted({o["group"] for o in scene.objects if "group" in o.keys()}))

if RENDER:
    views = RENDER.split(":")[1].split(",") if ":" in RENDER else ["front34", "rear34", "side"]
    # The Diamondback's chassis, wheels and interior underneath.
    skip = ("body_low", "cabin_low", "Element", "tire_low2", "HiJack", "jackBed", "winch", "brushguard", "TireExplosion", "rollbar")
    with bpy.data.libraries.load(DPT_BLEND) as (src, dst):
        dst.objects = list(src.objects)
    models = os.path.dirname(os.path.abspath(DPT_BLEND))
    for img in bpy.data.images:
        if img.filepath.startswith("//tex_links"):
            img.filepath = os.path.join(models, "tex_links", os.path.basename(img.filepath))
    for o in dst.objects:
        if o is None:
            continue
        p = o.get("path", o.name)
        if o.hide_render or p.startswith(skip):
            continue
        scene.collection.objects.link(o)
    # Paint preview: the clean paint layer (game paint mask) over the F100 base.
    paint = bpy.data.materials["Tex_RGB_23"]
    nt = paint.node_tree
    bsdf = nt.nodes["Principled BSDF"]
    base_tex = [n for n in nt.nodes if n.type == "TEX_IMAGE"][0]
    pt = nt.nodes.new("ShaderNodeTexImage")
    pt.image = bpy.data.images.load(os.path.join(os.path.dirname(DPT_BLEND), "ranger_assets", "ranger_paint.png"))
    tint = nt.nodes.new("ShaderNodeMix")
    tint.data_type = "RGBA"
    tint.blend_type = "MULTIPLY"
    tint.inputs["Factor"].default_value = 1.0
    nt.links.new(pt.outputs["Color"], tint.inputs[6])
    tint.inputs[7].default_value = tuple(float(c) for c in os.environ.get("UPPER", "0.78,0.02,0.015").split(",")) + (1,)
    mask = nt.nodes.new("ShaderNodeMath")
    mask.operation = "GREATER_THAN"
    mask.inputs[1].default_value = 0.5
    nt.links.new(pt.outputs["Alpha"], mask.inputs[0])
    over = nt.nodes.new("ShaderNodeMix")
    over.data_type = "RGBA"
    nt.links.new(mask.outputs[0], over.inputs["Factor"])
    nt.links.new(base_tex.outputs["Color"], over.inputs[6])
    nt.links.new(tint.outputs[2], over.inputs[7])
    nt.links.new(over.outputs[2], bsdf.inputs["Base Color"])
    rough = nt.nodes.new("ShaderNodeMix")
    rough.data_type = "FLOAT"
    nt.links.new(mask.outputs[0], rough.inputs["Factor"])
    rough.inputs[2].default_value = 0.6
    rough.inputs[3].default_value = 0.28
    nt.links.new(rough.outputs[0], bsdf.inputs["Roughness"])
    # Two-tone preview: the lower groups (and the bumper) in their own color.
    lower = paint.copy()
    lower.name = "Tex_RGB_23_lower_preview"
    for n in lower.node_tree.nodes:
        if n.type == "MIX" and n.blend_type == "MULTIPLY":
            n.inputs[7].default_value = tuple(float(c) for c in os.environ.get("LOWER", "0.06,0.065,0.07").split(",")) + (1,)
    for o in scene.objects:
        if o.type == "MESH" and (str(o.get("group", "")).endswith("Lower") or o.get("group") == "Bumper"):
            for i, m in enumerate(o.data.materials):
                if m is paint:
                    o.data.materials[i] = lower
    sc = scene
    for img in bpy.data.images:
        if img.filepath and not os.path.exists(bpy.path.abspath(img.filepath)):
            img.filepath = os.path.join(os.path.dirname(DPT_BLEND), "tex_links", os.path.basename(img.filepath))
    sc.render.engine = "CYCLES"
    sc.cycles.samples = int(os.environ.get("SAMPLES", "24"))
    sc.cycles.device = "CPU"
    sc.render.resolution_x = 1200
    sc.render.resolution_y = 750
    sc.view_settings.view_transform = "Standard"
    w = bpy.data.worlds.new("w")
    sc.world = w
    w.use_nodes = True
    w.node_tree.nodes["Background"].inputs[0].default_value = (0.62, 0.72, 0.85, 1)
    w.node_tree.nodes["Background"].inputs[1].default_value = 0.9
    g = bpy.data.meshes.new("ground")
    gb = bmesh.new()
    bmesh.ops.create_grid(gb, x_segments=1, y_segments=1, size=40)
    gb.to_mesh(g)
    go = bpy.data.objects.new("ground", g)
    go.location.z = 0.05
    gm = bpy.data.materials.new("groundm")
    gm.use_nodes = True
    gm.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (0.32, 0.30, 0.27, 1)
    g.materials.append(gm)
    sc.collection.objects.link(go)
    sun = bpy.data.lights.new("sun", "SUN")
    sun.energy = 4.0
    sun.angle = math.radians(8)
    so = bpy.data.objects.new("sun", sun)
    so.rotation_euler = (math.radians(50), math.radians(10), math.radians(140))
    sc.collection.objects.link(so)
    cam = bpy.data.cameras.new("cam")
    cam.lens = float(os.environ.get("LENS", "50"))
    co = bpy.data.objects.new("cam", cam)
    sc.collection.objects.link(co)
    sc.camera = co
    V = {
        "front34": ((5.9, 5.7, 2.5), (0, 0.15, 1.05)),
        "front": ((0.0, 8.5, 1.4), (0, 0, 1.15)),
        "rear34": ((-5.4, -6.2, 2.7), (0, -0.3, 1.05)),
        "side": ((-8.8, -0.1, 1.6), (0, -0.15, 1.1)),
        "nose": ((2.2, 4.4, 1.6), (0, 1.9, 1.15)),
        "fenderL": ((-2.6, 1.0, 1.55), (-0.85, 1.0, 1.3)),
        "driver": ((-0.36, 0.02, 1.80), (-0.30, 2.0, 1.45)),
        "passenger": ((0.30, -0.10, 1.75), (-0.9, 0.9, 1.45)),
        "top": ((0.0, 1.55, 6.0), (0.0, 1.5501, 0.0)),
        "cornerR": ((1.75, 3.2, 1.30), (0.62, 2.0, 1.02)),
        "sideR": ((3.2, 1.75, 1.10), (0.7, 1.75, 1.05)),
        "lowR": ((1.4, 3.4, 0.55), (0.55, 2.0, 0.95)),
        "bay": ((0.0, 2.9, 2.6), (0.0, 1.3, 0.9)),
        "cab": ((-2.5, 1.9, 2.4), (-0.6, 0.45, 1.9)),
    }
    for name in views:
        loc, tgt = V[name]
        bpy.data.objects["fhood"].hide_render = name == "bay"   # the hood up
        co.location = loc
        cam.type = "ORTHO" if name == "top" else "PERSP"
        cam.ortho_scale = 2.6
        co.rotation_euler = (Vector(tgt) - Vector(loc)).to_track_quat("-Z", "Y").to_euler()
        sc.render.filepath = os.path.join(OUT, "r_" + name + ".png")
        bpy.ops.render.render(write_still=True)
        print("rendered", name)
