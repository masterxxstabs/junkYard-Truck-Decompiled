using System;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002F6 RID: 758
	[Preserve]
	[ES3Properties(new string[]
	{
		"warrant",
		"raidWarrant",
		"charge_dui",
		"charge_evasion",
		"charge_posession",
		"charge_reckless",
		"charge_hitandrun",
		"charge_speeding",
		"charge_headlights",
		"charge_opencontainer",
		"charge_theft",
		"charge_obstruction",
		"charge_pubIntox",
		"delinquentDays",
		"activeCitations",
		"numArrests"
	})]
	public class ES3UserType_Officer : ES3ComponentType
	{
		// Token: 0x060013F0 RID: 5104 RVA: 0x000D2A49 File Offset: 0x000D0C49
		public ES3UserType_Officer() : base(typeof(Officer))
		{
			ES3UserType_Officer.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x000D2A68 File Offset: 0x000D0C68
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Officer officer = (Officer)obj;
			writer.WriteProperty("warrant", officer.warrant, ES3Type_bool.Instance);
			writer.WriteProperty("raidWarrant", officer.raidWarrant, ES3Type_bool.Instance);
			writer.WriteProperty("charge_dui", officer.charge_dui, ES3Type_int.Instance);
			writer.WriteProperty("charge_evasion", officer.charge_evasion, ES3Type_int.Instance);
			writer.WriteProperty("charge_posession", officer.charge_posession, ES3Type_int.Instance);
			writer.WriteProperty("charge_reckless", officer.charge_reckless, ES3Type_int.Instance);
			writer.WriteProperty("charge_hitandrun", officer.charge_hitandrun, ES3Type_int.Instance);
			writer.WriteProperty("charge_speeding", officer.charge_speeding, ES3Type_int.Instance);
			writer.WriteProperty("charge_headlights", officer.charge_headlights, ES3Type_int.Instance);
			writer.WriteProperty("charge_opencontainer", officer.charge_opencontainer, ES3Type_int.Instance);
			writer.WriteProperty("charge_theft", officer.charge_theft, ES3Type_int.Instance);
			writer.WriteProperty("charge_obstruction", officer.charge_obstruction, ES3Type_int.Instance);
			writer.WriteProperty("charge_pubIntox", officer.charge_pubIntox, ES3Type_int.Instance);
			writer.WriteProperty("delinquentDays", officer.delinquentDays, ES3Type_int.Instance);
			writer.WriteProperty("activeCitations", officer.activeCitations, ES3Type_int.Instance);
			writer.WriteProperty("numArrests", officer.numArrests, ES3Type_int.Instance);
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x000D2C2C File Offset: 0x000D0E2C
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Officer officer = (Officer)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2168831419U)
				{
					if (num <= 502856387U)
					{
						if (num <= 218303523U)
						{
							if (num != 65480136U)
							{
								if (num == 218303523U)
								{
									if (text == "delinquentDays")
									{
										officer.delinquentDays = reader.Read<int>(ES3Type_int.Instance);
										continue;
									}
								}
							}
							else if (text == "charge_obstruction")
							{
								officer.charge_obstruction = reader.Read<int>(ES3Type_int.Instance);
								continue;
							}
						}
						else if (num != 404500587U)
						{
							if (num == 502856387U)
							{
								if (text == "activeCitations")
								{
									officer.activeCitations = reader.Read<int>(ES3Type_int.Instance);
									continue;
								}
							}
						}
						else if (text == "charge_theft")
						{
							officer.charge_theft = reader.Read<int>(ES3Type_int.Instance);
							continue;
						}
					}
					else if (num <= 894891827U)
					{
						if (num != 775709441U)
						{
							if (num == 894891827U)
							{
								if (text == "charge_speeding")
								{
									officer.charge_speeding = reader.Read<int>(ES3Type_int.Instance);
									continue;
								}
							}
						}
						else if (text == "charge_headlights")
						{
							officer.charge_headlights = reader.Read<int>(ES3Type_int.Instance);
							continue;
						}
					}
					else if (num != 1396016588U)
					{
						if (num == 2168831419U)
						{
							if (text == "numArrests")
							{
								officer.numArrests = reader.Read<int>(ES3Type_int.Instance);
								continue;
							}
						}
					}
					else if (text == "charge_dui")
					{
						officer.charge_dui = reader.Read<int>(ES3Type_int.Instance);
						continue;
					}
				}
				else if (num <= 3776974809U)
				{
					if (num <= 2813362504U)
					{
						if (num != 2748117893U)
						{
							if (num == 2813362504U)
							{
								if (text == "charge_reckless")
								{
									officer.charge_reckless = reader.Read<int>(ES3Type_int.Instance);
									continue;
								}
							}
						}
						else if (text == "charge_pubIntox")
						{
							officer.charge_pubIntox = reader.Read<int>(ES3Type_int.Instance);
							continue;
						}
					}
					else if (num != 3327005905U)
					{
						if (num == 3776974809U)
						{
							if (text == "charge_evasion")
							{
								officer.charge_evasion = reader.Read<int>(ES3Type_int.Instance);
								continue;
							}
						}
					}
					else if (text == "charge_opencontainer")
					{
						officer.charge_opencontainer = reader.Read<int>(ES3Type_int.Instance);
						continue;
					}
				}
				else if (num <= 4137135462U)
				{
					if (num != 4081090493U)
					{
						if (num == 4137135462U)
						{
							if (text == "warrant")
							{
								officer.warrant = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
					}
					else if (text == "charge_posession")
					{
						officer.charge_posession = reader.Read<int>(ES3Type_int.Instance);
						continue;
					}
				}
				else if (num != 4207410777U)
				{
					if (num == 4228510306U)
					{
						if (text == "raidWarrant")
						{
							officer.raidWarrant = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
				}
				else if (text == "charge_hitandrun")
				{
					officer.charge_hitandrun = reader.Read<int>(ES3Type_int.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x04002462 RID: 9314
		public static ES3Type Instance;
	}
}
