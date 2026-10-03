using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class RWGTrailer : MonoBehaviour
{
	// Token: 0x0600078E RID: 1934 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x00062380 File Offset: 0x00060580
	public void LoadCargo(string cargo)
	{
		if (cargo.Contains("log"))
		{
			this.ClearCargo("none");
			this.container.GetChild(0).gameObject.SetActive(true);
			this.addWeight = 300f;
		}
		else if (cargo.Contains("skidsteer"))
		{
			this.ClearCargo("body");
			this.container.GetChild(1).gameObject.SetActive(true);
			this.addWeight = 600f;
		}
		else if (cargo.Contains("watercrate1"))
		{
			this.ClearCargo("watercrate");
			this.container.GetChild(2).gameObject.SetActive(true);
			this.addWeight = 200f;
		}
		else if (cargo.Contains("watercrate2"))
		{
			this.ClearCargo("watercrate");
			this.container.GetChild(3).gameObject.SetActive(true);
			this.addWeight = 200f;
		}
		else if (cargo.Contains("watercrate3"))
		{
			this.ClearCargo("watercrate");
			this.container.GetChild(4).gameObject.SetActive(true);
			this.addWeight = 200f;
		}
		else if (cargo.Contains("diggerbucket"))
		{
			this.ClearCargo("none");
			this.container.GetChild(5).gameObject.SetActive(true);
			this.addWeight = 200f;
		}
		else if (cargo.Contains("excavator"))
		{
			this.ClearCargo("none");
			this.container.GetChild(6).gameObject.SetActive(true);
			this.addWeight = 700f;
		}
		else if (cargo.Contains("pipe1"))
		{
			this.ClearCargo("pipe");
			this.container.GetChild(7).gameObject.SetActive(true);
			this.addWeight = 200f;
		}
		else if (cargo.Contains("pipe2"))
		{
			this.ClearCargo("pipe");
			this.container.GetChild(8).gameObject.SetActive(true);
			this.addWeight = 200f;
		}
		else if (cargo.Contains("beams"))
		{
			this.ClearCargo("none");
			this.container.GetChild(9).gameObject.SetActive(true);
			this.addWeight = 400f;
		}
		this.AddMass();
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x00062600 File Offset: 0x00060800
	private void PlaySound()
	{
		int num = Random.Range(0, 3);
		this.aSource[num].Play();
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x00062624 File Offset: 0x00060824
	public void UnloadCargo()
	{
		if (Vector3.Distance(this.mg.endTrigger.transform.position, base.transform.position) < 10f)
		{
			this.mg.CompleteHeavy();
			this.ClearCargo("none");
		}
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x00062673 File Offset: 0x00060873
	private void AddMass()
	{
		this.rb.mass += this.addWeight;
		this.mg.NextTracker();
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x00062698 File Offset: 0x00060898
	public void ClearCargo(string exclude)
	{
		foreach (object obj in this.container)
		{
			Transform transform = (Transform)obj;
			if (!transform.gameObject.name.Contains(exclude))
			{
				transform.gameObject.SetActive(false);
				foreach (object obj2 in transform)
				{
					((Transform)obj2).gameObject.SetActive(false);
				}
			}
		}
		this.addWeight = 0f;
		this.rb.mass = 300f;
		this.PlaySound();
	}

	// Token: 0x0400113D RID: 4413
	public Transform container;

	// Token: 0x0400113E RID: 4414
	public float addWeight;

	// Token: 0x0400113F RID: 4415
	public AudioSource[] aSource;

	// Token: 0x04001140 RID: 4416
	public MissionGen mg;

	// Token: 0x04001141 RID: 4417
	public Rigidbody rb;
}
