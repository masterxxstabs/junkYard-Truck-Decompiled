# Furniture Tweaks

Garage furniture to keep your stuff in. You buy each piece in the shops, it gets
delivered, and you can place it wherever you want.

* **Parts shelf:** a steel pallet rack with four levels of wooden decking. Pop
  parts on and off it like books on a bookshelf.
* **Safe:** a heavy enamel safe with a combination dial, for cash you don't want
  to carry.
* **Storage crate:** a slatted wooden crate with a Tarkov-style grid inventory.
  Parts take up space by their size, and you arrange them yourself.

Needs **Engine Cloner** (its save hooks keep your furniture with your save slot).

## Buying

* **JunkAuto PC:** the new **Garage Furniture** section has the parts shelf, safe
  and crate under **Storage**. Engine Cloner's engine stand is there too, under
  **Shop Equipment**.
* **The Junkyard Terminal's Parts Store** (when that mod is installed): listed as
  "Furniture - Parts shelf", "Furniture - Safe" and "Furniture - Storage crate".

Each piece is delivered where the shop delivers, standing on the ground and facing
you.

## Placing

Look at a piece and press **U** (the move key). It glides along the floor to where
you're looking:

* **Out in the open,** it faces you.
* **Aim at a wall** and it backs up against it, square to the wall.
* **Scroll** turns it from there, 15 degrees a notch (hold **Shift** for 90).
* **U** puts it down exactly where it's shown.
* **Backspace** puts it back where it was.

Whatever is on the shelf or in the crate moves with it.

## Using it

* **Parts shelf:** hold a part, look at the shelf and let go. The part is put on
  the free spot nearest where you're looking. It lies on its broadest side with
  its long side along the shelf, the way you'd shelve it, and stays there until
  you pick it up again. There are 12 spots, 3 per
  level. Tall parts only fit on the top level. Engine blocks go on an engine stand,
  not the shelf.
* **Storage crate:** a 10 × 8 grid, like Tarkov's.
  * **Size:** each part takes as many cells as its two longest sides, at 18 cm a
    cell. A spark plug is 1 × 1, a battery 2 × 2, a transmission about 4 × 2.
  * **Putting in:** hold a part, look at the crate and let go. It goes into the
    first place it fits, turned if that's what fits. If there's no room, you're
    told so.
  * **Opening:** with your hands free, look at the crate and press **R** (the
    crate key). The grid opens with a picture of each part, drawn from the part
    itself, plus its name and wear.
  * **In the grid:**
    * **Drag** a part to move it. A green outline shows where it will go, or red
      if it doesn't fit.
    * **R while dragging** turns it a quarter.
    * **Right-click** a part, or **drag it out of the window**, to take it out
      into your hands.
    * **R** or the **X** closes the grid, and so does walking away.

  While the grid is open you can't look around, and clicks don't reach the
  world.

  The crate takes parts the game itself saves. A part another mod makes and saves
  (a stereo part, a Ranger accessory) can't go in a crate, so put it on a shelf.
* **Safe:** look at it with your hands free:
  * **Scroll up** puts $100 of your cash in.
  * **Scroll down** takes $100 out, as a roll of bills like the ATM gives you.
    Pick the roll up to put it in your wallet.
  * Hold **Shift** to move $1,000 at a time.

  Letting go of a roll of cash while you look at the safe puts it in too.

A thrown part doesn't go onto a shelf or into a crate. Only a part you let go of
does.

## Settings

In `UserData/MelonPreferences.cfg` under `[FurnitureTweaks]`, or in the Mods Menu:

| Setting | Default | |
|---|---|---|
| ShelfPrice | 450 | Price of a parts shelf |
| SafePrice | 900 | Price of a safe |
| CratePrice | 120 | Price of a storage crate |
| MoveKey | U | Pick up and move the piece you're looking at; press again to put it down |
| TakeKey | R | Open and close a crate's grid |
| SafeStep | 100 | Cash moved in or out of a safe per scroll (×10 with Shift) |

## Saving

Your furniture is saved with your save slot in `UserData/FurnitureTweaks/slotN.txt`
(`auto.txt` for the autosave). That covers where each piece is, the cash in each
safe, and which parts are on each shelf and in each crate. Deleting a slot, or
overwriting it with a new game, clears it too.

The parts themselves are saved by the game, where they are:

* A shelf's parts sit on the shelf.
* A crate's parts are switched off inside it, the way the game's own inventory
  keeps things.

When a save loads, each piece finds its parts again by name and place. If the game
didn't save a part, the log says so.

## Install

Copy `FurnitureTweaks.dll` into `<game>/Mods/`, with Engine Cloner.

## How it works

* **Models:** each piece is built in code from boxes and cylinders, one mesh per
  material, with textures generated when the level loads (powder-coated steel with
  chips, worn wood with grain and knots, brass, chrome). It all uses the game's own
  Standard shader.
* **Shelf and crate:** a postfix on the game's `PickUp.LetGo` catches a part you
  let go of while looking at furniture. It skips throws and parts that just snapped
  onto a vehicle. A prefix on `PickUp.OnMouseDown` takes a part off its shelf when
  you pick it up.
* **Crate grid:** the pictures are rendered from the part's own meshes and
  materials, on a layer of their own below the world. They're filmed along the
  part's shortest side, with its long side across. Older crates (1.0) load with
  their parts placed into the grid automatically.
* **Safe:** cash goes in the way the ATM takes it (`Currency.money` and the
  inventory's bills). It comes out as the ATM's roll of bills.
