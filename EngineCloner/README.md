# Engine Cloner (MelonLoader mod for Junkyard Truck)

The engine patch behind extra engines in Junkyard Truck, plus new engines you can
buy. Every extra engine block works on its own: its own physics, and the truck
drives whichever block is bolted into it. Engines you buy (or clone in debug
mode) are saved with your save slot.

## Buying a new engine

New engines are sold in the **Parts Store** (ComputerPartStore mod) and on
**JunkAuto.com** (JunkAuto PC mod, under 1982 Diamondback Pickup → Engine):

| Engine | Block | Default price |
|---|---|---|
| Engine - I4 (new) | `engineblock` | $750 |
| Engine - V8 (new) | `v8_block` | $1500 |
| Engine - 2J inline-6 (new) | `i6block` | $1800 |

A new engine is a **bare block** by default: just the block, rust-free, with every
part off, taken off the same way the game removes a part, so they all bolt back in
as usual. Fit your own parts. With **New engines: bare block** turned off, it's a
complete engine instead: a factory-fresh copy of the **most complete** engine of
that type in your world:
* **Parts:** everything it carries is new (100%, no rust). Parts missing from every
  engine of that type stay missing; buy them separately.
* **Truck parts:** parts the engine scripts track that live on the truck (battery,
  radiator, transmission, diffs) aren't touched.
* **Mounting bolts:** it arrives with its mounting bolts out, ready for the hoist.
  Mount it in a truck like any engine.
* **Oil:** it ships **dry**. Add oil before you start it, or turn on "New engines
  come with oil".

An engine type that isn't anywhere in the level can't be copied, so it isn't for
sale there (the log says so).

**Saving:** bought engines, and debug clones, are saved per save slot in
`UserData/EngineCloner/slotN.txt` (`auto.txt` for the autosave):
* **Placement:** loose, on the engine stand, or bolted in a vehicle.
* **State:** every part's fitted state, condition and bolts, plus the engine's
  condition and fluids.

They're kept out of the game's own save, so they can never overwrite your
original engine's save entry.

## Extra engine stands

**Tool - Engine stand** ($350) is sold next to the engines. It's a copy of the
game's own stand, delivered standing upright on the ground in front of you (or
where JunkAuto drops your order). Use it like the original: drop a block onto it,
click it to turn the engine, and use its release lever.

* **Moving it:** look at a stand you bought, press **K** (setting "Move stand
  key"), look where you want it and press **K** again. It must have no engine on it.
* **Saving:** bought stands are saved with your save slot, and engines on them go
  back onto them.

The game finds "the" stand by name, so with more than one, three things are
patched to use the stand you're actually at:
* **Snapping:** a block snapped onto a stand is jointed to the nearest stand.
* **Turning:** clicking a stand turns that stand's hinge.
* **Releasing:** the release lever frees only the engine on its own stand.

If the game's stand can't be copied safely (it's part of a bigger object), stands
aren't sold, and the log says why.

## No speed limits

The game cuts each vehicle's engine at a fixed top speed: the Diamondback pickup at
85 (100 with a 5th gear), the F100 at 95/100, and the golf cart at 100 (which also
brakes hard). With **Remove speed limits** on (the default), those caps are gone and
vehicles go as fast as their engine, gearing and drag allow. The dirt bike never had
a hard cap. The AMC's only limit is in reverse, and that stays, as does "no power
in neutral". Turning the setting off brings the game's limits back immediately.

How: a Harmony transpiler on each vehicle's `control()` turns every
`maxSpeed = <number>` (other than 0) into `maxSpeed = SpeedLimits.Cap(<number>)`. The
log shows how many limits were patched per vehicle.

## Settings

Mods Menu → ENGINE CLONER, or `[EngineCloner]` in `UserData/MelonPreferences.cfg`:

| Setting | Default | |
|---|---|---|
| New I4 / V8 / 2J inline-6 price | 750 / 1500 / 1800 | Shop prices |
| Engine stand price | 350 | |
| Move stand key | K | Move a stand you bought |
| New engines: bare block | **on** | Off: a complete engine (copy of your most complete one, all parts new) |
| New engines come with oil | off | Off: shipped dry |
| Remove speed limits | **on** | No top-speed cut-off for pickup, F100, golf cart |
| Debug: clone key | **off** | Turns on the clone key below |
| Clone key (debug) | F8 | Copies what you look at (see below) |

## Debug: the clone key

Turn on **Debug: clone key**, then look at an engine block or a vehicle and press
**F8**. (The Junkyard Dumper mod also uses F8, so pick another key if you have it.)

* **Engine:** a loose copy, in its current state, appears in front of you.
* **Vehicle:** a copy is placed on solid ground beside you: in front if there's room,
  otherwise to your right, left or behind. If none of those is clear, the console
  says so; move somewhere more open. Get out of the vehicle first. Looking at an engine that's mounted in a truck clones the
  engine, not the truck, so aim at the body to clone the truck.

Get in any copy by its seat as usual, and you drive that one.

## Why clones used to move together

A loose engine block in Junkyard Truck is never fully free. Its `FixedJoint` stays
connected to a hidden "empty" rigidbody, and the game uses **one shared empty per
engine type**:

| Block          | Shared anchor            |
|----------------|--------------------------|
| `engineblock`  | `EmptyObjRigidbody`      |
| `v8_block`     | `EmptyObjRigidbodyV8`    |
| `i6block`      | `EmptyObjRigidbodyi6`    |
| `250_block`    | `EmptyObjRigidbody250`   |

`PickUp` (grabbing a block), every engine script's `Start()`, Jiggs' relocate menu and
the debug menu all teleport that shared empty to the block and re-connect the joint to
it (see `PickUp.cs`, `engine.cs`, `enginev8.cs`, `enginei6.cs`, `Engine250.cs`,
`JiggsCanvas.cs` in `../Assembly-CSharp`).

`Instantiate` copies the original's `FixedJoint`, so a clone gets connected to the same
body: the shared empty, or the truck or engine stand the original was on. Two blocks
fixed to one body are welded together, so moving one drags the other.

## What the mod does

* **Gives each block its own anchor.** Each block gets a private copy of its type's
  shared empty (same mass, drag and gravity). Every frame, in `LateUpdate` (after all
  game `Update`/`Start`/coroutine code and before the next physics step), any block
  whose joint points at a shared empty, or at another block's private anchor, is moved
  back to its own. So the game can keep using the shared empties and nothing gets
  welded. No game method has to be rewritten for this.
* **Adds a clone hotkey (F8).** Look at an engine block (any part of it works) and
  press F8. A copy appears in front of you, loose, unlocked and free, and it keeps the
  condition of every part it was cloned from. The clone keeps the original's name,
  because the game recognises blocks by name for pickup, the engine stand and
  mounting.
* **Rewires the vehicles to whichever engine is mounted.** On level load, the game
  wires each vehicle to one specific block per engine type: `car.enginescriptv8`,
  `GearBox`, `AudioControl`, `FluidHandler`, `DrainOil`, `Diagnostic`, `Interactor`,
  and `GameObject` fields for parts inside the block such as `engineFan` and
  `engineCrank`. Without the rewiring, a truck with a cloned engine keeps asking the
  original, which is loose, so it reports `canRun = false` and the truck won't start.
  When you mount a different block of the same type, the mod swaps all of those
  references (every scene script plus static fields, matched part-by-part through
  the identical hierarchy) to the mounted block. The MelonLoader console shows
  "Mounted v8_block: rewired N game references to it."
* **Rewires the game to whichever vehicle you get into.** Vehicles have the same
  problem engines had. `Interactor` is wired to one truck (`truck`, `carscript`,
  `seatMount`, `exitMount`...), one F100, one AMC, one golf cart and one dirt bike,
  and so are `GearBox`, `AudioControl`, `Officer`, `Winch`, `FluidHandler` and the
  rest. A Harmony prefix on `GetIn`, `GetInF`, `GetInCar`, `GetInCart` and
  `GetInDirtbike` swaps those references to the vehicle whose seat you clicked,
  just before the game seats you. Parts are matched by name and occurrence, so the
  engine inside each vehicle is swapped along with it.
* **Cuts cloned vehicles loose.** Any joint in the copy that points at something
  outside it (for example a dirt bike strapped into the original truck's bed) is
  removed, so the copy isn't welded to the original.
* **Rescues copies that fall out of the world.** `LostFound` (the out-of-world
  trigger) calls `PhoneScript.Option1b()` and its siblings, which warp the vehicle
  the game is *wired* to home, not the one that fell. They also switch it off and on
  and teleport you, once per collider that enters. A prefix on
  `LostFound.OnTriggerEnter` skips that for any vehicle the game isn't wired to and
  puts that vehicle back where it last came to rest.
* **Puts engines in the right bay.** `PickUp` finds the vehicle to mount an engine
  in with `GameObject.Find("dirt pickup truck")` and similar, which can return the
  wrong copy. A patch on `PickUp.LetGo` hands it the vehicle that owns the bay you
  dropped the engine into.
* **Patches `Interactor.EngReleaseStand`.** The stock version finds blocks with
  `GameObject.Find(name)`, which picks an arbitrary block once there are duplicates.
  The patch releases whichever block is actually on the stand.

The hotkey can be changed in `UserData/MelonPreferences.cfg`:

```
[EngineCloner]
CloneKey = "F8"
```

## Fitting parts to duplicate engines

When you bolt a part onto an engine, the game records it with
`addPart` on whichever engine `GameObject.Find("engineblock")` (or `v8_block`,
`i6block`, `250_block`) returns, and reads the part's health with
`GameObject.Find(partName)`. With clones, both can be the wrong engine. The mod
re-runs `addPart` on the engine that owns the slot the part went into, with
same-named slots on other engines briefly renamed so the game's lookup lands on the
right part. Junkyard ATV ships the same fix; whichever mod loads first runs it.

Only real game engine blocks (they all carry a `FixedJoint`) count as engine blocks.
Engines built into other mods' vehicles, like Junkyard ATV's, are never rewired,
swapped or cloned.

## Build

Requires the .NET SDK (or Visual Studio) and MelonLoader already installed into the game.

```
cd EngineCloner
dotnet build -c Release
```

References come straight from the game folder, by default
`C:\Program Files (x86)\Steam\steamapps\common\Junkyard Truck`. If the game is
somewhere else:

```
dotnet build -c Release -p:GameDir="D:\SteamLibrary\steamapps\common\Junkyard Truck"
```

If the game has a `Mods` folder, the build copies `EngineCloner.dll` into it
automatically. Otherwise, copy `bin/Release/EngineCloner.dll` into `<game>/Mods/`.

Works with MelonLoader 0.6/0.7 (`MelonLoader/net35`). For 0.5.x the project falls back
to `MelonLoader/MelonLoader.dll`, but 0.5 has no `OnInitializeMelon`, so rename it to
`OnApplicationStart`.

## Known limits

* **Clones are not saved.** The save system only knows the original engines and
  vehicles, so clones disappear on reload. It saves whichever copy the game is wired
  to, i.e. the one you last drove or last mounted an engine in. If that's a clone,
  its position and parts are saved as the original's.
* **Copies you aren't driving still run their own scripts.** If the original has its
  engine running when you clone it, the copy starts with its engine running too.
* **Some game logic is still name-based.** Scripts such as `FluidHandler` (oil and
  coolant) and some Interactor checks use `GameObject.Find("engineblock")` and similar,
  so with duplicates they may act on a different block of the same type than the one
  you're working on. This affects game logic only. Physics is fully independent.
* **One running engine of each type at a time.** The game drives a single block per
  engine type, so the most recently mounted one is the one that runs. If a V8 is
  still mounted in one vehicle, a second V8 mounted in another vehicle won't run
  until the first one is taken out and the second is remounted. The same applies to
  the I6. Different engine types are independent.
