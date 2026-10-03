using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Utility;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
	// Token: 0x0200027E RID: 638
	[Serializable]
	public class PowertrainComponent
	{
		// Token: 0x060010ED RID: 4333 RVA: 0x000BFD61 File Offset: 0x000BDF61
		public PowertrainComponent()
		{
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x000BFD98 File Offset: 0x000BDF98
		public PowertrainComponent(float inertia, string name)
		{
			this.name = name;
			this.inertia = inertia;
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x060010EF RID: 4335 RVA: 0x000BFDE5 File Offset: 0x000BDFE5
		// (set) Token: 0x060010F0 RID: 4336 RVA: 0x000BFDED File Offset: 0x000BDFED
		public float ComponentDamage
		{
			get
			{
				return this._componentDamage;
			}
			set
			{
				this._componentDamage = ((value > 1f) ? 1f : ((value < 0f) ? 0f : value));
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x060010F1 RID: 4337 RVA: 0x000BFE14 File Offset: 0x000BE014
		// (set) Token: 0x060010F2 RID: 4338 RVA: 0x000BFE1C File Offset: 0x000BE01C
		public float LowerAngularVelocityLimit
		{
			get
			{
				return this._lowerAngularVelocityLimit;
			}
			set
			{
				this._lowerAngularVelocityLimit = value;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x060010F3 RID: 4339 RVA: 0x000BFE25 File Offset: 0x000BE025
		[ShowInTelemetry]
		public float RPM
		{
			get
			{
				return UnitConverter.AngularVelocityToRPM(this.angularVelocity);
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x060010F4 RID: 4340 RVA: 0x000BFE32 File Offset: 0x000BE032
		// (set) Token: 0x060010F5 RID: 4341 RVA: 0x000BFE3A File Offset: 0x000BE03A
		public float UpperAngularVelocityLimit
		{
			get
			{
				return this._upperAngularVelocityLimit;
			}
			set
			{
				this._upperAngularVelocityLimit = value;
			}
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x000BFE43 File Offset: 0x000BE043
		public virtual void Initialize()
		{
			if (this.inertia < 0.01f)
			{
				this.inertia = 0.01f;
			}
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x00002188 File Offset: 0x00000388
		public virtual void OnEnable()
		{
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x00002188 File Offset: 0x00000388
		public virtual void OnDisable()
		{
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x000BFE60 File Offset: 0x000BE060
		public virtual void FindInput(Solver solver)
		{
			List<PowertrainComponent> list = new List<PowertrainComponent>();
			foreach (PowertrainComponent powertrainComponent in solver.Components)
			{
				powertrainComponent.GetAllOutputs(ref list);
				foreach (PowertrainComponent powertrainComponent2 in list)
				{
					if (powertrainComponent2 != null && powertrainComponent2 == this)
					{
						this.input = powertrainComponent;
						this._inputIsNull = false;
						return;
					}
				}
			}
			this.input = null;
			this._inputIsNull = true;
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x000BFF18 File Offset: 0x000BE118
		public virtual void FindOutputs(Solver solver)
		{
			if (string.IsNullOrEmpty(this.outputASelector.name))
			{
				return;
			}
			PowertrainComponent component = solver.GetComponent(this.outputASelector.name);
			if (component == null)
			{
				Debug.LogError("Unknown component '" + this.outputASelector.name + "'");
				return;
			}
			this.outputA = component;
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x000BFF74 File Offset: 0x000BE174
		public virtual void GetAllOutputs(ref List<PowertrainComponent> outputs)
		{
			outputs.Clear();
			outputs.Add(this.outputA);
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x00002188 File Offset: 0x00000388
		public virtual void Integrate(float dt, int iterationCounter)
		{
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x000BFF8C File Offset: 0x000BE18C
		public virtual void OnPostSolve()
		{
			this.angularVelocity = ((this.angularVelocity < this._lowerAngularVelocityLimit) ? this._lowerAngularVelocityLimit : this.angularVelocity);
			this.angularVelocity = ((this.angularVelocity > this._upperAngularVelocityLimit) ? this._upperAngularVelocityLimit : this.angularVelocity);
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x000BFFDD File Offset: 0x000BE1DD
		public virtual void OnPreSolve()
		{
			this._inputIsNull = (this.input == null);
			this._outputAIsNull = (this.outputA == null);
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x000BFFFD File Offset: 0x000BE1FD
		public virtual float QueryAngularVelocity(float inputAngularVelocity, float dt)
		{
			this.angularVelocity = inputAngularVelocity;
			if (this._outputAIsNull)
			{
				return 0f;
			}
			return this.outputA.QueryAngularVelocity(inputAngularVelocity, dt);
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x000C0024 File Offset: 0x000BE224
		public virtual float QueryInertia()
		{
			if (this._outputAIsNull)
			{
				return this.inertia;
			}
			float num = this.inertia;
			float num2 = this.outputA.QueryInertia();
			return num + num2;
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x000C0054 File Offset: 0x000BE254
		public virtual float SendTorque(float torque, float inertiaSum, float dt)
		{
			if (this._outputAIsNull)
			{
				return torque;
			}
			return this.outputA.SendTorque(torque, inertiaSum + this.inertia, dt);
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x000C0075 File Offset: 0x000BE275
		public void SetOutput(PowertrainComponent outputComponent)
		{
			if (string.IsNullOrEmpty(outputComponent.name))
			{
				Debug.LogWarning("Trying to set powertrain component output to a nameless component. Output will be set to [none]");
			}
			this.SetOutput(outputComponent.name);
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x000C009A File Offset: 0x000BE29A
		public void SetOutput(string outputName)
		{
			if (string.IsNullOrEmpty(outputName))
			{
				this.outputASelector.name = "[none]";
				return;
			}
			this.outputASelector.name = outputName;
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x000C00C1 File Offset: 0x000BE2C1
		public virtual void Validate(VehicleController vc)
		{
			if (this.inertia < 0.01f)
			{
				this.inertia = 0.01f;
				Debug.LogWarning(this.name + ": Inertia must be larger than 0.01f. Setting to 0.01f.");
			}
		}

		// Token: 0x0400216B RID: 8555
		[Tooltip("    Angular velocity of the component.")]
		public float angularVelocity;

		// Token: 0x0400216C RID: 8556
		[Range(0.01f, 1f)]
		[Tooltip("Angular inertia of the component. Higher inertia value will result in a powertrain that is slower to spin up, but\r\nalso slower to spin down. Too high values will result in (apparent) sluggish response while too low values will\r\nresult in vehicle being easy to stall.")]
		public float inertia = 0.02f;

		// Token: 0x0400216D RID: 8557
		[Tooltip("    Input component. Set automatically.")]
		public PowertrainComponent input;

		// Token: 0x0400216E RID: 8558
		[Tooltip("    Name of the component. Only unique names should be used on the same vehicle.")]
		public string name;

		// Token: 0x0400216F RID: 8559
		[Tooltip("    Output component.")]
		public PowertrainComponent outputA;

		// Token: 0x04002170 RID: 8560
		protected bool _inputIsNull;

		// Token: 0x04002171 RID: 8561
		protected float _lowerAngularVelocityLimit = float.NegativeInfinity;

		// Token: 0x04002172 RID: 8562
		protected bool _outputAIsNull;

		// Token: 0x04002173 RID: 8563
		protected float _upperAngularVelocityLimit = float.PositiveInfinity;

		// Token: 0x04002174 RID: 8564
		[SerializeField]
		protected OutputSelector outputASelector = new OutputSelector();

		// Token: 0x04002175 RID: 8565
		private float _componentDamage;
	}
}
