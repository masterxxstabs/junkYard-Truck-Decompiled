using System;
using UnityEngine;

namespace AshVP
{
	// Token: 0x02000335 RID: 821
	public class Sensors : MonoBehaviour
	{
		// Token: 0x06001505 RID: 5381 RVA: 0x000DE668 File Offset: 0x000DC868
		private void FixedUpdate()
		{
			foreach (Sensors.Sensor sensor in this.sensorArray)
			{
				if (sensor.sensorPoint.localPosition.x == 0f)
				{
					sensor.direction = 0f;
				}
				else
				{
					sensor.direction = sensor.sensorPoint.localPosition.x / Mathf.Abs(sensor.sensorPoint.localPosition.x);
				}
				if (Physics.Raycast(sensor.sensorPoint.position, sensor.sensorPoint.forward, out sensor.hit, this.sensorLength))
				{
					if (sensor.hit.collider.CompareTag(this.IgnoreSensorTag))
					{
						sensor.weight = 0f;
					}
					else
					{
						sensor.weight = 1f;
					}
				}
				else
				{
					sensor.weight = 0f;
				}
			}
			this.obstacleInPath = this.IsobstacleInPath();
			this.SensorTurnAmount = this.SensorValue(this.sensorArray);
			if (this.SensorTurnAmount == 0f && this.obstacleInPath)
			{
				this.ObstacleAngle = Vector3.Dot(this.sensorArray[1].hit.normal, base.transform.right);
				if (this.ObstacleAngle > 0f)
				{
					this.turnmultiplyer = -1f;
				}
				if (this.ObstacleAngle < 0f)
				{
					this.turnmultiplyer = 1f;
					return;
				}
			}
			else
			{
				this.turnmultiplyer = Mathf.Sign(this.SensorTurnAmount);
			}
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x000DE7E8 File Offset: 0x000DC9E8
		private bool IsobstacleInPath()
		{
			for (int i = 0; i < this.sensorArray.Length; i++)
			{
				if (this.sensorArray[i].weight == 1f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x000DE820 File Offset: 0x000DCA20
		private float SensorValue(Sensors.Sensor[] sensors)
		{
			float num = 0f;
			for (int i = 0; i < sensors.Length; i++)
			{
				num += sensors[i].weight * sensors[i].direction;
			}
			return num;
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x000DE858 File Offset: 0x000DCA58
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Sensors.Sensor[] array = this.sensorArray;
			for (int i = 0; i < array.Length; i++)
			{
				float weight = array[i].weight;
			}
		}

		// Token: 0x040025A8 RID: 9640
		[SerializeField]
		private Sensors.Sensor[] sensorArray;

		// Token: 0x040025A9 RID: 9641
		[HideInInspector]
		public float turnmultiplyer;

		// Token: 0x040025AA RID: 9642
		public float sensorLength;

		// Token: 0x040025AB RID: 9643
		[HideInInspector]
		public float SensorTurnAmount;

		// Token: 0x040025AC RID: 9644
		[HideInInspector]
		public bool obstacleInPath;

		// Token: 0x040025AD RID: 9645
		[HideInInspector]
		public float ObstacleAngle;

		// Token: 0x040025AE RID: 9646
		public string IgnoreSensorTag;

		// Token: 0x040025AF RID: 9647
		public string PlayerTag;

		// Token: 0x020004F0 RID: 1264
		[Serializable]
		private class Sensor
		{
			// Token: 0x04002CD8 RID: 11480
			[HideInInspector]
			public float weight;

			// Token: 0x04002CD9 RID: 11481
			public Transform sensorPoint;

			// Token: 0x04002CDA RID: 11482
			[HideInInspector]
			public float direction;

			// Token: 0x04002CDB RID: 11483
			[HideInInspector]
			public RaycastHit hit;
		}
	}
}
