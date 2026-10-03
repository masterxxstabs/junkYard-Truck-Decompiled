using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class Dumpster : MonoBehaviour
{
	// Token: 0x06000199 RID: 409 RVA: 0x000115E4 File Offset: 0x0000F7E4
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<PickUp>() != null && other.gameObject.GetComponent<PickUp>().tradein > 0f)
		{
			Object.Destroy(other.gameObject);
			int num = Random.Range(0, this.clips.Length);
			this.aSource.clip = this.clips[num];
			this.aSource.Play();
		}
	}

	// Token: 0x04000485 RID: 1157
	public AudioSource aSource;

	// Token: 0x04000486 RID: 1158
	public AudioClip[] clips;
}
