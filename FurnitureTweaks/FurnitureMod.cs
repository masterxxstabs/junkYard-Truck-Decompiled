using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EngineCloner;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(FurnitureTweaks.FurnitureMod), "Furniture Tweaks", "1.0.0", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace FurnitureTweaks
{
	// Garage furniture to keep things in: a parts shelf to pop parts on and off,
	// a safe for cash, and storage crates. Bought in the shops (JunkAuto's Garage
	// Furniture section, the Parts Store), delivered, and placed where you want
	// them; saved with your save slot (through Engine Cloner's save hooks).
	public class FurnitureMod : MelonMod
	{
		private static MelonLogger.Instance log;
		private static MelonPreferences_Entry<float> shelfPrice, safePrice, cratePrice;
		private static MelonPreferences_Entry<KeyCode> moveKey, takeKey;
		private static MelonPreferences_Entry<float> safeStep;

		public static float ShelfPrice { get { return shelfPrice != null ? shelfPrice.Value : 450f; } }
		public static float SafePrice { get { return safePrice != null ? safePrice.Value : 900f; } }
		public static float CratePrice { get { return cratePrice != null ? cratePrice.Value : 120f; } }
		public static KeyCode MoveKey { get { return moveKey != null ? moveKey.Value : KeyCode.U; } }
		public static KeyCode TakeKey { get { return takeKey != null ? takeKey.Value : KeyCode.R; } }
		public static float SafeStep { get { return safeStep != null ? Mathf.Max(1f, safeStep.Value) : 100f; } }

		public static string Folder
		{
			get { return Path.Combine(Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"), "FurnitureTweaks"); }
		}

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category c = MelonPreferences.CreateCategory("FurnitureTweaks", "Furniture Tweaks");
			shelfPrice = c.CreateEntry("ShelfPrice", 450f, "Parts shelf price", "What a parts shelf costs in the shops.");
			safePrice = c.CreateEntry("SafePrice", 900f, "Safe price", "What a safe costs in the shops.");
			cratePrice = c.CreateEntry("CratePrice", 120f, "Storage crate price", "What a storage crate costs in the shops.");
			moveKey = c.CreateEntry("MoveKey", KeyCode.U, "Move key", "Look at a piece of furniture and press this to pick it up and move it; press it again to put it down.");
			takeKey = c.CreateEntry("TakeKey", KeyCode.R, "Take out key", "Look at a crate with your hands free and press this to take out the part shown.");
			safeStep = c.CreateEntry("SafeStep", 100f, "Safe step", "How much cash each scroll puts in or takes out of a safe (hold Shift for ten times as much).");
			ShopProviders.Register(Shop.ShopItems);
			VehicleApi.Saving += Save.Write;
			VehicleApi.Loading += Save.Read;
			VehicleApi.Deleting += Save.Delete;
			Log("Ready.");
		}

		public override void OnLateInitializeMelon()
		{
			Shop.HookStore(HarmonyInstance);
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			Looks.Reset();
			Interact.Reset();
			Save.Reset();
			Shop.HookStore(HarmonyInstance);
		}

		public override void OnUpdate()
		{
			Save.Update();
			Interact.Update();
		}

		public override void OnGUI()
		{
			Interact.Draw();
		}

		// Every part a piece of furniture is holding (so two don't claim one).
		public static HashSet<PickUp> Claimed()
		{
			HashSet<PickUp> set = new HashSet<PickUp>();
			foreach (PartsShelf s in Object.FindObjectsOfType<PartsShelf>())
			{
				s.Claim(set);
			}
			foreach (StorageCrate c in Object.FindObjectsOfType<StorageCrate>())
			{
				c.Claim(set);
			}
			return set;
		}

		public static void Log(string message)
		{
			if (log != null)
			{
				log.Msg(message);
			}
			else
			{
				MelonLogger.Msg("[Furniture Tweaks] " + message);
			}
		}
	}

	// --- Building a piece. ---

	internal static class Build
	{
		public const string Shelf = "shelf", SafeKind = "safe", Crate = "crate";
		public static readonly string[] Kinds = { Shelf, SafeKind, Crate };

		public static string NameOf(string kind)
		{
			return kind == Shelf ? "parts shelf" : kind == SafeKind ? "safe" : "storage crate";
		}

		// A new piece standing on the ground at `at`, facing `face`.
		public static Furniture Make(string kind, Vector3 at, float yaw, string id = null)
		{
			GameObject go = new GameObject("Furniture_" + kind);
			Furniture f;
			if (kind == Shelf)
			{
				f = go.AddComponent<PartsShelf>();
				Models.Shelf(go);
			}
			else if (kind == SafeKind)
			{
				f = go.AddComponent<Safe>();
				Models.Safe(go);
			}
			else if (kind == Crate)
			{
				StorageCrate crate = go.AddComponent<StorageCrate>();
				crate.lid = Models.Crate(go).transform;
				f = crate;
			}
			else
			{
				Object.Destroy(go);
				return null;
			}
			f.kind = kind;
			f.id = id ?? kind + "-" + DateTime.UtcNow.Ticks.ToString("x").Substring(5);
			go.transform.SetPositionAndRotation(at, Quaternion.Euler(0f, yaw, 0f));
			return f;
		}

		// The ground under (or at) a point.
		public static Vector3 Ground(Vector3 near, Collider[] ignore = null)
		{
			foreach (RaycastHit hit in Sorted(Physics.RaycastAll(near + Vector3.up * 1.5f, Vector3.down, 8f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)))
			{
				if (ignore != null && Array.IndexOf(ignore, hit.collider) >= 0)
				{
					continue;
				}
				if (hit.collider.attachedRigidbody != null && !hit.collider.attachedRigidbody.isKinematic)
				{
					continue;   // loose things on the floor
				}
				return hit.point;
			}
			return near;
		}

		public static RaycastHit[] Sorted(RaycastHit[] hits)
		{
			Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
			return hits;
		}
	}

	// --- The shops. ---

	// Sold in JunkAuto (the shared shop list, as "Furniture - ...", which it lists
	// under Garage Furniture) and in the Parts Store. What's sold is a marker: the
	// shop puts it where it delivers, and the piece is built there.
	internal static class Shop
	{
		private const string Category = "Truck Part";
		private static GameObject holder;
		private static readonly Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();
		private static FieldInfo prefabField, nameField, priceField, categoryField;
		private static Type entryType;
		private static bool hooked;

		public static string ShopName(string kind)
		{
			return kind == Build.Shelf ? "Furniture - Parts shelf" : kind == Build.SafeKind ? "Furniture - Safe" : "Furniture - Storage crate";
		}

		private static float Price(string kind)
		{
			return kind == Build.Shelf ? FurnitureMod.ShelfPrice : kind == Build.SafeKind ? FurnitureMod.SafePrice : FurnitureMod.CratePrice;
		}

		private static GameObject Marker(string kind)
		{
			GameObject m;
			if (markers.TryGetValue(kind, out m) && m != null)
			{
				return m;
			}
			if (holder == null)
			{
				holder = new GameObject("FurnitureTweaks_Shop");
				holder.SetActive(false);
				Object.DontDestroyOnLoad(holder);
			}
			m = new GameObject(ShopName(kind) + " (delivery)");
			m.transform.SetParent(holder.transform, false);
			m.AddComponent<FurnitureDelivery>().kind = kind;
			markers[kind] = m;
			return m;
		}

		public static object[][] ShopItems()
		{
			List<object[]> items = new List<object[]>();
			foreach (string k in Build.Kinds)
			{
				items.Add(new object[] { Marker(k), ShopName(k), Price(k), Category });
			}
			return items.ToArray();
		}

		public static void HookStore(HarmonyLib.Harmony harmony)
		{
			if (hooked)
			{
				return;
			}
			Type store = null;
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				store = a.GetType("ComputerFeatures.PartsStoreService", false);
				if (store != null)
				{
					break;
				}
			}
			if (store == null)
			{
				return;
			}
			MethodInfo getCatalog = store.GetMethod("GetCatalog", BindingFlags.Static | BindingFlags.Public);
			entryType = store.GetNestedType("PartEntry", BindingFlags.Public | BindingFlags.NonPublic);
			if (getCatalog == null || entryType == null)
			{
				return;
			}
			prefabField = entryType.GetField("prefab");
			nameField = entryType.GetField("displayName");
			priceField = entryType.GetField("price");
			categoryField = entryType.GetField("category");
			if (prefabField == null || nameField == null || priceField == null || categoryField == null)
			{
				return;
			}
			harmony.Patch(getCatalog, null, new HarmonyMethod(typeof(Shop).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));
			hooked = true;
			FurnitureMod.Log("Furniture is sold in the Parts Store.");
		}

		private static void Postfix(object __result)
		{
			System.Collections.IList list = __result as System.Collections.IList;
			if (list == null)
			{
				return;
			}
			try
			{
				foreach (string k in Build.Kinds)
				{
					string name = ShopName(k);
					object found = null;
					foreach (object entry in list)
					{
						if ((string)nameField.GetValue(entry) == name)
						{
							found = entry;
							break;
						}
					}
					if (found == null)
					{
						found = Activator.CreateInstance(entryType);
						nameField.SetValue(found, name);
						categoryField.SetValue(found, Category);
						list.Add(found);
					}
					prefabField.SetValue(found, Marker(k));
					priceField.SetValue(found, Price(k));
				}
			}
			catch (Exception e)
			{
				FurnitureMod.Log("Couldn't list the furniture in the Parts Store: " + e.Message);
			}
		}
	}

	// What a shop delivers: the piece, standing on the ground where it lands,
	// facing you.
	public class FurnitureDelivery : MonoBehaviour
	{
		public string kind;

		private void Start()
		{
			Camera cam = Camera.main;
			Vector3 at = Build.Ground(transform.position);
			Vector3 face = cam != null ? Vector3.ProjectOnPlane(cam.transform.position - at, Vector3.up) : Vector3.forward;
			float yaw = face.sqrMagnitude > 0.01f ? Quaternion.LookRotation(face).eulerAngles.y : 0f;
			Furniture f = Build.Make(kind, at, yaw);
			if (f != null)
			{
				Interact.Hint("Your " + Build.NameOf(kind) + " has been delivered. Look at it and press [" + FurnitureMod.MoveKey + "] to move it.", 8f);
				FurnitureMod.Log("Delivered a " + Build.NameOf(kind) + " (" + f.id + ").");
			}
			Destroy(gameObject);
		}
	}

	// --- Looking at furniture, using it, moving it. ---

	internal static class Interact
	{
		private const float Reach = 3.2f;

		private static string hint = "";
		private static float hintUntil;
		private static string look = "";

		// Moving a piece.
		private static Furniture moving;
		private static Vector3 movingFrom;
		private static Quaternion movingRot;
		private static float movingYaw;
		private static Collider[] movingColliders;

		private static float nextScroll;

		private static Component interactor;
		private static FieldInfo heldField, invField, currencyField, moneyField;
		private static MethodInfo subtractMoney;

		public static void Reset()
		{
			moving = null;
			interactor = null;
			look = "";
		}

		public static void Hint(string text, float seconds = 5f)
		{
			hint = text;
			hintUntil = Time.time + seconds;
		}

		private static Component Interactor()
		{
			if (interactor == null)
			{
				interactor = Object.FindObjectOfType<Interactor>();
				if (interactor != null && heldField == null)
				{
					Type t = interactor.GetType();
					heldField = AccessTools.Field(t, "pickedUpObject");
					invField = AccessTools.Field(t, "inv");
					currencyField = AccessTools.Field(t, "currency");
				}
			}
			return interactor;
		}

		// What you're holding, if anything.
		private static PickUp Held()
		{
			Component i = Interactor();
			GameObject go = i != null && heldField != null ? heldField.GetValue(i) as GameObject : null;
			PickUp p = go != null ? go.GetComponent<PickUp>() : null;
			return p != null && p.holding ? p : null;
		}

		// The furniture you're looking at (and where), within reach.
		private static Furniture Aimed(Camera cam, out Vector3 point, GameObject skip = null)
		{
			point = Vector3.zero;
			foreach (RaycastHit hit in Build.Sorted(Physics.RaycastAll(cam.transform.position, cam.transform.forward, Reach, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)))
			{
				if (skip != null && hit.collider.transform.IsChildOf(skip.transform))
				{
					continue;
				}
				if (hit.collider.GetComponentInParent<PickUp>() != null && hit.collider.GetComponentInParent<Furniture>() == null)
				{
					// A loose part in the way (on the shelf, say): look past it, but
					// only as far as the furniture behind it.
					continue;
				}
				Furniture f = hit.collider.GetComponentInParent<Furniture>();
				point = hit.point;
				return f;
			}
			return null;
		}

		public static void Update()
		{
			look = "";
			Camera cam = Camera.main;
			if (cam == null)
			{
				return;
			}
			if (moving != null)
			{
				UpdateMove(cam);
				return;
			}
			if (VehicleApi.PlayerInVehicle())
			{
				return;
			}
			PickUp held = Held();
			Vector3 point;
			Furniture f = Aimed(cam, out point, held != null ? held.gameObject : null);
			if (f == null)
			{
				return;
			}
			string move = "[" + FurnitureMod.MoveKey + "] move it";
			if (held == null && Input.GetKeyDown(FurnitureMod.MoveKey))
			{
				StartMove(f);
				return;
			}
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			int step = 0;
			if (Mathf.Abs(scroll) > 0.01f && Time.time >= nextScroll)
			{
				step = scroll > 0f ? 1 : -1;
				nextScroll = Time.time + 0.12f;
			}
			PartsShelf shelf = f as PartsShelf;
			StorageCrate crate = f as StorageCrate;
			Safe safe = f as Safe;
			if (shelf != null)
			{
				look = "Parts shelf (" + shelf.Count + "/" + shelf.Size + ")\n" + (held != null ? "Let go to put the " + Items.Label(held) + " on the shelf" : "Pick a part up to take it off · " + move);
			}
			else if (crate != null)
			{
				crate.wantOpen = true;
				crate.Tidy();
				string head = "Storage crate (" + crate.items.Count + "/" + StorageCrate.Capacity + ")\n";
				if (held != null)
				{
					look = head + "Let go to put the " + Items.Label(held) + " in";
				}
				else
				{
					if (step != 0)
					{
						crate.Scroll(-step);
					}
					PickUp sel = crate.Selected;
					if (sel != null && Input.GetKeyDown(FurnitureMod.TakeKey))
					{
						crate.TakeOut(cam);
						return;
					}
					look = head + (sel != null
						? "< " + Items.Label(sel) + " (" + Mathf.RoundToInt(sel.thisDurability) + "%) >  " + (crate.selected + 1) + " of " + crate.items.Count + "\nScroll to choose · [" + FurnitureMod.TakeKey + "] take it out · " + move
						: "Empty. Let go of a part while you look at it to put it in · " + move);
				}
			}
			else if (safe != null)
			{
				if (held == null && step != 0)
				{
					float amount = SafeAmount();
					if (step > 0)
					{
						Deposit(safe, amount);
					}
					else
					{
						Withdraw(safe, amount);
					}
				}
				string amountText = "$" + SafeAmount().ToString("#,0");
				look = "Safe: $" + safe.cash.ToString("#,0.##") + " inside · $" + Wallet().ToString("#,0.##") + " on you\n" + (held != null
					? (held.name.StartsWith("Cash", StringComparison.Ordinal) ? "Let go to put the cash in" : "")
					: "Scroll up: put " + amountText + " in · Scroll down: take " + amountText + " out (hold Shift: x10) · " + move);
			}
		}

		private static float SafeAmount()
		{
			return FurnitureMod.SafeStep * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 10f : 1f);
		}

		// --- Cash. The wallet is the game's Currency.money, and the bills in the
		// inventory; paying in takes it off both, like the ATM does. ---

		private static Component Currency()
		{
			Component i = Interactor();
			Component c = i != null && currencyField != null ? currencyField.GetValue(i) as Component : null;
			if (c != null && moneyField == null)
			{
				moneyField = AccessTools.Field(c.GetType(), "money");
			}
			return c;
		}

		private static float Wallet()
		{
			Component c = Currency();
			return c != null && moneyField != null ? (float)moneyField.GetValue(c) : 0f;
		}

		private static void Deposit(Safe safe, float amount)
		{
			Component c = Currency();
			object inv = invField != null ? invField.GetValue(Interactor()) : null;
			if (c == null || moneyField == null || inv == null)
			{
				return;
			}
			if (subtractMoney == null)
			{
				subtractMoney = AccessTools.Method(inv.GetType(), "SubtractMoney");
			}
			float money = (float)moneyField.GetValue(c);
			amount = Mathf.Min(amount, Mathf.Floor(money * 100f) / 100f);
			if (amount <= 0f || subtractMoney == null)
			{
				Hint("You've no cash on you.");
				return;
			}
			subtractMoney.Invoke(inv, new object[] { amount });
			moneyField.SetValue(c, Mathf.Round((money - amount) * 100f) / 100f);
			safe.cash = Mathf.Round((safe.cash + amount) * 100f) / 100f;
		}

		// A roll of bills from the safe, in front of it, like the ATM's.
		private static void Withdraw(Safe safe, float amount)
		{
			amount = Mathf.Min(amount, safe.cash);
			if (amount <= 0f)
			{
				Hint("The safe is empty.");
				return;
			}
			GameObject roll = MoneyRoll();
			if (roll == null)
			{
				Hint("Couldn't find the game's cash to hand out.");
				return;
			}
			Vector3 at = safe.transform.TransformPoint(Models.SafeCashOut);
			GameObject cash = Object.Instantiate(roll, at, safe.transform.rotation);
			cash.SetActive(true);
			PickUp p = cash.GetComponent<PickUp>();
			if (p != null)
			{
				p.thisDurability = amount;
				p.description = "Cash ($" + amount + ")";
			}
			safe.cash = Mathf.Round((safe.cash - amount) * 100f) / 100f;
		}

		private static GameObject moneyRoll;

		private static GameObject MoneyRoll()
		{
			if (moneyRoll == null)
			{
				Type atm = AccessTools.TypeByName("Atm");
				Object a = atm != null ? Object.FindObjectOfType(atm) : null;
				FieldInfo f = atm != null ? AccessTools.Field(atm, "moneyRoll") : null;
				moneyRoll = a != null && f != null ? f.GetValue(a) as GameObject : null;
			}
			return moneyRoll;
		}

		// --- Letting go of a part while looking at furniture. ---

		public static void Dropped(PickUp item)
		{
			Camera cam = Camera.main;
			if (cam == null || moving != null)
			{
				return;
			}
			Vector3 point;
			Furniture f = Aimed(cam, out point, item.gameObject);
			if (f == null)
			{
				return;
			}
			string why = null;
			if (f is PartsShelf)
			{
				if (Items.IsEngine(item.gameObject))
				{
					why = "Engine blocks go on an engine stand.";
				}
				else
				{
					why = ((PartsShelf)f).Place(item, point);
				}
			}
			else if (f is StorageCrate)
			{
				why = ((StorageCrate)f).Stow(item);
			}
			else if (f is Safe && item.name.StartsWith("Cash", StringComparison.Ordinal))
			{
				Safe safe = (Safe)f;
				safe.cash = Mathf.Round((safe.cash + Mathf.Max(0f, item.thisDurability)) * 100f) / 100f;
				Object.Destroy(item.gameObject);
			}
			if (why != null)
			{
				Hint(why);
			}
		}

		public static void Grabbed(PickUp item)
		{
			// The game only picks up what's within reach.
			Component i = Interactor();
			if (i != null && (item.transform.position - i.transform.position).sqrMagnitude >= 2.5f * 2.5f)
			{
				return;
			}
			foreach (PartsShelf s in Object.FindObjectsOfType<PartsShelf>())
			{
				if (s.Release(item))
				{
					Items.Freeze(item.gameObject, false);
					return;
				}
			}
		}

		// --- Moving. ---

		private static void StartMove(Furniture f)
		{
			moving = f;
			movingFrom = f.transform.position;
			movingRot = f.transform.rotation;
			movingYaw = f.transform.eulerAngles.y;
			movingColliders = f.GetComponentsInChildren<Collider>();
			foreach (Collider c in movingColliders)
			{
				c.enabled = false;
			}
		}

		private static void UpdateMove(Camera cam)
		{
			if (moving == null)
			{
				return;
			}
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			if (Mathf.Abs(scroll) > 0.01f && Time.time >= nextScroll)
			{
				movingYaw += scroll > 0f ? 15f : -15f;
				nextScroll = Time.time + 0.08f;
			}
			RaycastHit[] hits = Build.Sorted(Physics.RaycastAll(cam.transform.position, cam.transform.forward, 6f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore));
			Vector3 at = cam.transform.position + Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized * 2.5f;
			foreach (RaycastHit hit in hits)
			{
				if (hit.collider.GetComponentInParent<PickUp>() != null || hit.collider.transform.IsChildOf(moving.transform))
				{
					continue;
				}
				at = hit.point;
				break;
			}
			at = Build.Ground(at, movingColliders);
			moving.transform.SetPositionAndRotation(at, Quaternion.Euler(0f, movingYaw, 0f));
			moving.Moved();
			look = "Moving the " + Build.NameOf(moving.kind) + "\n[" + FurnitureMod.MoveKey + "] put it here · scroll to turn it · [Backspace] put it back";
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				moving.transform.SetPositionAndRotation(movingFrom, movingRot);
				EndMove();
			}
			else if (Input.GetKeyDown(FurnitureMod.MoveKey))
			{
				EndMove();
			}
		}

		private static void EndMove()
		{
			foreach (Collider c in movingColliders)
			{
				if (c != null)
				{
					c.enabled = true;
				}
			}
			moving.Moved();
			moving = null;
			movingColliders = null;
		}

		// --- On screen. ---

		public static void Draw()
		{
			string text = look;
			if (Time.time < hintUntil && !string.IsNullOrEmpty(hint))
			{
				text = string.IsNullOrEmpty(text) ? hint : hint + "\n" + text;
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.fontSize = 17;
			style.alignment = TextAnchor.UpperCenter;
			style.normal.textColor = Color.white;
			style.wordWrap = true;
			Rect r = new Rect(Screen.width * 0.15f, Screen.height * 0.72f, Screen.width * 0.7f, 120f);
			GUI.color = new Color(0f, 0f, 0f, 0.75f);
			GUI.Label(new Rect(r.x + 1, r.y + 1, r.width, r.height), text, style);
			GUI.color = Color.white;
			GUI.Label(r, text, style);
		}
	}

	// --- The game's parts. ---

	// Let go of while you look at furniture: onto the shelf, into the crate, cash
	// into the safe. Only something you were holding and let go of (not thrown),
	// that didn't just snap onto a vehicle (the game's own snap points come first).
	[HarmonyPatch(typeof(PickUp), "LetGo")]
	internal static class LetGoPatch
	{
		private static void Prefix(PickUp __instance, int throwing, bool ___canSnap, out bool __state)
		{
			__state = __instance != null && throwing == 0 && __instance.holding && !___canSnap;
		}

		private static void Postfix(PickUp __instance, bool __state)
		{
			if (!__state || __instance == null || !__instance.gameObject.activeSelf || __instance.transform.parent != null)
			{
				return;
			}
			try
			{
				Interact.Dropped(__instance);
			}
			catch (Exception e)
			{
				FurnitureMod.Log("Couldn't put that away: " + e);
			}
		}
	}

	// Picked up: off the shelf, and free to move again.
	[HarmonyPatch(typeof(PickUp), "OnMouseDown")]
	internal static class GrabPatch
	{
		private static void Prefix(PickUp __instance)
		{
			if (__instance == null || __instance.holding)
			{
				return;
			}
			try
			{
				Interact.Grabbed(__instance);
			}
			catch (Exception e)
			{
				FurnitureMod.Log("Couldn't take that off the shelf: " + e.Message);
			}
		}
	}
}
