using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200015E RID: 350
public class WaterTank : MonoBehaviour
{
	// Token: 0x060008AB RID: 2219 RVA: 0x00070924 File Offset: 0x0006EB24
	private void Start()
	{
		base.transform.parent.position = new Vector3(base.transform.parent.position.x, base.transform.parent.position.y + 1f, base.transform.parent.position.z);
		base.transform.parent.gameObject.transform.GetChild(8).gameObject.SetActive(false);
		base.StartCoroutine(this.HaltTank());
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x000709BE File Offset: 0x0006EBBE
	private IEnumerator HaltTank()
	{
		base.transform.parent.gameObject.GetComponent<Rigidbody>().drag = 1000f;
		base.transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 1000f;
		yield return new WaitForSeconds(2f);
		base.transform.parent.gameObject.GetComponent<Rigidbody>().drag = 0f;
		base.transform.parent.gameObject.GetComponent<Rigidbody>().angularDrag = 0.5f;
		base.transform.parent.gameObject.transform.GetChild(8).gameObject.SetActive(true);
		if (Vector3.Dot(base.transform.parent.up, Vector3.down) > 0f)
		{
			base.transform.parent.eulerAngles = new Vector3(0f, base.transform.parent.eulerAngles.y, 0f);
		}
		yield break;
	}

	// Token: 0x0400140C RID: 5132
	public float waterLevel;
}
