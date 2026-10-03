# Truck Parts QOL (MelonLoader mod for Junkyard Truck)

Extra parts for your vehicles, in the spirit of My Summer Car:

* **Stereo:** a CD head unit, speakers and a subwoofer, playing CDs burned from
  your own music or the game's FM radio.
* **Bed covers:** a tarp, a tri-fold tonneau cover or a hard top (camper shell),
  fitted to your pickup's bed.

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
| 6.5" speaker | $35 | Door/dash speaker, cuts deep bass |
| 12" subwoofer box | $120 | Bass only, louder, heavy (14 kg) |
| CD *n* | $5 | One per non-empty `CD` folder |
| Bed tarp | $40 | Strapped-down tarp; rolls up toward the cab |
| Tonneau cover | $250 | Tri-fold hard cover; folds up against the cab |
| Hard top | $600 | Camper shell to cab height, with a lifting rear glass hatch |

## Fitting

* **Pick up** a part with the normal left click.
* **Install:** hold it, look at the spot on the vehicle where you want it (dash,
  doors, rear deck, bed...) and press **Y**. It mounts flush to that surface, facing
  out. Mounted on a door, it swings with the door.
* **Remove:** look at an installed part and press **Y**. It drops loose.
* **CDs:** hold a CD, look at the head unit and press **Y** to insert it.

The head unit plays through **every speaker installed in the same vehicle**, all in
sync. With no speakers you only hear its tiny built-in speaker. It needs the
vehicle's battery: a flat battery means no music.

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
`CdPrice`, `TarpPrice`, `TonneauPrice`, `HardTopPrice`). `FreeParts = true` makes everything free, in both shops.

## Saving

Parts are saved on their own, to `UserData/TruckPartsQOL.txt`, every minute and
when you quit. This includes installed parts with their exact spots, loose parts,
power, volume, mode, track, the CD in the slot, and whether each bed cover is open. They are restored when a level
with vehicles loads. Because this save is separate from the game's, it doesn't
follow the game's save slots, and it keeps changes even if you quit without saving
the game.

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
