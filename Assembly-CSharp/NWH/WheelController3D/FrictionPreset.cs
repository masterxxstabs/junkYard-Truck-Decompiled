using System;
using UnityEngine;

namespace NWH.WheelController3D
{
	// Token: 0x0200024B RID: 587
	[CreateAssetMenu(fileName = "NWH Vehicle Physics", menuName = "NWH Vehicle Physics/Friction Preset", order = 1)]
	[Serializable]
	public class FrictionPreset : ScriptableObject
	{
		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000EEC RID: 3820 RVA: 0x000B2276 File Offset: 0x000B0476
		public AnimationCurve Curve
		{
			get
			{
				return this._curve;
			}
		}

		// Token: 0x06000EED RID: 3821 RVA: 0x000B227E File Offset: 0x000B047E
		private void Awake()
		{
			this.GenerateLUT();
		}

		// Token: 0x06000EEE RID: 3822 RVA: 0x000B2288 File Offset: 0x000B0488
		public void UpdateFrictionCurve()
		{
			this._curve = new AnimationCurve();
			int num = new Keyframe[60].Length;
			for (int i = 0; i < num; i++)
			{
				float num2 = (float)i / 59f;
				float frictionValue = FrictionPreset.GetFrictionValue(num2, this.BCDE);
				this._curve.AddKey(num2, frictionValue);
			}
			this.GenerateLUT();
		}

		// Token: 0x06000EEF RID: 3823 RVA: 0x000B22E0 File Offset: 0x000B04E0
		public void GenerateLUT()
		{
			this.LUT = new float[1000];
			for (int i = 0; i < 1000; i++)
			{
				float time = (float)i / 1000f;
				this.LUT[i] = this._curve.Evaluate(time);
			}
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x000B227E File Offset: 0x000B047E
		private void OnValidate()
		{
			this.GenerateLUT();
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x000B232C File Offset: 0x000B052C
		private static float GetFrictionValue(float slip, Vector4 p)
		{
			float x = p.x;
			float y = p.y;
			float z = p.z;
			float w = p.w;
			float num = Mathf.Abs(slip);
			return z * Mathf.Sin(y * Mathf.Atan(x * num - w * (x * num - Mathf.Atan(x * num))));
		}

		// Token: 0x04001F4C RID: 8012
		public const int LUT_RESOLUTION = 1000;

		// Token: 0x04001F4D RID: 8013
		[Tooltip("    B, C, D and E parameters of short version of Pacejka's magic formula.")]
		public Vector4 BCDE;

		// Token: 0x04001F4E RID: 8014
		public float[] LUT = new float[0];

		// Token: 0x04001F4F RID: 8015
		[SerializeField]
		private AnimationCurve _curve;
	}
}
