using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BA RID: 186
public class GroceryBag : MonoBehaviour
{
	// Token: 0x06000460 RID: 1120 RVA: 0x0002F307 File Offset: 0x0002D507
	public void OpenBag()
	{
		if (this.emptying == null)
		{
			this.emptying = base.StartCoroutine(this.OpeningBag());
		}
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x0002F323 File Offset: 0x0002D523
	private IEnumerator OpeningBag()
	{
		if (this.cramCount > 0)
		{
			int num;
			for (int i = 0; i < this.cramCount; i = num + 1)
			{
				Object.Instantiate<GameObject>(this.cram, base.transform.position, base.transform.rotation);
				yield return new WaitForSeconds(0.2f);
				num = i;
			}
		}
		if (this.beefareenoCount > 0)
		{
			int num;
			for (int i = 0; i < this.beefareenoCount; i = num + 1)
			{
				Object.Instantiate<GameObject>(this.beefareeno, base.transform.position, base.transform.rotation);
				yield return new WaitForSeconds(0.2f);
				num = i;
			}
		}
		if (this.fuegoCount > 0)
		{
			int num;
			for (int i = 0; i < this.fuegoCount; i = num + 1)
			{
				Object.Instantiate<GameObject>(this.fuego, base.transform.position, base.transform.rotation);
				yield return new WaitForSeconds(0.2f);
				num = i;
			}
		}
		if (this.sugarCount > 0)
		{
			int num;
			for (int i = 0; i < this.sugarCount; i = num + 1)
			{
				Object.Instantiate<GameObject>(this.sugar, base.transform.position, base.transform.rotation);
				yield return new WaitForSeconds(0.2f);
				num = i;
			}
		}
		if (this.cornmealCount > 0)
		{
			int num;
			for (int i = 0; i < this.cornmealCount; i = num + 1)
			{
				Object.Instantiate<GameObject>(this.cornmeal, base.transform.position, base.transform.rotation);
				yield return new WaitForSeconds(0.2f);
				num = i;
			}
		}
		if (this.yeastCount > 0)
		{
			int num;
			for (int i = 0; i < this.yeastCount; i = num + 1)
			{
				Object.Instantiate<GameObject>(this.yeast, base.transform.position, base.transform.rotation);
				yield return new WaitForSeconds(0.2f);
				num = i;
			}
		}
		if (this.nrgCount > 0)
		{
			int num;
			for (int i = 0; i < this.nrgCount; i = num + 1)
			{
				Object.Instantiate<GameObject>(this.nrgDrink, base.transform.position, base.transform.rotation);
				yield return new WaitForSeconds(0.2f);
				num = i;
			}
		}
		if (this.zenCount > 0)
		{
			int num;
			for (int i = 0; i < this.zenCount; i = num + 1)
			{
				Object.Instantiate<GameObject>(this.zen, base.transform.position, base.transform.rotation);
				yield return new WaitForSeconds(0.2f);
				num = i;
			}
		}
		yield return new WaitForSeconds(0.1f);
		this.emptying = null;
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04000920 RID: 2336
	public int cramCount;

	// Token: 0x04000921 RID: 2337
	public int beefareenoCount;

	// Token: 0x04000922 RID: 2338
	public int fuegoCount;

	// Token: 0x04000923 RID: 2339
	public int sugarCount;

	// Token: 0x04000924 RID: 2340
	public int cornmealCount;

	// Token: 0x04000925 RID: 2341
	public int yeastCount;

	// Token: 0x04000926 RID: 2342
	public int nrgCount;

	// Token: 0x04000927 RID: 2343
	public int zenCount;

	// Token: 0x04000928 RID: 2344
	public GameObject cram;

	// Token: 0x04000929 RID: 2345
	public GameObject beefareeno;

	// Token: 0x0400092A RID: 2346
	public GameObject fuego;

	// Token: 0x0400092B RID: 2347
	public GameObject sugar;

	// Token: 0x0400092C RID: 2348
	public GameObject cornmeal;

	// Token: 0x0400092D RID: 2349
	public GameObject yeast;

	// Token: 0x0400092E RID: 2350
	public GameObject nrgDrink;

	// Token: 0x0400092F RID: 2351
	public GameObject zen;

	// Token: 0x04000930 RID: 2352
	private Coroutine emptying;
}
