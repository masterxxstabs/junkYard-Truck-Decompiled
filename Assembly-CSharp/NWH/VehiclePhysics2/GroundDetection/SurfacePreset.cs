using System;
using NWH.WheelController3D;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.GroundDetection
{
	// Token: 0x020002AA RID: 682
	[CreateAssetMenu(fileName = "NWH Vehicle Physics", menuName = "NWH Vehicle Physics/Surface Preset", order = 1)]
	[Serializable]
	public class SurfacePreset : ScriptableObject
	{
		// Token: 0x0400225E RID: 8798
		[Tooltip("Type of particles generated.\r\n- Smoke - depends on wheel slip. Use for hard surfaces. Will not emit when there is no wheel slip.\r\n- Dust - depends on speed only. Use for dusty surfaces, e.g. gravel or sand.")]
		public SurfacePreset.ParticleType particleType;

		// Token: 0x0400225F RID: 8799
		[FormerlySerializedAs("dustColor")]
		[Tooltip("    Color of generated particles on this surface type.")]
		public Color particleColor = new Color(0.9f, 0.9f, 0.9f, 0.9f);

		// Token: 0x04002260 RID: 8800
		[Range(0f, 2f)]
		[Tooltip("    Maximum amount of particles emitted over distance.")]
		public float maxParticleEmissionRateOverDistance = 0.36f;

		// Token: 0x04002261 RID: 8801
		[Tooltip("   Initial size of the emitted particles.")]
		public float particleSize = 1f;

		// Token: 0x04002262 RID: 8802
		[Tooltip("Should the particles be emitted on this surface type?")]
		public bool emitParticles = true;

		// Token: 0x04002263 RID: 8803
		[Tooltip("Should dirt chunks / stones be thrown behind the wheel on this surface type?")]
		public bool emitChunks;

		// Token: 0x04002264 RID: 8804
		[Tooltip("Maximum amount of chunks emitted over distance.")]
		public float maxChunkEmissionRateOverDistance = 1f;

		// Token: 0x04002265 RID: 8805
		[Tooltip("Determines maximum distance from the wheel that the chunk can stay alive.")]
		public float chunkLifeDistance = 3f;

		// Token: 0x04002266 RID: 8806
		[FormerlySerializedAs("maxChunkLifeTime")]
		[Tooltip("Maximum life time of an emitted chunk.")]
		public float maxChunkLifetime = 0.5f;

		// Token: 0x04002267 RID: 8807
		[Range(0f, 1f)]
		[Tooltip("Maximum alpha value start color of an emitted particle can achieve.")]
		public float particleMaxAlpha = 0.8f;

		// Token: 0x04002268 RID: 8808
		[Tooltip("Maximum particle start lifetime.")]
		public float maxParticleLifetime = 3.5f;

		// Token: 0x04002269 RID: 8809
		[Tooltip("Maximum distance from the vehicle a particle can achieve.")]
		public float particleLifeDistance = 10f;

		// Token: 0x0400226A RID: 8810
		[Tooltip("Friction preset of WC3D that will be used for this surface. More presets can be added in WheelController.FrictionPresets.")]
		public FrictionPreset frictionPreset;

		// Token: 0x0400226B RID: 8811
		[Tooltip("Name of the surface map.")]
		public new string name;

		// Token: 0x0400226C RID: 8812
		[Tooltip("    AudioClip used for wheel skidding sound effect.")]
		public AudioClip skidSoundClip;

		// Token: 0x0400226D RID: 8813
		[Tooltip("Should tire skid sounds be played for this surface type?")]
		public bool playSkidSounds = true;

		// Token: 0x0400226E RID: 8814
		[Tooltip("    Sound pitch of wheel skidding over the surface.")]
		public float skidSoundPitch = 1f;

		// Token: 0x0400226F RID: 8815
		[Tooltip("    Sound volume of wheel skidding over the surface.")]
		public float skidSoundVolume = 0.3f;

		// Token: 0x04002270 RID: 8816
		[Range(0f, 1f)]
		public float slipFactor = 0.5f;

		// Token: 0x04002271 RID: 8817
		[FormerlySerializedAs("slipSensitiveSound")]
		[Tooltip("If set to true surface volume will be dependent on slip (asphalt, concrete, etc.). Set to false for dirt, grass and other soft surfaces.")]
		public bool slipSensitiveSurfaceSound;

		// Token: 0x04002272 RID: 8818
		[Tooltip("Should tire rolling over the surface sound be played for this surface type?")]
		public bool playSurfaceSounds = true;

		// Token: 0x04002273 RID: 8819
		[Tooltip("    AudioClip used for wheel rolling sound effect.")]
		public AudioClip surfaceSoundClip;

		// Token: 0x04002274 RID: 8820
		[Tooltip("    Sound pitch of wheel rolling over the surface.")]
		public float surfaceSoundPitch = 1f;

		// Token: 0x04002275 RID: 8821
		[Tooltip("    Sound volume of wheel rolling over the surface.")]
		public float surfaceSoundVolume = 0.3f;

		// Token: 0x04002276 RID: 8822
		[Tooltip("Should skid/thread marks be drawn on this surface?")]
		public bool drawSkidmarks = true;

		// Token: 0x04002277 RID: 8823
		[Tooltip("Material used for skid/thread marks on this type of surface.")]
		public Material skidmarkMaterial;

		// Token: 0x04002278 RID: 8824
		[FormerlySerializedAs("baseIntensity")]
		[Range(0f, 1f)]
		[Tooltip("Intensity of the skidmarks when there is no wheel slip.\r\nSet to 0 for hard surfaces and >0 for soft surfaces where the tire leaves the mark by rolling over it.")]
		public float skidmarkBaseIntensity = 0.5f;

		// Token: 0x020004C3 RID: 1219
		public enum ParticleType
		{
			// Token: 0x04002C1D RID: 11293
			Smoke,
			// Token: 0x04002C1E RID: 11294
			Dust
		}
	}
}
