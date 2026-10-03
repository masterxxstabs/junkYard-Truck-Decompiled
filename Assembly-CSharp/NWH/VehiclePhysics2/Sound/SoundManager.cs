using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Sound.SoundComponents;
using UnityEngine;
using UnityEngine.Audio;

namespace NWH.VehiclePhysics2.Sound
{
	// Token: 0x02000267 RID: 615
	[Serializable]
	public class SoundManager : VehicleComponent
	{
		// Token: 0x0600102F RID: 4143 RVA: 0x000BA5F0 File Offset: 0x000B87F0
		public override void Initialize()
		{
			if (this.mixer == null)
			{
				this.mixer = (Resources.Load("Sound/VehicleAudioMixer") as AudioMixer);
			}
			if (this.mixer != null)
			{
				this.masterGroup = this.mixer.FindMatchingGroups("Master")[0];
				this.engineMixerGroup = this.mixer.FindMatchingGroups("Engine")[0];
				this.transmissionMixerGroup = this.mixer.FindMatchingGroups("Transmission")[0];
				this.surfaceNoiseMixerGroup = this.mixer.FindMatchingGroups("SurfaceNoise")[0];
				this.turboMixerGroup = this.mixer.FindMatchingGroups("Turbo")[0];
				this.suspensionMixerGroup = this.mixer.FindMatchingGroups("Suspension")[0];
				this.crashMixerGroup = this.mixer.FindMatchingGroups("Crash")[0];
				this.otherMixerGroup = this.mixer.FindMatchingGroups("Other")[0];
			}
			this.mixer.GetFloat("attenuation", out this.originalAttenuation);
			this.engineStartComponent.audioMixerGroup = this.engineMixerGroup;
			this.engineStartComponent.container = this.engineSourceGO;
			this.engineStartComponent.Initialize();
			this.engineRunningComponent.audioMixerGroup = this.engineMixerGroup;
			this.engineRunningComponent.container = this.engineSourceGO;
			this.engineRunningComponent.Initialize();
			this.engineFanComponent.audioMixerGroup = this.engineMixerGroup;
			this.engineFanComponent.container = this.engineSourceGO;
			this.engineFanComponent.Initialize();
			this.turboWhistleComponent.audioMixerGroup = this.turboMixerGroup;
			this.turboWhistleComponent.container = this.engineSourceGO;
			this.turboWhistleComponent.Initialize();
			this.turboFlutterComponent.audioMixerGroup = this.turboMixerGroup;
			this.turboFlutterComponent.container = this.engineSourceGO;
			this.turboFlutterComponent.Initialize();
			this.transmissionWhineComponent.audioMixerGroup = this.transmissionMixerGroup;
			this.transmissionWhineComponent.container = this.transmissionSourceGO;
			this.transmissionWhineComponent.Initialize();
			this.gearChangeComponent.audioMixerGroup = this.transmissionMixerGroup;
			this.gearChangeComponent.container = this.transmissionSourceGO;
			this.gearChangeComponent.Initialize();
			this.brakeHissComponent.audioMixerGroup = this.otherMixerGroup;
			this.brakeHissComponent.container = this.otherSourceGO;
			this.brakeHissComponent.Initialize();
			this.blinkerComponent.audioMixerGroup = this.otherMixerGroup;
			this.blinkerComponent.container = this.otherSourceGO;
			this.blinkerComponent.Initialize();
			this.hornComponent.audioMixerGroup = this.otherMixerGroup;
			this.hornComponent.container = this.otherSourceGO;
			this.hornComponent.Initialize();
			this.wheelSkidComponent.audioMixerGroup = this.surfaceNoiseMixerGroup;
			this.wheelSkidComponent.container = this.otherSourceGO;
			this.wheelSkidComponent.Initialize();
			this.wheelTireNoiseComponent.audioMixerGroup = this.surfaceNoiseMixerGroup;
			this.wheelTireNoiseComponent.container = this.otherSourceGO;
			this.wheelTireNoiseComponent.Initialize();
			this.crashComponent.audioMixerGroup = this.crashMixerGroup;
			this.crashComponent.container = this.crashSourceGO;
			this.crashComponent.Initialize();
			this.suspensionBumpComponent.audioMixerGroup = this.suspensionMixerGroup;
			this.suspensionBumpComponent.container = this.otherSourceGO;
			this.suspensionBumpComponent.Initialize();
			this.reverseBeepComponent.audioMixerGroup = this.otherMixerGroup;
			this.reverseBeepComponent.container = this.otherSourceGO;
			this.reverseBeepComponent.Initialize();
			this.initialized = true;
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x000BA9B4 File Offset: 0x000B8BB4
		public override void Awake(VehicleController vc)
		{
			base.Awake(vc);
			this.GetComponentsList(ref this.components);
			foreach (SoundComponent soundComponent in this.components)
			{
				soundComponent.Awake(vc);
			}
			this.CreateSourceGO("EngineAudioSources", vc.enginePosition, vc.transform, ref this.engineSourceGO);
			this.CreateSourceGO("TransmissionAudioSources", vc.transmissionPosition, vc.transform, ref this.transmissionSourceGO);
			this.CreateSourceGO("ExhaustAudioSources", vc.exhaustPosition, vc.transform, ref this.exhaustSourceGO);
			this.CreateSourceGO("CrashAudioSources", Vector3.zero, vc.transform, ref this.crashSourceGO);
			this.CreateSourceGO("OtherAudioSources", new Vector3(0f, 0.2f, 0f), vc.transform, ref this.otherSourceGO);
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x000BAAB8 File Offset: 0x000B8CB8
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (!this.wasInsideVehicle && this.insideVehicle)
			{
				this.mixer.SetFloat("attenuation", this.interiorAttenuation);
				this.mixer.SetFloat("lowPassFrequency", this.lowPassFrequency);
				this.mixer.SetFloat("lowPassQ", this.lowPassQ);
			}
			else if (this.wasInsideVehicle && !this.insideVehicle)
			{
				this.mixer.SetFloat("attenuation", this.originalAttenuation);
				this.mixer.SetFloat("lowPassFrequency", 22000f);
				this.mixer.SetFloat("lowPassQ", 1f);
			}
			this.wasInsideVehicle = this.insideVehicle;
			foreach (SoundComponent soundComponent in this.components)
			{
				soundComponent.Update();
			}
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x000BABC8 File Offset: 0x000B8DC8
		public override void OnDrawGizmosSelected(VehicleController vc)
		{
			base.OnDrawGizmosSelected(vc);
			Gizmos.color = Color.white;
			if (this.components == null || this.components.Count == 0)
			{
				this.GetComponentsList(ref this.components);
			}
			foreach (SoundComponent soundComponent in this.components)
			{
				soundComponent.OnDrawGizmosSelected(vc);
			}
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x000BAC4C File Offset: 0x000B8E4C
		public void CreateSourceGO(string name, Vector3 localPosition, Transform parent, ref GameObject sourceGO)
		{
			sourceGO = new GameObject();
			sourceGO.name = name;
			sourceGO.transform.SetParent(parent);
			sourceGO.transform.localPosition = localPosition;
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x000BAC7C File Offset: 0x000B8E7C
		public override void CheckState(int lodIndex)
		{
			base.CheckState(lodIndex);
			foreach (SoundComponent soundComponent in this.components)
			{
				soundComponent.CheckState(lodIndex);
			}
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x000BACD4 File Offset: 0x000B8ED4
		public void SetAudioSourceDefaults(AudioSource audioSource, bool play = false, bool loop = false, float volume = 0f, AudioClip clip = null)
		{
			if (audioSource != null)
			{
				audioSource.spatialBlend = this.spatialBlend;
				audioSource.playOnAwake = play;
				audioSource.loop = loop;
				audioSource.volume = volume * this.vc.soundManager.masterVolume;
				audioSource.clip = clip;
				audioSource.priority = 200;
				if (play)
				{
					if (!audioSource.isPlaying)
					{
						audioSource.Play();
						return;
					}
				}
				else if (audioSource.isPlaying)
				{
					audioSource.Stop();
					return;
				}
			}
			else
			{
				Debug.LogWarning("AudioSource is null. Defaults cannot be set.");
			}
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x000BAD5C File Offset: 0x000B8F5C
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			if (this.mixer == null)
			{
				this.mixer = Resources.Load<AudioMixer>("NWH Vehicle Physics/Defaults/Sound/VehicleAudioMixer");
				if (this.mixer == null)
				{
					Debug.LogWarning("VehicleAudioMixer resource could not be loaded from resources.");
				}
			}
			this.GetComponentsList(ref this.components);
			foreach (SoundComponent soundComponent in this.components)
			{
				soundComponent.SetDefaults(vc);
			}
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x000BADF8 File Offset: 0x000B8FF8
		public override void Validate(VehicleController vc)
		{
			if (this.mixer == null)
			{
				Debug.LogError("Audio mixer of 'SoundManager' is not assigned.");
			}
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x000BAE14 File Offset: 0x000B9014
		private void GetComponentsList(ref List<SoundComponent> components)
		{
			components = new List<SoundComponent>
			{
				this.engineStartComponent,
				this.engineRunningComponent,
				this.engineFanComponent,
				this.turboWhistleComponent,
				this.turboFlutterComponent,
				this.transmissionWhineComponent,
				this.gearChangeComponent,
				this.brakeHissComponent,
				this.blinkerComponent,
				this.hornComponent,
				this.wheelSkidComponent,
				this.wheelTireNoiseComponent,
				this.crashComponent,
				this.suspensionBumpComponent,
				this.reverseBeepComponent
			};
		}

		// Token: 0x040020AE RID: 8366
		[Tooltip("Tick-tock sound of a working blinker. First clip is played when blinker is turning on and second clip is played when blinker is turning off.")]
		public BlinkerComponent blinkerComponent = new BlinkerComponent();

		// Token: 0x040020AF RID: 8367
		[Tooltip("    Sound of air brakes releasing air. Supports multiple clips.")]
		public BrakeHissComponent brakeHissComponent = new BrakeHissComponent();

		// Token: 0x040020B0 RID: 8368
		[Tooltip("List of all SoundComponents.\r\nEmpty before the sound manager is initialized.\r\nIf using external sound components add them to this list so they get updated.")]
		public List<SoundComponent> components = new List<SoundComponent>();

		// Token: 0x040020B1 RID: 8369
		[Tooltip("    Sound of vehicle hitting other objects. Supports multiple clips.")]
		public CrashComponent crashComponent = new CrashComponent();

		// Token: 0x040020B2 RID: 8370
		[Tooltip("    Mixer group for crash sound effects.")]
		public AudioMixerGroup crashMixerGroup;

		// Token: 0x040020B3 RID: 8371
		[Tooltip("    GameObject containing all the crash audio sources.")]
		public GameObject crashSourceGO;

		// Token: 0x040020B4 RID: 8372
		public EngineFanComponent engineFanComponent = new EngineFanComponent();

		// Token: 0x040020B5 RID: 8373
		public AudioMixerGroup engineMixerGroup;

		// Token: 0x040020B6 RID: 8374
		[Tooltip("    Sound of engine idling.")]
		public EngineRunningComponent engineRunningComponent = new EngineRunningComponent();

		// Token: 0x040020B7 RID: 8375
		[Tooltip("    GameObject containing all the engine audio sources.")]
		public GameObject engineSourceGO;

		// Token: 0x040020B8 RID: 8376
		[Tooltip("    Engine start / stop component. First clip is for starting and second one is for stopping.")]
		public EngineStartComponent engineStartComponent = new EngineStartComponent();

		// Token: 0x040020B9 RID: 8377
		[Tooltip("    GameObject containing all the exhaust audio sources.")]
		public GameObject exhaustSourceGO;

		// Token: 0x040020BA RID: 8378
		[Tooltip("    Sound from changing gears. Supports multiple clips.")]
		public GearChangeComponent gearChangeComponent = new GearChangeComponent();

		// Token: 0x040020BB RID: 8379
		[Tooltip("Horn sound.")]
		public HornComponent hornComponent = new HornComponent();

		// Token: 0x040020BC RID: 8380
		[Tooltip("    Set to true if listener inside vehicle. Mixer must be set up.")]
		public bool insideVehicle;

		// Token: 0x040020BD RID: 8381
		[Tooltip("    Sound attenuation inside vehicle.")]
		public float interiorAttenuation = -7f;

		// Token: 0x040020BE RID: 8382
		public float lowPassFrequency = 1600f;

		// Token: 0x040020BF RID: 8383
		[Range(0.01f, 10f)]
		public float lowPassQ = 1f;

		// Token: 0x040020C0 RID: 8384
		public AudioMixerGroup masterGroup;

		// Token: 0x040020C1 RID: 8385
		[Range(0f, 2f)]
		[Tooltip("    Master volume of a vehicle. To adjust volume of all vehicles or their components check audio mixer.")]
		public float masterVolume = 1f;

		// Token: 0x040020C2 RID: 8386
		[Tooltip("    Optional custom mixer. If left empty default will be used (VehicleAudioMixer in Resources folder).")]
		public AudioMixer mixer;

		// Token: 0x040020C3 RID: 8387
		public AudioMixerGroup otherMixerGroup;

		// Token: 0x040020C4 RID: 8388
		[Tooltip("    GameObject containing all other audio sources.")]
		public GameObject otherSourceGO;

		// Token: 0x040020C5 RID: 8389
		public ReverseBeepComponent reverseBeepComponent = new ReverseBeepComponent();

		// Token: 0x040020C6 RID: 8390
		[Range(0f, 1f)]
		[Tooltip("    Spatial blend of all audio sources. Can not be changed at runtime.")]
		public float spatialBlend = 0.9f;

		// Token: 0x040020C7 RID: 8391
		public AudioMixerGroup surfaceNoiseMixerGroup;

		// Token: 0x040020C8 RID: 8392
		[Tooltip("    Sound from wheels hitting ground and/or obstracles. Supports multiple clips.")]
		public SuspensionBumpComponent suspensionBumpComponent = new SuspensionBumpComponent();

		// Token: 0x040020C9 RID: 8393
		public AudioMixerGroup suspensionMixerGroup;

		// Token: 0x040020CA RID: 8394
		public AudioMixerGroup transmissionMixerGroup;

		// Token: 0x040020CB RID: 8395
		[Tooltip("    GameObject containing all transmission audio sources.")]
		public GameObject transmissionSourceGO;

		// Token: 0x040020CC RID: 8396
		[Tooltip("    Transmission whine from straight cut gears or just a noisy gearbox.")]
		public TransmissionWhineComponent transmissionWhineComponent = new TransmissionWhineComponent();

		// Token: 0x040020CD RID: 8397
		[Tooltip("    Sound of turbo's wastegate. Supports multiple clips.")]
		public TurboFlutterComponent turboFlutterComponent = new TurboFlutterComponent();

		// Token: 0x040020CE RID: 8398
		public AudioMixerGroup turboMixerGroup;

		// Token: 0x040020CF RID: 8399
		[Tooltip("Forced induction whistle component. Can be used for air intake noise or supercharger if spool up time is set to 0 under engine settings.")]
		public TurboWhistleComponent turboWhistleComponent = new TurboWhistleComponent();

		// Token: 0x040020D0 RID: 8400
		[Tooltip("    Sound produced by wheel skidding over a surface. Tire squeal.")]
		public WheelSkidComponent wheelSkidComponent = new WheelSkidComponent();

		// Token: 0x040020D1 RID: 8401
		[Tooltip("    Sound produced by wheel rolling over a surface. Tire hum.")]
		public WheelTireNoiseComponent wheelTireNoiseComponent = new WheelTireNoiseComponent();

		// Token: 0x040020D2 RID: 8402
		private float originalAttenuation;

		// Token: 0x040020D3 RID: 8403
		private bool wasInsideVehicle;
	}
}
