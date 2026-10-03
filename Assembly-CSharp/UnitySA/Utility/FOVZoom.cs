using System;
using System.Collections;
using UnityEngine;

namespace UnitySA.Utility
{
	// Token: 0x020001AC RID: 428
	[Serializable]
	public class FOVZoom
	{
		// Token: 0x06000A8C RID: 2700 RVA: 0x0008D31B File Offset: 0x0008B51B
		public void Setup(Camera camera)
		{
			this.CheckStatus(camera);
			this.Camera = camera;
			this.originalFov = camera.fieldOfView;
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x0008D337 File Offset: 0x0008B537
		private void CheckStatus(Camera camera)
		{
			if (camera == null)
			{
				throw new Exception("FOVKick camera is null, please supply the camera to the constructor");
			}
			if (this.IncreaseCurve == null)
			{
				throw new Exception("FOVKick Increase curve is null, please define the curve for the field of view kicks");
			}
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x0008D360 File Offset: 0x0008B560
		public void ChangeCamera(Camera camera)
		{
			this.Camera = camera;
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x0008D369 File Offset: 0x0008B569
		public IEnumerator FOVKickUp()
		{
			float t = Mathf.Abs((this.Camera.fieldOfView - this.originalFov) / this.FOVIncrease);
			while (t < this.TimeToIncrease)
			{
				this.Camera.fieldOfView = this.originalFov + this.IncreaseCurve.Evaluate(t / this.TimeToIncrease) * this.FOVIncrease;
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0008D378 File Offset: 0x0008B578
		public IEnumerator FOVKickDown()
		{
			float t = Mathf.Abs((this.Camera.fieldOfView - this.originalFov) / this.FOVIncrease);
			while (t > 0f)
			{
				this.Camera.fieldOfView = this.originalFov + this.IncreaseCurve.Evaluate(t / this.TimeToDecrease) * this.FOVIncrease;
				t -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			this.Camera.fieldOfView = this.originalFov;
			yield break;
		}

		// Token: 0x04001C9E RID: 7326
		public Camera Camera;

		// Token: 0x04001C9F RID: 7327
		[HideInInspector]
		public float originalFov;

		// Token: 0x04001CA0 RID: 7328
		public float FOVIncrease = 3f;

		// Token: 0x04001CA1 RID: 7329
		public float TimeToIncrease = 1f;

		// Token: 0x04001CA2 RID: 7330
		public float TimeToDecrease = 1f;

		// Token: 0x04001CA3 RID: 7331
		public AnimationCurve IncreaseCurve;
	}
}
