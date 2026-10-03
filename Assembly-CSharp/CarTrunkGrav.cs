using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class CarTrunkGrav : MonoBehaviour
{
	// Token: 0x06000124 RID: 292 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x06000125 RID: 293 RVA: 0x0000E484 File Offset: 0x0000C684
	private void Update()
	{
		if (Time.time >= (float)this.weightCheckInterval)
		{
			this.weightCheckInterval = Mathf.FloorToInt(Time.time) + 2;
			if (Vector3.Dot(this.car.up, Vector3.down) > 0f)
			{
				this.EmptyBed();
				return;
			}
			this.CheckBed();
		}
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00002188 File Offset: 0x00000388
	public void EnableBed()
	{
	}

	// Token: 0x06000127 RID: 295 RVA: 0x0000E4DC File Offset: 0x0000C6DC
	public void CheckBed()
	{
		this.itemsInsideBed = Physics.OverlapBox(base.transform.position, base.transform.localScale, base.transform.rotation);
		this.haulWeight = 0f;
		foreach (Collider collider in this.itemsInsideBed)
		{
			if (collider.GetComponent<Rigidbody>() != null)
			{
				Rigidbody component = collider.GetComponent<Rigidbody>();
				if (component != null && component.velocity.x < 0.05f && component.velocity.y < 0.05f && component.velocity.z < 0.05f && (collider.transform.parent == null || collider.transform.parent == this.car) && collider.GetComponent<PickUp>() != null)
				{
					if (collider.GetComponent<FixedJoint>() == null)
					{
						collider.gameObject.AddComponent<FixedJoint>().connectedBody = this.car.GetComponent<Rigidbody>();
						component.angularDrag = 0f;
						component.drag = 0f;
						component.mass = 1f;
					}
					float trueMass = collider.GetComponent<PickUp>().trueMass;
					this.haulWeight += trueMass;
				}
			}
		}
		this.car.GetComponent<Rigidbody>().mass = 1500f + this.haulWeight;
		float num = 0.3f - this.haulWeight / 5000f;
		if (num < 0.1f)
		{
			num = 0.1f;
		}
		this.wcRR.suspensionDistance = num;
		this.wcRL.suspensionDistance = num;
	}

	// Token: 0x06000128 RID: 296 RVA: 0x0000E6A0 File Offset: 0x0000C8A0
	public void EmptyBed()
	{
		this.itemsInsideBed = Physics.OverlapBox(base.transform.position, base.transform.localScale / 2f, Quaternion.identity);
		foreach (Collider collider in this.itemsInsideBed)
		{
			if (collider.GetComponent<Rigidbody>() != null && collider.GetComponent<PickUp>() != null)
			{
				collider.GetComponent<Rigidbody>().mass = collider.GetComponent<PickUp>().trueMass;
				Object.Destroy(collider.GetComponent<FixedJoint>());
			}
		}
		this.car.GetComponent<Rigidbody>().mass = 1500f;
	}

	// Token: 0x06000129 RID: 297 RVA: 0x0000E748 File Offset: 0x0000C948
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(base.transform.position, base.transform.localScale);
	}

	// Token: 0x04000346 RID: 838
	public Transform car;

	// Token: 0x04000347 RID: 839
	public Collider[] itemsInsideBed;

	// Token: 0x04000348 RID: 840
	private int weightCheckInterval = 3;

	// Token: 0x04000349 RID: 841
	public float haulWeight;

	// Token: 0x0400034A RID: 842
	public WheelCollider wcRR;

	// Token: 0x0400034B RID: 843
	public WheelCollider wcRL;
}
