using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200005C RID: 92
public class Drag : MonoBehaviour
{
	// Token: 0x060001AA RID: 426 RVA: 0x00011A01 File Offset: 0x0000FC01
	private void Start()
	{
		this.slotpos = base.transform.position;
		LayerMask.GetMask(new string[]
		{
			"UI"
		});
	}

	// Token: 0x060001AB RID: 427 RVA: 0x00011A30 File Offset: 0x0000FC30
	public void DragHandler(BaseEventData data)
	{
		PointerEventData pointerEventData = (PointerEventData)data;
		RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)this.canvas.transform, pointerEventData.position, this.canvas.worldCamera, out this.position);
		base.transform.position = this.canvas.transform.TransformPoint(this.position);
	}

	// Token: 0x060001AC RID: 428 RVA: 0x00011A98 File Offset: 0x0000FC98
	public void DragRelease()
	{
		this.name.text = "";
		this.desc.text = "";
		this.icon.enabled = false;
		base.transform.position = this.slotpos;
		if (this.position.x > 35f && this.position.x < 260f && this.position.y > 35f && this.position.y < 90f)
		{
			this.invMgr.RetrieveObject2(this.slotNum);
			return;
		}
		this.invMgr.DropObject(this.slotNum);
	}

	// Token: 0x0400049A RID: 1178
	[SerializeField]
	private Canvas canvas;

	// Token: 0x0400049B RID: 1179
	private Vector2 slotpos;

	// Token: 0x0400049C RID: 1180
	private int layer_mask;

	// Token: 0x0400049D RID: 1181
	public CanvasGroup canvasGroup;

	// Token: 0x0400049E RID: 1182
	public new Text name;

	// Token: 0x0400049F RID: 1183
	public Text desc;

	// Token: 0x040004A0 RID: 1184
	public RawImage icon;

	// Token: 0x040004A1 RID: 1185
	public InventoryItems invMgr;

	// Token: 0x040004A2 RID: 1186
	public int slotNum;

	// Token: 0x040004A3 RID: 1187
	private Vector2 position;
}
