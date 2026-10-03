using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
public class DeadTreeMgr : MonoBehaviour
{
	// Token: 0x06000147 RID: 327 RVA: 0x0000F0C0 File Offset: 0x0000D2C0
	private void Start()
	{
		for (int i = 0; i < this.treeLoc.Length; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.treePrefab, this.treeLoc[i].position, Quaternion.identity);
			gameObject.transform.parent = this.treeContainer;
			this.trees[i] = gameObject;
			gameObject.name = "cuttableTree" + i;
			if (this.cutTimes[i] > 0 && this.curr.playMinutes - this.cutTimes[i] < 120)
			{
				gameObject.transform.GetChild(1).gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000148 RID: 328 RVA: 0x0000F170 File Offset: 0x0000D370
	public void UpdateStumps()
	{
		for (int i = 0; i < this.treeLoc.Length; i++)
		{
			if (this.cutTimes[i] > 0 && this.curr.playMinutes - this.cutTimes[i] > 119)
			{
				Object.Destroy(this.trees[i]);
				GameObject gameObject = Object.Instantiate<GameObject>(this.treePrefab, this.treeLoc[i].position, Quaternion.identity);
				this.trees[i] = gameObject;
				gameObject.name = "cuttableTree" + i;
				gameObject.transform.parent = this.treeContainer;
			}
		}
	}

	// Token: 0x04000391 RID: 913
	public Currency curr;

	// Token: 0x04000392 RID: 914
	public GameObject treePrefab;

	// Token: 0x04000393 RID: 915
	public Transform[] treeLoc;

	// Token: 0x04000394 RID: 916
	public int[] cutTimes;

	// Token: 0x04000395 RID: 917
	public GameObject[] trees;

	// Token: 0x04000396 RID: 918
	public Transform treeContainer;
}
