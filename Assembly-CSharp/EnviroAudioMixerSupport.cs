using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000096 RID: 150
[AddComponentMenu("Enviro/Utility/Audio Mixer Support")]
public class EnviroAudioMixerSupport : MonoBehaviour
{
	// Token: 0x06000342 RID: 834 RVA: 0x0001DDDC File Offset: 0x0001BFDC
	private void Start()
	{
		if (this.audioMixer != null && EnviroSkyMgr.instance != null)
		{
			base.StartCoroutine(this.Setup());
		}
	}

	// Token: 0x06000343 RID: 835 RVA: 0x0001DE06 File Offset: 0x0001C006
	private IEnumerator Setup()
	{
		yield return 0;
		if (EnviroSkyMgr.instance.IsStarted())
		{
			if (this.ambientMixerGroup != "")
			{
				EnviroSkyMgr.instance.AudioSettings.AudioSourceAmbient.audiosrc.outputAudioMixerGroup = this.audioMixer.FindMatchingGroups(this.ambientMixerGroup)[0];
				EnviroSkyMgr.instance.AudioSettings.AudioSourceAmbient2.audiosrc.outputAudioMixerGroup = this.audioMixer.FindMatchingGroups(this.ambientMixerGroup)[0];
			}
			if (this.weatherMixerGroup != "")
			{
				EnviroSkyMgr.instance.AudioSettings.AudioSourceWeather.audiosrc.outputAudioMixerGroup = this.audioMixer.FindMatchingGroups(this.weatherMixerGroup)[0];
				EnviroSkyMgr.instance.AudioSettings.AudioSourceWeather2.audiosrc.outputAudioMixerGroup = this.audioMixer.FindMatchingGroups(this.weatherMixerGroup)[0];
			}
			if (this.thunderMixerGroup != "")
			{
				EnviroSkyMgr.instance.AudioSettings.AudioSourceThunder.audiosrc.outputAudioMixerGroup = this.audioMixer.FindMatchingGroups(this.thunderMixerGroup)[0];
			}
		}
		else
		{
			base.StartCoroutine(this.Setup());
		}
		yield break;
	}

	// Token: 0x0400074A RID: 1866
	[Header("Mixer")]
	public AudioMixer audioMixer;

	// Token: 0x0400074B RID: 1867
	[Header("Group Names")]
	public string ambientMixerGroup;

	// Token: 0x0400074C RID: 1868
	public string weatherMixerGroup;

	// Token: 0x0400074D RID: 1869
	public string thunderMixerGroup;
}
