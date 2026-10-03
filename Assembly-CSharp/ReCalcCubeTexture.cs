using System;
using UnityEngine;

// Token: 0x02000199 RID: 409
[ExecuteInEditMode]
public class ReCalcCubeTexture : MonoBehaviour
{
	// Token: 0x06000A01 RID: 2561 RVA: 0x00089EBC File Offset: 0x000880BC
	private void Start()
	{
		this.Calculate();
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x00089EBC File Offset: 0x000880BC
	private void Update()
	{
		this.Calculate();
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x00089EC4 File Offset: 0x000880C4
	public void Calculate()
	{
		if (this._currentScale == base.transform.localScale)
		{
			return;
		}
		if (this.CheckForDefaultSize())
		{
			return;
		}
		this._currentScale = base.transform.localScale;
		Mesh mesh = this.GetMesh();
		mesh.uv = this.SetupUvMap(mesh.uv);
		mesh.name = "Cube Instance";
		if (base.GetComponent<Renderer>().sharedMaterial.mainTexture.wrapMode != TextureWrapMode.Repeat)
		{
			base.GetComponent<Renderer>().sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
		}
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x00089F55 File Offset: 0x00088155
	private Mesh GetMesh()
	{
		return base.GetComponent<MeshFilter>().mesh;
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x00089F64 File Offset: 0x00088164
	private Vector2[] SetupUvMap(Vector2[] meshUVs)
	{
		float x = this._currentScale.x;
		float z = this._currentScale.z;
		float y = this._currentScale.y;
		meshUVs[2] = new Vector2(0f, y);
		meshUVs[3] = new Vector2(x, y);
		meshUVs[0] = new Vector2(0f, 0f);
		meshUVs[1] = new Vector2(x, 0f);
		meshUVs[7] = new Vector2(0f, 0f);
		meshUVs[6] = new Vector2(x, 0f);
		meshUVs[11] = new Vector2(0f, y);
		meshUVs[10] = new Vector2(x, y);
		meshUVs[19] = new Vector2(z, 0f);
		meshUVs[17] = new Vector2(0f, y);
		meshUVs[16] = new Vector2(0f, 0f);
		meshUVs[18] = new Vector2(z, y);
		meshUVs[23] = new Vector2(z, 0f);
		meshUVs[21] = new Vector2(0f, y);
		meshUVs[20] = new Vector2(0f, 0f);
		meshUVs[22] = new Vector2(z, y);
		meshUVs[4] = new Vector2(x, 0f);
		meshUVs[5] = new Vector2(0f, 0f);
		meshUVs[8] = new Vector2(x, z);
		meshUVs[9] = new Vector2(0f, z);
		meshUVs[13] = new Vector2(x, 0f);
		meshUVs[14] = new Vector2(0f, 0f);
		meshUVs[12] = new Vector2(x, z);
		meshUVs[15] = new Vector2(0f, z);
		return meshUVs;
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x0008A158 File Offset: 0x00088358
	private bool CheckForDefaultSize()
	{
		if (this._currentScale != Vector3.one)
		{
			return false;
		}
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Object.DestroyImmediate(base.GetComponent<MeshFilter>());
		base.gameObject.AddComponent<MeshFilter>();
		base.GetComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
		Object.DestroyImmediate(gameObject);
		return true;
	}

	// Token: 0x04001BF2 RID: 7154
	private Vector3 _currentScale;
}
