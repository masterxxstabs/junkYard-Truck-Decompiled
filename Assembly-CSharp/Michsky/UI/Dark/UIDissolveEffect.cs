using System;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
	// Token: 0x02000328 RID: 808
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class UIDissolveEffect : BaseMeshEffect
	{
		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x060014A5 RID: 5285 RVA: 0x000DBD6A File Offset: 0x000D9F6A
		public new Graphic graphic
		{
			get
			{
				return base.graphic;
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x060014A6 RID: 5286 RVA: 0x000DBD72 File Offset: 0x000D9F72
		// (set) Token: 0x060014A7 RID: 5287 RVA: 0x000DBD7A File Offset: 0x000D9F7A
		public float location
		{
			get
			{
				return this.m_Location;
			}
			set
			{
				this.m_Location = Mathf.Clamp(value, 0f, 1f);
				this._SetDirty();
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x060014A8 RID: 5288 RVA: 0x000DBD98 File Offset: 0x000D9F98
		// (set) Token: 0x060014A9 RID: 5289 RVA: 0x000DBDA0 File Offset: 0x000D9FA0
		public float width
		{
			get
			{
				return this.m_Width;
			}
			set
			{
				this.m_Width = Mathf.Clamp(value, 0f, 1f);
				this._SetDirty();
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x060014AA RID: 5290 RVA: 0x000DBDBE File Offset: 0x000D9FBE
		// (set) Token: 0x060014AB RID: 5291 RVA: 0x000DBDC6 File Offset: 0x000D9FC6
		public float softness
		{
			get
			{
				return this.m_Softness;
			}
			set
			{
				this.m_Softness = Mathf.Clamp(value, 0f, 1f);
				this._SetDirty();
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060014AC RID: 5292 RVA: 0x000DBDE4 File Offset: 0x000D9FE4
		// (set) Token: 0x060014AD RID: 5293 RVA: 0x000DBDEC File Offset: 0x000D9FEC
		public Color color
		{
			get
			{
				return this.m_Color;
			}
			set
			{
				this.m_Color = value;
				this._SetDirty();
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x060014AE RID: 5294 RVA: 0x000DBDFB File Offset: 0x000D9FFB
		public UIDissolveEffect.ColorMode colorMode
		{
			get
			{
				return this.m_ColorMode;
			}
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x060014AF RID: 5295 RVA: 0x000DBE03 File Offset: 0x000DA003
		public virtual Material effectMaterial
		{
			get
			{
				return this.m_EffectMaterial;
			}
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x000DBE0B File Offset: 0x000DA00B
		protected override void OnEnable()
		{
			this.graphic.material = this.effectMaterial;
			base.OnEnable();
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x000DBE24 File Offset: 0x000DA024
		protected override void OnDisable()
		{
			this.graphic.material = null;
			base.OnDisable();
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x000DBE38 File Offset: 0x000DA038
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!this.IsActive())
			{
				return;
			}
			Rect rect = this.graphic.rectTransform.rect;
			UIVertex uivertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref uivertex, i);
				float x = Mathf.Clamp01(uivertex.position.x / rect.width + 0.5f);
				float y = Mathf.Clamp01(uivertex.position.y / rect.height + 0.5f);
				uivertex.uv1 = new Vector2(UIDissolveEffect._PackToFloat(x, y, this.location, this.m_Width), UIDissolveEffect._PackToFloat(this.m_Color.r, this.m_Color.g, this.m_Color.b, this.m_Softness));
				vh.SetUIVertex(uivertex, i);
			}
		}

		// Token: 0x060014B3 RID: 5299 RVA: 0x000DBF1A File Offset: 0x000DA11A
		private void _SetDirty()
		{
			if (this.graphic)
			{
				this.graphic.SetVerticesDirty();
			}
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x000DBF34 File Offset: 0x000DA134
		private static float _PackToFloat(float x, float y, float z)
		{
			return (float)((Mathf.FloorToInt(z * 255f) << 16) + (Mathf.FloorToInt(y * 255f) << 8) + Mathf.FloorToInt(x * 255f));
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x000DBF62 File Offset: 0x000DA162
		private static float _PackToFloat(float x, float y, float z, float w)
		{
			return (float)((Mathf.FloorToInt(w * 63f) << 18) + (Mathf.FloorToInt(z * 63f) << 12) + (Mathf.FloorToInt(y * 63f) << 6) + Mathf.FloorToInt(x * 63f));
		}

		// Token: 0x0400251A RID: 9498
		public const string shaderName = "Custom/UI/Dissolve Effect";

		// Token: 0x0400251B RID: 9499
		[SerializeField]
		[Range(0f, 1f)]
		private float m_Location = 0.5f;

		// Token: 0x0400251C RID: 9500
		[SerializeField]
		[Range(0f, 1f)]
		private float m_Width = 0.5f;

		// Token: 0x0400251D RID: 9501
		[SerializeField]
		[Range(0f, 1f)]
		private float m_Softness = 0.5f;

		// Token: 0x0400251E RID: 9502
		[SerializeField]
		[ColorUsage(false)]
		private Color m_Color = new Color(0f, 0.25f, 1f);

		// Token: 0x0400251F RID: 9503
		[SerializeField]
		private UIDissolveEffect.ColorMode m_ColorMode = UIDissolveEffect.ColorMode.Add;

		// Token: 0x04002520 RID: 9504
		[SerializeField]
		private Material m_EffectMaterial;

		// Token: 0x020004EB RID: 1259
		public enum ColorMode
		{
			// Token: 0x04002CC4 RID: 11460
			None,
			// Token: 0x04002CC5 RID: 11461
			Set,
			// Token: 0x04002CC6 RID: 11462
			Add,
			// Token: 0x04002CC7 RID: 11463
			Sub
		}
	}
}
