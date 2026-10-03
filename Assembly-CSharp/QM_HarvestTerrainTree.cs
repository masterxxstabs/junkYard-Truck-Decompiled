using System;
using UnityEngine;

// Token: 0x02000133 RID: 307
public class QM_HarvestTerrainTree : MonoBehaviour
{
	// Token: 0x060007FD RID: 2045 RVA: 0x0006B9A4 File Offset: 0x00069BA4
	private void Start()
	{
		if (this.harvestTreeDistance <= 0)
		{
			Debug.Log("harvestTreeDistance unset in Inspector, using value: 6");
			this.harvestTreeDistance = 6;
		}
		if (this.respawnTimer <= 0f)
		{
			Debug.Log("respawnTimer unset in Inspector, using quick test value: 15");
			this.respawnTimer = 15f;
		}
		this.myTransform = base.transform;
		this.lastTerrain = null;
		this.rMgr = GameObject.FindGameObjectWithTag("GameController").GetComponent<QM_ResourceManager>();
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0006BA18 File Offset: 0x00069C18
	private void Update()
	{
		if (Input.GetMouseButtonUp(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out this.hit, 30f))
		{
			if (this.hit.collider.gameObject.GetComponent<Terrain>() == null)
			{
				return;
			}
			if (this.lastTerrain == null || this.lastTerrain != this.hit.collider.name)
			{
				this.terrain = this.hit.collider.gameObject.GetComponent<Terrain>();
				this.lastTerrain = this.terrain.name;
			}
			float num = this.terrain.SampleHeight(this.hit.point);
			if (this.hit.point.y - 0.2f > num && this.CheckProximity())
			{
				this.HarvestWood();
			}
		}
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0006BB04 File Offset: 0x00069D04
	private bool CheckProximity()
	{
		bool result = true;
		if (Vector3.Distance(this.myTransform.position, this.hit.point) > (float)this.harvestTreeDistance)
		{
			Debug.Log("Out of Range");
			result = false;
		}
		return result;
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0006BB44 File Offset: 0x00069D44
	private bool CheckRecentUsage(string _terrainName, int _treeINDEX)
	{
		bool result = false;
		for (int i = 0; i < this.rMgr.managedTrees.Count; i++)
		{
			if (this.rMgr.managedTrees[i].terrainName == _terrainName && this.rMgr.managedTrees[i].treeINDEX == _treeINDEX)
			{
				Debug.Log("Tree has been used recently");
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0006BBB4 File Offset: 0x00069DB4
	private void HarvestWood()
	{
		int num = -1;
		int num2 = this.terrain.terrainData.treeInstances.Length;
		float num3 = (float)this.harvestTreeDistance;
		Vector3 position = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < num2; i++)
		{
			Vector3 vector = Vector3.Scale(this.terrain.terrainData.treeInstances[i].position, this.terrain.terrainData.size) + this.terrain.transform.position;
			float num4 = Vector3.Distance(vector, this.hit.point);
			if (num4 < num3)
			{
				num = i;
				num3 = num4;
				position = vector;
			}
		}
		if (num == -1)
		{
			Debug.Log("Out of Range");
			return;
		}
		if (!this.CheckRecentUsage(this.terrain.name, num))
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gameObject.transform.position = position;
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.felledTree, position, Quaternion.identity);
			gameObject2.gameObject.AddComponent<Rigidbody>();
			Object.Destroy(gameObject2, 4f);
			this.rMgr.AddTerrainTree(this.terrain.name, num, Time.time + this.respawnTimer, gameObject.transform);
			if (this.rotatePlayer)
			{
				Vector3 worldPosition = new Vector3(this.hit.point.x, this.myTransform.position.y, this.hit.point.z);
				this.myTransform.LookAt(worldPosition);
			}
		}
	}

	// Token: 0x040012CA RID: 4810
	public int harvestTreeDistance;

	// Token: 0x040012CB RID: 4811
	public bool rotatePlayer = true;

	// Token: 0x040012CC RID: 4812
	private Transform myTransform;

	// Token: 0x040012CD RID: 4813
	private Terrain terrain;

	// Token: 0x040012CE RID: 4814
	private RaycastHit hit;

	// Token: 0x040012CF RID: 4815
	private string lastTerrain;

	// Token: 0x040012D0 RID: 4816
	public GameObject felledTree;

	// Token: 0x040012D1 RID: 4817
	private QM_ResourceManager rMgr;

	// Token: 0x040012D2 RID: 4818
	public float respawnTimer;
}
