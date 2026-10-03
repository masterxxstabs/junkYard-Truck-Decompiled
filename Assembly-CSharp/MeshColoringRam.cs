using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class MeshColoringRam : MonoBehaviour
{
	// Token: 0x060006A6 RID: 1702 RVA: 0x00051948 File Offset: 0x0004FB48
	private void Start()
	{
		if (this.colorMeshLive)
		{
			if (MeshColoringRam.ramSplines == null)
			{
				MeshColoringRam.ramSplines = Object.FindObjectsOfType<RamSpline>();
			}
			if (MeshColoringRam.lakePolygons == null)
			{
				MeshColoringRam.lakePolygons = Object.FindObjectsOfType<LakePolygon>();
			}
			this.colored = false;
			this.meshFilters = base.gameObject.GetComponentsInChildren<MeshFilter>();
		}
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x00051997 File Offset: 0x0004FB97
	private void Update()
	{
		if (this.colorMeshLive)
		{
			this.ColorMeshLive();
		}
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x000519A8 File Offset: 0x0004FBA8
	public void ColorMeshLive()
	{
		this.colored = true;
		Ray ray = default(Ray);
		ray.direction = Vector3.up;
		Vector3 b = -Vector3.up * (this.height + this.threshold);
		Color white = Color.white;
		List<MeshCollider> list = new List<MeshCollider>();
		foreach (RamSpline ramSpline in MeshColoringRam.ramSplines)
		{
			list.Add(ramSpline.gameObject.AddComponent<MeshCollider>());
		}
		foreach (LakePolygon lakePolygon in MeshColoringRam.lakePolygons)
		{
			list.Add(lakePolygon.gameObject.AddComponent<MeshCollider>());
		}
		bool queriesHitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		foreach (MeshFilter meshFilter in this.meshFilters)
		{
			Mesh mesh = meshFilter.sharedMesh;
			if (meshFilter.sharedMesh != null)
			{
				if (!this.colored)
				{
					mesh = Object.Instantiate<Mesh>(meshFilter.sharedMesh);
					meshFilter.sharedMesh = mesh;
					this.colored = true;
				}
				int num = mesh.vertices.Length;
				Vector3[] vertices = mesh.vertices;
				Color[] array4 = mesh.colors;
				Transform transform = meshFilter.transform;
				float num2 = float.MaxValue;
				Vector3 origin = vertices[0];
				for (int j = 0; j < num; j++)
				{
					vertices[j] = transform.TransformPoint(vertices[j]) + b;
					if (vertices[j].y < num2)
					{
						num2 = vertices[j].y;
						origin = vertices[j];
					}
				}
				if (array4.Length == 0)
				{
					array4 = new Color[num];
					for (int k = 0; k < array4.Length; k++)
					{
						array4[k] = white;
					}
				}
				ray.origin = origin;
				num2 = float.MinValue;
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, 100f, this.layer))
				{
					num2 = raycastHit.point.y;
				}
				for (int l = 0; l < num; l++)
				{
					if (vertices[l].y < num2)
					{
						float num3 = Mathf.Abs(vertices[l].y - num2);
						if (num3 > this.threshold)
						{
							array4[l].r = 0f;
						}
						else
						{
							array4[l].r = Mathf.Lerp(1f, 0f, num3 / this.threshold);
						}
					}
					else
					{
						array4[l] = white;
					}
				}
				mesh.colors = array4;
			}
		}
		foreach (MeshCollider obj in list)
		{
			Object.Destroy(obj);
		}
		Physics.queriesHitBackfaces = queriesHitBackfaces;
	}

	// Token: 0x04000E3A RID: 3642
	public float height = 0.5f;

	// Token: 0x04000E3B RID: 3643
	public float threshold = 0.5f;

	// Token: 0x04000E3C RID: 3644
	public bool autoColor = true;

	// Token: 0x04000E3D RID: 3645
	public bool newMesh = true;

	// Token: 0x04000E3E RID: 3646
	public Vector3 oldPosition = Vector3.zero;

	// Token: 0x04000E3F RID: 3647
	public bool colorMeshLive;

	// Token: 0x04000E40 RID: 3648
	public LayerMask layer;

	// Token: 0x04000E41 RID: 3649
	private MeshFilter[] meshFilters;

	// Token: 0x04000E42 RID: 3650
	private bool colored;

	// Token: 0x04000E43 RID: 3651
	private static RamSpline[] ramSplines;

	// Token: 0x04000E44 RID: 3652
	private static LakePolygon[] lakePolygons;
}
