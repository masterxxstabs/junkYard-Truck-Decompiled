using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
	// Token: 0x020002BE RID: 702
	public abstract class InputProvider : MonoBehaviour
	{
		// Token: 0x060012D9 RID: 4825 RVA: 0x000C981B File Offset: 0x000C7A1B
		public void Awake()
		{
			InputProvider.Instances.Add(this);
		}

		// Token: 0x060012DA RID: 4826
		public abstract bool EngineStartStop();

		// Token: 0x060012DB RID: 4827
		public abstract bool ChangeCamera();

		// Token: 0x060012DC RID: 4828
		public abstract bool ChangeVehicle();

		// Token: 0x060012DD RID: 4829
		public abstract float Clutch();

		// Token: 0x060012DE RID: 4830
		public abstract bool ExtraLights();

		// Token: 0x060012DF RID: 4831
		public abstract bool HighBeamLights();

		// Token: 0x060012E0 RID: 4832
		public abstract float Handbrake();

		// Token: 0x060012E1 RID: 4833
		public abstract bool HazardLights();

		// Token: 0x060012E2 RID: 4834
		public abstract float Horizontal();

		// Token: 0x060012E3 RID: 4835
		public abstract bool Horn();

		// Token: 0x060012E4 RID: 4836
		public abstract bool LeftBlinker();

		// Token: 0x060012E5 RID: 4837
		public abstract bool LowBeamLights();

		// Token: 0x060012E6 RID: 4838
		public abstract bool RightBlinker();

		// Token: 0x060012E7 RID: 4839
		public abstract bool ShiftDown();

		// Token: 0x060012E8 RID: 4840
		public abstract int ShiftInto();

		// Token: 0x060012E9 RID: 4841
		public abstract bool ShiftUp();

		// Token: 0x060012EA RID: 4842
		public abstract bool TrailerAttachDetach();

		// Token: 0x060012EB RID: 4843
		public abstract float Vertical();

		// Token: 0x060012EC RID: 4844
		public abstract bool FlipOver();

		// Token: 0x060012ED RID: 4845
		public abstract bool Boost();

		// Token: 0x060012EE RID: 4846
		public abstract bool CruiseControl();

		// Token: 0x04002365 RID: 9061
		public static List<InputProvider> Instances = new List<InputProvider>();
	}
}
