using System;
using UnityEngine;

// Token: 0x020000F0 RID: 240
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombine : MonoBehaviour
{
	// Token: 0x060005F2 RID: 1522 RVA: 0x00048A74 File Offset: 0x00046C74
	private void Start()
	{
		MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] array = new CombineInstance[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			array[i].mesh = componentsInChildren[i].sharedMesh;
			array[i].transform = componentsInChildren[i].transform.localToWorldMatrix;
			componentsInChildren[i].gameObject.SetActive(false);
		}
		base.transform.GetComponent<MeshFilter>().mesh = new Mesh();
		base.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(array);
		base.transform.gameObject.SetActive(true);
	}
}
