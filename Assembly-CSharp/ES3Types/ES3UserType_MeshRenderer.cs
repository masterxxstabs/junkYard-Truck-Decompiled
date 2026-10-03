using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002F2 RID: 754
	[Preserve]
	[ES3Properties(new string[]
	{
		"enabled"
	})]
	public class ES3UserType_MeshRenderer : ES3ComponentType
	{
		// Token: 0x060013E6 RID: 5094 RVA: 0x000D231D File Offset: 0x000D051D
		public ES3UserType_MeshRenderer() : base(typeof(MeshRenderer))
		{
			ES3UserType_MeshRenderer.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x000D233C File Offset: 0x000D053C
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			MeshRenderer meshRenderer = (MeshRenderer)obj;
			writer.WriteProperty("enabled", meshRenderer.enabled, ES3Type_bool.Instance);
		}

		// Token: 0x060013E8 RID: 5096 RVA: 0x000D236C File Offset: 0x000D056C
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			MeshRenderer meshRenderer = (MeshRenderer)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (a == "enabled")
				{
					meshRenderer.enabled = reader.Read<bool>(ES3Type_bool.Instance);
				}
				else
				{
					reader.Skip();
				}
			}
		}

		// Token: 0x0400245E RID: 9310
		public static ES3Type Instance;
	}
}
