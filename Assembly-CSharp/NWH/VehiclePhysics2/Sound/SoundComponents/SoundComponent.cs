using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x02000268 RID: 616
	[Serializable]
	public abstract class SoundComponent : VehicleComponent
	{
		// Token: 0x1700017C RID: 380
		// (get) Token: 0x0600103B RID: 4155 RVA: 0x000BAFD6 File Offset: 0x000B91D6
		// (set) Token: 0x0600103C RID: 4156 RVA: 0x000BAFF4 File Offset: 0x000B91F4
		public AudioClip Clip
		{
			get
			{
				if (this.clips.Count <= 0)
				{
					return null;
				}
				return this.clips[0];
			}
			set
			{
				if (this.clips.Count > 0)
				{
					this.clips[0] = value;
					return;
				}
				this.clips.Add(value);
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x0600103D RID: 4157 RVA: 0x000BB01E File Offset: 0x000B921E
		// (set) Token: 0x0600103E RID: 4158 RVA: 0x000BB026 File Offset: 0x000B9226
		public List<AudioClip> Clips
		{
			get
			{
				return this.clips;
			}
			set
			{
				this.clips = value;
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x0600103F RID: 4159 RVA: 0x000BB02F File Offset: 0x000B922F
		public AudioClip RandomClip
		{
			get
			{
				return this.clips[Random.Range(0, this.clips.Count)];
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06001040 RID: 4160 RVA: 0x000BB04D File Offset: 0x000B924D
		// (set) Token: 0x06001041 RID: 4161 RVA: 0x000BB06B File Offset: 0x000B926B
		public AudioSource Source
		{
			get
			{
				if (this.sources.Count > 0)
				{
					return this.sources[0];
				}
				return null;
			}
			set
			{
				if (this.sources.Count > 0)
				{
					this.sources[0] = value;
				}
				this.sources.Add(value);
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06001042 RID: 4162 RVA: 0x000BB094 File Offset: 0x000B9294
		// (set) Token: 0x06001043 RID: 4163 RVA: 0x000BB09C File Offset: 0x000B929C
		public List<AudioSource> Sources
		{
			get
			{
				return this.sources;
			}
			set
			{
				this.sources = value;
			}
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x000BB0A8 File Offset: 0x000B92A8
		public override void Enable()
		{
			base.Enable();
			foreach (AudioSource audioSource in this.sources)
			{
				if (!audioSource.enabled)
				{
					audioSource.enabled = true;
					if (audioSource.loop)
					{
						audioSource.Play();
					}
				}
			}
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x000BB118 File Offset: 0x000B9318
		public override void Disable()
		{
			base.Disable();
			foreach (AudioSource audioSource in this.sources)
			{
				if (audioSource.isPlaying)
				{
					this.Stop();
				}
				if (audioSource.enabled)
				{
					audioSource.enabled = false;
				}
				audioSource.volume = 0f;
			}
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x000BB194 File Offset: 0x000B9394
		public void AddSourcesToMixer()
		{
			foreach (AudioSource audioSource in this.sources)
			{
				audioSource.outputAudioMixerGroup = this.audioMixerGroup;
			}
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x000BB1EC File Offset: 0x000B93EC
		public float GetPitch()
		{
			if (!base.Active)
			{
				return 0f;
			}
			return this.Source.pitch;
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x000BB207 File Offset: 0x000B9407
		public float GetVolume()
		{
			if (!base.Active)
			{
				return 0f;
			}
			return this.Source.volume;
		}

		// Token: 0x06001049 RID: 4169 RVA: 0x000BB222 File Offset: 0x000B9422
		public void Play()
		{
			if (!base.Active)
			{
				return;
			}
			if (this.Source.isPlaying)
			{
				return;
			}
			this.Source.Play();
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x000BB248 File Offset: 0x000B9448
		public void Play(int index)
		{
			if (!base.Active)
			{
				return;
			}
			AudioSource audioSource = this.Sources[index];
			if (audioSource.isPlaying)
			{
				return;
			}
			audioSource.Play();
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x000BB27A File Offset: 0x000B947A
		public void SetPitch(float pitch, int index)
		{
			if (!base.Active)
			{
				return;
			}
			pitch = ((pitch < 0f) ? 0f : ((pitch > 5f) ? 5f : pitch));
			this.sources[index].pitch = pitch;
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x000BB2B8 File Offset: 0x000B94B8
		public void SetPitch(float pitch)
		{
			if (!base.Active)
			{
				return;
			}
			pitch = ((pitch < 0f) ? 0f : ((pitch > 5f) ? 5f : pitch));
			this.Source.pitch = pitch;
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x000BB2F0 File Offset: 0x000B94F0
		public void SetVolume(float volume, int index)
		{
			if (!base.Active)
			{
				return;
			}
			volume = ((volume < 0f) ? 0f : ((volume > 2f) ? 2f : volume));
			this.sources[index].volume = volume * this.vc.soundManager.masterVolume;
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x000BB34C File Offset: 0x000B954C
		public void SetVolume(float volume, AudioSource source)
		{
			if (!base.Active)
			{
				return;
			}
			volume = ((volume < 0f) ? 0f : ((volume > 2f) ? 2f : volume));
			source.volume = volume * this.vc.soundManager.masterVolume;
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x000BB39B File Offset: 0x000B959B
		public void SetVolume(float volume)
		{
			if (!base.Active)
			{
				return;
			}
			this.Source.volume = volume * this.vc.soundManager.masterVolume;
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x000BB3C3 File Offset: 0x000B95C3
		public void Stop()
		{
			if (!this.Source.isPlaying)
			{
				return;
			}
			this.Source.Stop();
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x000BB3E0 File Offset: 0x000B95E0
		public void Stop(int index)
		{
			AudioSource audioSource = this.Sources[index];
			if (!audioSource.isPlaying)
			{
				return;
			}
			audioSource.Stop();
		}

		// Token: 0x040020D4 RID: 8404
		[Tooltip("    AudioMixerGroup that this SoundComponent belongs to.")]
		public AudioMixerGroup audioMixerGroup;

		// Token: 0x040020D5 RID: 8405
		[FormerlySerializedAs("pitch")]
		[Range(0f, 2f)]
		[Tooltip("    Base pitch of the sound component.")]
		public float basePitch = 1f;

		// Token: 0x040020D6 RID: 8406
		[FormerlySerializedAs("volume")]
		[Range(0f, 1f)]
		[Tooltip("    Base volume of the sound component.")]
		public float baseVolume = 0.1f;

		// Token: 0x040020D7 RID: 8407
		[Tooltip("List of audio clips this component can use. Some components can use multiple clips in which case they will be chosen at random, and some components can use only one in which case only the first clip will be selected. Check manual for more details.")]
		public List<AudioClip> clips = new List<AudioClip>();

		// Token: 0x040020D8 RID: 8408
		[Tooltip("Container to which this SoundComponent belongs to. All AudioSources will be attached to this object.")]
		public GameObject container;

		// Token: 0x040020D9 RID: 8409
		[NonSerialized]
		protected List<AudioSource> sources = new List<AudioSource>();
	}
}
