# Truck Exporter (modding tool)

A MelonLoader mod for modders, not for playing: it writes the game's vehicles (the
Diamondback, the F100, the AMC, the golf cart, the dirt bike) to
`<game>/UserData/TruckExport/`, so they can be worked on in Blender.

For each vehicle, a folder with:

* `model.obj` / `model.mtl`: every part as its own group, named by its path in the
  game (`cabin_low/cardoor_d`), in the vehicle's frame (meters, Y up, front +Z, X
  mirrored like our other OBJ files). A group ending in `#off` was hidden at export
  (a part not fitted, an upgrade not bought).
* `hierarchy.txt`: every object with its position, components, colliders, joints
  (door hinges), wheel colliders, and the vehicle script's fields.
* Textures as PNG in `TruckExport/textures/`.

Meshes the game didn't keep readable are copied back from the GPU; only if that
fails does a part go in as its bounding box (the log says how many).

It exports once, a few seconds after a save loads; **F11** exports again.

AssetRipper (on the game's files) gives the same models with more detail; this is
for when that's not handy.
