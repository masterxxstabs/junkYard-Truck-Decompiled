using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Powertrain
{
	// Token: 0x0200027D RID: 637
	[Serializable]
	public class EngineComponent : PowertrainComponent
	{
		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060010D1 RID: 4305 RVA: 0x000BF3BC File Offset: 0x000BD5BC
		// (set) Token: 0x060010D2 RID: 4306 RVA: 0x000BF3C4 File Offset: 0x000BD5C4
		[ShowInTelemetry]
		public float GeneratedTorque { get; private set; }

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060010D3 RID: 4307 RVA: 0x000BF3CD File Offset: 0x000BD5CD
		public float IdleAngularVelocity
		{
			get
			{
				return UnitConverter.RPMToAngularVelocity(this.idleRPM);
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060010D4 RID: 4308 RVA: 0x000BF3DA File Offset: 0x000BD5DA
		public bool IsRunning
		{
			get
			{
				return this.ignition && this.angularVelocity > this._stallAngularVelocity;
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060010D5 RID: 4309 RVA: 0x000BF3F4 File Offset: 0x000BD5F4
		public float MaxAngularVelocity
		{
			get
			{
				return UnitConverter.RPMToAngularVelocity(this.maxRPM);
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060010D6 RID: 4310 RVA: 0x000BF401 File Offset: 0x000BD601
		public float MinAngularVelocity
		{
			get
			{
				return UnitConverter.RPMToAngularVelocity(this.minRPM);
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060010D7 RID: 4311 RVA: 0x000BF40E File Offset: 0x000BD60E
		public float RevLimiterAngularVelocity
		{
			get
			{
				return UnitConverter.RPMToAngularVelocity(this.revLimiterRPM);
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060010D8 RID: 4312 RVA: 0x000BF41B File Offset: 0x000BD61B
		public float RPMPercent
		{
			get
			{
				return base.RPM / this.maxRPM;
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060010D9 RID: 4313 RVA: 0x000BF42A File Offset: 0x000BD62A
		public float StallAngularVelocity
		{
			get
			{
				return UnitConverter.RPMToAngularVelocity(this.stallRPM);
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060010DA RID: 4314 RVA: 0x000BF437 File Offset: 0x000BD637
		// (set) Token: 0x060010DB RID: 4315 RVA: 0x000BF43F File Offset: 0x000BD63F
		public bool StarterActive
		{
			get
			{
				return this.starterActive;
			}
			set
			{
				this.starterActive = value;
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060010DC RID: 4316 RVA: 0x000BF448 File Offset: 0x000BD648
		public float StarterMaxAngularVelocity
		{
			get
			{
				return UnitConverter.RPMToAngularVelocity(this.starterRPMLimit);
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060010DD RID: 4317 RVA: 0x000BF455 File Offset: 0x000BD655
		// (set) Token: 0x060010DE RID: 4318 RVA: 0x000BF45D File Offset: 0x000BD65D
		public float ThrottlePosition
		{
			get
			{
				return this.throttlePosition;
			}
			set
			{
				this.throttlePosition = Mathf.Clamp01(value);
			}
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x000BF46C File Offset: 0x000BD66C
		public override void Initialize()
		{
			this._revLimiterStartTime = -999f;
			if (this._isElectric)
			{
				this.revLimiterCutoffDuration = 0f;
				this.stallingEnabled = false;
				this.idleRPM = 0f;
				this.minRPM = -this.revLimiterRPM;
				this.maxRPM = this.revLimiterRPM;
				this.starterActive = false;
				this.starterRunTime = 0f;
				this.ignition = true;
				this.isStalling = false;
				this.stallRPM = float.NegativeInfinity;
			}
			if (this.engineType == EngineComponent.EngineType.ICE)
			{
				this.calculateTorqueDelegate = new EngineComponent.CalculateTorque(this.CalculateTorqueICE);
				return;
			}
			if (this.engineType == EngineComponent.EngineType.Electric)
			{
				this.calculateTorqueDelegate = new EngineComponent.CalculateTorque(this.CalculateTorqueElectric);
			}
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x000BF524 File Offset: 0x000BD724
		public void Start()
		{
			this.ignition = true;
			if (base.ComponentDamage < 1f && !this.IsRunning)
			{
				if (this.flyingStartEnabled)
				{
					this.isStalling = false;
					this.starterActive = false;
					this.ignition = true;
					this.angularVelocity = UnitConverter.RPMToAngularVelocity(this.idleRPM);
				}
				else
				{
					this.StarterActive = true;
				}
				this.OnStart.Invoke();
				return;
			}
			this.StarterActive = false;
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x000BF597 File Offset: 0x000BD797
		public override void OnEnable()
		{
			base.OnEnable();
			if (this.flyingStartEnabled || this._wasRunning)
			{
				this.Start();
			}
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x000BF5B5 File Offset: 0x000BD7B5
		public override void OnDisable()
		{
			base.OnDisable();
			this._wasRunning = this.IsRunning;
			if (this.IsRunning)
			{
				this.Stop();
			}
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x000BF5D7 File Offset: 0x000BD7D7
		public void StartStop()
		{
			if (this.IsRunning)
			{
				this.Stop();
				return;
			}
			this.Start();
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x000BF5F0 File Offset: 0x000BD7F0
		public float CalculateTorqueElectric(float angularVelocity, float dt)
		{
			this.throttlePosition = this._initThrottlePosition;
			if (angularVelocity > -this._revLimiterAngularVelocity && angularVelocity < this._revLimiterAngularVelocity)
			{
				this.generatedPower = this.powerCurve.Evaluate(angularVelocity / this._maxAngularVelocity) * this.maxPower * this.throttlePosition * this.powerModifierSum;
				if (this.throttlePosition != 0f && angularVelocity == 0f)
				{
					this.generatedPower = 1f;
				}
			}
			else
			{
				this.generatedPower = 0f;
				this.throttlePosition = 0f;
			}
			return this.generatedPower * 1000f / ((angularVelocity == 0f) ? 0.1f : angularVelocity);
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x000BF6A0 File Offset: 0x000BD8A0
		public float CalculateTorqueICE(float angularVelocity, float dt)
		{
			this.throttlePosition = (this.starterActive ? 0f : this._initThrottlePosition);
			if (angularVelocity < this._idleAngularVelocity && angularVelocity > this._stallAngularVelocity)
			{
				float num = (angularVelocity - this._idleAngularVelocity) * dt * 550f;
				num = ((num < -0.4f) ? -0.4f : ((num > 0.4f) ? 0.4f : num));
				this.throttlePosition -= num;
				this.throttlePosition = ((this.throttlePosition < 0f) ? 0f : ((this.throttlePosition > 1f) ? 1f : this.throttlePosition));
			}
			if (angularVelocity >= 0f)
			{
				if (this._revLimiterStartTime < 0f)
				{
					this.revLimiterActive = (this.revLimiterEnabled && angularVelocity > this._revLimiterAngularVelocity);
					if (this.revLimiterActive)
					{
						this._revLimiterStartTime = this._realtimeSinceStartup + dt;
						this.OnRevLimiter.Invoke();
					}
				}
				else if (this._realtimeSinceStartup >= this._revLimiterStartTime + this.revLimiterCutoffDuration)
				{
					this.revLimiterActive = false;
					this._revLimiterStartTime = -999f;
				}
				if (this.revLimiterActive || this.shiftSignal)
				{
					this.generatedPower = 0f;
					this.throttlePosition = 0f;
				}
				this.isStalling = (this.stallingEnabled && angularVelocity < this._stallAngularVelocity);
				if (this.isStalling || this.revLimiterActive || !this.ignition)
				{
					this.generatedPower = 0f;
				}
				else
				{
					this.generatedPower = this.powerCurve.Evaluate(angularVelocity / this._maxAngularVelocity) * this.maxPower * this.throttlePosition * this.powerModifierSum * this.forcedInduction.PowerGainMultiplier;
				}
				float num2 = this.isStalling ? 0f : (this.generatedPower * 1000f / ((angularVelocity == 0f) ? 0.1f : angularVelocity));
				if (angularVelocity > this._idleAngularVelocity)
				{
					float rpmpercent = this.RPMPercent;
					this.lossTorque = this.maxLossTorque * (1f - this.throttlePosition) * rpmpercent;
				}
				else
				{
					this.lossTorque = 0f;
				}
				num2 -= this.lossTorque;
				if (this.starterActive && angularVelocity < this._starterMaxAngularVelocity)
				{
					num2 += this.starterTorque;
				}
				return num2;
			}
			if (this.starterActive)
			{
				this.angularVelocity = 0.001f;
				return this.starterTorque;
			}
			return 0f;
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x000BF914 File Offset: 0x000BDB14
		public float GetLoad()
		{
			float rpmpercent = this.RPMPercent;
			return Mathf.Clamp01(this.generatedPower / this.maxPower * 0.6f + rpmpercent * 0.4f);
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x000BF948 File Offset: 0x000BDB48
		public override void Integrate(float dt, int iterationCounter)
		{
			if (this._outputAIsNull)
			{
				return;
			}
			float inertia = this.inertia;
			float num = this.outputA.QueryInertia();
			float num2 = inertia + num;
			float angularVelocity = this.angularVelocity;
			float num3 = this.QueryAngularVelocity(this.angularVelocity, dt);
			float num4 = inertia / num2 * angularVelocity + num / num2 * num3;
			this.GeneratedTorque = this.calculateTorqueDelegate(angularVelocity, dt);
			float num5 = (num4 - angularVelocity) * inertia / (dt / this._fixedDeltaTime);
			float num6 = this.slipTorque * this.clutchEnagagement;
			num5 = ((num5 < -num6) ? (-num6) : ((num5 > num6) ? num6 : num5));
			float num7 = this.SendTorque(this.GeneratedTorque - num5, 0f, dt);
			float num8 = this.GeneratedTorque + num7 + num5;
			this.angularVelocity += num8 / num2 * dt;
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x000BFA1C File Offset: 0x000BDC1C
		public override void OnPreSolve()
		{
			base.OnPreSolve();
			if (base.ComponentDamage > 0.999f)
			{
				this.ignition = false;
			}
			this._fixedDeltaTime = Time.fixedDeltaTime;
			this._realtimeSinceStartup = Time.realtimeSinceStartup;
			this._isElectric = (this.engineType == EngineComponent.EngineType.Electric);
			this.maxRPM = this.revLimiterRPM * 1.2f;
			this._maxAngularVelocity = this.MaxAngularVelocity;
			this._minAngularVelocity = this.MinAngularVelocity;
			this._idleAngularVelocity = this.IdleAngularVelocity;
			this._stallAngularVelocity = this.StallAngularVelocity;
			this._revLimiterAngularVelocity = this.RevLimiterAngularVelocity;
			this._starterMaxAngularVelocity = this.StarterMaxAngularVelocity;
			this._lowerAngularVelocityLimit = this._minAngularVelocity;
			this._upperAngularVelocityLimit = this._maxAngularVelocity;
			if (!this._isElectric)
			{
				if (!this.IsRunning && !this.starterActive && this.autoStartOnThrottle && this.throttlePosition > 0.5f && base.ComponentDamage < 0.999f)
				{
					this.Start();
				}
				if (this.starterActive)
				{
					this._starterTimer += this._fixedDeltaTime;
					if (this._starterTimer > this.starterRunTime)
					{
						this.starterActive = false;
						this._starterTimer = 0f;
					}
				}
			}
			else
			{
				this._starterTimer = 0f;
				this.starterActive = false;
				this.revLimiterCutoffDuration = 0f;
			}
			if (this.shiftSignal)
			{
				this.throttlePosition = 0f;
			}
			if (base.ComponentDamage > 0.999f || !this.ignition)
			{
				this.throttlePosition = 0f;
			}
			this.powerModifierSum = this.SumPowerModifiers();
			this._initThrottlePosition = this.throttlePosition;
			if (!this._isElectric)
			{
				this.forcedInduction.Update(this);
			}
			this._initThrottlePosition = this.throttlePosition;
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x000BFBDF File Offset: 0x000BDDDF
		public void Stop()
		{
			if (this.IsRunning)
			{
				this.ignition = false;
				this.OnStop.Invoke();
				this.angularVelocity = 0f;
			}
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x000BEC9F File Offset: 0x000BCE9F
		public override void Validate(VehicleController vc)
		{
			base.Validate(vc);
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x000BFC08 File Offset: 0x000BDE08
		private float SumPowerModifiers()
		{
			if (this.powerModifiers.Count == 0)
			{
				return 1f;
			}
			float num = 1f;
			int count = this.powerModifiers.Count;
			for (int i = 0; i < count; i++)
			{
				num *= this.powerModifiers[i]();
			}
			return Mathf.Clamp(num, 0f, float.PositiveInfinity);
		}

		// Token: 0x04002139 RID: 8505
		[Tooltip("    If true starter will be ran for [starterRunTime] seconds if engine receives any throttle input.")]
		public bool autoStartOnThrottle = true;

		// Token: 0x0400213A RID: 8506
		[Tooltip("    Assign your own delegate to use different type of torque calculation.")]
		public EngineComponent.CalculateTorque calculateTorqueDelegate;

		// Token: 0x0400213B RID: 8507
		[Tooltip("    Current engagement of the clutch.")]
		public float clutchEnagagement = 1f;

		// Token: 0x0400213C RID: 8508
		[Tooltip("Engine layout / orientation. Either longitudinal or lateral. Will change\r\naxis of torque applied to chassis. Also check chassisTorqueMultiplier.")]
		public EngineComponent.EngineLayout engineLayout;

		// Token: 0x0400213D RID: 8509
		[Tooltip("Engine type. ICE (Internal Combustion Engine) supports features such as starter, stalling, etc.\r\nElectric engine (motor) can run in reverse, can not be stalled and does not use starter.")]
		public EngineComponent.EngineType engineType;

		// Token: 0x0400213E RID: 8510
		[Tooltip("    Turbocharger or supercharger.")]
		public EngineComponent.ForcedInduction forcedInduction;

		// Token: 0x0400213F RID: 8511
		[ShowInTelemetry]
		[Tooltip("    Power generated by the engine in kW")]
		public float generatedPower;

		// Token: 0x04002140 RID: 8512
		[SerializeField]
		[Tooltip("    RPM at which idler circuit will try to keep RPMs when there is no input.")]
		public float idleRPM = 900f;

		// Token: 0x04002141 RID: 8513
		[Tooltip("    Is the ignition on? If off engine will refuse to start and run.")]
		public bool ignition = true;

		// Token: 0x04002142 RID: 8514
		[Tooltip("    Is the engine stalling? Stalling will happen if engine RPM is lower than stall RPM.")]
		public bool isStalling;

		// Token: 0x04002143 RID: 8515
		[Tooltip("    Current loss torque. Pumping losses, friction, etc.")]
		public float lossTorque;

		// Token: 0x04002144 RID: 8516
		[Tooltip("    Torque representing losses in the engine.\r\n    Higher value will result in engine slowing down faster.")]
		public float maxLossTorque = 300f;

		// Token: 0x04002145 RID: 8517
		[ShowInSettings("Max. Power", 20f, 800f, 40f)]
		[Tooltip("    Maximum engine power in [kW].")]
		public float maxPower = 120f;

		// Token: 0x04002146 RID: 8518
		[Tooltip("Maximum engine RPM. This is a hard limit set in solver that the engine will never be able to go over.\r\nDiffers from rev limiter RPM and should always be higher than rev limiter.")]
		public float maxRPM = 5000f;

		// Token: 0x04002147 RID: 8519
		[Tooltip("Minimum engine RPM. Hard limit set in solver that the engine will never go under.\r\nIt is recommended to set it to a negative value as otherwise the engine will always have RPM >0.")]
		public float minRPM = -50f;

		// Token: 0x04002148 RID: 8520
		[Tooltip("Called when engine hits rev limiter.")]
		public UnityEvent OnRevLimiter = new UnityEvent();

		// Token: 0x04002149 RID: 8521
		[Tooltip("Called when engine is started.")]
		public UnityEvent OnStart = new UnityEvent();

		// Token: 0x0400214A RID: 8522
		[Tooltip("Called when engine is stopped.")]
		public UnityEvent OnStop = new UnityEvent();

		// Token: 0x0400214B RID: 8523
		[Tooltip("If true the engine will be started immediately, without running the starter, when the vehicle is enabled.\r\nSets engine angular velocity to idle angular velocity.")]
		public bool flyingStartEnabled;

		// Token: 0x0400214C RID: 8524
		[Tooltip("Power curve with RPM range [0,1] on the X axis and power coefficient [0,1] on Y axis.\r\nBoth values are represented as percentages and should be in 0 to 1 range.\r\nPower coefficient is multiplied by maxPower to get the final power at given RPM.")]
		public AnimationCurve powerCurve;

		// Token: 0x0400214D RID: 8525
		[Tooltip("List of callbacks that influence engine power. Examples would be traction control which\r\nreduces power (returns less than 1) or forced induction which increases power (returns more than 1).\r\nCan also be used by modules to reduce engine power in certain situations.\r\nFinal power modifier value is calculated by multiplying return values of all callbacks.")]
		public List<EngineComponent.PowerModifier> powerModifiers = new List<EngineComponent.PowerModifier>();

		// Token: 0x0400214E RID: 8526
		[Tooltip("    Is the engine currently hitting the rev limiter?")]
		public bool revLimiterActive;

		// Token: 0x0400214F RID: 8527
		[Tooltip("If engine RPM rises above revLimiterRPM, how long should fuel cutoff last?\r\nHigher values make hitting rev limiter more rough and choppy.")]
		public float revLimiterCutoffDuration = 0.02f;

		// Token: 0x04002150 RID: 8528
		[Tooltip("Should engine use rev limiter? If disabled engine will be able to rotate up to maxRPM which is\r\nthe hard limit for the solver.")]
		public bool revLimiterEnabled = true;

		// Token: 0x04002151 RID: 8529
		[Tooltip("    Engine RPM at which rev limiter activates.")]
		public float revLimiterRPM = 4700f;

		// Token: 0x04002152 RID: 8530
		[Tooltip("    Is the transmission shifting? If set to true engine will cut throttle.")]
		public bool shiftSignal;

		// Token: 0x04002153 RID: 8531
		public float slipTorque = 9999f;

		// Token: 0x04002154 RID: 8532
		[Tooltip("Can the vehicle be stalled?\r\nIf disabled engine will run no matter the RPM. Automatically disabled when electric engine type is used.")]
		public bool stallingEnabled = true;

		// Token: 0x04002155 RID: 8533
		[Tooltip("    Engine RPM under which the engine stalls.")]
		public float stallRPM = 300f;

		// Token: 0x04002156 RID: 8534
		[Tooltip("    Is the starter currently active?")]
		public bool starterActive;

		// Token: 0x04002157 RID: 8535
		[Tooltip("Maximum RPM the starter motor alone can achieve when spinning up the engine.\r\nIf set too low the engine may fail to start (depending on stall RPM and loss torque).")]
		public float starterRPMLimit = 600f;

		// Token: 0x04002158 RID: 8536
		[Tooltip("    How long will the starter run after it is triggered.")]
		public float starterRunTime = 1f;

		// Token: 0x04002159 RID: 8537
		[Tooltip("Torque starter motor can put out. Make sure that this torque is more than loss torque\r\nat the starter RPM limit. If too low the engine will fail to start.")]
		public float starterTorque = 60f;

		// Token: 0x0400215A RID: 8538
		public float test123;

		// Token: 0x0400215B RID: 8539
		[ShowInTelemetry]
		[Tooltip("    Current throttle position in range [0,1]")]
		public float throttlePosition = 1f;

		// Token: 0x0400215C RID: 8540
		private float _fixedDeltaTime;

		// Token: 0x0400215D RID: 8541
		private float _idleAngularVelocity;

		// Token: 0x0400215E RID: 8542
		private float _initThrottlePosition;

		// Token: 0x0400215F RID: 8543
		private bool _isElectric;

		// Token: 0x04002160 RID: 8544
		private float _maxAngularVelocity;

		// Token: 0x04002161 RID: 8545
		private float _minAngularVelocity;

		// Token: 0x04002162 RID: 8546
		private float _realtimeSinceStartup;

		// Token: 0x04002163 RID: 8547
		private float _revLimiterAngularVelocity;

		// Token: 0x04002164 RID: 8548
		private float _revLimiterStartTime;

		// Token: 0x04002165 RID: 8549
		private float _stallAngularVelocity;

		// Token: 0x04002166 RID: 8550
		private float _starterMaxAngularVelocity;

		// Token: 0x04002167 RID: 8551
		private float _starterTimer;

		// Token: 0x04002168 RID: 8552
		private bool _wasRunning;

		// Token: 0x04002169 RID: 8553
		[SerializeField]
		private float powerModifierSum;

		// Token: 0x020004B1 RID: 1201
		[Serializable]
		public class ForcedInduction
		{
			// Token: 0x1700039E RID: 926
			// (get) Token: 0x06001B09 RID: 6921 RVA: 0x000F83A5 File Offset: 0x000F65A5
			public float PowerGainMultiplier
			{
				get
				{
					if (!this.useForcedInduction)
					{
						return 1f;
					}
					return this.RPM / this.maxRPM * this.powerGainMultiplier;
				}
			}

			// Token: 0x06001B0A RID: 6922 RVA: 0x000F83CC File Offset: 0x000F65CC
			public void Update(EngineComponent engine)
			{
				if (!this.useForcedInduction)
				{
					return;
				}
				float f = engine.throttlePosition * engine.RPMPercent;
				float num = this.maxRPM * Mathf.Pow(f, 2f - this.linearity);
				if (this.forcedInductionType == EngineComponent.ForcedInduction.ForcedInductionType.Turbocharger)
				{
					if (this.hasWastegate && engine.throttlePosition < 0.5f && this.boost > 0.3f)
					{
						this.wastegateFlag = true;
						this.wastegateBoost = this.boost;
						this.RPM = 0f;
					}
					else
					{
						this.RPM = Mathf.SmoothDamp(this.RPM, num, ref this.spoolVelocity, this.spoolUpTime);
					}
				}
				else
				{
					this.RPM = num;
				}
				this.RPM = ((this.RPM > this.maxRPM) ? this.maxRPM : ((this.RPM < 0f) ? 0f : this.RPM));
				this.boost = this.RPM * this.RPM / (this.maxRPM * this.maxRPM);
			}

			// Token: 0x04002BE2 RID: 11234
			[ShowInTelemetry]
			[Range(0f, 1f)]
			[Tooltip("    Boost value as percentage in 0 to 1 range. Unitless.\r\n    Can be used for boost gauges.")]
			public float boost;

			// Token: 0x04002BE3 RID: 11235
			[ShowInTelemetry]
			[ShowInSettings("Forced Induction Type")]
			[Tooltip("    Type of forced induction.")]
			public EngineComponent.ForcedInduction.ForcedInductionType forcedInductionType;

			// Token: 0x04002BE4 RID: 11236
			[Tooltip("Imitates wastegate in a turbo setup.\r\nEnable if you want turbo flutter sound effects and/or boost to drop off faster after closing throttle.\r\nNot used with superchargers.")]
			public bool hasWastegate = true;

			// Token: 0x04002BE5 RID: 11237
			[Range(0f, 1f)]
			[Tooltip("Use values towards 0 for large turbos, and near 1 for small turbos.\r\nLower linearity values will need higher engine RPM to spool up.\r\nForced to 1 for superchargers")]
			public float linearity = 0.7f;

			// Token: 0x04002BE6 RID: 11238
			[Range(0f, 2f)]
			[Tooltip("Additional power that will be added to the engine's power.\r\nThis is the maximum value possible and depends on spool percent.")]
			[ShowInSettings("Power Gain", 1f, 3f, 0.2f)]
			public float powerGainMultiplier = 1.4f;

			// Token: 0x04002BE7 RID: 11239
			[Range(0.1f, 2f)]
			[ShowInSettings("Spool Up Time", 0f, 2f, 0.2f)]
			[Tooltip("Shortest time possible needed for turbo to spool up to its maximum RPM.\r\nUse larger values for larger turbos and vice versa.\r\nForced to 0 for superchargers.")]
			public float spoolUpTime = 0.12f;

			// Token: 0x04002BE8 RID: 11240
			[Tooltip("    Should forced induction be used?")]
			public bool useForcedInduction = true;

			// Token: 0x04002BE9 RID: 11241
			[Tooltip("    Cached boost value at the time of wastegate releasing pressure for sound effects.")]
			public float wastegateBoost;

			// Token: 0x04002BEA RID: 11242
			[Tooltip("    Flag for sound effects.")]
			public bool wastegateFlag;

			// Token: 0x04002BEB RID: 11243
			private float maxRPM = 120000f;

			// Token: 0x04002BEC RID: 11244
			private float RPM;

			// Token: 0x04002BED RID: 11245
			private float spoolVelocity;

			// Token: 0x02000504 RID: 1284
			public enum ForcedInductionType
			{
				// Token: 0x04002CFA RID: 11514
				Turbocharger,
				// Token: 0x04002CFB RID: 11515
				Supercharger
			}
		}

		// Token: 0x020004B2 RID: 1202
		// (Invoke) Token: 0x06001B0D RID: 6925
		public delegate float CalculateTorque(float angularVelocity, float dt);

		// Token: 0x020004B3 RID: 1203
		// (Invoke) Token: 0x06001B11 RID: 6929
		public delegate float PowerModifier();

		// Token: 0x020004B4 RID: 1204
		public enum EngineLayout
		{
			// Token: 0x04002BEF RID: 11247
			Longitudinal,
			// Token: 0x04002BF0 RID: 11248
			Transverse
		}

		// Token: 0x020004B5 RID: 1205
		public enum EngineType
		{
			// Token: 0x04002BF2 RID: 11250
			ICE,
			// Token: 0x04002BF3 RID: 11251
			Electric
		}
	}
}
