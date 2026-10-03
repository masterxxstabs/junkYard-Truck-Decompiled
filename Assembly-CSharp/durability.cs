using System;
using UnityEngine;

// Token: 0x0200016F RID: 367
public class durability : MonoBehaviour
{
	// Token: 0x06000901 RID: 2305 RVA: 0x000758DC File Offset: 0x00073ADC
	public void Start()
	{
		if (this.painted)
		{
			Material[] materials = base.GetComponent<Renderer>().materials;
			materials[this.paintSlot].color = new Color(this.red, this.green, this.blue, 1f);
			if (this.metallic > 0f)
			{
				materials[this.paintSlot].SetFloat("_Metallic", this.metallic);
			}
			if (this.smoothness > 0f)
			{
				materials[this.paintSlot].SetFloat("_Glossiness", this.smoothness);
			}
		}
		if (this.mat != null)
		{
			if (this.mat2 == null)
			{
				if (base.GetComponent<MeshRenderer>() != null)
				{
					Material material = base.GetComponent<MeshRenderer>().material;
					if (this.mat.name.Contains("RustMetal"))
					{
						this.newRust = 2f - 2f * (this.health / 100f);
						material.SetFloat("_RustIntensity", this.newRust);
					}
				}
			}
			else
			{
				Material[] materials2 = base.GetComponent<MeshRenderer>().materials;
				if (this.mat.name.Contains("RustMetal"))
				{
					this.newRust = 2f - 2f * (this.health / 100f);
					materials2[0].SetFloat("_RustIntensity", this.newRust);
				}
				if (this.mat2.name.Contains("RustMetal"))
				{
					this.newRust = 2f - 2f * (this.health / 100f);
					materials2[1].SetFloat("_RustIntensity", this.newRust);
				}
			}
		}
		if (base.GetComponent<MeshRenderer>() != null && base.gameObject.GetComponent<Renderer>().enabled)
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.gameObject.name == "bolt")
				{
					transform.gameObject.GetComponent<Renderer>().enabled = true;
					transform.gameObject.GetComponent<BoxCollider>().enabled = true;
					transform.gameObject.GetComponent<BoltScript>().boltturns = 10;
					if (base.transform.parent != null && base.transform.parent.name.Contains("sparemount") && !base.transform.parent.GetComponent<Renderer>().enabled)
					{
						transform.gameObject.GetComponent<Renderer>().enabled = false;
						transform.gameObject.GetComponent<BoxCollider>().enabled = false;
						transform.gameObject.GetComponent<BoltScript>().boltturns = 0;
					}
				}
			}
		}
	}

	// Token: 0x0400156B RID: 5483
	public float health;

	// Token: 0x0400156C RID: 5484
	public int numBolts;

	// Token: 0x0400156D RID: 5485
	public float boltStr;

	// Token: 0x0400156E RID: 5486
	public bool canDetach;

	// Token: 0x0400156F RID: 5487
	public GameObject template;

	// Token: 0x04001570 RID: 5488
	public GameObject template2;

	// Token: 0x04001571 RID: 5489
	public GameObject template3;

	// Token: 0x04001572 RID: 5490
	public GameObject template4;

	// Token: 0x04001573 RID: 5491
	public GameObject template5;

	// Token: 0x04001574 RID: 5492
	private float newRust;

	// Token: 0x04001575 RID: 5493
	public WheelCollider wc;

	// Token: 0x04001576 RID: 5494
	public Material mat;

	// Token: 0x04001577 RID: 5495
	public Material mat2;

	// Token: 0x04001578 RID: 5496
	public float red;

	// Token: 0x04001579 RID: 5497
	public float green;

	// Token: 0x0400157A RID: 5498
	public float blue;

	// Token: 0x0400157B RID: 5499
	public float metallic;

	// Token: 0x0400157C RID: 5500
	public float smoothness;

	// Token: 0x0400157D RID: 5501
	public bool painted;

	// Token: 0x0400157E RID: 5502
	public int paintSlot;
}
