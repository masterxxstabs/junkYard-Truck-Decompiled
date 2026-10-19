# Junkyard Ranger (MelonLoader mod for Junkyard Truck)

A 2004 Ford Ranger regular cab, built from the game's own trucks so it looks and
drives like it belongs:

* **The body is the game's F100**, made into a Ranger: brought down to a Ranger's
  size (1.77 m wide, 2.81 m wheelbase), the 1977 grille, chrome and mirrors off, and
  a 2001-04 front end on: a rounded nose with a domed hood that slopes down to it,
  a chunky body-colored grille surround with the honeycomb and the oval, clear
  headlights that wrap round the corners under the hood's edge (twin lamps, amber
  corners), and a big rounded body-colored bumper with the plate, a black intake and
  fog lights; black mirrors. It uses the game's own textures and grime.
* **Underneath it's the Diamondback**: a copy of the game's truck, so it drives the
  same and takes the game's engines (I4, V8, 2J inline-6), gearbox, wheels and
  tires, and every part you can fit to the Diamondback. Its dash, seats and steering
  wheel are inside the Ranger's cab.

**Requires Engine Cloner 2.4 or newer** (it makes the copy work as a second truck).

## Getting one

**Vehicle - 2004 Ford Ranger** ($4500) is sold on **JunkAuto.com** (under Vehicles)
and in the **Parts Store** (Junkyard Terminal mod). It's delivered on open ground
near where the shop delivers. If you're sitting in a vehicle at the time, it arrives
when you get out.

A new Ranger comes:

* with a full tank, fresh fluids, every part new, a hood, and stock wheels;
* with a new engine bolted in: the I4 by default, complete and with oil (see
  settings: V8, 2J or none, complete or a bare block);
* painted Ranger red (see settings).

## Using it

* **Driving:** get in by the door like the Diamondback. The game switches over to
  the Ranger (seat, gauges, gearbox, its engine) when you do.
* **Doors, hood, tailgate:** open and close like the Diamondback's; the Ranger's
  panels move with them.
* **Fuel:** the filler is on the driver's side, behind the rear wheel (where a
  Ranger's is). Open the cap and pour, as usual.
* **Paint:** spray it with the game's paint cans. The cab and bed, the hood, each
  door and the tailgate take paint separately, like the game's trucks.
* **Engines:** swap them like on the Diamondback. Engines bought from the shop and
  bolted into a Ranger are saved in it.

## Settings

In the Mods Menu (or `UserData/MelonPreferences.cfg`, section `JunkyardRanger`):

| Setting | Default | What it does |
|---|---|---|
| Price | 4500 | Price in the shops |
| Engine | I4 | The engine a new Ranger comes with: I4, V8, I6_2J or None |
| Engine: complete | on | On: every part on, ready to run. Off: a bare block to build up |
| Paint color | #C8102E | A new Ranger's color (#RRGGBB) |
| Weathered paint | off | On: the paint wears through to rust in places, like the game's other trucks |

## Saving

Rangers are saved with your save slot in `UserData/JunkyardRanger/slotN.txt`
(`auto.txt` for the autosave): where each one is, its paint, and the state of every
part. Deleting a slot, or overwriting it with a new game, clears them too.

## Install

1. Engine Cloner 2.4+ in `Mods`.
2. `JunkyardRanger.dll` in `Mods`.
3. The body files in `UserData/JunkyardRanger/models/`: `ranger.obj`, `ranger.mtl`,
   `ranger.json`, `ranger_paint.png` (in the download).

## How it works

* **The truck:** Engine Cloner's `VehicleApi` makes an inactive copy of the
  Diamondback, takes its engine out, and the copy is changed before any of its
  scripts run: the Diamondback's body panels are hidden from view (`body_low`,
  `cabin_low`; their colliders, doors, hood and lights keep working), the Ranger
  body goes on, and the fuel inlet and lid move to the driver's side. Then it's
  started where it's delivered, tagged with its id, and an engine is bolted in.
* **The body:** `ranger.obj` is in the Diamondback's frame. The doors, hood and
  tailgate are in the frame of the Diamondback's own door, hood and tailgate, and are
  hung on them, so they swing with them.
* **Materials:** the game's own, by name: the F100's body, drawn in its layers
  (base, paint, mud), with the paint layer's texture (`ranger_paint.png`) being the
  game's own paint mask with clean paint. Lamps and trim are made on the game's
  shader from the MTL.
* **Painting:** `Interactor.PaintSurface` is patched so a spray at a Ranger paints
  the Ranger (and is saved with it), not the Diamondback's saved colors.

The body is made from the game's F100 by the scripts in `tools/` (Blender), from
the game's assets; see `tools/README.md`.

## Known limits

* **One Diamondback is needed** in the level to build Rangers on (it's always there
  in the game).
* **Some game logic finds the truck by name** (`GameObject.Find("dirt pickup
  truck")`). Engine Cloner covers getting in, mounting engines and fuel; other
  things (some of the Diamondback's upgrades bought at the garage) may apply to the
  truck you last drove.
* **Collisions use the Diamondback's shapes.** They're close to the Ranger's size,
  but not identical.
