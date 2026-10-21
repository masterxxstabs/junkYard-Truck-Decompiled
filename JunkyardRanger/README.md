# Junkyard Ranger (MelonLoader mod for Junkyard Truck)

A 2004 Ford Ranger regular cab, built from the game's own trucks so it looks and
drives like it belongs:

* **The body is the game's F100**, made into a Ranger: brought down to a Ranger's
  size (1.77 m wide, 2.81 m wheelbase), the 1977 grille, chrome and mirrors off, and
  a 2001-04 front end on: a rounded nose with a domed hood that slopes down to it,
  a chunky body-colored grille surround with the honeycomb and the oval, clear
  headlights that wrap round the corners under the hood's edge (the Diamondback's
  own headlight lenses, two round lamps a side, and its corner lamps), and a big rounded body-colored bumper with the plate, a black intake and
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

**Or spawn one: press F7** (on foot, in the level). A free Ranger appears on open
ground in front of you with an **empty engine bay**, ready for an engine you've built
yourself (a V8, say). Everything else is as below. The key can be changed or turned
off in settings.

A new Ranger from the shops comes:

* with a full tank, fresh fluids, every part new, a hood, and stock wheels;
* with a new engine bolted in: the I4 by default, complete and with oil (see
  settings: V8, 2J or none, complete or a bare block);
* painted two-tone like a 2001-04 Edge: white above, charcoal below the line along
  the sides and on the bumper (see settings).

## Using it

* **Driving:** get in by the door like the Diamondback. The game switches over to
  the Ranger (seat, gauges, gearbox, its engine) when you do.
* **Doors, hood, tailgate:** open and close like the Diamondback's; the Ranger's
  panels move with them.
* **Fuel:** the filler is on the driver's side, behind the rear wheel (where a
  Ranger's is). Open the cap and pour, as usual.
* **Paint:** spray it with the game's paint cans. The cab and bed, the hood, each
  door, the tailgate and the sun visor take paint separately, like the game's trucks. **Two-tone:**
  spray below the line along the sides (through the wheel arches), or the bumper,
  and you paint the lower color instead, all round the truck.
* **Engines:** swap them like on the Diamondback. Engines bought from the shop and
  bolted into a Ranger are saved in it.

## Accessories

All sold in the same shops, and all fitted **the game's way**, like its winch or
brush guard: carry the part to its place until the **snap mark** shows, let go and
it pops on, then tighten any bolts with the ratchet. To take one off, loosen its
bolts and right-click it; you get the loose part back. They fit Rangers only, come
back fitted when you load, and loose ones are saved where they lie.

| Part | Price | Where | Bolts |
|---|---|---|---|
| Ranger - Bull bar | $350 | Front, on the frame | 4, under the bumper |
| Ranger - Rain guards (pair) | $60 | Top of each door window (one fits either door; you get two) | none, they snap on |
| Ranger - Sun visor | $180 | Over the windshield, on the roof; comes in the body color, and paints on its own with the paint cans | 4, on the roof |
| Ranger - Roof light bar (1.7) | $380 | On the roof behind the visor; two work lamps, switched with **G** (the light bar key) | 4, on the roof |
| Ranger - Headache rack (1.7) | $420 | On the bed rails behind the cab: tube frame with a louvered window guard | 4, on the bed rails |
| Ranger - Side steps (pair) (1.7) | $260 | Under each door (one fits either side; you get two) | 2 each, up into the rocker |
| Ranger - Snorkel (1.7) | $300 | Passenger's side, from the fender up the windshield pillar | 2, in the fender |
| Ranger - Performance exhaust (1.7) | $550 | Under the bed: polished muffler and a chrome turndown tip. **An upgrade: the truck pulls 8% harder** while it's on | 2 hangers, up into the bed floor |

The light bar's lamps light the road ahead while they're switched on; the key
switches every Ranger's bar. The exhaust's extra pull is added to whatever engine
is in the truck (it raises the drive torque the game puts on the wheels).

### Bull bar

**Ranger - Bull bar** ($350, same shops) is a black steel grille guard: two
uprights bolted to the frame, a hoop over the grille, and three tubes wrapping
round each headlight. It fits **the game's way**, like its winch or brush guard:

1. Pick it up and carry it to the front of a Ranger. The **snap mark** shows when
   it's in place.
2. Let go: it pops on.
3. Tighten its **4 bolts** (on the frame, under the bumper) with the ratchet.

To take it off, loosen all 4 bolts and pull it off; you get the loose bull bar back.
It fits Rangers only (it's made for the Ranger's front). Fitted ones are saved with
the Ranger, loose ones where they lie.

## Settings

In the Mods Menu (or `UserData/MelonPreferences.cfg`, section `JunkyardRanger`):

| Setting | Default | What it does |
|---|---|---|
| Price | 4500 | Price in the shops |
| Engine | I4 | The engine a new Ranger comes with: I4, V8, I6_2J, Duramax_6_6 (Engine Cloner's 6.6L Duramax) or None |
| Engine: complete | on | On: every part on, ready to run. Off: a bare block to build up |
| Paint color | #E8E8E4 | A new Ranger's color above the two-tone line (#RRGGBB) |
| Two-tone lower color | #3A3D40 | A new Ranger's color below the line and on the bumper |
| Weathered paint | off | On: the paint wears through to rust in places, like the game's other trucks |
| Bull bar price | 350 | Price of the bull bar in the shops |
| Rain guards price | 60 | Price of a pair of rain guards |
| Sun visor price | 180 | Price of the sun visor |
| Light bar / headache rack / side steps / snorkel / exhaust price | 380 / 420 / 260 / 300 / 550 | Shop prices of the 1.7 accessories |
| Light bar key | G | Switches the roof light bar's lamps |
| Spawn key | F7 | Spawns a free Ranger in front of you, no engine in it. None: off |

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
  (base and paint; not the F100's mud layer, which the game dirties with the F100), with the paint layer's texture (`ranger_paint.png`) being the
  game's own paint mask with clean paint. The headlights, corner lamps and fog
  lights use the Diamondback's own lamp materials (`frontLight`, `frontTurn`).
  Trim is made on the game's shader from the MTL.
* **Accessories:** each has a slot like the game's own (a hidden part named
  `..._e` with a solid box, a snap trigger, a durability and maybe bolts). The bull
  bar takes over the Diamondback's empty brush guard slot (`brushguard_e`, renamed
  `brushguardR_e` on a Ranger); the rain guards (`rainguardD_e`, `rainguardP_e`, on
  the doors), the sun visor, light bar, side steps and snorkel (`sunvisor_e`,
  `lightbar_e`, `sidestepD_e`, `sidestepP_e`, `snorkel_e`, on the cab), the headache
  rack and exhaust (`headrack_e`, `rexhaust_e`, on the bed) get new ones, with copies
  of the game's bolts set at the seats in `ranger.json` (and, for the light bar, two
  spot lights). The loose parts are game `PickUp`s with `attachTo` set to
  their slot, and the slot's `durability.template`, so the game's own snap, bolt and
  removal code does the rest.
* **Two-tone:** the body, doors and tailgate are cut at the line in `tools/`, and
  the lower parts (and the bumper) share the "Lower" paint group; a spray hitting
  below the line (`twoTone` in `ranger.json`) paints that group.
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
