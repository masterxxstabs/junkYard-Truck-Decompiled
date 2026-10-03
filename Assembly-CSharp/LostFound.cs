using System;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class LostFound : MonoBehaviour
{
	// Token: 0x060005D4 RID: 1492 RVA: 0x00047A90 File Offset: 0x00045C90
	private void OnTriggerEnter(Collider other)
	{
		this.warpObj = null;
		if (other.GetComponent<Rigidbody>() != null)
		{
			this.warpObj = other.gameObject;
		}
		if (this.warpObj == null && other.transform.parent.gameObject.GetComponent<Rigidbody>() != null)
		{
			this.warpObj = other.transform.parent.gameObject;
		}
		if (this.warpObj == null && other.transform.parent.parent.gameObject.GetComponent<Rigidbody>() != null)
		{
			this.warpObj = other.transform.parent.parent.gameObject;
		}
		if (this.warpObj == null && other.transform.parent.parent.parent.gameObject.GetComponent<Rigidbody>() != null)
		{
			this.warpObj = other.transform.parent.parent.parent.gameObject;
		}
		if (other.name.Contains("Ore"))
		{
			Object.Destroy(other);
		}
		if (this.warpObj.name == "cop8")
		{
			if (this.warpObj.GetComponent<Officer>().destination != null)
			{
				this.warpObj.transform.position = this.warpObj.GetComponent<Officer>().destination.position;
			}
			else
			{
				this.warpObj.transform.position = this.warpObj.GetComponent<Officer>().entrance.position;
			}
		}
		if (this.warpObj != null && this.warpObj.name != "FPSController" && this.warpObj.name != "amc" && this.warpObj.name != "dirt pickup truck")
		{
			this.warpObj.transform.position = this.lostBinPos.position;
			this.warpObj.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.warpObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
		if (this.warpObj.name == "FPSController")
		{
			this.ps.Option5b();
		}
		if (this.warpObj.name == "dirt pickup truck")
		{
			this.ps.Option1b();
		}
		if (this.warpObj.name == "amc")
		{
			this.ps.Option3b();
		}
		if (this.warpObj.name == "DirtBike")
		{
			this.ps.Option4b();
		}
		if (this.warpObj.name == "f1003")
		{
			this.ps.Option6b();
		}
	}

	// Token: 0x04000CAD RID: 3245
	private GameObject warpObj;

	// Token: 0x04000CAE RID: 3246
	public Transform lostBinPos;

	// Token: 0x04000CAF RID: 3247
	public PhoneScript ps;
}
