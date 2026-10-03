using System;
using UnityEngine;

// Token: 0x02000195 RID: 405
public class skidMarksCar : MonoBehaviour
{
	// Token: 0x060009EE RID: 2542 RVA: 0x000889F8 File Offset: 0x00086BF8
	private void Start()
	{
		this.em = this.skidSmoke.GetComponent<ParticleSystem>().emission;
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00088A10 File Offset: 0x00086C10
	private void Update()
	{
		if (this.car1.userControlled)
		{
			base.transform.GetComponent<WheelCollider>().GetGroundHit(out this.hit);
			this.currentfrictionValue = Mathf.Abs(this.hit.sidewaysSlip);
			this.stress = this.force - this.hit.force;
			this.force = this.hit.force;
			if ((this.stress <= 35000f || this.stress >= 40001f) && (this.stress <= 30000f || this.stress >= 35001f) && this.stress > 26000f)
			{
				float num = this.stress;
			}
			this.BumpPlay(this.stress);
			float rpm = base.transform.GetComponent<WheelCollider>().rpm;
			if (this.skidAt <= this.currentfrictionValue || (rpm < this.minToSlip && Input.GetAxis("Vertical") > 0f && this.rear && this.hit.collider && this.car1.torque > 0f))
			{
				this.SkidMesh();
				this.skid = true;
			}
			else
			{
				this.skidding = 0;
				this.enableEmission = false;
				this.skid = false;
			}
			if (base.transform.GetComponentInParent<Rigidbody>().velocity.magnitude > 5f)
			{
				this.enableEmission = true;
			}
			else
			{
				this.enableEmission = false;
			}
			if (this.skidSmoke != null)
			{
				this.em.enabled = this.enableEmission;
			}
		}
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00088BB4 File Offset: 0x00086DB4
	private void SkidMesh()
	{
		GameObject gameObject = new GameObject("Mark");
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[4];
		int[] array2 = new int[6];
		if (this.skidding == 0)
		{
			array[0] = this.hit.point + Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z) * new Vector3(this.markWidth, 0.01f, 0f);
			array[0] = this.hit.point + Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z) * new Vector3(-this.markWidth, 0.01f, 0f);
			array[2] = this.hit.point + Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z) * new Vector3(-this.markWidth, 0.01f, 0f);
			array[3] = this.hit.point + Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z) * new Vector3(this.markWidth, 0.01f, 0f);
			this.lastPos[0] = array[2];
			this.lastPos[1] = array[3];
			this.skidding = 1;
		}
		else
		{
			array[1] = this.lastPos[0];
			array[0] = this.lastPos[1];
			array[2] = this.hit.point + Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z) * new Vector3(-this.markWidth, 0.01f, 0f);
			array[3] = this.hit.point + Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z) * new Vector3(this.markWidth, 0.01f, 0f);
			this.lastPos[0] = array[2];
			this.lastPos[1] = array[3];
		}
		array2[0] = 0;
		array2[1] = 1;
		array2[2] = 2;
		array2[3] = 2;
		array2[4] = 3;
		array2[5] = 0;
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.RecalculateNormals();
		mesh.uv = new Vector2[]
		{
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		meshFilter.mesh = mesh;
		gameObject.GetComponent<Renderer>().material = this.skidMaterial;
		gameObject.AddComponent<destroyAfter>();
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00088F94 File Offset: 0x00087194
	private void BumpPlay(float SuspensionStress)
	{
		float num = Mathf.InverseLerp(this.bumpMinForce, this.bumpMaxForse, SuspensionStress);
		if (num > 0f)
		{
			this.bumpPitch = num;
			this.bumpVolume = Mathf.Lerp(this.bumpMinVolume, this.bumpMaxVolume, num);
			this.bump = true;
			return;
		}
		this.bump = false;
	}

	// Token: 0x04001B99 RID: 7065
	public float bumpMinForce = 4000f;

	// Token: 0x04001B9A RID: 7066
	public float bumpMaxForse = 18000f;

	// Token: 0x04001B9B RID: 7067
	private float bumpMinVolume = 0.2f;

	// Token: 0x04001B9C RID: 7068
	private float bumpMaxVolume = 0.6f;

	// Token: 0x04001B9D RID: 7069
	public car3 car1;

	// Token: 0x04001B9E RID: 7070
	public float minToSlip = 150f;

	// Token: 0x04001B9F RID: 7071
	public bool rear;

	// Token: 0x04001BA0 RID: 7072
	public float skidAt = 5.5f;

	// Token: 0x04001BA1 RID: 7073
	public float markWidth = 0.2f;

	// Token: 0x04001BA2 RID: 7074
	public Material skidMaterial;

	// Token: 0x04001BA3 RID: 7075
	public GameObject skidSmoke;

	// Token: 0x04001BA4 RID: 7076
	public float currentfrictionValue;

	// Token: 0x04001BA5 RID: 7077
	private int skidding;

	// Token: 0x04001BA6 RID: 7078
	private Vector3[] lastPos = new Vector3[2];

	// Token: 0x04001BA7 RID: 7079
	private WheelHit hit;

	// Token: 0x04001BA8 RID: 7080
	private float force;

	// Token: 0x04001BA9 RID: 7081
	private float stress;

	// Token: 0x04001BAA RID: 7082
	private bool enableEmission;

	// Token: 0x04001BAB RID: 7083
	private ParticleSystem.EmissionModule em;

	// Token: 0x04001BAC RID: 7084
	public bool skid;

	// Token: 0x04001BAD RID: 7085
	public bool bump;

	// Token: 0x04001BAE RID: 7086
	public float bumpPitch;

	// Token: 0x04001BAF RID: 7087
	public float bumpVolume;

	// Token: 0x04001BB0 RID: 7088
	public AudioSource[] aSources;
}
