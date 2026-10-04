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
| Seeing each other | The other keeper is drawn walking, chopping, digging, planting and climbing, with a name above their head. |
| Time of day | The guest's day and clock follow the host's. |
| World objects | Objects added or removed near the player who did it are mirrored: trees chopped, rocks mined, things built or torn down, things placed. |
| Things you use | The object you're facing or working on (chest, workbench, garden bed, grave...) has its full state re-sent whenever it changes. |
| Guest's character | The guest's inventory, stats and position are stored on the host's PC (every 30 s and when they leave) and given back the next time they join that save. |
| Chat | Small text chat in the panel. |

## What doesn't (yet)

- **Story, quests, dialogue and cutscenes are host-only.** The guest's quest progress isn't saved. Leave the story to the host.
- **Combat isn't synced.** Enemies, attacks and damage happen separately in each game.
- **Items lying on the ground aren't synced.** Each player picks up their own loot.
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
   `Graveyard Keeper 2 Co-op 0.1.0 loaded`.

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
