using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002EE RID: 750
	[Preserve]
	[ES3Properties(new string[]
	{
		"spawnItems",
		"junkContainer",
		"bikeContainer",
		"spawn250Items",
		"spawned18",
		"spawnedv8s",
		"v8transform",
		"v8junk"
	})]
	public class ES3UserType_JunkSpawner : ES3ComponentType
	{
		// Token: 0x060013DC RID: 5084 RVA: 0x000D1F05 File Offset: 0x000D0105
		public ES3UserType_JunkSpawner() : base(typeof(JunkSpawner))
		{
			ES3UserType_JunkSpawner.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x000D1F24 File Offset: 0x000D0124
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			JunkSpawner junkSpawner = (JunkSpawner)obj;
			writer.WriteProperty("spawnItems", junkSpawner.spawnItems, ES3Type_GameObjectArray.Instance);
			writer.WritePropertyByRef("junkContainer", junkSpawner.junkContainer);
			writer.WritePropertyByRef("bikeContainer", junkSpawner.bikeContainer);
			writer.WriteProperty("spawn250Items", junkSpawner.spawn250Items, ES3Type_GameObjectArray.Instance);
			writer.WriteProperty("spawned18", junkSpawner.spawned18, ES3Type_bool.Instance);
			writer.WritePrivateField("spawnedv8s", junkSpawner);
			writer.WriteProperty("v8transform", junkSpawner.v8transform, ES3UserType_TransformArray.Instance);
			writer.WritePropertyByRef("v8junk", junkSpawner.v8junk);
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x000D1FD4 File Offset: 0x000D01D4
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			JunkSpawner junkSpawner = (JunkSpawner)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 831117380U)
				{
					if (num <= 347202079U)
					{
						if (num != 231729116U)
						{
							if (num == 347202079U)
							{
								if (text == "bikeContainer")
								{
									junkSpawner.bikeContainer = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "spawnItems")
						{
							junkSpawner.spawnItems = reader.Read<GameObject[]>(ES3Type_GameObjectArray.Instance);
							continue;
						}
					}
					else if (num != 695258549U)
					{
						if (num == 831117380U)
						{
							if (text == "spawnedv8s")
							{
								reader.SetPrivateField("spawnedv8s", reader.Read<bool>(), junkSpawner);
								continue;
							}
						}
					}
					else if (text == "v8transform")
					{
						junkSpawner.v8transform = reader.Read<Transform[]>(ES3UserType_TransformArray.Instance);
						continue;
					}
				}
				else if (num <= 2479165926U)
				{
					if (num != 923445317U)
					{
						if (num == 2479165926U)
						{
							if (text == "junkContainer")
							{
								junkSpawner.junkContainer = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
					}
					else if (text == "spawn250Items")
					{
						junkSpawner.spawn250Items = reader.Read<GameObject[]>(ES3Type_GameObjectArray.Instance);
						continue;
					}
				}
				else if (num != 2583262645U)
				{
					if (num == 4003991104U)
					{
						if (text == "spawned18")
						{
							junkSpawner.spawned18 = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
				}
				else if (text == "v8junk")
				{
					junkSpawner.v8junk = reader.Read<GameObject>(ES3Type_GameObject.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x0400245A RID: 9306
		public static ES3Type Instance;
	}
}
