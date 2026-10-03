using System;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x02000300 RID: 768
	[Preserve]
	[ES3Properties(new string[]
	{
		"cycles",
		"dead"
	})]
	public class ES3UserType_TractionBuddy : ES3ComponentType
	{
		// Token: 0x06001409 RID: 5129 RVA: 0x000D44FD File Offset: 0x000D26FD
		public ES3UserType_TractionBuddy() : base(typeof(TractionBuddy))
		{
			ES3UserType_TractionBuddy.Instance = this;
			this.priority = 1;
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x000D451C File Offset: 0x000D271C
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			TractionBuddy tractionBuddy = (TractionBuddy)obj;
			writer.WriteProperty("cycles", tractionBuddy.cycles, ES3Type_int.Instance);
			writer.WritePrivateField("dead", tractionBuddy);
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x000D4558 File Offset: 0x000D2758
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			TractionBuddy tractionBuddy = (TractionBuddy)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (!(a == "cycles"))
				{
					if (!(a == "dead"))
					{
						reader.Skip();
					}
					else
					{
						reader.SetPrivateField("dead", reader.Read<bool>(), tractionBuddy);
					}
				}
				else
				{
					tractionBuddy.cycles = reader.Read<int>(ES3Type_int.Instance);
				}
			}
		}

		// Token: 0x0400246C RID: 9324
		public static ES3Type Instance;
	}
}
