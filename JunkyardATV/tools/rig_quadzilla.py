"""Rig the Tinkercad Suzuki Quadzilla 500 for Junkyard ATV.

Tinkercad merges every shape of one color into one group, so the wheels aren't
separate. This finds the four tires (dark grey cylinders), assigns every face
inside each wheel's cylinder (tire, rim, hub) to that wheel, splits out the
handlebars and the stock engine, converts Z-up millimeters to Y-up meters facing forward, adds
crease-aware normals, and writes quadzilla.obj/.mtl.

Usage: python3 rig_quadzilla.py <folder with tinker.obj and obj.mtl>
Writes quadzilla.obj and quadzilla.mtl to the current folder (needs numpy).
"""
import math, collections, json, sys
import numpy as np

import os
HERE = sys.argv[1] if len(sys.argv) > 1 else '.'
SRC = os.path.join(HERE, 'tinker.obj')
LENGTH_M = 1.87  # Suzuki LT500R Quadzilla overall length

V = []; faces = []  # faces: (verts, material)
mtl = None
for line in open(SRC):
    if line.startswith('v '):
        p = line.split(); V.append((float(p[1]), float(p[2]), float(p[3])))
    elif line.startswith('usemtl'):
        mtl = line.split()[1]
    elif line.startswith('f '):
        faces.append(([int(t.split('/')[0]) - 1 for t in line.split()[1:]], mtl))
V = np.array(V)
F = [f for f, _ in faces]; M = [m for _, m in faces]
cent = np.array([V[f].mean(axis=0) for f in F])
lo, hi = V.min(axis=0), V.max(axis=0)
cx = (lo[0] + hi[0]) / 2
cy = (lo[1] + hi[1]) / 2
print('bounds', lo, hi)

TIRE = 'color_2829873'
# Tire faces: dark, low, out at the sides.
is_tire = np.array([m == TIRE for m in M]) & (cent[:, 2] < 25) & (np.abs(cent[:, 0] - cx) > 14)
wheels = {}
for side in (-1, 1):
    for end in (-1, 1):
        sel = is_tire & (np.sign(cent[:, 0] - cx) == side) & (np.sign(cent[:, 1] - cy) == end)
        vs = np.unique(np.concatenate([F[i] for i in np.nonzero(sel)[0]]))
        p = V[vs]
        mn, mx = p.min(axis=0), p.max(axis=0)
        # Front is -Y (handlebars sit at -Y). Vehicle's left is +X here.
        name = 'Wheel_' + ('F' if end < 0 else 'R') + ('L' if side > 0 else 'R')
        wheels[name] = dict(xmin=mn[0], xmax=mx[0], cy=(mn[1] + mx[1]) / 2, cz=(mn[2] + mx[2]) / 2,
                            r=max(mx[1] - mn[1], mx[2] - mn[2]) / 2)
        w = wheels[name]
        print(f"{name}: x {w['xmin']:.1f}..{w['xmax']:.1f}  axle y {w['cy']:.1f} z {w['cz']:.1f}  radius {w['r']:.2f}  ({sel.sum()} tire faces)")

# The Quadzilla's own engine (crankcase, covers, cylinder, head and carb) is
# fused into the grey frame mesh. Carve it out by region into its own part so
# the mod can hide it and the game's 250 sits in the empty bay. Boxes are
# Tinkercad mm (x, y, z ranges; front is -Y); the side frame rails run at
# x < 3.4 and x > 14.8, the lower rail below z 12.2, the down tubes in front of
# y -11.6 and the swingarm pivot behind y 5.6, so those stay on the Body.
FRAME = 'color_12568524'
STOCK_ENGINE = [
    (2.3, 15.8, -11.6, 5.4, 12.2, 22.4),  # crankcase, side covers, cylinder
    (3.6, 14.6, -10.5, 5.5, 22.4, 28.0),  # head, valve cover, carb, intake boot
]
def in_stock_engine(c):
    if c[2] < 14 and c[1] < -9.5:
        return False  # foot of the front down tubes
    return any(b[0] <= c[0] <= b[1] and b[2] <= c[1] <= b[3] and b[4] <= c[2] <= b[5] for b in STOCK_ENGINE)

part = []
for i in range(len(F)):
    c = cent[i]; who = 'Body'
    for name, w in wheels.items():
        if w['xmin'] - 0.3 <= c[0] <= w['xmax'] + 0.3 and math.hypot(c[1] - w['cy'], c[2] - w['cz']) <= w['r'] + 0.15:
            who = name; break
    if who == 'Body' and c[2] > 39.5 and c[1] < -4:
        who = 'Handlebars'
    if who == 'Body' and M[i] == FRAME and in_stock_engine(c):
        who = 'StockEngine'
    part.append(who)
print(collections.Counter(part))

# Z-up Tinkercad mm -> Y-up meters, front toward +Z, origin at ground center.
# (x, y, z) -> (x, z, -y): a proper rotation, so nothing gets mirrored; the
# mod's loader then mirrors X going into Unity's left-handed space.
s = LENGTH_M / (hi[1] - lo[1])
def out(p):
    return np.array([(p[0] - cx) * s, (p[2] - lo[2]) * s, -(p[1] - cy) * s])
VO = np.array([out(p) for p in V])

# Wheels: modelled with camber (tilted). The game spins each wheel about a
# horizontal axle, so straighten each one about its own center: its axle is the
# tire's thinnest direction (smallest principal axis of the tire's vertices).
PARTS = ['Body', 'Handlebars', 'StockEngine', 'Wheel_FL', 'Wheel_FR', 'Wheel_RL', 'Wheel_RR']
objects = {}  # name -> (local vertex array, list of (local face, material))
for name in PARTS:
    ids = [i for i in range(len(F)) if part[i] == name]
    used = sorted(set(v for i in ids for v in F[i]))
    remap = {v: k for k, v in enumerate(used)}
    verts = VO[used].copy()
    if name.startswith('Wheel'):
        tire = [remap[v] for i in ids if M[i] == TIRE for v in F[i]]
        p = verts[sorted(set(tire))]
        center = p.mean(axis=0)
        w, vecs = np.linalg.eigh(np.cov((p - center).T))
        axis = vecs[:, 0]
        if axis[0] < 0:
            axis = -axis
        target = np.array([1.0, 0.0, 0.0])
        angle = math.degrees(math.acos(max(-1.0, min(1.0, float(np.dot(axis, target))))))
        k = np.cross(axis, target); kn = np.linalg.norm(k)
        if kn > 1e-9:
            k /= kn; th = math.radians(angle)
            K = np.array([[0, -k[2], k[1]], [k[2], 0, -k[0]], [-k[1], k[0], 0]])
            R = np.eye(3) + math.sin(th) * K + (1 - math.cos(th)) * K @ K
            verts = (verts - center) @ R.T + center
        # Sit the straightened wheel back on the ground plane.
        bottom = verts[:, 1].min()
        print(f"{name}: tilted {angle:.1f} deg, straightened; lowest point {bottom:.3f} m")
    objects[name] = (verts, [([remap[v] for v in F[i]], M[i]) for i in ids])

# Crease-aware normals per part: average face normals around a vertex, but
# only across edges flatter than 35 degrees.
cos_crease = math.cos(math.radians(35))
normals = []; nindex = {}
def part_normals(verts, flist):
    fn = []
    for f, _ in flist:
        a, b, c = verts[f[0]], verts[f[1]], verts[f[2]]
        n = np.cross(b - a, c - a); l = np.linalg.norm(n)
        fn.append(n / l if l > 0 else np.array([0, 1, 0]))
    vf = collections.defaultdict(list)
    for i, (f, _) in enumerate(flist):
        for v in f:
            vf[v].append(i)
    result = []
    for i, (f, _) in enumerate(flist):
        ids = []
        for v in f:
            acc = np.zeros(3)
            for j in vf[v]:
                if np.dot(fn[i], fn[j]) >= cos_crease:
                    acc += fn[j]
            l = np.linalg.norm(acc); n = acc / l if l > 0 else fn[i]
            key = tuple(np.round(n, 3))
            if key not in nindex:
                nindex[key] = len(normals); normals.append(key)
            ids.append(nindex[key])
        result.append(ids)
    return result
fnorm = {name: part_normals(*objects[name]) for name in PARTS}

names = {'color_2829873': 'Rubber', 'color_12568524': 'Metal', 'color_16768282': 'SuzukiYellow', 'color_24813': 'Blue'}
with open('quadzilla.mtl', 'w') as m:
    m.write('# Suzuki Quadzilla 500 (rigged for Junkyard ATV)\n')
    for line in open(os.path.join(HERE, 'obj.mtl')).read().split('newmtl')[1:]:
        key = line.split()[0]
        kd = [l for l in line.splitlines() if l.strip().startswith('Kd')][0].strip()
        ns = {'Rubber': 40, 'Metal': 600, 'SuzukiYellow': 450, 'Blue': 450}[names[key]]
        m.write(f'\nnewmtl {names[key]}\n{kd}\nNs {ns}\nd 1\n')

with open('quadzilla.obj', 'w') as o:
    o.write('# Suzuki Quadzilla 500, rigged for Junkyard ATV\n# Y up, meters, front toward +Z. Parts: Body, Handlebars, StockEngine, Wheel_FL/FR/RL/RR\nmtllib quadzilla.mtl\n')
    for n in normals:
        o.write(f'vn {n[0]} {n[1]} {n[2]}\n')
    base = 0
    for name in PARTS:
        verts, flist = objects[name]
        o.write(f'o {name}\n')
        for p in verts:
            o.write(f'v {p[0]:.5f} {p[1]:.5f} {p[2]:.5f}\n')
        bymat = collections.defaultdict(list)
        for i, (f, mat) in enumerate(flist):
            bymat[mat].append(i)
        for mat, ids in bymat.items():
            o.write(f'usemtl {names[mat]}\n')
            for i in ids:
                f = flist[i][0]
                o.write('f ' + ' '.join(f'{base + v + 1}//{n + 1}' for v, n in zip(f, fnorm[name][i])) + '\n')
        base += len(verts)

# Points of interest for atv.cfg, in the output (Unity) frame.
def u(p):  # output frame -> Unity (loader mirrors X)
    q = out(np.array(p)); return [-q[0], q[1], q[2]]
blue = np.array([m == 'color_24813' for m in M])
seat_sel = blue & (cent[:, 1] > 0) & (cent[:, 1] < 20)
seat_top = cent[seat_sel][:, 2].max()
seat = u([cx, 8, seat_top])
tank_sel = (np.array([m in ('color_24813', 'color_16768282') for m in M])) & (cent[:, 1] > -14) & (cent[:, 1] < -4) & (np.abs(cent[:, 0] - cx) < 4)
tank_top = cent[tank_sel][:, 2].max()
fuel = u([cx, -9, tank_top])
# The empty engine bay: the biggest clear box (no vertex of what's left inside)
# grown out from the middle of where the stock engine was. The engine point is
# the middle of its floor; the mod sits the 250's lowest point there.
import itertools
eng_faces = [i for i in range(len(F)) if part[i] == 'StockEngine']
rest = V[np.unique(np.concatenate([F[i] for i in range(len(F)) if part[i] != 'StockEngine']))]
ev = V[np.unique(np.concatenate([F[i] for i in eng_faces]))]
seed = (ev.min(axis=0) + ev.max(axis=0)) / 2
near = rest[np.all((rest > seed - 30) & (rest < seed + 30), axis=1)]
def clear(a, b):
    return not np.any(np.all((near > a) & (near < b), axis=1))
bay = None
for order in itertools.permutations(range(6)):
    a, b = seed - 0.5, seed + 0.5
    grew = True
    while grew:
        grew = False
        for k in order:
            a2, b2 = a.copy(), b.copy()
            if k % 2 == 0:
                a2[k // 2] -= 0.1
            else:
                b2[k // 2] += 0.1
            if clear(a2, b2):
                a, b, grew = a2, b2, True
    if bay is None or np.prod(b - a) > np.prod(bay[1] - bay[0]):
        bay = (a, b)
a, b = bay
engine = u([(a[0] + b[0]) / 2, (a[1] + b[1]) / 2, a[2]])
bay_size = ((b - a) * s)[[0, 2, 1]]  # width, height, length in meters
print(f"stock engine: {len(eng_faces)} faces; empty bay {bay_size[0]:.3f} wide x {bay_size[1]:.3f} tall x {bay_size[2]:.3f} long (m)")
radius = max(w['r'] for w in wheels.values()) * s
info = dict(scale=s, seat=seat, fuel=fuel, engine=engine, bay=bay_size, wheelRadius=radius,
            size_m=list((hi - lo)[[0, 2, 1]] * s), parts=collections.Counter(part))
print(json.dumps(info, default=lambda o: o.tolist() if hasattr(o, 'tolist') else str(o), indent=1))
