using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.VehicleGUI
{
	// Token: 0x020002AB RID: 683
	public class AnalogGauge : MonoBehaviour
	{
		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06001234 RID: 4660 RVA: 0x000C4900 File Offset: 0x000C2B00
		// (set) Token: 0x06001235 RID: 4661 RVA: 0x000C4908 File Offset: 0x000C2B08
		public float Value
		{
			get
			{
				return this._currentValue;
			}
			set
			{
				this._currentValue = Mathf.Clamp(value, 0f, this.maxValue);
			}
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x000C4921 File Offset: 0x000C2B21
		private void Awake()
		{
			this._needle = base.transform.Find("Needle").gameObject;
		}

		// Token: 0x06001237 RID: 4663 RVA: 0x000C493E File Offset: 0x000C2B3E
		private void Start()
		{
			this._angle = this.startAngle;
		}

		// Token: 0x06001238 RID: 4664 RVA: 0x000C494C File Offset: 0x000C2B4C
		private void Update()
		{
			this._percent = Mathf.Clamp01(this._currentValue / this.maxValue);
			this._prevAngle = this._angle;
			this._angle = Mathf.Lerp(this.startAngle + (this.endAngle - this.startAngle) * this._percent, this._prevAngle, this.needleSmoothing);
			if (this.lockAtEnd)
			{
				this._angle = this.endAngle;
			}
			if (this.lockAtStart)
			{
				this._angle = this.startAngle;
			}
			this._needle.transform.eulerAngles = new Vector3(this._needle.transform.eulerAngles.x, this._needle.transform.eulerAngles.y, this._angle);
		}

		// Token: 0x04002279 RID: 8825
		[Tooltip("Angle of the needle at the highest value. You can use lock at end option to adjust this value while in play mode.")]
		public float endAngle = 330f;

		// Token: 0x0400227A RID: 8826
		[Tooltip("    Locks the needle position at the end angle (play mode only).")]
		public bool lockAtEnd;

		// Token: 0x0400227B RID: 8827
		[Tooltip("    Locks the needle position at the start angle (play mode only).")]
		public bool lockAtStart;

		// Token: 0x0400227C RID: 8828
		[Tooltip("    Value at the end of needle travel, at the end angle.")]
		public float maxValue;

		// Token: 0x0400227D RID: 8829
		[Range(0f, 1f)]
		[Tooltip("    Smooths the travel of the needle making it more inert, as if actually had some mass and resistance.")]
		public float needleSmoothing;

		// Token: 0x0400227E RID: 8830
		[Tooltip("Angle of the needle at the lowest value. You can use lock at start option to adjust this value while in play mode.")]
		public float startAngle = 574f;

		// Token: 0x0400227F RID: 8831
		private float _angle;

		// Token: 0x04002280 RID: 8832
		private float _currentValue;

		// Token: 0x04002281 RID: 8833
		private GameObject _needle;

		// Token: 0x04002282 RID: 8834
		private float _percent;

		// Token: 0x04002283 RID: 8835
		private float _prevAngle;
	}
}
