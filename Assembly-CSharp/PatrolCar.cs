using System;
using System.Collections;
using AshVP;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x0200010D RID: 269
public class PatrolCar : MonoBehaviour
{
	// Token: 0x06000702 RID: 1794 RVA: 0x0005A9EB File Offset: 0x00058BEB
	private void Start()
	{
		this.ToggleAi(false);
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x0005A9F4 File Offset: 0x00058BF4
	public void ToggleLights(bool tf)
	{
		this.lightbar.SetActive(tf);
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x0005AA02 File Offset: 0x00058C02
	public void ToggleSiren(bool tf)
	{
		if (tf)
		{
			this.siren.Play();
			return;
		}
		this.siren.Stop();
		this.timer = 0f;
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x0005AA29 File Offset: 0x00058C29
	public void ToggleEngineSound(bool tf)
	{
		this.carAi2.engineSounds[0].mute = tf;
		this.carAi2.engineSounds[1].mute = tf;
		this.carAi2.engineSounds[2].mute = tf;
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x0005AA64 File Offset: 0x00058C64
	public void GetOut()
	{
		this.ToggleAi(false);
		if (this.officer.playerCuffed)
		{
			this.officer.OfficerExitCoroutine();
			return;
		}
		this.officer.idleAtStation = true;
		if (this.officer.criminalActivity || this.officer.chasing)
		{
			this.officer.criminalActivity = false;
			this.officer.warrant = true;
			this.officer.chasing = false;
		}
		this.officer.destination = this.officer.idleStation;
		this.officer.OfficerExitCoroutine();
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x0005AAFC File Offset: 0x00058CFC
	public void GetOutGasStation()
	{
		this.carAi2.getOutGas = false;
		this.ToggleAi(false);
		if (this.officer.playerCuffed)
		{
			this.ToggleAi(true);
			return;
		}
		this.officer.idleAtGasStation = true;
		if (this.officer.criminalActivity || this.officer.chasing)
		{
			this.officer.criminalActivity = false;
			this.officer.warrant = true;
			this.officer.chasing = false;
		}
		this.officer.destination = this.officer.idleGas;
		this.officer.OfficerExitCoroutine();
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x0005AB9C File Offset: 0x00058D9C
	public void ResumeOrRecalc()
	{
		if (this.wpt.target == null)
		{
			this.ResumeBeat();
			return;
		}
		if (Vector3.Distance(base.transform.position, this.wpt.target.position) < 15f)
		{
			this.ResumeBeat();
			Debug.Log("beat resumed");
			return;
		}
		float num = Vector3.Distance(base.transform.position, this.wpt.target.position);
		Debug.Log("finding nearest wp " + num);
		this.NearestBeat();
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x0005AC38 File Offset: 0x00058E38
	public void NearestBeat()
	{
		this.Pursue(false);
		this.closestWaypoint = null;
		int circuitTemp = -1;
		int progressNumTemp = -1;
		float num = float.MaxValue;
		for (int i = 0; i < this.waypointContainers.Length; i++)
		{
			Transform transform = this.waypointContainers[i];
			for (int j = 0; j < transform.childCount; j++)
			{
				this.wp = transform.GetChild(j);
				if (!(this.wp.name == "station") && !(this.wp.name == "gasstation"))
				{
					float num2 = Vector3.Distance(this.lightbar.transform.position, this.wp.position);
					Vector3 normalized = (this.stationTransform.position - this.wp.position).normalized;
					if (Vector3.Dot(this.wp.forward, normalized) > 0.5f)
					{
						num2 -= 20f;
					}
					else
					{
						num2 += 5f;
					}
					if (num2 < num)
					{
						num = num2;
						this.closestWaypoint = this.wp;
						circuitTemp = i;
						progressNumTemp = j;
					}
				}
			}
		}
		this.wpt.circuitTemp = circuitTemp;
		this.wpt.progressNumTemp = progressNumTemp;
		this.wpt.Recalibrate();
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x0005AD92 File Offset: 0x00058F92
	public void ResumeBeat()
	{
		this.Pursue(false);
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x0005AD9C File Offset: 0x00058F9C
	public void Pursue(bool tf)
	{
		if (!tf)
		{
			Debug.Log("stoppursue");
			this.wpt.enabled = true;
			this.pursuing = false;
			this.carAi2.turnTorque = 600f;
			this.carAi2.accelerationForce = 600f;
			this.carAi2.TargetTransform = this.wpt.target;
			return;
		}
		this.pursuing = true;
		this.carAi2.turnTorque = 800f;
		this.carAi2.accelerationForce = 800f;
		this.wpt.enabled = false;
		this.carAi2.TargetTransform = this.player.transform;
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x0005AE49 File Offset: 0x00059049
	public void ToggleAi(bool tf)
	{
		if (!tf)
		{
			this.carAi2.stop_Vehicle();
			return;
		}
		this.carAi2.start_Vehicle();
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x0005AE65 File Offset: 0x00059065
	private void FlipInstantly()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.forward, Vector3.up);
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x0005AE87 File Offset: 0x00059087
	private IEnumerator SmoothFlipUpright()
	{
		Quaternion targetRotation = Quaternion.LookRotation(base.transform.forward, Vector3.up);
		while (Quaternion.Angle(base.transform.rotation, targetRotation) > 0.1f)
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, targetRotation, Time.deltaTime * this.smoothFlipSpeed);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x0005AE98 File Offset: 0x00059098
	private void Update()
	{
		if (this.pursuing && !this.siren.isPlaying)
		{
			this.timer += Time.deltaTime;
			if (this.timer >= 10f)
			{
				this.ToggleSiren(true);
			}
		}
		float num = Vector3.Dot(base.transform.up, Vector3.up);
		this.isUpsideDown = (num < 0f);
		if (this.isUpsideDown)
		{
			this.upsideDownTimer += Time.deltaTime;
			if (this.upsideDownTimer >= this.flipDelay)
			{
				if (this.useSmoothFlip)
				{
					base.StartCoroutine(this.SmoothFlipUpright());
				}
				else
				{
					this.FlipInstantly();
				}
				this.upsideDownTimer = 0f;
				return;
			}
		}
		else
		{
			this.upsideDownTimer = 0f;
		}
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x0005AF60 File Offset: 0x00059160
	private IEnumerator RunOverPlayer()
	{
		this.playerCap.enabled = true;
		if (this.playerRb == null)
		{
			this.playerRb = this.fpst.gameObject.GetComponent<Rigidbody>();
		}
		this.playerRb.isKinematic = false;
		this.cc.enabled = false;
		this.fpsc.enabled = false;
		float d = 10f;
		Vector3 onUnitSphere = Random.onUnitSphere;
		if (onUnitSphere.y < 0f)
		{
			onUnitSphere.y = -onUnitSphere.y;
		}
		this.playerRb.AddForce(onUnitSphere.normalized * d, ForceMode.Impulse);
		this.officer.VehAudio("bodyimpact");
		yield return new WaitForSeconds(2f);
		this.RecordHit();
		yield return new WaitForSeconds(2f);
		this.fpst.position += Vector3.up * 0.4f;
		this.playerCap.enabled = false;
		this.playerRb.isKinematic = true;
		this.cc.enabled = true;
		this.fpsc.enabled = true;
		this.runOverCoroutine = null;
		yield break;
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x0005AF70 File Offset: 0x00059170
	private void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Player") && col.gameObject.name == "FPSController" && this.fpst.parent == null && !this.officer.playerCuffed && this.officer.driving && this.runOverCoroutine == null)
		{
			this.runOverCoroutine = base.StartCoroutine(this.RunOverPlayer());
		}
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x0005AFF0 File Offset: 0x000591F0
	public void RecordHit()
	{
		float time = Time.time;
		if (this.firstHitTime < 0f || time - this.firstHitTime > this.timeWindow)
		{
			this.firstHitTime = time;
			this.hitCount = 1;
			return;
		}
		this.hitCount++;
		if (this.hitCount == 2)
		{
			this.officer.charge_obstruction++;
			this.officer.criminalActivity = true;
			this.officer.chasing = true;
			this.hitCount = 0;
		}
	}

	// Token: 0x04000FD1 RID: 4049
	public AiCarContrtoller carAi2;

	// Token: 0x04000FD2 RID: 4050
	public AudioSource siren;

	// Token: 0x04000FD3 RID: 4051
	public GameObject lightbar;

	// Token: 0x04000FD4 RID: 4052
	public GameObject player;

	// Token: 0x04000FD5 RID: 4053
	public bool pursuing;

	// Token: 0x04000FD6 RID: 4054
	public Officer officer;

	// Token: 0x04000FD7 RID: 4055
	private float speed;

	// Token: 0x04000FD8 RID: 4056
	private float timer;

	// Token: 0x04000FD9 RID: 4057
	private Transform closestWaypoint;

	// Token: 0x04000FDA RID: 4058
	public Transform[] waypointContainers;

	// Token: 0x04000FDB RID: 4059
	private CarAIWaipointTracker cwpt;

	// Token: 0x04000FDC RID: 4060
	public WaypointProgressTracker wpt;

	// Token: 0x04000FDD RID: 4061
	private Transform wp;

	// Token: 0x04000FDE RID: 4062
	private bool isUpsideDown;

	// Token: 0x04000FDF RID: 4063
	public float flipDelay = 6f;

	// Token: 0x04000FE0 RID: 4064
	public float smoothFlipSpeed = 2f;

	// Token: 0x04000FE1 RID: 4065
	public bool useSmoothFlip = true;

	// Token: 0x04000FE2 RID: 4066
	private float upsideDownTimer;

	// Token: 0x04000FE3 RID: 4067
	public Transform stationTransform;

	// Token: 0x04000FE4 RID: 4068
	public CharacterController cc;

	// Token: 0x04000FE5 RID: 4069
	public Rigidbody playerRb;

	// Token: 0x04000FE6 RID: 4070
	public CapsuleCollider playerCap;

	// Token: 0x04000FE7 RID: 4071
	public FirstPersonController fpsc;

	// Token: 0x04000FE8 RID: 4072
	private Coroutine runOverCoroutine;

	// Token: 0x04000FE9 RID: 4073
	public Transform fpst;

	// Token: 0x04000FEA RID: 4074
	private float firstHitTime = -1f;

	// Token: 0x04000FEB RID: 4075
	private int hitCount;

	// Token: 0x04000FEC RID: 4076
	public float timeWindow = 60f;
}
