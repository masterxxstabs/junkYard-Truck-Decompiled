using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x020000AE RID: 174
public class FSMScript : MonoBehaviour
{
	// Token: 0x06000438 RID: 1080 RVA: 0x0002C801 File Offset: 0x0002AA01
	private void Start()
	{
		if (this.pageNum == 0)
		{
			this.prevArrow.SetActive(false);
		}
		if (this.pageNum == 27)
		{
			this.nextArrow.SetActive(false);
		}
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x0002C82D File Offset: 0x0002AA2D
	public void CloseBook()
	{
		this.canv.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x0002C854 File Offset: 0x0002AA54
	public void NextPage()
	{
		if (this.pageNum < 27)
		{
			this.pageNum++;
			this.ChangePage(this.pageNum);
			if (this.pageNum == 27)
			{
				this.nextArrow.SetActive(false);
				return;
			}
			this.nextArrow.SetActive(true);
			this.prevArrow.SetActive(true);
		}
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x0002C8B4 File Offset: 0x0002AAB4
	public void PrevPage()
	{
		if (this.pageNum > 0)
		{
			this.pageNum--;
			this.ChangePage(this.pageNum);
			if (this.pageNum == 0)
			{
				this.prevArrow.SetActive(false);
				return;
			}
			this.prevArrow.SetActive(true);
			this.nextArrow.SetActive(true);
		}
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x0002C914 File Offset: 0x0002AB14
	private void ChangePage(int pageNum)
	{
		switch (pageNum)
		{
		case 0:
			this.pageHolder.GetComponent<RawImage>().texture = this.page0;
			return;
		case 1:
			this.pageHolder.GetComponent<RawImage>().texture = this.page1;
			return;
		case 2:
			this.pageHolder.GetComponent<RawImage>().texture = this.page2;
			return;
		case 3:
			this.pageHolder.GetComponent<RawImage>().texture = this.page3;
			return;
		case 4:
			this.pageHolder.GetComponent<RawImage>().texture = this.page4;
			return;
		case 5:
			this.pageHolder.GetComponent<RawImage>().texture = this.page5;
			return;
		case 6:
			this.pageHolder.GetComponent<RawImage>().texture = this.page6;
			return;
		case 7:
			this.pageHolder.GetComponent<RawImage>().texture = this.page7;
			return;
		case 8:
			this.pageHolder.GetComponent<RawImage>().texture = this.page8;
			return;
		case 9:
			this.pageHolder.GetComponent<RawImage>().texture = this.page9;
			return;
		case 10:
			this.pageHolder.GetComponent<RawImage>().texture = this.page10;
			return;
		case 11:
			this.pageHolder.GetComponent<RawImage>().texture = this.page11;
			return;
		case 12:
			this.pageHolder.GetComponent<RawImage>().texture = this.page12;
			return;
		case 13:
			this.pageHolder.GetComponent<RawImage>().texture = this.page13;
			return;
		case 14:
			this.pageHolder.GetComponent<RawImage>().texture = this.page14;
			return;
		case 15:
			this.pageHolder.GetComponent<RawImage>().texture = this.page15;
			return;
		case 16:
			this.pageHolder.GetComponent<RawImage>().texture = this.page16;
			return;
		case 17:
			this.pageHolder.GetComponent<RawImage>().texture = this.page17;
			return;
		case 18:
			this.pageHolder.GetComponent<RawImage>().texture = this.page18;
			return;
		case 19:
			this.pageHolder.GetComponent<RawImage>().texture = this.page19;
			return;
		case 20:
			this.pageHolder.GetComponent<RawImage>().texture = this.page20;
			return;
		case 21:
			this.pageHolder.GetComponent<RawImage>().texture = this.page21;
			return;
		case 22:
			this.pageHolder.GetComponent<RawImage>().texture = this.page22;
			return;
		case 23:
			this.pageHolder.GetComponent<RawImage>().texture = this.page23;
			return;
		case 24:
			this.pageHolder.GetComponent<RawImage>().texture = this.page24;
			return;
		case 25:
			this.pageHolder.GetComponent<RawImage>().texture = this.page25;
			return;
		case 26:
			this.pageHolder.GetComponent<RawImage>().texture = this.page26;
			return;
		case 27:
			this.pageHolder.GetComponent<RawImage>().texture = this.page27;
			return;
		default:
			return;
		}
	}

	// Token: 0x04000872 RID: 2162
	public Texture page0;

	// Token: 0x04000873 RID: 2163
	public Texture page1;

	// Token: 0x04000874 RID: 2164
	public Texture page2;

	// Token: 0x04000875 RID: 2165
	public Texture page3;

	// Token: 0x04000876 RID: 2166
	public Texture page4;

	// Token: 0x04000877 RID: 2167
	public Texture page5;

	// Token: 0x04000878 RID: 2168
	public Texture page6;

	// Token: 0x04000879 RID: 2169
	public Texture page7;

	// Token: 0x0400087A RID: 2170
	public Texture page8;

	// Token: 0x0400087B RID: 2171
	public Texture page9;

	// Token: 0x0400087C RID: 2172
	public Texture page10;

	// Token: 0x0400087D RID: 2173
	public Texture page11;

	// Token: 0x0400087E RID: 2174
	public Texture page12;

	// Token: 0x0400087F RID: 2175
	public Texture page13;

	// Token: 0x04000880 RID: 2176
	public Texture page14;

	// Token: 0x04000881 RID: 2177
	public Texture page15;

	// Token: 0x04000882 RID: 2178
	public Texture page16;

	// Token: 0x04000883 RID: 2179
	public Texture page17;

	// Token: 0x04000884 RID: 2180
	public Texture page18;

	// Token: 0x04000885 RID: 2181
	public Texture page19;

	// Token: 0x04000886 RID: 2182
	public Texture page20;

	// Token: 0x04000887 RID: 2183
	public Texture page21;

	// Token: 0x04000888 RID: 2184
	public Texture page22;

	// Token: 0x04000889 RID: 2185
	public Texture page23;

	// Token: 0x0400088A RID: 2186
	public Texture page24;

	// Token: 0x0400088B RID: 2187
	public Texture page25;

	// Token: 0x0400088C RID: 2188
	public Texture page26;

	// Token: 0x0400088D RID: 2189
	public Texture page27;

	// Token: 0x0400088E RID: 2190
	public GameObject pageHolder;

	// Token: 0x0400088F RID: 2191
	public int pageNum;

	// Token: 0x04000890 RID: 2192
	public GameObject canv;

	// Token: 0x04000891 RID: 2193
	public GameObject nextArrow;

	// Token: 0x04000892 RID: 2194
	public GameObject prevArrow;

	// Token: 0x04000893 RID: 2195
	public FirstPersonController fpc;
}
