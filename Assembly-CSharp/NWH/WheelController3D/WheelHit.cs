using System;
using UnityEngine;

namespace NWH.WheelController3D
{
	// Token: 0x02000250 RID: 592
	public class WheelHit
	{
		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000F62 RID: 3938 RVA: 0x000B5ED4 File Offset: 0x000B40D4
		public Collider collider
		{
			get
			{
				return this.raycastHit.collider;
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000F63 RID: 3939 RVA: 0x000B5EE1 File Offset: 0x000B40E1
		public Vector3 normal
		{
			get
			{
				return this.raycastHit.normal;
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000F64 RID: 3940 RVA: 0x000B5EEE File Offset: 0x000B40EE
		public Vector3 point
		{
			get
			{
				return this.groundPoint;
			}
		}

		// Token: 0x04001FDA RID: 8154
		public float angleForward;

		// Token: 0x04001FDB RID: 8155
		public float curvatureOffset;

		// Token: 0x04001FDC RID: 8156
		public float distanceFromTire;

		// Token: 0x04001FDD RID: 8157
		[Tooltip("    The magnitude of the force being applied for the contact. [N]")]
		public float force;

		// Token: 0x04001FDE RID: 8158
		[Tooltip("    The direction the wheel is pointing in.")]
		public Vector3 forwardDir;

		// Token: 0x04001FDF RID: 8159
		[Tooltip("    Tire slip in the rolling direction.")]
		public float forwardSlip;

		// Token: 0x04001FE0 RID: 8160
		public Vector3 groundPoint;

		// Token: 0x04001FE1 RID: 8161
		public Vector2 offset;

		// Token: 0x04001FE2 RID: 8162
		[SerializeField]
		public RaycastHit raycastHit;

		// Token: 0x04001FE3 RID: 8163
		[Tooltip("    The sideways direction of the wheel.")]
		public Vector3 sidewaysDir;

		// Token: 0x04001FE4 RID: 8164
		[Tooltip("    The slip in the sideways direction.")]
		public float sidewaysSlip;

		// Token: 0x04001FE5 RID: 8165
		public bool valid;

		// Token: 0x04001FE6 RID: 8166
		public float weight;
	}
}
