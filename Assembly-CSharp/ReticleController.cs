using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000125 RID: 293
public class ReticleController : MonoBehaviour
{
	// Token: 0x060007AE RID: 1966 RVA: 0x00063328 File Offset: 0x00061528
	private void Start()
	{
		this.defaultIcon = GameObject.Find("DefaultIcon");
		this.interactIcon = GameObject.Find("InteractIcon");
		this.snapIcon = GameObject.Find("SnapIcon");
		this.descriptor = GameObject.Find("Descriptor");
		this.interactIcon.SetActive(false);
		this.snapIcon.SetActive(false);
		this.food = GameObject.Find("Food");
		this.water = GameObject.Find("Water");
		this.sleep = GameObject.Find("Sleep");
		this.stress = GameObject.Find("Sanity");
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x000633CD File Offset: 0x000615CD
	public void ShowIcon(bool isInteractIcon)
	{
		this.defaultIcon.SetActive(!isInteractIcon);
		this.interactIcon.SetActive(isInteractIcon);
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x000633EA File Offset: 0x000615EA
	public void ShowSnapIcon(bool isSnappable)
	{
		this.snapIcon.SetActive(isSnappable);
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x000633F8 File Offset: 0x000615F8
	public void ShowDetachIcon(bool isDetachable)
	{
		if (!this.snapIcon.activeSelf && (this.player.transform.parent == null || this.player.transform.parent.name == "creepermount"))
		{
			this.detachIcon.SetActive(isDetachable);
		}
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00063457 File Offset: 0x00061657
	public void ShowDescriptor(string objdesc)
	{
		this.descriptor.GetComponent<Text>().text = objdesc.ToString();
	}

	// Token: 0x04001180 RID: 4480
	private string objdesc;

	// Token: 0x04001181 RID: 4481
	public GameObject defaultIcon;

	// Token: 0x04001182 RID: 4482
	public GameObject interactIcon;

	// Token: 0x04001183 RID: 4483
	public GameObject descriptor;

	// Token: 0x04001184 RID: 4484
	public GameObject food;

	// Token: 0x04001185 RID: 4485
	public GameObject water;

	// Token: 0x04001186 RID: 4486
	public GameObject sleep;

	// Token: 0x04001187 RID: 4487
	public GameObject stress;

	// Token: 0x04001188 RID: 4488
	public GameObject snapIcon;

	// Token: 0x04001189 RID: 4489
	public GameObject fsm;

	// Token: 0x0400118A RID: 4490
	public GameObject player;

	// Token: 0x0400118B RID: 4491
	private float xpos;

	// Token: 0x0400118C RID: 4492
	private float ypos;

	// Token: 0x0400118D RID: 4493
	private float xposC;

	// Token: 0x0400118E RID: 4494
	private float zposC;

	// Token: 0x0400118F RID: 4495
	private float rposC;

	// Token: 0x04001190 RID: 4496
	public GameObject detachIcon;
}
