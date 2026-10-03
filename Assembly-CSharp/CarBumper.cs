using System;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class CarBumper : MonoBehaviour
{
	// Token: 0x06000486 RID: 1158 RVA: 0x0002FC23 File Offset: 0x0002DE23
	private void Awake()
	{
		base.GetComponent<ImpactDeformable>().OnDeform += this.CarBumper_OnDeform;
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0002FC3C File Offset: 0x0002DE3C
	public void OnDisable()
	{
		base.GetComponent<ImpactDeformable>().OnDeform -= this.CarBumper_OnDeform;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0002FC58 File Offset: 0x0002DE58
	private void CarBumper_OnDeform(ImpactDeformable deformable)
	{
		if (deformable.StructuralDamage > 0.1f)
		{
			base.transform.parent = base.transform.parent.parent;
			base.gameObject.AddComponent<Rigidbody>().mass = 0.1f;
			Object.Destroy(this);
			Object.Destroy(base.gameObject, 10f);
		}
	}
}
