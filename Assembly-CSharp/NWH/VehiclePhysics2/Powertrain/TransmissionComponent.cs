using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Powertrain
{
	// Token: 0x0200027F RID: 639
	[Serializable]
	public class TransmissionComponent : PowertrainComponent
	{
		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06001105 RID: 4357 RVA: 0x000C00F0 File Offset: 0x000BE2F0
		public int CurrentGearIndex
		{
			get
			{
				return this._currentGearIndex;
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06001106 RID: 4358 RVA: 0x000C00F8 File Offset: 0x000BE2F8
		public float DownshiftAngularVelocity
		{
			get
			{
				return UnitConverter.RPMToAngularVelocity(this._downshiftRPM);
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06001107 RID: 4359 RVA: 0x000C0105 File Offset: 0x000BE305
		// (set) Token: 0x06001108 RID: 4360 RVA: 0x000C010D File Offset: 0x000BE30D
		public float DownshiftRPM
		{
			get
			{
				return this._downshiftRPM;
			}
			set
			{
				this._downshiftRPM = Mathf.Clamp(value, 0f, float.PositiveInfinity);
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06001109 RID: 4361 RVA: 0x000C0125 File Offset: 0x000BE325
		public int ForwardGearCount
		{
			get
			{
				return this.gearRatios.Count - 1 - this.gearingProfile.reverseGears.Count;
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x0600110A RID: 4362 RVA: 0x000C0145 File Offset: 0x000BE345
		// (set) Token: 0x0600110B RID: 4363 RVA: 0x000C0152 File Offset: 0x000BE352
		public List<float> ForwardGears
		{
			get
			{
				return this.gearingProfile.forwardGears;
			}
			set
			{
				this.gearingProfile.forwardGears = value;
				this.ReconstructGearList();
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x0600110C RID: 4364 RVA: 0x000C0168 File Offset: 0x000BE368
		// (set) Token: 0x0600110D RID: 4365 RVA: 0x000C01CC File Offset: 0x000BE3CC
		public int Gear
		{
			get
			{
				int count = this.gearingProfile.reverseGears.Count;
				int count2 = this.gearRatios.Count;
				if (this._currentGearIndex < 0 - count)
				{
					return this._currentGearIndex = 0 - count;
				}
				if (this._currentGearIndex >= count2 - count - 1)
				{
					return this._currentGearIndex = count2 - count - 1;
				}
				return this._currentGearIndex;
			}
			set
			{
				int count = this.gearingProfile.reverseGears.Count;
				int count2 = this.gearRatios.Count;
				if (value < 0 - count)
				{
					this._currentGearIndex = 0 - count;
					return;
				}
				if (value >= count2 - count - 1)
				{
					this._currentGearIndex = this.ForwardGearCount;
					return;
				}
				if (value >= -100)
				{
					this._currentGearIndex = value;
				}
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600110E RID: 4366 RVA: 0x000C0228 File Offset: 0x000BE428
		[ShowInTelemetry]
		public string GearName
		{
			get
			{
				float num = (float)this.Gear;
				if (num == 0f)
				{
					return "N";
				}
				if (num > 0f)
				{
					return this.Gear.ToString();
				}
				if (this.gearingProfile.reverseGears.Count > 1)
				{
					return "R" + ((num < 0f) ? (-num) : num);
				}
				return "R";
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x0600110F RID: 4367 RVA: 0x000C0297 File Offset: 0x000BE497
		public List<float> Gears
		{
			get
			{
				return this.gearRatios;
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06001110 RID: 4368 RVA: 0x000C029F File Offset: 0x000BE49F
		public bool IsInReverse
		{
			get
			{
				return this._ratio < 0f;
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06001111 RID: 4369 RVA: 0x000C02AE File Offset: 0x000BE4AE
		public bool IsShifting
		{
			get
			{
				return Time.realtimeSinceStartup < this.lastShiftTime + this.shiftDuration;
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06001112 RID: 4370 RVA: 0x000C02C7 File Offset: 0x000BE4C7
		[ShowInTelemetry]
		public float Ratio
		{
			get
			{
				return this._ratio;
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06001113 RID: 4371 RVA: 0x000C02CF File Offset: 0x000BE4CF
		public int ReverseGearCount
		{
			get
			{
				return this.gearingProfile.reverseGears.Count;
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06001114 RID: 4372 RVA: 0x000C02E1 File Offset: 0x000BE4E1
		// (set) Token: 0x06001115 RID: 4373 RVA: 0x000C02EE File Offset: 0x000BE4EE
		public List<float> ReverseGears
		{
			get
			{
				return this.gearingProfile.reverseGears;
			}
			set
			{
				this.gearingProfile.reverseGears = value;
				this.ReconstructGearList();
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06001116 RID: 4374 RVA: 0x000C0302 File Offset: 0x000BE502
		[ShowInTelemetry]
		public float TargetDownshiftRPM
		{
			get
			{
				return this._targetDownshiftRPM;
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06001117 RID: 4375 RVA: 0x000C030A File Offset: 0x000BE50A
		[ShowInTelemetry]
		public float TargetUpshiftRPM
		{
			get
			{
				return this._targetUpshiftRPM;
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06001118 RID: 4376 RVA: 0x000C0312 File Offset: 0x000BE512
		// (set) Token: 0x06001119 RID: 4377 RVA: 0x000C031A File Offset: 0x000BE51A
		public TransmissionComponent.Type TransmissionType
		{
			get
			{
				return this._transmissionType;
			}
			set
			{
				this._transmissionType = value;
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x0600111A RID: 4378 RVA: 0x000C0323 File Offset: 0x000BE523
		public float UpshiftAngularVelocity
		{
			get
			{
				return UnitConverter.RPMToAngularVelocity(this._upshiftRPM);
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x0600111B RID: 4379 RVA: 0x000C0330 File Offset: 0x000BE530
		// (set) Token: 0x0600111C RID: 4380 RVA: 0x000C0338 File Offset: 0x000BE538
		public float UpshiftRPM
		{
			get
			{
				return this._upshiftRPM;
			}
			set
			{
				this._upshiftRPM = Mathf.Clamp(value, 0f, float.PositiveInfinity);
			}
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x000C0350 File Offset: 0x000BE550
		public override void Initialize()
		{
			this.ReconstructGearList();
			this.cAutoShift = new TransmissionComponent.Shift(this.AutomaticShift);
			this.cManualShift = new TransmissionComponent.Shift(this.ManualShift);
			this.cCvtShift = new TransmissionComponent.Shift(this.CVTShift);
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x000C0390 File Offset: 0x000BE590
		public void CheckForShift(float throttle, bool wheelSpin, bool wheelSkid, bool wheelAir, float engineRPM, float minEngineRPM, float revLimiterRPM, float vehicleSpeed, float clutchEngagement, Vector3 forward, Vector3 up, bool upshiftSignal, bool downshiftSignal, int shiftIntoSignal = -999)
		{
			this.noWheelSpin = !wheelSpin;
			this.noWheelSkid = !wheelSkid;
			this.noWheelAir = !wheelAir;
			this.clutchEngaged = (clutchEngagement > 0.8f);
			this._ratio = this.GetCurrentGearRatio();
			if (!this.noWheelSpin || !this.noWheelSkid || !this.noWheelAir || !this.clutchEngaged)
			{
				this.shiftCheckTime = Time.realtimeSinceStartup;
				this.shiftCheckValid = false;
			}
			else if (this.shiftCheckTime + this.shiftCheckCooldown <= Time.realtimeSinceStartup)
			{
				this.shiftCheckValid = true;
			}
			if (this._transmissionType == TransmissionComponent.Type.Manual)
			{
				this.shiftDelegate = this.cManualShift;
			}
			else if (this._transmissionType == TransmissionComponent.Type.Automatic || this._transmissionType == TransmissionComponent.Type.AutomaticSequential)
			{
				this.shiftDelegate = this.cAutoShift;
			}
			else if (this._transmissionType == TransmissionComponent.Type.CVT)
			{
				this.shiftDelegate = this.cCvtShift;
			}
			this.externalShiftChecksValid = true;
			using (List<TransmissionComponent.ShiftCheck>.Enumerator enumerator = this.shiftChecks.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current())
					{
						this.externalShiftChecksValid = false;
						break;
					}
				}
			}
			this.shiftDelegate(upshiftSignal, downshiftSignal, shiftIntoSignal, this.externalShiftChecksValid, throttle, engineRPM, minEngineRPM, revLimiterRPM, vehicleSpeed, forward, up);
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x000C04F0 File Offset: 0x000BE6F0
		public float GetCurrentGearRatio()
		{
			return this.gearRatios[this.GearToIndex(this._currentGearIndex)] * this.finalGearRatio;
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x000C0510 File Offset: 0x000BE710
		public float GetGearRatio(int g)
		{
			return this.gearRatios[this.GearToIndex(g)] * this.finalGearRatio;
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x000C052B File Offset: 0x000BE72B
		public override void OnPreSolve()
		{
			base.OnPreSolve();
			this.totalReceivedTorque = 0f;
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x000C053E File Offset: 0x000BE73E
		public override float QueryAngularVelocity(float inputAngularVelocity, float dt)
		{
			this.angularVelocity = inputAngularVelocity;
			if (this._ratio == 0f || this._outputAIsNull)
			{
				return inputAngularVelocity;
			}
			return this.outputA.QueryAngularVelocity(inputAngularVelocity, dt) * this._ratio;
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x000C0572 File Offset: 0x000BE772
		public override float QueryInertia()
		{
			if (this._outputAIsNull || this._ratio == 0f)
			{
				return this.inertia;
			}
			return this.inertia + this.outputA.QueryInertia() / (this._ratio * this._ratio);
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x000C05B0 File Offset: 0x000BE7B0
		public void ReconstructGearList()
		{
			this.gearRatios.Clear();
			this.gearRatios.AddRange(this.gearingProfile.reverseGears);
			this.gearRatios.Add(0f);
			this.gearRatios.AddRange(this.gearingProfile.forwardGears);
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x000C0604 File Offset: 0x000BE804
		public float ReverseTransmitRPM(float inputRPM, int g)
		{
			float num = inputRPM * this.gearRatios[this.GearToIndex(g)] * this.finalGearRatio;
			return (num < 0f) ? (-num) : num;
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x000C063C File Offset: 0x000BE83C
		public override float SendTorque(float torque, float inertiaSum, float dt)
		{
			this.totalReceivedTorque += torque;
			if (this._outputAIsNull)
			{
				return torque;
			}
			if (this._ratio == 0f)
			{
				this.outputA.SendTorque(0f, 0f, dt);
				return torque;
			}
			return this.outputA.SendTorque(torque * this._ratio, (inertiaSum + this.inertia) * (this._ratio * this._ratio), dt) / this._ratio;
		}

		// Token: 0x06001127 RID: 4391 RVA: 0x000C06B8 File Offset: 0x000BE8B8
		public void ShiftInto(int g)
		{
			if (g == this.Gear)
			{
				return;
			}
			int gear = this.Gear;
			if (this.externalShiftChecksValid && Time.realtimeSinceStartup > this.lastShiftTime + this.shiftDuration + this.postShiftBan)
			{
				if (gear < this.Gear)
				{
					this.onUpshift.Invoke(g);
				}
				else if (gear > this.Gear)
				{
					this.onDownshift.Invoke(g);
				}
				this.onShift.Invoke(g);
				if (this.Gear != 0)
				{
					this.lastShiftTime = Time.realtimeSinceStartup;
				}
				this.Gear = g;
			}
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x000C074C File Offset: 0x000BE94C
		public override void Validate(VehicleController vc)
		{
			base.Validate(vc);
			if (this.gearingProfile == null)
			{
				Debug.LogError("Transmission gearing profile not assigned.");
			}
			else
			{
				foreach (float num in this.gearingProfile.reverseGears)
				{
				}
				foreach (float num2 in this.gearingProfile.forwardGears)
				{
				}
			}
			if (this._upshiftRPM > vc.powertrain.engine.revLimiterRPM || this._upshiftRPM > vc.powertrain.engine.maxRPM)
			{
				Debug.LogWarning("Transmission upshift RPM set to higher RPM than the engine can achieve (check engine maxRPM and revLimiterRPM).");
			}
			if (this._downshiftRPM < vc.powertrain.engine.stallRPM || this._downshiftRPM < vc.powertrain.engine.idleRPM)
			{
				Debug.LogWarning("Transmission downshift RPM set to lower RPM than the engine can achieve (check engine stallRPM and idleRPM).");
			}
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x000C0874 File Offset: 0x000BEA74
		private void AutomaticShift(bool upshiftSignal, bool downshiftSignal, int shiftIntoSignal, bool checksValid, float yAxis, float engineRPM, float minEngineRPM, float revLimiterRPM, float vehicleSpeed, Vector3 forward, Vector3 up)
		{
			float num = (yAxis < 0f) ? 0f : yAxis;
			float smoothTime = (num > this.smoothedYAxis01) ? 0.2f : 2f;
			this.smoothedYAxis01 = Mathf.SmoothDamp(this.smoothedYAxis01, num, ref this.verticalInputChangeVelocity, smoothTime);
			this._targetDownshiftRPM = this._downshiftRPM;
			this._targetUpshiftRPM = this._upshiftRPM;
			if (this.variableShiftPoint)
			{
				this._targetUpshiftRPM = this._upshiftRPM + Mathf.Clamp01(this.smoothedYAxis01 * this.variableShiftIntensity) * revLimiterRPM;
				this._targetDownshiftRPM = this._downshiftRPM + Mathf.Clamp01(this.smoothedYAxis01 * this.variableShiftIntensity) * revLimiterRPM;
				this._targetUpshiftRPM = Mathf.Clamp(this._targetUpshiftRPM, this._upshiftRPM, revLimiterRPM);
				this._targetDownshiftRPM = Mathf.Clamp(this._targetDownshiftRPM, minEngineRPM * 1.1f, this._targetUpshiftRPM * 0.6f);
				float num2 = Mathf.Clamp01(Vector3.Dot(forward, Vector3.up) * this.inclineEffectCoeff);
				num2 *= num2;
				this._targetUpshiftRPM += revLimiterRPM * (num2 * 3f);
				this._targetDownshiftRPM += revLimiterRPM * (num2 * 1.8f);
			}
			int gear = this.Gear;
			if (gear == 0)
			{
				if (yAxis > 0.1f)
				{
					this.Gear = 1;
					return;
				}
				if (yAxis < -0.1f)
				{
					this.Gear = -1;
					return;
				}
			}
			else if (gear < 0)
			{
				if (yAxis > 0.1f && vehicleSpeed < 1f)
				{
					this.ShiftInto(1);
				}
				else if (yAxis > -0.1f && yAxis < 0.1f && vehicleSpeed < 1f)
				{
					this.ShiftInto(0);
				}
				if (engineRPM > this.TargetUpshiftRPM && Mathf.Abs(gear - 1) < this.ReverseGearCount)
				{
					this.ShiftInto(gear - 1);
					return;
				}
				if (engineRPM < this.TargetDownshiftRPM)
				{
					if (gear == -1 && yAxis > -0.1f && yAxis < 0.1f)
					{
						this.ShiftInto(0);
						return;
					}
					if (gear < -1)
					{
						this.ShiftInto(gear + 1);
						return;
					}
				}
			}
			else if (vehicleSpeed > 0.2f)
			{
				if (gear < this.ForwardGearCount && engineRPM > this.TargetUpshiftRPM && this.shiftCheckValid)
				{
					if (this.TransmissionType != TransmissionComponent.Type.Automatic)
					{
						this.ShiftInto(gear + 1);
						return;
					}
					int i = gear;
					int num3 = (this.smoothedYAxis01 > 0.8f) ? (gear + 1) : (gear + 2);
					while (i < this.ForwardGearCount)
					{
						i++;
						if (i == num3)
						{
							break;
						}
						if (this.ReverseTransmitRPM(base.RPM / this._ratio, i) < this.TargetDownshiftRPM)
						{
							i--;
							break;
						}
					}
					if (i != gear)
					{
						this.ShiftInto(i);
						return;
					}
				}
				else if (engineRPM < this.TargetDownshiftRPM)
				{
					if (this._transmissionType == TransmissionComponent.Type.Automatic)
					{
						if (gear != 1)
						{
							int j = gear;
							while (j > 1)
							{
								j--;
								if (this.ReverseTransmitRPM(base.RPM / this._ratio, j) > this.TargetUpshiftRPM)
								{
									j++;
									break;
								}
							}
							if (j != gear)
							{
								this.ShiftInto(j);
								return;
							}
						}
						else if (vehicleSpeed < 1f && yAxis > -0.1f && yAxis < 0.1f)
						{
							this.ShiftInto(0);
							return;
						}
					}
					else
					{
						if (this.Gear != 1)
						{
							this.ShiftInto(this.Gear - 1);
							return;
						}
						if (vehicleSpeed < 1f && yAxis < 0.1f && yAxis > -0.1f)
						{
							this.ShiftInto(0);
							return;
						}
					}
				}
			}
			else
			{
				if (vehicleSpeed < 0.2f && yAxis <= -0.1f)
				{
					this.ShiftInto(-1);
					return;
				}
				if (yAxis < 0.1f && yAxis > -0.1f && vehicleSpeed > -0.2f && vehicleSpeed < 0.2f)
				{
					this.ShiftInto(0);
				}
			}
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x000C0C3C File Offset: 0x000BEE3C
		private void CVTShift(bool upshiftSignal, bool downshiftSignal, int shiftIntoSignal, bool checksValid, float yAxis, float engineRPM, float minEngineRPM, float maxEngineRPM, float vehicleSpeed, Vector3 forward, Vector3 up)
		{
			float num = this.gearingProfile.forwardGears[0];
			float num2 = this.gearingProfile.forwardGears[1];
			this.totalReceivedTorque *= Time.fixedDeltaTime;
			float num3 = ((this.totalReceivedTorque < 0f) ? (-this.totalReceivedTorque) : this.totalReceivedTorque) / this.cvtMaxInputTorque;
			num3 = ((num3 < 0f) ? 0f : ((num3 > 1f) ? 1f : num3));
			this.Gear = 1;
			this._ratio = num * num3 + num2 * (1f - num3);
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x000C0CDD File Offset: 0x000BEEDD
		private int GearToIndex(int g)
		{
			return g + this.gearingProfile.reverseGears.Count;
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x000C0CF1 File Offset: 0x000BEEF1
		private void ManualShift(bool upshiftSignal, bool downshiftSignal, int shiftIntoSignal, bool checksValid, float yAxis, float engineRPM, float minEngineRPM, float maxEngineRPM, float vehicleSpeed, Vector3 forward, Vector3 up)
		{
			if (upshiftSignal)
			{
				this.ShiftInto(this.Gear + 1);
			}
			if (downshiftSignal)
			{
				this.ShiftInto(this.Gear - 1);
			}
			if (shiftIntoSignal > -100)
			{
				this.ShiftInto(shiftIntoSignal);
			}
		}

		// Token: 0x04002176 RID: 8566
		public float cvtMaxInputTorque = 300f;

		// Token: 0x04002177 RID: 8567
		[ShowInSettings("Final Gear Ratio", 1f, 8f, 0.5f)]
		[Tooltip("    Final gear multiplier. Each gear gets multiplied by this value.\r\n    Equivalent to axle/differential ratio in real life.")]
		public float finalGearRatio = 3f;

		// Token: 0x04002178 RID: 8568
		[Tooltip("    Currently active gearing profile.\r\n    Final gear ratio will be determined from this and final gear ratio.")]
		public TransmissionGearingProfile gearingProfile;

		// Token: 0x04002179 RID: 8569
		[Range(0f, 4f)]
		public float inclineEffectCoeff;

		// Token: 0x0400217A RID: 8570
		[SerializeField]
		public TransmissionComponent.ShiftEvent onDownshift;

		// Token: 0x0400217B RID: 8571
		[SerializeField]
		public TransmissionComponent.ShiftEvent onShift;

		// Token: 0x0400217C RID: 8572
		[SerializeField]
		public TransmissionComponent.ShiftEvent onUpshift;

		// Token: 0x0400217D RID: 8573
		[Tooltip("    Time after shifting in which shifting can not be done again.")]
		public float postShiftBan = 0.5f;

		// Token: 0x0400217E RID: 8574
		[Tooltip("Behavior when switching from neutral or forward gears to reverse gear. Auto - if the vehicle speed is low enough and vertical input negative, transmission will shift to reverse. DoubleTap - once all the requirements exist for shifting into reverse, user has to release the button and press it again to shift, otherwise the vehicle will stand still in neutral with brakes applied.")]
		public TransmissionComponent.ReverseType reverseType;

		// Token: 0x0400217F RID: 8575
		public float shiftCheckCooldown = 0.1f;

		// Token: 0x04002180 RID: 8576
		[HideInInspector]
		public List<TransmissionComponent.ShiftCheck> shiftChecks = new List<TransmissionComponent.ShiftCheck>();

		// Token: 0x04002181 RID: 8577
		public bool shiftCheckValid = true;

		// Token: 0x04002182 RID: 8578
		public TransmissionComponent.Shift shiftDelegate;

		// Token: 0x04002183 RID: 8579
		[ShowInSettings("Shift Duration", 0.01f, 0.5f, 0.05f)]
		[Tooltip("    Time it takes transmission to shift between gears.")]
		public float shiftDuration = 0.2f;

		// Token: 0x04002184 RID: 8580
		[Range(0f, 1f)]
		public float variableShiftIntensity = 0.3f;

		// Token: 0x04002185 RID: 8581
		[Tooltip("    If enabled transmission will adjust both shift up and down points to match current load.")]
		public bool variableShiftPoint = true;

		// Token: 0x04002186 RID: 8582
		protected float _ratio;

		// Token: 0x04002187 RID: 8583
		[SerializeField]
		private int _currentGearIndex;

		// Token: 0x04002188 RID: 8584
		[Tooltip("RPM at which automatic transmission will shift down. If dynamic shift point is enabled this value will change depending on load.")]
		[SerializeField]
		private float _downshiftRPM = 1400f;

		// Token: 0x04002189 RID: 8585
		[SerializeField]
		private float _targetDownshiftRPM;

		// Token: 0x0400218A RID: 8586
		[SerializeField]
		private float _targetUpshiftRPM;

		// Token: 0x0400218B RID: 8587
		[SerializeField]
		[Tooltip("Manual - gears can only be shifted by manual user input. Automatic - automatic gear changing. Allows for gear skipping (e.g. 3rd->5th) which can be useful in trucks and other high gear count vehicles. AutomaticSequential - automatic gear changing but only one gear at the time can be shifted (e.g. 3rd->4th)")]
		[ShowInSettings("Transmission Type")]
		private TransmissionComponent.Type _transmissionType = TransmissionComponent.Type.AutomaticSequential;

		// Token: 0x0400218C RID: 8588
		[Tooltip("RPM at which automatic transmission will shift up. If dynamic shift point is enabled this value will change depending on load.")]
		[SerializeField]
		private float _upshiftRPM = 2800f;

		// Token: 0x0400218D RID: 8589
		private TransmissionComponent.Shift cAutoShift;

		// Token: 0x0400218E RID: 8590
		private TransmissionComponent.Shift cCvtShift;

		// Token: 0x0400218F RID: 8591
		[SerializeField]
		private bool clutchEngaged;

		// Token: 0x04002190 RID: 8592
		private TransmissionComponent.Shift cManualShift;

		// Token: 0x04002191 RID: 8593
		[SerializeField]
		private bool externalShiftChecksValid = true;

		// Token: 0x04002192 RID: 8594
		private List<float> gearRatios = new List<float>();

		// Token: 0x04002193 RID: 8595
		private float lastShiftTime = -10f;

		// Token: 0x04002194 RID: 8596
		[SerializeField]
		private bool noWheelAir;

		// Token: 0x04002195 RID: 8597
		[SerializeField]
		private bool noWheelSkid;

		// Token: 0x04002196 RID: 8598
		[SerializeField]
		private bool noWheelSpin;

		// Token: 0x04002197 RID: 8599
		private float shiftCheckTime = -999f;

		// Token: 0x04002198 RID: 8600
		private float smoothedYAxis01;

		// Token: 0x04002199 RID: 8601
		private float totalReceivedTorque;

		// Token: 0x0400219A RID: 8602
		private float verticalInputChangeVelocity;

		// Token: 0x020004B6 RID: 1206
		// (Invoke) Token: 0x06001B15 RID: 6933
		public delegate void Shift(bool upshiftSignal, bool downshiftSignal, int shiftIntoSignal, bool checksValid, float yAxis, float engineRPM, float minEngineRPM, float maxEngineRPM, float vehicleSpeed, Vector3 forward, Vector3 up);

		// Token: 0x020004B7 RID: 1207
		// (Invoke) Token: 0x06001B19 RID: 6937
		public delegate bool ShiftCheck();

		// Token: 0x020004B8 RID: 1208
		public enum ReverseType
		{
			// Token: 0x04002BF5 RID: 11253
			Auto,
			// Token: 0x04002BF6 RID: 11254
			DoubleTap
		}

		// Token: 0x020004B9 RID: 1209
		public enum Type
		{
			// Token: 0x04002BF8 RID: 11256
			Manual,
			// Token: 0x04002BF9 RID: 11257
			Automatic,
			// Token: 0x04002BFA RID: 11258
			AutomaticSequential,
			// Token: 0x04002BFB RID: 11259
			CVT,
			// Token: 0x04002BFC RID: 11260
			External
		}

		// Token: 0x020004BA RID: 1210
		[Serializable]
		public class ShiftEvent : UnityEvent<int>
		{
		}
	}
}
