# Truck Parts QOL (MelonLoader mod for Junkyard Truck)

Extra parts for your vehicles, in the spirit of My Summer Car:

* **Stereo:** a CD head unit, speakers and a subwoofer, playing CDs burned from
  your own music or the game's FM radio.
* **Bed covers:** a tarp, a tri-fold tonneau cover or a hard top (camper shell),
  fitted to your pickup's bed. *(Work in progress: fitment is being reworked.)*
* **OBD scanner:** a handheld tool on key **8**, next to the game's own tools,
  that reads the condition of every part on an engine.
* **Paintable rims and turbos:** spray them with the game's paint cans, like body
  panels and valve covers.

Formerly **Truck Stereo**. Separate from the Engine Cloner mod; install either or
both.

## Upgrading from Truck Stereo

**Delete `Mods/TruckStereo.dll`.** Truck Parts QOL replaces it. If both are
installed, the console shows an error and every part is doubled. On first launch:

* `TruckStereo/` (your CD folders) is moved to `TruckPartsQOL/`.
* `UserData/TruckStereo.txt` (your installed parts) is moved to
  `UserData/TruckPartsQOL.txt`.
* Any keys or prices you changed under `[TruckStereo]` in `MelonPreferences.cfg` are
  copied to `[TruckPartsQOL]`.

## Install

Copy `TruckPartsQOL.dll` into `<game>/Mods/` (MelonLoader 0.6 or newer).

On first launch the mod creates a `TruckPartsQOL` folder in the game directory:

```
Junkyard Truck/
  TruckPartsQOL/
    CD1/   <- your music for CD 1
    CD2/
    CD3/   (you can add folders up to CD9)
```

Each folder is one CD, played in file-name order (name them `01 ...`, `02 ...` to
control the order). Use **.ogg** or **.wav**. MP3 decoding depends on the game's
Unity version; if a track shows `READ ERROR`, convert it to .ogg.

**3D models:** copy the `UserData/TruckPartsQOL/models/` folder from the download
into the game's `UserData` folder (`speaker.obj`, `amp.obj`, their `.mtl` files and
the amp's `amp_tex*.jpg` textures). Without it the speakers and amp use a simple
built-in look. `tools/convert_audio_models.py` rebuilds them from the source
models (a free-to-use door speaker `.blend` and a D4S amp `.usdz`).

## Buying parts

**With the Junkyard Terminal (ComputerPartStore mod) installed**, everything is
sold in its **Parts Store** tab, listed under *All* and *Truck Parts* as
`Stereo - ...` and `Bed - ...`. Order them like any other part; they're delivered in front of you.
The CD entries follow your `CD` folders: add music to a new folder and its CD
appears within a few seconds.

**Without it**, press **F9** for the mod's own small shop.

Either way, items are paid from your wallet like any part.

| Part | Default price | Notes |
|---|---|---|
| CD head unit | $150 | 1-DIN unit with display, CD slot, FM radio |
| 6.5" door speaker | $35 | Sits in the door panel with only its frame proud; cuts deep bass |
| 12" subwoofer box | $120 | Bass only, heavy (14 kg); needs an amp for full output |
| 4-channel amplifier | $220 | Drives the speakers harder and the sub at full output |
| CD *n* | $5 | One per non-empty `CD` folder |
| Bed tarp | $40 | Strapped-down tarp; rolls up toward the cab |
| Tonneau cover | $250 | Tri-fold hard cover; folds up against the cab |
| Hard top | $600 | Camper shell to cab height, with a lifting rear glass hatch |
| OBD scanner | $80 | Tool: pick up the box and it joins your tools on key 8 |

## Fitting

* **Pick up** a part with the normal left click.
* **Install:** hold it, look at the spot on the vehicle where you want it (dash,
  doors, rear deck, bed, floor...) and press **Y**. It mounts flush to that surface,
  facing out; door speakers sink into the panel. Mounted on a door, it swings with
  the door. On a floor the amp lies lengthwise along the vehicle.
* **Bolt it down** like any part in the game: take out the **ratchet (tool 2)**, aim
  at each of the part's bolts and **scroll up** until it's tight (the bolts glow
  when aimed at). Head units have 2 bolts, speakers, the sub and the amp 4. The
  hint shows how many are tight. A part that's barely bolted **falls off** once
  the vehicle gets going.
* **Remove:** **scroll down** on its bolts with the ratchet until they're all out,
  then look at the part and press **Y**. It drops loose.

The bolts are the game's own kind (its ratchet turns them), so they work and feel
exactly like vanilla bolts. Parts fitted before bolts existed come back tight.
* **CDs:** hold a CD, look at the head unit and press **Y** to insert it.

The head unit plays through **every speaker installed in the same vehicle**, all in
sync. With no speakers you only hear its tiny built-in speaker. It needs the
vehicle's battery: a flat battery means no music.

**Amplifier:** with an amp fitted in the same vehicle, the speakers play about a
third louder and the subwoofer gets its full output. Without one, a sub only gets a
weak signal (half volume).

## OBD scanner

The game's tools sit on the number keys: 1 hands, 2 ratchet, 3 multimeter,
4 depth gauge, 5 phone, 6 tire gauge, 7 crowbar. The scanner goes on **8**. Buy it
(`Tool - OBD scanner`) and pick up its box; from then on it's yours (for every save,
since it's stored in `MelonPreferences.cfg`). `ScannerFree = true` skips buying it.

Press **8** to take it out. Aim at a vehicle, or at an engine on the stand or the
floor, and it links to that engine. The readout stays up while you work. Pressing
any of 1-7 (or 8 again) puts it away.

The readout shows:

* **Status:** whether the engine will run and crank, and the check-engine light, as
  of the game's last check (it checks when you get in, for example).
* **Oil level** and any **torque lost** to worn parts.
* **Trouble codes,** made up from live part condition in OBD-II style: cylinder
  misfire (P0301...) for a bad piston, knock (P0325) for worn bearings, timing
  correlation (P0016), low voltage (P0562), alternator (P0620), restricted air
  filter (P0101), overheat from the head gasket (P0217), ignition (P0340), lean
  intake or carburetor (P0171), oil filter (P0521) and low oil (P0520).
* **Every part** on the engine, worst first, with a color-coded condition bar.
  Missing parts are listed at the top. Scroll with the mouse wheel.

Readings are live: the scanner reads each part's `durability.health` straight from
the engine script (`engine`, `enginev8`, `enginei6`, `Engine250`). It never calls the
game's `Refresh()`, which changes battery charge and the truck's torque as a side
effect.

## Painting rims and turbos

Use a paint can on a rim or a turbo the same way you paint the truck: the color,
metallic and gloss come from the can. Rims can be painted mounted or loose; only the
rim is painted, never the tire. Turbos can be painted fitted to the engine or loose.

**Turbos** (`turbo`, `turboXL`, the V8's aftermarket turbos, loose turbos) are
painted exactly the way the game paints a valve cover: through the paint fields on
the part's `durability` slot or `PickUp`. So the game itself carries the paint
across when you unbolt or fit the turbo, and saves it with your game.

**Rims** have no paint fields in the game, so the mod keeps track of them:

* A wheel's models (on a loose wheel: child 0 the stock rim, 1-5 the other rims,
  6+ the tires; on a mounted wheel, the slot's `WHEEL_HOLDER`, laid out the same
  way) are switched on and off to show the fitted rim and tire. The paint goes on
  the active rim model.
* Mounting a painted wheel (`PickUp.LetGo`) and taking one off (`Interactor.Update`
  spawning the loose wheel) carry the rim's paint across. Mounting an unpainted rim
  where a painted one was restores the original finish.
* Rim paint is saved with your game save (see Saving): rims on a vehicle by their
  place on it; loose wheels by name and position) and restored when the level
  loads.

Paint is applied by a prefix on `Interactor.PaintSurface`; anything that isn't a rim
or a turbo still goes through the game's own painting code.

## Bed covers

Bed covers come boxed. Carry the box to a pickup (the Diamondback or the F100),
look at the truck and press **Y**. The cover fits itself to that bed. One cover per
bed.

| Key | Action |
|---|---|
| **O** | Open / close: roll the tarp up, fold the tonneau against the cab, or lift the hard top's rear hatch |
| **Y** | Remove the cover. It goes back in its box, which drops into the bed |
| **Page Up / Page Down** | Raise / lower the fitted cover 1 cm (hold Shift for 5 cm). Saved per cover |

Each fit writes what it measured to the MelonLoader console (bed zone, floor, rail
height, which end is the cab, plus the colliders the floor probe hit). If a cover
sits wrong on some vehicle, that log line shows why.

A closed cover is solid: cargo can't bounce out, and you can stack things on a
tonneau. It won't close on cargo sticking up above the rails ("Something's
sticking up in the way"). Likewise, a hard top won't go on until the bed is clear
above the rails. You can mount stereo parts on a hard top's walls too.

**How the fit works:** each pickup's bed has a `TruckBedGrav` zone (the game uses
it to find cargo to strap down). The cover reads that zone's size, raycasts for the
bed floor, the rail tops and the cab roof, and builds itself to match, so it works
on any pickup, including Engine Cloner copies. Fitted, its colliders become part of
the truck. The truck's center of mass is kept where the game set it, and the thin
panels add next to nothing to its inertia.

## Head unit controls

Look at the installed head unit:

| Key | Action |
|---|---|
| **P** | Power on / off |
| **M** | Switch CD / FM radio |
| **N** / **B** | Next / previous track (CD) or station (radio) |
| **Mouse wheel** | Volume |
| **J** | Eject the CD (it pops out of the slot) |
| **Y** | Remove the head unit |

FM radio plays the songs the game ships for its own radios, tuned in mid-song.

All keys can be changed in `UserData/MelonPreferences.cfg` under `[TruckPartsQOL]`,
and so can the prices (`HeadUnitPrice`, `SpeakerPrice`, `SubwooferPrice`,
`CdPrice`, `TarpPrice`, `TonneauPrice`, `HardTopPrice`, `ScannerPrice`). The scanner
key is `ScannerKey` (default `Alpha8`). `FreeParts = true` makes everything free, in both shops.

## Saving

Truck Parts QOL saves **with the game's save slots**: when you save the game to slot
1, 2 or 3 (or the game autosaves), the mod saves its parts with it, and loading that
slot restores them. A new game starts without any.

What's saved: every stereo part, bed cover and parts box (installed parts with their
exact spot on the vehicle, loose parts where they lie), head unit power, volume,
mode, track and the CD in the slot, bed cover open/closed and height, painted rims,
and whether you own the OBD scanner. Turbo paint is saved by the game itself.

Files, in `UserData/TruckPartsQOL/`:

| Game slot | ES3 file | Truck Parts QOL file |
|---|---|---|
| 1 | `JY.es3` | `slot1.txt` |
| 2 | `JY2.es3` | `slot2.txt` |
| 3 | `JY3.es3` | `slot3.txt` |
| Autosave | `JYAuto.es3` | `auto.txt` |

* Like the game, quitting without saving keeps your last save.
* Deleting a slot in the menu (or starting a new game over slot 1) deletes its
  Truck Parts QOL file too.
* Each save and load is logged in the MelonLoader console, e.g.
  `Saved 5 part(s) and 2 painted rim(s) with save slot 1.`

* Starting a new game over slot 1 ("overwrite") starts without any parts, even
  though the game leaves its last-loaded slot setting as it was.

**Upgrading:** older versions kept everything in one file,
`UserData/TruckPartsQOL.txt`. The first time you load a slot, that file is loaded
into it once and renamed to `TruckPartsQOL.txt.old`. Save the game to keep the
parts in that slot.

How it works: the mod hooks the game's save calls (`MainMenu.OptionSave`,
`OptionSave2`, `OptionSave3`, `OptionSaveAuto`, and Easy Save's
`ES3AutoSaveMgr.Save1/2/3/SaveAuto`) and saves in a prefix, while everything is
still in the world. It reads the slot being loaded from the game's
`PlayerPrefs "LoadSlot"` (0 = new game). Parts are restored once per level, as soon
as its vehicles exist. Nothing is saved before that, so a save can't wipe the file.
Files are written to a temp file first, then swapped in.

## How it fits into the game

* **Carrying** uses the game's own `PickUp` component, so parts handle like any other
  part. A `PickUp` added from code starts with `null` strings and arrays, where
  scene-loaded ones have `""` and empty arrays; the mod fills them in the same way,
  because `PickUp` relies on that (e.g. `attachTo != ""`).
* **Installed parts** are parented to the vehicle with their rigidbody removed and
  their colliders turned into triggers. They add no mass and don't collide with the
  vehicle, but you can still look at them.
* **Power** comes from the vehicle script's `canAcc` flag, which the engine sets from
  the battery (`engine.Refresh`).
* **Models** are built from Unity primitives at runtime, so there are no asset
  bundles.
* **Parts Store integration** is a Harmony postfix on
  `ComputerFeatures.PartsStoreService.GetCatalog()`. It appends `PartEntry` items
  (prefab, name, price, category) to the returned list, whether that's the cached
  catalog or the fresh list built before the junkyard loads, and re-sorts it the way
  the store does. Everything goes through reflection, so Truck Parts QOL still loads
  without the store. The prefabs are dormant templates under an inactive
  `DontDestroyOnLoad` object, because `TryPurchase` instantiates the prefab and the
  store caches its catalog across levels. The store drops orders at foot level, so
  loose stereo parts lift themselves clear of the ground when they spawn.
* Works with Engine Cloner: clone a vehicle with a stereo and the copy gets its own
  working stereo. Parts on a clone are saved against the vehicle's name, so after a
  reload they appear in the original.

## Build

```
cd TruckPartsQOL
dotnet build -c Release
```

Same setup as Engine Cloner. References come from the game folder (default Steam
path); override with `-p:GameDir="..."`.
