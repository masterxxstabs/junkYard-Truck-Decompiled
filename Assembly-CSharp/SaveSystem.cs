using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Token: 0x0200012B RID: 299
public static class SaveSystem
{
	// Token: 0x060007C5 RID: 1989 RVA: 0x00063BAC File Offset: 0x00061DAC
	public static void SavePlayer(Currency currency)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = new FileStream(Application.persistentDataPath + "/player.pyr", FileMode.Create);
		PlayerData graph = new PlayerData(currency);
		binaryFormatter.Serialize(fileStream, graph);
		fileStream.Close();
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x00063BE8 File Offset: 0x00061DE8
	public static PlayerData LoadPlayer()
	{
		string path = Application.persistentDataPath + "/player.pyr";
		if (File.Exists(path))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(path, FileMode.Open);
			PlayerData result = binaryFormatter.Deserialize(fileStream) as PlayerData;
			fileStream.Close();
			return result;
		}
		Debug.Log("No save found");
		return null;
	}
}
