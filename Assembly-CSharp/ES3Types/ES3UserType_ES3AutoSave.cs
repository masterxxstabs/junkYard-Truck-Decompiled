using System;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002DE RID: 734
	[Preserve]
	[ES3Properties(new string[]
	{

	})]
	public class ES3UserType_ES3AutoSave : ES3ComponentType
	{
		// Token: 0x060013B4 RID: 5044 RVA: 0x000CCD7D File Offset: 0x000CAF7D
		public ES3UserType_ES3AutoSave() : base(typeof(ES3AutoSave))
		{
			ES3UserType_ES3AutoSave.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x000CCD9C File Offset: 0x000CAF9C
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			ES3AutoSave es3AutoSave = (ES3AutoSave)obj;
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x000CCDA8 File Offset: 0x000CAFA8
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			ES3AutoSave es3AutoSave = (ES3AutoSave)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				reader.Skip();
			}
		}

		// Token: 0x0400244A RID: 9290
		public static ES3Type Instance;
	}
}
