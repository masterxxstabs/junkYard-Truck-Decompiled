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

		// A crate cell, in meters.
		public const float Cell = 0.18f;

		// The part's size in its own frame (switched off or not): every visible
		// mesh's bounds, in the part's local space.
		public static Bounds LocalBounds(Transform root)
		{
			Bounds b = new Bounds(Vector3.zero, Vector3.zero);
			bool any = false;
			foreach (MeshFilter mf in root.GetComponentsInChildren<MeshFilter>(true))
			{
				Renderer r = mf.GetComponent<Renderer>();
				if (mf.sharedMesh == null || r == null || !r.enabled || !ShownUnder(mf.transform, root))
				{
					continue;
				}
				Bounds mb = mf.sharedMesh.bounds;
				for (int i = 0; i < 8; i++)
				{
					Vector3 corner = mb.center + Vector3.Scale(mb.extents, new Vector3((i & 1) == 0 ? -1f : 1f, (i & 2) == 0 ? -1f : 1f, (i & 4) == 0 ? -1f : 1f));
					Vector3 p = root.InverseTransformPoint(mf.transform.TransformPoint(corner));
					if (!any)
					{
						b = new Bounds(p, Vector3.zero);
						any = true;
					}
					else
					{
						b.Encapsulate(p);
					}
				}
			}
			return b;
		}

		// Shown when the part is: every object between it and the part is on.
		public static bool ShownUnder(Transform t, Transform root)
		{
			for (; t != null && t != root; t = t.parent)
			{
				if (!t.gameObject.activeSelf)
				{
					return false;
				}
			}
			return true;
		}

		// Its three sides in meters, shortest first.
		public static float[] Sides(PickUp p)
		{
			Vector3 s = Vector3.Scale(LocalBounds(p.transform).size, p.transform.lossyScale);
			float[] d = { Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z) };
			Array.Sort(d);
			return d;
		}

		// A rotation that lays the part on its broadest side (its shortest axis
		// up) with its longest axis along `along`.
		public static Quaternion LyingFlat(Transform t, Vector3 along)
		{
			Vector3 size = LocalBounds(t).size;
			Vector3[] axes = { Vector3.right, Vector3.up, Vector3.forward };
			float[] len = { Mathf.Abs(size.x * t.lossyScale.x), Mathf.Abs(size.y * t.lossyScale.y), Mathf.Abs(size.z * t.lossyScale.z) };
			int[] order = { 0, 1, 2 };
			Array.Sort((float[])len.Clone(), order);
			Vector3 longest = axes[order[2]], shortest = axes[order[0]];
			// Which way up and which end first: whichever is nearer how it's held now.
			if (Vector3.Dot(t.rotation * shortest, Vector3.up) < 0f)
			{
				shortest = -shortest;
			}
			if (Vector3.Dot(t.rotation * longest, along) < 0f)
			{
				longest = -longest;
			}
			Vector3 flatAlong = Vector3.ProjectOnPlane(along, Vector3.up).normalized;
			return Quaternion.LookRotation(flatAlong, Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(longest, shortest));
		}

		// Its size in crate cells, unturned (w >= h): its two longest sides.
		public static void Footprint(PickUp p, out int w, out int h)
		{
			Vector3 s = LocalBounds(p.transform).size;
			s = Vector3.Scale(s, p.transform.lossyScale);
			float[] d = { Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z) };
			Array.Sort(d);
			w = Mathf.Clamp(Mathf.CeilToInt(d[2] / Cell - 0.15f), 1, StorageCrate.Cols);
			h = Mathf.Clamp(Mathf.CeilToInt(d[1] / Cell - 0.15f), 1, StorageCrate.Rows);
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
			// Laid on its broadest side: its shortest side is its height.
			float[] d = Items.Sides(item);
			if (d[2] > 1.6f)
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
				if (!top && d[0] > Models.ShelfClearance)
				{
					continue;
				}
				float dist = (s.at - local).sqrMagnitude;
				if (dist < bestDist)
				{
					best = s;
					bestDist = dist;
				}
			}
			if (best == null)
			{
				return "The shelf is full.";
			}
			Put(best, item, true);
			return null;
		}

		// Put it on the spot the way you'd shelve it: lying on its broadest side,
		// its long side along the shelf, resting on the deck.
		private void Put(Spot s, PickUp item, bool square)
		{
			Transform t = item.transform;
			Items.Freeze(item.gameObject, true);
			if (square)
			{
				t.rotation = Items.LyingFlat(t, transform.right);
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

	// A grid inventory, like Tarkov's: 10 x 8 cells, each part taking as many as
	// its size (a spark plug 1 x 1, a transmission 4 x 2), turned either way. Let
	// go of a part while you're looking at the crate and it goes into the first
	// place it fits (out of sight, like the game's own inventory keeps things).
	// Open it (the open key) to see the parts laid out, drag them around, turn
	// them and take them out (CrateWindow).
	public class StorageCrate : Furniture
	{
		public const int Cols = 10;
		public const int Rows = 8;

		public class Slot
		{
			public PickUp item;
			public int x, y;          // top-left cell
			public bool turned;       // turned a quarter: w and h swapped
			public int w, h;          // the part's size in cells, unturned (w >= h)
			public int spot;          // its hiding place inside (see SpotOf)
			public string pendingName; // restoring a save
			public bool legacy;       // saved by 1.0 (its spot worked out the old way)

			public int W
			{
				get { return turned ? h : w; }
			}

			public int H
			{
				get { return turned ? w : h; }
			}
		}

		public readonly List<Slot> slots = new List<Slot>();
		public Transform lid;
		public float open;
		public bool wantOpen;

		// Each stored thing has its own spot inside, a few millimeters apart, so a
		// restored save can tell them apart.
		private Vector3 SpotOf(int i)
		{
			return transform.TransformPoint(Models.CrateInside + new Vector3(((i % 8) - 3.5f) * 0.1f, (i / 8) * 0.03f, 0f));
		}

		// Where 1.0 kept the i-th part.
		private Vector3 LegacySpotOf(int i)
		{
			return transform.TransformPoint(Models.CrateInside + new Vector3(((i % 6) - 2.5f) * 0.12f, (i / 6) * 0.04f, 0f));
		}

		private Vector3 SavedSpotOf(Slot s)
		{
			return s.legacy ? LegacySpotOf(s.spot) : SpotOf(s.spot);
		}

		private int FreeSpot()
		{
			for (int i = 0; ; i++)
			{
				if (!slots.Exists(s => s.spot == i))
				{
					return i;
				}
			}
		}

		public int Count
		{
			get
			{
				Tidy();
				return slots.Count;
			}
		}

		public int UsedCells
		{
			get
			{
				int n = 0;
				foreach (Slot s in slots)
				{
					n += s.w * s.h;
				}
				return n;
			}
		}

		// Whether a w x h part fits with its top-left at (x, y), leaving out `skip`.
		public bool Fits(int x, int y, int w, int h, Slot skip)
		{
			if (x < 0 || y < 0 || x + w > Cols || y + h > Rows)
			{
				return false;
			}
			foreach (Slot s in slots)
			{
				if (s == skip || s.x < 0)
				{
					continue;
				}
				if (x < s.x + s.W && s.x < x + w && y < s.y + s.H && s.y < y + h)
				{
					return false;
				}
			}
			return true;
		}

		// The first place a w x h part fits, top row first, as it is or turned.
		public bool FindPlace(int w, int h, Slot skip, out int px, out int py, out bool turned)
		{
			foreach (bool t in new[] { false, true })
			{
				int ww = t ? h : w, hh = t ? w : h;
				for (int y = 0; y < Rows; y++)
				{
					for (int x = 0; x < Cols; x++)
					{
						if (Fits(x, y, ww, hh, skip))
						{
							px = x;
							py = y;
							turned = t;
							return true;
						}
					}
				}
			}
			px = py = 0;
			turned = false;
			return false;
		}

		public string Stow(PickUp item)
		{
			Tidy();
			if (Items.IsEngine(item.gameObject))
			{
				return "Engine blocks don't go in a crate; use an engine stand.";
			}
			if (!Items.GameSaves(item.gameObject))
			{
				return "That part is saved by its own mod, so it can't be kept in a crate. Put it on a shelf.";
			}
			int w, h;
			Items.Footprint(item, out w, out h);
			int x, y;
			bool turned;
			if (!FindPlace(w, h, null, out x, out y, out turned))
			{
				return "No room in the crate for the " + Items.Label(item) + " (" + w + " x " + h + ").";
			}
			Slot s = new Slot { item = item, x = x, y = y, turned = turned, w = w, h = h, spot = FreeSpot() };
			slots.Add(s);
			Hide(s);
			return null;
		}

		private void Hide(Slot s)
		{
			Items.Freeze(s.item.gameObject, true);
			s.item.transform.position = SpotOf(s.spot);
			s.item.gameObject.SetActive(false);
		}

		// Drops anything that's gone (destroyed, or taken out some other way).
		public void Tidy()
		{
			slots.RemoveAll(s => s.pendingName == null && (s.item == null || s.item.gameObject.activeSelf));
		}

		// Out, into your hands.
		public void TakeOut(Slot s, Camera cam)
		{
			if (s == null || s.item == null || !slots.Contains(s))
			{
				return;
			}
			slots.Remove(s);
			PickUp p = s.item;
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
			foreach (Slot s in slots)
			{
				if (s.item != null)
				{
					s.item.transform.position = SpotOf(s.spot);
				}
			}
		}

		private void Update()
		{
			float target = wantOpen || CrateWindow.IsOpen(this) ? 1f : 0f;
			wantOpen = false;
			open = Mathf.MoveTowards(open, target, Time.deltaTime * 2.5f);
			if (lid != null)
			{
				lid.localRotation = Quaternion.Euler(-Mathf.SmoothStep(0f, 1f, open) * 75f, 0f, 0f);
			}
		}

		// "name~x~y~turned~spot|..." (1.0 saved just "name|name|...", the spot
		// being its place in the list).
		public override string SaveData()
		{
			Tidy();
			List<string> parts = new List<string>();
			foreach (Slot s in slots)
			{
				string name = s.item != null ? Items.NameOf(s.item.gameObject) : s.pendingName;
				parts.Add(Items.Escape(name) + "~" + s.x + "~" + s.y + "~" + (s.turned ? 1 : 0) + "~" + s.spot);
			}
			return string.Join("|", parts.ToArray());
		}

		public override void LoadData(string data)
		{
			slots.Clear();
			string[] entries = (data ?? "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < entries.Length; i++)
			{
				string[] f = entries[i].Split('~');
				Slot s = new Slot { pendingName = f[0], spot = i, x = -1, y = -1, legacy = true };
				int v;
				if (f.Length >= 5 && int.TryParse(f[1], out s.x) && int.TryParse(f[2], out s.y) && int.TryParse(f[3], out v) && int.TryParse(f[4], out s.spot))
				{
					s.turned = v == 1;
					s.legacy = false;
				}
				else
				{
					s.x = s.y = -1;
					s.spot = i;
				}
				slots.Add(s);
			}
		}

		// The game brings back what was in the crate as it was saved: switched off,
		// at its spot inside. Each is picked up by name at its spot.
		public override bool Adopt()
		{
			if (!slots.Exists(s => s.pendingName != null))
			{
				return true;
			}
			HashSet<PickUp> claimed = FurnitureMod.Claimed();
			Dictionary<Slot, PickUp> found = new Dictionary<Slot, PickUp>();
			foreach (Slot s in slots)
			{
				if (s.pendingName == null)
				{
					continue;
				}
				PickUp p = Items.Nearest(s.pendingName, SavedSpotOf(s), 0.05f, claimed);
				if (p == null)
				{
					return false;
				}
				claimed.Add(p);
				found[s] = p;
			}
			foreach (KeyValuePair<Slot, PickUp> pair in found)
			{
				Settle(pair.Key, pair.Value);
			}
			return true;
		}

		// Gave up waiting: keep what was found.
		public override void Forget()
		{
			HashSet<PickUp> claimed = FurnitureMod.Claimed();
			int lost = 0, total = 0;
			foreach (Slot s in new List<Slot>(slots))
			{
				if (s.pendingName == null)
				{
					continue;
				}
				total++;
				PickUp p = Items.Nearest(s.pendingName, SavedSpotOf(s), 0.05f, claimed);
				if (p == null)
				{
					slots.Remove(s);
					lost++;
					continue;
				}
				claimed.Add(p);
				Settle(s, p);
			}
			if (lost > 0)
			{
				FurnitureMod.Log("Crate " + id + ": " + lost + " of " + total + " stored part(s) weren't in the game's save; they're gone.");
			}
		}

		// A restored part in its slot: its size measured, its place kept if it still
		// fits there (else the first free one).
		private void Settle(Slot s, PickUp p)
		{
			s.item = p;
			s.pendingName = null;
			s.legacy = false;
			Items.Footprint(p, out s.w, out s.h);
			if (s.x < 0 || !Fits(s.x, s.y, s.W, s.H, s))
			{
				int x, y;
				bool turned;
				if (FindPlace(s.w, s.h, s, out x, out y, out turned))
				{
					s.x = x;
					s.y = y;
					s.turned = turned;
				}
				else
				{
					// Over full: still kept, parked at the corner until there's room.
					s.x = 0;
					s.y = 0;
					s.turned = false;
				}
			}
			Hide(s);
		}

		public void Claim(HashSet<PickUp> into)
		{
			foreach (Slot s in slots)
			{
				if (s.item != null)
				{
					into.Add(s.item);
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
