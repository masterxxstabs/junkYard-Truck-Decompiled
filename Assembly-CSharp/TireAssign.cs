using System;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class TireAssign : MonoBehaviour
{
	// Token: 0x0600086C RID: 2156 RVA: 0x0006E758 File Offset: 0x0006C958
	public void Start()
	{
		if (this.rimNumX == 0)
		{
			int num = this.tireNumX;
		}
		MeshCollider[] components = base.GetComponents<MeshCollider>();
		if (this.rimNumX > 0)
		{
			base.transform.GetChild(this.rimNumX).gameObject.SetActive(true);
			base.transform.GetChild(0).gameObject.SetActive(false);
			if (this.tireNumX == 0)
			{
				components[0].enabled = true;
			}
		}
		if (this.tireNumX > 0)
		{
			base.transform.GetChild(this.tireNumX).gameObject.SetActive(true);
			if (this.rimNumX == 0)
			{
				base.transform.GetChild(0).gameObject.SetActive(true);
			}
			components[1].enabled = true;
		}
	}

	// Token: 0x04001388 RID: 5000
	public int rimNumX;

	// Token: 0x04001389 RID: 5001
	public int tireNumX;
}
