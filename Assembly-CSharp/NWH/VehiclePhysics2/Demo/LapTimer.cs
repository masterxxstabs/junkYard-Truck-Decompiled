using System;
using UnityEngine;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.Demo
{
	// Token: 0x0200025D RID: 605
	public class LapTimer : MonoBehaviour
	{
		// Token: 0x06000FED RID: 4077 RVA: 0x000B9B4C File Offset: 0x000B7D4C
		private void OnTriggerEnter(Collider other)
		{
			this._started = true;
			if (this.currentLapTime < 5f)
			{
				return;
			}
			if (this.currentLapTime < this.bestLapTime)
			{
				this.bestLapTime = this.currentLapTime;
			}
			this.previousLapTime = this.currentLapTime;
			this.currentLapTime = 0f;
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x000B9B9F File Offset: 0x000B7D9F
		private void Start()
		{
			this.currentLapTime = 9999f;
			this.bestLapTime = 9999f;
			this.previousLapTime = 9999f;
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x000B9BC4 File Offset: 0x000B7DC4
		private void Update()
		{
			if (!this._started)
			{
				return;
			}
			this.currentLapTime += Time.deltaTime;
			if (this.currentLapTime < 9998f)
			{
				this.currentLapTimeText.text = this.currentLapTime.ToString("F2");
			}
			if (this.previousLapTime < 9998f)
			{
				this.previousLapTimeText.text = this.previousLapTime.ToString("F2");
			}
			if (this.bestLapTime < 9998f)
			{
				this.bestLapTimeText.text = this.bestLapTime.ToString("F2");
			}
		}

		// Token: 0x04002081 RID: 8321
		public float bestLapTime = 9999f;

		// Token: 0x04002082 RID: 8322
		public Text bestLapTimeText;

		// Token: 0x04002083 RID: 8323
		public float currentLapTime = 9999f;

		// Token: 0x04002084 RID: 8324
		public Text currentLapTimeText;

		// Token: 0x04002085 RID: 8325
		public float previousLapTime = 9999f;

		// Token: 0x04002086 RID: 8326
		public Text previousLapTimeText;

		// Token: 0x04002087 RID: 8327
		private bool _started;
	}
}
