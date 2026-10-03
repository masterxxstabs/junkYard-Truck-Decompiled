# Engine Cloner (MelonLoader mod for Junkyard Truck)

Clone any engine block (4-cyl, V8, I6, 250) and move each copy around on its own.

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

* **Clones are not saved.** The save system only knows the original blocks, so clones
  disappear on reload.
* **Some game logic is still name-based.** Scripts such as `FluidHandler` (oil and
  coolant) and some Interactor checks use `GameObject.Find("engineblock")` and similar,
  so with duplicates they may act on a different block of the same type than the one
  you're working on. This affects game logic only. Physics is fully independent.
* **One engine per truck.** A truck tracks a single engine, so mounting a clone in a
  truck that already has one installed is not supported.
