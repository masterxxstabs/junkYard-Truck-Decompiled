using System;
using UnityEngine;

namespace UnitySA.Characters.FirstPerson
{
	// Token: 0x020001AA RID: 426
	[Serializable]
	public class MLook
	{
		// Token: 0x06000A82 RID: 2690 RVA: 0x0008CEB3 File Offset: 0x0008B0B3
		public void Init(Transform character, Transform camera)
		{
			this.m_CharacterTargetRot = character.localRotation;
			this.m_CameraTargetRot = camera.localRotation;
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x0008CED0 File Offset: 0x0008B0D0
		public void LookRotation(Transform character, Transform camera)
		{
			float y = Input.GetAxis("Mouse X") * this.XSensitivity;
			float num = Input.GetAxis("Mouse Y") * this.YSensitivity;
			this.m_CharacterTargetRot *= Quaternion.Euler(0f, y, 0f);
			this.m_CameraTargetRot *= Quaternion.Euler(-num, 0f, 0f);
			if (this.clampVerticalRotation)
			{
				this.m_CameraTargetRot = this.ClampRotationAroundXAxis(this.m_CameraTargetRot);
			}
			if (this.smooth)
			{
				character.localRotation = Quaternion.Slerp(character.localRotation, this.m_CharacterTargetRot, this.smoothTime * Time.deltaTime);
				camera.localRotation = Quaternion.Slerp(camera.localRotation, this.m_CameraTargetRot, this.smoothTime * Time.deltaTime);
			}
			else
			{
				character.localRotation = this.m_CharacterTargetRot;
				camera.localRotation = this.m_CameraTargetRot;
			}
			this.UpdateCursorLock();
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x0008CFCC File Offset: 0x0008B1CC
		public void SetCursorLock(bool value)
		{
			this.lockCursor = value;
			if (!this.lockCursor)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x0008CFE9 File Offset: 0x0008B1E9
		public void UpdateCursorLock()
		{
			if (this.lockCursor)
			{
				this.InternalLockUpdate();
			}
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x0008CFFC File Offset: 0x0008B1FC
		private void InternalLockUpdate()
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				this.m_cursorIsLocked = false;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				this.m_cursorIsLocked = true;
			}
			if (this.m_cursorIsLocked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				return;
			}
			if (!this.m_cursorIsLocked)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x0008D054 File Offset: 0x0008B254
		private Quaternion ClampRotationAroundXAxis(Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1f;
			float num = 114.59156f * Mathf.Atan(q.x);
			num = Mathf.Clamp(num, this.MinimumX, this.MaximumX);
			q.x = Mathf.Tan(0.008726646f * num);
			return q;
		}

		// Token: 0x04001C8A RID: 7306
		public float XSensitivity = 2f;

		// Token: 0x04001C8B RID: 7307
		public float YSensitivity = 2f;

		// Token: 0x04001C8C RID: 7308
		public bool clampVerticalRotation = true;

		// Token: 0x04001C8D RID: 7309
		public float MinimumX = -90f;

		// Token: 0x04001C8E RID: 7310
		public float MaximumX = 90f;

		// Token: 0x04001C8F RID: 7311
		public bool smooth;

		// Token: 0x04001C90 RID: 7312
		public float smoothTime = 5f;

		// Token: 0x04001C91 RID: 7313
		public bool lockCursor = true;

		// Token: 0x04001C92 RID: 7314
		private Quaternion m_CharacterTargetRot;

		// Token: 0x04001C93 RID: 7315
		private Quaternion m_CameraTargetRot;

		// Token: 0x04001C94 RID: 7316
		private bool m_cursorIsLocked = true;
	}
}
