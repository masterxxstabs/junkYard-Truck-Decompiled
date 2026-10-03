using System;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class boltturn : MonoBehaviour
{
	// Token: 0x060008D2 RID: 2258 RVA: 0x000719F0 File Offset: 0x0006FBF0
	private void Start()
	{
		this.aSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x00071A00 File Offset: 0x0006FC00
	private void Update()
	{
		if (Input.GetAxis("Mouse ScrollWheel") > 0f && this.numTurns < 10f)
		{
			base.transform.Rotate(0f, 0f, -30f, Space.Self);
			base.transform.Translate(Vector3.forward * 0.01f);
			this.numTurns += 1f;
			if (this.numTurns % 2f == 0f)
			{
				this.aSource.Play();
			}
			float num = this.numTurns;
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0f && this.numTurns > 0f)
		{
			base.transform.Rotate(0f, 0f, 30f, Space.Self);
			base.transform.Translate(-Vector3.forward * 0.01f);
			this.numTurns -= 1f;
			if (this.numTurns < 1f)
			{
				Debug.Log("ss");
				Debug.Log(base.transform.parent);
			}
			if (this.numTurns % 2f == 0f)
			{
				this.aSource.Play();
			}
		}
	}

	// Token: 0x04001455 RID: 5205
	private float numTurns;

	// Token: 0x04001456 RID: 5206
	private AudioSource aSource;
}
