using System;
using System.Collections.Generic;
using System.Linq;
using NWH.VehiclePhysics2.Input;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using NWH.WheelController3D;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
	// Token: 0x0200027A RID: 634
	[Serializable]
	public class Powertrain : VehicleComponent
	{
		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060010A7 RID: 4263 RVA: 0x000BD9BC File Offset: 0x000BBBBC
		// (set) Token: 0x060010A8 RID: 4264 RVA: 0x000BD9C4 File Offset: 0x000BBBC4
		public bool HasWheelAir { get; private set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060010A9 RID: 4265 RVA: 0x000BD9CD File Offset: 0x000BBBCD
		// (set) Token: 0x060010AA RID: 4266 RVA: 0x000BD9D5 File Offset: 0x000BBBD5
		public bool HasWheelSkid { get; private set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060010AB RID: 4267 RVA: 0x000BD9DE File Offset: 0x000BBBDE
		// (set) Token: 0x060010AC RID: 4268 RVA: 0x000BD9E6 File Offset: 0x000BBBE6
		public bool HasWheelSpin { get; private set; }

		// Token: 0x060010AD RID: 4269 RVA: 0x000BD9F0 File Offset: 0x000BBBF0
		public override void Initialize()
		{
			List<PowertrainComponent> powertrainComponents = this.GetPowertrainComponents();
			this.solver.Initialize(powertrainComponents);
			foreach (PowertrainComponent powertrainComponent in powertrainComponents)
			{
				powertrainComponent.FindOutputs(this.solver);
			}
			foreach (PowertrainComponent powertrainComponent2 in powertrainComponents)
			{
				powertrainComponent2.FindInput(this.solver);
			}
			foreach (WheelComponent wheelComponent in this.wheels)
			{
				wheelComponent.vc = this.vc;
			}
			foreach (WheelGroup wheelGroup in this.wheelGroups)
			{
				wheelGroup.Initialize(this.vc);
			}
			this.initialized = true;
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x000BDB28 File Offset: 0x000BBD28
		public override void FixedUpdate()
		{
			this._frameCount++;
			foreach (WheelGroup wheelGroup in this.wheelGroups)
			{
				wheelGroup.CalculateCamber();
			}
			if (!base.Active)
			{
				return;
			}
			bool autoSyncTransforms = Physics.autoSyncTransforms;
			Physics.autoSyncTransforms = false;
			foreach (WheelGroup wheelGroup2 in this.wheelGroups)
			{
				wheelGroup2.CalculateARB();
			}
			Physics.autoSyncTransforms = autoSyncTransforms;
			this.HasWheelSkid = false;
			this.HasWheelSpin = false;
			this.HasWheelAir = false;
			int count = this.wheels.Count;
			if (this._frameCount >= this._groundCheckFrameIndex + count)
			{
				this._groundCheckFrameIndex = this._frameCount;
			}
			int num = this._frameCount - this._groundCheckFrameIndex;
			for (int i = 0; i < count; i++)
			{
				WheelComponent wheelComponent = this.wheels[i];
				if (wheelComponent.HasLongitudinalSlip)
				{
					this.HasWheelSpin = true;
				}
				if (wheelComponent.HasLateralSlip)
				{
					this.HasWheelSkid = true;
				}
				if (!wheelComponent.IsGrounded)
				{
					this.HasWheelAir = true;
				}
				if (this._prevFrameCount != this._frameCount && num == i)
				{
					this.vc.groundDetection.GetCurrentSurfaceMap(wheelComponent.wheelController, ref wheelComponent.surfaceMapIndex, ref wheelComponent.surfacePreset);
				}
			}
			float vertical = this.vc.input.Vertical;
			this.transmission.CheckForShift(vertical, this.HasWheelSpin, this.HasWheelSkid, this.HasWheelAir, this.engine.RPM, this.engine.minRPM, this.engine.revLimiterRPM, this.vc.vehicleRigidbody.velocity.magnitude, this.clutch.clutchEngagement, this.vc.transform.forward, this.vc.transform.up, this.vc.input.ShiftUp, this.vc.input.ShiftDown, this.vc.input.ShiftInto);
			this.vc.input.ResetShiftUpFlag();
			this.vc.input.ResetShiftDownFlag();
			this.vc.input.ShiftInto = -999;
			int gear = this.transmission.Gear;
			float throttlePosition = this.vc.input.Vertical;
			if (this.transmission.IsShifting)
			{
				throttlePosition = 0f;
			}
			else if (this.vc.input.throttleType == NWH.VehiclePhysics2.Input.Input.ThrottleType.WForwardSReverse)
			{
				throttlePosition = ((gear < 0) ? (-vertical) : vertical);
			}
			this.engine.ThrottlePosition = throttlePosition;
			this.engine.slipTorque = (this.clutch.hasTorqueConverter ? this.clutch.torqueConverterSlipTorque : this.clutch.slipTorque);
			this.clutch.fwdAcceleration = this.vc.ForwardAcceleration;
			this.clutch.gear = gear;
			this.clutch.shiftSignal = this.transmission.IsShifting;
			if (!this.clutch.isAutomatic)
			{
				this.clutch.clutchEngagement = this.vc.input.Clutch;
			}
			if (this.vc.input.EngineStartStop)
			{
				this.engine.StartStop();
				this.vc.input.ResetEngineStartStopFlag();
			}
			foreach (WheelComponent wheelComponent2 in this.wheels)
			{
				wheelComponent2.PreUpdate();
			}
			this.solver.Solve();
			foreach (WheelComponent wheelComponent3 in this.wheels)
			{
				wheelComponent3.PostUpdate();
			}
			if (this._frameCount > 9999999)
			{
				this._frameCount = 0;
			}
			this._prevFrameCount = this._frameCount;
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x00002188 File Offset: 0x00000388
		public override void Update()
		{
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x000BDF7C File Offset: 0x000BC17C
		public override void Enable()
		{
			base.Enable();
			foreach (PowertrainComponent powertrainComponent in this.solver.Components)
			{
				powertrainComponent.OnEnable();
			}
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x000BDFD8 File Offset: 0x000BC1D8
		public override void Disable()
		{
			base.Disable();
			foreach (PowertrainComponent powertrainComponent in this.solver.Components)
			{
				powertrainComponent.OnDisable();
			}
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x000BE034 File Offset: 0x000BC234
		public List<string> GetPowertrainComponentNames(List<PowertrainComponent> powertrainComponents)
		{
			return (from c in powertrainComponents
			select c.name).ToList<string>();
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x000BE060 File Offset: 0x000BC260
		public List<string> GetPowertrainComponentNames()
		{
			return this.GetPowertrainComponentNames(this.GetPowertrainComponents());
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x000BE06E File Offset: 0x000BC26E
		public List<string> GetPowertrainComponentNamesWithType(List<PowertrainComponent> powertrainComponents)
		{
			return powertrainComponents.Select(delegate(PowertrainComponent c)
			{
				string[] array = new string[5];
				array[0] = "[";
				int num = 1;
				string text = c.GetType().ToString().Split(new char[]
				{
					'.'
				}).LastOrDefault<string>();
				array[num] = ((text != null) ? text.Replace("Component", "") : null);
				array[2] = "] ";
				array[3] = c.name;
				array[4] = " ";
				return string.Concat(array);
			}).ToList<string>();
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x000BE09C File Offset: 0x000BC29C
		public List<PowertrainComponent> GetPowertrainComponents()
		{
			List<PowertrainComponent> list = new List<PowertrainComponent>
			{
				this.engine,
				this.clutch,
				this.transmission
			};
			foreach (DifferentialComponent item in this.differentials)
			{
				list.Add(item);
			}
			foreach (WheelComponent item2 in this.wheels)
			{
				list.Add(item2);
			}
			return list;
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x000BE160 File Offset: 0x000BC360
		public void GetWheelStates(out bool wheelSpin, out bool wheelSkid, out bool wheelAir)
		{
			wheelSpin = false;
			wheelSkid = false;
			wheelAir = false;
			foreach (WheelComponent wheelComponent in this.wheels)
			{
				if (wheelComponent.HasLongitudinalSlip)
				{
					wheelSpin = true;
				}
				if (wheelComponent.HasLateralSlip)
				{
					wheelSkid = true;
				}
				if (!wheelComponent.IsGrounded)
				{
					wheelAir = true;
				}
			}
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x000BE1D4 File Offset: 0x000BC3D4
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.engine.name = "Engine";
			this.engine.inertia = 0.2f;
			this.clutch.name = "Clutch";
			this.transmission.name = "Transmission";
			this.engine.SetOutput(this.clutch);
			this.clutch.SetOutput(this.transmission);
			this.engine.powerCurve = new AnimationCurve
			{
				keys = new Keyframe[]
				{
					new Keyframe(0f, 0f, 0f, 1f),
					new Keyframe(0.3f, 0.53f, 1f, 1f),
					new Keyframe(0.5f, 0.8f, 1f, 1f),
					new Keyframe(1f, 1f)
				}
			};
			this.transmission.gearingProfile = (Resources.Load("NWH Vehicle Physics/Defaults/DefaultGearingProfile") as TransmissionGearingProfile);
			this.wheels = new List<WheelComponent>();
			foreach (WheelController wheelController in vc.GetComponentsInChildren<WheelController>())
			{
				this.wheels.Add(new WheelComponent
				{
					name = "Wheel" + wheelController.name,
					wheelController = wheelController
				});
				Debug.Log("VehicleController setup: Found WheelController '" + wheelController.name + "'");
			}
			if (this.wheels.Count == 0)
			{
				Debug.LogWarning("No WheelControllers found, skipping powertrain auto-setup.");
				return;
			}
			this.wheels = (from w in this.wheels
			orderby w.wheelController.transform.localPosition.z descending
			select w).ToList<WheelComponent>();
			List<int> list = new List<int>();
			int num = 1;
			float num2 = this.wheels[0].wheelController.transform.localPosition.z;
			for (int j = 0; j < this.wheels.Count; j++)
			{
				float z = this.wheels[j].wheelController.transform.localPosition.z;
				if (Mathf.Abs(z - num2) > 0.2f)
				{
					num++;
				}
				else if (j > 0 && this.wheels[j].wheelController.transform.localPosition.x < this.wheels[j - 1].wheelController.transform.localPosition.x)
				{
					WheelComponent value = this.wheels[j - 1];
					this.wheels[j - 1] = this.wheels[j];
					this.wheels[j] = value;
				}
				list.Add(num - 1);
				num2 = z;
			}
			this.wheelGroups = new List<WheelGroup>();
			for (int k = 0; k < num; k++)
			{
				string arg = (k == 0) ? "Front" : ((k == num - 1) ? "Rear" : "Middle");
				string text = string.Format("{0} Axle {1}", arg, k);
				this.wheelGroups.Add(new WheelGroup
				{
					name = text,
					brakeCoefficient = ((k == 0 || num > 2) ? 1f : 0.7f),
					handbrakeCoefficient = ((k == num - 1) ? 1f : 0f),
					steerCoefficient = ((k == 0) ? 1f : ((k == 1 && num > 2) ? 0.5f : 0f)),
					ackermanPercent = ((k == 0) ? 0.12f : 0f),
					antiRollBarForce = 3000f,
					isSolid = false
				});
				Debug.Log("VehicleController setup: Creating WheelGroup '" + text + "'");
			}
			this.differentials = new List<DifferentialComponent>();
			Debug.Log("[Powertrain] Adding 'Front Differential'");
			this.differentials.Add(new DifferentialComponent
			{
				name = "Front Differential"
			});
			Debug.Log("[Powertrain] Adding 'Rear Differential'");
			this.differentials.Add(new DifferentialComponent
			{
				name = "Rear Differential"
			});
			Debug.Log("[Powertrain] Adding 'Center Differential'");
			this.differentials.Add(new DifferentialComponent
			{
				name = "Center Differential"
			});
			this.differentials[2].SetOutput(this.differentials[0], this.differentials[1]);
			Debug.Log("[Powertrain] Setting transmission output to '" + this.differentials[2].name + "'");
			this.transmission.SetOutput(this.differentials[2]);
			for (int l = 0; l < this.wheels.Count; l++)
			{
				int index = list[l];
				this.wheels[l].wheelGroupSelector = new WheelGroupSelector
				{
					index = index
				};
				Debug.Log(string.Concat(new string[]
				{
					"[Powertrain] Adding '",
					this.wheels[l].name,
					"' to '",
					this.wheelGroups[index].name,
					"'"
				}));
			}
			int count = this.differentials.Count;
			int num3 = this.wheelGroups.Count;
			num3 = ((num > 2) ? 2 : num);
			for (int m = 0; m < num3; m++)
			{
				List<WheelComponent> list2 = this.wheelGroups[m].FindWheelsBelongingToGroup(ref this.wheels, m);
				if (list2.Count == 2)
				{
					Debug.Log(string.Concat(new string[]
					{
						"[Powertrain] Setting output of '",
						this.differentials[m].name,
						"' to '",
						list2[0].name,
						"'"
					}));
					if (list2[0].wheelController.vehicleSide == WheelController.Side.Left)
					{
						this.differentials[m].SetOutput(list2[0], list2[1]);
					}
					else if (list2[0].wheelController.vehicleSide == WheelController.Side.Right)
					{
						this.differentials[m].SetOutput(list2[1], list2[0]);
					}
					else
					{
						Debug.LogWarning("[Powertrain] Powertrain settings for center wheels have to be manually set up. If powered either connect it directly to transmission (motorcycle) or to one side of center differential (trike).");
					}
				}
			}
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x000BE880 File Offset: 0x000BCA80
		public override void Validate(VehicleController vc)
		{
			base.Validate(vc);
			this.engine.Validate(vc);
			this.clutch.Validate(vc);
			this.transmission.Validate(vc);
			foreach (DifferentialComponent differentialComponent in this.differentials)
			{
				differentialComponent.Validate(vc);
			}
			foreach (WheelComponent wheelComponent in this.wheels)
			{
				wheelComponent.Validate(vc);
			}
		}

		// Token: 0x04002107 RID: 8455
		[Tooltip("Powertrain components")]
		public ClutchComponent clutch = new ClutchComponent();

		// Token: 0x04002108 RID: 8456
		public List<DifferentialComponent> differentials = new List<DifferentialComponent>();

		// Token: 0x04002109 RID: 8457
		public EngineComponent engine = new EngineComponent();

		// Token: 0x0400210A RID: 8458
		public Solver solver = new Solver();

		// Token: 0x0400210B RID: 8459
		public TransmissionComponent transmission = new TransmissionComponent();

		// Token: 0x0400210C RID: 8460
		public List<WheelGroup> wheelGroups = new List<WheelGroup>();

		// Token: 0x0400210D RID: 8461
		public List<WheelComponent> wheels = new List<WheelComponent>();

		// Token: 0x0400210E RID: 8462
		private int _frameCount;

		// Token: 0x0400210F RID: 8463
		private int _groundCheckFrameIndex = -999;

		// Token: 0x04002110 RID: 8464
		private int _prevFrameCount = -999;

		// Token: 0x04002111 RID: 8465
		private bool engineWasRunning;
	}
}
