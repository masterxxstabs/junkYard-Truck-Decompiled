using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(JunkAutoPC.JunkAutoMod), "JunkAuto PC", "1.0.0", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace JunkAutoPC
{
	// JunkAuto.com on the garage PC: a RockAuto-style parts site in a tab next to
	// the game's email, orders delivered after a short wait, and order and delivery
	// emails in the game's inbox.
	public class JunkAutoMod : MelonMod
	{
		private static MelonLogger.Instance log;
		private static MelonPreferences_Entry<float> deliveryMinutes;
		private static MelonPreferences_Entry<float> shippingCost;
		private static MelonPreferences_Entry<float> freeShippingOver;
		private static MelonPreferences_Entry<bool> deliverToPlayer;
		private static MelonPreferences_Entry<bool> notifications;

		public static float DeliveryMinutes { get { return deliveryMinutes.Value; } }
		public static float ShippingCost { get { return shippingCost.Value; } }
		public static float FreeShippingOver { get { return freeShippingOver.Value; } }
		public static bool DeliverToPlayer { get { return deliverToPlayer.Value; } }

		private static Interactor interactor;
		private static float nextFind;
		private static GameObject restoredWith;
		private static bool savingDisabled;

		// The PC screen: showing the store hides the mail screen's own objects.
		private static bool storeShown;
		private static readonly List<GameObject> hidden = new List<GameObject>();
		private static bool wasOpen;

		private static string toast = "";
		private static float toastUntil;
		private static GUIStyle toastStyle;

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category c = MelonPreferences.CreateCategory("JunkAutoPC", "JunkAuto PC");
			deliveryMinutes = c.CreateEntry("DeliveryMinutes", 3f, "Delivery time (minutes)", "Minutes of play from placing an order to it arriving.");
			shippingCost = c.CreateEntry("ShippingCost", 9.99f, "Shipping cost", "Flat shipping charge per order.");
			freeShippingOver = c.CreateEntry("FreeShippingOver", 250f, "Free shipping over", "Orders at least this big ship free (0 = never).");
			deliverToPlayer = c.CreateEntry("DeliverToPlayer", false, "Deliver to wherever you are", "Off: orders are left where you placed them (by the PC). On: they arrive in front of you, wherever you are.");
			notifications = c.CreateEntry("Notifications", true, "Email pop-ups", "Show a pop-up when a JunkAuto email arrives.");
		}

		public override void OnLateInitializeMelon()
		{
			Patches.PatchEasySave(HarmonyInstance);
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			interactor = null;
			storeShown = false;
			hidden.Clear();
			wasOpen = false;
			MailBridge.Reset();
			Site.ResetForLevel();
		}

		public static void Log(string msg)
		{
			log.Msg(msg);
		}

		public override void OnUpdate()
		{
			if (interactor == null)
			{
				if (Time.realtimeSinceStartup < nextFind)
				{
					return;
				}
				nextFind = Time.realtimeSinceStartup + 1f;
				interactor = Object.FindObjectOfType<Interactor>();
				if (interactor == null)
				{
					return;
				}
			}
			if (restoredWith == null)
			{
				Restore();
			}
			Orders.Tick(Time.deltaTime); // game time: stops while paused
			UpdatePc();
		}

		// Once per level: load this slot's orders and emails.
		private static void Restore()
		{
			restoredWith = new GameObject("JunkAutoPC_Restored");
			UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(restoredWith, interactor.gameObject.scene);
			savingDisabled = false;
			int slot = Patches.NewGame ? 0 : PlayerPrefs.GetInt("LoadSlot", 0);
			Patches.NewGame = false;
			try
			{
				Orders.Load(slot);
			}
			catch (Exception e)
			{
				savingDisabled = true;
				Orders.Clear();
				Log("Couldn't restore JunkAuto orders; saving them is off for this level: " + e);
			}
		}

		public static void OnGameSave(int slot)
		{
			if (restoredWith != null && !savingDisabled)
			{
				Orders.Save(slot);
			}
		}

		private static void UpdatePc()
		{
			GameObject pc = interactor.pcCanvas;
			bool isOpen = pc != null && pc.activeSelf;
			if (!isOpen)
			{
				if (wasOpen)
				{
					// Closed (Esc or Exit): back to the mail for next time.
					ShowMail();
				}
				wasOpen = false;
				return;
			}
			wasOpen = true;
			if (!storeShown)
			{
				MailBridge.Update(interactor);
			}
		}

		public static void OpenStore()
		{
			if (storeShown || interactor == null || interactor.pcCanvas == null)
			{
				return;
			}
			hidden.Clear();
			foreach (Transform child in interactor.pcCanvas.transform)
			{
				if (child.gameObject.activeSelf)
				{
					hidden.Add(child.gameObject);
					child.gameObject.SetActive(false);
				}
			}
			storeShown = true;
		}

		public static void ShowMail()
		{
			foreach (GameObject g in hidden)
			{
				if (g != null)
				{
					g.SetActive(true);
				}
			}
			hidden.Clear();
			storeShown = false;
		}

		public override void OnGUI()
		{
			if (storeShown && interactor != null && interactor.pcCanvas != null && interactor.pcCanvas.activeSelf)
			{
				Site.Draw();
			}
			if (toast.Length > 0 && Time.realtimeSinceStartup < toastUntil)
			{
				if (toastStyle == null)
				{
					toastStyle = new GUIStyle(GUI.skin.box);
					toastStyle.fontSize = Mathf.Max(14, Screen.height / 50);
					toastStyle.alignment = TextAnchor.MiddleCenter;
					toastStyle.wordWrap = true;
					toastStyle.normal.textColor = new Color(0.2f, 1f, 0.4f);
				}
				float w = Mathf.Min(Screen.width * 0.4f, 620f);
				GUI.Box(new Rect(Screen.width - w - 20f, 20f, w, toastStyle.fontSize * 3.2f), toast, toastStyle);
			}
		}

		public static void Toast(string text)
		{
			if (notifications != null && notifications.Value)
			{
				toast = text;
				toastUntil = Time.realtimeSinceStartup + 6f;
			}
		}

		// --- Money and places. ---

		public static float Cash()
		{
			GameObject es = GameObject.Find("EventSystem2");
			Currency c = es != null ? es.GetComponent<Currency>() : null;
			return c != null ? c.money : 0f;
		}

		// Paid the way the Parts Store pays: the cash in your inventory and the
		// money total both go down.
		public static bool Pay(float amount, out string why, out Vector3 spot)
		{
			spot = Vector3.zero;
			GameObject player = GameObject.Find("FPSController");
			GameObject es = GameObject.Find("EventSystem2");
			Interactor who = player != null ? player.GetComponent<Interactor>() : interactor;
			Currency currency = es != null ? es.GetComponent<Currency>() : null;
			if (who == null || who.inv == null || currency == null)
			{
				why = "Checkout isn't available right now. Try again in a moment.";
				return false;
			}
			if (currency.money < amount)
			{
				why = "Not enough cash: this order is $" + amount.ToString("0.00") + " and you have $" + currency.money.ToString("0.00") + ".";
				return false;
			}
			who.inv.SubtractMoney(amount);
			currency.money = Mathf.Round((currency.money - amount) * 100f) / 100f;
			// Left where you ordered from: by the PC (it drops to the floor).
			Transform t = player != null ? player.transform : who.transform;
			spot = t.position - t.forward * 0.4f;
			why = "";
			return true;
		}

		public static Vector3 InFrontOfPlayer()
		{
			GameObject player = GameObject.Find("FPSController");
			Transform t = player != null ? player.transform : interactor != null ? interactor.transform : null;
			return t != null ? t.position + t.forward * 1.5f : Vector3.zero;
		}
	}
}
