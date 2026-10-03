using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
	// Token: 0x02000280 RID: 640
	[CreateAssetMenu(fileName = "NWH Vehicle Physics", menuName = "NWH Vehicle Physics/Gearing Profile", order = 1)]
	[Serializable]
	public class TransmissionGearingProfile : ScriptableObject
	{
		// Token: 0x0400219B RID: 8603
		[SerializeField]
		[Tooltip("    List of forward gear ratios starting from 1st forward gear.")]
		public List<float> forwardGears = new List<float>
		{
			8f,
			5.5f,
			4f,
			3f,
			2.2f,
			1.7f,
			1.3f
		};

		// Token: 0x0400219C RID: 8604
		[SerializeField]
		[Tooltip("    List of reverse gear ratios starting from 1st reverse gear.")]
		public List<float> reverseGears = new List<float>
		{
			-5f
		};
	}
}
