using System;
using UnityEngine;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.VehicleGUI
{
	// Token: 0x020002AD RID: 685
	[RequireComponent(typeof(Image))]
	public class DashLight : MonoBehaviour
	{
		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x000C4E57 File Offset: 0x000C3057
		// (set) Token: 0x06001240 RID: 4672 RVA: 0x000C4E60 File Offset: 0x000C3060
		public bool Active
		{
			get
			{
				return this._active;
			}
			set
			{
				this._wasActive = this._active;
				this._active = value;
				if (!this._active && this._wasActive)
				{
					if (this._fadeOutTimer < 0f)
					{
						this._fadeOutTimer = 0f;
						return;
					}
				}
				else if (this._active && !this._wasActive)
				{
					this._icon.color = this.onColor;
				}
			}
		}

		// Token: 0x06001241 RID: 4673 RVA: 0x000C4ECA File Offset: 0x000C30CA
		private void Start()
		{
			this._active = false;
			this._icon = base.GetComponent<Image>();
			this._icon.color = this.offColor;
			this._fadeOutTimer = -1f;
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x000C4EFC File Offset: 0x000C30FC
		private void Update()
		{
			if (this.fadeTime > 0f)
			{
				if (this._fadeOutTimer >= 0f && this._fadeOutTimer <= this.fadeTime + this.holdTime)
				{
					this._icon.color = Color.Lerp(this.onColor, this.offColor, (this._fadeOutTimer - this.holdTime) / this.fadeTime);
				}
			}
			else if (this._fadeOutTimer >= 0f)
			{
				this._icon.color = this.offColor;
			}
			if (this._fadeOutTimer >= 0f)
			{
				this._fadeOutTimer += Time.deltaTime;
				if (this._fadeOutTimer > this.fadeTime + this.holdTime)
				{
					this._fadeOutTimer = -1f;
				}
			}
		}

		// Token: 0x040022A3 RID: 8867
		[Tooltip("Time it takes for the light to turn off, once Hold Time expires. Imitates dash lights that use conventional bulbs.")]
		public float fadeTime = 0.5f;

		// Token: 0x040022A4 RID: 8868
		[Tooltip("Time it takes for the light to start turning off after the state has changed. Useful to remove flicker from signals that are not persistent but rather change on frame-to-frame basis, e.g. ABS and TCS. ")]
		public float holdTime = 0.2f;

		// Token: 0x040022A5 RID: 8869
		[Tooltip("    Color of the ''Image'' when the light is off. Black by default.")]
		public Color offColor = Color.black;

		// Token: 0x040022A6 RID: 8870
		[Tooltip("    Color of the ''Image'' when the light is on. White by default.")]
		public Color onColor = Color.white;

		// Token: 0x040022A7 RID: 8871
		private bool _active;

		// Token: 0x040022A8 RID: 8872
		private float _fadeOutTimer;

		// Token: 0x040022A9 RID: 8873
		private Image _icon;

		// Token: 0x040022AA RID: 8874
		private bool _wasActive;
	}
}
