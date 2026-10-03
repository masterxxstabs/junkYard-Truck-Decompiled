using System;
using UnityEngine;

namespace ch.sycoforge.Decal.Demo
{
	// Token: 0x020001B3 RID: 435
	public class ProxyRegister : MonoBehaviour
	{
		// Token: 0x06000AA6 RID: 2726 RVA: 0x0008DC08 File Offset: 0x0008BE08
		private void Start()
		{
			EasyDecal.SetStaticProxyCollection(this.ProxyCollection);
		}

		// Token: 0x04001CB8 RID: 7352
		public StaticProxyCollection ProxyCollection;
	}
}
