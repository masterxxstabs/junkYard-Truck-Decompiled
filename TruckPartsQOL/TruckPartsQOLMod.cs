using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(TruckPartsQOL.TruckPartsQOLMod), "Truck Parts QOL", "1.1.0", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace TruckPartsQOL
{
	public class TruckPartsQOLMod : MelonMod
	{
		private const float Reach = 3f;

		private static MelonLogger.Instance log;

		private MelonPreferences_Entry<KeyCode> useKey;
		private MelonPreferences_Entry<KeyCode> powerKey;
		private MelonPreferences_Entry<KeyCode> nextKey;
		private MelonPreferences_Entry<KeyCode> prevKey;
		private MelonPreferences_Entry<KeyCode> modeKey;
		private MelonPreferences_Entry<KeyCode> ejectKey;
		private MelonPreferences_Entry<KeyCode> shopKey;
		private MelonPreferences_Entry<KeyCode> openKey;
		private MelonPreferences_Entry<float> tarpPrice;
		private MelonPreferences_Entry<float> tonneauPrice;
		private MelonPreferences_Entry<float> hardTopPrice;
		private MelonPreferences_Entry<bool> freeParts;
		private MelonPreferences_Entry<float> headUnitPrice;
		private MelonPreferences_Entry<float> speakerPrice;
		private MelonPreferences_Entry<float> subwooferPrice;
		private MelonPreferences_Entry<float> cdPrice;

		private bool shopOpen;
		private Rect shopRect = new Rect(40f, 40f, 320f, 10f);
		private string shopMessage = "";
		private bool fpcWasEnabled;

		private string hint = "";

		// Save/load: parts are restored once per level, after vehicles exist, and
		// nothing is saved before that (so the main menu never wipes the save).
		private bool loadedThisScene;
		private float nextLoadTry;
		private float nextAutosave;

		// Set if restoring failed: then the save file is left alone rather than
		// overwritten with whatever (little) is in the world.
		private bool savingDisabled;

		public static void Log(string message)
		{
			log.Msg(message);
		}

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category c = MelonPreferences.CreateCategory("TruckPartsQOL", "Truck Parts QOL");
			useKey = c.CreateEntry("UseKey", KeyCode.Y, "Install / remove / insert CD");
			powerKey = c.CreateEntry("PowerKey", KeyCode.P, "Head unit power");
			nextKey = c.CreateEntry("NextKey", KeyCode.N, "Next track / station");
			prevKey = c.CreateEntry("PrevKey", KeyCode.B, "Previous track / station");
			modeKey = c.CreateEntry("ModeKey", KeyCode.M, "Switch CD / radio");
			ejectKey = c.CreateEntry("EjectKey", KeyCode.J, "Eject CD");
			shopKey = c.CreateEntry("ShopKey", KeyCode.F9, "Open the parts shop (when the Parts Store mod isn't installed)");
			openKey = c.CreateEntry("OpenKey", KeyCode.O, "Open / close a bed cover");
			tarpPrice = c.CreateEntry("TarpPrice", 40f, "Price of the bed tarp");
			tonneauPrice = c.CreateEntry("TonneauPrice", 250f, "Price of the tonneau cover");
			hardTopPrice = c.CreateEntry("HardTopPrice", 600f, "Price of the hard top");
			freeParts = c.CreateEntry("FreeParts", false, "Shop items cost nothing");
			headUnitPrice = c.CreateEntry("HeadUnitPrice", 150f, "Price of the CD head unit");
			speakerPrice = c.CreateEntry("SpeakerPrice", 35f, "Price of a 6.5\" speaker");
			subwooferPrice = c.CreateEntry("SubwooferPrice", 120f, "Price of the subwoofer box");
			cdPrice = c.CreateEntry("CdPrice", 5f, "Price of a CD");
			StoreIntegration.PriceOf = Price;
			MigrateOldSettings(c);
			PartSave.MigrateOldSave();
			MusicLibrary.EnsureFolders();
		}

		// This mod used to be called Truck Stereo, with settings under [TruckStereo].
		// Copy any the player changed, once.
		private void MigrateOldSettings(MelonPreferences_Category current)
		{
			MelonPreferences_Entry<bool> migrated = current.CreateEntry("MigratedFromTruckStereo", false, "Settings copied from Truck Stereo", null, true);
			if (migrated.Value)
			{
				return;
			}
			migrated.Value = true;
			MelonPreferences_Category old = MelonPreferences.CreateCategory("TruckStereo");
			int copied = 0;
			copied += Copy(old, useKey) + Copy(old, powerKey) + Copy(old, nextKey) + Copy(old, prevKey) + Copy(old, modeKey) + Copy(old, ejectKey) + Copy(old, shopKey);
			copied += Copy(old, freeParts) + Copy(old, headUnitPrice) + Copy(old, speakerPrice) + Copy(old, subwooferPrice) + Copy(old, cdPrice);
			if (copied > 0)
			{
				Log("Copied " + copied + " setting(s) from Truck Stereo.");
			}
			MelonPreferences.Save();
		}

		private static int Copy<T>(MelonPreferences_Category old, MelonPreferences_Entry<T> entry)
		{
			// Reads the old value from the cfg file if it's there; otherwise it's just
			// the default and nothing changes.
			MelonPreferences_Entry<T> previous = old.CreateEntry(entry.Identifier, entry.DefaultValue);
			if (Equals(previous.Value, entry.DefaultValue))
			{
				return 0;
			}
			entry.Value = previous.Value;
			return 1;
		}

		// After every mod has loaded, so the Parts Store mod is there to find.
		public override void OnLateInitializeMelon()
		{
			foreach (MelonMod mod in MelonMod.RegisteredMelons)
			{
				if (mod != this && mod.Info.Name == "Truck Stereo")
				{
					LoggerInstance.Error("TruckStereo.dll is still in your Mods folder. Truck Parts QOL replaces it: delete TruckStereo.dll, or every part will be doubled.");
				}
			}
			StoreIntegration.TryInstall(HarmonyInstance);
		}

		private float Price(PartKind kind)
		{
			if (freeParts.Value)
			{
				return 0f;
			}
			switch (kind)
			{
			case PartKind.HeadUnit:
				return headUnitPrice.Value;
			case PartKind.Speaker:
				return speakerPrice.Value;
			case PartKind.Subwoofer:
				return subwooferPrice.Value;
			case PartKind.Tarp:
				return tarpPrice.Value;
			case PartKind.Tonneau:
				return tonneauPrice.Value;
			case PartKind.HardTop:
				return hardTopPrice.Value;
			default:
				return cdPrice.Value;
			}
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			loadedThisScene = false;
			nextLoadTry = 0f;
			shopOpen = false;
			StoreIntegration.TryInstall(HarmonyInstance);
			MusicLibrary.ClearCache();
		}

		public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
		{
			if (loadedThisScene)
			{
				SaveNow();
			}
			loadedThisScene = false;
		}

		public override void OnApplicationQuit()
		{
			if (loadedThisScene)
			{
				SaveNow();
			}
		}

		private void SaveNow()
		{
			if (!savingDisabled)
			{
				PartSave.Save();
			}
		}

		public override void OnUpdate()
		{
			if (!loadedThisScene)
			{
				if (Time.time >= nextLoadTry)
				{
					nextLoadTry = Time.time + 1f;
					if (Vehicles.AllVehicles().Count > 0)
					{
						loadedThisScene = true;
						nextAutosave = Time.time + 60f;
						try
						{
							PartSave.Load();
						}
						catch (Exception e)
						{
							savingDisabled = true;
							Log("Couldn't restore your parts, so saving is off until restart to protect UserData/TruckPartsQOL.txt: " + e);
						}
					}
				}
				return;
			}
			if (Time.time >= nextAutosave)
			{
				nextAutosave = Time.time + 60f;
				SaveNow();
			}
			if (Input.GetKeyDown(shopKey.Value))
			{
				if (StoreIntegration.Active)
				{
					Log("Truck Parts QOL items are in the Junkyard Terminal's Parts Store now.");
				}
				else
				{
					SetShopOpen(!shopOpen);
				}
			}
			hint = "";
			if (!shopOpen && Camera.main != null)
			{
				HandleLook();
			}
		}

		private void HandleLook()
		{
			TruckPart held = null;
			foreach (TruckPart part in TruckPart.All)
			{
				if (part.IsHeld)
				{
					held = part;
					break;
				}
			}
			RaycastHit hit;
			if (!LookAt(held, out hit))
			{
				return;
			}
			TruckPart target = hit.collider.GetComponentInParent<TruckPart>();
			HeadUnit unit = target != null ? target.GetComponent<HeadUnit>() : null;

			if (held != null)
			{
				if (held.kind == PartKind.CD)
				{
					if (unit != null && unit.insertedCd == 0)
					{
						hint = Key(useKey) + " Insert " + held.DisplayName;
						if (Input.GetKeyDown(useKey.Value))
						{
							unit.InsertCd(held.cdNumber);
							held.Pick.LetGo(0);
							Object.Destroy(held.gameObject);
						}
					}
					return;
				}
				if (BedCover.IsCover(held.kind))
				{
					HandleHeldCover(held, hit);
					return;
				}
				// Installing: on any vehicle surface that isn't another of our parts
				// (a fitted bed cover counts as the vehicle, e.g. speakers in a camper).
				bool onCover = target != null && target.GetComponent<BedCover>() != null;
				GameObject vehicle = target == null || onCover ? Vehicles.FindRoot(hit.collider.transform) : null;
				if (vehicle == null)
				{
					return;
				}
				hint = Key(useKey) + " Install " + held.DisplayName;
				if (Input.GetKeyDown(useKey.Value))
				{
					// Mount to the rigidbody that was hit (body, door...) so the part
					// moves with it; fall back to the vehicle itself.
					Transform mount = hit.rigidbody != null && Vehicles.FindRoot(hit.rigidbody.transform) == vehicle ? hit.rigidbody.transform : vehicle.transform;
					held.Install(mount, hit.point, hit.normal);
				}
				return;
			}

			if (target == null || !target.installed)
			{
				return;
			}
			BedCover fittedCover = target.GetComponent<BedCover>();
			if (fittedCover != null)
			{
				string what = target.kind == PartKind.HardTop ? "hatch" : target.DisplayName;
				hint = Key(openKey) + (fittedCover.open ? " Close " : " Open ") + what + "   " + Key(useKey) + " Remove " + target.DisplayName + "\n[PgUp/PgDn] Raise/lower (Shift: faster)";
				float step = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 0.05f : 0.01f;
				if (Input.GetKeyDown(KeyCode.PageUp))
				{
					fittedCover.AdjustHeight(step);
				}
				else if (Input.GetKeyDown(KeyCode.PageDown))
				{
					fittedCover.AdjustHeight(-step);
				}
				if (Input.GetKeyDown(openKey.Value))
				{
					string problem;
					fittedCover.SetOpen(!fittedCover.open, out problem);
					if (problem != null)
					{
						Flash(problem);
					}
				}
				else if (Input.GetKeyDown(useKey.Value))
				{
					target.Remove();
				}
				return;
			}
			if (unit == null)
			{
				hint = Key(useKey) + " Remove " + target.DisplayName;
				if (Input.GetKeyDown(useKey.Value))
				{
					target.Remove();
				}
				return;
			}
			hint = Key(powerKey) + " Power   " + Key(modeKey) + " CD/Radio   " + Key(prevKey) + Key(nextKey) + " Track   Scroll Volume\n" + Key(ejectKey) + " Eject   " + Key(useKey) + " Remove unit";
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			if (scroll > 0f)
			{
				unit.ChangeVolume(1f / 30f);
			}
			else if (scroll < 0f)
			{
				unit.ChangeVolume(-1f / 30f);
			}
			if (Input.GetKeyDown(powerKey.Value))
			{
				unit.TogglePower();
			}
			if (Input.GetKeyDown(nextKey.Value))
			{
				unit.Next();
			}
			if (Input.GetKeyDown(prevKey.Value))
			{
				unit.Previous();
			}
			if (Input.GetKeyDown(modeKey.Value))
			{
				unit.ToggleMode();
			}
			if (Input.GetKeyDown(ejectKey.Value))
			{
				int cd = unit.EjectCd();
				if (cd != 0)
				{
					Transform t = unit.transform;
					TruckPart disc = PartFactory.Create(PartKind.CD, cd, t.position + t.forward * (target.halfDepth + 0.08f), t.rotation);
					disc.GetComponent<Rigidbody>().velocity = t.forward * 0.8f;
				}
			}
			if (Input.GetKeyDown(useKey.Value))
			{
				target.Remove();
			}
		}

		private void HandleHeldCover(TruckPart held, RaycastHit hit)
		{
			GameObject vehicle = Vehicles.FindRoot(hit.collider.transform);
			if (vehicle == null)
			{
				return;
			}
			if (BedCover.FindBed(vehicle) == null)
			{
				hint = "This vehicle has no bed for a " + held.DisplayName + ".";
				return;
			}
			if (BedCover.CoverOn(vehicle) != null)
			{
				hint = "This bed already has a cover.";
				return;
			}
			hint = Key(useKey) + " Fit " + held.DisplayName + " to the bed";
			if (Input.GetKeyDown(useKey.Value))
			{
				string problem;
				if (!held.GetComponent<BedCover>().Fit(vehicle, true, out problem))
				{
					Flash(problem);
				}
			}
		}

		// A message shown in place of the hint for a couple of seconds.
		private string flash = "";
		private float flashUntil;

		private void Flash(string message)
		{
			flash = message;
			flashUntil = Time.time + 2.5f;
		}

		// What the player is looking at, ignoring the item in their hands.
		private static bool LookAt(TruckPart held, out RaycastHit result)
		{
			result = default(RaycastHit);
			Transform cam = Camera.main.transform;
			RaycastHit[] hits = Physics.RaycastAll(cam.position, cam.forward, Reach, ~0, QueryTriggerInteraction.Collide);
			Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
			foreach (RaycastHit hit in hits)
			{
				if (held != null && hit.transform.IsChildOf(held.transform))
				{
					continue;
				}
				if (hit.collider.GetComponentInParent<CharacterController>() != null)
				{
					continue; // the player
				}
				// Game trigger volumes (snap zones, water...) aren't surfaces, but our
				// installed parts are triggers too.
				if (hit.collider.isTrigger && hit.collider.GetComponentInParent<TruckPart>() == null)
				{
					continue;
				}
				result = hit;
				return true;
			}
			return false;
		}

		private static string Key(MelonPreferences_Entry<KeyCode> entry)
		{
			return "[" + entry.Value + "]";
		}

		public override void OnGUI()
		{
			string shown = Time.time < flashUntil ? flash : hint;
			if (!string.IsNullOrEmpty(shown) && !shopOpen)
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
			if (shopOpen)
			{
				shopRect = GUILayout.Window(0x5743, shopRect, DrawShop, "Truck parts shop");
			}
		}

		private void DrawShop(int id)
		{
			GUILayout.Label(freeParts.Value ? "Everything is free (FreeParts is on)." : "Cash: $" + Money().ToString("0.00"));
			ShopItem(PartKind.HeadUnit, 0, "CD head unit", Price(PartKind.HeadUnit));
			ShopItem(PartKind.Speaker, 0, "6.5\" speaker", Price(PartKind.Speaker));
			ShopItem(PartKind.Subwoofer, 0, "12\" subwoofer box", Price(PartKind.Subwoofer));
			GUILayout.Space(6f);
			ShopItem(PartKind.Tarp, 0, "Bed tarp", Price(PartKind.Tarp));
			ShopItem(PartKind.Tonneau, 0, "Tonneau cover (tri-fold)", Price(PartKind.Tonneau));
			ShopItem(PartKind.HardTop, 0, "Hard top (camper shell)", Price(PartKind.HardTop));
			GUILayout.Space(6f);
			bool anyCd = false;
			for (int i = 1; i <= MusicLibrary.MaxCds; i++)
			{
				int count = MusicLibrary.Tracks(i).Count;
				if (count > 0)
				{
					anyCd = true;
					ShopItem(PartKind.CD, i, "CD " + i + "  (" + count + " tracks)", Price(PartKind.CD));
				}
			}
			if (!anyCd)
			{
				GUILayout.Label("No CDs yet: put .ogg or .wav files in the CD1, CD2... folders.");
			}
			if (GUILayout.Button("Open music folder"))
			{
				Application.OpenURL(new Uri(MusicLibrary.Root).AbsoluteUri);
			}
			if (!string.IsNullOrEmpty(shopMessage))
			{
				GUILayout.Label(shopMessage);
			}
			if (GUILayout.Button("Close (" + shopKey.Value + ")"))
			{
				SetShopOpen(false);
			}
			GUI.DragWindow();
		}

		private void ShopItem(PartKind kind, int cd, string label, float price)
		{
			if (!GUILayout.Button(label + (freeParts.Value ? "" : "   $" + price.ToString("0"))))
			{
				return;
			}
			if (!freeParts.Value)
			{
				if (GetCurrency() == null)
				{
					shopMessage = "Couldn't find your money. Turn on FreeParts in MelonPreferences.cfg.";
					return;
				}
				if (!Charge(price))
				{
					shopMessage = "Not enough cash.";
					return;
				}
			}
			Transform cam = Camera.main.transform;
			Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
			Vector3 spot = cam.position + forward * 1.2f;
			RaycastHit ground;
			if (Physics.Raycast(spot, Vector3.down, out ground, 5f, ~0, QueryTriggerInteraction.Ignore))
			{
				spot = ground.point + Vector3.up * 0.4f;
			}
			PartFactory.Create(kind, cd, spot, Quaternion.LookRotation(-forward));
			shopMessage = "Bought " + label + ". It's in front of you.";
		}

		// Same bookkeeping the game does when you buy a part (PickUp): wallet cash
		// slots and the money total.
		private static bool Charge(float price)
		{
			Currency currency = GetCurrency();
			if (currency == null || currency.money < price)
			{
				return false;
			}
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			if (interactor != null && interactor.inv != null)
			{
				interactor.inv.SubtractMoney(price);
			}
			currency.money -= price;
			currency.money = Mathf.Round(currency.money * 100f) / 100f;
			return true;
		}

		private static float Money()
		{
			Currency currency = GetCurrency();
			return currency != null ? currency.money : 0f;
		}

		private static Currency GetCurrency()
		{
			GameObject controller = GameObject.FindWithTag("GameController");
			return controller != null ? controller.GetComponent<Currency>() : null;
		}

		// Free the mouse the way the game's own menus do (FirstPersonController).
		private void SetShopOpen(bool open)
		{
			shopOpen = open;
			shopMessage = "";
			Behaviour fpc = PlayerController();
			if (open)
			{
				Invoke(fpc, "UnlockMouse");
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				if (fpc != null)
				{
					fpcWasEnabled = fpc.enabled;
					fpc.enabled = false;
				}
			}
			else
			{
				Invoke(fpc, "LockMouse");
				if (fpc != null)
				{
					fpc.enabled = fpcWasEnabled;
				}
			}
		}

		private static Behaviour PlayerController()
		{
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			if (interactor == null)
			{
				return null;
			}
			FieldInfo field = typeof(Interactor).GetField("fpc", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return field != null ? field.GetValue(interactor) as Behaviour : null;
		}

		private static void Invoke(Behaviour target, string method)
		{
			if (target == null)
			{
				return;
			}
			MethodInfo info = target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
			if (info != null)
			{
				info.Invoke(target, null);
			}
		}
	}
}
