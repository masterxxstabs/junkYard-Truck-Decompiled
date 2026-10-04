# JunkAuto PC

A MelonLoader mod for Junkyard Truck that puts **JunkAuto.com**, a RockAuto-style
parts site, on the garage computer. It sits in a tab next to the game's email, and
order and delivery notices arrive as emails.

## Using it

Click **My Computer** as usual. The mail screen has a new link under
`[ Exit ]` / `[ Save Game ]`:

* **`[ JunkAuto.com ]`** opens the store (the link shows how many unread JunkAuto
  emails you have). The store's own green tab strip has **`[ Mail ]`** to go back.
  Esc or the game's Exit closes the PC as always.

### The store

* **Catalog:** your vehicles on the left (1982 Diamondback Pickup, 250cc Dirt
  Bike, Vehicles, Universal/Other). Open one with `[+]`, then pick a category:
  Engine, Fuel & Air, Ignition & Electrical, Cooling System, Exhaust & Emission,
  Transmission, Driveline & Axle, Brake & Wheel Hub, Suspension & Steering,
  Body & Accessories, Audio & Electronics or Miscellaneous. Parts are listed with
  part number and price; **Add to Cart**.
* **Cart:** change quantities, see subtotal, shipping and total, then **Place
  Order**. It's paid from your cash, the same way the Parts Store pays.
* **Order Status:** every order with its items, total, and time to arrival or
  "Delivered".

### Delivery and email

An order arrives after a few minutes of play (3 by default). It's left where you
were standing when you placed it, by the PC, unless "Deliver to wherever you are"
is on. Parts arrive brand new. You get two emails in the game's own inbox,
**"Order #… confirmed"** and **"Order #… delivered"**, shown as extra rows in the
same green style. Click one to read it. A pop-up also tells you when one arrives.

## What it sells

Everything the Junkyard Terminal's **Parts Store** sells (the ComputerPartStore
mod's catalog). That includes what our other mods add: Parts Store Plus's
missing dirt bike parts, Truck Parts QOL's stereo parts and the Junkyard ATV.
Without the Parts Store mod it sells what the junkyard can spawn. Parts are named
by their in-game description when they have a short one, otherwise by their
tidied-up object name. Categories are guessed from keywords in the name.

## Settings

Mods Menu → JUNKAUTO PC, or `[JunkAutoPC]` in `UserData/MelonPreferences.cfg`:

| Setting | Default | |
|---|---|---|
| Delivery time (minutes) | 3 | Minutes of play until an order arrives |
| Shipping cost | 9.99 | Per order |
| Free shipping over | 250 | Order size that ships free (0 = never) |
| Deliver to wherever you are | off | On: orders arrive in front of you |
| Email pop-ups | on | Pop-up when a JunkAuto email arrives |

## Saves

Orders (including ones still on the way) and JunkAuto emails are saved with the
game's save slots in `UserData/JunkAutoPC/slotN.txt` (`auto.txt` for the
autosave). The game's own email list and save are never changed. JunkAuto's
emails only borrow the look of its rows, so removing the mod leaves your saves as
they were.
