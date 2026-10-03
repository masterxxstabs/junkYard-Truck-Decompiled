using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class uTireExample2DCarController : MonoBehaviour
{
	// Token: 0x0600000A RID: 10 RVA: 0x00002194 File Offset: 0x00000394
	private void Awake()
	{
		foreach (uTireExample2DCarController.wheelData2D wheelData2D in this.joints)
		{
			wheelData2D.jointMotor = default(JointMotor2D);
			wheelData2D.wheelJoint2D.useMotor = true;
		}
	}

	// Token: 0x0600000B RID: 11 RVA: 0x000021F8 File Offset: 0x000003F8
	private void Update()
	{
		if (Input.GetKey(KeyCode.A))
		{
			foreach (uTireExample2DCarController.wheelData2D wheelData2D in this.joints)
			{
				wheelData2D.SetSpeed(this.maxSpeed, new float?(this.maxTorque));
			}
		}
		if (Input.GetKey(KeyCode.D))
		{
			foreach (uTireExample2DCarController.wheelData2D wheelData2D2 in this.joints)
			{
				wheelData2D2.SetSpeed(-this.maxSpeed, new float?(this.maxTorque));
			}
		}
		if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
		{
			foreach (uTireExample2DCarController.wheelData2D wheelData2D3 in this.joints)
			{
				wheelData2D3.SetSpeed(0f, null);
			}
		}
	}

	// Token: 0x0600000C RID: 12 RVA: 0x0000231C File Offset: 0x0000051C
	private void OnGUI()
	{
		GUILayout.Box("Press A to move left, B to move right. The scene is entirely using 2D physics.", Array.Empty<GUILayoutOption>());
	}

	// Token: 0x04000003 RID: 3
	public List<uTireExample2DCarController.wheelData2D> joints;

	// Token: 0x04000004 RID: 4
	public float maxSpeed = 500f;

	// Token: 0x04000005 RID: 5
	public float maxTorque = 3000f;

	// Token: 0x0200035B RID: 859
	[Serializable]
	public class wheelData2D
	{
		// Token: 0x060015D7 RID: 5591 RVA: 0x000E2114 File Offset: 0x000E0314
		public void SetSpeed(float _speed, float? _torque = null)
		{
			this.jointMotor.motorSpeed = _speed;
			this.jointMotor.maxMotorTorque = (_torque ?? this.jointMotor.maxMotorTorque);
			this.wheelJoint2D.motor = this.jointMotor;
		}

		// Token: 0x04002692 RID: 9874
		public WheelJoint2D wheelJoint2D;

		// Token: 0x04002693 RID: 9875
		public JointMotor2D jointMotor;
	}
}
