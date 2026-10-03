using System;
using UnityEngine;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.VehicleGUI
{
	// Token: 0x020002AE RID: 686
	[RequireComponent(typeof(Text))]
	public class DigitalGauge : MonoBehaviour
	{
		// Token: 0x06001244 RID: 4676 RVA: 0x000C4FFC File Offset: 0x000C31FC
		private void Start()
		{
			Transform transform = base.transform.Find("Readout");
			if (transform != null)
			{
				this._readout = transform.gameObject.GetComponent<Text>();
			}
			Transform transform2 = base.transform.Find("Line");
			if (transform2 != null)
			{
				this._line = transform2.gameObject.GetComponent<Image>();
			}
			if (this.gaugeType == DigitalGauge.GaugeType.Textual)
			{
				this.showProgressBar = false;
			}
			if (this._line != null)
			{
				this._fullLineWidth = this._line.rectTransform.sizeDelta.x;
			}
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x000C5098 File Offset: 0x000C3298
		private void Update()
		{
			if (this._readout != null)
			{
				this._readout.text = "";
				if (this.gaugeType == DigitalGauge.GaugeType.Numerical)
				{
					this.numericalValue = Mathf.SmoothStep(this._prevNumericalValue, this.numericalValue, 1.01f - this.numericalSmoothing);
					Text readout = this._readout;
					readout.text += this.numericalValue.ToString(this.format);
					this._prevNumericalValue = this.numericalValue;
				}
				string str = "";
				if (this.unit != "")
				{
					str = " ";
				}
				Text readout2 = this._readout;
				readout2.text = readout2.text + this.stringValue + str + this.unit;
			}
			if (this._line != null && this.showProgressBar)
			{
				float num = Mathf.Clamp01(this.numericalValue / this.maxValue);
				this._line.rectTransform.sizeDelta = new Vector2(num * this._fullLineWidth, this._line.rectTransform.sizeDelta.y);
			}
		}

		// Token: 0x040022AB RID: 8875
		[Tooltip("    Numerical value formatting.")]
		public string format = "0.0";

		// Token: 0x040022AC RID: 8876
		[Tooltip("Should the stringValue or numericalValue be used. String value is useful for e.g. gear (R, N, 1, 2, 3) and numerical\r\nfor\r\nspeed, RPM or similar.")]
		public DigitalGauge.GaugeType gaugeType;

		// Token: 0x040022AD RID: 8877
		[Tooltip("    Maximum value that the gauge can display. Only used if showProgressBar enabled.")]
		public float maxValue;

		// Token: 0x040022AE RID: 8878
		[Range(0f, 1f)]
		[Tooltip("    Time over which the numerical value will be smoothed.")]
		public float numericalSmoothing = 0.5f;

		// Token: 0x040022AF RID: 8879
		[Tooltip("    Numerical value that will be displayed on the gauge.")]
		public float numericalValue;

		// Token: 0x040022B0 RID: 8880
		[Tooltip("    Should the progress line/bar be displayed for better visualization?")]
		public bool showProgressBar;

		// Token: 0x040022B1 RID: 8881
		[Tooltip("    String value that will be displayed on the gauge.")]
		public string stringValue;

		// Token: 0x040022B2 RID: 8882
		[Tooltip("    Unit displayed after the value, e.g. km/h.")]
		public string unit;

		// Token: 0x040022B3 RID: 8883
		private float _fullLineWidth;

		// Token: 0x040022B4 RID: 8884
		private Image _line;

		// Token: 0x040022B5 RID: 8885
		private float _prevNumericalValue;

		// Token: 0x040022B6 RID: 8886
		private Text _readout;

		// Token: 0x020004C5 RID: 1221
		public enum GaugeType
		{
			// Token: 0x04002C23 RID: 11299
			Numerical,
			// Token: 0x04002C24 RID: 11300
			Textual
		}
	}
}
