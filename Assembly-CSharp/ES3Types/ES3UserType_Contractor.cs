using System;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002DA RID: 730
	[Preserve]
	[ES3Properties(new string[]
	{
		"dialogueNum"
	})]
	public class ES3UserType_Contractor : ES3ComponentType
	{
		// Token: 0x060013AA RID: 5034 RVA: 0x000CCBA5 File Offset: 0x000CADA5
		public ES3UserType_Contractor() : base(typeof(Contractor))
		{
			ES3UserType_Contractor.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x000CCBC4 File Offset: 0x000CADC4
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Contractor contractor = (Contractor)obj;
			writer.WriteProperty("dialogueNum", contractor.dialogueNum, ES3Type_int.Instance);
		}

		// Token: 0x060013AC RID: 5036 RVA: 0x000CCBF4 File Offset: 0x000CADF4
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Contractor contractor = (Contractor)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (a == "dialogueNum")
				{
					contractor.dialogueNum = reader.Read<int>(ES3Type_int.Instance);
				}
				else
				{
					reader.Skip();
				}
			}
		}

		// Token: 0x04002446 RID: 9286
		public static ES3Type Instance;
	}
}
