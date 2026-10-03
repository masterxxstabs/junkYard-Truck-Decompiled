using System;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x0200030C RID: 780
	[Preserve]
	[ES3Properties(new string[]
	{
		"spawned"
	})]
	public class ES3UserType_vaultring : ES3ComponentType
	{
		// Token: 0x06001427 RID: 5159 RVA: 0x000D96B9 File Offset: 0x000D78B9
		public ES3UserType_vaultring() : base(typeof(vaultring))
		{
			ES3UserType_vaultring.Instance = this;
			this.priority = 1;
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x000D96D8 File Offset: 0x000D78D8
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			vaultring vaultring = (vaultring)obj;
			writer.WriteProperty("spawned", vaultring.spawned, ES3Type_bool.Instance);
		}

		// Token: 0x06001429 RID: 5161 RVA: 0x000D9708 File Offset: 0x000D7908
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			vaultring vaultring = (vaultring)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (a == "spawned")
				{
					vaultring.spawned = reader.Read<bool>(ES3Type_bool.Instance);
				}
				else
				{
					reader.Skip();
				}
			}
		}

		// Token: 0x04002478 RID: 9336
		public static ES3Type Instance;
	}
}
