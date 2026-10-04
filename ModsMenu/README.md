# Mods Menu

A MelonLoader mod for Junkyard Truck that adds **MODS** to the main menu, between
UPDATES and SETTINGS.

* The button is a copy of the game's own UPDATES button, so it has the same font,
  hover animation and click sound.
* Clicking it opens a panel in the middle of the screen (dark backdrop, centered
  text in the menu's font) listing every installed MelonLoader mod (name, version,
  author), with **BACK** (or Esc) to return.
* **Mod settings:** click a mod to see and change its settings, one per line as
  `NAME: VALUE`:
  * **On/off:** click to flip it.
  * **Keys:** click, then press the new key (Esc cancels).
  * **Choices:** click to step to the next option.
  * **Numbers and text:** click, type, then Enter to keep it or Esc to cancel.

  The line under the title shows the setting's description. Long lists page with
  NEXT PAGE / PREVIOUS PAGE or the mouse wheel. RESET THESE TO DEFAULTS puts the
  page back. Changes save to `UserData/MelonPreferences.cfg` right away, and mods
  that read their settings as they go (all of ours do) use them immediately.

## Which mods get settings

Any mod that keeps its settings in MelonPreferences shows up, with no changes
needed. A settings category belongs to the mod it's named after: the mod's name,
namespace or assembly, ignoring case, spaces and punctuation. For example,
"Parts Picker" keeps its settings in `ComputerFeatures`, its namespace. Hidden
categories and settings are left out. A category that matches no mod is still
listed at the bottom as `SETTINGS: <name>`.

For a new mod, create the category with the mod's name (`EngineCloner` for
"Engine Cloner") and give each entry a display name and description, and it shows
up with readable names and help text.

The Mods Menu's own settings (under MODS → MODS MENU):
* **Text size** (1): scale the panel's text, e.g. 1.2 for bigger.
* **Panel width** (0.7): share of the screen the panel spans.
* **Settings per page** (8).

## Install

Copy `Mods/ModsMenu.dll` into `<game>/Mods/`. It works on its own and needs none
of the other mods.

## How it works

The menu is Unity UI (the Michsky Dark UI kit). When a scene loads, the mod looks
for the UPDATES and SETTINGS labels (UI Text or TextMeshPro) in the same button
column. It copies the UPDATES entry, renames it and gives it its own click event,
so it can't do what UPDATES does. Then it slots the copy in before SETTINGS. If the
column has a layout group, that places it; if not, SETTINGS and everything below
it move down one slot. If the menu reports its scene as loaded twice, the existing
MODS button is reused instead of adding another.

If MODS doesn't appear, the log says why, and 10 seconds after the menu loads the
mod writes the menu's layout to `UserData/ModsMenu/ui_<scene>.txt`. UI and TextMeshPro types are reached by reflection, so the
mod builds against nothing but MelonLoader and Unity's core modules.

## Build

`dotnet build` with `GameDir` pointing at the game, or the same mcs command as the
other mods (references: MelonLoader, UnityEngine.CoreModule,
UnityEngine.InputLegacyModule).
