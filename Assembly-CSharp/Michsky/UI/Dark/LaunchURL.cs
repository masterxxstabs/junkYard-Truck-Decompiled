using System;
using UnityEngine;

namespace Michsky.UI.Dark
{
	// Token: 0x02000316 RID: 790
	public class LaunchURL : MonoBehaviour
	{
		// Token: 0x06001450 RID: 5200 RVA: 0x000DA3C4 File Offset: 0x000D85C4
		public void urlLinkOrWeb()
		{
			Application.OpenURL(this.URL);
		}

		// Token: 0x040024B7 RID: 9399
		public string URL;
	}
}
