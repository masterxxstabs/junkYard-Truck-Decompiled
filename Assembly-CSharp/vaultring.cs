using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using Steamworks.Data;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public class vaultring : MonoBehaviour
{
	// Token: 0x06000A22 RID: 2594 RVA: 0x0008A6E4 File Offset: 0x000888E4
	private void Start()
	{
		this.iTweenArgs = iTween.Hash(Array.Empty<object>());
		this.iTweenArgs.Add("position", this.rotAngle);
		this.iTweenArgs.Add("time", this.animationTime);
		this.iTweenArgs.Add("islocal", true);
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x0008A750 File Offset: 0x00088950
	public void Rotate()
	{
		this.rotAngle.x = 0f;
		this.rotAngle.y = this.rotAngle.y + 30f;
		if (this.rotAngle.y >= 360f)
		{
			this.rotAngle.y = this.rotAngle.y - 360f;
		}
		this.rotAngle.z = 0f;
		this.iTweenArgs["rotation"] = this.rotAngle;
		iTween.RotateTo(base.gameObject, this.iTweenArgs);
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x0008A7E4 File Offset: 0x000889E4
	public void Push()
	{
		this.movePos.y = 2.1674f;
		this.iTweenArgs["position"] = this.movePos;
		iTween.MoveTo(base.gameObject, this.iTweenArgs);
		base.StartCoroutine(this.Retract());
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x0008A83A File Offset: 0x00088A3A
	private IEnumerator Retract()
	{
		yield return new WaitForSeconds(1.5f);
		this.movePos.y = 2.2674f;
		this.iTweenArgs["position"] = this.movePos;
		iTween.MoveTo(base.gameObject, this.iTweenArgs);
		this.comb4a = this.comb3a;
		this.comb4b = this.comb3b;
		this.comb3a = this.comb2a;
		this.comb3b = this.comb2b;
		this.comb2a = this.comb1a;
		this.comb2b = this.comb1b;
		this.comb1a = Mathf.Round(this.outerR.transform.eulerAngles.y / 30f);
		this.comb1b = Mathf.Round(this.innerR.transform.eulerAngles.y / 30f);
		this.sha1 = string.Concat(new object[]
		{
			this.comb1a,
			"=",
			this.comb1b,
			"=",
			this.comb2a,
			"=",
			this.comb2b,
			"=",
			this.comb3a,
			"=",
			this.comb3b,
			"=",
			this.comb4a,
			"=",
			this.comb4b,
			"=Kaitlin"
		});
		if (vaultring.Sha1Sum2(this.sha1) == "62-BB-43-76-1B-61-4E-34-70-3A-67-BF-84-5A-D1-5F-A7-0E-91-65")
		{
			if (!this.spawned)
			{
				Object.Instantiate<GameObject>(this.pyramid).transform.position = this.pyramidPos.position;
				this.spawned = true;
				AudioSource.PlayClipAtPoint(this.clip1, this.pyramidPos.position, 0.9f);
			}
			Achievement achievement = new Achievement("ACH_EXALTED");
			achievement.Trigger(true);
		}
		yield break;
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0008A84C File Offset: 0x00088A4C
	public static string Sha1Sum2(string str)
	{
		byte[] bytes = new ASCIIEncoding().GetBytes(str);
		return BitConverter.ToString(new SHA1CryptoServiceProvider().ComputeHash(bytes));
	}

	// Token: 0x04001C11 RID: 7185
	private float tempAngle;

	// Token: 0x04001C12 RID: 7186
	private Hashtable iTweenArgs;

	// Token: 0x04001C13 RID: 7187
	private float animationTime = 1f;

	// Token: 0x04001C14 RID: 7188
	public Vector3 rotAngle;

	// Token: 0x04001C15 RID: 7189
	public Vector3 movePos;

	// Token: 0x04001C16 RID: 7190
	public float comb1a;

	// Token: 0x04001C17 RID: 7191
	public float comb1b;

	// Token: 0x04001C18 RID: 7192
	public float comb2a;

	// Token: 0x04001C19 RID: 7193
	public float comb2b;

	// Token: 0x04001C1A RID: 7194
	public float comb3a;

	// Token: 0x04001C1B RID: 7195
	public float comb3b;

	// Token: 0x04001C1C RID: 7196
	public float comb4a;

	// Token: 0x04001C1D RID: 7197
	public float comb4b;

	// Token: 0x04001C1E RID: 7198
	public GameObject innerR;

	// Token: 0x04001C1F RID: 7199
	public GameObject outerR;

	// Token: 0x04001C20 RID: 7200
	public string sha1;

	// Token: 0x04001C21 RID: 7201
	public GameObject pyramid;

	// Token: 0x04001C22 RID: 7202
	public bool spawned;

	// Token: 0x04001C23 RID: 7203
	public Transform pyramidPos;

	// Token: 0x04001C24 RID: 7204
	public AudioSource aSource;

	// Token: 0x04001C25 RID: 7205
	public AudioClip clip1;
}
