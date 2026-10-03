using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002BA RID: 698
	[Serializable]
	public struct SkidmarkRect
	{
		// Token: 0x06001294 RID: 4756 RVA: 0x000C81EC File Offset: 0x000C63EC
		public static void LocalToWorldRect(ref SkidmarkRect localRect, ref SkidmarkRect worldRect, Transform t)
		{
			worldRect.position = t.TransformPoint(localRect.position);
			worldRect.normal = t.TransformDirection(localRect.normal);
			worldRect.tangent = t.TransformDirection(localRect.tangent);
			worldRect.positionLeft = t.TransformPoint(localRect.positionLeft);
			worldRect.positionRight = t.TransformPoint(localRect.positionRight);
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x000C8260 File Offset: 0x000C6460
		public static void WorldToLocalRect(ref SkidmarkRect worldRect, ref SkidmarkRect localRect, Transform t)
		{
			localRect.position = t.InverseTransformPoint(worldRect.position);
			localRect.normal = t.InverseTransformDirection(worldRect.normal);
			localRect.tangent = t.InverseTransformDirection(worldRect.tangent);
			localRect.positionLeft = t.InverseTransformPoint(worldRect.positionLeft);
			localRect.positionRight = t.InverseTransformPoint(worldRect.positionRight);
		}

		// Token: 0x04002340 RID: 9024
		public Vector3 normal;

		// Token: 0x04002341 RID: 9025
		public Vector3 position;

		// Token: 0x04002342 RID: 9026
		public Vector3 positionLeft;

		// Token: 0x04002343 RID: 9027
		public Vector3 positionRight;

		// Token: 0x04002344 RID: 9028
		public Vector4 tangent;
	}
}
