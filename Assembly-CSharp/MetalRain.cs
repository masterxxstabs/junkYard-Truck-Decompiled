using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class MetalRain : MonoBehaviour
{
	// Token: 0x06000491 RID: 1169 RVA: 0x0002FDE3 File Offset: 0x0002DFE3
	private void Start()
	{
		base.StartCoroutine(this.Rain());
		this.impactDeformable = base.GetComponent<ImpactDeformable>();
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0002FDFE File Offset: 0x0002DFFE
	private IEnumerator Rain()
	{
		for (;;)
		{
			Object.Instantiate<GameObject>(this.RainObject, new Vector3((Random.value - 0.5f) * 20f, Random.value * 40f + 10f, (Random.value - 0.5f) * 20f), Quaternion.identity);
			yield return new WaitForSeconds(Random.value * 0.2f + 0.1f);
		}
		yield break;
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0002FE10 File Offset: 0x0002E010
	private void OnMouseDown()
	{
		this.impactDeformable.Repair(0.25f, null, null);
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0002FE3F File Offset: 0x0002E03F
	public void SetHardness(float value)
	{
		this.impactDeformable.Hardness = value;
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0002FE4D File Offset: 0x0002E04D
	public void SetDeformMeshCollider(bool fd)
	{
		this.impactDeformable.DeformMeshCollider = fd;
	}

	// Token: 0x0400095A RID: 2394
	public GameObject RainObject;

	// Token: 0x0400095B RID: 2395
	private ImpactDeformable impactDeformable;
}
