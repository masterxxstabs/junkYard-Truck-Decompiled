using System;
using NWH.VehiclePhysics2.Modules.ABS;
using NWH.VehiclePhysics2.Modules.TCS;
using NWH.VehiclePhysics2.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.VehicleGUI
{
	// Token: 0x020002AC RID: 684
	[RequireComponent(typeof(Canvas))]
	public class DashGUIController : MonoBehaviour
	{
		// Token: 0x0600123A RID: 4666 RVA: 0x000C4A3C File Offset: 0x000C2C3C
		private void Awake()
		{
			this._canvas = base.GetComponent<Canvas>();
			if (this.vehicleController != null)
			{
				this.vehicleController.onWake.AddListener(new UnityAction(this.OnWake));
				this.vehicleController.onSleep.AddListener(new UnityAction(this.OnSleep));
			}
		}

		// Token: 0x0600123B RID: 4667 RVA: 0x000C4A9C File Offset: 0x000C2C9C
		private void Update()
		{
			if (this.dataSource == DashGUIController.DataSource.VehicleChanger)
			{
				this.vehicleController = VehicleChanger.ActiveVehicleController;
			}
			if (this._prevVc != this.vehicleController && this.vehicleController != null)
			{
				TCSModuleWrapper component = this.vehicleController.GetComponent<TCSModuleWrapper>();
				this._tcsModule = ((component != null) ? component.module : null);
				ABSModuleWrapper component2 = this.vehicleController.GetComponent<ABSModuleWrapper>();
				this._absModule = ((component2 != null) ? component2.module : null);
			}
			if (this.vehicleController != null && this._update)
			{
				if (this.useAnalogRpmGauge)
				{
					this.analogRpmGauge.Value = this.vehicleController.powertrain.engine.RPM;
				}
				if (this.useDigitalRpmGauge)
				{
					this.digitalRpmGauge.numericalValue = this.vehicleController.powertrain.engine.RPM;
				}
				if (this.useAnalogSpeedGauge)
				{
					this.analogSpeedGauge.Value = this.vehicleController.Speed * 3.6f;
				}
				if (this.useDigitalSpeedGauge)
				{
					this.digitalSpeedGauge.numericalValue = this.vehicleController.Speed * 3.6f;
				}
				if (this.useDigitalGearGauge)
				{
					this.digitalGearGauge.stringValue = this.vehicleController.powertrain.transmission.GearName;
				}
				if (this.useLeftBlinkerDashLight)
				{
					this.leftBlinkerDashLight.Active = this.vehicleController.effectsManager.lightsManager.leftBlinkers.On;
				}
				if (this.useRightBlinkerDashLight)
				{
					this.rightBlinkerDashLight.Active = this.vehicleController.effectsManager.lightsManager.rightBlinkers.On;
				}
				if (this.useLowBeamDashLight)
				{
					this.lowBeamDashLight.Active = this.vehicleController.effectsManager.lightsManager.lowBeamLights.On;
				}
				if (this.useHighBeamDashLight)
				{
					this.highBeamDashLight.Active = this.vehicleController.effectsManager.lightsManager.highBeamLights.On;
				}
				if (this.useTcsDashLight && this._tcsModule != null)
				{
					this.tcsDashLight.Active = this._tcsModule.active;
				}
				if (this.useAbsDashLight && this._absModule != null)
				{
					this.absDashLight.Active = this._absModule.active;
				}
				if (this.useCheckEngineDashLight)
				{
					this.checkEngineDashLight.Active = (this.vehicleController.damageHandler.Damage > 0.9999f);
				}
			}
			else
			{
				if (this.useAnalogRpmGauge)
				{
					this.analogRpmGauge.Value = 0f;
				}
				if (this.useAnalogSpeedGauge)
				{
					this.analogSpeedGauge.Value = 0f;
				}
				if (this.useDigitalSpeedGauge)
				{
					this.digitalSpeedGauge.numericalValue = 0f;
				}
				if (this.useDigitalGearGauge)
				{
					this.digitalGearGauge.stringValue = "";
				}
				if (this.useLeftBlinkerDashLight)
				{
					this.leftBlinkerDashLight.Active = false;
				}
				if (this.useRightBlinkerDashLight)
				{
					this.rightBlinkerDashLight.Active = false;
				}
				if (this.useLowBeamDashLight)
				{
					this.lowBeamDashLight.Active = false;
				}
				if (this.useHighBeamDashLight)
				{
					this.highBeamDashLight.Active = false;
				}
				if (this.useTcsDashLight)
				{
					this.tcsDashLight.Active = false;
				}
				if (this.useAbsDashLight)
				{
					this.absDashLight.Active = false;
				}
				if (this.useCheckEngineDashLight)
				{
					this.checkEngineDashLight.Active = false;
				}
			}
			this._prevVc = this.vehicleController;
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x000C4E1E File Offset: 0x000C301E
		private void OnWake()
		{
			this._canvas.enabled = true;
			this._update = true;
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x000C4E33 File Offset: 0x000C3033
		private void OnSleep()
		{
			this._canvas.enabled = false;
			this._update = false;
		}

		// Token: 0x04002284 RID: 8836
		[FormerlySerializedAs("ABS")]
		public DashLight absDashLight;

		// Token: 0x04002285 RID: 8837
		public AnalogGauge analogRpmGauge;

		// Token: 0x04002286 RID: 8838
		public AnalogGauge analogSpeedGauge;

		// Token: 0x04002287 RID: 8839
		[FormerlySerializedAs("checkEngine")]
		public DashLight checkEngineDashLight;

		// Token: 0x04002288 RID: 8840
		public DashGUIController.DataSource dataSource;

		// Token: 0x04002289 RID: 8841
		public DigitalGauge digitalGearGauge;

		// Token: 0x0400228A RID: 8842
		public DigitalGauge digitalRpmGauge;

		// Token: 0x0400228B RID: 8843
		public DigitalGauge digitalSpeedGauge;

		// Token: 0x0400228C RID: 8844
		[FormerlySerializedAs("highBeam")]
		public DashLight highBeamDashLight;

		// Token: 0x0400228D RID: 8845
		[FormerlySerializedAs("leftBlinker")]
		public DashLight leftBlinkerDashLight;

		// Token: 0x0400228E RID: 8846
		[FormerlySerializedAs("lowBeam")]
		public DashLight lowBeamDashLight;

		// Token: 0x0400228F RID: 8847
		[FormerlySerializedAs("rightBlinker")]
		public DashLight rightBlinkerDashLight;

		// Token: 0x04002290 RID: 8848
		[FormerlySerializedAs("TCS")]
		public DashLight tcsDashLight;

		// Token: 0x04002291 RID: 8849
		public bool useAbsDashLight;

		// Token: 0x04002292 RID: 8850
		public bool useAnalogRpmGauge;

		// Token: 0x04002293 RID: 8851
		public bool useAnalogSpeedGauge;

		// Token: 0x04002294 RID: 8852
		public bool useCheckEngineDashLight;

		// Token: 0x04002295 RID: 8853
		public bool useDigitalGearGauge;

		// Token: 0x04002296 RID: 8854
		public bool useDigitalRpmGauge;

		// Token: 0x04002297 RID: 8855
		public bool useDigitalSpeedGauge;

		// Token: 0x04002298 RID: 8856
		public bool useHighBeamDashLight;

		// Token: 0x04002299 RID: 8857
		public bool useLeftBlinkerDashLight;

		// Token: 0x0400229A RID: 8858
		public bool useLowBeamDashLight;

		// Token: 0x0400229B RID: 8859
		public bool useRightBlinkerDashLight;

		// Token: 0x0400229C RID: 8860
		public bool useTcsDashLight;

		// Token: 0x0400229D RID: 8861
		public VehicleController vehicleController;

		// Token: 0x0400229E RID: 8862
		private Canvas _canvas;

		// Token: 0x0400229F RID: 8863
		private VehicleController _prevVc;

		// Token: 0x040022A0 RID: 8864
		private ABSModule _absModule;

		// Token: 0x040022A1 RID: 8865
		private TCSModule _tcsModule;

		// Token: 0x040022A2 RID: 8866
		private bool _update = true;

		// Token: 0x020004C4 RID: 1220
		public enum DataSource
		{
			// Token: 0x04002C20 RID: 11296
			VehicleController,
			// Token: 0x04002C21 RID: 11297
			VehicleChanger
		}
	}
}
