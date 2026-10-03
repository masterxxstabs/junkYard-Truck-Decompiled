using System;
using UnityEngine;

// Token: 0x020000E3 RID: 227
[Serializable]
public class OffroadWheel
{
	// Token: 0x04000C2D RID: 3117
	public WheelCollider wheelCollider;

	// Token: 0x04000C2E RID: 3118
	public GameObject SteeringJoint1;

	// Token: 0x04000C2F RID: 3119
	public GameObject SteeringJoint2;

	// Token: 0x04000C30 RID: 3120
	public GameObject Wheel;

	// Token: 0x04000C31 RID: 3121
	public GameObject Arm1;

	// Token: 0x04000C32 RID: 3122
	public GameObject Arm2;

	// Token: 0x04000C33 RID: 3123
	public GameObject AbsorberUp;

	// Token: 0x04000C34 RID: 3124
	public GameObject AbsorberDown;

	// Token: 0x04000C35 RID: 3125
	public GameObject Spring;

	// Token: 0x04000C36 RID: 3126
	public GameObject Knuckle;

	// Token: 0x04000C37 RID: 3127
	[HideInInspector]
	public Vector3 WCPosition;

	// Token: 0x04000C38 RID: 3128
	[HideInInspector]
	public Quaternion WCRotation;

	// Token: 0x04000C39 RID: 3129
	[HideInInspector]
	public float AngularVelocity;

	// Token: 0x04000C3A RID: 3130
	public Vector3 AbsorberUpDefPos;

	// Token: 0x04000C3B RID: 3131
	public Vector3 WheelColliderDefPos;
}
