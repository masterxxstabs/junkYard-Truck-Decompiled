using System;
using System.Collections.Generic;
using TSD.uTireRuntime;
using UnityEngine;

namespace TSD.uTireSettings
{
	// Token: 0x02000357 RID: 855
	[CreateAssetMenu(fileName = "uTire Prefab", menuName = "TSD/Tire Deformation/Prefab Settings")]
	public class uTirePrefabSettings : ScriptableObject
	{
		// Token: 0x060015CD RID: 5581 RVA: 0x000E1F68 File Offset: 0x000E0168
		public uTirePrefabSettings(GameObject _prefab, float _maxSteeringAngle, float _tireFlatnessMultiplier, List<float> _tireFlantess, float _tireRadiusMultiplier, MinMax _slideMinMaxOverride)
		{
			this.Prefab = _prefab;
			this.maxSteeringAngle = _maxSteeringAngle;
			this.tirePressureMultiplier = _tireFlatnessMultiplier;
			this.tirePressure = new List<float>();
			this.tirePressure.AddRange(_tireFlantess);
			this.tireRadiusMultiplier = _tireRadiusMultiplier;
			this.slideMinMaxOverride = _slideMinMaxOverride;
		}

		// Token: 0x04002677 RID: 9847
		public GameObject Prefab;

		// Token: 0x04002678 RID: 9848
		public float maxSteeringAngle;

		// Token: 0x04002679 RID: 9849
		public float tirePressureMultiplier;

		// Token: 0x0400267A RID: 9850
		public List<float> tirePressure;

		// Token: 0x0400267B RID: 9851
		public float tireRadiusMultiplier;

		// Token: 0x0400267C RID: 9852
		public MinMax slideMinMaxOverride;
	}
}
