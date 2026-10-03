using System;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002EC RID: 748
	[Preserve]
	[ES3Properties(new string[]
	{
		"displayTime",
		"displayBTime",
		"displayLTime",
		"displayOTime",
		"displayATime",
		"displayTTime",
		"infuserMission",
		"lime",
		"orange",
		"blackberry",
		"ambrosia",
		"bootLeggerMission"
	})]
	public class ES3UserType_Jake8 : ES3ComponentType
	{
		// Token: 0x060013D7 RID: 5079 RVA: 0x000D1A8D File Offset: 0x000CFC8D
		public ES3UserType_Jake8() : base(typeof(Jake8))
		{
			ES3UserType_Jake8.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x000D1AAC File Offset: 0x000CFCAC
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Jake8 jake = (Jake8)obj;
			writer.WriteProperty("displayTime", jake.displayTime, ES3Type_float.Instance);
			writer.WriteProperty("displayBTime", jake.displayBTime, ES3Type_float.Instance);
			writer.WriteProperty("displayLTime", jake.displayLTime, ES3Type_float.Instance);
			writer.WriteProperty("displayOTime", jake.displayOTime, ES3Type_float.Instance);
			writer.WriteProperty("displayATime", jake.displayATime, ES3Type_float.Instance);
			writer.WriteProperty("displayTTime", jake.displayTTime, ES3Type_float.Instance);
			writer.WritePrivateField("infuserMission", jake);
			writer.WritePrivateField("lime", jake);
			writer.WritePrivateField("orange", jake);
			writer.WritePrivateField("blackberry", jake);
			writer.WritePrivateField("ambrosia", jake);
			writer.WriteProperty("bootLeggerMission", jake.bootLeggerMission, ES3Type_int.Instance);
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x000D1BBC File Offset: 0x000CFDBC
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Jake8 jake = (Jake8)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1528624180U)
				{
					if (num <= 444769167U)
					{
						if (num != 132336572U)
						{
							if (num != 157686231U)
							{
								if (num == 444769167U)
								{
									if (text == "ambrosia")
									{
										reader.SetPrivateField("ambrosia", reader.Read<bool>(), jake);
										continue;
									}
								}
							}
							else if (text == "bootLeggerMission")
							{
								jake.bootLeggerMission = reader.Read<int>(ES3Type_int.Instance);
								continue;
							}
						}
						else if (text == "lime")
						{
							reader.SetPrivateField("lime", reader.Read<bool>(), jake);
							continue;
						}
					}
					else if (num != 777461883U)
					{
						if (num != 1169454059U)
						{
							if (num == 1528624180U)
							{
								if (text == "displayTime")
								{
									jake.displayTime = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
						}
						else if (text == "orange")
						{
							reader.SetPrivateField("orange", reader.Read<bool>(), jake);
							continue;
						}
					}
					else if (text == "infuserMission")
					{
						reader.SetPrivateField("infuserMission", reader.Read<bool>(), jake);
						continue;
					}
				}
				else if (num <= 2461202244U)
				{
					if (num != 2052034260U)
					{
						if (num != 2269605469U)
						{
							if (num == 2461202244U)
							{
								if (text == "blackberry")
								{
									reader.SetPrivateField("blackberry", reader.Read<bool>(), jake);
									continue;
								}
							}
						}
						else if (text == "displayATime")
						{
							jake.displayATime = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (text == "displayBTime")
					{
						jake.displayBTime = reader.Read<float>(ES3Type_float.Instance);
						continue;
					}
				}
				else if (num != 3092964714U)
				{
					if (num != 3942635442U)
					{
						if (num == 4256497543U)
						{
							if (text == "displayOTime")
							{
								jake.displayOTime = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
					}
					else if (text == "displayLTime")
					{
						jake.displayLTime = reader.Read<float>(ES3Type_float.Instance);
						continue;
					}
				}
				else if (text == "displayTTime")
				{
					jake.displayTTime = reader.Read<float>(ES3Type_float.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x04002458 RID: 9304
		public static ES3Type Instance;
	}
}
