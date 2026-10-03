using System;
using TSD.uTireSettings;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x02000350 RID: 848
	public class uTireWorldSpaceBehaviour3D : uTireWorldSpaceBehaviour
	{
		// Token: 0x060015BD RID: 5565 RVA: 0x000E1BC4 File Offset: 0x000DFDC4
		public override void CalculateRaycasts(out Vector4 hitData, Vector3 startPos, Vector3 dir, float scaledRayLength)
		{
			Ray ray = new Ray(startPos, dir);
			RaycastHit raycastHit = default(RaycastHit);
			if (Physics.Raycast(ray, out raycastHit, scaledRayLength, uTireGlobalSettings.Instance.phyicsLayermask))
			{
				float num = Mathf.Max(raycastHit.distance, this.minRadius * base.uniformScale);
				hitData = raycastHit.point + raycastHit.normal * (-(scaledRayLength * 2f) * (num / scaledRayLength));
				hitData = new Vector4(hitData.x, hitData.y, hitData.z, 1f - num / scaledRayLength);
				return;
			}
			hitData = startPos + dir;
			hitData.w = 0f;
		}
	}
}
