using System;
using UnityEngine;

namespace UnitySA.Utility
{
	// Token: 0x020001AB RID: 427
	[Serializable]
	public class CurveCtrlBob
	{
		// Token: 0x06000A89 RID: 2697 RVA: 0x0008D13C File Offset: 0x0008B33C
		public void Setup(Camera camera, float bobBaseInterval)
		{
			this.m_BobBaseInterval = bobBaseInterval;
			this.m_OriginalCameraPosition = camera.transform.localPosition;
			this.m_Time = this.Bobcurve[this.Bobcurve.length - 1].time;
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x0008D188 File Offset: 0x0008B388
		public Vector3 DoHeadBob(float speed)
		{
			float x = this.m_OriginalCameraPosition.x + this.Bobcurve.Evaluate(this.m_CyclePositionX) * this.HorizontalBobRange;
			float y = this.m_OriginalCameraPosition.y + this.Bobcurve.Evaluate(this.m_CyclePositionY) * this.VerticalBobRange;
			this.m_CyclePositionX += speed * Time.deltaTime / this.m_BobBaseInterval;
			this.m_CyclePositionY += speed * Time.deltaTime / this.m_BobBaseInterval * this.VerticaltoHorizontalRatio;
			if (this.m_CyclePositionX > this.m_Time)
			{
				this.m_CyclePositionX -= this.m_Time;
			}
			if (this.m_CyclePositionY > this.m_Time)
			{
				this.m_CyclePositionY -= this.m_Time;
			}
			return new Vector3(x, y, 0f);
		}

		// Token: 0x04001C95 RID: 7317
		public float HorizontalBobRange = 0.33f;

		// Token: 0x04001C96 RID: 7318
		public float VerticalBobRange = 0.33f;

		// Token: 0x04001C97 RID: 7319
		public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.5f, 1f),
			new Keyframe(1f, 0f),
			new Keyframe(1.5f, -1f),
			new Keyframe(2f, 0f)
		});

		// Token: 0x04001C98 RID: 7320
		public float VerticaltoHorizontalRatio = 1f;

		// Token: 0x04001C99 RID: 7321
		private float m_CyclePositionX;

		// Token: 0x04001C9A RID: 7322
		private float m_CyclePositionY;

		// Token: 0x04001C9B RID: 7323
		private float m_BobBaseInterval;

		// Token: 0x04001C9C RID: 7324
		private Vector3 m_OriginalCameraPosition;

		// Token: 0x04001C9D RID: 7325
		private float m_Time;
	}
}
