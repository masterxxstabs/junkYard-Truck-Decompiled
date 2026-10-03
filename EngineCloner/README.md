# Engine Cloner (MelonLoader mod for Junkyard Truck)

Clone any engine block (4-cyl, V8, I6, 250) or vehicle (Diamondback pickup, F100,
AMC, golf cart, dirt bike) and use each copy on its own.

## Usage

Look at an engine block or a vehicle and press **F8**.

* **Engine:** a loose copy appears in front of you.
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
