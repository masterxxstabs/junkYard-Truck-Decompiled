using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Michsky.UI.Dark
{
	// Token: 0x02000320 RID: 800
	public class QualityManager : MonoBehaviour
	{
		// Token: 0x06001476 RID: 5238 RVA: 0x000DAF40 File Offset: 0x000D9140
		private void Start()
		{
			this.mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat(this.masterSlider.sliderTag + "DarkSliderValue")) * 20f);
			if (!this.isMobile)
			{
				this.resolutionSelector.dropdownItems.RemoveRange(0, this.resolutionSelector.dropdownItems.Count);
				this.resolutions = Screen.resolutions;
				for (int i = 0; i < this.resolutions.Length; i++)
				{
					string item = this.resolutions[i].width + " x " + this.resolutions[i].height;
					this.options.Add(item);
					if (this.resolutions[i].width == Screen.currentResolution.width && this.resolutions[i].height == Screen.currentResolution.height)
					{
						int num = i;
						this.resolutionSelector.selectedItemIndex = num;
						this.resolutionSelector.index = num;
						this.resOverride = i;
					}
					this.resolutionSelector.CreateNewOption(this.options[i]);
					CustomDropdown.Item item2 = this.resolutionSelector.dropdownItems[i];
					item2.OnItemSelection = new UnityEvent();
					item2.OnItemSelection.AddListener(new UnityAction(this.UpdateResolution));
				}
				this.resolutionSelector.SetupDropdown();
			}
			if (SceneManager.GetActiveScene().buildIndex == 0)
			{
				int @int = PlayerPrefs.GetInt("MyResolution");
				if (PlayerPrefs.HasKey("MyResolution"))
				{
					if (PlayerPrefs.GetInt("MyResolution") != 0)
					{
						this.SetResolution(@int);
						base.StartCoroutine(this.ResFix(@int));
					}
				}
				else
				{
					int num2 = this.resolutionSelector.dropdownItems.Count - 1;
					if (num2 != 0)
					{
						PlayerPrefs.SetInt("MyResolution", num2);
					}
					this.SetResolution(num2);
					base.StartCoroutine(this.ResFix(num2));
				}
			}
			this.lastRes = this.resolutionSelector.index;
			PlayerPrefs.GetInt("antiAliasing");
			this.AntiAlisasingSet(PlayerPrefs.GetInt("antiAliasing"));
			if (PlayerPrefs.GetInt("antiAliasing") == 0)
			{
				this.AliasDD.ChangeDropdownInfo(0);
			}
			else if (PlayerPrefs.GetInt("antiAliasing") == 2)
			{
				this.AliasDD.ChangeDropdownInfo(1);
			}
			else if (PlayerPrefs.GetInt("antiAliasing") == 4)
			{
				this.AliasDD.ChangeDropdownInfo(2);
			}
			else if (PlayerPrefs.GetInt("antiAliasing") == 8)
			{
				this.AliasDD.ChangeDropdownInfo(3);
			}
			PlayerPrefs.GetInt("shadowResolution");
			this.ShadowResolutionSet(PlayerPrefs.GetInt("shadowResolution"));
			this.ShadresDD.ChangeDropdownInfo(PlayerPrefs.GetInt("shadowResolution"));
			PlayerPrefs.GetInt("textureQuality");
			this.TextureSet(PlayerPrefs.GetInt("textureQuality"));
			this.TexDD.ChangeDropdownInfo(PlayerPrefs.GetInt("textureQuality"));
			int int2 = PlayerPrefs.GetInt("textureQuality");
			this.TexDD.ChangeDropdownInfo(int2);
			this.VsyncSet(1);
			this.ShadowsCascasedSet(0);
			PlayerPrefs.GetInt("reflectionsSet");
			this.ReflectionSet(PlayerPrefs.GetInt("reflectionsSet"));
			this.ReflDD.ChangeDropdownInfo(PlayerPrefs.GetInt("reflectionsSet"));
			PlayerPrefs.GetInt("aniso");
			this.AnisoDD.ChangeDropdownInfo(PlayerPrefs.GetInt("aniso"));
			if (PlayerPrefs.GetInt("aniso") == 1)
			{
				this.AnisotrpicFilteringEnable();
				return;
			}
			this.AnisotrpicFilteringDisable();
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x000DB2DB File Offset: 0x000D94DB
		private IEnumerator ResFix(int maxResTitle)
		{
			yield return new WaitForSeconds(0.1f);
			this.SetResolution(maxResTitle);
			Screen.SetResolution(this.resolutions[maxResTitle].width, this.resolutions[maxResTitle].height, Screen.fullScreen);
			yield break;
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x000DB2F4 File Offset: 0x000D94F4
		public void UpdateResolution()
		{
			this.clickEvent.Invoke(this.resolutionSelector.index);
			this.resolutionSelector.UpdateValues();
			base.StartCoroutine(this.FixResolution());
			this.SetResolution(this.resolutionSelector.index);
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x000DB340 File Offset: 0x000D9540
		private IEnumerator FixResolution()
		{
			yield return new WaitForSeconds(0.1f);
			this.clickEvent.Invoke(this.resolutionSelector.index);
			base.StopCoroutine("FixResolution");
			yield break;
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x000DB350 File Offset: 0x000D9550
		public void SetResolution(int resolutionIndex)
		{
			PlayerPrefs.GetInt("ResolutionDropdown");
			if (resolutionIndex != this.lastRes)
			{
				Screen.SetResolution(this.resolutions[resolutionIndex].width, this.resolutions[resolutionIndex].height, Screen.fullScreen);
				this.lastRes = resolutionIndex;
			}
			PlayerPrefs.SetInt("ResolutionDropdown", resolutionIndex);
			if (resolutionIndex != 0)
			{
				PlayerPrefs.SetInt("MyResolution", resolutionIndex);
			}
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x000DB3BF File Offset: 0x000D95BF
		public void AnisotrpicFilteringEnable()
		{
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			PlayerPrefs.SetInt("aniso", 1);
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x000DB3D2 File Offset: 0x000D95D2
		public void AnisotrpicFilteringDisable()
		{
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
			PlayerPrefs.SetInt("aniso", 0);
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x000DB3E5 File Offset: 0x000D95E5
		public void AntiAlisasingSet(int index)
		{
			QualitySettings.antiAliasing = index;
			PlayerPrefs.SetInt("antiAliasing", index);
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x000DB3F8 File Offset: 0x000D95F8
		public void VsyncSet(int index)
		{
			QualitySettings.vSyncCount = index;
			Application.targetFrameRate = 60;
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x000DB407 File Offset: 0x000D9607
		public void ShadowResolutionSet(int index)
		{
			PlayerPrefs.SetInt("shadowResolution", index);
			if (index == 3)
			{
				QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
				return;
			}
			if (index == 2)
			{
				QualitySettings.shadowResolution = ShadowResolution.High;
				return;
			}
			if (index == 1)
			{
				QualitySettings.shadowResolution = ShadowResolution.Medium;
				return;
			}
			if (index == 0)
			{
				QualitySettings.shadowResolution = ShadowResolution.Low;
			}
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x000DB43E File Offset: 0x000D963E
		public void ShadowsSet(int index)
		{
			PlayerPrefs.SetInt("shadowSet", index);
			if (index == 0)
			{
				QualitySettings.shadows = ShadowQuality.Disable;
				return;
			}
			if (index == 1)
			{
				QualitySettings.shadows = ShadowQuality.All;
			}
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x000DB45F File Offset: 0x000D965F
		public void ShadowsCascasedSet(int index)
		{
			QualitySettings.shadowCascades = 0;
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x000DB467 File Offset: 0x000D9667
		public void TextureSet(int index)
		{
			QualitySettings.masterTextureLimit = index;
			PlayerPrefs.SetInt("textureQuality", index);
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x000DB47A File Offset: 0x000D967A
		public void SoftParticleSet(int index)
		{
			if (index == 0)
			{
				QualitySettings.softParticles = false;
				return;
			}
			if (index == 1)
			{
				QualitySettings.softParticles = true;
			}
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x000DB490 File Offset: 0x000D9690
		public void ReflectionSet(int index)
		{
			PlayerPrefs.SetInt("reflectionsSet", index);
			if (index == 0)
			{
				QualitySettings.realtimeReflectionProbes = false;
				return;
			}
			if (index == 1)
			{
				QualitySettings.realtimeReflectionProbes = true;
			}
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x000DB4B1 File Offset: 0x000D96B1
		public void VolumeSetMaster(float volume)
		{
			this.mixer.SetFloat("Master", Mathf.Log10(volume) * 20f);
			AudioListener.volume = volume;
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x000DB4D6 File Offset: 0x000D96D6
		public void VolumeSetMusic(float volume)
		{
			this.mixer.SetFloat("Music", Mathf.Log10(volume) * 20f);
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x000DB4F5 File Offset: 0x000D96F5
		public void VolumeSetSFX(float volume)
		{
			this.mixer.SetFloat("SFX", Mathf.Log10(volume) * 20f);
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x0006C748 File Offset: 0x0006A948
		public void SetOverallQuality(int qualityIndex)
		{
			QualitySettings.SetQualityLevel(qualityIndex);
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x000DB514 File Offset: 0x000D9714
		public void WindowFullscreen()
		{
			Screen.fullScreen = true;
			Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x000DB522 File Offset: 0x000D9722
		public void WindowBorderless()
		{
			Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x000DB52A File Offset: 0x000D972A
		public void WindowWindowed()
		{
			Screen.fullScreen = false;
			Screen.fullScreenMode = FullScreenMode.Windowed;
		}

		// Token: 0x040024E1 RID: 9441
		[Header("AUDIO")]
		public AudioMixer mixer;

		// Token: 0x040024E2 RID: 9442
		public SliderManager masterSlider;

		// Token: 0x040024E3 RID: 9443
		public SliderManager musicSlider;

		// Token: 0x040024E4 RID: 9444
		public SliderManager sfxSlider;

		// Token: 0x040024E5 RID: 9445
		[Header("RESOLUTION")]
		public CustomDropdown resolutionSelector;

		// Token: 0x040024E6 RID: 9446
		public QualityManager.DynamicRes clickEvent;

		// Token: 0x040024E7 RID: 9447
		[Header("SETTINGS")]
		public bool isMobile;

		// Token: 0x040024E8 RID: 9448
		private Resolution[] resolutions;

		// Token: 0x040024E9 RID: 9449
		private List<string> options = new List<string>();

		// Token: 0x040024EA RID: 9450
		private int resOverride;

		// Token: 0x040024EB RID: 9451
		private int lastRes;

		// Token: 0x040024EC RID: 9452
		public CustomDropdown TexDD;

		// Token: 0x040024ED RID: 9453
		public CustomDropdown AliasDD;

		// Token: 0x040024EE RID: 9454
		public CustomDropdown ShadresDD;

		// Token: 0x040024EF RID: 9455
		public CustomDropdown ShadDD;

		// Token: 0x040024F0 RID: 9456
		public CustomDropdown ReflDD;

		// Token: 0x040024F1 RID: 9457
		public CustomDropdown AnisoDD;

		// Token: 0x020004E7 RID: 1255
		[Serializable]
		public class DynamicRes : UnityEvent<int>
		{
		}
	}
}
