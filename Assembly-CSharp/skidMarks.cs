using System;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class skidMarks : MonoBehaviour
{
	// Token: 0x060009E9 RID: 2537 RVA: 0x00088050 File Offset: 0x00086250
	private void Start()
	{
		this.em = this.skidSmoke.GetComponent<ParticleSystem>().emission;
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00088068 File Offset: 0x00086268
	private void FixedUpdate()
	{
		if (this.car1.controlled)
		{
			this.wc.GetGroundHit(out this.hit);
			this.currentfrictionValue = Mathf.Abs(this.hit.sidewaysSlip);
			this.stress = this.force - this.hit.force;
			this.force = this.hit.force;
			if (this.stress > (float)this.maxStress && this.car1.speed > 40f)
			{
				if (this.bolt1.boltturns > 0)
				{
					this.bolt1.boltturns -= 2;
					this.bolt1.Start();
				}
				if (this.bolt2.boltturns > 0)
				{
					this.bolt2.boltturns -= 2;
					this.bolt2.Start();
				}
				if (this.bolt3.boltturns > 0)
				{
					this.bolt3.boltturns -= 2;
					this.bolt3.Start();
				}
				if (this.bolt4.boltturns > 0)
				{
					this.bolt4.boltturns -= 2;
					this.bolt4.Start();
				}
				if (this.bolt1.boltturns + this.bolt2.boltturns + this.bolt3.boltturns + this.bolt4.boltturns < 4)
				{
					this.snap = true;
				}
				if (this.snap)
				{
					for (int i = 1; i < 6; i++)
					{
						if (this.thisWheelHolder.GetChild(i).gameObject.activeSelf)
						{
							this.rimNum = i;
							this.thisWheelHolder.GetChild(i).gameObject.SetActive(false);
							this.snap = true;
							break;
						}
						this.snap = false;
					}
				}
				if (this.snap)
				{
					for (int j = 6; j < 20; j++)
					{
						if (this.thisWheelHolder.GetChild(j).gameObject.activeSelf)
						{
							this.tireNum = j;
							this.thisWheelHolder.GetChild(j).gameObject.SetActive(false);
							this.thisWheelHolder.GetChild(0).gameObject.SetActive(false);
						}
					}
					this.wc.radius = 0.23f;
					this.brokenWheel = Object.Instantiate<GameObject>(this.spawnPart, this.detachPos.position, this.detachPos.rotation);
					this.brokenWheel.transform.GetChild(this.rimNum).gameObject.SetActive(true);
					this.brokenWheel.transform.GetChild(this.tireNum).gameObject.SetActive(true);
					this.brokenWheel.GetComponent<TireAssign>().rimNumX = this.rimNum;
					this.brokenWheel.GetComponent<TireAssign>().tireNumX = this.tireNum;
					this.thisDura = this.brakeDisk.GetComponent<durability>().health;
					this.brokenWheel.GetComponent<PickUp>().thisDurability = this.thisDura;
					this.aSources[0].Play();
					this.snap = false;
				}
			}
			else if (this.stress > 70000f && this.stress < (float)this.maxStress)
			{
				if (!this.aSources[1].isPlaying)
				{
					this.aSources[1].Play();
					this.car1.DegradeTires(this.bdNum);
				}
			}
			else if (this.stress > 60000f && this.stress < 40000f)
			{
				if (!this.aSources[2].isPlaying)
				{
					this.aSources[2].Play();
					this.car1.DegradeTires(this.bdNum);
				}
			}
			else if (this.stress > 50000f && this.stress < 35000f && !this.aSources[3].isPlaying)
			{
				this.aSources[3].Play();
				this.car1.DegradeTires(this.bdNum);
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

	// Token: 0x060009EB RID: 2539 RVA: 0x00088554 File Offset: 0x00086754
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

	// Token: 0x060009EC RID: 2540 RVA: 0x00088934 File Offset: 0x00086B34
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

	// Token: 0x04001B71 RID: 7025
	public float bumpMinForce = 4000f;

	// Token: 0x04001B72 RID: 7026
	public float bumpMaxForse = 50000f;

	// Token: 0x04001B73 RID: 7027
	private float bumpMinVolume = 0.2f;

	// Token: 0x04001B74 RID: 7028
	private float bumpMaxVolume = 0.6f;

	// Token: 0x04001B75 RID: 7029
	public car car1;

	// Token: 0x04001B76 RID: 7030
	public Transform thisWheelHolder;

	// Token: 0x04001B77 RID: 7031
	public GameObject spawnPart;

	// Token: 0x04001B78 RID: 7032
	public Transform brakeDisk;

	// Token: 0x04001B79 RID: 7033
	public float minToSlip = 150f;

	// Token: 0x04001B7A RID: 7034
	public bool rear;

	// Token: 0x04001B7B RID: 7035
	public float skidAt = 5.5f;

	// Token: 0x04001B7C RID: 7036
	public float markWidth = 0.2f;

	// Token: 0x04001B7D RID: 7037
	public Material skidMaterial;

	// Token: 0x04001B7E RID: 7038
	public GameObject skidSmoke;

	// Token: 0x04001B7F RID: 7039
	public float currentfrictionValue;

	// Token: 0x04001B80 RID: 7040
	public float thisDura;

	// Token: 0x04001B81 RID: 7041
	private GameObject brokenWheel;

	// Token: 0x04001B82 RID: 7042
	private bool snap;

	// Token: 0x04001B83 RID: 7043
	public int maxStress;

	// Token: 0x04001B84 RID: 7044
	private int skidding;

	// Token: 0x04001B85 RID: 7045
	private Vector3[] lastPos = new Vector3[2];

	// Token: 0x04001B86 RID: 7046
	private WheelHit hit;

	// Token: 0x04001B87 RID: 7047
	private float force;

	// Token: 0x04001B88 RID: 7048
	private float stress;

	// Token: 0x04001B89 RID: 7049
	private bool enableEmission;

	// Token: 0x04001B8A RID: 7050
	private ParticleSystem.EmissionModule em;

	// Token: 0x04001B8B RID: 7051
	public bool skid;

	// Token: 0x04001B8C RID: 7052
	public bool bump;

	// Token: 0x04001B8D RID: 7053
	public float bumpPitch;

	// Token: 0x04001B8E RID: 7054
	public float bumpVolume;

	// Token: 0x04001B8F RID: 7055
	public AudioSource[] aSources;

	// Token: 0x04001B90 RID: 7056
	public WheelCollider wc;

	// Token: 0x04001B91 RID: 7057
	public BoltScript bolt1;

	// Token: 0x04001B92 RID: 7058
	public BoltScript bolt2;

	// Token: 0x04001B93 RID: 7059
	public BoltScript bolt3;

	// Token: 0x04001B94 RID: 7060
	public BoltScript bolt4;

	// Token: 0x04001B95 RID: 7061
	public Transform detachPos;

	// Token: 0x04001B96 RID: 7062
	private int rimNum;

	// Token: 0x04001B97 RID: 7063
	private int tireNum;

	// Token: 0x04001B98 RID: 7064
	public int bdNum;
}
