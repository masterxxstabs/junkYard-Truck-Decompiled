using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x020000AD RID: 173
public class ExitScript : MonoBehaviour
{
	// Token: 0x06000430 RID: 1072 RVA: 0x0002C774 File Offset: 0x0002A974
	public void ExitGame()
	{
		Application.Quit();
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x0002C77B File Offset: 0x0002A97B
	public void OptionCancel()
	{
		this.fpc.enabled = true;
		this.fpc.LockMouse();
		this.exitPanel.SetActive(false);
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00002188 File Offset: 0x00000388
	public void OptionSave()
	{
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00002188 File Offset: 0x00000388
	public void OptionLoad()
	{
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x0002C7A0 File Offset: 0x0002A9A0
	public void ExitControls()
	{
		this.fpc.enabled = true;
		this.fpc.LockMouse();
		this.controlsPanel.SetActive(false);
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x0002C7C5 File Offset: 0x0002A9C5
	public void ShowControls()
	{
		this.controlsPanel.SetActive(true);
		this.exitPanel.SetActive(false);
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x0002C7E0 File Offset: 0x0002A9E0
	public void ToMenu()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}

	// Token: 0x0400086F RID: 2159
	public FirstPersonController fpc;

	// Token: 0x04000870 RID: 2160
	public GameObject exitPanel;

	// Token: 0x04000871 RID: 2161
	public GameObject controlsPanel;
}
