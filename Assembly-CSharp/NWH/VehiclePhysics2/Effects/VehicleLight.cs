using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002B6 RID: 694
	[Serializable]
	public class VehicleLight
	{
		// Token: 0x170001CC RID: 460
		// (get) Token: 0x0600127D RID: 4733 RVA: 0x000C6E1B File Offset: 0x000C501B
		// (set) Token: 0x0600127E RID: 4734 RVA: 0x000C6E23 File Offset: 0x000C5023
		public bool On
		{
			get
			{
				return this.isOn;
			}
			set
			{
				this.isOn = value;
			}
		}

		// Token: 0x0600127F RID: 4735 RVA: 0x000C6E2C File Offset: 0x000C502C
		public void SetState(bool state)
		{
			if (state && !this.isOn)
			{
				this.TurnOn();
				return;
			}
			if (!state && this.isOn)
			{
				this.TurnOff();
			}
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x000C6E51 File Offset: 0x000C5051
		public void Toggle()
		{
			if (this.isOn)
			{
				this.TurnOff();
				return;
			}
			this.TurnOn();
		}

		// Token: 0x06001281 RID: 4737 RVA: 0x000C6E68 File Offset: 0x000C5068
		public void TurnOff()
		{
			if (!this.isOn)
			{
				return;
			}
			foreach (LightSource lightSource in this.lightSources)
			{
				lightSource.TurnOff();
			}
			this.isOn = false;
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x000C6EC8 File Offset: 0x000C50C8
		public void TurnOn()
		{
			if (this.isOn)
			{
				return;
			}
			foreach (LightSource lightSource in this.lightSources)
			{
				lightSource.TurnOn();
			}
			this.isOn = true;
		}

		// Token: 0x040022FB RID: 8955
		[Tooltip("    All the light sources representing the vehicle light.\r\n    E.g. low beam can be represented by a directional light to represent light beam and\r\n    and emissive mesh to represent light optics.")]
		public List<LightSource> lightSources = new List<LightSource>();

		// Token: 0x040022FC RID: 8956
		protected bool isOn;
	}
}
