using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002E2 RID: 738
	[Preserve]
	[ES3Properties(new string[]
	{
		"aSource",
		"missionNum",
		"pointer1",
		"pointer2",
		"pointer3b",
		"deliveryZone1",
		"deliveryZone2b",
		"isBusy",
		"pipepallet",
		"toolbox"
	})]
	public class ES3UserType_Fireman : ES3ComponentType
	{
		// Token: 0x060013BE RID: 5054 RVA: 0x000CCECD File Offset: 0x000CB0CD
		public ES3UserType_Fireman() : base(typeof(Fireman))
		{
			ES3UserType_Fireman.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013BF RID: 5055 RVA: 0x000CCEEC File Offset: 0x000CB0EC
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Fireman fireman = (Fireman)obj;
			writer.WritePropertyByRef("aSource", fireman.aSource);
			writer.WriteProperty("missionNum", fireman.missionNum, ES3Type_int.Instance);
			writer.WritePropertyByRef("pointer1", fireman.pointer1);
			writer.WritePropertyByRef("pointer2", fireman.pointer2);
			writer.WritePropertyByRef("pointer3b", fireman.pointer3b);
			writer.WritePropertyByRef("deliveryZone1", fireman.deliveryZone1);
			writer.WritePropertyByRef("deliveryZone2b", fireman.deliveryZone2b);
			writer.WriteProperty("isBusy", fireman.isBusy, ES3Type_bool.Instance);
			writer.WritePropertyByRef("pipepallet", fireman.pipepallet);
			writer.WritePropertyByRef("toolbox", fireman.toolbox);
		}

		// Token: 0x060013C0 RID: 5056 RVA: 0x000CCFC0 File Offset: 0x000CB1C0
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Fireman fireman = (Fireman)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2054349215U)
				{
					if (num <= 1196138696U)
					{
						if (num != 135290811U)
						{
							if (num == 1196138696U)
							{
								if (text == "pointer2")
								{
									fireman.pointer2 = reader.Read<showmission>();
									continue;
								}
							}
						}
						else if (text == "pointer3b")
						{
							fireman.pointer3b = reader.Read<showmission>();
							continue;
						}
					}
					else if (num != 1246471553U)
					{
						if (num != 1542310823U)
						{
							if (num == 2054349215U)
							{
								if (text == "pipepallet")
								{
									fireman.pipepallet = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "deliveryZone2b")
						{
							fireman.deliveryZone2b = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (text == "pointer1")
					{
						fireman.pointer1 = reader.Read<showmission>();
						continue;
					}
				}
				else if (num <= 2863242192U)
				{
					if (num != 2161552402U)
					{
						if (num == 2863242192U)
						{
							if (text == "isBusy")
							{
								fireman.isBusy = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
					}
					else if (text == "deliveryZone1")
					{
						fireman.deliveryZone1 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num != 4008009335U)
				{
					if (num != 4071080281U)
					{
						if (num == 4075824792U)
						{
							if (text == "toolbox")
							{
								fireman.toolbox = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
					}
					else if (text == "aSource")
					{
						fireman.aSource = reader.Read<AudioSource>();
						continue;
					}
				}
				else if (text == "missionNum")
				{
					fireman.missionNum = reader.Read<int>(ES3Type_int.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x0400244E RID: 9294
		public static ES3Type Instance;
	}
}
