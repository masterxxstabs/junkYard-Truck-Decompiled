using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class HydraulicLift : MonoBehaviour
{
	// Token: 0x0600046B RID: 1131 RVA: 0x0002F490 File Offset: 0x0002D690
	private void Start()
	{
		this.DownPosition = this.arms.transform.position;
		this.UpPosition = base.transform.position + new Vector3(0f, this.LiftHeight, 0f);
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0002F4DE File Offset: 0x0002D6DE
	public void UseLift()
	{
		if (this.IsUp && !this.Lifting)
		{
			this.LiftDown();
			return;
		}
		if (!this.IsUp && !this.Lifting)
		{
			this.LiftUp();
		}
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x0002F50D File Offset: 0x0002D70D
	private IEnumerator Moving(Vector3 From, Vector3 To)
	{
		this.Lifting = true;
		this.aSource.Play();
		this.liftCollider.SetActive(true);
		if (!this.IsUp)
		{
			yield return new WaitForSeconds(1f);
		}
		for (float i = 0f; i < 1f; i += 0.007f)
		{
			this.arms.transform.position = Vector3.Lerp(From, To, i);
			yield return null;
		}
		this.Lifting = false;
		if (!this.IsUp)
		{
			this.anim.Play("droparms");
			this.liftCollider.SetActive(false);
		}
		yield break;
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x0002F52C File Offset: 0x0002D72C
	public void LiftUp()
	{
		if (this.Lifting || this.IsUp)
		{
			return;
		}
		this.anim.Play("liftarms");
		base.StartCoroutine(this.Moving(this.DownPosition, this.UpPosition));
		this.IsUp = true;
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x0002F57B File Offset: 0x0002D77B
	public void LiftDown()
	{
		if (this.Lifting || !this.IsUp)
		{
			return;
		}
		base.StartCoroutine(this.Moving(this.UpPosition, this.DownPosition));
		this.IsUp = false;
	}

	// Token: 0x0400093B RID: 2363
	private bool Lifting;

	// Token: 0x0400093C RID: 2364
	private bool IsUp;

	// Token: 0x0400093D RID: 2365
	public Animation anim;

	// Token: 0x0400093E RID: 2366
	public Transform arm1;

	// Token: 0x0400093F RID: 2367
	public Transform arm2;

	// Token: 0x04000940 RID: 2368
	public Transform arm3;

	// Token: 0x04000941 RID: 2369
	public Transform arm4;

	// Token: 0x04000942 RID: 2370
	public float LiftHeight;

	// Token: 0x04000943 RID: 2371
	public GameObject arms;

	// Token: 0x04000944 RID: 2372
	public GameObject liftCollider;

	// Token: 0x04000945 RID: 2373
	public AudioSource aSource;

	// Token: 0x04000946 RID: 2374
	public Vector3 DownPosition;

	// Token: 0x04000947 RID: 2375
	public Vector3 UpPosition;
}
