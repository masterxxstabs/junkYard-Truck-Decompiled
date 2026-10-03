using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002F4 RID: 756
	[Preserve]
	[ES3Properties(new string[]
	{
		"jobNum",
		"spawnedItem",
		"spawnedItem2",
		"job1Condition",
		"job2Condition",
		"job3Condition",
		"job4Condition",
		"job5Condition",
		"tireSlot",
		"isIdle",
		"resetJobs",
		"spawnedTire",
		"spawnedTire2",
		"spawnedTurbo",
		"spawnedRadar",
		"spawnedAdditive",
		"spawnedBuddy"
	})]
	public class ES3UserType_ModWomanJobs : ES3ComponentType
	{
		// Token: 0x060013EB RID: 5099 RVA: 0x000D2409 File Offset: 0x000D0609
		public ES3UserType_ModWomanJobs() : base(typeof(ModWomanJobs))
		{
			ES3UserType_ModWomanJobs.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x000D2428 File Offset: 0x000D0628
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			ModWomanJobs modWomanJobs = (ModWomanJobs)obj;
			writer.WriteProperty("jobNum", modWomanJobs.jobNum, ES3Type_int.Instance);
			writer.WritePrivateFieldByRef("spawnedItem", modWomanJobs);
			writer.WritePrivateFieldByRef("spawnedItem2", modWomanJobs);
			writer.WriteProperty("job1Condition", modWomanJobs.job1Condition, ES3Type_bool.Instance);
			writer.WriteProperty("job2Condition", modWomanJobs.job2Condition, ES3Type_bool.Instance);
			writer.WriteProperty("job3Condition", modWomanJobs.job3Condition, ES3Type_bool.Instance);
			writer.WriteProperty("job4Condition", modWomanJobs.job4Condition, ES3Type_bool.Instance);
			writer.WriteProperty("job5Condition", modWomanJobs.job5Condition, ES3Type_bool.Instance);
			writer.WriteProperty("tireSlot", modWomanJobs.tireSlot, ES3Type_string.Instance);
			writer.WriteProperty("isIdle", modWomanJobs.isIdle, ES3Type_bool.Instance);
			writer.WriteProperty("resetJobs", modWomanJobs.resetJobs, ES3Type_bool.Instance);
			writer.WritePropertyByRef("spawnedTire", modWomanJobs.spawnedTire);
			writer.WritePropertyByRef("spawnedTire2", modWomanJobs.spawnedTire2);
			writer.WritePropertyByRef("spawnedTurbo", modWomanJobs.spawnedTurbo);
			writer.WriteProperty("spawnedRadar", modWomanJobs.spawnedRadar, ES3Type_bool.Instance);
			writer.WriteProperty("spawnedAdditive", modWomanJobs.spawnedAdditive, ES3Type_bool.Instance);
			writer.WriteProperty("spawnedBuddy", modWomanJobs.spawnedBuddy, ES3Type_bool.Instance);
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x000D25C8 File Offset: 0x000D07C8
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			ModWomanJobs modWomanJobs = (ModWomanJobs)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2287182054U)
				{
					if (num <= 1252225501U)
					{
						if (num <= 611954172U)
						{
							if (num != 600915743U)
							{
								if (num == 611954172U)
								{
									if (text == "job5Condition")
									{
										modWomanJobs.job5Condition = reader.Read<bool>(ES3Type_bool.Instance);
										continue;
									}
								}
							}
							else if (text == "tireSlot")
							{
								modWomanJobs.tireSlot = reader.Read<string>(ES3Type_string.Instance);
								continue;
							}
						}
						else if (num != 1008415260U)
						{
							if (num == 1252225501U)
							{
								if (text == "job4Condition")
								{
									modWomanJobs.job4Condition = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
						}
						else if (text == "jobNum")
						{
							modWomanJobs.jobNum = reader.Read<int>(ES3Type_int.Instance);
							continue;
						}
					}
					else if (num <= 2043179921U)
					{
						if (num != 1932552672U)
						{
							if (num == 2043179921U)
							{
								if (text == "spawnedAdditive")
								{
									modWomanJobs.spawnedAdditive = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
						}
						else if (text == "spawnedItem2")
						{
							reader.SetPrivateField("spawnedItem2", reader.Read<GameObject>(), modWomanJobs);
							continue;
						}
					}
					else if (num != 2129631890U)
					{
						if (num == 2287182054U)
						{
							if (text == "job3Condition")
							{
								modWomanJobs.job3Condition = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
					}
					else if (text == "spawnedItem")
					{
						reader.SetPrivateField("spawnedItem", reader.Read<GameObject>(), modWomanJobs);
						continue;
					}
				}
				else if (num <= 2707600767U)
				{
					if (num <= 2656024512U)
					{
						if (num != 2341819280U)
						{
							if (num == 2656024512U)
							{
								if (text == "resetJobs")
								{
									modWomanJobs.resetJobs = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
						}
						else if (text == "job1Condition")
						{
							modWomanJobs.job1Condition = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num != 2686660015U)
					{
						if (num == 2707600767U)
						{
							if (text == "job2Condition")
							{
								modWomanJobs.job2Condition = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
					}
					else if (text == "spawnedRadar")
					{
						modWomanJobs.spawnedRadar = reader.Read<bool>(ES3Type_bool.Instance);
						continue;
					}
				}
				else if (num <= 3683282863U)
				{
					if (num != 3463693443U)
					{
						if (num == 3683282863U)
						{
							if (text == "spawnedTire2")
							{
								modWomanJobs.spawnedTire2 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
					}
					else if (text == "isIdle")
					{
						modWomanJobs.isIdle = reader.Read<bool>(ES3Type_bool.Instance);
						continue;
					}
				}
				else if (num != 3970340687U)
				{
					if (num != 4229962439U)
					{
						if (num == 4267518567U)
						{
							if (text == "spawnedBuddy")
							{
								modWomanJobs.spawnedBuddy = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
					}
					else if (text == "spawnedTire")
					{
						modWomanJobs.spawnedTire = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (text == "spawnedTurbo")
				{
					modWomanJobs.spawnedTurbo = reader.Read<GameObject>(ES3Type_GameObject.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x04002460 RID: 9312
		public static ES3Type Instance;
	}
}
