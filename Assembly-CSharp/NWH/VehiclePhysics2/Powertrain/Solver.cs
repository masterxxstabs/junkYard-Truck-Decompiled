using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
	// Token: 0x02000282 RID: 642
	[Serializable]
	public class Solver
	{
		// Token: 0x170001BA RID: 442
		// (get) Token: 0x0600115C RID: 4444 RVA: 0x000C15FB File Offset: 0x000BF7FB
		public List<PowertrainComponent> Components
		{
			get
			{
				return this._components;
			}
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x000C1604 File Offset: 0x000BF804
		public void Initialize(List<PowertrainComponent> powertrainComponents)
		{
			this._components = powertrainComponents;
			foreach (PowertrainComponent powertrainComponent in this._components)
			{
				powertrainComponent.Initialize();
			}
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x000C165C File Offset: 0x000BF85C
		public void AddComponent(PowertrainComponent i)
		{
			this._components.Add(i);
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x000C166A File Offset: 0x000BF86A
		public PowertrainComponent GetComponent(int index)
		{
			if (index < 0 || index >= this._components.Count)
			{
				Debug.LogError("Component index out of bounds.");
				return null;
			}
			return this._components[index];
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x000C1698 File Offset: 0x000BF898
		public PowertrainComponent GetComponent(string name)
		{
			return this._components.FirstOrDefault((PowertrainComponent c) => c.name == name);
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x000C16C9 File Offset: 0x000BF8C9
		public List<string> GetComponentNames()
		{
			return (from c in this._components
			select c.name).ToList<string>();
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x000C16FA File Offset: 0x000BF8FA
		public void RemoveComponent(PowertrainComponent i)
		{
			this._components.Remove(i);
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x000C170C File Offset: 0x000BF90C
		public void Solve()
		{
			int count = this._components.Count;
			if (count == 0)
			{
				return;
			}
			this._iterations = (float)this.physicsQuality;
			this._dt = Time.fixedDeltaTime * (1f / this._iterations);
			for (int i = 0; i < count; i++)
			{
				this._components[i].OnPreSolve();
			}
			EngineComponent engineComponent = this._components[0] as EngineComponent;
			ClutchComponent clutchComponent = this._components[1] as ClutchComponent;
			int num = 0;
			while ((float)num < this._iterations)
			{
				engineComponent.clutchEnagagement = clutchComponent.clutchEngagement;
				engineComponent.Integrate(this._dt, num);
				num++;
			}
			for (int j = 0; j < count; j++)
			{
				this._components[j].OnPostSolve();
			}
		}

		// Token: 0x040021AF RID: 8623
		[Tooltip("Number of iterations that will be run in one FixedUpdate.\r\nHigher number of iterations will equal higher powertrain physics quality, but will also result in linear\r\ndecrease of performance. Values less than 8 (Low) are not recommended and might result in calculation instability.")]
		public Solver.PhysicsQuality physicsQuality = Solver.PhysicsQuality.High;

		// Token: 0x040021B0 RID: 8624
		[SerializeField]
		private List<PowertrainComponent> _components = new List<PowertrainComponent>();

		// Token: 0x040021B1 RID: 8625
		private float _dt;

		// Token: 0x040021B2 RID: 8626
		private float _iterations = 16f;

		// Token: 0x020004BB RID: 1211
		public enum PhysicsQuality
		{
			// Token: 0x04002BFE RID: 11262
			Low = 8,
			// Token: 0x04002BFF RID: 11263
			Medium = 12,
			// Token: 0x04002C00 RID: 11264
			High = 16,
			// Token: 0x04002C01 RID: 11265
			VeryHigh = 24,
			// Token: 0x04002C02 RID: 11266
			Ultra = 32,
			// Token: 0x04002C03 RID: 11267
			Overkill = 48
		}
	}
}
