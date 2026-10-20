# Junkyard Truck Mods

[MelonLoader](https://github.com/LavaGang/MelonLoader) mods for **Junkyard Truck**: new
parts and engines, a 2004 Ford Ranger, a drivable ATV, a car stereo, an online parts
shop on the garage PC, a mods menu, and fixes so the game copes with more than one of
everything.

Every mod is separate: install the ones you want. They also work together (see
[How they fit together](#how-they-fit-together)).

| Mod | Version | What it does |
|---|---|---|
| [Engine Cloner](EngineCloner/) | 2.8.0 | Lets extra engines and trucks work on their own; sells new I4 / V8 / 2J engine blocks and an 6.6L Duramax V8 turbo-diesel and extra engine stands; removes vehicle speed limits |
| [Junkyard Ranger](JunkyardRanger/) | 1.6.1 | A 2004 Ford Ranger made from the game's own F100, on the Diamondback's chassis: takes the game's engines, gearbox and parts (needs Engine Cloner) |
| [Truck Parts QOL](TruckPartsQOL/) | 1.5.0 | Car stereo (head unit, door speakers, sub, amp, CDs from your music), rock lights in any color, wheel spacers, bolt-down parts, bed covers, OBD scanner tool, paintable rims and turbos |
| [Junkyard ATV](JunkyardATV/) | 1.0.0 | A drivable ATV (Suzuki Quadzilla 500 model) running on the game's own 250 engine |
| [JunkAuto PC](JunkAutoPC/) | 1.1.0 | JunkAuto.com, a RockAuto-style parts site on the garage computer (parts by vehicle and engine), with delivery and order emails |
| [Parts Store Plus](PartsStorePlus/) | 1.1.0 | Sells every dirt bike part and every truck engine part (pistons, bearings, crank...), not just what the junkyard spawns |
| [Mods Menu](ModsMenu/) | 1.2.0 | A **MODS** button in the main menu: every installed mod and its settings, in the game's own style |

Each folder has its own README with the details.

## Install

1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) 0.6 or newer
   into Junkyard Truck.
2. Copy the mod `.dll` files into `<game>/Mods/`.
3. Extra files, for the mods that use them:
   * **Truck Parts QOL:** the speaker and amp models go in
     `<game>/UserData/TruckPartsQOL/models/`. Put your music in
     `<game>/TruckPartsQOL/CD1/`, `CD2/` and so on (`.ogg` or `.wav`).
   * **Junkyard ATV:** the ATV model goes in `<game>/JunkyardATV/`. Without it, the ATV
     uses a simple built-in body.
   * **Junkyard Ranger:** the body files go in `<game>/UserData/JunkyardRanger/models/`.
     It needs Engine Cloner.

Settings live in `UserData/MelonPreferences.cfg`. With the Mods Menu installed you can
change them in game: **MODS → (mod name)**.

## The mods

### Engine Cloner

A patch layer that lets the game run with more than one engine of a type, plus the
gameplay built on it.

* **Independent engines:**
  * **Own physics:** every engine block gets its own physics anchor, so copies no
    longer drag each other around. The game shares one anchor per engine type.
  * **Wiring:** the truck, gauges and gearbox are rewired to whichever block is
    actually bolted in.
  * **Fitting parts:** a part you fit goes to the engine you fitted it to.
* **New engines for sale:** I4 ($750), V8 ($1500) and 2J inline-6 ($1800).
  * They come as **bare blocks**, rust-free, with every part off and the mounting
    bolts out.
  * They ship dry: add oil before starting.
  * Optionally, a complete engine instead.
* **Extra engine stands** ($350): a copy of the game's stand.
  * **Using it:** snapping, rotating and the release lever all act on the stand
    you're using.
  * **Moving it:** press **K**.
* **No speed limits:** removes the top-speed cut-off of the pickup (85/100), the F100
  (95/100) and the golf cart (100). This is a setting, on by default.
* **Saving:** bought engines and stands are saved with your save slot. That covers
  loose, on the stand, or bolted in a vehicle, with every part, bolt and fluid.
* **More than one truck:** copies of the game's trucks (the Ranger) each run their
  own engine, take fuel where you pour it, and are kept out of the game's own save.
  Other mods build on this through `VehicleApi`.
* **Debug:** a clone key (off by default) copies the engine or vehicle you're
  looking at.

### Truck Parts QOL

Extra parts for your vehicles, in the spirit of My Summer Car.

* **Stereo:**
  * **Head unit:** a CD/radio head unit that plays your own music folders as CDs.
  * **Speakers and sub:** 6.5" door speakers that sit in the door panel, and a 12"
    subwoofer box.
  * **Amp:** a 4-channel amplifier that drives the speakers harder and gives the sub
    its full output.
  * **Power:** everything plays in sync through every speaker in the vehicle, off the
    vehicle's battery.
* **Fitting:** put a part anywhere you want, then **bolt it down with the ratchet**,
  just like the game's own parts.
  * These are real game bolts. The ratchet's scroll wheel turns them.
  * A barely bolted part falls off when you drive.
  * Undo the bolts to take the part off.
* **Bed covers:** tarp, tonneau cover and hard top (work in progress).
* **OBD scanner:** a tool on key **8** that reads the condition of the engine parts.
* **Paint:** rims and turbos take the game's paint cans.
* **Saving:** everything, including bolt states, is saved with your save slot.

### Junkyard Ranger

* **The truck:** a 2004 Ford Ranger regular cab, $4500 on JunkAuto and in the Parts
  Store.
* **Built from the game's trucks:**
  * **Body:** the game's F100, brought down to Ranger size and given the 2001-04
    front end (rounded nose, wrap-around headlights, chunky grille and bumper),
    with the game's own textures.
  * **Chassis:** a copy of the Diamondback, so it drives like it and takes the
    game's engines (I4, V8, 2J), gearbox, wheels and parts. Its dash and seats are
    in the cab.
* **Comes with:** a new engine (I4 by default, complete, or pick V8, 2J or none), a
  full tank, and two-tone paint (white over charcoal; paint either with the cans).
* **Accessories:** bull bar, rain guards, sun visor; they pop on (and bolt down) the
  game's way (snap mark, ratchet).
* **Spawn key (F7):** a free Ranger in front of you with an empty engine bay, for
  an engine you build yourself.
* **Like the game's trucks:** doors, hood and tailgate open; fuel on the driver's
  side; paint it with the spray cans, panel by panel.
* **Saving:** saved with your save slot, including the engine bolted in it.

### Junkyard ATV

* **The ATV:** a four-wheeler built around a real copy of the game's 250 engine, so
  every part can wear, break, be removed and be replaced.
* **Fuel and oil:** filled with the game's own nozzle and bottle.
* **Model:**
  * **Shipped model:** it uses a rigged Suzuki Quadzilla 500.
  * **Your own:** load any `.glb`, `.gltf` or `.obj` model and line it up in
    **fit mode** (F7).
* **Buying and saving:** buy it in the shops, and it's saved with your save slot.

### JunkAuto PC

* **The site:** click the garage computer, then **[ JunkAuto.com ]**.
  * **Catalog:** a RockAuto-style parts site, organised by vehicle (each truck engine
    has its own section) and category.
  * **Ordering:** a cart, checkout and order status.
* **Delivery:** orders arrive after a few minutes of play, where you placed them.
* **Email:** confirmation and delivery emails appear in the game's own inbox.
* **What it sells:** the Parts Store's catalog, or the junkyard's parts if you don't
  have that mod, plus everything the other mods sell.

### Parts Store Plus

* **What it adds:** the game's shop only lists what the junkyard can spawn. This adds
  the rest: every dirt bike part, and every truck engine part (pistons, bearings,
  crank, cam...).
* **How it finds them:** it reads the part slots on the bike and the engines.
* **Settings:** price multiplier and minimum price.

### Mods Menu

* **The button:** a **MODS** entry between UPDATES and SETTINGS in the main menu,
  copied from the game's own button.
* **The page:** it lists every installed MelonLoader mod. Click one to change its
  settings in game:
  * on/off switches;
  * key binds;
  * choices;
  * numbers and text.
* **Compatibility:** works with any mod that uses MelonPreferences.

## How they fit together

* **One shop list:** Truck Parts QOL, Junkyard ATV, Junkyard Ranger, Parts Store Plus
  and Engine Cloner publish what they sell to a shared list. JunkAuto PC, and the Junkyard
  Terminal's Parts Store if you have it, sell all of it.
  * The list is `AppDomain` data under `JunkyardTruckMods.ShopProviders`.
  * It's built from plain .NET types only, so no mod needs another's DLL.
* **Saves follow the game's slots:** each mod saves its own data in
  `UserData/<Mod>/slot1.txt` to `slot3.txt` and `auto.txt`, alongside the game's
  save slots.
  * Deleting a slot, or overwriting it with a new game, clears that data too.
  * The game's own save files are never changed.
* **Settings:** every mod's settings show up in the Mods Menu.

## Controls

| Key | Mod | Action |
|---|---|---|
| **Y** | Truck Parts QOL | Install / remove a part, insert a CD |
| **P / N / B / M / J** | Truck Parts QOL | Head unit: power, next, previous, CD/radio, eject |
| **O** | Truck Parts QOL | Open / close a bed cover |
| **8** | Truck Parts QOL | OBD scanner |
| **F9** | Truck Parts QOL | Parts shop (only without the Parts Store mod) |
| **2** (ratchet), scroll | game | Tighten / undo bolts, including on our parts |
| **W A S D, Space, I** | Junkyard ATV | Drive, handbrake, ignition |
| **F10** | Junkyard ATV | Buy an ATV (only without the Parts Store mod) |
| **F7 / F6** | Junkyard ATV | Fit mode / export the engine (in fit mode) |
| **K** | Engine Cloner | Move a bought engine stand |
| **F8** | Engine Cloner | Debug clone (only with "Debug: clone key" on) |

All keys can be changed in the Mods Menu or `MelonPreferences.cfg`. Note that the
Junkyard Dumper mod also uses F8.

## Building

Each mod is a separate project targeting .NET Framework 4.7.2.

```
cd EngineCloner
dotnet build -p:GameDir="C:\Program Files (x86)\Steam\steamapps\common\Junkyard Truck"
```

`GameDir` defaults to the usual Steam path. The build references the game's
`Assembly-CSharp.dll` and Unity modules plus MelonLoader's assemblies, and copies the
mod into `<game>/Mods` when that folder exists.

Junkyard Ranger also references `EngineCloner.dll` (from the game's `Mods` folder).

Tools:
* **ATV model rig:** `JunkyardATV/tools/` (Python).
* **Stereo model converter:** `TruckPartsQOL/tools/` (Python with Blender's `bpy`).
* **Ranger body:** `JunkyardRanger/tools/` (Python with Blender's `bpy`): makes the
  Ranger from the game's F100 and Diamondback, exported with AssetRipper.
* **Truck Exporter:** `tools/TruckExporter/`, a modding tool that writes the game's
  vehicles (meshes, textures, part hierarchy) to OBJ from inside the game.

## Repository layout

```
Assembly-CSharp/   Decompiled game code, for reference only (never modified or shipped)
EngineCloner/      Engine Cloner
JunkyardRanger/    Junkyard Ranger
TruckPartsQOL/     Truck Parts QOL
JunkyardATV/       Junkyard ATV
JunkAutoPC/        JunkAuto PC
PartsStorePlus/    Parts Store Plus
ModsMenu/          Mods Menu
tools/             Modding tools (Truck Exporter)
```

3D models aren't part of the repository; they're distributed with the mod downloads.
