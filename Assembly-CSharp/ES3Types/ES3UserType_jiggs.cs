using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x0200030A RID: 778
	[Preserve]
	[ES3Properties(new string[]
	{
		"bomb",
		"missionNum",
		"pulled1",
		"pulled2",
		"pulled3",
		"spawnedpaddle"
	})]
	public class ES3UserType_jiggs : ES3ComponentType
	{
		// Token: 0x06001422 RID: 5154 RVA: 0x000D94B1 File Offset: 0x000D76B1
		public ES3UserType_jiggs() : base(typeof(jiggs))
		{
			ES3UserType_jiggs.Instance = this;
			this.priority = 1;
		}

		// Token: 0x06001423 RID: 5155 RVA: 0x000D94D0 File Offset: 0x000D76D0
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			jiggs jiggs = (jiggs)obj;
			writer.WritePropertyByRef("bomb", jiggs.bomb);
			writer.WriteProperty("missionNum", jiggs.missionNum, ES3Type_int.Instance);
			writer.WriteProperty("pulled1", jiggs.pulled1, ES3Type_bool.Instance);
			writer.WriteProperty("pulled2", jiggs.pulled2, ES3Type_bool.Instance);
			writer.WriteProperty("pulled3", jiggs.pulled3, ES3Type_bool.Instance);
			writer.WritePropertyByRef("spawnedpaddle", jiggs.spawnedpaddle);
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x000D9574 File Offset: 0x000D7774
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			jiggs jiggs = (jiggs)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (!(a == "bomb"))
				{
					if (!(a == "missionNum"))
					{
						if (!(a == "pulled1"))
						{
							if (!(a == "pulled2"))
							{
								if (!(a == "pulled3"))
								{
									if (!(a == "spawnedpaddle"))
									{
										reader.Skip();
									}
									else
									{
										jiggs.spawnedpaddle = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									}
								}
								else
								{
									jiggs.pulled3 = reader.Read<bool>(ES3Type_bool.Instance);
								}
							}
							else
							{
								jiggs.pulled2 = reader.Read<bool>(ES3Type_bool.Instance);
							}
						}
						else
						{
							jiggs.pulled1 = reader.Read<bool>(ES3Type_bool.Instance);
						}
					}
					else
					{
						jiggs.missionNum = reader.Read<int>(ES3Type_int.Instance);
					}
				}
				else
				{
					jiggs.bomb = reader.Read<GameObject>(ES3Type_GameObject.Instance);
				}
			}
		}

		// Token: 0x04002476 RID: 9334
		public static ES3Type Instance;
	}
}
