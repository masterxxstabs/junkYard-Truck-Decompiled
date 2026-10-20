# Controller Support

Play Junkyard Truck with a controller. The keyboard and mouse keep working
alongside it.

* **Bolts:** hold **L2** to tighten and **R2** to loosen. A bolt turns one notch
  at a time while you hold, just like rolling the scroll wheel.
* **Driving, like a driving game:**
  * **R2:** gas.
  * **L2:** brake while you're rolling forward, then reverse once you've stopped.
    A light press slows you down gently; past halfway the brakes bite.
  * **Left stick:** steer.

  This works in every vehicle (Diamondback, F100, AMC, golf cart, dirt bike) and
  in the Junkyard Ranger.
* **Walking:** left stick to move (analog, so you can creep), right stick to look.

## Install

1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) 0.6 or
   newer.
2. Copy `ControllerSupport.dll` into `<game>/Mods/`.
3. Plug in the controller (it's picked up any time, including mid-game).

## Controllers

* **Xbox controllers** (360, One, Series) work as they are. The mod reads them
  straight from Windows (XInput).
* **PlayStation and other pads:** turn on **Steam Input** for the game, or use
  DS4Windows. Either one makes the pad look like an Xbox controller. On a
  PlayStation pad, L2/R2 are the triggers, cross is A, circle B, square X and
  triangle Y.

## Buttons

Defaults below. Every button can be changed, separately for on foot and for
driving, in **MODS → Controller Support** (or `UserData/MelonPreferences.cfg`).
Buttons that stand for a game control (jump, interact, throw...) press whatever
key you set for that control in the game's options.

| Button | On foot | Driving |
|---|---|---|
| **L2 (hold)** | Tighten bolts | Brake / reverse |
| **R2 (hold)** | Loosen bolts | Gas |
| Left stick | Walk | Steer |
| Right stick | Look | Look |
| A | Jump | Handbrake |
| B | Throw | Interact |
| X | Interact | Clutch |
| Y | Put in inventory | Get out |
| RB | Left click (pick up, use, drop) | Winch pull |
| LB | Right click (remove a part) | Winch release |
| L3 | Sprint (hold) | Wheelie (dirt bike) |
| R3 | Crouch | Switch camera |
| D-pad up | Ratchet | Flashlight |
| D-pad down | Flashlight | |
| D-pad left / right | Previous / next tool | Lean out / lean in |
| Back | Map | Map |
| Start | Pause | Pause |

The tools cycle in the game's order: hands, ratchet, multimeter, depth gauge,
phone, tire gauge, crowbar.

## Settings

| Setting | Default | What it does |
|---|---|---|
| Enabled | on | Turns the controller off without removing the mod. |
| Look speed | 90 | How fast the right stick turns the camera. |
| Invert look up/down | off | Push up to look down. |
| Bolt turns per second | 8 | How fast bolts turn while L2 / R2 are held. |
| Stick deadzone | 0.2 | How far a stick moves before it counts. |
| Trigger threshold | 0.15 | How far L2 / R2 are pressed before they count. |
| Steering curve | 1.4 | Higher = gentler steering near the middle of the stick. |

## Notes

* **Menus need the mouse.** Pause menu, inventory screen, phone, shop and the
  garage PC are clicked with the mouse. Start, Back and Y open and close them.
* **How it works:**
  * **Driving:** the game already takes analog gas, brake and steering for its
    Logitech wheel support. The mod feeds the pad into the same inputs. If a
    Logitech wheel is connected, the wheel wins.
  * **Everything else:** at start-up, every place in the game (and in other mods)
    that reads `UnityEngine.Input` is pointed at the mod instead. It answers with
    the keyboard and mouse plus the controller, and nothing else about those
    methods changes. The log says how many it hooked.
* **Holding a part:** like the scroll wheel, L2 / R2 also turn a part you're
  holding.
