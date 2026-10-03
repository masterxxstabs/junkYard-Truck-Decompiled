using System;
using System.Collections;
using Michsky.UI.Dark;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x020000EF RID: 239
public class MainMenu : MonoBehaviour
{
	// Token: 0x060005E1 RID: 1505 RVA: 0x000487F6 File Offset: 0x000469F6
	public void Start()
	{
		this.LoadText.SetActive(false);
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00048804 File Offset: 0x00046A04
	public void PlayGame()
	{
		PlayerPrefs.SetInt("LoadSlot", 0);
		AudioListener.pause = false;
		Time.timeScale = 1f;
		this.loadingScreen.SetActive(true);
		base.StartCoroutine(this.LoadLevelAsync());
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x0004883C File Offset: 0x00046A3C
	public void PlayGameOverwrite()
	{
		ES3.DeleteFile("JY.es3");
		ES3.DeleteFile("JY.es3", new ES3Settings(new Enum[]
		{
			ES3.Location.File
		}));
		AudioListener.pause = false;
		Time.timeScale = 1f;
		this.loadingScreen.SetActive(true);
		base.StartCoroutine(this.LoadLevelAsync());
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x0004889A File Offset: 0x00046A9A
	public void Continue()
	{
		AudioListener.pause = false;
		Time.timeScale = 1f;
		this.loadingScreen.SetActive(true);
		base.StartCoroutine(this.LoadLevelAsync());
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x000488C5 File Offset: 0x00046AC5
	public void Load2()
	{
		this.blur.BlurInAnim();
		this.slotsModal.ModalWindowIn();
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x000488DD File Offset: 0x00046ADD
	public void DeleteConfirm(int slot)
	{
		this.slotToDelete = slot;
		this.blur.BlurInAnim();
		this.deleteConfirm.ModalWindowIn();
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x000488FC File Offset: 0x00046AFC
	public void DeleteSlot()
	{
		if (this.slotToDelete == 1)
		{
			ES3.DeleteFile("JY.es3");
			ES3.DeleteFile("JY.es3", new ES3Settings(new Enum[]
			{
				ES3.Location.File
			}));
		}
		if (this.slotToDelete == 2)
		{
			ES3.DeleteFile("JY2.es3");
			ES3.DeleteFile("JY2.es3", new ES3Settings(new Enum[]
			{
				ES3.Location.File
			}));
		}
		if (this.slotToDelete == 3)
		{
			ES3.DeleteFile("JY3.es3");
			ES3.DeleteFile("JY3.es3", new ES3Settings(new Enum[]
			{
				ES3.Location.File
			}));
		}
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0004899C File Offset: 0x00046B9C
	private IEnumerator LoadLevelAsync()
	{
		AsyncOperation loadOperation = SceneManager.LoadSceneAsync(1);
		while (!loadOperation.isDone)
		{
			float value = Mathf.Clamp01(loadOperation.progress / 0.9f);
			this.loadingSlider.value = value;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x000489AB File Offset: 0x00046BAB
	public void OptionSave()
	{
		ES3AutoSaveMgr.Current.Save1();
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x000489B7 File Offset: 0x00046BB7
	public void OptionSave2()
	{
		ES3AutoSaveMgr.Current.Save2();
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x000489C3 File Offset: 0x00046BC3
	public void OptionSave3()
	{
		ES3AutoSaveMgr.Current.Save3();
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x000489CF File Offset: 0x00046BCF
	public void OptionSaveAuto()
	{
		ES3AutoSaveMgr.Current.SaveAuto();
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x000489DC File Offset: 0x00046BDC
	public void GoToMainMenu()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x000489FD File Offset: 0x00046BFD
	public void OptionCancel()
	{
		Time.timeScale = 1f;
		this.fpc.enabled = true;
		this.fpc.LockMouse();
		this.exitPanel.SetActive(false);
		AudioListener.pause = false;
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00048A32 File Offset: 0x00046C32
	public void QuitGame()
	{
		Time.timeScale = 1f;
		SteamClient.Shutdown();
		Application.Quit();
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x00048A48 File Offset: 0x00046C48
	public void QuitToMenu()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}

	// Token: 0x04000CE8 RID: 3304
	public GameObject LoadText;

	// Token: 0x04000CE9 RID: 3305
	public FirstPersonController fpc;

	// Token: 0x04000CEA RID: 3306
	public BlurManager blur;

	// Token: 0x04000CEB RID: 3307
	public ModalWindowManager overwriteModal;

	// Token: 0x04000CEC RID: 3308
	public ModalWindowManager slotsModal;

	// Token: 0x04000CED RID: 3309
	public GameObject exitPanel;

	// Token: 0x04000CEE RID: 3310
	public GameObject loadOptions;

	// Token: 0x04000CEF RID: 3311
	public ModalWindowManager deleteConfirm;

	// Token: 0x04000CF0 RID: 3312
	private int slotToDelete;

	// Token: 0x04000CF1 RID: 3313
	[SerializeField]
	private GameObject loadingScreen;

	// Token: 0x04000CF2 RID: 3314
	[SerializeField]
	private GameObject mainMenu;

	// Token: 0x04000CF3 RID: 3315
	[SerializeField]
	private Slider loadingSlider;
}
