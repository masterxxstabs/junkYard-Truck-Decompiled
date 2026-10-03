using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x020000D8 RID: 216
public class JiggsCanvas : MonoBehaviour
{
	// Token: 0x0600056F RID: 1391 RVA: 0x000446C8 File Offset: 0x000428C8
	private void OnEnable()
	{
		if (this.v8.GetComponent<PickUp>().pickable)
		{
			if (this.v8.transform.parent == null)
			{
				this.optionv8.GetComponent<Button>().enabled = true;
				this.optionv8.GetComponent<Text>().color = Color.white;
			}
			else if (this.v8.transform.parent.name != "dirt pickup truck" && this.v8.transform.parent.name != "f100")
			{
				this.optionv8.GetComponent<Button>().enabled = true;
				this.optionv8.GetComponent<Text>().color = Color.white;
			}
		}
		if (this.block250.GetComponent<PickUp>().pickable)
		{
			if (this.block250.transform.parent == null)
			{
				this.option250.GetComponent<Button>().enabled = true;
				this.option250.GetComponent<Text>().color = Color.white;
			}
			else if (this.block250.transform.parent.name != "DirtBike")
			{
				this.option250.GetComponent<Button>().enabled = true;
				this.option250.GetComponent<Text>().color = Color.white;
			}
		}
		if (this.v4.GetComponent<PickUp>().pickable)
		{
			if (this.v4.transform.parent == null)
			{
				this.optioni4.GetComponent<Button>().enabled = true;
				this.optioni4.GetComponent<Text>().color = Color.white;
			}
			else if (this.v4.transform.parent.name != "dirt pickup truck")
			{
				this.optioni4.GetComponent<Button>().enabled = true;
				this.optioni4.GetComponent<Text>().color = Color.white;
			}
		}
		if (this.i6.GetComponent<PickUp>().pickable)
		{
			if (this.i6.transform.parent == null)
			{
				this.optioni6.GetComponent<Button>().enabled = true;
				this.optioni6.GetComponent<Text>().color = Color.white;
			}
			else if (this.i6.transform.parent.name != "dirt pickup truck" && this.i6.transform.parent.name != "amc")
			{
				this.optioni6.GetComponent<Button>().enabled = true;
				this.optioni6.GetComponent<Text>().color = Color.white;
			}
		}
		if (!this.jackMount.GetComponent<Renderer>().enabled)
		{
			this.optionJack.GetComponent<Button>().enabled = true;
			this.optionJack.GetComponent<Text>().color = Color.white;
		}
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x000449C0 File Offset: 0x00042BC0
	public void RelocV4()
	{
		if (this.currency.money >= 30f)
		{
			this.v4.transform.parent = null;
			this.v4.GetComponent<FixedJoint>().connectedBody = GameObject.Find("EmptyObjRigidbody").GetComponent<Rigidbody>();
			this.v4.transform.position = this.spawnDest.position;
			this.v4.GetComponent<PickUp>().pickable = true;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x00044A4C File Offset: 0x00042C4C
	public void RelocB1()
	{
		if (this.currency.money >= 30f)
		{
			this.bucket1.transform.parent = null;
			this.bucket1.transform.position = this.spawnDest.position;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x00044AA8 File Offset: 0x00042CA8
	public void RelocCan()
	{
		if (this.currency.money >= 30f)
		{
			this.can.transform.parent = null;
			this.can.transform.position = this.spawnDest.position;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x00044B04 File Offset: 0x00042D04
	public void RelocCreep()
	{
		if (this.currency.money >= 30f)
		{
			this.creeper.transform.parent = null;
			this.creeper.transform.position = this.spawnDest.position;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x00044B60 File Offset: 0x00042D60
	public void RelocChain()
	{
		if (this.currency.money >= 30f)
		{
			this.chainsaw.transform.parent = null;
			this.chainsaw.transform.position = this.spawnDest.position;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x00044BBC File Offset: 0x00042DBC
	public void RelocB2()
	{
		if (this.currency.money >= 30f)
		{
			this.bucket2.transform.parent = null;
			this.bucket2.transform.position = this.spawnDest.position;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x00044C18 File Offset: 0x00042E18
	public void RelocB3()
	{
		if (this.currency.money >= 30f)
		{
			this.bucket3.transform.parent = null;
			this.bucket3.transform.position = this.spawnDest.position;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x00044C74 File Offset: 0x00042E74
	public void RelocV8()
	{
		if (this.currency.money >= 30f)
		{
			this.v8.transform.parent = null;
			this.v8.GetComponent<FixedJoint>().connectedBody = GameObject.Find("EmptyObjRigidbodyV8").GetComponent<Rigidbody>();
			this.v8.transform.position = this.spawnDest.position;
			this.v8.GetComponent<PickUp>().pickable = true;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x00044D00 File Offset: 0x00042F00
	public void RelocI6()
	{
		if (this.currency.money >= 30f)
		{
			this.i6.transform.parent = null;
			this.i6.GetComponent<FixedJoint>().connectedBody = GameObject.Find("EmptyObjRigidbodyi6").GetComponent<Rigidbody>();
			this.i6.transform.position = this.spawnDest.position;
			this.i6.GetComponent<PickUp>().pickable = true;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x00044D8C File Offset: 0x00042F8C
	public void Reloc250()
	{
		if (this.currency.money >= 30f && this.block250.GetComponent<PickUp>().price == 0f)
		{
			this.block250.transform.parent = null;
			this.block250.GetComponent<FixedJoint>().connectedBody = GameObject.Find("EmptyObjRigidbody250").GetComponent<Rigidbody>();
			this.block250.transform.position = this.spawnDest.position;
			this.block250.GetComponent<PickUp>().pickable = true;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x00044E34 File Offset: 0x00043034
	public void ReloJack()
	{
		if (this.currency.money >= 30f)
		{
			GameObject gameObject = GameObject.Find("HiJack");
			if (gameObject == null)
			{
				gameObject = GameObject.Find("HiJack(Clone)");
				if (gameObject == null)
				{
					gameObject = this.jack;
				}
			}
			gameObject.transform.parent = null;
			gameObject.transform.position = this.spawnDest.position;
			this.jiggsScript.PlayFound();
			this.OptionExit();
		}
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x00044EB5 File Offset: 0x000430B5
	public void OptionExit()
	{
		this.jiggsPanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x04000BC9 RID: 3017
	public FirstPersonController fpc;

	// Token: 0x04000BCA RID: 3018
	public GameObject v4;

	// Token: 0x04000BCB RID: 3019
	public GameObject v8;

	// Token: 0x04000BCC RID: 3020
	public GameObject i6;

	// Token: 0x04000BCD RID: 3021
	public GameObject bucket1;

	// Token: 0x04000BCE RID: 3022
	public GameObject bucket2;

	// Token: 0x04000BCF RID: 3023
	public GameObject bucket3;

	// Token: 0x04000BD0 RID: 3024
	public GameObject can;

	// Token: 0x04000BD1 RID: 3025
	public GameObject chainsaw;

	// Token: 0x04000BD2 RID: 3026
	public GameObject creeper;

	// Token: 0x04000BD3 RID: 3027
	public Transform spawnDest;

	// Token: 0x04000BD4 RID: 3028
	public GameObject jiggsPanel;

	// Token: 0x04000BD5 RID: 3029
	public jiggs jiggsScript;

	// Token: 0x04000BD6 RID: 3030
	public Currency currency;

	// Token: 0x04000BD7 RID: 3031
	public GameObject block250;

	// Token: 0x04000BD8 RID: 3032
	public GameObject optionv8;

	// Token: 0x04000BD9 RID: 3033
	public GameObject option250;

	// Token: 0x04000BDA RID: 3034
	public GameObject optioni4;

	// Token: 0x04000BDB RID: 3035
	public GameObject optioni6;

	// Token: 0x04000BDC RID: 3036
	public GameObject optionJack;

	// Token: 0x04000BDD RID: 3037
	public GameObject jack;

	// Token: 0x04000BDE RID: 3038
	public GameObject jackMount;
}
