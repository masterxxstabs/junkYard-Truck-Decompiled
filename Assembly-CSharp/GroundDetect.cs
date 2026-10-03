using System;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x0200012E RID: 302
public class GroundDetect : MonoBehaviour
{
	// Token: 0x060007D3 RID: 2003 RVA: 0x00064358 File Offset: 0x00062558
	private void Start()
	{
		if (this.dirty > 1f)
		{
			this.dirty = 1f;
		}
		else if (this.dirty < 0f)
		{
			this.dirty = 0f;
		}
		if (this.dirty2 > 5f)
		{
			this.dirty2 = 5f;
		}
		if (this.dirty2 < 0f)
		{
			this.dirty2 = 0f;
		}
		else if (this.dirty2 < 0f)
		{
			this.dirty2 = 0f;
		}
		if (this.dirty250 <= 1f && this.dirty250 >= 0f)
		{
			this.dbtireF.material.SetFloat("_Blend", this.dirty250);
			this.dbtireR.material.SetFloat("_Blend", this.dirty250);
		}
		if (this.dirty2502 <= 5f && this.dirty2502 >= 0f)
		{
			this.plastic.material.SetFloat("_RustIntensity", this.dirty2502);
			this.plastic2.material.SetFloat("_RustIntensity", this.dirty2502);
			this.plastic3.material.SetFloat("_RustIntensity", this.dirty2502);
		}
		this.tireFL.material.SetFloat("_Blend", this.dirty);
		this.tireFR.material.SetFloat("_Blend", this.dirty);
		this.tireRL.material.SetFloat("_Blend", this.dirty);
		this.tireRR.material.SetFloat("_Blend", this.dirty);
		this.body.material.SetFloat("_RustIntensity", this.dirty2);
		this.cabin.material.SetFloat("_RustIntensity", this.dirty2);
		this.doorp.material.SetFloat("_RustIntensity", this.dirty2);
		this.doord.material.SetFloat("_RustIntensity", this.dirty2);
		this.tailg.material.SetFloat("_RustIntensity", this.dirty2);
		this.gasdoor.material.SetFloat("_RustIntensity", this.dirty2);
		Material[] materials = this.hoodF.GetComponent<MeshRenderer>().materials;
		this.f100MudMathood = materials[1];
		Material[] materials2 = this.bodyF.GetComponent<MeshRenderer>().materials;
		this.f100MudMatbody = materials2[2];
		Material[] materials3 = this.doorpF.GetComponent<MeshRenderer>().materials;
		this.f100MudMatdoorP = materials3[2];
		Material[] materials4 = this.doordF.GetComponent<MeshRenderer>().materials;
		this.f100MudMatdoorD = materials4[2];
		this.tireFLF.material.SetFloat("_Blend", this.dirtyF);
		this.tireFRF.material.SetFloat("_Blend", this.dirtyF);
		this.tireRLF.material.SetFloat("_Blend", this.dirtyF);
		this.tireRRF.material.SetFloat("_Blend", this.dirtyF);
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x00064680 File Offset: 0x00062880
	private void ConvertPosition(Vector3 playerPosition)
	{
		Vector3 vector = playerPosition - this.t.transform.position;
		Vector3 vector2 = new Vector3(vector.x / this.t.terrainData.size.x, 0f, vector.z / this.t.terrainData.size.z);
		float num = vector2.x * (float)this.t.terrainData.alphamapWidth;
		float num2 = vector2.z * (float)this.t.terrainData.alphamapHeight;
		this.posX = (int)num;
		this.posZ = (int)num2;
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x00064728 File Offset: 0x00062928
	private void CheckTexture()
	{
		float[,,] alphamaps = this.t.terrainData.GetAlphamaps(this.posX, this.posZ, 1, 1);
		if (alphamaps[0, 0, 10] > 0.2f || alphamaps[0, 0, 17] > 0.2f)
		{
			this.deepMud = 1;
		}
		else
		{
			this.deepMud = 0;
		}
		if (alphamaps[0, 0, 0] > 0.2f || alphamaps[0, 0, 1] > 0.2f || alphamaps[0, 0, 2] > 0.2f)
		{
			this.leaves = 1;
		}
		else
		{
			this.leaves = 0;
		}
		if (alphamaps[0, 0, 3] > 0.2f || alphamaps[0, 0, 7] > 0.2f || alphamaps[0, 0, 8] > 0.2f)
		{
			this.rockSurface = 1;
		}
		else
		{
			this.rockSurface = 0;
		}
		if (alphamaps[0, 0, 16] > 0.2f)
		{
			this.deepWater = 1;
		}
		else
		{
			this.deepWater = 0;
		}
		if (alphamaps[0, 0, 4] > 0.2f || alphamaps[0, 0, 11] > 0.2f)
		{
			this.fpc.roughness = 0;
			this.road = 1;
		}
		else
		{
			this.fpc.roughness = 1;
			this.road = 0;
		}
		if (alphamaps[0, 0, 5] > 0.2f || alphamaps[0, 0, 9] > 0.2f || alphamaps[0, 0, 10] > 0.2f || alphamaps[0, 0, 17] > 0.2f)
		{
			if (this.dirty < 1f)
			{
				this.dirty += 0.2f;
				this.ChangeDirt();
			}
			if (this.dirty2 < 5f)
			{
				this.dirty2 += 0.05f;
				this.ChangeDirt();
			}
			if (this.dirtyF < 1f)
			{
				this.dirtyF += 0.2f;
				this.ChangeDirt();
			}
			if (this.dirty2F < 1f && this.deepMud == 1)
			{
				this.dirty2F += 0.05f;
				this.ChangeDirt();
			}
			if (this.dirty250 < 1f)
			{
				this.dirty250 += 0.2f;
				this.ChangeDirt();
			}
			if (this.dirty2502 < 5f)
			{
				this.dirty2502 += 0.05f;
				this.ChangeDirt();
			}
		}
		else if (alphamaps[0, 0, 4] > 0.2f || alphamaps[0, 0, 11] > 0.2f)
		{
			this.deepMud = 0;
			if (this.dirty > 0f)
			{
				this.dirty -= 0.2f;
				this.ChangeDirt();
			}
			if (this.dirtyF > 0f)
			{
				this.dirtyF -= 0.2f;
				this.ChangeDirt();
			}
			if (this.dirty250 > 0f)
			{
				this.dirty250 -= 0.2f;
				this.ChangeDirt();
			}
		}
		this.UpdateTraction();
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x00064A44 File Offset: 0x00062C44
	public void ChangeDirt()
	{
		if (this.FPSt.parent != null)
		{
			if (this.FPSt.parent.name == "DriverCameraController")
			{
				if (this.dirty <= 1f && this.dirty >= 0f)
				{
					this.tireFL.material.SetFloat("_Blend", this.dirty);
					this.tireFR.material.SetFloat("_Blend", this.dirty);
					this.tireRL.material.SetFloat("_Blend", this.dirty);
					this.tireRR.material.SetFloat("_Blend", this.dirty);
				}
				if (this.dirty2 <= 5f && this.dirty2 >= 0f)
				{
					this.body.material.SetFloat("_RustIntensity", this.dirty2);
					this.cabin.material.SetFloat("_RustIntensity", this.dirty2);
					this.doorp.material.SetFloat("_RustIntensity", this.dirty2);
					this.doord.material.SetFloat("_RustIntensity", this.dirty2);
					this.tailg.material.SetFloat("_RustIntensity", this.dirty2);
					this.gasdoor.material.SetFloat("_RustIntensity", this.dirty2);
				}
			}
			if (this.FPSt.parent.name == "f100seatmount")
			{
				if (this.dirtyF <= 1f && this.dirtyF >= 0f)
				{
					this.tireFLF.material.SetFloat("_Blend", this.dirtyF);
					this.tireFRF.material.SetFloat("_Blend", this.dirtyF);
					this.tireRLF.material.SetFloat("_Blend", this.dirtyF);
					this.tireRRF.material.SetFloat("_Blend", this.dirtyF);
				}
				if (this.dirty2F <= 1f && this.dirty2F >= 0f)
				{
					Color color = this.f100MudMathood.color;
					color.a = Mathf.Clamp(this.dirty2F, 0f, 1f);
					this.f100MudMathood.color = color;
					this.f100MudMatbody.color = color;
					this.f100MudMatdoorP.color = color;
					this.f100MudMatdoorD.color = color;
					return;
				}
			}
			else if (this.FPSt.parent.name == "SeatMount")
			{
				if (this.dirty250 <= 1f && this.dirty250 >= 0f)
				{
					this.dbtireF.material.SetFloat("_Blend", this.dirty250);
					this.dbtireR.material.SetFloat("_Blend", this.dirty250);
				}
				if (this.dirty2502 <= 5f && this.dirty2502 >= 0f)
				{
					this.plastic.material.SetFloat("_RustIntensity", this.dirty2502);
					this.plastic2.material.SetFloat("_RustIntensity", this.dirty2502);
					this.plastic3.material.SetFloat("_RustIntensity", this.dirty2502);
				}
			}
		}
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x00064DC8 File Offset: 0x00062FC8
	public void WashDirt(int vehNum)
	{
		if (vehNum == 1)
		{
			this.tireFL.material.SetFloat("_Blend", this.dirty);
			this.tireFR.material.SetFloat("_Blend", this.dirty);
			this.tireRL.material.SetFloat("_Blend", this.dirty);
			this.tireRR.material.SetFloat("_Blend", this.dirty);
			this.body.material.SetFloat("_RustIntensity", this.dirty2);
			this.cabin.material.SetFloat("_RustIntensity", this.dirty2);
			this.doorp.material.SetFloat("_RustIntensity", this.dirty2);
			this.doord.material.SetFloat("_RustIntensity", this.dirty2);
			this.tailg.material.SetFloat("_RustIntensity", this.dirty2);
			this.gasdoor.material.SetFloat("_RustIntensity", this.dirty2);
		}
		if (vehNum == 2)
		{
			this.tireFLF.material.SetFloat("_Blend", this.dirtyF);
			this.tireFRF.material.SetFloat("_Blend", this.dirtyF);
			this.tireRLF.material.SetFloat("_Blend", this.dirtyF);
			this.tireRRF.material.SetFloat("_Blend", this.dirtyF);
			Color color = this.f100MudMathood.color;
			color.a = Mathf.Clamp(this.dirty2F, 0f, 1f);
			this.f100MudMathood.color = color;
			this.f100MudMatbody.color = color;
			this.f100MudMatdoorP.color = color;
			this.f100MudMatdoorD.color = color;
		}
		if (vehNum == 3)
		{
			this.dbtireF.material.SetFloat("_Blend", this.dirty250);
			this.dbtireR.material.SetFloat("_Blend", this.dirty250);
			this.plastic.material.SetFloat("_RustIntensity", this.dirty2502);
			this.plastic2.material.SetFloat("_RustIntensity", this.dirty2502);
			this.plastic3.material.SetFloat("_RustIntensity", this.dirty2502);
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x00002188 File Offset: 0x00000388
	public void ChangeDirtB()
	{
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x00065044 File Offset: 0x00063244
	public void UpdateTraction()
	{
		if (this.rockSurface == 1)
		{
			this.newSurf = 3;
		}
		if (this.deepMud == 0 && this.rockSurface == 0)
		{
			this.newSurf = 1;
		}
		if (this.deepMud == 1)
		{
			this.newSurf = 2;
		}
		if (this.deepWater == 1)
		{
			this.newSurf = 4;
		}
		if ((this.lastTractSurf != this.newSurf || this.lastwd4 != this.wd4) && this.FPSt.parent != null)
		{
			if (this.FPSt.parent.name == "DriverCameraController")
			{
				this.interactor.AdjustTractionSurf(this.newSurf);
			}
			else if (this.FPSt.parent.name == "f100seatmount")
			{
				this.interactor.AdjustTractionSurfF(this.newSurf);
			}
			this.lastTractSurf = this.newSurf;
			this.lastwd4 = this.wd4;
		}
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0006513E File Offset: 0x0006333E
	public void GetTerrainTexture()
	{
		this.ConvertPosition(base.transform.position);
		this.CheckTexture();
	}

	// Token: 0x040011EB RID: 4587
	public Terrain t;

	// Token: 0x040011EC RID: 4588
	public int posX;

	// Token: 0x040011ED RID: 4589
	public int posZ;

	// Token: 0x040011EE RID: 4590
	public float dirty;

	// Token: 0x040011EF RID: 4591
	public float dirty2;

	// Token: 0x040011F0 RID: 4592
	public float dirty250;

	// Token: 0x040011F1 RID: 4593
	public float dirty2502;

	// Token: 0x040011F2 RID: 4594
	public float dirtyF;

	// Token: 0x040011F3 RID: 4595
	public float dirty2F;

	// Token: 0x040011F4 RID: 4596
	public int deepMud;

	// Token: 0x040011F5 RID: 4597
	public int leaves;

	// Token: 0x040011F6 RID: 4598
	public int rockSurface;

	// Token: 0x040011F7 RID: 4599
	public int deepWater;

	// Token: 0x040011F8 RID: 4600
	public int road;

	// Token: 0x040011F9 RID: 4601
	public Renderer tireFL;

	// Token: 0x040011FA RID: 4602
	public Renderer tireFR;

	// Token: 0x040011FB RID: 4603
	public Renderer tireRL;

	// Token: 0x040011FC RID: 4604
	public Renderer tireRR;

	// Token: 0x040011FD RID: 4605
	public Renderer tireFLF;

	// Token: 0x040011FE RID: 4606
	public Renderer tireFRF;

	// Token: 0x040011FF RID: 4607
	public Renderer tireRLF;

	// Token: 0x04001200 RID: 4608
	public Renderer tireRRF;

	// Token: 0x04001201 RID: 4609
	public Renderer dbtireR;

	// Token: 0x04001202 RID: 4610
	public Renderer dbtireF;

	// Token: 0x04001203 RID: 4611
	public Renderer plastic;

	// Token: 0x04001204 RID: 4612
	public Renderer plastic2;

	// Token: 0x04001205 RID: 4613
	public Renderer plastic3;

	// Token: 0x04001206 RID: 4614
	public Renderer body;

	// Token: 0x04001207 RID: 4615
	public Renderer cabin;

	// Token: 0x04001208 RID: 4616
	public Renderer doorp;

	// Token: 0x04001209 RID: 4617
	public Renderer doord;

	// Token: 0x0400120A RID: 4618
	public Renderer tailg;

	// Token: 0x0400120B RID: 4619
	public Renderer gasdoor;

	// Token: 0x0400120C RID: 4620
	public Renderer bodyF;

	// Token: 0x0400120D RID: 4621
	public Renderer doorpF;

	// Token: 0x0400120E RID: 4622
	public Renderer doordF;

	// Token: 0x0400120F RID: 4623
	public Renderer tailgF;

	// Token: 0x04001210 RID: 4624
	public Renderer hoodF;

	// Token: 0x04001211 RID: 4625
	private int tireNumFL;

	// Token: 0x04001212 RID: 4626
	private int tireNumFR;

	// Token: 0x04001213 RID: 4627
	private int tireNumRL;

	// Token: 0x04001214 RID: 4628
	private int tireNumRR;

	// Token: 0x04001215 RID: 4629
	public WheelCollider wcFL;

	// Token: 0x04001216 RID: 4630
	public WheelCollider wcFR;

	// Token: 0x04001217 RID: 4631
	public WheelCollider wcRL;

	// Token: 0x04001218 RID: 4632
	public WheelCollider wcRR;

	// Token: 0x04001219 RID: 4633
	private int lastTractSurf;

	// Token: 0x0400121A RID: 4634
	private int newSurf;

	// Token: 0x0400121B RID: 4635
	public bool lastwd4;

	// Token: 0x0400121C RID: 4636
	public bool wd4;

	// Token: 0x0400121D RID: 4637
	public Interactor interactor;

	// Token: 0x0400121E RID: 4638
	private int i;

	// Token: 0x0400121F RID: 4639
	public Transform FPSt;

	// Token: 0x04001220 RID: 4640
	public int roughness;

	// Token: 0x04001221 RID: 4641
	public FirstPersonController fpc;

	// Token: 0x04001222 RID: 4642
	public Material f100MudMathood;

	// Token: 0x04001223 RID: 4643
	public Material f100MudMatbody;

	// Token: 0x04001224 RID: 4644
	public Material f100MudMatdoorD;

	// Token: 0x04001225 RID: 4645
	public Material f100MudMatdoorP;
}
