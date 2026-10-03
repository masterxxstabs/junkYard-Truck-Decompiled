using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class Bucket : MonoBehaviour
{
	// Token: 0x060000D0 RID: 208 RVA: 0x0000A3C4 File Offset: 0x000085C4
	private void Start()
	{
		this.keg = GameObject.Find("keg");
		this.aSource = base.GetComponent<AudioSource>();
		this.Ferment();
		if (!this.fill1.activeSelf && this.fullness > 0)
		{
			this.fill1.SetActive(true);
		}
		if (!this.fill2.activeSelf && this.fullness > 3)
		{
			this.fill2.SetActive(true);
		}
		if (!this.fill3.activeSelf && this.fullness > 6)
		{
			this.fill3.SetActive(true);
		}
		if (!this.fillWater.activeSelf && this.water >= 10f)
		{
			this.fillWater.SetActive(true);
		}
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x0000A480 File Offset: 0x00008680
	private void OnTriggerEnter(Collider other)
	{
		this.colName = other.gameObject.name;
		if (this.colName.Length > 4)
		{
			if (this.colName.Substring(0, 5) == "sugar")
			{
				if (this.fullness < 10)
				{
					this.fullness++;
					this.sugar++;
					Object.Destroy(other.gameObject);
				}
			}
			else if (this.colName.Substring(0, 5) == "cornm")
			{
				if (this.fullness < 10)
				{
					Object.Destroy(other.gameObject);
					this.fullness++;
					this.cornmeal++;
				}
			}
			else if (this.colName.Substring(0, 5) == "yeast")
			{
				Object.Destroy(other.gameObject);
				this.yeast++;
			}
			else if (this.colName == "waterstream")
			{
				this.water = 10f;
			}
			if (!this.fill1.activeSelf && this.fullness > 0)
			{
				this.fill1.SetActive(true);
			}
			if (!this.fill2.activeSelf && this.fullness > 3)
			{
				this.fill2.SetActive(true);
			}
			if (!this.fill3.activeSelf && this.fullness > 6)
			{
				this.fill3.SetActive(true);
			}
			if (!this.fillWater.activeSelf && this.water >= 10f)
			{
				this.fillWater.SetActive(true);
			}
		}
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x0000A62C File Offset: 0x0000882C
	public void Ferment()
	{
		if (this.water == 10f && this.yeast > 0 && this.fermentedPerc < 100 && this.bucketLid.GetComponent<Renderer>().enabled && (this.sugar > 0 || this.cornmeal > 0))
		{
			if (this.yeast > 0)
			{
				this.aSource.Play();
				this.fermentedPerc += 5;
				this.alcohol += this.sugar * 2;
				this.alcohol += this.cornmeal;
				this.quality += this.cornmeal;
				if (this.sugar > 0)
				{
					this.quality++;
				}
			}
			if (!this.aSource.isPlaying && this.fermentedPerc < 100)
			{
				this.aSource.Play();
			}
			if (this.aSource.isPlaying && this.fermentedPerc >= 100)
			{
				this.aSource.Stop();
			}
		}
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x0000A744 File Offset: 0x00008944
	public void FillKeg()
	{
		if (this.bucketLid.GetComponent<MeshRenderer>().enabled)
		{
			base.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = true;
			base.transform.parent.transform.parent = null;
			return;
		}
		if (this.water > 0f)
		{
			this.keg.GetComponent<Still>().AddBucket(this.alcohol, this.quality, base.transform.parent.gameObject);
			this.fill1.SetActive(false);
			this.fill2.SetActive(false);
			this.fill3.SetActive(false);
			this.fillWater.SetActive(false);
			this.fullness = 0;
			this.sugar = 0;
			this.water = 0f;
			this.cornmeal = 0;
			this.yeast = 0;
			this.fermentedPerc = 0;
			this.alcohol = 0;
			this.quality = 0;
			this.aSource.Stop();
			base.transform.parent.gameObject.SetActive(false);
			return;
		}
		base.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = true;
		base.transform.parent.transform.parent = null;
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x0000A894 File Offset: 0x00008A94
	public void DumpBucket()
	{
		if (this.bucketLid.GetComponent<MeshRenderer>().enabled)
		{
			base.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = true;
			base.transform.parent.transform.parent = null;
			return;
		}
		if (this.water > 0f || this.sugar > 0 || this.cornmeal > 0 || this.yeast > 0)
		{
			base.StartCoroutine(this.BucketAnim(base.transform.parent.gameObject));
			this.fill1.SetActive(false);
			this.fill2.SetActive(false);
			this.fill3.SetActive(false);
			this.fillWater.SetActive(false);
			this.fullness = 0;
			this.sugar = 0;
			this.water = 0f;
			this.cornmeal = 0;
			this.yeast = 0;
			this.fermentedPerc = 0;
			this.alcohol = 0;
			this.quality = 0;
			this.aSource.Stop();
			return;
		}
		base.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = true;
		base.transform.parent.transform.parent = null;
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x0000A9D9 File Offset: 0x00008BD9
	private IEnumerator BucketAnim(GameObject thisBucket)
	{
		thisBucket.GetComponent<Renderer>().enabled = false;
		this.bucketVisual.GetComponent<MeshRenderer>().enabled = true;
		this.bucketVisual.GetComponent<InteractiveObject>().PerformAction();
		yield return new WaitForSeconds(1f);
		this.bucketVisual.GetComponent<MeshRenderer>().enabled = false;
		this.bucketVisual.GetComponent<InteractiveObject>().PerformAction();
		thisBucket.GetComponent<Renderer>().enabled = true;
		thisBucket.GetComponent<Rigidbody>().useGravity = true;
		thisBucket.transform.parent = null;
		yield break;
	}

	// Token: 0x04000200 RID: 512
	private int fullness;

	// Token: 0x04000201 RID: 513
	public float water;

	// Token: 0x04000202 RID: 514
	private string colName;

	// Token: 0x04000203 RID: 515
	public int sugar;

	// Token: 0x04000204 RID: 516
	public int cornmeal;

	// Token: 0x04000205 RID: 517
	public int yeast;

	// Token: 0x04000206 RID: 518
	public int fermentedPerc;

	// Token: 0x04000207 RID: 519
	public int alcohol;

	// Token: 0x04000208 RID: 520
	public int quality;

	// Token: 0x04000209 RID: 521
	public GameObject fill1;

	// Token: 0x0400020A RID: 522
	public GameObject fill2;

	// Token: 0x0400020B RID: 523
	public GameObject fill3;

	// Token: 0x0400020C RID: 524
	public GameObject fillWater;

	// Token: 0x0400020D RID: 525
	public GameObject waterVisual;

	// Token: 0x0400020E RID: 526
	public GameObject bucketLid;

	// Token: 0x0400020F RID: 527
	public GameObject keg;

	// Token: 0x04000210 RID: 528
	private AudioSource aSource;

	// Token: 0x04000211 RID: 529
	public GameObject bucketVisual;

	// Token: 0x04000212 RID: 530
	private int sugarhalf;
}
