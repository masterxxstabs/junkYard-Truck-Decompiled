using System;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class AssignMaterial : MonoBehaviour
{
	// Token: 0x06000021 RID: 33 RVA: 0x00002660 File Offset: 0x00000860
	private void Start()
	{
		if (this.tailgateR != 0.1529412f)
		{
			this.tailgate.material.color = new Color(this.tailgateR, this.tailgateG, this.tailgateB, 1f);
			if (this.tailgateM != 0f)
			{
				this.tailgate.material.SetFloat("_Metallic", this.tailgateM);
			}
			if (this.tailgateS != 0f)
			{
				this.tailgate.material.SetFloat("_Smoothness", this.tailgateS);
			}
		}
		if (this.gasdoorR != 0.1529412f)
		{
			this.gasdoor.material.color = new Color(this.gasdoorR, this.gasdoorG, this.gasdoorB, 1f);
			if (this.gasdoorM != 0f)
			{
				this.gasdoor.material.SetFloat("_Metallic", this.gasdoorM);
			}
			if (this.gasdoorS != 0f)
			{
				this.gasdoor.material.SetFloat("_Smoothness", this.gasdoorS);
			}
		}
		if (this.doorpR != 0.1529412f)
		{
			this.doorp.material.color = new Color(this.doorpR, this.doorpG, this.doorpB, 1f);
			if (this.doorpM != 0f)
			{
				this.doorp.material.SetFloat("_Metallic", this.doorpM);
			}
			if (this.doorpS != 0f)
			{
				this.doorp.material.SetFloat("_Smoothness", this.doorpS);
			}
		}
		if (this.doordR != 0.1529412f)
		{
			this.doord.material.color = new Color(this.doordR, this.doordG, this.doordB, 1f);
			if (this.doordM != 0f)
			{
				this.doord.material.SetFloat("_Metallic", this.doordM);
			}
			if (this.doordS != 0f)
			{
				this.doord.material.SetFloat("_Smoothness", this.doordS);
			}
		}
		if (this.hoodR != 0.1529412f)
		{
			this.hood.material.color = new Color(this.hoodR, this.hoodG, this.hoodB, 1f);
			if (this.hoodM != 0f)
			{
				this.hood.material.SetFloat("_Metallic", this.hoodM);
			}
			if (this.hoodS != 0f)
			{
				this.hood.material.SetFloat("_Smoothness", this.hoodS);
			}
		}
		if (this.cabinR != 0.1529412f)
		{
			this.cabin.material.color = new Color(this.cabinR, this.cabinG, this.cabinB, 1f);
			if (this.cabinM != 0f)
			{
				this.cabin.material.SetFloat("_Metallic", this.cabinM);
			}
			if (this.cabinS != 0f)
			{
				this.cabin.material.SetFloat("_Smoothness", this.cabinS);
			}
		}
		if (this.bodyR != 0.1529412f)
		{
			this.body.material.color = new Color(this.bodyR, this.bodyG, this.bodyB, 1f);
			if (this.bodyM != 0f)
			{
				this.body.material.SetFloat("_Metallic", this.bodyM);
			}
			if (this.bodyS != 0f)
			{
				this.body.material.SetFloat("_Smoothness", this.bodyS);
			}
		}
		Material[] materials = this.tailgateF.materials;
		if (this.painted1)
		{
			materials[1].SetTexture("_MainTex", this.m_PaintTexture);
			materials[1].color = new Color(this.tailgateFR, this.tailgateFG, this.tailgateFB, 1f);
			if (this.tailgateFM != 0f)
			{
				materials[1].SetFloat("_Metallic", this.tailgateFM);
			}
			if (this.tailgateFS != 0f)
			{
				materials[1].SetFloat("_Glossiness", this.tailgateFS);
			}
		}
		else
		{
			materials[1].SetTexture("_MainTex", this.m_RustTexture);
		}
		Material[] materials2 = this.doorpF.materials;
		if (this.painted2)
		{
			materials2[1].SetTexture("_MainTex", this.m_PaintTexture);
			materials2[1].color = new Color(this.doorpFR, this.doorpFG, this.doorpFB, 1f);
			if (this.doorpFM != 0f)
			{
				materials2[1].SetFloat("_Metallic", this.doorpFM);
			}
			if (this.doorpFS != 0f)
			{
				materials2[1].SetFloat("_Glossiness", this.doorpFS);
			}
		}
		else
		{
			materials2[1].SetTexture("_MainTex", this.m_RustTexture);
		}
		Material[] materials3 = this.doordF.materials;
		if (this.painted3)
		{
			materials3[1].SetTexture("_MainTex", this.m_PaintTexture);
			materials3[1].color = new Color(this.doordFR, this.doordFG, this.doordFB, 1f);
			if (this.doordFM != 0f)
			{
				materials3[1].SetFloat("_Metallic", this.doordFM);
			}
			if (this.doordFS != 0f)
			{
				materials3[1].SetFloat("_Glossiness", this.doordFS);
			}
		}
		else
		{
			materials3[1].SetTexture("_MainTex", this.m_RustTexture);
		}
		if (this.painted4)
		{
			this.hoodF.material.SetTexture("_MainTex", this.m_PaintTexture);
			this.hoodF.material.color = new Color(this.hoodFR, this.hoodFG, this.hoodFB, 1f);
			if (this.hoodFM != 0f)
			{
				this.hoodF.material.SetFloat("_Metallic", this.hoodFM);
			}
			if (this.hoodFS != 0f)
			{
				this.hoodF.material.SetFloat("_Glossiness", this.hoodFS);
			}
		}
		else
		{
			this.hoodF.material.SetTexture("_MainTex", this.m_RustTexture);
		}
		Material[] materials4 = this.cabinF.materials;
		materials4[1].color = new Color(this.cabinFR, this.cabinFG, this.cabinFB, 1f);
		if (this.cabinFM != 0f)
		{
			materials4[1].SetFloat("_Metallic", this.cabinFM);
		}
		if (this.cabinFS != 0f)
		{
			materials4[1].SetFloat("_Glossiness", this.cabinFS);
		}
		Material[] materials5 = this.bodyF.materials;
		if (this.painted5)
		{
			materials5[1].SetTexture("_MainTex", this.m_PaintTexture);
			materials5[1].color = new Color(this.bodyFR, this.bodyFG, this.bodyFB, 1f);
			if (this.bodyFM != 0f)
			{
				materials5[1].SetFloat("_Metallic", this.bodyFM);
			}
			if (this.bodyFS != 0f)
			{
				materials5[1].SetFloat("_Glossiness", this.bodyFS);
			}
		}
		else
		{
			materials5[1].SetTexture("_MainTex", this.m_RustTexture);
		}
		this.body250.material.color = new Color(this.motoR, this.motoG, this.motoB, 1f);
		this.forks250.material.color = new Color(this.motoR, this.motoG, this.motoB, 1f);
		this.handlebars250.material.color = new Color(this.motoR, this.motoG, this.motoB, 1f);
		if (this.motoM != 0f)
		{
			this.body250.material.SetFloat("_Metallic", this.motoM);
			this.forks250.material.SetFloat("_Metallic", this.motoM);
			this.handlebars250.material.SetFloat("_Metallic", this.motoM);
		}
		if (this.motoS != 0f)
		{
			this.body250.material.SetFloat("_Smoothness", this.motoS);
			this.forks250.material.SetFloat("_Smoothness", this.motoS);
			this.handlebars250.material.SetFloat("_Smoothness", this.motoS);
		}
		this.v8block.material.color = new Color(this.v8blockR, this.v8blockG, this.v8blockB, 1f);
		if (this.v8blockM != 0f)
		{
			this.v8block.material.SetFloat("_Metallic", this.v8blockM);
		}
		if (this.v8blockS != 0f)
		{
			this.v8block.material.SetFloat("_Glossiness", this.v8blockS);
		}
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00002FA0 File Offset: 0x000011A0
	public void RemoveRust(int partNum)
	{
		if (partNum == 1)
		{
			this.tailgateF.materials[1].SetTexture("_MainTex", this.m_PaintTexture);
			return;
		}
		if (partNum == 2)
		{
			this.doorpF.materials[1].SetTexture("_MainTex", this.m_PaintTexture);
			return;
		}
		if (partNum == 3)
		{
			this.doordF.materials[1].SetTexture("_MainTex", this.m_PaintTexture);
			return;
		}
		if (partNum == 4)
		{
			this.hoodF.material.SetTexture("_MainTex", this.m_PaintTexture);
			return;
		}
		if (partNum == 5)
		{
			this.bodyF.materials[1].SetTexture("_MainTex", this.m_PaintTexture);
		}
	}

	// Token: 0x04000019 RID: 25
	public Renderer body;

	// Token: 0x0400001A RID: 26
	public Renderer tailgate;

	// Token: 0x0400001B RID: 27
	public Renderer gasdoor;

	// Token: 0x0400001C RID: 28
	public Renderer doorp;

	// Token: 0x0400001D RID: 29
	public Renderer doord;

	// Token: 0x0400001E RID: 30
	public Renderer hood;

	// Token: 0x0400001F RID: 31
	public Renderer cabin;

	// Token: 0x04000020 RID: 32
	public Renderer bodyF;

	// Token: 0x04000021 RID: 33
	public Renderer tailgateF;

	// Token: 0x04000022 RID: 34
	public Renderer doorpF;

	// Token: 0x04000023 RID: 35
	public Renderer doordF;

	// Token: 0x04000024 RID: 36
	public Renderer hoodF;

	// Token: 0x04000025 RID: 37
	public Renderer cabinF;

	// Token: 0x04000026 RID: 38
	public Renderer body250;

	// Token: 0x04000027 RID: 39
	public Renderer forks250;

	// Token: 0x04000028 RID: 40
	public Renderer handlebars250;

	// Token: 0x04000029 RID: 41
	public Renderer v8block;

	// Token: 0x0400002A RID: 42
	public float cabinR;

	// Token: 0x0400002B RID: 43
	public float cabinG;

	// Token: 0x0400002C RID: 44
	public float cabinB;

	// Token: 0x0400002D RID: 45
	public float cabinM;

	// Token: 0x0400002E RID: 46
	public float cabinS;

	// Token: 0x0400002F RID: 47
	public float bodyR;

	// Token: 0x04000030 RID: 48
	public float bodyG;

	// Token: 0x04000031 RID: 49
	public float bodyB;

	// Token: 0x04000032 RID: 50
	public float bodyM;

	// Token: 0x04000033 RID: 51
	public float bodyS;

	// Token: 0x04000034 RID: 52
	public float tailgateR;

	// Token: 0x04000035 RID: 53
	public float tailgateG;

	// Token: 0x04000036 RID: 54
	public float tailgateB;

	// Token: 0x04000037 RID: 55
	public float tailgateM;

	// Token: 0x04000038 RID: 56
	public float tailgateS;

	// Token: 0x04000039 RID: 57
	public float gasdoorR;

	// Token: 0x0400003A RID: 58
	public float gasdoorG;

	// Token: 0x0400003B RID: 59
	public float gasdoorB;

	// Token: 0x0400003C RID: 60
	public float gasdoorM;

	// Token: 0x0400003D RID: 61
	public float gasdoorS;

	// Token: 0x0400003E RID: 62
	public float doorpR;

	// Token: 0x0400003F RID: 63
	public float doorpG;

	// Token: 0x04000040 RID: 64
	public float doorpB;

	// Token: 0x04000041 RID: 65
	public float doorpM;

	// Token: 0x04000042 RID: 66
	public float doorpS;

	// Token: 0x04000043 RID: 67
	public float doordR;

	// Token: 0x04000044 RID: 68
	public float doordG;

	// Token: 0x04000045 RID: 69
	public float doordB;

	// Token: 0x04000046 RID: 70
	public float doordM;

	// Token: 0x04000047 RID: 71
	public float doordS;

	// Token: 0x04000048 RID: 72
	public float hoodR;

	// Token: 0x04000049 RID: 73
	public float hoodG;

	// Token: 0x0400004A RID: 74
	public float hoodB;

	// Token: 0x0400004B RID: 75
	public float hoodM;

	// Token: 0x0400004C RID: 76
	public float hoodS;

	// Token: 0x0400004D RID: 77
	public float cabinFR;

	// Token: 0x0400004E RID: 78
	public float cabinFG;

	// Token: 0x0400004F RID: 79
	public float cabinFB;

	// Token: 0x04000050 RID: 80
	public float cabinFM;

	// Token: 0x04000051 RID: 81
	public float cabinFS;

	// Token: 0x04000052 RID: 82
	public float bodyFR;

	// Token: 0x04000053 RID: 83
	public float bodyFG;

	// Token: 0x04000054 RID: 84
	public float bodyFB;

	// Token: 0x04000055 RID: 85
	public float bodyFM;

	// Token: 0x04000056 RID: 86
	public float bodyFS;

	// Token: 0x04000057 RID: 87
	public float tailgateFR;

	// Token: 0x04000058 RID: 88
	public float tailgateFG;

	// Token: 0x04000059 RID: 89
	public float tailgateFB;

	// Token: 0x0400005A RID: 90
	public float tailgateFM;

	// Token: 0x0400005B RID: 91
	public float tailgateFS;

	// Token: 0x0400005C RID: 92
	public float doorpFR;

	// Token: 0x0400005D RID: 93
	public float doorpFG;

	// Token: 0x0400005E RID: 94
	public float doorpFB;

	// Token: 0x0400005F RID: 95
	public float doorpFM;

	// Token: 0x04000060 RID: 96
	public float doorpFS;

	// Token: 0x04000061 RID: 97
	public float doordFR;

	// Token: 0x04000062 RID: 98
	public float doordFG;

	// Token: 0x04000063 RID: 99
	public float doordFB;

	// Token: 0x04000064 RID: 100
	public float doordFM;

	// Token: 0x04000065 RID: 101
	public float doordFS;

	// Token: 0x04000066 RID: 102
	public float hoodFR;

	// Token: 0x04000067 RID: 103
	public float hoodFG;

	// Token: 0x04000068 RID: 104
	public float hoodFB;

	// Token: 0x04000069 RID: 105
	public float hoodFM;

	// Token: 0x0400006A RID: 106
	public float hoodFS;

	// Token: 0x0400006B RID: 107
	public bool painted1;

	// Token: 0x0400006C RID: 108
	public bool painted2;

	// Token: 0x0400006D RID: 109
	public bool painted3;

	// Token: 0x0400006E RID: 110
	public bool painted4;

	// Token: 0x0400006F RID: 111
	public bool painted5;

	// Token: 0x04000070 RID: 112
	public Texture m_RustTexture;

	// Token: 0x04000071 RID: 113
	public Texture m_PaintTexture;

	// Token: 0x04000072 RID: 114
	public float motoR;

	// Token: 0x04000073 RID: 115
	public float motoG;

	// Token: 0x04000074 RID: 116
	public float motoB;

	// Token: 0x04000075 RID: 117
	public float motoM;

	// Token: 0x04000076 RID: 118
	public float motoS;

	// Token: 0x04000077 RID: 119
	public float v8blockR;

	// Token: 0x04000078 RID: 120
	public float v8blockG;

	// Token: 0x04000079 RID: 121
	public float v8blockB;

	// Token: 0x0400007A RID: 122
	public float v8blockM;

	// Token: 0x0400007B RID: 123
	public float v8blockS;
}
