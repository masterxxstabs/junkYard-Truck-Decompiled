# Parts Store Plus

A MelonLoader add-on for the Junkyard Terminal's **Parts Store** (the
ComputerPartStore / "Parts Picker" mod). It makes the store sell **every dirt bike
part**, not just the ones the junkyard happens to spawn.

## Why parts were missing

The Parts Store builds its catalog from the junkyard spawner: `JunkSpawner.spawnItems`
become "Truck Part" and `JunkSpawner.spawn250Items` become "Dirt Bike Part". A bike
part that isn't in the junkyard's spawn list never showed up in the store.

## What this does

Every part slot on the dirt bike and its 250 engine is a `durability` whose
`template` (and `template2`-`template5`) is the loose part the game itself spawns
when you take that part off. Those templates are the complete set of dirt bike
parts. Every few seconds the mod collects them from the dirt bike and every 250
engine in the level (the bike's engine counts even when it's out of the bike). It
then adds any the store doesn't already sell, matched by prefab or by name, under
**Dirt Bike Parts**, sorted like the store's own list.

Bought parts arrive like any other store order: brand new, in front of you. The
log lists the parts it added the first time the store opens.

The store's own entries are never changed. Our ATV and stereo entries are added
by their own mods.

## Settings

In the Mods Menu (MODS → PARTS STORE PLUS) or `UserData/MelonPreferences.cfg`
under `[PartsStorePlus]`:

| Setting | Default | |
|---|---|---|
| Sell every dirt bike part | on | Off removes the added parts again |
| Price multiplier | 1 | Times the part's own game price |
| Minimum price | 5 | Floor for parts priced lower or at 0 |

## Install

Copy `Mods/PartsStorePlus.dll` into `<game>/Mods/` next to `ComputerPartStore.dll`.
Without the Parts Store it does nothing.
