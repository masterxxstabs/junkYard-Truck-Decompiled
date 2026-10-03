using System;
using UnityEngine;

// Token: 0x02000097 RID: 151
public class EnviroAudioSource : MonoBehaviour
{
	// Token: 0x06000345 RID: 837 RVA: 0x0001DE18 File Offset: 0x0001C018
	private void Start()
	{
		if (EnviroSkyMgr.instance == null)
		{
			Debug.Log("EnviroSky Manager not found. Deactivate enviro AudioSource");
			base.enabled = false;
			return;
		}
		if (this.audiosrc == null)
		{
			this.audiosrc = base.GetComponent<AudioSource>();
		}
		if (this.myFunction == EnviroAudioSource.AudioSourceFunction.Weather1 || this.myFunction == EnviroAudioSource.AudioSourceFunction.Weather2)
		{
			this.audiosrc.loop = true;
			this.audiosrc.volume = 0f;
		}
		this.currentAmbientVolume = EnviroSkyMgr.instance.ambientAudioVolume;
		this.currentWeatherVolume = EnviroSkyMgr.instance.weatherAudioVolume;
	}

	// Token: 0x06000346 RID: 838 RVA: 0x0001DEAB File Offset: 0x0001C0AB
	public void FadeOut()
	{
		this.isFadingOut = true;
		this.isFadingIn = false;
	}

	// Token: 0x06000347 RID: 839 RVA: 0x0001DEBB File Offset: 0x0001C0BB
	public void FadeIn(AudioClip clip)
	{
		this.isFadingIn = true;
		this.isFadingOut = false;
		this.audiosrc.clip = clip;
		this.audiosrc.Play();
	}

	// Token: 0x06000348 RID: 840 RVA: 0x0001DEE2 File Offset: 0x0001C0E2
	public void PlayOneShot(AudioClip clip)
	{
		this.audiosrc.loop = false;
		this.audiosrc.clip = clip;
		this.audiosrc.Play();
	}

	// Token: 0x06000349 RID: 841 RVA: 0x0001DF08 File Offset: 0x0001C108
	private void Update()
	{
		if (EnviroSkyMgr.instance == null && !EnviroSkyMgr.instance.IsStarted())
		{
			return;
		}
		this.currentAmbientVolume = Mathf.Lerp(this.currentAmbientVolume, EnviroSkyMgr.instance.ambientAudioVolume + EnviroSkyMgr.instance.ambientAudioVolumeModifier, 10f * Time.deltaTime);
		this.currentWeatherVolume = Mathf.Lerp(this.currentWeatherVolume, EnviroSkyMgr.instance.weatherAudioVolume + EnviroSkyMgr.instance.weatherAudioVolumeModifier, 10f * Time.deltaTime);
		if (this.myFunction == EnviroAudioSource.AudioSourceFunction.Weather1 || this.myFunction == EnviroAudioSource.AudioSourceFunction.Weather2 || this.myFunction == EnviroAudioSource.AudioSourceFunction.Thunder)
		{
			if (this.isFadingIn && this.audiosrc.volume < this.currentWeatherVolume)
			{
				this.audiosrc.volume += EnviroSkyMgr.instance.audioTransitionSpeed * Time.deltaTime;
			}
			else if (this.isFadingIn && this.audiosrc.volume >= this.currentWeatherVolume - 0.01f)
			{
				this.isFadingIn = false;
			}
			if (this.isFadingOut && this.audiosrc.volume > 0f)
			{
				this.audiosrc.volume -= EnviroSkyMgr.instance.audioTransitionSpeed * Time.deltaTime;
			}
			else if (this.isFadingOut && this.audiosrc.volume == 0f)
			{
				this.audiosrc.Stop();
				this.isFadingOut = false;
			}
			if (this.audiosrc.isPlaying && !this.isFadingOut && !this.isFadingIn)
			{
				this.audiosrc.volume = this.currentWeatherVolume;
				return;
			}
		}
		else if (this.myFunction == EnviroAudioSource.AudioSourceFunction.Ambient || this.myFunction == EnviroAudioSource.AudioSourceFunction.Ambient2)
		{
			if (this.isFadingIn && this.audiosrc.volume < this.currentAmbientVolume)
			{
				this.audiosrc.volume += EnviroSkyMgr.instance.audioTransitionSpeed * Time.deltaTime;
			}
			else if (this.isFadingIn && this.audiosrc.volume >= this.currentAmbientVolume - 0.01f)
			{
				this.isFadingIn = false;
			}
			if (this.isFadingOut && this.audiosrc.volume > 0f)
			{
				this.audiosrc.volume -= EnviroSkyMgr.instance.audioTransitionSpeed * Time.deltaTime;
			}
			else if (this.isFadingOut && this.audiosrc.volume == 0f)
			{
				this.audiosrc.Stop();
				this.isFadingOut = false;
			}
			if (this.audiosrc.isPlaying && !this.isFadingOut && !this.isFadingIn)
			{
				this.audiosrc.volume = this.currentAmbientVolume;
				return;
			}
		}
		else if (this.myFunction == EnviroAudioSource.AudioSourceFunction.ZoneAmbient)
		{
			if (this.isFadingIn && this.audiosrc.volume < EnviroSkyMgr.instance.interiorZoneAudioVolume)
			{
				this.audiosrc.volume += EnviroSkyMgr.instance.interiorZoneAudioFadingSpeed * Time.deltaTime;
			}
			else if (this.isFadingIn && this.audiosrc.volume >= EnviroSkyMgr.instance.interiorZoneAudioVolume - 0.01f)
			{
				this.isFadingIn = false;
			}
			if (this.isFadingOut && this.audiosrc.volume > 0f)
			{
				this.audiosrc.volume -= EnviroSkyMgr.instance.interiorZoneAudioFadingSpeed * Time.deltaTime;
			}
			else if (this.isFadingOut && this.audiosrc.volume == 0f)
			{
				this.audiosrc.Stop();
				this.isFadingOut = false;
			}
			if (this.audiosrc.isPlaying && !this.isFadingOut && !this.isFadingIn)
			{
				this.audiosrc.volume = EnviroSkyMgr.instance.interiorZoneAudioVolume;
			}
		}
	}

	// Token: 0x0400074E RID: 1870
	public EnviroAudioSource.AudioSourceFunction myFunction;

	// Token: 0x0400074F RID: 1871
	public AudioSource audiosrc;

	// Token: 0x04000750 RID: 1872
	public bool isFadingIn;

	// Token: 0x04000751 RID: 1873
	public bool isFadingOut;

	// Token: 0x04000752 RID: 1874
	private float currentAmbientVolume;

	// Token: 0x04000753 RID: 1875
	private float currentWeatherVolume;

	// Token: 0x04000754 RID: 1876
	private float currentZoneVolume;

	// Token: 0x02000398 RID: 920
	public enum AudioSourceFunction
	{
		// Token: 0x04002758 RID: 10072
		Weather1,
		// Token: 0x04002759 RID: 10073
		Weather2,
		// Token: 0x0400275A RID: 10074
		Ambient,
		// Token: 0x0400275B RID: 10075
		Ambient2,
		// Token: 0x0400275C RID: 10076
		Thunder,
		// Token: 0x0400275D RID: 10077
		ZoneAmbient
	}
}
