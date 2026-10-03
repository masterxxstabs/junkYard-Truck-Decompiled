using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;

namespace NWH.WheelController3D
{
	// Token: 0x0200024E RID: 590
	[Serializable]
	public class Wheel
	{
		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x000B24A5 File Offset: 0x000B06A5
		// (set) Token: 0x06000EF7 RID: 3831 RVA: 0x000B24AD File Offset: 0x000B06AD
		public GameObject Visual
		{
			get
			{
				return this.visual;
			}
			set
			{
				this.visual = value;
				this.visualIsNull = (this.visual == null);
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x000B24C8 File Offset: 0x000B06C8
		// (set) Token: 0x06000EF9 RID: 3833 RVA: 0x000B24D0 File Offset: 0x000B06D0
		public GameObject NonRotatingVisual
		{
			get
			{
				return this.nonRotatingVisual;
			}
			set
			{
				this.nonRotatingVisual = value;
				this.nonRotatingVisualIsNull = (this.nonRotatingVisual == null);
			}
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x000B24EC File Offset: 0x000B06EC
		public void Initialize(WheelController wc)
		{
			this.visualIsNull = (this.visual == null);
			this.nonRotatingVisualIsNull = (this.nonRotatingVisual == null);
			this.inertia = 0.5f * this.mass * (this.radius * this.radius + this.radius * this.radius);
			if (this.rimColliderGO != null || !wc.useRimCollider || this.visual == null)
			{
				return;
			}
			this.rimColliderGO = new GameObject();
			this.rimColliderGO.name = "RimCollider";
			this.rimColliderGO.transform.position = wc.transform.position + wc.transform.right * (this.rimOffset * (float)wc.vehicleSide);
			this.rimColliderGO.transform.parent = wc.transform;
			this.rimColliderGO.layer = LayerMask.NameToLayer("Ignore Raycast");
			MeshFilter meshFilter = this.rimColliderGO.AddComponent<MeshFilter>();
			meshFilter.name = "Rim Mesh Filter";
			meshFilter.mesh = this.GenerateRimColliderMesh(this.visual.transform);
			meshFilter.mesh.name = "Rim Mesh";
			MeshCollider meshCollider = this.rimColliderGO.AddComponent<MeshCollider>();
			meshCollider.name = "Rim MeshCollider";
			meshCollider.convex = true;
			meshCollider.material = new PhysicMaterial
			{
				staticFriction = 0f,
				dynamicFriction = 0f,
				bounciness = 0.3f
			};
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x000B267C File Offset: 0x000B087C
		private Mesh GenerateRimColliderMesh(Transform rt)
		{
			Mesh mesh = new Mesh();
			List<Vector3> list = new List<Vector3>();
			List<int> list2 = new List<int>();
			float d = this.width / 1.5f;
			float num = 0f;
			float num2 = 0.17453294f;
			float d2 = this.radius * 0.5f * Mathf.Cos(num);
			float d3 = this.radius * 0.5f * Mathf.Sin(num);
			Vector3 a = rt.InverseTransformPoint(this.worldPosition + this.up * d3 + this.forward * d2);
			int num3 = 0;
			for (num = num2; num <= 6.2831855f + num2; num += 0.2617994f)
			{
				if (num <= 3.1415927f - num2)
				{
					d2 = this.radius * 1.06f * Mathf.Cos(num);
					d3 = this.radius * 1.06f * Mathf.Sin(num);
				}
				else
				{
					d2 = this.radius * 0.05f * Mathf.Cos(num);
					d3 = this.radius * 0.05f * Mathf.Sin(num);
				}
				Vector3 vector = rt.InverseTransformPoint(this.worldPosition + this.up * d3 + this.forward * d2);
				Vector3 item = a - rt.InverseTransformDirection(this.right) * d;
				Vector3 item2 = vector - rt.InverseTransformDirection(this.right) * d;
				Vector3 item3 = a + rt.InverseTransformDirection(this.right) * d;
				Vector3 item4 = vector + rt.InverseTransformDirection(this.right) * d;
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				list.Add(item4);
				list2.Add(num3 + 3);
				list2.Add(num3 + 1);
				list2.Add(num3);
				list2.Add(num3);
				list2.Add(num3 + 2);
				list2.Add(num3 + 3);
				a = vector;
				num3 += 4;
			}
			mesh.vertices = list.ToArray();
			mesh.triangles = list2.ToArray();
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			return mesh;
		}

		// Token: 0x04001F6B RID: 8043
		[ShowInTelemetry]
		[Tooltip("    Current angular velocity of the wheel in rad/s.")]
		public float angularVelocity;

		// Token: 0x04001F6C RID: 8044
		[ShowInTelemetry]
		[Tooltip("    Brake torque applied to the wheel in Nm.")]
		public float brakeTorque;

		// Token: 0x04001F6D RID: 8045
		[ShowInTelemetry]
		[Tooltip("    Current camber angle.")]
		public float camberAngle;

		// Token: 0x04001F6E RID: 8046
		[Tooltip("    Camber angle at the bottom of suspension travel (fully extended).")]
		public float camberAtBottom;

		// Token: 0x04001F6F RID: 8047
		[Tooltip("    Camber angle at the top of suspension travel (fully compressed).")]
		public float camberAtTop;

		// Token: 0x04001F70 RID: 8048
		[Tooltip("    Forward vector of the wheel in world coordinates.")]
		public Vector3 forward;

		// Token: 0x04001F71 RID: 8049
		[Tooltip("    Inertia of the wheel.")]
		public float inertia;

		// Token: 0x04001F72 RID: 8050
		[Tooltip("    Vector in world coordinates pointing towards the inside of the wheel.")]
		public Vector3 inside;

		// Token: 0x04001F73 RID: 8051
		[ShowInTelemetry]
		[Tooltip("    Tire load in Nm.")]
		public float load;

		// Token: 0x04001F74 RID: 8052
		[Tooltip("    Mass of the wheel. Inertia is calculated from this.")]
		public float mass = 20f;

		// Token: 0x04001F75 RID: 8053
		[ShowInTelemetry]
		[Tooltip("Motor torque applied to the wheel. Since NWH Vehicle Physics 2 the value is readonly and setting it will have no effect\r\nsince torque calculation is done inside powertrain solver.")]
		public float motorTorque;

		// Token: 0x04001F76 RID: 8054
		[Tooltip("    Position offset of the non-rotating part.")]
		public Vector3 nonRotatingPositionOffset;

		// Token: 0x04001F77 RID: 8055
		public bool nonRotatingVisualIsNull;

		// Token: 0x04001F78 RID: 8056
		public float prevAngularVelocity;

		// Token: 0x04001F79 RID: 8057
		[Tooltip("    Total radius of the tire in [m].")]
		public float radius = 0.35f;

		// Token: 0x04001F7A RID: 8058
		[Tooltip("    Vector in world coordinates pointing to the right of the wheel.")]
		public Vector3 right;

		// Token: 0x04001F7B RID: 8059
		[Tooltip("GameObject containing the rim MeshCollider. This is used to prevent objects from penetrating into the wheel from sides\r\nor top,\r\nwhere the ground detection does not work.")]
		public GameObject rimColliderGO;

		// Token: 0x04001F7C RID: 8060
		[Tooltip("    Offset of the rim from the center of steering rotation.")]
		public float rimOffset;

		// Token: 0x04001F7D RID: 8061
		[Tooltip("    Current rotation angle of the wheel visual in regards to it's X axis vector.")]
		public float rotationAngle;

		// Token: 0x04001F7E RID: 8062
		[Tooltip("    Current wheel RPM.")]
		public float RPM;

		// Token: 0x04001F7F RID: 8063
		[ShowInTelemetry]
		[Tooltip("    Current steer angle of the wheel.")]
		public float steerAngle;

		// Token: 0x04001F80 RID: 8064
		[Tooltip("    Wheel's up vector in world coordinates.")]
		public Vector3 up;

		// Token: 0x04001F81 RID: 8065
		public bool visualIsNull;

		// Token: 0x04001F82 RID: 8066
		[Tooltip("In cases where wheel visual's model might have wrong pivot point this field can\r\nbe used to center the wheel or move it in/out. It is always preferable to\r\nfix the model in modelling software or by parenting it to another, empty, transform\r\nand resetting the pivot that way.\r\nhttps://docs.unity3d.com/Manual/HOWTO-FixZAxisIsUp.html")]
		public Vector3 visualPositionOffset = Vector3.zero;

		// Token: 0x04001F83 RID: 8067
		[Tooltip("Use if wheel visual's model has wrong rotation or if you want to make the wheel appear to wobble (adjust Z axis to get the wobble).\r\nIt is always preferable to\r\nfix the model in modelling software or by parenting it to another, empty, transform\r\nand resetting the pivot that way.\r\nhttps://docs.unity3d.com/Manual/HOWTO-FixZAxisIsUp.html")]
		public Vector3 visualRotationOffset = Vector3.zero;

		// Token: 0x04001F84 RID: 8068
		[Tooltip("    Width of the tyre.")]
		public float width = 0.25f;

		// Token: 0x04001F85 RID: 8069
		[Tooltip("    Position of the wheel in world coordinates.")]
		public Vector3 worldPosition;

		// Token: 0x04001F86 RID: 8070
		[Tooltip("    Rotation of the wheel in world coordinates.")]
		public Quaternion worldRotation;

		// Token: 0x04001F87 RID: 8071
		[Tooltip("Object representing non-rotating part of the wheel. This could be things such as brake calipers, external fenders, etc.")]
		[SerializeField]
		private GameObject nonRotatingVisual;

		// Token: 0x04001F88 RID: 8072
		[Tooltip("GameObject representing the visual aspect of the wheel / wheel mesh.\r\nShould not have any physics colliders attached to it.")]
		[SerializeField]
		private GameObject visual;
	}
}
