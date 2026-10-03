using System;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class lights : MonoBehaviour
{
	// Token: 0x06000987 RID: 2439 RVA: 0x0007FDBC File Offset: 0x0007DFBC
	private void Start()
	{
		this.flare = base.GetComponent<LensFlare>();
		this.flareColor = this.flare.color;
		this.flare.enabled = true;
		if (base.GetComponent<Light>() != null)
		{
			this.m_light = base.GetComponent<Light>();
			this.initialValueOfTheLight = this.m_light.intensity;
			this.lightColor = this.m_light.color;
		}
		this.m_Camera = Camera.main.transform;
		this.brokenAudio = base.gameObject.AddComponent<AudioSource>();
		this.brokenAudio.loop = false;
		this.brokenAudio.volume = 1f;
		this.brokenAudio.spatialBlend = 1f;
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x0007FE7C File Offset: 0x0007E07C
	private void Update()
	{
		this.Fracture();
		this.finalColor = Color.white * Mathf.LinearToGammaSpace(this.emissive * this.lightIntensity);
		if (base.transform.parent != null)
		{
			base.transform.parent.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.finalColor);
		}
		if (this.lightIntensity > 0f && !this.destroyed)
		{
			this.emissive = 1f;
			this.direction = this.FlareDir(this.flareDirection);
			this.distanceTocam = Vector3.Distance(base.transform.position, this.m_Camera.position);
			this.angle = Vector3.Angle(this.direction, this.m_Camera.position - base.transform.position);
			if (!this.udirected)
			{
				if (this.angle != 0f && !this.twoSided)
				{
					this.finalFlareBrightness = this.flareBrightness * (4f / this.distanceTocam) * ((100f - 1.11f * this.angle) / 100f);
				}
				else if (this.angle != 0f && this.twoSided)
				{
					this.finalFlareBrightness = this.flareBrightness * (4f / this.distanceTocam * (Mathf.Abs(100f - 1.11f * this.angle) / 100f));
				}
			}
			else if (this.angle != 0f)
			{
				this.finalFlareBrightness = this.flareBrightness * (4f / this.distanceTocam);
			}
			this.flare.brightness = this.finalFlareBrightness * this.lightIntensity;
		}
		else
		{
			this.flare.brightness = 0f;
			this.emissive = 0f;
		}
		if (base.GetComponent<Light>() != null)
		{
			if (this.lightIntensity > 0f && !this.destroyed)
			{
				this.m_light.enabled = true;
				this.m_light.intensity = this.lightIntensity * this.initialValueOfTheLight;
				return;
			}
			this.m_light.enabled = false;
		}
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x000800C0 File Offset: 0x0007E2C0
	private void Fracture()
	{
		if (this.destroy && !this.destroyed)
		{
			if (this.destroyedPrefab != null)
			{
				this.cloneOfPrefab = Object.Instantiate<GameObject>(this.destroyedPrefab, base.transform.root.position, base.transform.root.rotation);
			}
			if (base.transform.parent != null)
			{
				base.transform.parent.GetComponent<MeshRenderer>().enabled = false;
			}
			if (base.transform.parent.GetComponent<MeshCollider>() != null)
			{
				base.transform.parent.GetComponent<MeshCollider>().enabled = false;
			}
			if (this.brokenSound.Length != 0)
			{
				this.brokenAudio.clip = this.brokenSound[Random.Range(0, this.brokenSound.Length)];
				this.brokenAudio.Play();
			}
			this.lightIntensity = 0f;
			foreach (Collider collider in Physics.OverlapSphere(base.transform.position, this.radius))
			{
				if (collider.attachedRigidbody != null)
				{
					collider.attachedRigidbody.AddExplosionForce(this.force, base.transform.position, this.radius, this.upwardModifier, this.forceMode);
				}
			}
		}
		if (!this.destroy && this.destroyed)
		{
			if (base.transform.parent != null)
			{
				base.transform.parent.GetComponent<MeshRenderer>().enabled = true;
			}
			if (base.transform.parent.GetComponent<MeshCollider>() != null)
			{
				base.transform.parent.GetComponent<MeshCollider>().enabled = true;
			}
			if (this.cloneOfPrefab != null)
			{
				Object.Destroy(this.cloneOfPrefab);
			}
		}
		this.destroyed = this.destroy;
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x000802AC File Offset: 0x0007E4AC
	private Vector3 FlareDir(lights.directions d)
	{
		switch (d)
		{
		case lights.directions.forvard:
			return base.transform.forward;
		case lights.directions.back:
			return base.transform.forward * -1f;
		case lights.directions.up:
			return base.transform.up;
		case lights.directions.down:
			return base.transform.up * -1f;
		case lights.directions.left:
			return base.transform.right * -1f;
		case lights.directions.right:
			return base.transform.right;
		case lights.directions.forvardLeft:
			return base.transform.forward + base.transform.right * -1f;
		case lights.directions.forvardRight:
			return base.transform.forward + base.transform.right;
		case lights.directions.backLeft:
			return base.transform.forward * -1f + base.transform.right * -1f;
		case lights.directions.backRight:
			return base.transform.forward * -1f + base.transform.right;
		default:
			return base.transform.forward;
		}
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x000803F5 File Offset: 0x0007E5F5
	public void SetColorOfFlare(Color col)
	{
		this.flare.color = col;
		if (this.m_light != null)
		{
			this.m_light.color = col;
		}
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x0008041D File Offset: 0x0007E61D
	public void SetColorOfFlare(bool value)
	{
		if (!value)
		{
			this.flare.color = this.flareColor;
			if (this.m_light != null)
			{
				this.m_light.color = this.lightColor;
			}
		}
	}

	// Token: 0x04001970 RID: 6512
	[Range(0f, 1f)]
	public float lightIntensity;

	// Token: 0x04001971 RID: 6513
	private Transform m_Camera;

	// Token: 0x04001972 RID: 6514
	private float distanceTocam;

	// Token: 0x04001973 RID: 6515
	private float emissive;

	// Token: 0x04001974 RID: 6516
	private float angle;

	// Token: 0x04001975 RID: 6517
	private float initialValueOfTheLight;

	// Token: 0x04001976 RID: 6518
	private LensFlare flare;

	// Token: 0x04001977 RID: 6519
	private Color flareColor;

	// Token: 0x04001978 RID: 6520
	private Color lightColor;

	// Token: 0x04001979 RID: 6521
	public float flareBrightness = 1f;

	// Token: 0x0400197A RID: 6522
	private float finalFlareBrightness;

	// Token: 0x0400197B RID: 6523
	public bool udirected;

	// Token: 0x0400197C RID: 6524
	public lights.directions flareDirection;

	// Token: 0x0400197D RID: 6525
	private Vector3 direction;

	// Token: 0x0400197E RID: 6526
	private Color finalColor;

	// Token: 0x0400197F RID: 6527
	private Light m_light;

	// Token: 0x04001980 RID: 6528
	public bool twoSided;

	// Token: 0x04001981 RID: 6529
	public GameObject destroyedPrefab;

	// Token: 0x04001982 RID: 6530
	private GameObject cloneOfPrefab;

	// Token: 0x04001983 RID: 6531
	public bool destroy;

	// Token: 0x04001984 RID: 6532
	private bool destroyed;

	// Token: 0x04001985 RID: 6533
	public float force = 125f;

	// Token: 0x04001986 RID: 6534
	public float radius = 5f;

	// Token: 0x04001987 RID: 6535
	public float upwardModifier = 0.1f;

	// Token: 0x04001988 RID: 6536
	public ForceMode forceMode;

	// Token: 0x04001989 RID: 6537
	public AudioClip[] brokenSound;

	// Token: 0x0400198A RID: 6538
	private AudioSource brokenAudio;

	// Token: 0x0200043A RID: 1082
	public enum directions
	{
		// Token: 0x0400299F RID: 10655
		forvard,
		// Token: 0x040029A0 RID: 10656
		back,
		// Token: 0x040029A1 RID: 10657
		up,
		// Token: 0x040029A2 RID: 10658
		down,
		// Token: 0x040029A3 RID: 10659
		left,
		// Token: 0x040029A4 RID: 10660
		right,
		// Token: 0x040029A5 RID: 10661
		forvardLeft,
		// Token: 0x040029A6 RID: 10662
		forvardRight,
		// Token: 0x040029A7 RID: 10663
		backLeft,
		// Token: 0x040029A8 RID: 10664
		backRight
	}
}
