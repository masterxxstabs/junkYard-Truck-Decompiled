using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public class Car : MonoBehaviour
{
	// Token: 0x0600047B RID: 1147 RVA: 0x0002F82D File Offset: 0x0002DA2D
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody>();
		base.StartCoroutine(this.StartVolume());
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0002F848 File Offset: 0x0002DA48
	private void Start()
	{
		this.AI = (base.GetComponent<AIControl>() != null);
		base.transform.Find("CarHull").GetComponent<MeshRenderer>().material.color = this.Color;
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x0002F881 File Offset: 0x0002DA81
	private IEnumerator StartVolume()
	{
		while (this.EngineSound.volume < 1f)
		{
			this.EngineSound.volume += Time.deltaTime * 0.5f;
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0002F890 File Offset: 0x0002DA90
	public void FixedUpdate()
	{
		float magnitude = this.body.velocity.magnitude;
		float magnitude2 = this.body.angularVelocity.magnitude;
		if (this.Broken)
		{
			this.Control = Vector3.zero;
		}
		if (base.transform.position.y < 0.1f)
		{
			this.body.AddForce(base.transform.forward * this.Control.y * 20f);
			if (magnitude2 < 2f)
			{
				float num = this.Control.x;
				if (this.Control.y < 0f)
				{
					num *= -1f;
				}
				this.body.AddTorque(base.transform.up * num * 20f);
			}
		}
		if (magnitude2 < 0.1f && magnitude < 0.1f)
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			if (eulerAngles.x > 180f)
			{
				eulerAngles.x -= 360f;
			}
			if (eulerAngles.z > 180f)
			{
				eulerAngles.z -= 360f;
			}
			eulerAngles.x = Mathf.Abs(eulerAngles.x);
			eulerAngles.z = Mathf.Abs(eulerAngles.z);
			if (eulerAngles.x > 1f || eulerAngles.z > 1f)
			{
				base.Invoke("UnturnCar", 2f);
			}
		}
		Vector3 localEulerAngles = new Vector3(0f, this.Control.x * 45f, 0f);
		this.LTire.transform.localEulerAngles = localEulerAngles;
		this.RTire.transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x0002FA74 File Offset: 0x0002DC74
	private void UnturnCar()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.eulerAngles = eulerAngles;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x0002FAB1 File Offset: 0x0002DCB1
	private void Update()
	{
		this.ProcessEngineSound();
		if (!this.AI)
		{
			this.ProcessDriftSound();
		}
		this.CheckDamage();
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x0002FAD0 File Offset: 0x0002DCD0
	private void ProcessEngineSound()
	{
		float num = this.body.velocity.magnitude / 20f;
		if (this.Control.y != 0f)
		{
			num += 0.25f;
			num *= 2f;
		}
		num += 0.2f;
		if (this.Broken)
		{
			num = 0f;
		}
		this.RPM = Mathf.Lerp(this.RPM, num, Time.deltaTime * 3f);
		this.EngineSound.pitch = this.RPM;
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x0002FB60 File Offset: 0x0002DD60
	private void ProcessDriftSound()
	{
		float num = Mathf.Abs(this.body.angularVelocity.y);
		if (num >= 1f && !this.DriftSound.isPlaying)
		{
			this.DriftSound.Play();
		}
		if (num < 1f && this.DriftSound.isPlaying)
		{
			this.DriftSound.Stop();
		}
		if (this.DriftSound.isPlaying)
		{
			this.DriftSound.volume = num * 0.5f;
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x06000483 RID: 1155 RVA: 0x0002FBE2 File Offset: 0x0002DDE2
	public float CarDamage
	{
		get
		{
			return Mathf.Clamp01(this.CarHullDeformable.StructuralDamage / 0.065f);
		}
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x0002FBFA File Offset: 0x0002DDFA
	private void CheckDamage()
	{
		this.Broken = (this.CarDamage >= 1f);
		this.Smoke.enableEmission = this.Broken;
	}

	// Token: 0x0400094A RID: 2378
	public Color Color;

	// Token: 0x0400094B RID: 2379
	public Transform RTire;

	// Token: 0x0400094C RID: 2380
	public Transform LTire;

	// Token: 0x0400094D RID: 2381
	public Vector2 Control;

	// Token: 0x0400094E RID: 2382
	public AudioSource EngineSound;

	// Token: 0x0400094F RID: 2383
	public AudioSource DriftSound;

	// Token: 0x04000950 RID: 2384
	public ParticleSystem Smoke;

	// Token: 0x04000951 RID: 2385
	public ImpactDeformable CarHullDeformable;

	// Token: 0x04000952 RID: 2386
	private float RPM;

	// Token: 0x04000953 RID: 2387
	private Rigidbody body;

	// Token: 0x04000954 RID: 2388
	private bool AI;

	// Token: 0x04000955 RID: 2389
	private ImpactDeformable impactDeformable;

	// Token: 0x04000956 RID: 2390
	private bool Broken;
}
