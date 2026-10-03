using System;
using UnityEngine;

// Token: 0x02000135 RID: 309
public class TexCoord : MonoBehaviour
{
	// Token: 0x06000807 RID: 2055 RVA: 0x0006BE07 File Offset: 0x0006A007
	private void Start()
	{
		this.cam = base.GetComponent<Camera>();
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x0006BE18 File Offset: 0x0006A018
	private void Update()
	{
		if (!Input.GetMouseButton(0))
		{
			return;
		}
		RaycastHit raycastHit;
		if (!Physics.Raycast(this.cam.ScreenPointToRay(Input.mousePosition), out raycastHit))
		{
			return;
		}
		Renderer component = raycastHit.transform.GetComponent<Renderer>();
		MeshCollider x = raycastHit.collider as MeshCollider;
		if (component == null || component.sharedMaterial == null || component.sharedMaterial.mainTexture == null || x == null)
		{
			return;
		}
		Texture2D texture2D = component.material.mainTexture as Texture2D;
		Vector2 textureCoord = raycastHit.textureCoord;
		textureCoord.x *= (float)texture2D.width;
		textureCoord.y *= (float)texture2D.height;
		texture2D.SetPixel((int)textureCoord.x, (int)textureCoord.y, Color.black);
		texture2D.Apply();
	}

	// Token: 0x040012D4 RID: 4820
	public Camera cam;
}
