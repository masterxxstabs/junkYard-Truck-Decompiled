using System;

namespace NWH.VehiclePhysics2.Demo
{
	// Token: 0x0200025F RID: 607
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ShowInSettings : Attribute
	{
		// Token: 0x06000FF7 RID: 4087 RVA: 0x000B9F22 File Offset: 0x000B8122
		public ShowInSettings(string name)
		{
			this.name = name;
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x000B9F47 File Offset: 0x000B8147
		public ShowInSettings(float min, float max, float step = 0.1f)
		{
			this.min = min;
			this.max = max;
			this.step = step;
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x000B9F7A File Offset: 0x000B817A
		public ShowInSettings(string name, float min, float max, float step = 0.1f)
		{
			this.name = name;
			this.min = min;
			this.max = max;
			this.step = step;
		}

		// Token: 0x06000FFA RID: 4090 RVA: 0x000B9FB5 File Offset: 0x000B81B5
		public ShowInSettings()
		{
		}

		// Token: 0x04002094 RID: 8340
		public string name;

		// Token: 0x04002095 RID: 8341
		public float min;

		// Token: 0x04002096 RID: 8342
		public float max = 1f;

		// Token: 0x04002097 RID: 8343
		public float step = 0.1f;
	}
}
