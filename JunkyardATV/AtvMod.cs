using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(JunkyardATV.AtvMod), "Junkyard ATV", "1.0.0", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace JunkyardATV
{
	public class AtvMod : MelonMod
	{
		private static MelonLogger.Instance log;

		internal static AtvConfig Config;

		private MelonPreferences_Entry<KeyCode> forwardKey;
		private MelonPreferences_Entry<KeyCode> backKey;
		private MelonPreferences_Entry<KeyCode> leftKey;
		private MelonPreferences_Entry<KeyCode> rightKey;
		private MelonPreferences_Entry<KeyCode> brakeKey;
		private MelonPreferences_Entry<KeyCode> ignitionKey;
		private MelonPreferences_Entry<KeyCode> mountKey;
		private MelonPreferences_Entry<KeyCode> buyKey;
		private MelonPreferences_Entry<KeyCode> fitKey;
		private static MelonPreferences_Entry<float> price;

		private static AtvVehicle riding;
		private static Transform rider;
		private static Transform riderOldParent;
		private static int riderOldLayer;
		private static readonly List<Collider> ignored = new List<Collider>();

		private string hint = "";
		private string flash = "";
		private float flashUntil;

		// Restore once per level (see Truck Parts QOL): a marker object in the level.
		private static GameObject restoredWith;
		private float nextLoadTry;
		private static bool savingDisabled;
		private static int lastSaveFrame = -1;
		private static int lastSaveSlot = -1;

		private readonly FitMode fit = new FitMode();

		public static float Price
		{
			get { return price != null ? price.Value : 2500f; }
		}

		public static void Log(string message)
		{
			log.Msg(message);
		}

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category c = MelonPreferences.CreateCategory("JunkyardATV", "Junkyard ATV");
			forwardKey = c.CreateEntry("ForwardKey", KeyCode.W, "Throttle");
			backKey = c.CreateEntry("BackKey", KeyCode.S, "Brake / reverse");
			leftKey = c.CreateEntry("LeftKey", KeyCode.A, "Steer left");
			rightKey = c.CreateEntry("RightKey", KeyCode.D, "Steer right");
			brakeKey = c.CreateEntry("HandbrakeKey", KeyCode.Space, "Handbrake");
			ignitionKey = c.CreateEntry("IgnitionKey", KeyCode.I, "Start / stop the engine");
			mountKey = c.CreateEntry("MountKey", KeyCode.None, "Get on / off (None = the game's Interact key)");
			buyKey = c.CreateEntry("BuyKey", KeyCode.F10, "Buy an ATV (only when the Parts Store mod isn't installed)");
			fitKey = c.CreateEntry("FitKey", KeyCode.F7, "Fit mode: line the model up");
			price = c.CreateEntry("Price", 2500f, "Price of an ATV");
			Config = AtvConfig.Load();
		}

		public override void OnLateInitializeMelon()
		{
			AtvStore.TryInstall(HarmonyInstance);
			AtvSave.PatchEasySave(HarmonyInstance);
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			riding = null;
			rider = null;
			fit.Close(false);
			AtvStore.TryInstall(HarmonyInstance);
		}

		public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
		{
			if (restoredWith == null)
			{
				AtvEngine.Reset();
			}
		}

		public static void OnGameSave(int slot)
		{
			if (restoredWith == null || savingDisabled || lastSaveFrame == Time.frameCount && lastSaveSlot == slot)
			{
				return;
			}
			lastSaveFrame = Time.frameCount;
			lastSaveSlot = slot;
			AtvSave.Save(slot);
		}

		public override void OnUpdate()
		{
			if (restoredWith == null)
			{
				if (Time.realtimeSinceStartup >= nextLoadTry)
				{
					nextLoadTry = Time.realtimeSinceStartup + 1f;
					Interactor interactor = Object.FindObjectOfType<Interactor>();
					if (interactor != null)
					{
						restoredWith = new GameObject("JunkyardATV_Restored");
						UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(restoredWith, interactor.gameObject.scene);
						savingDisabled = false;
						AtvEngine.Reset();
						int slot = PlayerPrefs.GetInt("LoadSlot", 0);
						try
						{
							AtvSave.Load(slot);
						}
						catch (Exception e)
						{
							savingDisabled = true;
							Log("Couldn't restore ATVs; saving them is off for this level: " + e);
						}
					}
				}
				return;
			}
			hint = "";
			if (Input.GetKeyDown(fitKey.Value))
			{
				AtvVehicle target = riding ?? LookedAt();
				if (fit.IsOpen)
				{
					fit.Close(true);
				}
				else if (riding != null)
				{
					Flash("Get off the ATV to fit it.");
				}
				else if (target != null)
				{
					fit.Open(target);
				}
			}
			if (fit.IsOpen)
			{
				fit.Update();
				return;
			}
			if (!AtvStore.Active && Input.GetKeyDown(buyKey.Value) && riding == null)
			{
				Buy();
			}
			if (riding != null)
			{
				Ride();
			}
			else
			{
				AtvVehicle atv = LookedAt();
				if (atv != null)
				{
					hint = "[" + MountKeyName() + "] Get on the ATV   [" + fitKey.Value + "] Fit mode";
					if (Input.GetKeyDown(MountKey()))
					{
						Mount(atv);
					}
				}
			}
		}

		// --- Riding. ---

		private void Ride()
		{
			if (riding == null)
			{
				return;
			}
			float throttle = (Input.GetKey(forwardKey.Value) ? 1f : 0f) - (Input.GetKey(backKey.Value) ? 1f : 0f);
			float steer = (Input.GetKey(rightKey.Value) ? 1f : 0f) - (Input.GetKey(leftKey.Value) ? 1f : 0f);
			riding.throttle = throttle;
			riding.steer = steer;
			riding.handbrake = Input.GetKey(brakeKey.Value);
			if (Input.GetKeyDown(ignitionKey.Value))
			{
				if (riding.Running)
				{
					riding.Stop();
				}
				else
				{
					string why;
					if (!riding.TryStart(out why))
					{
						Flash(why);
					}
				}
			}
			hint = "[" + ignitionKey.Value + "] " + (riding.Running ? "Stop" : "Start") + " engine   [" + MountKeyName() + "] Get off";
			if (Input.GetKeyDown(MountKey()))
			{
				Dismount();
			}
		}

		private void Mount(AtvVehicle atv)
		{
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			if (interactor == null || interactor.person == null || interactor.drivingCar)
			{
				return;
			}
			rider = interactor.person.transform;
			riderOldParent = rider.parent;
			riderOldLayer = rider.gameObject.layer;
			// Like the game's GetInCart: parent to the seat, freeze walking.
			rider.SetParent(atv.Seat, false);
			rider.localPosition = Vector3.zero;
			rider.localRotation = Quaternion.identity;
			rider.gameObject.layer = 9;
			SetPlayerCanMove(interactor, false);
			ResetLook(interactor);
			// The player's capsule sits inside the ATV: never let them push each other.
			ignored.Clear();
			Collider capsule = rider.GetComponent<Collider>();
			if (capsule != null)
			{
				foreach (Collider c in atv.GetComponentsInChildren<Collider>())
				{
					Physics.IgnoreCollision(capsule, c, true);
					ignored.Add(c);
				}
			}
			atv.ridden = true;
			atv.GetComponent<Rigidbody>().mass = Config.mass + 80f;
			riding = atv;
		}

		private void Dismount()
		{
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			AtvVehicle atv = riding;
			riding = null;
			atv.ridden = false;
			atv.throttle = 0f;
			atv.steer = 0f;
			atv.GetComponent<Rigidbody>().mass = Config.mass;
			if (rider != null)
			{
				rider.SetParent(riderOldParent, true);
				rider.position = atv.transform.position - atv.transform.right * 1.1f + Vector3.up * 1.0f;
				rider.rotation = Quaternion.Euler(0f, atv.transform.eulerAngles.y, 0f);
				rider.gameObject.layer = riderOldLayer;
				Collider capsule = rider.GetComponent<Collider>();
				if (capsule != null)
				{
					foreach (Collider c in ignored)
					{
						if (c != null)
						{
							Physics.IgnoreCollision(capsule, c, false);
						}
					}
				}
			}
			ignored.Clear();
			if (interactor != null)
			{
				SetPlayerCanMove(interactor, true);
			}
			rider = null;
		}

		// interactor.fpc (FirstPersonController, in the plugins assembly).
		private static object Fpc(Interactor interactor)
		{
			FieldInfo f = typeof(Interactor).GetField("fpc", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return f != null ? f.GetValue(interactor) : null;
		}

		private static void SetPlayerCanMove(Interactor interactor, bool canMove)
		{
			object fpc = Fpc(interactor);
			FieldInfo f = fpc != null ? fpc.GetType().GetField("canMove", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : null;
			if (f != null)
			{
				f.SetValue(fpc, canMove);
			}
		}

		// Face forward, like the game does when you get in a vehicle
		// (fpc.m_MouseLook.m_CharacterTargetRot / m_CameraTargetRot).
		private static void ResetLook(Interactor interactor)
		{
			object fpc = Fpc(interactor);
			FieldInfo lookField = fpc != null ? fpc.GetType().GetField("m_MouseLook", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : null;
			object look = lookField != null ? lookField.GetValue(fpc) : null;
			if (look == null)
			{
				return;
			}
			FieldInfo character = look.GetType().GetField("m_CharacterTargetRot", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo camera = look.GetType().GetField("m_CameraTargetRot", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (character != null)
			{
				character.SetValue(look, Quaternion.identity);
			}
			if (camera != null)
			{
				camera.SetValue(look, Quaternion.Euler(10f, 0f, 0f));
			}
		}

		// The game's Interact key (Interactor.cr.Interact), unless set in prefs.
		private KeyCode MountKey()
		{
			if (mountKey.Value != KeyCode.None)
			{
				return mountKey.Value;
			}
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			FieldInfo crField = typeof(Interactor).GetField("cr", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			object cr = interactor != null && crField != null ? crField.GetValue(interactor) : null;
			object key = null;
			if (cr != null)
			{
				// ControlRef lives in the plugins assembly: field or property.
				FieldInfo field = cr.GetType().GetField("Interact", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				PropertyInfo property = cr.GetType().GetProperty("Interact", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				key = field != null ? field.GetValue(cr) : property != null ? property.GetValue(cr, null) : null;
			}
			return key is KeyCode ? (KeyCode)key : KeyCode.E;
		}

		private string MountKeyName()
		{
			return MountKey().ToString();
		}

		private static AtvVehicle LookedAt()
		{
			Camera cam = Camera.main;
			RaycastHit hit;
			if (cam == null || !Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3.5f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
			{
				return null;
			}
			return hit.collider.GetComponentInParent<AtvVehicle>();
		}

		private void Flash(string message)
		{
			flash = message;
			flashUntil = Time.time + 3f;
		}

		// --- Buying (when the Parts Store mod isn't installed). ---

		private void Buy()
		{
			GameObject controller = GameObject.FindWithTag("GameController");
			Currency currency = controller != null ? controller.GetComponent<Currency>() : null;
			if (currency == null || currency.money < Price)
			{
				Flash("An ATV costs $" + Price.ToString("0") + ".");
				return;
			}
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			if (interactor != null && interactor.inv != null)
			{
				interactor.inv.SubtractMoney(Price);
			}
			currency.money = Mathf.Round((currency.money - Price) * 100f) / 100f;
			SpawnNearPlayer();
			Flash("Bought an ATV.");
		}

		// On open ground a few meters in front of the player.
		public static void SpawnNearPlayer()
		{
			Camera cam = Camera.main;
			if (cam == null)
			{
				return;
			}
			Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
			Vector3 right = Vector3.Cross(Vector3.up, forward);
			foreach (Vector3 dir in new[] { forward, right, -right, -forward })
			{
				Vector3 spot = cam.transform.position + dir * 3.5f;
				RaycastHit ground;
				if (!Physics.Raycast(spot + Vector3.up * 1.5f, Vector3.down, out ground, 20f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
				{
					continue;
				}
				Vector3 center = ground.point + Vector3.up * 0.9f;
				if (Physics.CheckBox(center, new Vector3(0.6f, 0.45f, 1.0f), Quaternion.LookRotation(forward), Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
				{
					continue;
				}
				// Side-on to the player, so the seat is right there.
				AtvVehicle.Create(ground.point + Vector3.up * 0.4f, Quaternion.LookRotation(right), null);
				return;
			}
			Log("No room to put the ATV down here.");
		}

		// --- HUD. ---

		public override void OnGUI()
		{
			if (fit.IsOpen)
			{
				fit.OnGUI();
				return;
			}
			string shown = Time.time < flashUntil ? flash : hint;
			if (!string.IsNullOrEmpty(shown))
			{
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.alignment = TextAnchor.UpperCenter;
				style.fontSize = 16;
				Rect rect = new Rect(0f, Screen.height * 0.58f, Screen.width, 60f);
				GUI.color = Color.black;
				GUI.Label(new Rect(rect.x + 1f, rect.y + 1f, rect.width, rect.height), shown, style);
				GUI.color = Color.white;
				GUI.Label(rect, shown, style);
			}
			if (riding != null && riding.Engine != null)
			{
				GUIStyle hud = new GUIStyle(GUI.skin.label);
				hud.fontSize = 18;
				hud.alignment = TextAnchor.LowerRight;
				float fuel = AtvEngine.GetFloat(riding.Engine, "newFuelLevel");
				string text = Mathf.Abs(riding.SpeedKmh).ToString("0") + " km/h   " + (riding.Running ? riding.Rpm.ToString("0") + " rpm" : "engine off") + "   fuel " + Mathf.Clamp01(fuel / 1300f).ToString("P0");
				Rect r = new Rect(Screen.width - 420f, Screen.height - 60f, 400f, 40f);
				GUI.color = Color.black;
				GUI.Label(new Rect(r.x + 1f, r.y + 1f, r.width, r.height), text, hud);
				GUI.color = Color.white;
				GUI.Label(r, text, hud);
			}
		}
	}
}
