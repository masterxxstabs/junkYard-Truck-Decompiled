using System;
using System.Collections;
using UnityEngine;

namespace CalmWater
{
	// Token: 0x02000330 RID: 816
	[RequireComponent(typeof(Projector))]
	public class CausticAnimator : MonoBehaviour
	{
		// Token: 0x060014D7 RID: 5335 RVA: 0x000DCEBC File Offset: 0x000DB0BC
		private void OnEnable()
		{
			if (this._causticFrames.Length < 2)
			{
				base.enabled = false;
			}
			if (this._projector == null)
			{
				this._projector = base.GetComponent<Projector>();
			}
			if (this._mat == null)
			{
				this._mat = this._projector.material;
			}
			this._currentFrame = 0;
			this._propID = Shader.PropertyToID("_CausticTex");
			this._delay = new WaitForSeconds(this._frameDuration);
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x000DCF3C File Offset: 0x000DB13C
		private void OnDisabled()
		{
			base.StopCoroutine(this.AnimateCaustic());
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x000DCF4A File Offset: 0x000DB14A
		private void Awake()
		{
			base.StartCoroutine(this.AnimateCaustic());
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x000DCF59 File Offset: 0x000DB159
		private int NextFrame()
		{
			this._currentFrame++;
			this._currentFrame = ((this._currentFrame >= this._causticFrames.Length) ? 0 : this._currentFrame);
			return this._currentFrame;
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x000DCF8E File Offset: 0x000DB18E
		private IEnumerator AnimateCaustic()
		{
			for (;;)
			{
				yield return this._delay;
				this._mat.SetTexture(this._propID, this._causticFrames[this.NextFrame()]);
			}
			yield break;
		}

		// Token: 0x04002547 RID: 9543
		[SerializeField]
		private float _frameDuration = 0.1f;

		// Token: 0x04002548 RID: 9544
		[SerializeField]
		private Texture2D[] _causticFrames;

		// Token: 0x04002549 RID: 9545
		private Projector _projector;

		// Token: 0x0400254A RID: 9546
		private Material _mat;

		// Token: 0x0400254B RID: 9547
		private WaitForSeconds _delay;

		// Token: 0x0400254C RID: 9548
		private int _propID;

		// Token: 0x0400254D RID: 9549
		private int _currentFrame;
	}
}
