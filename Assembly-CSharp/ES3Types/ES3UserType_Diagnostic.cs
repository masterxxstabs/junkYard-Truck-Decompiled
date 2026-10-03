using System;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002DC RID: 732
	[Preserve]
	[ES3Properties(new string[]
	{
		"jobs"
	})]
	public class ES3UserType_Diagnostic : ES3ComponentType
	{
		// Token: 0x060013AF RID: 5039 RVA: 0x000CCC91 File Offset: 0x000CAE91
		public ES3UserType_Diagnostic() : base(typeof(Diagnostic))
		{
			ES3UserType_Diagnostic.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x000CCCB0 File Offset: 0x000CAEB0
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Diagnostic diagnostic = (Diagnostic)obj;
			writer.WriteProperty("jobs", diagnostic.jobs, ES3Type_int.Instance);
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x000CCCE0 File Offset: 0x000CAEE0
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Diagnostic diagnostic = (Diagnostic)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (a == "jobs")
				{
					diagnostic.jobs = reader.Read<int>(ES3Type_int.Instance);
				}
				else
				{
					reader.Skip();
				}
			}
		}

		// Token: 0x04002448 RID: 9288
		public static ES3Type Instance;
	}
}
