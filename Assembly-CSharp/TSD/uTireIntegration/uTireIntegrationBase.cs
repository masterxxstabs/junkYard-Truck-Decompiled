using System;
using System.Collections.Generic;
using TSD.uTireRuntime;
using UnityEngine;

namespace TSD.uTireIntegration
{
	// Token: 0x02000359 RID: 857
	public class uTireIntegrationBase : MonoBehaviour
	{
		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060015D0 RID: 5584 RVA: 0x000E1FC0 File Offset: 0x000E01C0
		// (set) Token: 0x060015D1 RID: 5585 RVA: 0x000E1FC7 File Offset: 0x000E01C7
		public static Vehicle activeVehicle { get; private set; }

		// Token: 0x060015D2 RID: 5586 RVA: 0x000E1FCF File Offset: 0x000E01CF
		public static void SetActiveVehicle(Vehicle newVehicle)
		{
			uTireIntegrationBase.activeVehicle = newVehicle;
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x000E1FD8 File Offset: 0x000E01D8
		public static void FindConnections()
		{
			List<WheelMeshConnection> list = new List<WheelMeshConnection>();
			for (int i = 0; i < uTireIntegrationBase.wheelControllers.Length; i++)
			{
				list.Add(new WheelMeshConnection(uTireIntegrationBase.wheelControllers[i], uTireIntegrationBase.meshRenderers[i]));
			}
			if (list.Count == uTireIntegrationBase.wheelControllers.Length)
			{
				foreach (Vehicle vehicle in Singleton<uTireManager>.Instance.vehicles)
				{
					if (vehicle.rigidBody == uTireIntegrationBase.vehicleRigidbody)
					{
						uTireIntegrationBase.activeVehicle = vehicle;
						uTireIntegrationBase.activeVehicle.wheels = list.ToArray();
						return;
					}
				}
				uTireIntegrationBase.activeVehicle = new Vehicle(uTireIntegrationBase.vehicleRigidbody, list, uTireIntegrationBase.flatTireRadiusMultiplier, uTireIntegrationBase.maximumSteeringAngle, 0f, 0f, Measurement.KPH);
			}
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x000E20BC File Offset: 0x000E02BC
		public static void RegisterVehicle()
		{
			if (uTireIntegrationBase.activeVehicle == null)
			{
				Debug.LogWarning("The vehicle you tried to register was null");
				return;
			}
			uTireManager.RegisterVehicle(uTireIntegrationBase.activeVehicle);
		}

		// Token: 0x0400267D RID: 9853
		public static Rigidbody vehicleRigidbody;

		// Token: 0x0400267E RID: 9854
		public static Object[] wheelControllers;

		// Token: 0x0400267F RID: 9855
		public static MeshRenderer[] meshRenderers;

		// Token: 0x04002680 RID: 9856
		public static float flatTireRadiusMultiplier;

		// Token: 0x04002681 RID: 9857
		public static float maximumSteeringAngle;
	}
}
