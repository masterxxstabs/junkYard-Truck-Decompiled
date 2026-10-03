using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class Gasstationchime : MonoBehaviour
{
	// Token: 0x06000455 RID: 1109 RVA: 0x0002EEA7 File Offset: 0x0002D0A7
	private void Start()
	{
		if (this.aSource == null)
		{
			this.aSource = base.GetComponent<AudioSource>();
		}
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x0002EEC4 File Offset: 0x0002D0C4
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			this.inside = !this.inside;
			Vector3 forward = this.camera.forward;
			float num = Vector3.Dot(base.transform.forward, forward);
			if (num > 0.5f)
			{
				if (this.inside)
				{
					this.clerk.Entering();
				}
			}
			else if (num < -0.5f && !this.inside)
			{
				this.clerk.Exiting();
			}
			if (!this.buzzerBusy)
			{
				this.buzzerBusy = true;
				this.aSource.Play();
				base.StartCoroutine(this.BuzzerDelay());
			}
		}
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x0002EF72 File Offset: 0x0002D172
	private IEnumerator BuzzerDelay()
	{
		yield return new WaitForSeconds(10f);
		this.buzzerBusy = false;
		yield break;
	}

	// Token: 0x040008FD RID: 2301
	public AudioSource aSource;

	// Token: 0x040008FE RID: 2302
	private bool buzzerBusy;

	// Token: 0x040008FF RID: 2303
	public bool inside;

	// Token: 0x04000900 RID: 2304
	public Transform camera;

	// Token: 0x04000901 RID: 2305
	public Clerk clerk;
}
