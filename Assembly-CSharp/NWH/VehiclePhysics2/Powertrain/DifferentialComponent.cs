using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
	// Token: 0x0200027C RID: 636
	[Serializable]
	public class DifferentialComponent : PowertrainComponent
	{
		// Token: 0x060010C1 RID: 4289 RVA: 0x000BED3C File Offset: 0x000BCF3C
		public DifferentialComponent()
		{
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x000BED9C File Offset: 0x000BCF9C
		public DifferentialComponent(string name, PowertrainComponent outputA, PowertrainComponent outputB)
		{
			this.name = name;
			this.outputA = outputA;
			this.outputB = outputB;
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x000BEE14 File Offset: 0x000BD014
		public override void Initialize()
		{
			this.cOpenTorqueSplitDelegate = new DifferentialComponent.SplitTorque(this.OpenDiffTorqueSplit);
			this.cLockingTorqueSplitDelegate = new DifferentialComponent.SplitTorque(this.LockingDiffTorqueSplit);
			this.cVLSDTorqueSplitDelegate = new DifferentialComponent.SplitTorque(this.VLSDTorqueSplit);
			this.cHLSDTorqueSplitDelegate = new DifferentialComponent.SplitTorque(this.HLSDTorqueSplit);
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x000BEE6C File Offset: 0x000BD06C
		public override void FindOutputs(Solver solver)
		{
			if (string.IsNullOrEmpty(this.outputASelector.name) || string.IsNullOrEmpty(this.outputBSelector.name))
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
			PowertrainComponent component2 = solver.GetComponent(this.outputBSelector.name);
			if (component2 == null)
			{
				Debug.LogError("Unknown component '" + this.outputBSelector.name + "'");
				return;
			}
			this.outputB = component2;
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x000BEF16 File Offset: 0x000BD116
		public override void GetAllOutputs(ref List<PowertrainComponent> outputs)
		{
			outputs.Clear();
			outputs.Add(this.outputA);
			outputs.Add(this.outputB);
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x000BEF3C File Offset: 0x000BD13C
		public void HLSDTorqueSplit(float T, float Wa, float Wb, float Ia, float Ib, float dt, float biasAB, float preload, float stiffness, float powerRamp, float coastRamp, float slipTorque, out float Ta, out float Tb)
		{
			if (Wa < 0f || Wb < 0f)
			{
				Ta = T * (1f - biasAB);
				Tb = T * biasAB;
				return;
			}
			float num = (T > 0f) ? powerRamp : coastRamp;
			float num2 = ((T < 0f) ? (-T) : T) / slipTorque;
			num2 = ((num2 < -1f) ? -1f : ((num2 > 1f) ? 1f : num2));
			float num3 = ((Wa < 0f) ? (-Wa) : Wa) + ((Wb < 0f) ? (-Wb) : Wb);
			float num4 = ((Wa == 0f || Wb == 0f) ? 0f : ((Wa - Wb) / num3)) * stiffness * num * slipTorque * num2;
			Ta = T * (1f - biasAB) - num4;
			Tb = T * biasAB + num4;
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x000BF00C File Offset: 0x000BD20C
		public void VLSDTorqueSplit(float T, float Wa, float Wb, float Ia, float Ib, float dt, float biasAB, float preload, float stiffness, float powerRamp, float coastRamp, float slipTorque, out float Ta, out float Tb)
		{
			if (Wa < 0f || Wb < 0f)
			{
				Ta = T * (1f - biasAB);
				Tb = T * biasAB;
				return;
			}
			float num = (T > 0f) ? powerRamp : coastRamp;
			float num2 = ((Wa < 0f) ? (-Wa) : Wa) + ((Wb < 0f) ? (-Wb) : Wb);
			float num3 = ((Wa == 0f || Wb == 0f) ? 0f : ((Wa - Wb) / num2)) * stiffness * num * slipTorque;
			Ta = T * (1f - biasAB) - num3;
			Tb = T * biasAB + num3;
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x000BF0A8 File Offset: 0x000BD2A8
		public void LockingDiffTorqueSplit(float T, float Wa, float Wb, float Ia, float Ib, float dt, float biasAB, float preload, float stiffness, float powerRamp, float coastRamp, float slipTorque, out float Ta, out float Tb)
		{
			float num = Ia + Ib;
			float num2 = Ia / num * Wa + Ib / num * Wb;
			float num3 = (num2 - Wa) * Ia / dt;
			num3 *= stiffness;
			float num4 = (num2 - Wb) * Ib / dt;
			num4 *= stiffness;
			float num5 = 0.5f + (Wb - Wa) * 0.1f * stiffness;
			num5 = ((num5 < 0f) ? 0f : ((num5 > 1f) ? 1f : num5));
			Ta = T * num5 + num3;
			Tb = T * (1f - num5) + num4;
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x000BF130 File Offset: 0x000BD330
		public override void OnPreSolve()
		{
			base.OnPreSolve();
			this._outputBIsNull = (this.outputB == null);
			if (this.differentialType == DifferentialComponent.Type.Open)
			{
				this.splitTorqueDelegate = this.cOpenTorqueSplitDelegate;
			}
			else if (this.differentialType == DifferentialComponent.Type.Locked)
			{
				this.splitTorqueDelegate = this.cLockingTorqueSplitDelegate;
			}
			else if (this.differentialType == DifferentialComponent.Type.ViscousLSD)
			{
				this.splitTorqueDelegate = this.cVLSDTorqueSplitDelegate;
			}
			else if (this.differentialType == DifferentialComponent.Type.ClutchLSD)
			{
				this.splitTorqueDelegate = this.cHLSDTorqueSplitDelegate;
			}
			if (this.splitTorqueDelegate == null)
			{
				this.differentialType = DifferentialComponent.Type.Open;
				this.splitTorqueDelegate = this.cOpenTorqueSplitDelegate;
			}
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x000BF1C6 File Offset: 0x000BD3C6
		public void OpenDiffTorqueSplit(float T, float Wa, float Wb, float Ia, float Ib, float dt, float biasAB, float preload, float stiffness, float powerRamp, float coastRamp, float slipTorque, out float Ta, out float Tb)
		{
			Ta = T * (1f - biasAB);
			Tb = T * biasAB;
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x000BF1DC File Offset: 0x000BD3DC
		public override float QueryAngularVelocity(float inputAngularVelocity, float dt)
		{
			this.angularVelocity = inputAngularVelocity;
			if (this._outputAIsNull || this._outputBIsNull)
			{
				return inputAngularVelocity;
			}
			float num = this.outputA.QueryAngularVelocity(inputAngularVelocity, dt);
			float num2 = this.outputB.QueryAngularVelocity(inputAngularVelocity, dt);
			return (num + num2) * 0.5f;
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x000BF228 File Offset: 0x000BD428
		public override float QueryInertia()
		{
			if (this._outputAIsNull || this._outputBIsNull)
			{
				return this.inertia;
			}
			float num = this.outputA.QueryInertia();
			float num2 = this.outputB.QueryInertia();
			return this.inertia + (num + num2);
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x000BF270 File Offset: 0x000BD470
		public override float SendTorque(float torque, float inertiaSum, float dt)
		{
			if (this._outputAIsNull || this._outputBIsNull)
			{
				return torque;
			}
			float wa = this.outputA.QueryAngularVelocity(this.angularVelocity, dt);
			float wb = this.outputB.QueryAngularVelocity(this.angularVelocity, dt);
			float num = this.outputA.QueryInertia();
			float num2 = this.outputB.QueryInertia();
			float torque2;
			float torque3;
			this.splitTorqueDelegate(torque, wa, wb, num, num2, dt, this.biasAB, this.preload, this.stiffness, this.powerRamp, this.coastRamp, this.slipTorque, out torque2, out torque3);
			return this.outputA.SendTorque(torque2, inertiaSum + num, dt) + this.outputB.SendTorque(torque3, inertiaSum + num2, dt);
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x000BF329 File Offset: 0x000BD529
		public void SetOutput(PowertrainComponent outputAComponent, PowertrainComponent outputBComponent)
		{
			if (string.IsNullOrEmpty(outputAComponent.name) || string.IsNullOrEmpty(outputBComponent.name))
			{
				Debug.LogWarning("Trying to set powertrain component output to a nameless component. Output will be set to [none]");
			}
			this.SetOutput(outputAComponent.name, outputBComponent.name);
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x000BF364 File Offset: 0x000BD564
		public void SetOutput(string outputAName, string outputBName)
		{
			if (string.IsNullOrEmpty(outputAName))
			{
				this.outputASelector.name = "[none]";
			}
			else
			{
				this.outputASelector.name = outputAName;
			}
			if (string.IsNullOrEmpty(outputBName))
			{
				this.outputBSelector.name = "[none]";
				return;
			}
			this.outputBSelector.name = outputBName;
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x000BEC9F File Offset: 0x000BCE9F
		public override void Validate(VehicleController vc)
		{
			base.Validate(vc);
		}

		// Token: 0x0400212A RID: 8490
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("    Torque bias between left (A) and right (B) output in [0,1] range.")]
		public float biasAB = 0.5f;

		// Token: 0x0400212B RID: 8491
		[Range(0f, 1f)]
		public float coastRamp = 0.5f;

		// Token: 0x0400212C RID: 8492
		[ShowInSettings("Differential Type")]
		[Tooltip("    Differential type.")]
		public DifferentialComponent.Type differentialType;

		// Token: 0x0400212D RID: 8493
		[Tooltip("    Second output of differential.")]
		public PowertrainComponent outputB;

		// Token: 0x0400212E RID: 8494
		[SerializeField]
		[Range(0f, 1f)]
		public float powerRamp = 1f;

		// Token: 0x0400212F RID: 8495
		[SerializeField]
		public float preload = 10f;

		// Token: 0x04002130 RID: 8496
		[SerializeField]
		[Tooltip("    Slip torque of limited slip differentials.")]
		public float slipTorque = 10000f;

		// Token: 0x04002131 RID: 8497
		public DifferentialComponent.SplitTorque splitTorqueDelegate;

		// Token: 0x04002132 RID: 8498
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Stiffness of locking differential [0,1]. Higher value\r\nwill result in lower difference in rotational velocity between left and right wheel.\r\nToo high value might introduce slight oscillation due to drivetrain windup.")]
		public float stiffness = 0.1f;

		// Token: 0x04002133 RID: 8499
		[SerializeField]
		protected OutputSelector outputBSelector = new OutputSelector();

		// Token: 0x04002134 RID: 8500
		private bool _outputBIsNull;

		// Token: 0x04002135 RID: 8501
		private DifferentialComponent.SplitTorque cHLSDTorqueSplitDelegate;

		// Token: 0x04002136 RID: 8502
		private DifferentialComponent.SplitTorque cLockingTorqueSplitDelegate;

		// Token: 0x04002137 RID: 8503
		private DifferentialComponent.SplitTorque cOpenTorqueSplitDelegate;

		// Token: 0x04002138 RID: 8504
		private DifferentialComponent.SplitTorque cVLSDTorqueSplitDelegate;

		// Token: 0x020004AF RID: 1199
		// (Invoke) Token: 0x06001B06 RID: 6918
		public delegate void SplitTorque(float T, float Wa, float Wb, float Ia, float Ib, float dt, float biasAB, float preload, float stiffness, float powerRamp, float coastRamp, float slipTorque, out float Ta, out float Tb);

		// Token: 0x020004B0 RID: 1200
		public enum Type
		{
			// Token: 0x04002BDD RID: 11229
			Open,
			// Token: 0x04002BDE RID: 11230
			Locked,
			// Token: 0x04002BDF RID: 11231
			ViscousLSD,
			// Token: 0x04002BE0 RID: 11232
			ClutchLSD,
			// Token: 0x04002BE1 RID: 11233
			External
		}
	}
}
