# Truck Stereo (MelonLoader mod for Junkyard Truck)

A CD stereo system for your vehicles, in the spirit of My Summer Car: buy a head
unit, speakers and CDs burned from your own music, carry them to a vehicle and fit
them wherever you like.

Separate from the Engine Cloner mod. Install either or both.

## Install

Copy `TruckStereo.dll` into `<game>/Mods/` (MelonLoader 0.6 or newer).

On first launch the mod creates a `TruckStereo` folder in the game directory:

```
Junkyard Truck/
  TruckStereo/
    CD1/   <- your music for CD 1
    CD2/
    CD3/   (you can add folders up to CD9)
```

Each folder is one CD, played in file-name order (name them `01 ...`, `02 ...` to
control the order). Use **.ogg** or **.wav**. MP3 decoding depends on the game's
Unity version; if a track shows `READ ERROR`, convert it to .ogg.

## Buying parts

**With the Junkyard Terminal (ComputerPartStore mod) installed**, the stereo parts are
sold in its **Parts Store** tab, listed under *All* and *Truck Parts* as
`Stereo - ...`. Order them like any other part; they're delivered in front of you.
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

All keys can be changed in `UserData/MelonPreferences.cfg` under `[TruckStereo]`,
and so can the prices (`HeadUnitPrice`, `SpeakerPrice`, `SubwooferPrice`,
`CdPrice`). `FreeParts = true` makes everything free, in both shops.

## Saving

Stereo parts are saved on their own, to `UserData/TruckStereo.txt`, every minute and
when you quit. This includes installed parts with their exact spots, loose parts,
power, volume, mode, track and the CD in the slot. They are restored when a level
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
  the store does. Everything goes through reflection, so Truck Stereo still loads
  without the store. The prefabs are dormant templates under an inactive
  `DontDestroyOnLoad` object, because `TryPurchase` instantiates the prefab and the
  store caches its catalog across levels. The store drops orders at foot level, so
  loose stereo parts lift themselves clear of the ground when they spawn.
* Works with Engine Cloner: clone a vehicle with a stereo and the copy gets its own
  working stereo. Parts on a clone are saved against the vehicle's name, so after a
  reload they appear in the original.

## Build

```
cd TruckStereo
dotnet build -c Release
```

Same setup as Engine Cloner. References come from the game folder (default Steam
path); override with `-p:GameDir="..."`.
