using System;
using UnityEngine;

namespace ch.sycoforge.Decal.Demo
{
	// Token: 0x020001AF RID: 431
	public class BasicBulletHoles : MonoBehaviour
	{
		// Token: 0x06000A98 RID: 2712 RVA: 0x0008D51B File Offset: 0x0008B71B
		public void Start()
		{
			if (this.DecalPrefab == null)
			{
				Debug.LogError("The DynamicDemo script has no decal prefab attached.");
			}
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x0008D538 File Offset: 0x0008B738
		public void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, 200f))
				{
					Debug.DrawLine(ray.origin, raycastHit.point, Color.red);
					EasyDecal easyDecal = EasyDecal.ProjectAt(this.DecalPrefab.gameObject, raycastHit.collider.gameObject, raycastHit.point, raycastHit.normal);
					this.t = !this.t;
					if (this.t)
					{
						easyDecal.CancelFade();
					}
				}
			}
		}

		// Token: 0x04001CAA RID: 7338
		public EasyDecal DecalPrefab;

		// Token: 0x04001CAB RID: 7339
		private bool t;
	}
}
