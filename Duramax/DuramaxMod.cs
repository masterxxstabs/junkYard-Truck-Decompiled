using EngineCloner;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(Duramax.DuramaxMod), "6.6L Duramax", "1.0.0", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace Duramax
{
	// The 6.6L Duramax V8 turbo-diesel as a mod of its own, built on Engine
	// Cloner (the patch that makes extra engines work): registered with its
	// EngineApi as an engine built on the game's V8, so it's sold, mounted and
	// saved the way Engine Cloner's engines are, with this mod's model, power
	// and bolt-on accessories.
	public class DuramaxMod : MelonMod
	{
		public const string Id = "duramax";
		public const string StoreCategory = "6.6L Duramax Diesel";

		private static MelonLogger.Instance log;
		private static MelonPreferences_Entry<float> price;
		private static MelonPreferences_Entry<float> power;
		private static MelonPreferences_Entry<float> ramHornPrice;
		private static MelonPreferences_Entry<float> turboBlanketPrice;
		private static MelonPreferences_Entry<float> liftPumpPrice;

		public static float Price
		{
			get { return price != null ? price.Value : 4200f; }
		}

		public static float Power
		{
			get { return power != null ? Mathf.Clamp(power.Value, 0.5f, 4f) : 1.6f; }
		}

		public static float RamHornPrice
		{
			get { return ramHornPrice != null ? ramHornPrice.Value : 450f; }
		}

		public static float TurboBlanketPrice
		{
			get { return turboBlanketPrice != null ? turboBlanketPrice.Value : 140f; }
		}

		public static float LiftPumpPrice
		{
			get { return liftPumpPrice != null ? liftPumpPrice.Value : 650f; }
		}

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category c = MelonPreferences.CreateCategory("Duramax", "6.6L Duramax");
			price = c.CreateEntry("Price", 4200f, "Price", "What a new 6.6L Duramax costs in the shops.");
			power = c.CreateEntry("Power", 1.6f, "Power", "How much harder the Duramax pulls than the game's V8 (1.6 = 60% more torque; turbos and accessories add on top).");
			ramHornPrice = c.CreateEntry("RamHornPrice", 450f, "Ram intake horn price", "Price of the billet ram intake horn (6% more pull when fitted).");
			turboBlanketPrice = c.CreateEntry("TurboBlanketPrice", 140f, "Turbo blanket price", "Price of the turbo blanket (3% more pull when fitted).");
			liftPumpPrice = c.CreateEntry("LiftPumpPrice", 650f, "Lift pump kit price", "Price of the lift pump and filter kit (6% more pull when fitted).");

			bool registered = EngineApi.RegisterEngine(new EngineApi.EngineKind
			{
				id = Id,
				shopName = "Engine - 6.6L Duramax V8 turbo-diesel (new)",
				baseEngine = "v8",
				price = () => Price,
				style = DuramaxModel.Style,
				storeCategory = StoreCategory,
				storeItems = DuramaxAccessories.ShopItems
			});
			if (!registered)
			{
				Log("Engine Cloner wouldn't take the Duramax; it won't be for sale.");
				return;
			}
			ShopProviders.Register(DuramaxAccessories.ShopItems);
			EngineApi.Saving += DuramaxAccessories.SaveLines;
			EngineApi.Loading += DuramaxAccessories.LoadLine;
			Log("Ready. The model is " + DuramaxModel.ModelPath + ".");
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			DuramaxAccessories.Reset();
		}

		public override void OnUpdate()
		{
			DuramaxAccessories.UpdateTriggers();
		}

		public static void Log(string message)
		{
			if (log != null)
			{
				log.Msg(message);
			}
			else
			{
				MelonLogger.Msg("[6.6L Duramax] " + message);
			}
		}
	}
}
