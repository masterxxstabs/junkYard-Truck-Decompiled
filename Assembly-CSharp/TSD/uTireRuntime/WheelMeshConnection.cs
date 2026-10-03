using System;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x02000348 RID: 840
	[Serializable]
	public class WheelMeshConnection
	{
		// Token: 0x06001582 RID: 5506 RVA: 0x000E099D File Offset: 0x000DEB9D
		public Object GetWheelObject()
		{
			return this.wheelObject;
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06001583 RID: 5507 RVA: 0x000E09A5 File Offset: 0x000DEBA5
		// (set) Token: 0x06001584 RID: 5508 RVA: 0x000E09AD File Offset: 0x000DEBAD
		private VehicleData vehicleData { get; set; }

		// Token: 0x06001585 RID: 5509 RVA: 0x000E09B6 File Offset: 0x000DEBB6
		public WheelMeshConnection(Object wc, MeshRenderer mr)
		{
			this.wheelObject = wc;
			this.meshRenderer = mr;
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x000E09D8 File Offset: 0x000DEBD8
		public void init(float m_tireFlatMultiplier, Vehicle m_vehicle)
		{
			if (this.wheelObject.GetType() == typeof(WheelCollider))
			{
				this.wheel = new WC();
			}
			this.wheel.wheelObject = this.wheelObject;
			this.tireFlatMultiplier = m_tireFlatMultiplier;
			this.iWheel = (IWheel)this.wheel;
			this.tireFullRadius = this.iWheel.radius;
			if (this.tireFlatMultiplier > 0f)
			{
				this.tireFlatRadius = this.tireFullRadius * this.tireFlatMultiplier;
			}
			else
			{
				this.tireFlatRadius = this.tireFullRadius;
				Debug.LogWarning("Invalid flat tire radius, it has to be bigger than 0", this.iWheel.wheelObject);
			}
			this.vehicleData = m_vehicle.vehicleData;
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x000E0A98 File Offset: 0x000DEC98
		public void CalculatePressure(Vector3 wheelHitPoint)
		{
			if (this.iWheel.isGrounded)
			{
				this.springCompression = this.iWheel.springCompression;
				this.springCompression = 1f - this.springCompression + (1f - this.tirePressure * this.vehicleData.tirePressureMultiplier);
			}
			else
			{
				this.springCompression = 0f;
			}
			this.flatness = this.springCompression;
			this.flatness = Mathf.Clamp01(this.flatness);
		}

		// Token: 0x06001588 RID: 5512 RVA: 0x000E0B18 File Offset: 0x000DED18
		public void CalculateWheelAngle()
		{
			this.turnAngle = (Mathf.InverseLerp(this.vehicleData.steeringAngle.min, this.vehicleData.steeringAngle.max, this.iWheel.steerAngle) - 0.5f) * 2f;
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x000E0B67 File Offset: 0x000DED67
		public void SetPressure(float m_pressure)
		{
			this.tirePressure = Mathf.Clamp01(m_pressure);
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x000E0B75 File Offset: 0x000DED75
		public void SetWheelColliderRadius()
		{
			this.iWheel.radius = Mathf.Lerp(this.tireFullRadius, this.tireFlatRadius, this.flatness);
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x000E0B99 File Offset: 0x000DED99
		public void SetFlatRadius(float m_flatRadius)
		{
			this.tireFlatRadius = m_flatRadius;
		}

		// Token: 0x04002621 RID: 9761
		[SerializeField]
		private WheelBase wheel;

		// Token: 0x04002622 RID: 9762
		[SerializeField]
		public IWheel iWheel;

		// Token: 0x04002623 RID: 9763
		[SerializeField]
		private Object wheelObject;

		// Token: 0x04002624 RID: 9764
		public MeshRenderer meshRenderer;

		// Token: 0x04002625 RID: 9765
		[SerializeField]
		private float springCompression;

		// Token: 0x04002626 RID: 9766
		public float flatness;

		// Token: 0x04002627 RID: 9767
		[Range(0f, 1f)]
		public float tirePressure = 1f;

		// Token: 0x04002628 RID: 9768
		public float turnAngle;

		// Token: 0x0400262A RID: 9770
		private float tireFullRadius;

		// Token: 0x0400262B RID: 9771
		private float tireFlatRadius;

		// Token: 0x0400262C RID: 9772
		private float tireFlatMultiplier;
	}
}
