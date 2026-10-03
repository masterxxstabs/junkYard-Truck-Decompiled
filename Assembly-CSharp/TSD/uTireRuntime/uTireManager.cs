using System;
using System.Collections.Generic;
using TSD.uTireSettings;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x0200034A RID: 842
	public class uTireManager : Singleton<uTireManager>
	{
		// Token: 0x0600158D RID: 5517 RVA: 0x000E0BB8 File Offset: 0x000DEDB8
		private void Start()
		{
			this.initVehicles();
			this.cacheShaderPropertyIDs();
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x000E0BC6 File Offset: 0x000DEDC6
		private void FixedUpdate()
		{
			if (this.updateMaterial)
			{
				this.updateWheels();
			}
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x000E0BD8 File Offset: 0x000DEDD8
		public void updateWheels()
		{
			foreach (Vehicle vehicle in this.vehicles)
			{
				vehicle.speed = vehicle.rigidBody.velocity.magnitude * vehicle.speedMultiplier;
				vehicle.speedRatio = Mathf.InverseLerp(vehicle.sidewaysSlideOverride.min, vehicle.sidewaysSlideOverride.max, vehicle.speed);
				vehicle.CalculateHorizontalSpeed();
				foreach (WheelMeshConnection wheelMeshConnection in vehicle.wheels)
				{
					wheelMeshConnection.CalculatePressure(wheelMeshConnection.iWheel.GetGroundHitPoint());
					wheelMeshConnection.CalculateWheelAngle();
					if (this.updateWheelColliderRadius)
					{
						wheelMeshConnection.SetWheelColliderRadius();
					}
					this.setTireDeformationMaterialInstancedProperties(vehicle, wheelMeshConnection);
				}
			}
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x000E0CC8 File Offset: 0x000DEEC8
		private void setTireDeformationMaterialInstancedProperties(Vehicle m_vehicle, WheelMeshConnection m_wmc)
		{
			this.props.SetFloat(this.propID_TireFlatnessT, m_wmc.flatness);
			this.props.SetFloat(this.propID_turn, m_wmc.turnAngle * m_vehicle.speedRatio);
			m_wmc.meshRenderer.SetPropertyBlock(this.props);
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x000E0D1C File Offset: 0x000DEF1C
		public static WheelMeshConnection GetWheelMeshConnection(Object wc)
		{
			foreach (Vehicle vehicle in Singleton<uTireManager>.Instance.vehicles)
			{
				foreach (WheelMeshConnection wheelMeshConnection in vehicle.wheels)
				{
					if (wheelMeshConnection.iWheel.wheelObject == wc)
					{
						return wheelMeshConnection;
					}
				}
			}
			return null;
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x000E0DA0 File Offset: 0x000DEFA0
		public static Vehicle GetVehicle(Object wc)
		{
			foreach (Vehicle vehicle in Singleton<uTireManager>.Instance.vehicles)
			{
				WheelMeshConnection[] wheels = vehicle.wheels;
				for (int i = 0; i < wheels.Length; i++)
				{
					if (wheels[i].iWheel.wheelObject == wc)
					{
						return vehicle;
					}
				}
			}
			return null;
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x000E0E24 File Offset: 0x000DF024
		public static Vehicle GetVehicle(Rigidbody m_vehicleRigidbody)
		{
			foreach (Vehicle vehicle in Singleton<uTireManager>.Instance.vehicles)
			{
				if (vehicle.rigidBody == m_vehicleRigidbody)
				{
					return vehicle;
				}
			}
			return null;
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x000E0E8C File Offset: 0x000DF08C
		public static void SetTirePressure(WheelMeshConnection wmc, float m_pressure)
		{
			wmc.SetPressure(m_pressure);
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x000E0E95 File Offset: 0x000DF095
		public static void RegisterVehicle(Vehicle m_vehicle)
		{
			m_vehicle.InitWheels();
			m_vehicle.SetMeasurement(uTireGlobalSettings.Instance.GetMeasurement());
			Singleton<uTireManager>.Instance.vehicles.Add(m_vehicle);
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x000E0EC0 File Offset: 0x000DF0C0
		public static void RegisterVehicle(Rigidbody m_vehicleRigidbody, List<WheelCollider> m_wheelColliders, List<MeshRenderer> m_wheelMeshes, float m_flatTireRadius = 0.4f, float m_maxSteeringAngle = 40f, float m_sidewaysSlideMin = 0f, float m_sidewaysSlideMax = 0f, Measurement m_measurement = Measurement.KPH)
		{
			if (m_wheelColliders.Count != m_wheelMeshes.Count)
			{
				Debug.LogError("WheelCollider and WheelMesh count isn't the same, unable to register the vehicle", m_vehicleRigidbody);
				return;
			}
			List<WheelMeshConnection> list = new List<WheelMeshConnection>();
			for (int i = 0; i < m_wheelColliders.Count; i++)
			{
				list.Add(new WheelMeshConnection(m_wheelColliders[i], m_wheelMeshes[i]));
			}
			uTireManager.RegisterVehicle(new Vehicle(m_vehicleRigidbody, list, m_flatTireRadius, m_maxSteeringAngle, m_sidewaysSlideMin, m_sidewaysSlideMax, m_measurement));
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x000E0F2C File Offset: 0x000DF12C
		public static void RegisterVehicle(Rigidbody m_vehicleRigidbody, List<WheelCollider> m_wheelColliders, List<MeshRenderer> m_wheelMeshes, uTirePrefabSettings settingsToLoad)
		{
			uTireManager.RegisterVehicle(m_vehicleRigidbody, m_wheelColliders, m_wheelMeshes, settingsToLoad.tireRadiusMultiplier, settingsToLoad.maxSteeringAngle, settingsToLoad.slideMinMaxOverride.min, settingsToLoad.slideMinMaxOverride.max, Measurement.KPH);
		}

		// Token: 0x06001598 RID: 5528 RVA: 0x000E0F5C File Offset: 0x000DF15C
		public static void RemoveVehicle(Rigidbody m_vehicleRigidbody)
		{
			Vehicle vehicle = null;
			foreach (Vehicle vehicle2 in Singleton<uTireManager>.Instance.vehicles)
			{
				if (vehicle2.rigidBody == m_vehicleRigidbody)
				{
					vehicle = vehicle2;
					break;
				}
			}
			if (vehicle != null)
			{
				Singleton<uTireManager>.Instance.vehicles.Remove(vehicle);
			}
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x000E0FD4 File Offset: 0x000DF1D4
		public static void CleanupAfterVehicleRemoval()
		{
			Singleton<uTireManager>.Instance.vehicles.RemoveAll((Vehicle item) => item.rigidBody == null || item.rigidBody.Equals(null));
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x000E1008 File Offset: 0x000DF208
		private void initVehicles()
		{
			foreach (Vehicle vehicle in this.vehicles)
			{
				vehicle.SetMeasurement(uTireGlobalSettings.Instance.GetMeasurement());
				vehicle.InitSettings();
				vehicle.InitWheels();
			}
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x000E1070 File Offset: 0x000DF270
		private void cacheShaderPropertyIDs()
		{
			this.props = new MaterialPropertyBlock();
			this.propID_TireFlatnessT = Shader.PropertyToID("_TireFlatnessT");
			this.propID_turn = Shader.PropertyToID("_turn");
		}

		// Token: 0x0400262F RID: 9775
		public bool updateMaterial = true;

		// Token: 0x04002630 RID: 9776
		public bool updateWheelColliderRadius = true;

		// Token: 0x04002631 RID: 9777
		public List<Vehicle> vehicles = new List<Vehicle>();

		// Token: 0x04002632 RID: 9778
		private MaterialPropertyBlock props;

		// Token: 0x04002633 RID: 9779
		private int propID_TireFlatnessT;

		// Token: 0x04002634 RID: 9780
		private int propID_turn;

		// Token: 0x04002635 RID: 9781
		public static WheelHit wHit;
	}
}
