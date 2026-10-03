using System;
using UnityEngine;

namespace WaveMaker
{
	// Token: 0x020001A8 RID: 424
	internal static class WaveMakerUtils
	{
		// Token: 0x06000A74 RID: 2676 RVA: 0x0008C664 File Offset: 0x0008A864
		public static Bounds TransformBounds(Bounds inBounds, Matrix4x4 matrix)
		{
			Vector4 v = matrix.GetColumn(0) * inBounds.min.x;
			Vector4 v2 = matrix.GetColumn(0) * inBounds.max.x;
			Vector4 v3 = matrix.GetColumn(1) * inBounds.min.y;
			Vector4 v4 = matrix.GetColumn(1) * inBounds.max.y;
			Vector4 v5 = matrix.GetColumn(2) * inBounds.min.z;
			Vector4 v6 = matrix.GetColumn(2) * inBounds.max.z;
			Bounds result = default(Bounds);
			Vector3 b = matrix.GetColumn(3);
			result.SetMinMax(Vector3.Min(v, v2) + Vector3.Min(v3, v4) + Vector3.Min(v5, v6) + b, Vector3.Max(v, v2) + Vector3.Max(v3, v4) + Vector3.Max(v5, v6) + b);
			return result;
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x0008C7BC File Offset: 0x0008A9BC
		public static bool IsPointInsideCollider(Collider collider, Vector3 point)
		{
			Vector3 vector = collider.ClosestPoint(point);
			vector.x -= point.x;
			vector.y -= point.y;
			vector.z -= point.z;
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z < WaveMakerUtils.epsilon;
		}

		// Token: 0x06000A76 RID: 2678 RVA: 0x0008C834 File Offset: 0x0008AA34
		public static Vector3 VelocityAtPoint(Vector3 point, Vector3 center, Vector3 angularVelocity, Vector3 linearVelocity)
		{
			return Vector3.Cross(angularVelocity, point - center) + linearVelocity;
		}

		// Token: 0x06000A77 RID: 2679 RVA: 0x0008C84C File Offset: 0x0008AA4C
		public static Vector3 GetAngularVelocity(Quaternion oldQuat, Quaternion newQuat)
		{
			float d = 2f / Time.fixedDeltaTime;
			oldQuat.x = -oldQuat.x;
			oldQuat.y = -oldQuat.y;
			oldQuat.z = -oldQuat.z;
			oldQuat = newQuat * oldQuat;
			return new Vector3(oldQuat.x, oldQuat.y, oldQuat.z) * d;
		}

		// Token: 0x04001C6F RID: 7279
		private static float epsilon = 0.001f;
	}
}
