using System;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002FC RID: 764
	[Preserve]
	[ES3Properties(new string[]
	{
		"enabled"
	})]
	public class ES3UserType_RadarDetector : ES3ComponentType
	{
		// Token: 0x060013FF RID: 5119 RVA: 0x000D3DD9 File Offset: 0x000D1FD9
		public ES3UserType_RadarDetector() : base(typeof(RadarDetector))
		{
			ES3UserType_RadarDetector.Instance = this;
			this.priority = 1;
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x000D3DF8 File Offset: 0x000D1FF8
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			RadarDetector radarDetector = (RadarDetector)obj;
			writer.WriteProperty("enabled", radarDetector.enabled, ES3Type_bool.Instance);
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x000D3E28 File Offset: 0x000D2028
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			RadarDetector radarDetector = (RadarDetector)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (a == "enabled")
				{
					radarDetector.enabled = reader.Read<bool>(ES3Type_bool.Instance);
				}
				else
				{
					reader.Skip();
				}
			}
		}

		// Token: 0x04002468 RID: 9320
		public static ES3Type Instance;
	}
}
