using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FurnitureTweaks
{
	// A piece of furniture this mod built: what it is and its id in the saves.
	public class Furniture : MonoBehaviour
	{
		public string kind;
		public string id;

		// Where its stored things are, after it's been moved.
		public virtual void Moved()
		{
		}

		// Its stored things, for the save ("" for none).
		public virtual string SaveData()
		{
			return "";
		}

		public virtual void LoadData(string data)
		{
		}

		// Restoring a save: picks its things back up as the game brings them
		// back. True when there's nothing left to find.
		public virtual bool Adopt()
		{
			return true;
		}

		public virtual void Forget()
		{
		}
	}

	// The loose parts of the game, by name, wherever they are (inactive too:
	// what's in a crate, what the game has just restored).
	internal static class Items
	{
		public static string NameOf(GameObject go)
		{
			return go.name.Replace("(Clone)", "").Trim();
		}

		public static string Label(PickUp p)
		{
			string d = p.description;
			if (!string.IsNullOrEmpty(d))
			{
				d = d.Trim();
				int nl = d.IndexOf('\n');
				if (nl > 0)
				{
					d = d.Substring(0, nl).Trim();
				}
				if (d.Length > 0 && d.Length <= 40)
				{
					return d;
				}
			}
			return NameOf(p.gameObject).Replace('_', ' ');
		}

		// The game's engine blocks: they have their own stands and mounts.
		public static bool IsEngine(GameObject go)
		{
			string n = NameOf(go);
			return n == "engineblock" || n == "v8_block" || n == "i6block" || n == "250_block";
		}

		// Things the game itself saves (Easy Save tracks them); others are saved
		// by the mod that made them, which only finds them while they're out.
		public static bool GameSaves(GameObject go)
		{
			foreach (Component c in go.GetComponents<Component>())
			{
				if (c != null && c.GetType().Name.StartsWith("ES3", StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}

		// The loose part named `name` nearest `at`, within `range` (active or not,
		// unless `active`), that isn't in `taken`.
		public static PickUp Nearest(string name, Vector3 at, float range, HashSet<PickUp> taken, bool active = false)
		{
			PickUp best = null;
			float bestDist = range * range;
			foreach (PickUp p in Resources.FindObjectsOfTypeAll<PickUp>())
			{
				if (p == null || !p.gameObject.scene.IsValid() || taken.Contains(p) || NameOf(p.gameObject) != name || active && !p.gameObject.activeInHierarchy)
				{
					continue;
				}
				float d = (p.transform.position - at).sqrMagnitude;
				if (d <= bestDist)
				{
					best = p;
					bestDist = d;
				}
			}
			return best;
		}

		public static Bounds BoundsOf(GameObject go)
		{
			Renderer[] rs = go.GetComponentsInChildren<Renderer>();
			Bounds b = new Bounds(go.transform.position, Vector3.zero);
			bool any = false;
			foreach (Renderer r in rs)
			{
				if (!r.enabled || !(r is MeshRenderer || r is SkinnedMeshRenderer))
				{
					continue;
				}
				if (!any)
				{
					b = r.bounds;
					any = true;
				}
				else
				{
					b.Encapsulate(r.bounds);
				}
			}
			return b;
		}

		public static void Freeze(GameObject go, bool frozen)
		{
			Rigidbody rb = go.GetComponent<Rigidbody>();
			if (rb == null)
			{
				return;
			}
			if (frozen)
			{
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}
			rb.isKinematic = frozen;
			rb.useGravity = !frozen;
		}

		private static MethodInfo forceGrab;

		// Into the player's hands, the way the game's inventory hands things back.
		public static void Grab(PickUp p)
		{
			if (forceGrab == null)
			{
				forceGrab = AccessTools.Method(p.GetType(), "ForceGrab");
			}
			if (forceGrab != null)
			{
				forceGrab.Invoke(p, null);
			}
		}

		public static string Escape(string s)
		{
			return (s ?? "").Replace(";", "").Replace("|", "").Replace("~", "");
		}
	}

	// --- Parts shelf. ---

	// Twelve spots, three on each of the four levels. Let go of a part while
	// you're looking at the shelf and it's put on the spot nearest where you're
	// looking, standing upright and held there; pick it up to take it off.
	public class PartsShelf : Furniture
	{
		private class Spot
		{
			public Vector3 at;           // on the deck, in the shelf's frame
			public PickUp item;
			public Vector3 localPos;     // the item's pose, in the shelf's frame
			public Quaternion localRot;
			public string pendingName;   // restoring a save
		}

		private readonly List<Spot> spots = new List<Spot>();

		private void Awake()
		{
			foreach (Vector3 s in Models.ShelfSlots())
			{
				spots.Add(new Spot { at = s });
			}
		}

		public int Count
		{
			get
			{
				int n = 0;
				foreach (Spot s in spots)
				{
					if (Holding(s))
					{
						n++;
					}
				}
				return n;
			}
		}

		public int Size
		{
			get { return spots.Count; }
		}

		// Still on the spot: not picked up, put in the inventory or knocked off.
		private bool Holding(Spot s)
		{
			if (s.item == null || !s.item.gameObject.activeInHierarchy || s.item.holding || s.item.transform.parent != null)
			{
				return false;
			}
			return (s.item.transform.position - transform.TransformPoint(s.localPos)).sqrMagnitude < 0.04f;
		}

		// Put a part on the free spot nearest `aim`. Null if it went on, else why not.
		public string Place(PickUp item, Vector3 aim)
		{
			Bounds before = Items.BoundsOf(item.gameObject);
			if (before.size.x > 1.3f || before.size.y > 1.3f || before.size.z > 1.3f)
			{
				return "That's too big for the shelf.";
			}
			Vector3 local = transform.InverseTransformPoint(aim);
			Spot best = null;
			float bestDist = float.MaxValue;
			for (int i = 0; i < spots.Count; i++)
			{
				Spot s = spots[i];
				if (Holding(s))
				{
					continue;
				}
				// Tall parts only fit on the top level.
				bool top = i >= spots.Count - Models.ShelfSpots.Length;
				if (!top && before.size.y > Models.ShelfClearance && Mathf.Max(before.size.x, before.size.z) > Models.ShelfClearance)
				{
					continue;
				}
				float d = (s.at - local).sqrMagnitude;
				if (d < bestDist)
				{
					best = s;
					bestDist = d;
				}
			}
			if (best == null)
			{
				return "The shelf is full.";
			}
			Put(best, item, true);
			return null;
		}

		// Stand it on the spot: upright, the way the game holds it, turned square
		// to the shelf (to the nearest quarter turn), resting on the deck.
		private void Put(Spot s, PickUp item, bool square)
		{
			Transform t = item.transform;
			Items.Freeze(item.gameObject, true);
			if (square)
			{
				float yaw = Mathf.Round(Mathf.DeltaAngle(transform.eulerAngles.y, t.eulerAngles.y) / 90f) * 90f;
				t.rotation = transform.rotation * Quaternion.Euler(0f, yaw, 0f);
			}
			Vector3 deck = transform.TransformPoint(s.at);
			t.position = deck + Vector3.up * 0.5f;
			Bounds b = Items.BoundsOf(item.gameObject);
			t.position += new Vector3(deck.x - b.center.x, deck.y + 0.004f - b.min.y, deck.z - b.center.z);
			s.item = item;
			s.localPos = transform.InverseTransformPoint(t.position);
			s.localRot = Quaternion.Inverse(transform.rotation) * t.rotation;
			s.pendingName = null;
		}

		// Picked up: off the shelf.
		public bool Release(PickUp item)
		{
			foreach (Spot s in spots)
			{
				if (s.item == item)
				{
					s.item = null;
					return true;
				}
			}
			return false;
		}

		public override void Moved()
		{
			foreach (Spot s in spots)
			{
				if (s.item != null && s.item.gameObject.activeInHierarchy && !s.item.holding && s.item.transform.parent == null)
				{
					s.item.transform.SetPositionAndRotation(transform.TransformPoint(s.localPos), transform.rotation * s.localRot);
				}
			}
		}

		// "spot~name~x~y~z~qx~qy~qz~qw|..." (the pose in the shelf's frame).
		public override string SaveData()
		{
			List<string> parts = new List<string>();
			for (int i = 0; i < spots.Count; i++)
			{
				Spot s = spots[i];
				if (!Holding(s))
				{
					continue;
				}
				parts.Add(string.Join("~", new[]
				{
					i.ToString(Save.Inv), Items.Escape(Items.NameOf(s.item.gameObject)),
					Save.F(s.localPos.x), Save.F(s.localPos.y), Save.F(s.localPos.z),
					Save.F(s.localRot.x), Save.F(s.localRot.y), Save.F(s.localRot.z), Save.F(s.localRot.w)
				}));
			}
			return string.Join("|", parts.ToArray());
		}

		public override void LoadData(string data)
		{
			foreach (string entry in (data ?? "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
			{
				string[] f = entry.Split('~');
				int i;
				if (f.Length < 9 || !int.TryParse(f[0], out i) || i < 0 || i >= spots.Count)
				{
					continue;
				}
				try
				{
					Spot s = spots[i];
					s.pendingName = f[1];
					s.localPos = new Vector3(Save.P(f[2]), Save.P(f[3]), Save.P(f[4]));
					s.localRot = new Quaternion(Save.P(f[5]), Save.P(f[6]), Save.P(f[7]), Save.P(f[8]));
				}
				catch (FormatException)
				{
				}
			}
		}

		public override bool Adopt()
		{
			bool done = true;
			foreach (Spot s in spots)
			{
				if (s.pendingName == null)
				{
					continue;
				}
				// Within the shelf's height: if the game put it back before the shelf
				// was, it fell to the floor.
				PickUp p = Items.Nearest(s.pendingName, transform.TransformPoint(s.localPos), 2.2f, FurnitureMod.Claimed(), true);
				if (p == null)
				{
					done = false;
					continue;
				}
				p.transform.SetPositionAndRotation(transform.TransformPoint(s.localPos), transform.rotation * s.localRot);
				Items.Freeze(p.gameObject, true);
				s.item = p;
				s.pendingName = null;
			}
			return done;
		}

		public override void Forget()
		{
			foreach (Spot s in spots)
			{
				s.pendingName = null;
			}
		}

		public void Claim(HashSet<PickUp> into)
		{
			foreach (Spot s in spots)
			{
				if (s.item != null)
				{
					into.Add(s.item);
				}
			}
		}
	}

	// --- Storage crate. ---

	// Let go of a part while you're looking at the crate and it goes in (out of
	// sight, like the game's own inventory keeps things). Look at the crate with
	// your hands free to see what's in it; scroll to choose, and the take-out
	// key hands it to you.
	public class StorageCrate : Furniture
	{
		public const int Capacity = 30;

		public readonly List<PickUp> items = new List<PickUp>();
		public int selected;
		public Transform lid;
		public float open;
		public bool wantOpen;

		private readonly List<string> pending = new List<string>();

		// Each stored thing has its own spot inside, a few millimeters apart, so a
		// restored save can tell them apart.
		private Vector3 SpotOf(int i)
		{
			return transform.TransformPoint(Models.CrateInside + new Vector3(((i % 6) - 2.5f) * 0.12f, (i / 6) * 0.04f, 0f));
		}

		public string Stow(PickUp item)
		{
			Tidy();
			if (items.Count >= Capacity)
			{
				return "The crate is full.";
			}
			if (Items.IsEngine(item.gameObject))
			{
				return "Engine blocks don't go in a crate; use an engine stand.";
			}
			if (!Items.GameSaves(item.gameObject))
			{
				return "That part is saved by its own mod, so it can't be kept in a crate. Put it on a shelf.";
			}
			Items.Freeze(item.gameObject, true);
			item.transform.position = SpotOf(items.Count);
			item.gameObject.SetActive(false);
			items.Add(item);
			selected = items.Count - 1;
			return null;
		}

		// Drops anything that's gone (destroyed, or taken out some other way).
		public void Tidy()
		{
			items.RemoveAll(p => p == null || p.gameObject.activeSelf);
			if (selected >= items.Count)
			{
				selected = Mathf.Max(0, items.Count - 1);
			}
		}

		public PickUp Selected
		{
			get
			{
				Tidy();
				return items.Count > 0 ? items[Mathf.Clamp(selected, 0, items.Count - 1)] : null;
			}
		}

		public void Scroll(int by)
		{
			Tidy();
			if (items.Count > 0)
			{
				selected = ((selected + by) % items.Count + items.Count) % items.Count;
			}
		}

		// Out, into your hands.
		public void TakeOut(Camera cam)
		{
			PickUp p = Selected;
			if (p == null)
			{
				return;
			}
			items.Remove(p);
			// The rest keep their spots in order.
			Moved();
			Tidy();
			Transform t = p.transform;
			t.position = cam.transform.position + cam.transform.forward * 0.9f - Vector3.up * 0.2f;
			t.rotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
			p.gameObject.SetActive(true);
			Items.Freeze(p.gameObject, false);
			p.LetGo(0);
			Items.Grab(p);
		}

		public override void Moved()
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i] != null)
				{
					items[i].transform.position = SpotOf(i);
				}
			}
		}

		private void Update()
		{
			float target = wantOpen ? 1f : 0f;
			wantOpen = false;
			open = Mathf.MoveTowards(open, target, Time.deltaTime * 2.5f);
			if (lid != null)
			{
				lid.localRotation = Quaternion.Euler(-Mathf.SmoothStep(0f, 1f, open) * 75f, 0f, 0f);
			}
		}

		// "name|name|..." in order (each at its own spot).
		public override string SaveData()
		{
			Tidy();
			List<string> names = new List<string>();
			foreach (PickUp p in items)
			{
				names.Add(Items.Escape(Items.NameOf(p.gameObject)));
			}
			return string.Join("|", names.ToArray());
		}

		public override void LoadData(string data)
		{
			pending.Clear();
			pending.AddRange((data ?? "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
		}

		// The game brings back what was in the crate as it was saved: switched off,
		// at its spot inside. Each is picked up by name at its spot.
		public override bool Adopt()
		{
			if (pending.Count == 0)
			{
				return true;
			}
			HashSet<PickUp> claimed = FurnitureMod.Claimed();
			bool done = true;
			List<PickUp> found = new List<PickUp>();
			for (int i = 0; i < pending.Count; i++)
			{
				if (pending[i] == null)
				{
					found.Add(null);
					continue;
				}
				PickUp p = Items.Nearest(pending[i], SpotOf(i), 0.05f, claimed);
				if (p == null)
				{
					done = false;
				}
				else
				{
					claimed.Add(p);
				}
				found.Add(p);
			}
			if (!done)
			{
				return false;
			}
			items.Clear();
			foreach (PickUp p in found)
			{
				if (p != null)
				{
					Items.Freeze(p.gameObject, true);
					p.gameObject.SetActive(false);
					items.Add(p);
				}
			}
			pending.Clear();
			Moved();
			return true;
		}

		// Gave up waiting: keep what was found.
		public override void Forget()
		{
			HashSet<PickUp> claimed = FurnitureMod.Claimed();
			int lost = 0;
			for (int i = 0; i < pending.Count; i++)
			{
				PickUp p = Items.Nearest(pending[i], SpotOf(i), 0.05f, claimed);
				if (p == null)
				{
					lost++;
					continue;
				}
				claimed.Add(p);
				Items.Freeze(p.gameObject, true);
				p.gameObject.SetActive(false);
				items.Add(p);
			}
			if (lost > 0)
			{
				FurnitureMod.Log("Crate " + id + ": " + lost + " of " + pending.Count + " stored part(s) weren't in the game's save; they're gone.");
			}
			pending.Clear();
			Moved();
		}

		public void Claim(HashSet<PickUp> into)
		{
			foreach (PickUp p in items)
			{
				if (p != null)
				{
					into.Add(p);
				}
			}
		}
	}

	// --- Safe. ---

	// Money kept apart from your wallet. Look at it and scroll: up puts cash in,
	// down takes it out as a roll of bills, like the ATM's (hold Shift for more).
	// A roll of cash let go of while you're looking at it goes in too.
	public class Safe : Furniture
	{
		public float cash;

		public override string SaveData()
		{
			return Save.F(cash);
		}

		public override void LoadData(string data)
		{
			float v;
			if (float.TryParse(data, System.Globalization.NumberStyles.Float, Save.Inv, out v))
			{
				cash = Mathf.Max(0f, v);
			}
		}
	}
}
