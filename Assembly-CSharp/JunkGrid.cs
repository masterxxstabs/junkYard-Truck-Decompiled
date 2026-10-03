using System;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class JunkGrid : MonoBehaviour
{
	// Token: 0x06000582 RID: 1410 RVA: 0x000450F8 File Offset: 0x000432F8
	public void SpawnInsideCircle()
	{
		Vector3 b = Random.insideUnitSphere * Random.Range(this.minRadius, this.maxRadius);
		Vector3 vector = this.player.position + b;
		this.lastPos = this.player.position;
		this.xPosF = vector.x / this.terrainOb.terrainData.size.x;
		this.yPosF = vector.z / this.terrainOb.terrainData.size.z;
		this.finalPos = new Vector3(vector.x, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.yPosF) + 0.5f, vector.z);
		this.canSpawn = true;
		if (Vector3.Distance(this.truckPos.position, this.finalPos) < 15f)
		{
			this.canSpawn = false;
		}
		if (Vector3.Distance(this.carPos.position, this.finalPos) < 15f)
		{
			this.canSpawn = false;
		}
		if (Vector3.Distance(this.garagePos.position, this.finalPos) < 15f)
		{
			this.canSpawn = false;
		}
		if (Vector3.Distance(this.jyPos.position, this.finalPos) < 30f)
		{
			this.canSpawn = false;
		}
		if (Vector3.Distance(this.townPos.position, this.finalPos) < 30f)
		{
			this.canSpawn = false;
		}
		if (this.canSpawn)
		{
			int num = Random.Range(0, this.junkOb.Length);
			GameObject gameObject = Object.Instantiate<GameObject>(this.junkOb[num]);
			gameObject.transform.position = this.finalPos;
			gameObject.transform.parent = this.junkContainer;
		}
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x000452C4 File Offset: 0x000434C4
	public void NewJunk()
	{
		this.canSpawn = true;
		if (Random.Range(1, 6) == 1 && Vector3.Distance(this.player.position, this.lastPos) > 30f)
		{
			if (this.junkContainer.childCount < 13)
			{
				this.SpawnInsideCircle();
				return;
			}
			Object.Destroy(this.junkContainer.GetChild(0).gameObject);
			Object.Destroy(this.junkContainer.GetChild(1).gameObject);
			this.SpawnInsideCircle();
		}
	}

	// Token: 0x04000BE4 RID: 3044
	public GameObject[] junkOb;

	// Token: 0x04000BE5 RID: 3045
	public Transform player;

	// Token: 0x04000BE6 RID: 3046
	public Terrain terrainOb;

	// Token: 0x04000BE7 RID: 3047
	private Vector3 lastPos;

	// Token: 0x04000BE8 RID: 3048
	public float minRadius = 25f;

	// Token: 0x04000BE9 RID: 3049
	public float maxRadius = 45f;

	// Token: 0x04000BEA RID: 3050
	private float xPosF;

	// Token: 0x04000BEB RID: 3051
	private float yPosF;

	// Token: 0x04000BEC RID: 3052
	public Transform junkContainer;

	// Token: 0x04000BED RID: 3053
	private Vector3 finalPos;

	// Token: 0x04000BEE RID: 3054
	public Transform truckPos;

	// Token: 0x04000BEF RID: 3055
	public Transform carPos;

	// Token: 0x04000BF0 RID: 3056
	public Transform garagePos;

	// Token: 0x04000BF1 RID: 3057
	public Transform jyPos;

	// Token: 0x04000BF2 RID: 3058
	public Transform townPos;

	// Token: 0x04000BF3 RID: 3059
	private bool canSpawn;
}
