using System;
using NWH.VehiclePhysics2.Modules.ABS;
using NWH.VehiclePhysics2.Modules.Aerodynamics;
using NWH.VehiclePhysics2.Modules.ESC;
using NWH.VehiclePhysics2.Modules.FlipOver;
using NWH.VehiclePhysics2.Modules.TCS;
using NWH.VehiclePhysics2.Modules.Trailer;
using NWH.VehiclePhysics2.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2
{
	// Token: 0x02000259 RID: 601
	[RequireComponent(typeof(Canvas))]
	public class DemoGUIController : MonoBehaviour
	{
		// Token: 0x06000FC6 RID: 4038 RVA: 0x000B8434 File Offset: 0x000B6634
		private void Start()
		{
			this.absButton.onClick.AddListener(new UnityAction(this.ToggleABS));
			this.tcsButton.onClick.AddListener(new UnityAction(this.ToggleTCS));
			this.escButton.onClick.AddListener(new UnityAction(this.ToggleESC));
			this.aeroButton.onClick.AddListener(new UnityAction(this.ToggleAero));
			this.damageButton.onClick.AddListener(new UnityAction(this.ToggleDamage));
			this.repairButton.onClick.AddListener(new UnityAction(this.RepairDamage));
			this.resetButton.onClick.AddListener(new UnityAction(this.ResetScene));
			this.helpButton.onClick.AddListener(new UnityAction(this.ToggleHelpWindow));
			this.telemetryButton.onClick.AddListener(new UnityAction(this.ToggleTelemetryWindow));
			this.settingsButton.onClick.AddListener(new UnityAction(this.ToggleSettingsWindow));
			this._canvas = base.GetComponent<Canvas>();
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x000B8568 File Offset: 0x000B6768
		private void Update()
		{
			this._vc = VehicleChanger.ActiveVehicleController;
			this.promptText.text = "";
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				this._canvas.enabled = !this._canvas.enabled;
			}
			if (CharacterVehicleChanger.Instance != null && CharacterVehicleChanger.Instance.nearVehicle)
			{
				this.promptText.text = "Press V to enter the vehicle.";
			}
			if (this._vc == null)
			{
				return;
			}
			if (this._vc != this._prevVc)
			{
				this._trailerHitchModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<TrailerHitchModule>();
				this._flipOverModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<FlipOverModule>();
				this._absModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<ABSModule>();
				this._tcsModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<TCSModule>();
				this._escModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<ESCModule>();
				this._aeroModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<AerodynamicsModule>();
			}
			this.throttleSlider.value = Mathf.Clamp01(this._vc.input.states.vertical);
			this.brakeSlider.value = Mathf.Clamp01(-this._vc.input.states.vertical);
			this.clutchSlider.value = Mathf.Clamp01(this._vc.powertrain.clutch.clutchEngagement);
			this.handbrakeSlider.value = Mathf.Clamp01(this._vc.input.states.handbrake);
			this.horizontalLeftSlider.value = Mathf.Clamp01(-this._vc.input.Horizontal);
			this.horizontalRightSlider.value = Mathf.Clamp01(this._vc.input.Horizontal);
			if (this._trailerHitchModule != null && this._trailerHitchModule.trailerInRange && !this._trailerHitchModule.attached)
			{
				this.promptText.text = "Press T to attach the trailer.";
			}
			if (this._flipOverModule != null && this._flipOverModule.manual && this._flipOverModule.flippedOver)
			{
				this.promptText.text = "Press P to recover the vehicle.";
			}
			if (this._absModule != null)
			{
				this.absButton.targetGraphic.color = (this._absModule.IsEnabled ? DemoGUIController.enabledColor : DemoGUIController.disabledColor);
			}
			if (this._tcsModule != null)
			{
				this.tcsButton.targetGraphic.color = (this._tcsModule.IsEnabled ? DemoGUIController.enabledColor : DemoGUIController.disabledColor);
			}
			if (this._escModule != null)
			{
				this.escButton.targetGraphic.color = (this._escModule.IsEnabled ? DemoGUIController.enabledColor : DemoGUIController.disabledColor);
			}
			if (this._aeroModule != null)
			{
				this.aeroButton.targetGraphic.color = (this._aeroModule.IsEnabled ? DemoGUIController.enabledColor : DemoGUIController.disabledColor);
			}
			this.damageButton.targetGraphic.color = (this._vc.damageHandler.Active ? DemoGUIController.enabledColor : DemoGUIController.disabledColor);
			this.damageSlider.value = this._vc.damageHandler.Damage;
			this._prevVc = this._vc;
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x000B88CE File Offset: 0x000B6ACE
		public void ToggleDamage()
		{
			this._vc.damageHandler.LodIndex = -1;
			this._vc.damageHandler.ToggleState();
		}

		// Token: 0x06000FC9 RID: 4041 RVA: 0x000B88F1 File Offset: 0x000B6AF1
		public void RepairDamage()
		{
			if (this._vc != null && this._vc.damageHandler.Active)
			{
				this._vc.damageHandler.Repair();
			}
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x000B8923 File Offset: 0x000B6B23
		public void ToggleAero()
		{
			if (this._aeroModule != null)
			{
				this._aeroModule.LodIndex = -1;
				this._aeroModule.ToggleState();
			}
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x000B8944 File Offset: 0x000B6B44
		public void ToggleABS()
		{
			if (this._absModule != null)
			{
				this._absModule.LodIndex = -1;
				this._absModule.ToggleState();
			}
		}

		// Token: 0x06000FCC RID: 4044 RVA: 0x000B8965 File Offset: 0x000B6B65
		public void ToggleTCS()
		{
			if (this._tcsModule != null)
			{
				this._tcsModule.LodIndex = -1;
				this._tcsModule.ToggleState();
			}
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x000B8986 File Offset: 0x000B6B86
		public void ToggleESC()
		{
			if (this._escModule != null)
			{
				this._escModule.LodIndex = -1;
				this._escModule.ToggleState();
			}
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x000B89A7 File Offset: 0x000B6BA7
		public void ToggleHelpWindow()
		{
			this.helpWindow.SetActive(!this.helpWindow.activeInHierarchy);
			this.settingsWindow.SetActive(false);
			this.telemetryWindow.SetActive(false);
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x000B89DA File Offset: 0x000B6BDA
		public void ToggleSettingsWindow()
		{
			this.settingsWindow.SetActive(!this.settingsWindow.activeInHierarchy);
			this.helpWindow.SetActive(false);
			this.telemetryWindow.SetActive(false);
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x000B8A0D File Offset: 0x000B6C0D
		public void ToggleTelemetryWindow()
		{
			this.telemetryWindow.SetActive(!this.telemetryWindow.activeInHierarchy);
			this.settingsWindow.SetActive(false);
			this.helpWindow.SetActive(false);
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x000B8A40 File Offset: 0x000B6C40
		public void ResetScene()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		// Token: 0x04002059 RID: 8281
		public static Color disabledColor = new Color32(66, 66, 66, byte.MaxValue);

		// Token: 0x0400205A RID: 8282
		public static Color enabledColor = new Color32(76, 175, 80, byte.MaxValue);

		// Token: 0x0400205B RID: 8283
		public Text promptText;

		// Token: 0x0400205C RID: 8284
		public GameObject helpWindow;

		// Token: 0x0400205D RID: 8285
		public GameObject settingsWindow;

		// Token: 0x0400205E RID: 8286
		public GameObject telemetryWindow;

		// Token: 0x0400205F RID: 8287
		public Button absButton;

		// Token: 0x04002060 RID: 8288
		public Button tcsButton;

		// Token: 0x04002061 RID: 8289
		public Button escButton;

		// Token: 0x04002062 RID: 8290
		public Button aeroButton;

		// Token: 0x04002063 RID: 8291
		public Button damageButton;

		// Token: 0x04002064 RID: 8292
		public Button repairButton;

		// Token: 0x04002065 RID: 8293
		public Button resetButton;

		// Token: 0x04002066 RID: 8294
		public Button helpButton;

		// Token: 0x04002067 RID: 8295
		public Button settingsButton;

		// Token: 0x04002068 RID: 8296
		public Button telemetryButton;

		// Token: 0x04002069 RID: 8297
		public Slider throttleSlider;

		// Token: 0x0400206A RID: 8298
		public Slider brakeSlider;

		// Token: 0x0400206B RID: 8299
		public Slider clutchSlider;

		// Token: 0x0400206C RID: 8300
		public Slider handbrakeSlider;

		// Token: 0x0400206D RID: 8301
		public Slider horizontalLeftSlider;

		// Token: 0x0400206E RID: 8302
		public Slider horizontalRightSlider;

		// Token: 0x0400206F RID: 8303
		public Slider damageSlider;

		// Token: 0x04002070 RID: 8304
		private VehicleController _vc;

		// Token: 0x04002071 RID: 8305
		private VehicleController _prevVc;

		// Token: 0x04002072 RID: 8306
		private TrailerHitchModule _trailerHitchModule;

		// Token: 0x04002073 RID: 8307
		private FlipOverModule _flipOverModule;

		// Token: 0x04002074 RID: 8308
		private ABSModule _absModule;

		// Token: 0x04002075 RID: 8309
		private TCSModule _tcsModule;

		// Token: 0x04002076 RID: 8310
		private ESCModule _escModule;

		// Token: 0x04002077 RID: 8311
		private AerodynamicsModule _aeroModule;

		// Token: 0x04002078 RID: 8312
		private ColorBlock _colorBlock;

		// Token: 0x04002079 RID: 8313
		private Canvas _canvas;
	}
}
