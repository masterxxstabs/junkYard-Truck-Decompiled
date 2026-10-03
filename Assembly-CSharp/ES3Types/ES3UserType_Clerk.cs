using System;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002D8 RID: 728
	[Preserve]
	[ES3Properties(new string[]
	{
		"jobNum"
	})]
	public class ES3UserType_Clerk : ES3ComponentType
	{
		// Token: 0x060013A5 RID: 5029 RVA: 0x000CCAB9 File Offset: 0x000CACB9
		public ES3UserType_Clerk() : base(typeof(Clerk))
		{
			ES3UserType_Clerk.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x000CCAD8 File Offset: 0x000CACD8
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Clerk clerk = (Clerk)obj;
			writer.WriteProperty("jobNum", clerk.jobNum, ES3Type_int.Instance);
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x000CCB08 File Offset: 0x000CAD08
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Clerk clerk = (Clerk)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (a == "jobNum")
				{
					clerk.jobNum = reader.Read<int>(ES3Type_int.Instance);
				}
				else
				{
					reader.Skip();
				}
			}
		}

		// Token: 0x04002444 RID: 9284
		public static ES3Type Instance;
	}
}
