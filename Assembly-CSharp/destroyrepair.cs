using System;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class destroyrepair : MonoBehaviour
{
	// Token: 0x06000980 RID: 2432 RVA: 0x0007FADC File Offset: 0x0007DCDC
	private void Start()
	{
		for (int i = 0; i < this.remainderOfTheObject.Length; i++)
		{
			this.remainderOfTheObject[i].SetActive(false);
		}
		this.brokenAudio = base.gameObject.AddComponent<AudioSource>();
		this.brokenAudio.loop = false;
		this.brokenAudio.volume = 1f;
		this.brokenAudio.spatialBlend = 1f;
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x0007FB47 File Offset: 0x0007DD47
	private void Update()
	{
		this.Fracture();
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x0007FB50 File Offset: 0x0007DD50
	private void Fracture()
	{
		if (this.destroy && !this.destroyed)
		{
			this.cloneOfPrefab = Object.Instantiate<GameObject>(this.destroyedPrefab, base.transform.root.position, base.transform.root.rotation);
			base.transform.GetComponent<MeshRenderer>().enabled = false;
			if (base.transform.GetComponent<MeshCollider>() != null)
			{
				base.transform.GetComponent<MeshCollider>().enabled = false;
			}
			if (this.brokenSound.Length != 0)
			{
				this.brokenAudio.clip = this.brokenSound[Random.Range(0, this.brokenSound.Length)];
				this.brokenAudio.Play();
			}
			foreach (Collider collider in Physics.OverlapSphere(base.transform.position, this.radius))
			{
				if (collider.attachedRigidbody != null)
				{
					collider.attachedRigidbody.AddExplosionForce(this.force, base.transform.position, this.radius, this.upwardModifier, this.forceMode);
				}
				for (int j = 0; j < this.remainderOfTheObject.Length; j++)
				{
					this.remainderOfTheObject[j].SetActive(true);
				}
			}
		}
		if (!this.destroy && this.destroyed)
		{
			base.transform.GetComponent<MeshRenderer>().enabled = true;
			if (base.transform.GetComponent<MeshCollider>() != null)
			{
				base.transform.GetComponent<MeshCollider>().enabled = true;
			}
			Object.Destroy(this.cloneOfPrefab);
			for (int k = 0; k < this.remainderOfTheObject.Length; k++)
			{
				this.remainderOfTheObject[k].SetActive(false);
			}
		}
		this.destroyed = this.destroy;
	}

	// Token: 0x04001962 RID: 6498
	public GameObject destroyedPrefab;

	// Token: 0x04001963 RID: 6499
	private GameObject cloneOfPrefab;

	// Token: 0x04001964 RID: 6500
	public GameObject[] remainderOfTheObject;

	// Token: 0x04001965 RID: 6501
	public bool destroy;

	// Token: 0x04001966 RID: 6502
	public bool destroyed;

	// Token: 0x04001967 RID: 6503
	public float force = 125f;

	// Token: 0x04001968 RID: 6504
	public float radius = 5f;

	// Token: 0x04001969 RID: 6505
	public float upwardModifier = 0.1f;

	// Token: 0x0400196A RID: 6506
	public ForceMode forceMode;

	// Token: 0x0400196B RID: 6507
	public AudioClip[] brokenSound;

	// Token: 0x0400196C RID: 6508
	private AudioSource brokenAudio;
}
