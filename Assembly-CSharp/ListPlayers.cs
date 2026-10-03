using System;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class ListPlayers : MonoBehaviour
{
	// Token: 0x060005CE RID: 1486 RVA: 0x000477D4 File Offset: 0x000459D4
	private void Start()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		Debug.Log("Number of Players found: " + array.Length);
		foreach (GameObject gameObject in array)
		{
			Debug.Log("Found Player: " + gameObject.name);
		}
	}
}
