using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000F9 RID: 249
public class MissionWaypoint : MonoBehaviour
{
	// Token: 0x06000664 RID: 1636 RVA: 0x0004C610 File Offset: 0x0004A810
	private void Update()
	{
		if (this.showPointer)
		{
			Vector2 v = Camera.main.WorldToScreenPoint(this.target.position);
			if (Vector3.Dot(this.target.position - base.transform.position, base.transform.forward) < 0f)
			{
				this.img.enabled = false;
				this.meter.enabled = false;
			}
			else
			{
				this.EnablePointer();
			}
			this.img.transform.position = v;
			this.meter.text = ((int)Vector3.Distance(this.target.position, base.transform.position)).ToString() + "m";
		}
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x0004C6E4 File Offset: 0x0004A8E4
	public void HidePointer()
	{
		this.showPointer = false;
		this.img.enabled = false;
		this.meter.enabled = false;
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0004C705 File Offset: 0x0004A905
	public void EnablePointer()
	{
		this.showPointer = true;
		this.img.enabled = true;
		this.meter.enabled = true;
		this.ToggleDiamondMag();
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x0004C72C File Offset: 0x0004A92C
	public void ToggleDiamondMag()
	{
		if (this.useMag)
		{
			this.img.sprite = this.diamondMgf;
			return;
		}
		this.img.sprite = this.diamond;
	}

	// Token: 0x04000D57 RID: 3415
	public Image img;

	// Token: 0x04000D58 RID: 3416
	public Transform target;

	// Token: 0x04000D59 RID: 3417
	public Text meter;

	// Token: 0x04000D5A RID: 3418
	public bool showPointer;

	// Token: 0x04000D5B RID: 3419
	public Sprite diamond;

	// Token: 0x04000D5C RID: 3420
	public Sprite diamondMgf;

	// Token: 0x04000D5D RID: 3421
	public bool useMag;
}
