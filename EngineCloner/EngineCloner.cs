using System;
using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(EngineCloner.EngineClonerMod), "Engine Cloner", "1.0.0", "masterxxstabs")]
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

		// Per engine type, the block the game's vehicles, gearboxes, gauges etc. are
		// currently wired to. See ReferenceSwapper.
		private static readonly Dictionary<Type, GameObject> wiredBlock = new Dictionary<Type, GameObject>();

		private static MelonLogger.Instance log;

		private MelonPreferences_Entry<KeyCode> cloneKey;

		private float nextBlockScan;

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category category = MelonPreferences.CreateCategory("EngineCloner");
			cloneKey = category.CreateEntry("CloneKey", KeyCode.F8, "Clone key", "Look at an engine block or a vehicle and press this to clone it.");
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			anchorByBlock.Clear();
			ownerByAnchor.Clear();
			blocks.Clear();
			clones.Clear();
			wiredBlock.Clear();
			VehicleCloner.Reset();
			nextBlockScan = 0f;
		}

		public override void OnUpdate()
		{
			if (Input.GetKeyDown(cloneKey.Value))
			{
				CloneLookedAt();
			}
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

		public static void Log(string message)
		{
			log.Msg(message);
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
			// steal the wiring back and forth every frame.
			if (IsMounted(wired, wired.GetComponent<FixedJoint>()))
			{
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

		public static bool IsEngineBlock(GameObject go)
		{
			return go != null && (go.GetComponent<engine>() != null || go.GetComponent<enginev8>() != null || go.GetComponent<enginei6>() != null || go.GetComponent<Engine250>() != null);
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
			log.Msg("Cloned " + original.name + ".");
		}
	}

	// Stock EngReleaseStand looks blocks up with GameObject.Find(name), which picks
	// an arbitrary one once clones share the name, and can null-ref. Release every
	// block that is actually on the stand (DK_9) instead.
	[HarmonyPatch(typeof(Interactor), "EngReleaseStand")]
	internal static class EngReleaseStandPatch
	{
		private static bool Prefix()
		{
			foreach (PickUp pickUp in Object.FindObjectsOfType<PickUp>())
			{
				if (!EngineClonerMod.IsEngineBlock(pickUp.gameObject))
				{
					continue;
				}
				FixedJoint joint = pickUp.GetComponent<FixedJoint>();
				if (joint != null && joint.connectedBody != null && joint.connectedBody.name == "DK_9")
				{
					pickUp.pickable = true;
				}
			}
			return false;
		}
	}
}
