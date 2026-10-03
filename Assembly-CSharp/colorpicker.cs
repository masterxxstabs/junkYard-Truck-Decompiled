using System;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class colorpicker : MonoBehaviour
{
	// Token: 0x0600096A RID: 2410 RVA: 0x0007F5B9 File Offset: 0x0007D7B9
	private void Start()
	{
		this.garageMenu = (Object.FindObjectOfType(typeof(menu)) as menu);
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x0007F5D8 File Offset: 0x0007D7D8
	private void Update()
	{
		if (this.garageMenu != null)
		{
			if (this.garageMenu.carMenu)
			{
				if (this.rectX < 50f)
				{
					this.rectX += Time.deltaTime * this.garageMenu.scrollSpeed * 2f;
				}
			}
			else if (this.rectX > -300f)
			{
				this.rectX -= Time.deltaTime * this.garageMenu.scrollSpeed * 2f;
			}
		}
		this.textureRect.x = this.rectX;
		this.textureRect.y = (float)(Screen.height - 180);
		if (this.paint)
		{
			this.x = (int)(Input.mousePosition.x - this.textureRect.x);
			this.y = (int)(this.textureRect.y - ((float)Screen.height - Input.mousePosition.y - 130f));
			this.changedMaterial.SetColor("_Color", this.colors.GetPixel(this.x, this.y));
			if (Input.GetMouseButtonDown(0))
			{
				this.paint = false;
			}
		}
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x0007F714 File Offset: 0x0007D914
	private void OnGUI()
	{
		if (GUI.Button(new Rect(this.textureRect.x, this.textureRect.y - 40f, 100f, 30f), "chose color"))
		{
			this.paint = true;
		}
		GUI.DrawTexture(this.textureRect, this.colors);
	}

	// Token: 0x04001925 RID: 6437
	public Texture2D colors;

	// Token: 0x04001926 RID: 6438
	public Material changedMaterial;

	// Token: 0x04001927 RID: 6439
	public int x;

	// Token: 0x04001928 RID: 6440
	public int y;

	// Token: 0x04001929 RID: 6441
	private Rect textureRect = new Rect(-300f, (float)(Screen.height - 180), 250f, 150f);

	// Token: 0x0400192A RID: 6442
	private float rectX = -300f;

	// Token: 0x0400192B RID: 6443
	private bool paint;

	// Token: 0x0400192C RID: 6444
	private menu garageMenu;
}
