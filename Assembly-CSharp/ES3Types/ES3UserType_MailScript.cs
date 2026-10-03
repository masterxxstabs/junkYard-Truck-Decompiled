using System;
using System.Collections.Generic;
using ES3Internal;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002F0 RID: 752
	[Preserve]
	[ES3Properties(new string[]
	{
		"receivedEmails"
	})]
	public class ES3UserType_MailScript : ES3ComponentType
	{
		// Token: 0x060013E1 RID: 5089 RVA: 0x000D222D File Offset: 0x000D042D
		public ES3UserType_MailScript() : base(typeof(MailScript))
		{
			ES3UserType_MailScript.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x000D224C File Offset: 0x000D044C
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			MailScript mailScript = (MailScript)obj;
			writer.WriteProperty("receivedEmails", mailScript.receivedEmails, ES3TypeMgr.GetOrCreateES3Type(typeof(List<int>), true));
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x000D2284 File Offset: 0x000D0484
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			MailScript mailScript = (MailScript)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (a == "receivedEmails")
				{
					mailScript.receivedEmails = reader.Read<List<int>>();
				}
				else
				{
					reader.Skip();
				}
			}
		}

		// Token: 0x0400245C RID: 9308
		public static ES3Type Instance;
	}
}
