using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.Dark
{
	// Token: 0x0200032B RID: 811
	public class VirtualCursor : PointerInputModule
	{
		// Token: 0x060014C3 RID: 5315 RVA: 0x000DCA60 File Offset: 0x000DAC60
		public new void Start()
		{
			this.cursorObj = base.GetComponent<RectTransform>();
			this.pointer = new PointerEventData(this.vEventSystem);
			if (this.cursorType == VirtualCursor.CursorType.STANDARD)
			{
				this.standardCursor.SetActive(true);
				this.circleCursor.SetActive(false);
				this.frameCursor.SetActive(false);
				this.selectedCursor = this.standardCursor;
			}
			else if (this.cursorType == VirtualCursor.CursorType.CIRCLE)
			{
				this.standardCursor.SetActive(false);
				this.circleCursor.SetActive(true);
				this.frameCursor.SetActive(false);
				this.selectedCursor = this.circleCursor;
			}
			else if (this.cursorType == VirtualCursor.CursorType.FRAME)
			{
				this.standardCursor.SetActive(false);
				this.circleCursor.SetActive(false);
				this.frameCursor.SetActive(true);
				this.selectedCursor = this.frameCursor;
			}
			this.cursorAnim = this.selectedCursor.GetComponent<Animator>();
		}

		// Token: 0x060014C4 RID: 5316 RVA: 0x000DCB49 File Offset: 0x000DAD49
		public void AnimateCursorIn()
		{
			if (base.gameObject.activeSelf)
			{
				this.cursorAnim.Play("In");
			}
		}

		// Token: 0x060014C5 RID: 5317 RVA: 0x000DCB68 File Offset: 0x000DAD68
		public void AnimateCursorOut()
		{
			if (base.gameObject.activeSelf)
			{
				this.cursorAnim.Play("Out");
			}
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x000DCB88 File Offset: 0x000DAD88
		private void Update()
		{
			this.cursorPos.x = this.cursorPos.x + Input.GetAxis(this.horizontalAxis) * this.speed * Time.deltaTime;
			this.cursorPos.x = Mathf.Clamp(this.cursorPos.x, -this.border.rect.width / 2f, this.border.rect.width / 2f);
			this.cursorPos.y = this.cursorPos.y + Input.GetAxis(this.verticalAxis) * this.speed * Time.deltaTime;
			this.cursorPos.y = Mathf.Clamp(this.cursorPos.y, -this.border.rect.height / 2f, this.border.rect.height / 2f);
			this.cursorObj.anchoredPosition = this.cursorPos;
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x000DCC90 File Offset: 0x000DAE90
		public override void Process()
		{
			Vector2 position = Camera.main.WorldToScreenPoint(this.cursorObj.transform.position);
			this.pointer.position = position;
			base.eventSystem.RaycastAll(this.pointer, this.m_RaycastResultCache);
			RaycastResult raycastResult = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
			this.pointer.pointerCurrentRaycast = raycastResult;
			this.ProcessMove(this.pointer);
			if (!Input.GetButtonDown("Submit"))
			{
				this.pointer.pointerPress = null;
				this.pointer.rawPointerPress = null;
				return;
			}
			this.pointer.pressPosition = this.cursorPos;
			this.pointer.clickTime = Time.unscaledTime;
			this.pointer.pointerPressRaycast = raycastResult;
			if (this.m_RaycastResultCache.Count > 0)
			{
				this.pointer.selectedObject = raycastResult.gameObject;
				this.pointer.pointerPress = ExecuteEvents.ExecuteHierarchy<ISubmitHandler>(raycastResult.gameObject, this.pointer, ExecuteEvents.submitHandler);
				this.pointer.rawPointerPress = raycastResult.gameObject;
				return;
			}
			this.pointer.rawPointerPress = null;
		}

		// Token: 0x04002534 RID: 9524
		[Header("OBJECTS")]
		public RectTransform border;

		// Token: 0x04002535 RID: 9525
		public GameObject standardCursor;

		// Token: 0x04002536 RID: 9526
		public GameObject circleCursor;

		// Token: 0x04002537 RID: 9527
		public GameObject frameCursor;

		// Token: 0x04002538 RID: 9528
		[Header("INPUT")]
		public EventSystem vEventSystem;

		// Token: 0x04002539 RID: 9529
		public string horizontalAxis = "Horizontal";

		// Token: 0x0400253A RID: 9530
		public string verticalAxis = "Vertical";

		// Token: 0x0400253B RID: 9531
		[Header("SETTINGS")]
		[Tooltip("1000 equals 1.0 sensivity")]
		[Range(100f, 10000f)]
		public float speed = 1000f;

		// Token: 0x0400253C RID: 9532
		public VirtualCursor.CursorType cursorType;

		// Token: 0x0400253D RID: 9533
		private PointerEventData pointer;

		// Token: 0x0400253E RID: 9534
		private GameObject selectedCursor;

		// Token: 0x0400253F RID: 9535
		private Animator cursorAnim;

		// Token: 0x04002540 RID: 9536
		private Vector2 cursorPos;

		// Token: 0x04002541 RID: 9537
		private RectTransform cursorObj;

		// Token: 0x020004EC RID: 1260
		public enum CursorType
		{
			// Token: 0x04002CC9 RID: 11465
			STANDARD,
			// Token: 0x04002CCA RID: 11466
			CIRCLE,
			// Token: 0x04002CCB RID: 11467
			FRAME
		}
	}
}
