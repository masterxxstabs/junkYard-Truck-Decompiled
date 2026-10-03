using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000134 RID: 308
public class QM_ResourceManager : MonoBehaviour
{
	// Token: 0x06000803 RID: 2051 RVA: 0x0006BD55 File Offset: 0x00069F55
	private void Start()
	{
		base.InvokeRepeating("RespawnTree", 15f, 15f);
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0006BD6C File Offset: 0x00069F6C
	private void RespawnTree()
	{
		if (this.managedTrees.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.managedTrees.Count; i++)
		{
			if (this.managedTrees[i].respawnTime < Time.time)
			{
				Object.Destroy(this.managedTrees[i].marker.gameObject);
				this.managedTrees.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0006BDDD File Offset: 0x00069FDD
	public void AddTerrainTree(string _terrainName, int _treeIDX, float _respawnTime, Transform _marker)
	{
		this.managedTrees.Add(new QM_ResourceManager.QM_Tree(_terrainName, _treeIDX, _respawnTime, _marker));
	}

	// Token: 0x040012D3 RID: 4819
	public List<QM_ResourceManager.QM_Tree> managedTrees = new List<QM_ResourceManager.QM_Tree>();

	// Token: 0x0200041E RID: 1054
	public class QM_Tree
	{
		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06001941 RID: 6465 RVA: 0x000F2C9C File Offset: 0x000F0E9C
		// (set) Token: 0x06001942 RID: 6466 RVA: 0x000F2CA4 File Offset: 0x000F0EA4
		public string terrainName { get; set; }

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06001943 RID: 6467 RVA: 0x000F2CAD File Offset: 0x000F0EAD
		// (set) Token: 0x06001944 RID: 6468 RVA: 0x000F2CB5 File Offset: 0x000F0EB5
		public int treeINDEX { get; set; }

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06001945 RID: 6469 RVA: 0x000F2CBE File Offset: 0x000F0EBE
		// (set) Token: 0x06001946 RID: 6470 RVA: 0x000F2CC6 File Offset: 0x000F0EC6
		public float respawnTime { get; set; }

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06001947 RID: 6471 RVA: 0x000F2CCF File Offset: 0x000F0ECF
		// (set) Token: 0x06001948 RID: 6472 RVA: 0x000F2CD7 File Offset: 0x000F0ED7
		public Transform marker { get; set; }

		// Token: 0x06001949 RID: 6473 RVA: 0x000F2CE0 File Offset: 0x000F0EE0
		public QM_Tree(string _terrainName, int _treeINDEX, float _respawnTime, Transform _marker)
		{
			this.terrainName = _terrainName;
			this.treeINDEX = _treeINDEX;
			this.respawnTime = _respawnTime;
			this.marker = _marker;
		}
	}
}
