# 6.6L Duramax (MelonLoader mod for Junkyard Truck)

A GM-style 6.6L Duramax V8 turbo-diesel for the trucks, with its own detailed
model, more power, and bolt-on accessories. **Needs Engine Cloner** (3.0 or
newer): Engine Cloner is the patch that makes extra engines work; this mod adds
the Duramax on top of it (through Engine Cloner's `EngineApi`).

## Getting one

**Engine - 6.6L Duramax V8 turbo-diesel (new)**, $4200:
* **JunkAuto.com** (JunkAuto PC mod): **6.6L Duramax Diesel → Engine**, with every
  part that fits it (the V8's) under the other categories, and its accessories
  under **Performance & Accessories**.
* **Parts Store** (ComputerPartStore mod): category **6.6L Duramax Diesel**, first
  in the **All** list (and in Truck Parts).
* **Junkyard Ranger:** set its engine to the Duramax for a new Ranger with one.

Like Engine Cloner's other engines it comes as a bare block (or complete, with
Engine Cloner's "New engines: bare block" off), dry, and is saved with your save
slot. The shop needs a V8 somewhere in the level to build it from.

## The engine

A big turbo-diesel V8 for the trucks, built on the game's own V8 so it **works like any engine in
the game**: it drops into the same engine bays (Diamondback, F100), takes all the
V8's parts (heads, pistons, cam, intake, carb or fuel rail, headers, the
aftermarket turbos...), bolts in with the hoist, and needs oil, coolant and a
battery like the others. It's sold as a bare block or complete, like the rest, and
saved the same way.

What's different:
* **Its own model:** a GM-style turbo-diesel V8, modeled for this mod. Each
  part has its own baked textures, like the game's engines: shading in the
  corners, grime that collects low down and in the gaps, worn edges, cast grain,
  and a metal/gloss map (bare metal, paint, plastic and rubber each look their
  own).
  * **Block:** deep skirt flaring out to the banks, cylinder barrels, deck bolt
    bosses, ribs, freeze plugs, the oil cooler on the passenger's side, engine
    mounts with their rubbers, the bellhousing flange and its bolts, the
    dipstick tube up to the game's dipstick.
  * **Heads:** aluminium, with exhaust port faces, head bolts, glow plugs and
    lifting eyes.
  * **Valve covers:** cast rocker housings bolted all round, rounded black
    injector covers with **DURAMAX** and ribbed ends, the oil filler neck and a
    yellow **oil cap**, and a crankcase breather.
  * **Intake:** a smooth cast valley manifold, the Y-bridge with **6.6L**, its
    coupler and clamps, the EGR valve and cooler, the wiring harness and the
    charge pipe.
  * **Turbo:** a variable-geometry turbo in the valley: aluminium compressor
    scroll with its wheel in the inlet, cast iron turbine housing, actuator, oil
    lines, pedestal.
  * **Air:** a rubber inlet elbow and MAF housing off the turbo, and a pleated
    cone air filter.
  * **Fuel:** the CP3 high-pressure pump, fuel rails along both heads, steel
    injector lines with their nuts, feed lines, and the glow plug harness.
  * **Exhaust:** cast log manifolds and up-pipes with braided flex joints to
    the turbo.
  * **Front:** the front cover and its bolts, the damper, the water pump,
    a nine-blade fan on its clutch, the alternator (and its fan), the A/C
    compressor, the power steering pump, an idler and a ribbed serpentine belt
    round them all; the thermostat housing with the upper hose, and the lower
    hose.
  * **Below:** an oil pan with a deep sump, its bolts and drain plug; the oil
    filter; the starter and solenoid.

  * **Fuel filter head (2.10):** in the valley, with its ribbed cap, yellow
    primer button and bleed screw.

  It's shown on the V8's own parts (26 of them), so the engine comes apart,
  wears and goes back together the game's way. The V8's gasoline parts get
  diesel looks: the distributor is the turbo, the plug wires are the fuel lines,
  the carb and air cleaner are the inlet and air filter, the fuel pump is the
  CP3. A part taken off is the game's loose V8 part.
* **Power:** it pulls **60% harder** than the game's V8 (setting "Power", 1.6 = 60% more torque; turbos still add on top).

The model is `UserData/Duramax/models/duramax.obj` (with `.mtl`, its
`duramax_*` textures, three per part, and `duramax_acc.json`), in the download;
without it, the Duramax is the game's V8 in charcoal and gloss black. It's made by
`tools/duramax_model.py` (Blender's Python module) to the frames of the game's V8
parts.

Parts for it are the V8's: buy them as V8 parts.

## Accessories

Bolt-ons for the Duramax, sold next to it (JunkAuto: **6.6L Duramax Diesel →
Performance & Accessories**; Parts Store: category **6.6L Duramax Diesel**). They
fit the game's way: carry one to its place on a Duramax and the snap mark shows;
let go and it pops on, then tighten its bolts with the ratchet. Undo the bolts and
right-click it to take it off.

| Accessory | Goes on | Bolts | Pull | Price |
|---|---|---|---|---|
| Ram intake horn (billet) | the intake's Y-bridge (front of the engine) | 4 | +6% | $450 |
| Turbo blanket | the turbo's turbine housing | snaps on | +3% | $140 |
| Lift pump & filter kit | the driver's side of the block, low | 4 | +6% | $650 |

* **Needs its part:** the horn only fits while the intake is on, the blanket while
  the turbo is on.
* **Pull:** each one multiplies the Duramax's torque while it's fitted (all three
  together: about +16%). It counts from the next time the game refreshes the
  engine, for example when you get in.
* **Saving:** fitted accessories are saved with the engine; loose ones on their
  own.
* **Prices:** settings in **MODS → 6.6L Duramax**.

**Saving:** bought engines, and debug clones, are saved per save slot in
`UserData/EngineCloner/slotN.txt` (`auto.txt` for the autosave):
* **Placement:** loose, on the engine stand, or bolted in a vehicle.
* **State:** every part's fitted state, condition and bolts, plus the engine's
  condition and fluids.

They're kept out of the game's own save, so they can never overwrite your
original engine's save entry.

## Settings

In **MODS → 6.6L Duramax**, or `UserData/MelonPreferences.cfg` under `[Duramax]`:

| Setting | Default | What it does |
|---|---|---|
| Price | 4200 | What a new Duramax costs |
| Power | 1.6 | Its torque compared with the game's V8 (1.6 = 60% more) |
| Ram intake horn / turbo blanket / lift pump kit price | 450 / 140 / 650 | Shop prices of the accessories |

## Install

1. Install [Engine Cloner](../EngineCloner/) 3.0 or newer.
2. Copy `Duramax.dll` into `<game>/Mods/`.
3. Copy the `models` folder into `<game>/UserData/Duramax/`.

**From Engine Cloner 2.x** (when the Duramax was part of it): install this mod
and Engine Cloner 3.0; your Duramaxes, fitted accessories and loose ones load as
before (they're in Engine Cloner's save files). The model can stay in
`UserData/EngineCloner/models/` for now (it's found there too), but
`UserData/Duramax/models/` is where it goes. The old Duramax settings in
`[EngineCloner]` aren't used any more: set them again under `[Duramax]`.

## Build

```
cd Duramax
dotnet build -p:GameDir="C:\Program Files (x86)\Steam\steamapps\common\Junkyard Truck"
```

It references Engine Cloner from `<game>/Mods/EngineCloner.dll`.
