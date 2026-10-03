using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Utility
{
	// Token: 0x02000263 RID: 611
	public class PIDController
	{
		// Token: 0x06001004 RID: 4100 RVA: 0x000BA0F2 File Offset: 0x000B82F2
		public PIDController(float gainProportional, float gainIntegral, float gainDerivative, float outputMin, float outputMax)
		{
			this.GainDerivative = gainDerivative;
			this.GainIntegral = gainIntegral;
			this.GainProportional = gainProportional;
			this.maxValue = outputMax;
			this.minValue = outputMin;
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x000BA120 File Offset: 0x000B8320
		public float ControlVariable(float timeSinceLastUpdate)
		{
			float num = this.SetPoint - this.ProcessVariable;
			this.IntegralTerm += this.GainIntegral * num * timeSinceLastUpdate;
			this.IntegralTerm = Mathf.Clamp(this.IntegralTerm, this.minValue, this.maxValue);
			float num2 = this._processVariable - this.ProcessVariableLast;
			float num3 = this.GainDerivative * (num2 / timeSinceLastUpdate);
			return Mathf.Clamp(this.GainProportional * num + this.IntegralTerm - num3, this.minValue, this.maxValue);
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06001006 RID: 4102 RVA: 0x000BA1AA File Offset: 0x000B83AA
		// (set) Token: 0x06001007 RID: 4103 RVA: 0x000BA1B2 File Offset: 0x000B83B2
		public float GainDerivative { get; set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06001008 RID: 4104 RVA: 0x000BA1BB File Offset: 0x000B83BB
		// (set) Token: 0x06001009 RID: 4105 RVA: 0x000BA1C3 File Offset: 0x000B83C3
		public float GainIntegral { get; set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x0600100A RID: 4106 RVA: 0x000BA1CC File Offset: 0x000B83CC
		// (set) Token: 0x0600100B RID: 4107 RVA: 0x000BA1D4 File Offset: 0x000B83D4
		public float GainProportional { get; set; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x0600100C RID: 4108 RVA: 0x000BA1DD File Offset: 0x000B83DD
		// (set) Token: 0x0600100D RID: 4109 RVA: 0x000BA1E5 File Offset: 0x000B83E5
		public float IntegralTerm { get; private set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x0600100E RID: 4110 RVA: 0x000BA1EE File Offset: 0x000B83EE
		// (set) Token: 0x0600100F RID: 4111 RVA: 0x000BA1F6 File Offset: 0x000B83F6
		public float ProcessVariable
		{
			get
			{
				return this._processVariable;
			}
			set
			{
				this.ProcessVariableLast = this._processVariable;
				this._processVariable = value;
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06001010 RID: 4112 RVA: 0x000BA20B File Offset: 0x000B840B
		// (set) Token: 0x06001011 RID: 4113 RVA: 0x000BA213 File Offset: 0x000B8413
		public float ProcessVariableLast { get; private set; }

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06001012 RID: 4114 RVA: 0x000BA21C File Offset: 0x000B841C
		// (set) Token: 0x06001013 RID: 4115 RVA: 0x000BA224 File Offset: 0x000B8424
		public float SetPoint { get; set; }

		// Token: 0x0400209D RID: 8349
		public float maxValue;

		// Token: 0x0400209E RID: 8350
		public float minValue;

		// Token: 0x0400209F RID: 8351
		private float _processVariable;
	}
}
