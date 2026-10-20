using System;
using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(EngineCloner.EngineClonerMod), "Engine Cloner", "2.8.0", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace EngineCloner
{
	// Why cloned engines were "linked":
	//
	// A loose engine block is never truly free. Its FixedJoint stays connected to a
	// hidden "empty" rigidbody, and the game uses ONE shared empty per engine type
	// (EmptyObjRigidbody, EmptyObjRigidbodyV8, EmptyObjRigidbodyi6,
	// EmptyObjRigidbody250). PickUp, the engine Start() methods, Jiggs' relocate menu
	// and the debug menu all teleport that shared empty to the block and reconnect.
	// A clone copies the original's FixedJoint, so it ends up jointed to the same
	// body (or to the truck or stand the original was on). Two blocks fixed to one
	// body are welded together: move one and the other follows.
	//
	// The fix: every engine block gets its own private copy of that empty. Whenever a
	// block's joint points at a shared empty, or at another block's private one, it
	// is moved back to its own before the next physics step runs.
	public class EngineClonerMod : MelonMod
	{
		private static readonly HashSet<string> SharedAnchorNames = new HashSet<string>
		{
			"EmptyObjRigidbody",
			"EmptyObjRigidbodyV8",
			"EmptyObjRigidbodyi6",
			"EmptyObjRigidbody250"
		};

		// block instance id -> that block's private anchor
		private static readonly Dictionary<int, Rigidbody> anchorByBlock = new Dictionary<int, Rigidbody>();

		// anchor instance id -> block that owns it
		private static readonly Dictionary<int, GameObject> ownerByAnchor = new Dictionary<int, GameObject>();

		private static readonly List<GameObject> blocks = new List<GameObject>();

		// Blocks this mod created.
		private static readonly HashSet<int> clones = new HashSet<int>();

		// block:vehicle pairs already made to talk to each other.
		private static readonly HashSet<string> localized = new HashSet<string>();

		// Per engine type, the block the game's vehicles, gearboxes, gauges etc. are
		// currently wired to. See ReferenceSwapper.
		private static readonly Dictionary<Type, GameObject> wiredBlock = new Dictionary<Type, GameObject>();

		private static MelonLogger.Instance log;

		private MelonPreferences_Entry<KeyCode> cloneKey;
		private MelonPreferences_Entry<bool> debugClone;
		private static MelonPreferences_Entry<float> i4Price;
		private static MelonPreferences_Entry<float> v8Price;
		private static MelonPreferences_Entry<float> i6Price;
		private static MelonPreferences_Entry<float> duramaxPrice;
		private static MelonPreferences_Entry<float> duramaxPower;
		private static MelonPreferences_Entry<bool> shipWithOil;
		private static MelonPreferences_Entry<bool> removeSpeedLimits;
		private static MelonPreferences_Entry<bool> bareBlocks;
		private static MelonPreferences_Entry<float> standPrice;
		private MelonPreferences_Entry<KeyCode> moveStandKey;
		private string hint = "";
		private float hintUntil;

		// Restore bought engines once per level, when the level's engines are in.
		private static GameObject restoredWith;
		private static bool savingDisabled;
		private static int lastSaveFrame = -1;
		private static int lastSaveSlot;

		private float nextBlockScan;

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category category = MelonPreferences.CreateCategory("EngineCloner");
			debugClone = category.CreateEntry("DebugCloneKey", false, "Debug: clone key", "Debug only: lets the clone key copy the engine or vehicle you're looking at. Off: buy new engines in the shops instead.");
			cloneKey = category.CreateEntry("CloneKey", KeyCode.F8, "Clone key (debug)", "With the debug clone key on: look at an engine block or a vehicle and press this to clone it.");
			i4Price = category.CreateEntry("I4Price", 750f, "New I4 price", "Price of a new I4 engine in the shops.");
			v8Price = category.CreateEntry("V8Price", 1500f, "New V8 price", "Price of a new V8 engine in the shops.");
			i6Price = category.CreateEntry("I6Price", 1800f, "New 2J inline-6 price", "Price of a new 2J inline-6 engine in the shops.");
			duramaxPrice = category.CreateEntry("DuramaxPrice", 4200f, "New 6.6L Duramax price", "Price of a new 6.6L Duramax V8 turbo-diesel in the shops.");
			duramaxPower = category.CreateEntry("DuramaxPower", 1.6f, "6.6L Duramax power", "How much stronger the Duramax pulls than the game's V8 (1.6 = 60% more torque; turbos still add on top).");
			shipWithOil = category.CreateEntry("ShipWithOil", false, "New engines come with oil", "Off: new engines ship dry, like a real crate engine; add oil before starting it.");
			standPrice = category.CreateEntry("StandPrice", 350f, "Engine stand price", "Price of an extra engine stand in the shops.");
			moveStandKey = category.CreateEntry("MoveStandKey", KeyCode.K, "Move stand key", "Look at an engine stand you bought, press this, look where you want it and press it again.");
			ShopProviders.Register(StandShop.ShopItems);
			bareBlocks = category.CreateEntry("BareBlocks", true, "New engines: bare block", "On: a new engine is just the block, every part off (fit your own). Off: a complete engine, a copy of your most complete one with every part new.");
			removeSpeedLimits = category.CreateEntry("RemoveSpeedLimits", true, "Remove speed limits", "On: vehicles aren't cut off at the game's top speed (pickup 85/100, F100 95/100, golf cart 100); they go as fast as engine and gearing allow. Off: the game's limits.");
			ShopProviders.Register(EngineShop.ShopItems);
		}

		public static float StandPrice
		{
			get { return standPrice != null ? standPrice.Value : 350f; }
		}

		public static bool BareBlocks
		{
			get { return bareBlocks == null || bareBlocks.Value; }
		}

		public static bool RemoveSpeedLimits
		{
			get { return removeSpeedLimits == null || removeSpeedLimits.Value; }
		}

		public static float DuramaxPower
		{
			get { return duramaxPower != null ? Mathf.Clamp(duramaxPower.Value, 0.5f, 4f) : 1.6f; }
		}

		public static float PriceOf(string kind)
		{
			MelonPreferences_Entry<float> e = kind == "v8" ? v8Price : kind == "i6" ? i6Price : kind == "duramax" ? duramaxPrice : i4Price;
			return e != null ? e.Value : 1000f;
		}

		public static bool ShipWithOil
		{
			get { return shipWithOil != null && shipWithOil.Value; }
		}

		// Every engine this mod makes: its own anchor at once, tracked as ours.
		public static void Adopt(GameObject block)
		{
			if (!IsEngineBlock(block))
			{
				return;
			}
			Release(block);
			clones.Add(block.GetInstanceID());
			if (!blocks.Contains(block))
			{
				blocks.Add(block);
			}
		}

		public static void OnGameSave(string method)
		{
			int slot = method == "OptionSave" || method == "Save1" ? 1 : method == "OptionSave2" || method == "Save2" ? 2 : method == "OptionSave3" || method == "Save3" ? 3 : EngineSave.AutoSlot;
			if (restoredWith == null || savingDisabled || lastSaveFrame == Time.frameCount && lastSaveSlot == slot)
			{
				return;
			}
			lastSaveFrame = Time.frameCount;
			lastSaveSlot = slot;
			EngineSave.Save(slot);
		}

		private static void PatchEasySave(HarmonyLib.Harmony harmony)
		{
			Type manager = AccessTools.TypeByName("ES3AutoSaveMgr");
			if (manager == null)
			{
				return;
			}
			HarmonyMethod prefix = new HarmonyMethod(typeof(EngineClonerMod).GetMethod("EasySavePrefix", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic));
			HarmonyMethod finalizer = new HarmonyMethod(typeof(EngineClonerMod).GetMethod("EasySaveFinalizer", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic));
			foreach (string name in new[] { "Save1", "Save2", "Save3", "SaveAuto" })
			{
				System.Reflection.MethodInfo m = AccessTools.Method(manager, name);
				if (m != null)
				{
					harmony.Patch(m, prefix: prefix, finalizer: finalizer);
				}
			}
		}

		private static void EasySavePrefix(System.Reflection.MethodBase __originalMethod)
		{
			VehicleCloner.BeginSave();
			OnGameSave(__originalMethod.Name);
		}

		private static Exception EasySaveFinalizer(Exception __exception)
		{
			VehicleCloner.EndSave();
			return __exception;
		}

		// Once per level, after the game's own engines are in: bring back the
		// engines bought or cloned in this save slot.
		private static void TryRestore()
		{
			if (restoredWith != null)
			{
				return;
			}
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			if (interactor == null || blocks.Count == 0)
			{
				return;
			}
			restoredWith = new GameObject("EngineCloner_Restored");
			UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(restoredWith, interactor.gameObject.scene);
			savingDisabled = false;
			int slot = EngineOverwritePatch.NewGame ? 0 : PlayerPrefs.GetInt("LoadSlot", 0);
			EngineOverwritePatch.NewGame = false;
			try
			{
				EngineSave.Load(slot);
			}
			catch (Exception e)
			{
				savingDisabled = true;
				log.Msg("Couldn't restore bought engines; saving them is off for this level: " + e);
			}
		}

		public override void OnLateInitializeMelon()
		{
			AddPartFix.Install(HarmonyInstance);
			PatchEasySave(HarmonyInstance);
			EngineShop.HookStore(HarmonyInstance);
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			anchorByBlock.Clear();
			ownerByAnchor.Clear();
			blocks.Clear();
			clones.Clear();
			localized.Clear();
			wiredBlock.Clear();
			VehicleCloner.Reset();
			EngineShop.Reset();
			StandShop.Reset();
			EngineShop.HookStore(HarmonyInstance);
			nextBlockScan = 0f;
		}

		public override void OnUpdate()
		{
			// Debug only: copy what you're looking at. Gameplay is buying new engines.
			if (debugClone.Value && Input.GetKeyDown(cloneKey.Value))
			{
				CloneLookedAt();
			}
			if (restoredWith != null)
			{
				string message = StandShop.Update(moveStandKey.Value);
				if (message != null)
				{
					hint = message;
					hintUntil = Time.time + 2.5f;
				}
			}
		}

		private GUIStyle hintStyle;

		public override void OnGUI()
		{
			if (hint.Length == 0 || Time.time > hintUntil)
			{
				return;
			}
			if (hintStyle == null)
			{
				hintStyle = new GUIStyle(GUI.skin.label);
				hintStyle.alignment = TextAnchor.MiddleCenter;
				hintStyle.fontSize = Mathf.Max(16, Screen.height / 45);
				hintStyle.normal.textColor = Color.white;
			}
			GUI.Label(new Rect(0f, Screen.height * 0.62f, Screen.width, 40f), hint, hintStyle);
		}

		// LateUpdate runs after every Update, Start and coroutine of the frame and
		// before the next physics step, so a joint the game pointed at a shared
		// anchor this frame never gets simulated that way.
		public override void OnLateUpdate()
		{
			if (Time.time >= nextBlockScan)
			{
				nextBlockScan = Time.time + 1f;
				ScanForBlocks();
				TryRestore();
				VehicleCloner.Scan();
				VehicleCloner.RecordSafePoses();
			}
			for (int i = blocks.Count - 1; i >= 0; i--)
			{
				GameObject block = blocks[i];
				if (block == null)
				{
					blocks.RemoveAt(i);
					continue;
				}
				FixedJoint joint = block.GetComponent<FixedJoint>();
				if (joint != null && joint.connectedBody != null && IsForeignAnchor(block, joint.connectedBody))
				{
					Release(block);
				}
				if (IsMounted(block, joint))
				{
					WireToVehicle(block);
				}
			}
		}

		// Also called while Harmony patches are applied, which MelonLoader does
		// before OnInitializeMelon has set up the mod's own logger.
		public static void Log(string message)
		{
			if (log != null)
			{
				log.Msg(message);
			}
			else
			{
				MelonLogger.Msg("[Engine Cloner] " + message);
			}
		}

		// A vehicle swap also swapped the engines mounted in the two vehicles; keep
		// tracking whichever block the game is wired to now.
		public static void FollowVehicleSwap(Dictionary<Object, Object> map)
		{
			foreach (Type type in new List<Type>(wiredBlock.Keys))
			{
				GameObject block = wiredBlock[type];
				Object other;
				if (block != null && map.TryGetValue(block, out other) && other is GameObject)
				{
					wiredBlock[type] = (GameObject)other;
				}
			}
		}

		// Engine blocks that came along inside a cloned vehicle.
		public static void RegisterClonedBlocks(GameObject root)
		{
			foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
			{
				if (IsEngineBlock(t.gameObject))
				{
					clones.Add(t.gameObject.GetInstanceID());
					if (!blocks.Contains(t.gameObject))
					{
						blocks.Add(t.gameObject);
					}
				}
			}
		}

		// Bolted into a truck, car or bike: the game parents the block to the vehicle
		// and fixes its joint to the vehicle's rigidbody.
		private static bool IsMounted(GameObject block, FixedJoint joint)
		{
			Transform parent = block.transform.parent;
			return parent != null && joint != null && joint.connectedBody != null && joint.connectedBody.transform == parent;
		}

		// Make the vehicles talk to this block instead of whichever one of the same
		// type they were wired to before.
		private static void WireToVehicle(GameObject block)
		{
			Type type = EngineType(block);
			GameObject wired;
			if (!wiredBlock.TryGetValue(type, out wired) || wired == null || wired == block)
			{
				wiredBlock[type] = block;
				return;
			}
			// The game can only drive one block of each type. If the wired one is
			// still mounted somewhere, leave it alone, or two mounted blocks would
			// steal the wiring back and forth every frame. The vehicle this one is in
			// still needs to talk to it rather than to the other vehicle's engine;
			// getting in that vehicle then swaps the game over to both.
			if (IsMounted(wired, wired.GetComponent<FixedJoint>()))
			{
				GameObject vehicle = block.transform.parent.gameObject;
				if (localized.Add(block.GetInstanceID() + ":" + vehicle.GetInstanceID()))
				{
					int n = ReferenceSwapper.Localize(vehicle, block);
					log.Msg(vehicle.name + " and its " + block.name + ": pointed " + n + " references at each other.");
				}
				return;
			}
			int swapped = ReferenceSwapper.Swap(wired, block);
			wiredBlock[type] = block;
			log.Msg("Mounted " + block.name + ": rewired " + swapped + " game references to it.");
		}

		private static Type EngineType(GameObject block)
		{
			if (block.GetComponent<enginev8>() != null)
			{
				return typeof(enginev8);
			}
			if (block.GetComponent<enginei6>() != null)
			{
				return typeof(enginei6);
			}
			if (block.GetComponent<Engine250>() != null)
			{
				return typeof(Engine250);
			}
			return typeof(engine);
		}

		// A real, loose-able engine block. Every one in the game carries a
		// FixedJoint (the engine scripts use it unconditionally); engines built into
		// other mods' vehicles (Junkyard ATV's 250) don't, and must not be wired,
		// swapped or cloned as blocks.
		public static bool IsEngineBlock(GameObject go)
		{
			return go != null && go.GetComponent<FixedJoint>() != null && (go.GetComponent<engine>() != null || go.GetComponent<enginev8>() != null || go.GetComponent<enginei6>() != null || go.GetComponent<Engine250>() != null);
		}

		// Shared game anchor, or a private anchor belonging to some other block.
		private static bool IsForeignAnchor(GameObject block, Rigidbody body)
		{
			if (SharedAnchorNames.Contains(body.name))
			{
				return true;
			}
			GameObject owner;
			return ownerByAnchor.TryGetValue(body.GetInstanceID(), out owner) && owner != block;
		}

		private static void ScanForBlocks()
		{
			blocks.Clear();
			AddAll(Object.FindObjectsOfType<engine>());
			AddAll(Object.FindObjectsOfType<enginev8>());
			AddAll(Object.FindObjectsOfType<enginei6>());
			AddAll(Object.FindObjectsOfType<Engine250>());
		}

		private static void AddAll(MonoBehaviour[] scripts)
		{
			foreach (MonoBehaviour script in scripts)
			{
				GameObject block = script.gameObject;
				if (!IsEngineBlock(block))
				{
					continue;
				}
				if (!blocks.Contains(block))
				{
					blocks.Add(block);
				}
				// After a level loads the game is wired to the block it shipped with,
				// which is never one of ours.
				Type type = EngineType(block);
				if (!clones.Contains(block.GetInstanceID()) && (!wiredBlock.ContainsKey(type) || wiredBlock[type] == null))
				{
					wiredBlock[type] = block;
				}
			}
		}

		// Connect the block's FixedJoint to its own anchor, placed where the block is.
		// Same end state as the game's own "teleport the empty, then connect", minus
		// the sharing.
		public static void Release(GameObject block)
		{
			FixedJoint joint = block.GetComponent<FixedJoint>();
			if (joint == null)
			{
				return;
			}
			Rigidbody anchor = GetAnchor(block, joint.connectedBody);
			joint.connectedBody = null;
			anchor.transform.position = block.transform.position;
			anchor.position = block.transform.position;
			anchor.velocity = Vector3.zero;
			anchor.angularVelocity = Vector3.zero;
			joint.connectedBody = anchor;
		}

		private static Rigidbody GetAnchor(GameObject block, Rigidbody current)
		{
			Rigidbody anchor;
			if (anchorByBlock.TryGetValue(block.GetInstanceID(), out anchor) && anchor != null)
			{
				return anchor;
			}
			// Copy the game's shared anchor so mass, drag and gravity match exactly.
			GameObject template = GameObject.Find(SharedAnchorName(block));
			GameObject copy;
			if (template != null)
			{
				copy = Object.Instantiate(template);
			}
			else
			{
				copy = new GameObject();
				copy.AddComponent<Rigidbody>().useGravity = false;
			}
			copy.name = block.name + "_anchor_" + block.GetInstanceID();
			anchor = copy.GetComponent<Rigidbody>();
			anchorByBlock[block.GetInstanceID()] = anchor;
			ownerByAnchor[anchor.GetInstanceID()] = block;
			return anchor;
		}

		private static string SharedAnchorName(GameObject block)
		{
			if (block.GetComponent<enginev8>() != null)
			{
				return "EmptyObjRigidbodyV8";
			}
			if (block.GetComponent<enginei6>() != null)
			{
				return "EmptyObjRigidbodyi6";
			}
			if (block.GetComponent<Engine250>() != null)
			{
				return "EmptyObjRigidbody250";
			}
			return "EmptyObjRigidbody";
		}

		// The nearest engine block or vehicle up the hierarchy from what the player
		// is looking at. An engine mounted in a truck is found before the truck.
		private static GameObject FindLookedAt()
		{
			Camera camera = Camera.main;
			if (camera == null)
			{
				return null;
			}
			RaycastHit hit;
			if (!Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 6f))
			{
				return null;
			}
			// The ray usually hits a part (head, oil pan, door, wheel...), so walk up.
			for (Transform t = hit.collider.transform; t != null; t = t.parent)
			{
				if (IsEngineBlock(t.gameObject) || VehicleCloner.VehicleType(t.gameObject) != null)
				{
					return t.gameObject;
				}
			}
			return null;
		}

		private void CloneLookedAt()
		{
			GameObject original = FindLookedAt();
			if (original == null)
			{
				log.Msg("Look at an engine block or a vehicle to clone it.");
				return;
			}
			if (!IsEngineBlock(original))
			{
				VehicleCloner.Clone(original);
				return;
			}
			PickUp originalPickUp = original.GetComponent<PickUp>();
			if (originalPickUp != null && originalPickUp.holding)
			{
				log.Msg("Put the engine down before cloning it.");
				return;
			}
			Transform cam = Camera.main.transform;
			Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
			Vector3 position = cam.position + forward * 2.5f + Vector3.up * 0.5f;

			// No parent: the clone starts loose even if the original sits in a truck.
			GameObject clone = Object.Instantiate(original, position, original.transform.rotation);
			// The game identifies blocks by name ("engineblock", "v8_block", ...) for
			// pickup, the engine stand and mounting, so the clone must keep the name.
			clone.name = original.name;

			Rigidbody body = clone.GetComponent<Rigidbody>();
			if (body != null)
			{
				body.velocity = Vector3.zero;
				body.angularVelocity = Vector3.zero;
			}
			PickUp pickUp = clone.GetComponent<PickUp>();
			if (pickUp != null)
			{
				pickUp.pickable = true;
				pickUp.price = 0f;
			}

			// The clone's joint still points at whatever held the original (its anchor,
			// the stand, the truck). Cut that link before the first physics step.
			Release(clone);
			clones.Add(clone.GetInstanceID());
			blocks.Add(clone);
			// Saved like a bought engine (keeping its state), never over the original.
			EngineShop.StripGameSave(clone);
			EngineShop.Kind kind = EngineShop.KindOf(clone);
			if (kind != null && clone.GetComponent<ShopEngine>() == null)
			{
				ShopEngine marker = clone.AddComponent<ShopEngine>();
				marker.kind = kind.id;
				marker.ready = true;
			}
			log.Msg("Cloned " + original.name + ".");
		}
	}

	// Stock EngReleaseStand looks blocks up with GameObject.Find(name), which picks
	// an arbitrary one once clones share the name, and can null-ref. Release the
	// block on the stand whose lever was clicked (the DK_9 nearest the click).
	[HarmonyPatch(typeof(Interactor), "EngReleaseStand")]
	internal static class EngReleaseStandPatch
	{
		private static bool Prefix(RaycastHit ___hit)
		{
			Rigidbody stand = ___hit.collider != null ? StandShop.NearestStand(___hit.point) : null;
			foreach (PickUp pickUp in Object.FindObjectsOfType<PickUp>())
			{
				if (!EngineClonerMod.IsEngineBlock(pickUp.gameObject))
				{
					continue;
				}
				FixedJoint joint = pickUp.GetComponent<FixedJoint>();
				if (joint != null && joint.connectedBody != null && joint.connectedBody.name == "DK_9" && (stand == null || joint.connectedBody == stand))
				{
					pickUp.pickable = true;
				}
			}
			return false;
		}
	}
}
