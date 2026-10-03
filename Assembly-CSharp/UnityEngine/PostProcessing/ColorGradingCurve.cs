using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001EA RID: 490
	[Serializable]
	public sealed class ColorGradingCurve
	{
		// Token: 0x06000BBE RID: 3006 RVA: 0x00094A0C File Offset: 0x00092C0C
		public ColorGradingCurve(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds)
		{
			this.curve = curve;
			this.m_ZeroValue = zeroValue;
			this.m_Loop = loop;
			this.m_Range = bounds.magnitude;
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x00094A38 File Offset: 0x00092C38
		public void Cache()
		{
			if (!this.m_Loop)
			{
				return;
			}
			int length = this.curve.length;
			if (length < 2)
			{
				return;
			}
			if (this.m_InternalLoopingCurve == null)
			{
				this.m_InternalLoopingCurve = new AnimationCurve();
			}
			Keyframe key = this.curve[length - 1];
			key.time -= this.m_Range;
			Keyframe key2 = this.curve[0];
			key2.time += this.m_Range;
			this.m_InternalLoopingCurve.keys = this.curve.keys;
			this.m_InternalLoopingCurve.AddKey(key);
			this.m_InternalLoopingCurve.AddKey(key2);
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x00094AE8 File Offset: 0x00092CE8
		public float Evaluate(float t)
		{
			if (this.curve.length == 0)
			{
				return this.m_ZeroValue;
			}
			if (!this.m_Loop || this.curve.length == 1)
			{
				return this.curve.Evaluate(t);
			}
			return this.m_InternalLoopingCurve.Evaluate(t);
		}

		// Token: 0x04001DB2 RID: 7602
		public AnimationCurve curve;

		// Token: 0x04001DB3 RID: 7603
		[SerializeField]
		private bool m_Loop;

		// Token: 0x04001DB4 RID: 7604
		[SerializeField]
		private float m_ZeroValue;

		// Token: 0x04001DB5 RID: 7605
		[SerializeField]
		private float m_Range;

		// Token: 0x04001DB6 RID: 7606
		private AnimationCurve m_InternalLoopingCurve;
	}
}
