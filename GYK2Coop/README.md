# Graveyard Keeper 2 Co-op (BepInEx mod)

Two-player online co-op for Graveyard Keeper 2. One player **hosts** their own save and the
other **joins** it: you both walk around the same world, see each other, and the world changes
either of you make are mirrored to the other.

> **Status: untested in-game.** This was written against the game's decompiled code
> (`Assembly-CSharp.dll`) without being able to run the game. The network layer is tested;
> the in-game parts are not. Expect rough edges the first time you try it, and **back up your
> save folder first** (see below). If something breaks, send `BepInEx/LogOutput.log` from both PCs.

## What works

| | |
|---|---|
| Host / join | Host from inside your save, join from the main menu, using an address and port. |
| World transfer | The guest gets the host's world as it is when they join (the save file format, sent over the network). |
| Seeing each other | The other keeper is drawn walking, chopping, digging, planting and climbing, with a name above their head. Carrying shows the carry pose and the body or crate held overhead. Armor (with or without helmet, in its color) is shown too. |
| Bodies | Each of you still gets your own body deliveries, and both players see every body. Either player can pick up, carry, drop, bury and work on any body. The unburied-body count is recounted in both games whenever a body moves. |
| Graves and things you use | Burying, gravestones, fences, and putting things into or taking them out of an object (grave, chest, workbench) show up for the other player. |
| One player at a time | While one of you has an object's window open, or a craft running on it, it's locked for the other player: they can't select it, and it shows "In use by …" or "… is crafting 40%". When the craft finishes, the results (and anything left in the bench) appear for both of you. |
| Quests | The host's quest progress and objective arrow are copied to the guest, so both see the same quest markers. |
| Items on the ground | Things dropped near a player (loot, crates, bodies) appear for both of you, and picking one up removes it for both. |
| Time of day | The guest's day and clock follow the host's. |
| World objects | New objects that appear near a player are added for the other player (things built or placed). Changes and removals the player causes are mirrored (a tree becoming a stump, an empty grave becoming a filled one). Objects are changed in place, with the game's own function, and never deleted to be replaced. |
| Guest's character | The guest's inventory, stats and position are stored on the host's PC (every 30 s and when they leave) and given back the next time they join that save. |
| Chat | Small text chat in the panel. |

## What doesn't (yet)

- **Story, quests, dialogue and cutscenes are host-only.** The guest sees the host's quests, but the guest's game can't start or finish quests while connected. Hand-ins and story talks have to be done by the host.
- **Combat isn't synced.** Enemies, attacks and damage happen separately in each game.
- **Crafting at the same bench at the same time:** a craft runs only in the game of the player who started it. The other player sees the bench's result after the queue finishes. Don't both queue crafts on one bench at once.
- **NPCs, zombie workers and walking creatures** are simulated separately by each game. They mostly line up because both games start from the same save, but not exactly.
- **The guest starts as a copy of the host's character** (same look, same inventory) the first time they join a save. After that they keep their own progress.
- Two players only (one host, one guest).
- The guest can't save. The world lives in the host's save, so the **host saves as usual**.

## Install (both players)

1. **Back up your saves.** Copy the folder
   `%USERPROFILE%\AppData\LocalLow\<developer>\Graveyard Keeper 2` somewhere safe.
   (In File Explorer, paste `%USERPROFILE%\AppData\LocalLow` into the address bar and look for the
   Graveyard Keeper 2 folder.)
2. **Install BepInEx 5** (the `win_x64` build of 5.4.23 or newer, *not* BepInEx 6):
   - Download `BepInEx_win_x64_5.4.x.zip` from https://github.com/BepInEx/BepInEx/releases
   - Extract it into the game folder (Steam → right-click Graveyard Keeper 2 → Manage → Browse
     local files), so `winhttp.dll` sits next to the game's `.exe`.
   - Start the game once and quit. A `BepInEx\plugins` folder should now exist.
   - Steam Deck / Linux (Proton): set the game's launch options to
     `WINEDLLOVERRIDES="winhttp=n,b" %command%`
3. Copy **`dist/GYK2Coop.dll`** from this folder into `BepInEx\plugins\`.
4. Start the game. `BepInEx\LogOutput.log` should contain
   `Graveyard Keeper 2 Co-op 0.2.0 loaded`.

Both of you need the same game version and the same mod version. The mod checks both and
refuses to connect if they don't match.

## Connecting to each other

The guest needs to reach the host's PC on **TCP port 7777**. Pick one:

- **Same house / same Wi-Fi:** nothing to set up. Use the host's local IP shown in the panel.
- **Different houses, easiest:** both install [Tailscale](https://tailscale.com/) (free) and log in, then
  use the host's Tailscale IP (`100.x.y.z`). [ZeroTier](https://www.zerotier.com/) and
  [Radmin VPN](https://www.radmin-vpn.com/) work the same way.
- **Different houses, no VPN:** the host forwards TCP port 7777 on their router to their PC and
  gives the guest their public IP (search "what is my ip"). Windows Firewall may ask to allow
  the game the first time you host. Click **Allow**.

## Playing

**Host:**
1. Load your save as normal.
2. Press **F8**, type your name, then click **Host this world**.
3. Tell your partner one of the addresses shown in the panel.

**Guest:**
1. Stay on the **main menu**. Don't load a save.
2. Press **F8**, type your name, put the host's address in the **Address** box (e.g.
   `100.64.1.2` or `192.168.1.20:7777`) and click **Join**.
3. The host's world downloads and loads. You appear next to the host.

Press **F8** at any time for the panel: status, chat, and **Leave** / **Stop hosting**. While the panel is
open in-game, your character doesn't move, so you can type.

When the guest leaves (Leave button, or quitting to the menu) their character is sent to the host. If the
host closes the world, the guest is sent back to the main menu.

## Settings

`BepInEx\config\gyk2coop.multiplayer.cfg` (created on first run):

| Setting | Default | |
|---|---|---|
| `PlayerName` | your Windows user name | Name shown above your head. |
| `Port` | 7777 | TCP port to host on. |
| `ToggleKey` | F8 | Key that opens the panel. |
| `SyncWorldObjects` | true | Turn off to only see each other, with no world mirroring. |
| `SyncRadius` | 15 | Object changes are only sent if they happen within this many metres of the player who made them. |
| `SyncTime` | true | Guest follows the host's clock. |
| `ExcludedWgoTypes` | ZombieWgoData | World object classes that are never mirrored. |

Guest characters are stored on the host in `BepInEx\config\GYK2Coop\guests\`.

## Changes

**0.5.0** (update both PCs)
- Workbench crafting works together: the bench is locked for the other player while a craft runs, they see the progress over it, and the finished items show up in both games.
- No more "last change wins" on chests and graves: an object is locked for the other player while someone has its window open.
- Contents sync now includes a bench's craft inventory (inputs and outputs), not only its storage.
- The other player's armor is drawn (helmet or not, in its color), using a private copy of the armor skin so your own armor color isn't affected.

**0.4.0** (update both PCs)
- Burying now works for both players: the grave changes (empty, body, filled) and what's in it (body, gravestone, fence) are synced, and the unburied-body count is recounted the way the game does it, including bodies the other player is carrying.
- Fixed in 0.3.0: when the game changed an object in place (e.g. putting a body in a grave), the other game could *delete* that object. Changes are now applied in place with the game's own function.
- Fixed the other player's arms disappearing (armor/weapon animation layers were copied without the armor skin).
- Quest markers: the host's quest statuses and objective arrow are shown to the guest. The guest's own game no longer moves quests while connected.
- Fixed: a first-time guest started out carrying a copy of whatever the host was carrying.
- Fixed: the guest's character is now sent to the host right before "exit to menu", not after.
- Items picked up off the ground are only reported to the other game under the same rules as dropping (no flood of resource pickups).
- Small speed-ups (cached lookups used every frame).

**0.3.0: safety release. Update both PCs.**
- 0.1/0.2 could replace existing objects with a copy from the other game (remove, then re-add),
  and trusted every removal the other game reported. That could delete crafting tables or leave
  broken objects in the host's save. Both behaviours are gone:
  - Existing objects are never replaced. Only brand-new objects are added.
  - Removals are only sent while the player is chopping, mining, digging, planting, fighting or building, and never in bursts.
  - Removals are only applied to the exact same object, and **never** to anything holding items or a craft.
  - A burst of incoming removals is refused, with a chat message.
- Contents of chests and workbenches are no longer synced. Each game keeps its own.

**0.2.0**
- The other player's carry pose and carried item (bodies, crates) are now visible.
- Bodies and items on the ground are synced, so either player can carry or work on the other's body.
- Fixed workbenches getting stuck crafting forever (couldn't change recipe or remove the bench). Running crafts are no longer copied between games, and updates to a bench wait until neither game is crafting on it.
- Not compatible with 0.1.0. Both players must update.

If a bench in your save is still stuck from 0.1.0, save, quit to the menu and load again.

## Troubleshooting

- **No panel on F8:** check `BepInEx\LogOutput.log` for the "loaded" line. If BepInEx itself doesn't
  start, the `winhttp.dll` is in the wrong folder (or on Proton, the launch option is missing).
- **"Could not connect" / timed out:** the guest can't reach the host. Use Tailscale, or check the
  port forward and Windows Firewall.
- **"version mismatch":** update both games and use the same `GYK2Coop.dll`.
- **Guest gets stuck loading:** send both `LogOutput.log` files. The interesting lines start with
  `[Error  :Graveyard Keeper 2 Co-op]` or contain `GYK2Coop`.
- **Objects doubled or missing:** set `SyncRadius` lower, or turn off `SyncWorldObjects` and play
  "together but separate" until it's fixed.

## How it works (for whoever fixes it next)

- `Net/`: length-prefixed TCP frames, with background read and write threads; the game thread drains a queue.
- `CoopRunner.cs`: session state machine (host: listen → handshake → send world → playing;
  guest: connect → hello → receive world → `MainGame.ContinueGame` → ready → playing), plus
  the IMGUI panel.
- `Game/GameSerializer.cs`: uses the game's own `SaveSystem.OdinBinaryFileSerializer` (the
  serializer used for save files) by reflection, falling back to Odin's `SerializationUtility`.
- `Game/Puppet.cs`: the other player is a stripped, visual-only copy of your own `PlayerView`
  (sprites, animator and skin only). Animation events are off, so it can never trigger
  gameplay code.
- `Game/WorldSync.cs`: Harmony hooks on `GameSceneData.AddWgoData` / `RemoveWgoData`. Incoming
  additions are matched to an existing object with the same definition within 0.35 m (and kept
  if one exists), so objects both games spawn on their own aren't doubled.
  Incoming updates replace the local object but keep its local id.
- `Game/DropSync.cs`: Harmony hooks on `GameSceneData.AddDrop` / `RemoveDrop`. Bodies (item groups
  `body`/`corpse`) are always mirrored and never merged; other drops only near the player who
  made them, matched by item id and position.
- `Game/ObjectLocks.cs`: per-object locks (window open, or craft queued from this game), heartbeat every
  second with craft progress, expiry after 5 s; locked objects are filtered out of
  `PlayerInteractionComponent.GetWgoTargetsFromColliders`. Contents are sent before the unlock.
- `Game/QuestSync.cs`: host → guest copy of quest statuses (data only) and the objective arrow; the guest's
  quest transitions are blocked while connected.
- `Game/SavePatches.cs`: the guest's `SaveSystem.Save` does nothing while it's in the host's world.
- Anything typed by `LazyBearTechnology.dll` (the game's engine library) is reached by
  reflection in `Game/GameBridge.cs`, so a game patch breaks one lookup instead of the whole mod.

The game ships with an unfinished co-op prototype of its own (Unity Netcode, `UNetworkManager`,
`LobbyHelper`, `CharMoveCommand`). It's never switched on (`MainGame.InitNetwork()` is
empty), so this mod doesn't use it.

## Building from source

Needs the .NET SDK (6 or newer) and the game with BepInEx installed:

```
dotnet build -c Release -p:GameDir="C:\Program Files (x86)\Steam\steamapps\common\Graveyard Keeper 2"
```

The project finds `<Game>_Data\Managed` by itself and copies the DLL into `BepInEx\plugins`.
