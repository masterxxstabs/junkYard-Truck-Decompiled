using System;
using UnityEngine;

// Token: 0x02000156 RID: 342
public class TrailerBedGrav : MonoBehaviour
{
	// Token: 0x0600088B RID: 2187 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x00002188 File Offset: 0x00000388
	public void EnableBed()
	{
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x0006F157 File Offset: 0x0006D357
	public void Constrain()
	{
		if (this.isFlatbed)
		{
			if (this.rampIo.isOpen)
			{
				this.rb.constraints = RigidbodyConstraints.None;
				return;
			}
			this.rb.constraints = RigidbodyConstraints.FreezeAll;
		}
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x0006F188 File Offset: 0x0006D388
	public void CheckBed()
	{
		this.itemsInsideBed = Physics.OverlapBox(base.transform.position, base.transform.localScale / 2f, Quaternion.identity);
		this.haulWeight = 0f;
		foreach (Collider collider in this.itemsInsideBed)
		{
			if (collider.GetComponent<Rigidbody>() != null)
			{
				Rigidbody component = collider.GetComponent<Rigidbody>();
				if (component != null && component.velocity.x < 0.5f && component.velocity.y < 0.5f && component.velocity.z < 0.5f && (collider.transform.parent == null || collider.transform.parent == this.trailer))
				{
					if (collider.GetComponent<PickUp>() != null)
					{
						if (collider.name != "v8_block" && collider.name != "engineblock" && collider.name != "250_block")
						{
							if (collider.GetComponent<FixedJoint>() == null)
							{
								collider.gameObject.AddComponent<FixedJoint>().connectedBody = this.trailer.GetComponent<Rigidbody>();
								component.angularDrag = 0f;
								component.drag = 0f;
								component.mass = 1f;
							}
							float trueMass = collider.GetComponent<PickUp>().trueMass;
							this.haulWeight += trueMass;
						}
					}
					else if (collider.name == "amc" || collider.name == "dirt pickup truck" || collider.name == "f1003")
					{
						if (collider.name == "amc")
						{
							this.amcWheelCol[0].SetActive(false);
							this.amcWheelCol[1].SetActive(false);
							this.amcWheelCol[2].SetActive(false);
							this.amcWheelCol[3].SetActive(false);
						}
						else if (collider.name == "dirt pickup truck")
						{
							this.diamondSuspension.GetComponent<SuspensionOffroadCar>().enabled = false;
							this.diamondbackCol[0].SetActive(false);
							this.diamondbackCol[1].SetActive(false);
							this.diamondbackCol[2].SetActive(false);
							this.diamondbackCol[3].SetActive(false);
						}
						else if (collider.name == "f1003")
						{
							this.f100Suspension.GetComponent<SuspensionOffroadCar>().enabled = false;
							this.f100Col[0].SetActive(false);
							this.f100Col[1].SetActive(false);
							this.f100Col[2].SetActive(false);
							this.f100Col[3].SetActive(false);
						}
						if (collider.GetComponent<FixedJoint>() == null)
						{
							collider.gameObject.AddComponent<FixedJoint>().connectedBody = this.trailer.GetComponent<Rigidbody>();
							float mass = collider.GetComponent<Rigidbody>().mass;
							this.haulWeight += mass;
							Debug.Log(collider.name);
						}
					}
				}
			}
		}
		this.trailer.GetComponent<Rigidbody>().mass = 300f + this.haulWeight;
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0006F4E8 File Offset: 0x0006D6E8
	public void EmptyBed()
	{
		this.itemsInsideBed = Physics.OverlapBox(base.transform.position, base.transform.localScale / 2f, Quaternion.identity);
		foreach (Collider collider in this.itemsInsideBed)
		{
			if (collider.GetComponent<Rigidbody>() != null && collider.GetComponent<PickUp>() != null && collider.name != "v8_block" && collider.name != "engineblock" && collider.name != "250_block")
			{
				collider.GetComponent<Rigidbody>().mass = collider.GetComponent<PickUp>().trueMass;
				Object.Destroy(collider.GetComponent<FixedJoint>());
			}
		}
		this.trailer.GetComponent<Rigidbody>().mass = 300f;
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x0006F5CC File Offset: 0x0006D7CC
	private void FixedUpdate()
	{
		this.WheelOffset(this.lw, this.lwc);
		this.WheelOffset(this.rw, this.rwc);
		if (this.tandem)
		{
			this.WheelOffset(this.rlw, this.lwc);
			this.WheelOffset(this.rrw, this.rwc);
		}
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x0006F62C File Offset: 0x0006D82C
	private void WheelOffset(GameObject model, WheelCollider collider)
	{
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);
		if (!this.tandem)
		{
			model.transform.position = position;
		}
		model.transform.rotation = rotation;
	}

	// Token: 0x040013BF RID: 5055
	public Transform trailer;

	// Token: 0x040013C0 RID: 5056
	public Collider[] itemsInsideBed;

	// Token: 0x040013C1 RID: 5057
	private int weightCheckInterval = 3;

	// Token: 0x040013C2 RID: 5058
	public float haulWeight;

	// Token: 0x040013C3 RID: 5059
	public GameObject lw;

	// Token: 0x040013C4 RID: 5060
	public GameObject rw;

	// Token: 0x040013C5 RID: 5061
	public GameObject rlw;

	// Token: 0x040013C6 RID: 5062
	public GameObject rrw;

	// Token: 0x040013C7 RID: 5063
	public WheelCollider lwc;

	// Token: 0x040013C8 RID: 5064
	public WheelCollider rwc;

	// Token: 0x040013C9 RID: 5065
	public bool tandem;

	// Token: 0x040013CA RID: 5066
	public GameObject[] amcWheelCol;

	// Token: 0x040013CB RID: 5067
	public GameObject[] diamondbackCol;

	// Token: 0x040013CC RID: 5068
	public GameObject[] f100Col;

	// Token: 0x040013CD RID: 5069
	public GameObject diamondSuspension;

	// Token: 0x040013CE RID: 5070
	public GameObject f100Suspension;

	// Token: 0x040013CF RID: 5071
	public InteractiveObject rampIo;

	// Token: 0x040013D0 RID: 5072
	public Rigidbody rb;

	// Token: 0x040013D1 RID: 5073
	public bool isFlatbed;

	// Token: 0x040013D2 RID: 5074
	public GameObject thirdWheel;
}
