using System;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class golfcart : MonoBehaviour
{
	// Token: 0x0600091F RID: 2335 RVA: 0x0007C724 File Offset: 0x0007A924
	private void Start()
	{
		base.GetComponent<Rigidbody>().centerOfMass = this.centerOfMass.localPosition;
		this.rot = this.steerWheel.transform.localRotation;
		this.surfaceType = 0f;
		this.canAcc = true;
		this.cr = GameObject.Find("EventSystem2").GetComponent<ControlRef>();
		this.battery18.SetActive(true);
		this.battery.SetActive(true);
		if (this.battery.GetComponent<MeshRenderer>().enabled)
		{
			this.battery18.SetActive(false);
			this.maxTorque = 300f;
			this.maxTorqueStatic = 300f;
		}
		if (this.battery18.GetComponent<MeshRenderer>().enabled)
		{
			this.battery.SetActive(false);
			this.maxTorque = 450f;
			this.maxTorqueStatic = 450f;
		}
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x0007C804 File Offset: 0x0007AA04
	private void Update()
	{
		if (!this.userControlled)
		{
			this.torque = 0f;
			this.frontTorque = 0f;
			this.rearTorque = 0f;
			this.WheelRearLeft.brakeTorque = 3000f;
			this.WheelRearRight.brakeTorque = 3000f;
			this.WheelRearRight.motorTorque = 0f;
			this.WheelRearLeft.motorTorque = 0f;
			this.speed = 0f;
			return;
		}
		this.control();
		this.surfaceType = 0f;
		if (this.surfaceType == 1f)
		{
			this.maxTorque = this.maxTorqueStatic / 4f;
			return;
		}
		this.maxTorque = this.maxTorqueStatic;
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x0007C8C3 File Offset: 0x0007AAC3
	public void StopCart()
	{
		this.audioCtrl.GetComponent<AudioControlGolf>().StopSounds();
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x0007C8D8 File Offset: 0x0007AAD8
	private void FixedUpdate()
	{
		if (this.controlled)
		{
			if (Time.time > (float)this.materialCheckInterval && this.speed > 1f)
			{
				this.materialCheckInterval = Mathf.FloorToInt(Time.time) + 1;
				this.groundDetect.GetTerrainTexture();
				this.deepMud = this.groundDetect.deepMud;
			}
			if (this.deepMud == 1 && this.rb.drag < 4f)
			{
				this.rb.drag += 0.15f;
			}
			else
			{
				this.rb.drag = 0.5f;
			}
			this.speed = this.rb.velocity.magnitude * 3.6f;
			this.HandBrake();
			if (this.speed > 1f)
			{
				if (!this.using18v)
				{
					this.batterydura.health -= 0.1f * Time.deltaTime;
				}
				else
				{
					this.batterydura18.health -= 0.1f * Time.deltaTime;
				}
				if (Time.time >= (float)this.engineCheckInterval)
				{
					this.engineCheckInterval = Mathf.FloorToInt(Time.time) + 10;
					if ((this.batterydura.health >= 10f || this.using18v) && this.batterydura18.health < 10f)
					{
						bool flag = this.using18v;
					}
				}
			}
		}
		if (this.controlled)
		{
			this.Fdrag = this.Cdrag * Mathf.Pow(this.speed, 2f);
			this.WheelOffset(this.wheelFL, this.WheelFrontLeft);
			this.WheelOffset(this.wheelFR, this.WheelFrontRight);
			this.WheelOffset(this.wheelRL, this.WheelRearLeft);
			this.WheelOffset(this.wheelRR, this.WheelRearRight);
		}
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x0007CAB8 File Offset: 0x0007ACB8
	private void control()
	{
		if (this.userControlled)
		{
			this.torque = this.maxTorque * this.cr.Vert;
			if (this.cr.GasInput != 0f)
			{
				this.torque = this.maxTorque * this.cr.GasInput;
			}
		}
		this.maxSpeed = 100f;
		if (this.cr.Vert > 0f && this.speed > this.maxSpeed)
		{
			this.torque = 0f;
			this.frontTorque = 0f;
			this.rearTorque = 0f;
			this.WheelRearLeft.brakeTorque = 3000f;
			this.WheelRearRight.brakeTorque = 3000f;
			this.WheelRearRight.motorTorque = 0f;
			this.WheelRearLeft.motorTorque = 0f;
			this.speed = 0f;
		}
		this.WheelRearRight.motorTorque = this.torque;
		this.WheelRearLeft.motorTorque = this.torque;
		this.WheelFrontLeft.motorTorque = this.torque;
		this.WheelFrontRight.motorTorque = this.torque;
		if (this.userControlled && this.controlled)
		{
			this.SteerWheelControl();
		}
		if (this.batterydura.health < 2f && !this.using18v)
		{
			this.canRun = false;
			this.controlled = false;
			this.WheelRearLeft.brakeTorque = this.maxBrakeTorque;
			this.WheelRearRight.brakeTorque = this.maxBrakeTorque;
			this.WheelRearRight.motorTorque = 0f;
			this.WheelRearLeft.motorTorque = 0f;
		}
		if (this.batterydura18.health < 2f && this.using18v)
		{
			this.canRun = false;
			this.controlled = false;
			this.WheelRearLeft.brakeTorque = this.maxBrakeTorque;
			this.WheelRearRight.brakeTorque = this.maxBrakeTorque;
			this.WheelRearRight.motorTorque = 0f;
			this.WheelRearLeft.motorTorque = 0f;
		}
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x0007CCD4 File Offset: 0x0007AED4
	private void SteerWheelControl()
	{
		if (this.angleChangeTimer < 0f)
		{
			this.steerwheelOffset = Random.Range(-0.4f, 0.4f) * this.speed;
			this.angleChangeTimer += Random.Range(0.1f, 1f);
		}
		float t = this.speed / 3f / this.lowestSpeedAtSteer;
		float num = Mathf.Lerp(this.lowSpeedSteerAngle, this.highSpeedSteerAngle, t);
		if (this.cr.xAxes == 0f || this.cr.Horiz != 0f)
		{
			num *= this.cr.Horiz;
		}
		else
		{
			num *= this.cr.xAxes;
		}
		if (this.interactor.currency.drunk > 6 && this.speed > 2f)
		{
			float num2 = (float)this.interactor.currency.drunk * 0.4f;
			float num3 = Mathf.PerlinNoise(Time.time * 0.3f, 0f);
			float num4 = Mathf.Sin(Time.time * 0.6f) * (num2 * num3);
			num += num4;
		}
		this.WheelFrontRight.steerAngle = num;
		this.WheelFrontLeft.steerAngle = num;
		this.timer += Time.deltaTime * this.steerWheelRotateFactor;
		this.newAngle = Mathf.Lerp(this.oldAngle, num * 10f + this.steerwheelOffset, this.timer);
		this.steerWheelAngle = this.newAngle;
		this.steerWheel.transform.localRotation = this.rot * Quaternion.Euler(new Vector3(0f, this.steerWheelAngle, 0f));
		this.oldAngle = this.steerWheelAngle;
		if (this.oldAngle == this.newAngle)
		{
			this.timer = 0f;
		}
		this.angleChangeTimer -= Time.deltaTime;
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x0007CEC8 File Offset: 0x0007B0C8
	private void WheelOffset(GameObject model, WheelCollider collider)
	{
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);
		model.transform.position = position;
		model.transform.rotation = rotation;
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x0007CEF8 File Offset: 0x0007B0F8
	private void HandBrake()
	{
		if (Input.GetKey(this.cr.Brake) || (this.cr.Vert == 0f && this.cr.GasInput == 0f && (int)this.speed == 0) || this.cr.BrakeInput > 0.1f)
		{
			this.braked = true;
		}
		else
		{
			this.braked = false;
		}
		if (this.braked)
		{
			this.WheelRearLeft.brakeTorque = this.maxBrakeTorque;
			this.WheelRearRight.brakeTorque = this.maxBrakeTorque;
			this.WheelRearRight.motorTorque = 0f;
			this.WheelRearLeft.motorTorque = 0f;
			return;
		}
		this.WheelRearLeft.brakeTorque = 0f;
		this.WheelRearRight.brakeTorque = 0f;
		this.WheelFrontLeft.brakeTorque = 0f;
		this.WheelFrontRight.brakeTorque = 0f;
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x0007CFF4 File Offset: 0x0007B1F4
	public void AddBattery()
	{
		this.battery.GetComponent<Renderer>().enabled = true;
		this.battery.GetComponent<BoxCollider>().enabled = true;
		this.battery18.SetActive(false);
		this.using18v = false;
		this.maxTorque = 300f;
		this.maxTorqueStatic = 300f;
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x0007D04C File Offset: 0x0007B24C
	public void AddBattery18()
	{
		this.battery18.GetComponent<Renderer>().enabled = true;
		this.battery18.GetComponent<BoxCollider>().enabled = true;
		this.battery.SetActive(false);
		this.using18v = true;
		this.maxTorque = 450f;
		this.maxTorqueStatic = 450f;
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x0007D0A4 File Offset: 0x0007B2A4
	public void RemoveBattery()
	{
		this.battery.SetActive(true);
		this.battery18.SetActive(true);
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x0007D0BE File Offset: 0x0007B2BE
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "water" || other.tag == "DynamicWater")
		{
			this.deepWater = 1;
		}
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x0007D0EB File Offset: 0x0007B2EB
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "water" || other.tag == "DynamicWater")
		{
			this.deepWater = 0;
		}
	}

	// Token: 0x04001852 RID: 6226
	public bool controlled;

	// Token: 0x04001853 RID: 6227
	public bool userControlled;

	// Token: 0x04001854 RID: 6228
	public Transform centerOfMass;

	// Token: 0x04001855 RID: 6229
	public Transform steerWheel;

	// Token: 0x04001856 RID: 6230
	public float steerAngle;

	// Token: 0x04001857 RID: 6231
	public float Cdrag = 0.2f;

	// Token: 0x04001858 RID: 6232
	public float Fdrag;

	// Token: 0x04001859 RID: 6233
	public float maxFuel = 1300f;

	// Token: 0x0400185A RID: 6234
	public float minFuel;

	// Token: 0x0400185B RID: 6235
	public float fuel;

	// Token: 0x0400185C RID: 6236
	public durability batterydura;

	// Token: 0x0400185D RID: 6237
	public durability batterydura18;

	// Token: 0x0400185E RID: 6238
	public float maxTorque;

	// Token: 0x0400185F RID: 6239
	public float maxTorqueStatic;

	// Token: 0x04001860 RID: 6240
	public float maxBrakeTorque = 1000f;

	// Token: 0x04001861 RID: 6241
	public float MaxWheelRotateAngle = 30f;

	// Token: 0x04001862 RID: 6242
	public float lowestSpeedAtSteer = 30f;

	// Token: 0x04001863 RID: 6243
	public float lowSpeedSteerAngle = 15f;

	// Token: 0x04001864 RID: 6244
	public float highSpeedSteerAngle = 1f;

	// Token: 0x04001865 RID: 6245
	public float torque;

	// Token: 0x04001866 RID: 6246
	public float frontTorque;

	// Token: 0x04001867 RID: 6247
	public float rearTorque;

	// Token: 0x04001868 RID: 6248
	public float maxSpeed;

	// Token: 0x04001869 RID: 6249
	public float steerWheelAngle;

	// Token: 0x0400186A RID: 6250
	public float steerWheelRotateFactor = 1f;

	// Token: 0x0400186B RID: 6251
	public bool braked;

	// Token: 0x0400186C RID: 6252
	public WheelCollider WheelFrontRight;

	// Token: 0x0400186D RID: 6253
	public WheelCollider WheelFrontLeft;

	// Token: 0x0400186E RID: 6254
	public WheelCollider WheelRearRight;

	// Token: 0x0400186F RID: 6255
	public WheelCollider WheelRearLeft;

	// Token: 0x04001870 RID: 6256
	public GameObject wheelRay;

	// Token: 0x04001871 RID: 6257
	public float surfaceType;

	// Token: 0x04001872 RID: 6258
	public GameObject wheelFR;

	// Token: 0x04001873 RID: 6259
	public GameObject wheelFL;

	// Token: 0x04001874 RID: 6260
	public GameObject wheelRR;

	// Token: 0x04001875 RID: 6261
	public GameObject wheelRL;

	// Token: 0x04001876 RID: 6262
	public int deepWater;

	// Token: 0x04001877 RID: 6263
	public float speed;

	// Token: 0x04001878 RID: 6264
	private float oldAngle;

	// Token: 0x04001879 RID: 6265
	private float newAngle;

	// Token: 0x0400187A RID: 6266
	private float timer;

	// Token: 0x0400187B RID: 6267
	private float angleChangeTimer;

	// Token: 0x0400187C RID: 6268
	private float steerwheelOffset;

	// Token: 0x0400187D RID: 6269
	public bool canRun;

	// Token: 0x0400187E RID: 6270
	public bool canCrank;

	// Token: 0x0400187F RID: 6271
	public bool canAcc = true;

	// Token: 0x04001880 RID: 6272
	public bool canMove;

	// Token: 0x04001881 RID: 6273
	private int interval = 1;

	// Token: 0x04001882 RID: 6274
	private float nextTime;

	// Token: 0x04001883 RID: 6275
	private int engineCheckInterval = 2;

	// Token: 0x04001884 RID: 6276
	private int i;

	// Token: 0x04001885 RID: 6277
	private Quaternion rot;

	// Token: 0x04001886 RID: 6278
	private float newblend;

	// Token: 0x04001887 RID: 6279
	private bool enableEmission;

	// Token: 0x04001888 RID: 6280
	private bool enableEmissionB;

	// Token: 0x04001889 RID: 6281
	private ParticleSystem.EmissionModule em;

	// Token: 0x0400188A RID: 6282
	private ParticleSystem.EmissionModule emb;

	// Token: 0x0400188B RID: 6283
	public GameObject audioCtrl;

	// Token: 0x0400188C RID: 6284
	public GameObject[] splashFx;

	// Token: 0x0400188D RID: 6285
	public Rigidbody rb;

	// Token: 0x0400188E RID: 6286
	private int deepMud;

	// Token: 0x0400188F RID: 6287
	public GroundDetect groundDetect;

	// Token: 0x04001890 RID: 6288
	private int materialCheckInterval = 1;

	// Token: 0x04001891 RID: 6289
	public GameObject battery;

	// Token: 0x04001892 RID: 6290
	public GameObject battery18;

	// Token: 0x04001893 RID: 6291
	public ControlRef cr;

	// Token: 0x04001894 RID: 6292
	public bool using18v;

	// Token: 0x04001895 RID: 6293
	public Interactor interactor;
}
