using System;
using ES3Internal;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002E0 RID: 736
	[Preserve]
	[ES3Properties(new string[]
	{

	})]
	public class ES3UserType_ES3Prefab : ES3ComponentType
	{
		// Token: 0x060013B9 RID: 5049 RVA: 0x000CCE25 File Offset: 0x000CB025
		public ES3UserType_ES3Prefab() : base(typeof(ES3Prefab))
		{
			ES3UserType_ES3Prefab.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x000CCE44 File Offset: 0x000CB044
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			ES3Prefab es3Prefab = (ES3Prefab)obj;
		}

		// Token: 0x060013BB RID: 5051 RVA: 0x000CCE50 File Offset: 0x000CB050
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			ES3Prefab es3Prefab = (ES3Prefab)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				reader.Skip();
			}
		}

		// Token: 0x0400244C RID: 9292
		public static ES3Type Instance;
	}
}
