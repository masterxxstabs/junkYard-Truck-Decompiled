using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

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

		private static MelonLogger.Instance log;

		private MelonPreferences_Entry<KeyCode> cloneKey;

		private float nextBlockScan;

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category category = MelonPreferences.CreateCategory("EngineCloner");
			cloneKey = category.CreateEntry("CloneKey", KeyCode.F8, "Clone key", "Look at an engine block and press this to clone it.");
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			anchorByBlock.Clear();
			ownerByAnchor.Clear();
			blocks.Clear();
			nextBlockScan = 0f;
		}

		public override void OnUpdate()
		{
			if (Input.GetKeyDown(cloneKey.Value))
			{
				CloneLookedAtEngine();
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
			}
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
				if (!blocks.Contains(script.gameObject))
				{
					blocks.Add(script.gameObject);
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

		private static GameObject FindLookedAtEngine()
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
			// The ray usually hits a part (head, oil pan, bolt...), so walk up to the block.
			for (Transform t = hit.collider.transform; t != null; t = t.parent)
			{
				if (IsEngineBlock(t.gameObject))
				{
					return t.gameObject;
				}
			}
			return null;
		}

		private void CloneLookedAtEngine()
		{
			GameObject original = FindLookedAtEngine();
			if (original == null)
			{
				log.Msg("Look at an engine block to clone it.");
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
