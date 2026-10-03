using System;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
	// Token: 0x02000323 RID: 803
	public class SliderGamepadManager : MonoBehaviour
	{
		// Token: 0x06001492 RID: 5266 RVA: 0x000DB706 File Offset: 0x000D9906
		private void Start()
		{
			if (this.sliderObject == null)
			{
				this.sliderObject = base.gameObject.GetComponent<Slider>();
			}
			base.enabled = false;
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x000DB730 File Offset: 0x000D9930
		private void Update()
		{
			float axis = Input.GetAxis(this.horizontalAxis);
			if (axis == 1f)
			{
				this.sliderObject.value += this.changeValue;
				return;
			}
			if (axis == -1f)
			{
				this.sliderObject.value -= this.changeValue;
			}
		}

		// Token: 0x040024FA RID: 9466
		[Header("SLIDER")]
		public Slider sliderObject;

		// Token: 0x040024FB RID: 9467
		public float changeValue = 0.5f;

		// Token: 0x040024FC RID: 9468
		[Header("INPUT")]
		public string horizontalAxis = "Xbox Right Stick Horizontal";
	}
}
