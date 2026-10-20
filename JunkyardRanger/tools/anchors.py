import sys, json
sys.path.insert(0, '.')
from unitypkg import Package, Prefab
pkg = Package(sys.argv[1] if len(sys.argv) > 1 else 'dpt_raw')
pf = Prefab(pkg, pkg.by_path['Assets/Modded Stuff/V8 NEW/dirt pickup truck.prefab'])
want = ['cabin_low', 'cabin_low/cardoor_d', 'cabin_low/cardoor_p', 'cabin_low/hood_low1', 'body_low', 'body_low/tailgate', 'body_low/cap_low', 'body_low/gastankinput', 'interior_low', 'steeringWheel_low', 'DriverCameraController', 'brushguard_e', 'brushguard_e/bolt']
out = {}
for n in pf.nodes:
    if n['path'] in want:
        out[n['path']] = {'world': n['world'], 'active': n['active']}
for w in want:
    print(w, 'found' if w in out else 'MISSING', out.get(w, {}).get('active'))
json.dump(out, open('dpt_anchors.json', 'w'), indent=1)
