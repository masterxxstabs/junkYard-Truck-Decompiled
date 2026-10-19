# Making the Ranger's body

The body in the Nexus download (`ranger.obj` and friends) is made from the game's
own trucks with these scripts. They need Blender's Python module (`pip install
bpy`), Pillow and NumPy, and the game's trucks exported with AssetRipper as Unity
packages (the F100, `f1003`, and the Diamondback, `dirt pickup truck`).

1. Untar each package into a folder (`tar -xzf f1003.unitypackage -C f1003_raw`).
2. Turn each truck into a Blender file (every part named by its path in the game,
   with its materials and textures):

   ```
   python3 import_prefab.py f1003_raw "Assets/Modded Stuff/V8 NEW/f1003.prefab" f1003.blend
   python3 import_prefab.py dpt_raw "Assets/Modded Stuff/V8 NEW/dirt pickup truck.prefab" dpt.blend
   ```

3. Note where the Diamondback's doors, hood and tailgate are (`dpt_anchors.json`):

   ```
   python3 anchors.py dpt_raw
   ```

4. Build the Ranger (and optionally render it on the Diamondback's chassis):

   ```
   mkdir -p ranger_assets
   python3 ranger_from_f100.py f1003.blend dpt.blend out render:front34,rear34,side
   ```

   `out/assets/` then has `ranger.obj`, `ranger.mtl`, `ranger.json` and
   `ranger_paint.png`, ready for `UserData/JunkyardRanger/models/`.

What `ranger_from_f100.py` does, step by step, is described at the top of the file:
the F100's body panels are taken off its frame, the 1977 front end comes off, the
body is remapped onto the Diamondback (92% width, 87.5% height, the length remapped
section by section so both axles and the cab line up), the hood slopes down to a
nose with rounded corners, and the 2001-04 front end (swept round the nose's
outline), mirrors and paint layer are added. Extra close-up views for checking the
front end: `nose`, `front`, `cornerR`, `sideR`, `lowR`, `top`.

`unitypkg.py` reads the packages' text-format prefabs, meshes and materials.
