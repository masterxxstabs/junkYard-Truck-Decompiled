using System;
using UnityEngine;

namespace ch.sycoforge.Decal.Demo
{
	// Token: 0x020001AE RID: 430
	public class AdvancedBulletHoles : MonoBehaviour
	{
		// Token: 0x06000A95 RID: 2709 RVA: 0x0008D3C7 File Offset: 0x0008B5C7
		private void Start()
		{
			if (this.DecalPrefab == null)
			{
				Debug.LogError("The AdvancedBulletHoles script has no decal prefab attached.");
			}
			EasyDecal.HideMesh = false;
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x0008D3E8 File Offset: 0x0008B5E8
		private void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, 200f))
				{
					GameObject gameObject = raycastHit.collider.gameObject;
					Vector3 point = raycastHit.point;
					RaycastHit[] array = Physics.SphereCastAll(ray, this.CastRadius, Vector3.Distance(Camera.main.transform.position, point) + 2f);
					Vector3 vector = raycastHit.normal;
					if (array.Length != 0)
					{
						foreach (RaycastHit raycastHit2 in array)
						{
							Debug.DrawLine(ray.origin, raycastHit2.point, Color.red);
							vector += raycastHit2.normal;
						}
					}
					vector /= (float)(array.Length + 1);
					EasyDecal.ProjectAt(this.DecalPrefab.gameObject, gameObject, point, vector);
					if (this.ImpactParticles != null)
					{
						Quaternion rotation = Quaternion.FromToRotation(Vector3.up, vector);
						Object.Instantiate<GameObject>(this.ImpactParticles, point, rotation);
					}
				}
			}
		}

		// Token: 0x04001CA7 RID: 7335
		public EasyDecal DecalPrefab;

		// Token: 0x04001CA8 RID: 7336
		public GameObject ImpactParticles;

		// Token: 0x04001CA9 RID: 7337
		public float CastRadius = 0.25f;
	}
}
