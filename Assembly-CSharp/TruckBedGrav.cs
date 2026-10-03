using System;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class TruckBedGrav : MonoBehaviour
{
	// Token: 0x06000897 RID: 2199 RVA: 0x00002188 File Offset: 0x00000388
	private void Update()
	{
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x00002188 File Offset: 0x00000388
	public void EnableBed()
	{
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0006F790 File Offset: 0x0006D990
	public void CheckBed()
	{
		this.itemsInsideBed = Physics.OverlapBox(base.transform.position, base.transform.localScale / 2f, Quaternion.identity);
		this.haulWeight = 0f;
		foreach (Collider collider in this.itemsInsideBed)
		{
			if (collider.GetComponent<Rigidbody>() != null)
			{
				Rigidbody component = collider.GetComponent<Rigidbody>();
				if (component != null && component.velocity.x < 0.05f && component.velocity.y < 0.05f && component.velocity.z < 0.05f && (collider.transform.parent == null || collider.transform.parent == this.truck) && collider.GetComponent<PickUp>() != null)
				{
					if (collider.GetComponent<FixedJoint>() == null)
					{
						collider.gameObject.AddComponent<FixedJoint>().connectedBody = this.truck.GetComponent<Rigidbody>();
						component.angularDrag = 0f;
						component.drag = 0f;
						component.mass = 1f;
					}
					float trueMass = collider.GetComponent<PickUp>().trueMass;
					this.haulWeight += trueMass;
				}
			}
		}
		this.truck.GetComponent<Rigidbody>().mass = 1500f + this.haulWeight;
		float num = 0.3f - this.haulWeight / 5000f;
		if (num < 0.1f)
		{
			num = 0.1f;
		}
		this.wcRR.suspensionDistance = num;
		this.wcRL.suspensionDistance = num;
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x0006F958 File Offset: 0x0006DB58
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
		this.truck.GetComponent<Rigidbody>().mass = 1500f;
		this.wcRR.suspensionDistance = 0.3f;
		this.wcRL.suspensionDistance = 0.3f;
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x0000E748 File Offset: 0x0000C948
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(base.transform.position, base.transform.localScale);
	}

	// Token: 0x040013DD RID: 5085
	public Transform truck;

	// Token: 0x040013DE RID: 5086
	public Collider[] itemsInsideBed;

	// Token: 0x040013DF RID: 5087
	private int weightCheckInterval = 3;

	// Token: 0x040013E0 RID: 5088
	public float haulWeight;

	// Token: 0x040013E1 RID: 5089
	public WheelCollider wcRR;

	// Token: 0x040013E2 RID: 5090
	public WheelCollider wcRL;
}
