using System;
using TSD.uTireSettings;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x0200034F RID: 847
	[ExecuteInEditMode]
	public class uTireWorldSpaceBehaviour2D : uTireWorldSpaceBehaviour
	{
		// Token: 0x060015BB RID: 5563 RVA: 0x000E1AC0 File Offset: 0x000DFCC0
		public override void CalculateRaycasts(out Vector4 hitData, Vector3 startPos, Vector3 dir, float scaledRayLength)
		{
			RaycastHit2D raycastHit2D = Physics2D.Raycast(startPos, dir, this.rayLength, uTireGlobalSettings.Instance.phyicsLayermask.value);
			if (raycastHit2D.collider != null)
			{
				float num = Mathf.Max(raycastHit2D.distance, this.minRadius * base.uniformScale);
				Vector2 vector = raycastHit2D.point + raycastHit2D.normal * (-(scaledRayLength * 2f) * (num / scaledRayLength));
				hitData = new Vector4(vector.x, vector.y, startPos.z, 1f - num / scaledRayLength);
				raycastHit2D.normal * (-2f * (num / scaledRayLength));
				new Vector3(0f, vector.y, vector.x);
				return;
			}
			hitData = startPos + dir;
			hitData.w = 0f;
		}
	}
}
