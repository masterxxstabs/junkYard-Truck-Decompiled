# Mods Menu

A MelonLoader mod for Junkyard Truck that adds **MODS** to the main menu, between
UPDATES and SETTINGS.

* The button is a copy of the game's own UPDATES button, so it has the same font,
  hover animation and click sound.
* Clicking it turns the menu column into a list of every installed MelonLoader mod
  (name, version, author) in the same font, with **BACK** (or Esc) to return.

## Install

Copy `Mods/ModsMenu.dll` into `<game>/Mods/`. It works on its own and needs none
of the other mods.

## How it works

The menu is Unity UI (the Michsky Dark UI kit). When a scene loads, the mod looks
for the UPDATES and SETTINGS labels (UI Text or TextMeshPro) in the same button
column. It copies the UPDATES entry, renames it and gives it its own click event,
so it can't do what UPDATES does. Then it slots the copy in before SETTINGS. If the
column has a layout group, that places it; if not, SETTINGS and everything below
it move down one slot. UI and TextMeshPro types are reached by reflection, so the
mod builds against nothing but MelonLoader and Unity's core modules.

## Build

`dotnet build` with `GameDir` pointing at the game, or the same mcs command as the
other mods (references: MelonLoader, UnityEngine.CoreModule,
UnityEngine.InputLegacyModule).
