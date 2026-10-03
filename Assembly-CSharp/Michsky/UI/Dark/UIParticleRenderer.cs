using System;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
	// Token: 0x0200032A RID: 810
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasRenderer))]
	[RequireComponent(typeof(ParticleSystem))]
	public class UIParticleRenderer : MaskableGraphic
	{
		// Token: 0x170001EA RID: 490
		// (get) Token: 0x060014BC RID: 5308 RVA: 0x000DC101 File Offset: 0x000DA301
		public override Texture mainTexture
		{
			get
			{
				return this.currentTexture;
			}
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x000DC10C File Offset: 0x000DA30C
		protected bool Initialize()
		{
			if (this._transform == null)
			{
				this._transform = base.transform;
			}
			if (this.pSystem == null)
			{
				this.pSystem = base.GetComponent<ParticleSystem>();
				if (this.pSystem == null)
				{
					return false;
				}
				this.mainModule = this.pSystem.main;
				if (this.pSystem.main.maxParticles > 14000)
				{
					this.mainModule.maxParticles = 14000;
				}
				this.pRenderer = this.pSystem.GetComponent<ParticleSystemRenderer>();
				if (this.pRenderer != null)
				{
					this.pRenderer.enabled = false;
				}
				Material material = new Material(Shader.Find("UI/Particles/Additive"));
				if (this.material == null)
				{
					this.material = material;
				}
				this.currentMaterial = this.material;
				if (this.currentMaterial && this.currentMaterial.HasProperty("_MainTex"))
				{
					this.currentTexture = this.currentMaterial.mainTexture;
					if (this.currentTexture == null)
					{
						this.currentTexture = Texture2D.whiteTexture;
					}
				}
				this.material = this.currentMaterial;
				this.mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
				this.particles = null;
			}
			if (this.particles == null)
			{
				this.particles = new ParticleSystem.Particle[this.pSystem.main.maxParticles];
			}
			this.imageUV = new Vector4(0f, 0f, 1f, 1f);
			this.textureSheetAnimation = this.pSystem.textureSheetAnimation;
			this.textureSheetAnimationFrames = 0;
			this.textureSheetAnimationFrameSize = Vector2.zero;
			if (this.textureSheetAnimation.enabled)
			{
				this.textureSheetAnimationFrames = this.textureSheetAnimation.numTilesX * this.textureSheetAnimation.numTilesY;
				this.textureSheetAnimationFrameSize = new Vector2(1f / (float)this.textureSheetAnimation.numTilesX, 1f / (float)this.textureSheetAnimation.numTilesY);
			}
			return true;
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x000DC321 File Offset: 0x000DA521
		protected override void Awake()
		{
			base.Awake();
			if (!this.Initialize())
			{
				base.enabled = false;
			}
		}

		// Token: 0x060014BF RID: 5311 RVA: 0x000DC338 File Offset: 0x000DA538
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			Vector2 zero3 = Vector2.zero;
			int num = this.pSystem.GetParticles(this.particles);
			for (int i = 0; i < num; i++)
			{
				ParticleSystem.Particle particle = this.particles[i];
				Vector2 vector = (this.mainModule.simulationSpace == ParticleSystemSimulationSpace.Local) ? particle.position : this._transform.InverseTransformPoint(particle.position);
				float num2 = -particle.rotation * 0.017453292f;
				float f = num2 + 1.5707964f;
				Color32 currentColor = particle.GetCurrentColor(this.pSystem);
				float num3 = particle.GetCurrentSize(this.pSystem) * 0.5f;
				if (this.mainModule.scalingMode == ParticleSystemScalingMode.Shape)
				{
					vector /= base.canvas.scaleFactor;
				}
				Vector4 vector2 = this.imageUV;
				if (this.textureSheetAnimation.enabled)
				{
					float num4 = this.textureSheetAnimation.frameOverTime.curveMin.Evaluate(1f - particle.remainingLifetime / particle.startLifetime);
					num4 = Mathf.Repeat(num4 * (float)this.textureSheetAnimation.cycleCount, 1f);
					int num5 = 0;
					ParticleSystemAnimationType animation = this.textureSheetAnimation.animation;
					if (animation != ParticleSystemAnimationType.WholeSheet)
					{
						if (animation == ParticleSystemAnimationType.SingleRow)
						{
							num5 = Mathf.FloorToInt(num4 * (float)this.textureSheetAnimation.numTilesX);
							int rowIndex = this.textureSheetAnimation.rowIndex;
							num5 += rowIndex * this.textureSheetAnimation.numTilesX;
						}
					}
					else
					{
						num5 = Mathf.FloorToInt(num4 * (float)this.textureSheetAnimationFrames);
					}
					num5 %= this.textureSheetAnimationFrames;
					vector2.x = (float)(num5 % this.textureSheetAnimation.numTilesX) * this.textureSheetAnimationFrameSize.x;
					vector2.y = (float)Mathf.FloorToInt((float)(num5 / this.textureSheetAnimation.numTilesX)) * this.textureSheetAnimationFrameSize.y;
					vector2.z = vector2.x + this.textureSheetAnimationFrameSize.x;
					vector2.w = vector2.y + this.textureSheetAnimationFrameSize.y;
				}
				zero.x = vector2.x;
				zero.y = vector2.y;
				this._quad[0] = UIVertex.simpleVert;
				this._quad[0].color = currentColor;
				this._quad[0].uv0 = zero;
				zero.x = vector2.x;
				zero.y = vector2.w;
				this._quad[1] = UIVertex.simpleVert;
				this._quad[1].color = currentColor;
				this._quad[1].uv0 = zero;
				zero.x = vector2.z;
				zero.y = vector2.w;
				this._quad[2] = UIVertex.simpleVert;
				this._quad[2].color = currentColor;
				this._quad[2].uv0 = zero;
				zero.x = vector2.z;
				zero.y = vector2.y;
				this._quad[3] = UIVertex.simpleVert;
				this._quad[3].color = currentColor;
				this._quad[3].uv0 = zero;
				if (num2 == 0f)
				{
					zero2.x = vector.x - num3;
					zero2.y = vector.y - num3;
					zero3.x = vector.x + num3;
					zero3.y = vector.y + num3;
					zero.x = zero2.x;
					zero.y = zero2.y;
					this._quad[0].position = zero;
					zero.x = zero2.x;
					zero.y = zero3.y;
					this._quad[1].position = zero;
					zero.x = zero3.x;
					zero.y = zero3.y;
					this._quad[2].position = zero;
					zero.x = zero3.x;
					zero.y = zero2.y;
					this._quad[3].position = zero;
				}
				else
				{
					Vector2 b = new Vector2(Mathf.Cos(num2), Mathf.Sin(num2)) * num3;
					Vector2 b2 = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * num3;
					this._quad[0].position = vector - b - b2;
					this._quad[1].position = vector - b + b2;
					this._quad[2].position = vector + b + b2;
					this._quad[3].position = vector + b - b2;
				}
				vh.AddUIVertexQuad(this._quad);
			}
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x000DC8C0 File Offset: 0x000DAAC0
		private void Update()
		{
			if (!this.fixedTime && Application.isPlaying)
			{
				this.pSystem.Simulate(Time.unscaledDeltaTime, false, false, true);
				this.SetAllDirty();
				if ((this.currentMaterial != null && this.currentTexture != this.currentMaterial.mainTexture) || (this.material != null && this.currentMaterial != null && this.material.shader != this.currentMaterial.shader))
				{
					this.pSystem = null;
					this.Initialize();
				}
			}
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x000DC968 File Offset: 0x000DAB68
		private void LateUpdate()
		{
			if (!Application.isPlaying)
			{
				this.SetAllDirty();
			}
			else if (this.fixedTime)
			{
				this.pSystem.Simulate(Time.unscaledDeltaTime, false, false, true);
				this.SetAllDirty();
				if ((this.currentMaterial != null && this.currentTexture != this.currentMaterial.mainTexture) || (this.material != null && this.currentMaterial != null && this.material.shader != this.currentMaterial.shader))
				{
					this.pSystem = null;
					this.Initialize();
				}
			}
			if (this.material == this.currentMaterial)
			{
				return;
			}
			this.pSystem = null;
			this.Initialize();
		}

		// Token: 0x04002527 RID: 9511
		[Tooltip("Having this enabled run the system in LateUpdate rather than in Update making it faster but less precise (more clunky)")]
		public bool fixedTime = true;

		// Token: 0x04002528 RID: 9512
		private Transform _transform;

		// Token: 0x04002529 RID: 9513
		private ParticleSystem pSystem;

		// Token: 0x0400252A RID: 9514
		private ParticleSystem.Particle[] particles;

		// Token: 0x0400252B RID: 9515
		private UIVertex[] _quad = new UIVertex[4];

		// Token: 0x0400252C RID: 9516
		private Vector4 imageUV = Vector4.zero;

		// Token: 0x0400252D RID: 9517
		private ParticleSystem.TextureSheetAnimationModule textureSheetAnimation;

		// Token: 0x0400252E RID: 9518
		private int textureSheetAnimationFrames;

		// Token: 0x0400252F RID: 9519
		private Vector2 textureSheetAnimationFrameSize;

		// Token: 0x04002530 RID: 9520
		private ParticleSystemRenderer pRenderer;

		// Token: 0x04002531 RID: 9521
		private Material currentMaterial;

		// Token: 0x04002532 RID: 9522
		private Texture currentTexture;

		// Token: 0x04002533 RID: 9523
		private ParticleSystem.MainModule mainModule;
	}
}
