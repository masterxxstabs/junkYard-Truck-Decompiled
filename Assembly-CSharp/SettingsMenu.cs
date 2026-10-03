using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Token: 0x02000139 RID: 313
public class SettingsMenu : MonoBehaviour
{
	// Token: 0x0600081A RID: 2074 RVA: 0x0006C604 File Offset: 0x0006A804
	private void Start()
	{
		this.resolutions = Screen.resolutions;
		this.resolutionDropdown.ClearOptions();
		List<string> list = new List<string>();
		int value = 0;
		for (int i = 0; i < this.resolutions.Length; i++)
		{
			string item = this.resolutions[i].width + " x " + this.resolutions[i].height;
			list.Add(item);
			if (this.resolutions[i].width == Screen.currentResolution.width && this.resolutions[i].height == Screen.currentResolution.height)
			{
				value = i;
			}
		}
		this.resolutionDropdown.AddOptions(list);
		this.resolutionDropdown.value = value;
		this.resolutionDropdown.RefreshShownValue();
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0006C6F0 File Offset: 0x0006A8F0
	public void SetResolution(int resolutionIndex)
	{
		Resolution resolution = this.resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0006C722 File Offset: 0x0006A922
	public void SetVolume(float volume)
	{
		this.audioMixer.SetFloat("volume", volume);
		this.audioMixer2.SetFloat("volume", volume);
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0006C748 File Offset: 0x0006A948
	public void SetQuality(int qualityIndex)
	{
		QualitySettings.SetQualityLevel(qualityIndex);
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0006C750 File Offset: 0x0006A950
	public void SetFullScreen(bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;
	}

	// Token: 0x04001300 RID: 4864
	public AudioMixer audioMixer;

	// Token: 0x04001301 RID: 4865
	public AudioMixer audioMixer2;

	// Token: 0x04001302 RID: 4866
	public Dropdown resolutionDropdown;

	// Token: 0x04001303 RID: 4867
	private Resolution[] resolutions;
}
