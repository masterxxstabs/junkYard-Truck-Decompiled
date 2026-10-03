using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x02000304 RID: 772
	[Preserve]
	[ES3Properties(new string[]
	{
		"wheel",
		"pulley",
		"rpm",
		"batteryR",
		"batteryDur",
		"enabled"
	})]
	public class ES3UserType_Waterwheel : ES3ComponentType
	{
		// Token: 0x06001413 RID: 5139 RVA: 0x000D4779 File Offset: 0x000D2979
		public ES3UserType_Waterwheel() : base(typeof(Waterwheel))
		{
			ES3UserType_Waterwheel.Instance = this;
			this.priority = 1;
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x000D4798 File Offset: 0x000D2998
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Waterwheel waterwheel = (Waterwheel)obj;
			writer.WritePropertyByRef("wheel", waterwheel.wheel);
			writer.WritePropertyByRef("pulley", waterwheel.pulley);
			writer.WriteProperty("rpm", waterwheel.rpm, ES3Type_float.Instance);
			writer.WritePropertyByRef("batteryR", waterwheel.batteryR);
			writer.WritePropertyByRef("batteryDur", waterwheel.batteryDur);
			writer.WriteProperty("enabled", waterwheel.enabled, ES3Type_bool.Instance);
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x000D4828 File Offset: 0x000D2A28
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Waterwheel waterwheel = (Waterwheel)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (!(a == "wheel"))
				{
					if (!(a == "pulley"))
					{
						if (!(a == "rpm"))
						{
							if (!(a == "batteryR"))
							{
								if (!(a == "batteryDur"))
								{
									if (!(a == "enabled"))
									{
										reader.Skip();
									}
									else
									{
										waterwheel.enabled = reader.Read<bool>(ES3Type_bool.Instance);
									}
								}
								else
								{
									waterwheel.batteryDur = reader.Read<durability>();
								}
							}
							else
							{
								waterwheel.batteryR = reader.Read<Renderer>();
							}
						}
						else
						{
							waterwheel.rpm = reader.Read<float>(ES3Type_float.Instance);
						}
					}
					else
					{
						waterwheel.pulley = reader.Read<Transform>(ES3UserType_Transform.Instance);
					}
				}
				else
				{
					waterwheel.wheel = reader.Read<Transform>(ES3UserType_Transform.Instance);
				}
			}
		}

		// Token: 0x04002470 RID: 9328
		public static ES3Type Instance;
	}
}
