using System;
using EasyRoads3Dv3;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class runtimeScript : MonoBehaviour
{
	// Token: 0x060001A4 RID: 420 RVA: 0x000116F8 File Offset: 0x0000F8F8
	private void Start()
	{
		Debug.Log("Please read the comments at the top of the runtime script (/Assets/EasyRoads3D/Scripts/runtimeScript) before using the runtime API!");
		this.roadNetwork = new ERRoadNetwork();
		ERRoadType erroadType = new ERRoadType();
		erroadType.roadWidth = 6f;
		erroadType.roadMaterial = (Resources.Load("Materials/roads/road material") as Material);
		erroadType.layer = 1;
		erroadType.tag = "Untagged";
		Vector3[] markers = new Vector3[]
		{
			new Vector3(200f, 5f, 200f),
			new Vector3(250f, 5f, 200f),
			new Vector3(250f, 5f, 250f),
			new Vector3(300f, 5f, 250f)
		};
		this.road = this.roadNetwork.CreateRoad("road 1", erroadType, markers);
		this.road.AddMarker(new Vector3(300f, 5f, 300f));
		this.road.InsertMarker(new Vector3(275f, 5f, 235f));
		this.road.DeleteMarker(2);
		this.roadNetwork.BuildRoadNetwork();
		this.go = GameObject.CreatePrimitive(PrimitiveType.Cube);
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x00011840 File Offset: 0x0000FA40
	private void Update()
	{
		if (this.roadNetwork != null)
		{
			float num = Time.deltaTime * this.speed;
			this.distance += num;
			Vector3 position = this.road.GetPosition(this.distance, ref this.currentElement);
			position.y += 1f;
			this.go.transform.position = position;
		}
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x000118A9 File Offset: 0x0000FAA9
	private void OnDestroy()
	{
		if (this.roadNetwork != null && this.roadNetwork.isInBuildMode)
		{
			this.roadNetwork.RestoreRoadNetwork();
			Debug.Log("Restore Road Network");
		}
	}

	// Token: 0x0400048C RID: 1164
	public ERRoadNetwork roadNetwork;

	// Token: 0x0400048D RID: 1165
	public ERRoad road;

	// Token: 0x0400048E RID: 1166
	public GameObject go;

	// Token: 0x0400048F RID: 1167
	public int currentElement;

	// Token: 0x04000490 RID: 1168
	public float distance;

	// Token: 0x04000491 RID: 1169
	public float speed = 5f;
}
