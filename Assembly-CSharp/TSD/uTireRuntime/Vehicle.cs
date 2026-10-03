using System;
using System.Collections.Generic;
using TSD.uTireSettings;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x02000346 RID: 838
	[Serializable]
	public class Vehicle
	{
		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06001576 RID: 5494 RVA: 0x000E0701 File Offset: 0x000DE901
		// (set) Token: 0x06001577 RID: 5495 RVA: 0x000E070E File Offset: 0x000DE90E
		public float tirePressureMultiplier
		{
			get
			{
				return this.vehicleData.tirePressureMultiplier;
			}
			set
			{
				if (this.vehicleData == null)
				{
					this.vehicleData = new VehicleData();
				}
				this.vehicleData.tirePressureMultiplier = value;
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06001578 RID: 5496 RVA: 0x000E072F File Offset: 0x000DE92F
		// (set) Token: 0x06001579 RID: 5497 RVA: 0x000E073C File Offset: 0x000DE93C
		[HideInInspector]
		public MinMax steeringAngle
		{
			get
			{
				return this.vehicleData.steeringAngle;
			}
			set
			{
				if (this.vehicleData == null)
				{
					this.vehicleData = new VehicleData();
				}
				this.vehicleData.steeringAngle = value;
			}
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x000E0760 File Offset: 0x000DE960
		public void CalculateHorizontalSpeed()
		{
			Vector3 vector = this.rigidBody.transform.InverseTransformDirection(this.rigidBody.velocity);
			this.speedHorizontal = vector.x;
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x000E0798 File Offset: 0x000DE998
		public Vehicle(Rigidbody m_rigidbody, List<WheelMeshConnection> m_wmc, float m_flatTireRadiusMultiplier = 0.4f, float m_maxSteeringAngle = 40f, float m_sidewaysSlideMin = 0f, float m_sidewaysSlideMax = 0f, Measurement m_measurement = Measurement.KPH)
		{
			this.flatTireRadiusMultiplier = m_flatTireRadiusMultiplier;
			this.maxSteeringAngle = m_maxSteeringAngle;
			this.rigidBody = m_rigidbody;
			this.wheels = m_wmc.ToArray();
			this.sidewaysSlideOverride = new MinMax(m_sidewaysSlideMin, m_sidewaysSlideMax);
			this.SetMeasurement(m_measurement);
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x000E0805 File Offset: 0x000DEA05
		public void SetPressureMultiplier(float m_pressureMultiplier)
		{
			this.tirePressureMultiplier = m_pressureMultiplier;
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x000E080E File Offset: 0x000DEA0E
		public void SetMeasurement(Measurement m_measurement)
		{
			if (m_measurement == Measurement.KPH)
			{
				this.speedMultiplier = 3.6f;
				return;
			}
			if (m_measurement != Measurement.MPH)
			{
				this.speedMultiplier = 3.6f;
				return;
			}
			this.speedMultiplier = 2.237f;
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x000E083C File Offset: 0x000DEA3C
		public void SetVehicleSettings(uTirePrefabSettings uTireSavedSettings, bool logErrors = false)
		{
			this.SetVehicleSettings(uTireSavedSettings.maxSteeringAngle, uTireSavedSettings.tirePressureMultiplier, uTireSavedSettings.tireRadiusMultiplier, uTireSavedSettings.tirePressure, uTireSavedSettings.slideMinMaxOverride, logErrors);
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x000E0864 File Offset: 0x000DEA64
		public void SetVehicleSettings(float m_maxSteeringAngle, float m_tirePressureMultiplier, float m_tireRadiusMultiplier, List<float> m_tirePressure, MinMax m_sidewaysSlideOverride, bool logErrors = false)
		{
			this.maxSteeringAngle = m_maxSteeringAngle;
			this.tirePressureMultiplier = m_tirePressureMultiplier;
			this.flatTireRadiusMultiplier = m_tireRadiusMultiplier;
			this.sidewaysSlideOverride = m_sidewaysSlideOverride;
			if (m_tirePressure.Count == this.wheels.Length)
			{
				for (int i = 0; i < m_tirePressure.Count; i++)
				{
					this.wheels[i].SetPressure(m_tirePressure[i]);
				}
				return;
			}
			if (logErrors)
			{
				Debug.LogWarning("Tried to load pressure settings from file, but the amount of wheels is different.");
			}
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x000E08D8 File Offset: 0x000DEAD8
		public void InitSettings()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (this.sidewaysSlideOverride.min == 0f)
			{
				this.sidewaysSlideOverride.min = uTireGlobalSettings.Instance.GetSlideMinMax().min;
			}
			if (this.sidewaysSlideOverride.max == 0f)
			{
				this.sidewaysSlideOverride.max = uTireGlobalSettings.Instance.GetSlideMinMax().max;
			}
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x000E0948 File Offset: 0x000DEB48
		public void InitWheels()
		{
			this.steeringAngle = new MinMax(this.maxSteeringAngle * 0.5f, -this.maxSteeringAngle * 0.5f);
			WheelMeshConnection[] array = this.wheels;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].init(this.flatTireRadiusMultiplier, this);
			}
		}

		// Token: 0x04002614 RID: 9748
		public Rigidbody rigidBody;

		// Token: 0x04002615 RID: 9749
		public float flatTireRadiusMultiplier = 0.85f;

		// Token: 0x04002616 RID: 9750
		[Range(0f, 90f)]
		public float maxSteeringAngle = 40f;

		// Token: 0x04002617 RID: 9751
		public WheelMeshConnection[] wheels;

		// Token: 0x04002618 RID: 9752
		public float speed;

		// Token: 0x04002619 RID: 9753
		public VehicleData vehicleData;

		// Token: 0x0400261A RID: 9754
		public float speedMultiplier = 3.6f;

		// Token: 0x0400261B RID: 9755
		public float speedRatio;

		// Token: 0x0400261C RID: 9756
		public float speedHorizontal;

		// Token: 0x0400261D RID: 9757
		public MinMax sidewaysSlideOverride;
	}
}
