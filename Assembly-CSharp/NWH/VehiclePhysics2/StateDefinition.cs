using System;
using System.Linq;

namespace NWH.VehiclePhysics2
{
	// Token: 0x02000255 RID: 597
	[Serializable]
	public class StateDefinition
	{
		// Token: 0x06000F7E RID: 3966 RVA: 0x000B6E56 File Offset: 0x000B5056
		public StateDefinition()
		{
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x000B6E74 File Offset: 0x000B5074
		public StateDefinition(string fullName, bool isOn, bool isEnabled, int lod)
		{
			this.fullName = fullName;
			this.name = fullName.Split(new char[]
			{
				'.'
			}).Last<string>();
			this.isOn = isOn;
			this.isEnabled = isEnabled;
			this.lodIndex = lod;
		}

		// Token: 0x04002011 RID: 8209
		public string fullName;

		// Token: 0x04002012 RID: 8210
		public bool isEnabled = true;

		// Token: 0x04002013 RID: 8211
		public bool isOn = true;

		// Token: 0x04002014 RID: 8212
		public int lodIndex = -1;

		// Token: 0x04002015 RID: 8213
		public string name;
	}
}
