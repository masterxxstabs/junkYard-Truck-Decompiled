# Junkyard ATV (MelonLoader mod for Junkyard Truck)

A drivable four-wheel ATV powered by the game's own 250 engine. Separate from Engine
Cloner and Truck Parts QOL; works alongside them.

## Install

Copy `JunkyardATV.dll` into `<game>/Mods/` (MelonLoader 0.6 or newer). On first
launch it creates `<game>/JunkyardATV/atv.cfg`.

## Getting one

* **With the Junkyard Terminal (ComputerPartStore mod):** order
  `Vehicle - ATV (250cc)` from its Parts Store (under *All* and *Dirt Bike Parts*).
* **Without it:** press **F10** to buy one.

Default price is $2500 (`Price` in `MelonPreferences.cfg`). It's delivered on open
ground near you, side-on.

## Riding

| Key | |
|---|---|
| Interact (the game's key, usually **E**) | Get on / off (look at the ATV) |
| **I** | Start / stop the engine |
| **W / S** | Throttle / brake, and reverse when stopped |
| **A / D** | Steer |
| **Space** | Handbrake |
| **F7** | Fit mode (see below) |

All keys are in `MelonPreferences.cfg` under `[JunkyardATV]`. The HUD shows speed,
RPM and fuel.

## The engine

Each ATV has a real copy of the game's 250 dirt bike engine, factory fresh, mounted
under the seat:

* It starts only if the engine says it can (`Engine250.Refresh` / `canRun`): fuel in
  the tank, and no missing or broken parts.
* It burns fuel and wears its parts while running (`DegradeEngine` every 8 seconds,
  like the dirt bike). Worn parts cost power the same way they do on the bike.
* Its parts can be worked on like the bike's, and Truck Parts QOL's OBD scanner
  reads it.
* **Fuel and oil:** the game's fuel nozzle and oil fill it as they do the dirt bike.
  Fuel goes in at the fuel inlet (the yellow marker in fit mode); oil goes in the
  engine's oil filler. The game normally sends 250 fuel/oil to the dirt bike's engine
  (`GameObject.Find("250_block")`); a patch points it at the ATV whose inlet the
  nozzle is in.

The copy is cut loose from the bike: no joint or rigidbody of its own (it's part of
the ATV), it can't be picked up, it's named `250_block_atv` so the game still finds
the bike's engine by name, and `Refresh()` writes its power loss into a stand-in on
the ATV instead of the real dirt bike.

## The model: Suzuki Quadzilla 500

The mod ships with a Suzuki LT500R Quadzilla, rigged from a Tinkercad export
(`quadzilla.obj` + `quadzilla.mtl` in `<game>/JunkyardATV/`). If that file is there
and no other model is set, the mod sets everything up by itself.

Tinkercad merges every shape of one color into one group, so the original had no
separate wheels. `tools/rig_quadzilla.py` rigs it:

* **Splits the wheels.** It finds the four tires (dark grey cylinders), measures each
  wheel's axle, radius and width, and moves every triangle inside each wheel's
  cylinder (tire, rim, hub) into its own part: `Wheel_FL`, `Wheel_FR`, `Wheel_RL`,
  `Wheel_RR`. The handlebars become `Handlebars`; the rest is `Body`.
* **Straightens the wheels.** The model had camber (front wheels 6.5° and 13.2°,
  rears 3-4°). The game spins wheels about a level axle, so a tilted wheel would
  wobble. Each wheel is turned so its axle (the tire's thinnest direction) is level,
  about its own center.
* **Fixes size and orientation.** Tinkercad millimeters (an 8 cm toy) become
  meters: 1.87 m long like the real quad, giving 1.29 × 1.12 m and a 0.27 m wheel
  radius. Z-up becomes Y-up, facing forward. That's a proper rotation, so nothing is
  mirrored.
* **Adds normals.** The surfaces have hard edges kept sharp (creases over 35°)
  instead of everything being smoothed over.
* **Measures** the seat, fuel filler and engine bay positions used in `atv.cfg`.

The handlebars turn about their own center (the mod gives OBJ parts a pivot there).

To rerun it: `python3 tools/rig_quadzilla.py <folder with tinker.obj and obj.mtl>`
(needs numpy).

### Working on the engine

When you bolt a part on, the game records it on the dirt bike's engine
(`GameObject.Find("250_block")`). The mod redirects that to the engine the part
actually went into, so the ATV's and the bike's engines each keep their own parts.
Engine Cloner ships the same fix; whichever mod loads first runs it. A new ATV's
engine starts with every part new, and saves keep each part's condition exactly.

## Using your own model

Put a model file in `<game>/JunkyardATV/` and set `model=` in `atv.cfg`:

* **Formats:** `.glb` / `.gltf` (Sketchfab's default download) or `.obj` + `.mtl`.
  Textures (embedded or next to the file) are loaded. FBX isn't supported: export to
  glb from Blender.
* **Wheels:** parts whose names contain `wheel`, `tire`, `tyre` or `rim`
  (`wheelNames=`) are grouped per corner and spin and steer about their center. The
  wheel colliders' positions and size are measured from them. With no wheel parts,
  the wheels are placed at estimated corners and the model's wheels don't turn.
* **Handlebars:** `handlebars=` names the part that turns with the steering.
* **Hide:** `hide=` lists parts to hide (a rider figure, a stand...).

**Fit mode (F7, standing next to an ATV):** the ATV holds still and shows markers
(green seat, red engine, yellow fuel inlet).

| Key | |
|---|---|
| **[ / ]** | Choose: model position / rotation / scale, wheel size, seat, engine, fuel inlet |
| **Arrows, Page Up / Down** | Adjust (Shift: bigger steps; rotation steps 5°, or 90° with Shift) |
| **R** | Reload `atv.cfg` and the model file (swap models without restarting) |
| **F7** | Save to `atv.cfg` and refit every ATV |

Aim: the model's front points forward, the wheels sit where the colliders are, and
the seat marker is where you want to sit.

With no model file, the ATV uses a simple built-in placeholder.

## Saving

ATVs are saved with the game's save slots, the same way as Truck Parts QOL: saving
slot N writes `UserData/JunkyardATV/slotN.txt` (`auto.txt` for the autosave), and
loading the slot restores each ATV's position and its engine (fuel, oil, coolant,
transmission fluid, and every part's condition and whether it's fitted). A new game
starts without any. Deleting a slot deletes its file.

## Build

```
cd JunkyardATV
dotnet build -c Release
```

References come from the game folder (default Steam path); override with
`-p:GameDir="..."`.
