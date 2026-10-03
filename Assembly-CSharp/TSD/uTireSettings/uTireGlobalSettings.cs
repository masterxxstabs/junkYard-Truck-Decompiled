using System;
using System.Collections.Generic;
using System.Linq;
using TSD.uTireRuntime;
using UnityEngine;

namespace TSD.uTireSettings
{
	// Token: 0x02000356 RID: 854
	public class uTireGlobalSettings : ScriptableObject
	{
		// Token: 0x060015C6 RID: 5574 RVA: 0x000E1DEA File Offset: 0x000DFFEA
		public Measurement GetMeasurement()
		{
			return this.measurement;
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x000E1DF2 File Offset: 0x000DFFF2
		public void SetMeasurement(Measurement m_measurement)
		{
			this.measurement = m_measurement;
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x000E1DFB File Offset: 0x000DFFFB
		public MinMax GetSlideMinMax()
		{
			return this.slideMinMax;
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x000E1E03 File Offset: 0x000E0003
		private void loadPrefabs()
		{
			this.prefabDatabase = Resources.LoadAll("", typeof(uTirePrefabSettings)).Cast<uTirePrefabSettings>().ToList<uTirePrefabSettings>();
		}

		// Token: 0x060015CA RID: 5578 RVA: 0x000E1E2C File Offset: 0x000E002C
		public uTirePrefabSettings GetPrefabData(GameObject prefab, bool generateMessageIfPrefabNotFound = false)
		{
			foreach (uTirePrefabSettings uTirePrefabSettings in this.prefabDatabase)
			{
				if (uTirePrefabSettings.Prefab == prefab)
				{
					return uTirePrefabSettings;
				}
			}
			if (generateMessageIfPrefabNotFound)
			{
				Debug.LogError("uTire was unable to find the saved data for the provided GameObject. Make sure the GameObject is in the database. For more information please read the manual.");
			}
			return null;
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060015CB RID: 5579 RVA: 0x000E1E9C File Offset: 0x000E009C
		public static uTireGlobalSettings Instance
		{
			get
			{
				if (uTireGlobalSettings._instance == null)
				{
					uTireGlobalSettings[] array = Resources.LoadAll("", typeof(uTireGlobalSettings)).Cast<uTireGlobalSettings>().ToArray<uTireGlobalSettings>();
					if (array.Length == 0)
					{
						Debug.LogWarning("No uTireGlobalSettings were found, creating one.");
					}
					if (array.Length > 1)
					{
						Debug.LogWarning("You have multiple uTireGlobalSettings files, only the first one will be used, you should delete the rest.", array[0]);
					}
					uTireGlobalSettings._instance = array[0];
					uTireGlobalSettings._instance.loadPrefabs();
				}
				return uTireGlobalSettings._instance;
			}
		}

		// Token: 0x0400266C RID: 9836
		public LayerMask phyicsLayermask;

		// Token: 0x0400266D RID: 9837
		public int rayCount = 32;

		// Token: 0x0400266E RID: 9838
		public int rayRingCount = 3;

		// Token: 0x0400266F RID: 9839
		public float rayAngle = 360f;

		// Token: 0x04002670 RID: 9840
		public float rayAngleOffset;

		// Token: 0x04002671 RID: 9841
		public float animationSpeedOnCollision = 35f;

		// Token: 0x04002672 RID: 9842
		public float animationSpeedOnNoCollision = 75f;

		// Token: 0x04002673 RID: 9843
		[SerializeField]
		private Measurement measurement;

		// Token: 0x04002674 RID: 9844
		[Tooltip("The tire will slide sideways when the vehicle's speed is between these limits(above the effect is clamped). KPH or MPH is based on 'measurement'")]
		[SerializeField]
		private MinMax slideMinMax = new MinMax(20f, 40f);

		// Token: 0x04002675 RID: 9845
		private List<uTirePrefabSettings> prefabDatabase;

		// Token: 0x04002676 RID: 9846
		private static uTireGlobalSettings _instance;
	}
}
