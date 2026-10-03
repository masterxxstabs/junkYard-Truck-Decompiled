using System;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class Winch : MonoBehaviour
{
	// Token: 0x060008C8 RID: 2248 RVA: 0x00071184 File Offset: 0x0006F384
	private void Start()
	{
		this.lineRenderer = base.gameObject.GetComponent<LineRenderer>();
		this.rb = base.transform.gameObject.GetComponent<Rigidbody>();
		this.aSource = base.transform.gameObject.GetComponents<AudioSource>();
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x000711C4 File Offset: 0x0006F3C4
	private void Update()
	{
		if (Vector3.Distance(this.fps.position, base.transform.position) > this.maxPlayerDistance)
		{
			this.DetachHook();
		}
		RaycastHit raycastHit;
		if (Input.GetKeyDown(this.cr.Interact) && Physics.Raycast(this.currentCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, 3f))
		{
			if (raycastHit.transform.tag == "terrain")
			{
				Vector3 point = raycastHit.point;
				this.terrain = raycastHit.collider.gameObject.GetComponent<Terrain>();
				float num = this.terrain.SampleHeight(raycastHit.point);
				if (raycastHit.point.y - 0.2f > num)
				{
					this.anchor.position = raycastHit.point;
					this.lineRenderer.enabled = true;
					this.anchorAttached = true;
					this.lastDistance = Vector3.Distance(this.anchor.position, base.transform.position);
					this.fpsHook.GetComponent<Renderer>().enabled = false;
					this.aSource[2].Play();
				}
			}
			if (raycastHit.collider.transform.name == "vette" || raycastHit.collider.transform.name == "tree_stump")
			{
				Vector3 point2 = raycastHit.point;
				this.towObject = raycastHit.collider.transform.gameObject;
				this.rb2 = this.towObject.GetComponent<Rigidbody>();
				this.anchor.position = raycastHit.collider.transform.position;
				this.lineRenderer.enabled = true;
				this.anchorAttached = true;
				this.lastDistance = Vector3.Distance(this.anchor.position, base.transform.position);
				this.fpsHook.GetComponent<Renderer>().enabled = false;
				this.aSource[2].Play();
				if (raycastHit.collider.transform.name == "tree_stump" && raycastHit.collider.gameObject.GetComponent<ConfigurableJoint>() != null)
				{
					raycastHit.collider.gameObject.GetComponent<ConfigurableJoint>().breakForce = 200000f;
					raycastHit.collider.gameObject.GetComponent<ConfigurableJoint>().breakForce = 200000f;
				}
			}
			if (raycastHit.collider.transform.name == "towhookFF" || raycastHit.collider.transform.name == "towhookFR")
			{
				Vector3 point3 = raycastHit.point;
				if (this.truckNum != 2)
				{
					this.rb2 = this.truck2.GetComponent<Rigidbody>();
					this.towObject = raycastHit.collider.transform.gameObject;
					this.anchor.position = raycastHit.collider.transform.position;
					this.lineRenderer.enabled = true;
					this.anchorAttached = true;
					this.lastDistance = Vector3.Distance(this.anchor.position, base.transform.position);
					this.fpsHook.GetComponent<Renderer>().enabled = false;
					this.rb2.mass = 500f;
					foreach (WheelCollider wheelCollider in this.truck2WCs.GetComponentsInChildren<WheelCollider>())
					{
						wheelCollider.motorTorque = 25f;
						wheelCollider.brakeTorque = 0f;
					}
					return;
				}
			}
			else if (raycastHit.transform.name == "towhookF" || raycastHit.transform.name == "towhookR")
			{
				Vector3 point4 = raycastHit.point;
				if (this.truckNum != 1)
				{
					this.rb2 = this.truck.GetComponent<Rigidbody>();
					this.towObject = this.truck;
					this.anchor.position = raycastHit.collider.transform.position;
					this.lineRenderer.enabled = true;
					this.anchorAttached = true;
					this.lastDistance = Vector3.Distance(this.anchor.position, base.transform.position);
					this.fpsHook.GetComponent<Renderer>().enabled = false;
				}
			}
		}
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x0007161C File Offset: 0x0006F81C
	private void FixedUpdate()
	{
		this.v3Velocity = this.rb.velocity.magnitude;
		if (this.anchorAttached)
		{
			this.lineRenderer.SetPosition(0, this.origin.position);
			this.lineRenderer.SetPosition(1, this.anchor.position);
			if (this.towObject != null)
			{
				this.anchor.position = this.towObject.transform.position;
			}
			Vector3 normalized = (base.transform.position - this.anchor.position).normalized;
			if (Input.GetKey(this.cr.WinchPull) && this.rb.velocity.magnitude < (float)this.liftSpeed)
			{
				this.rb.AddForce(normalized * (float)this.winchForce * -1f);
				this.lastDistance = Vector3.Distance(base.transform.position, this.anchor.position);
				if (this.rb2 != null)
				{
					this.rb2.AddForce(normalized * (float)this.winchForce);
				}
				if (!this.aSource[0].isPlaying)
				{
					this.aSource[0].Play();
				}
			}
			else if (this.lastDistance < Vector3.Distance(this.anchor.position, base.transform.position) && this.rb.velocity.y < 0f)
			{
				this.rb.AddForce(normalized * (float)this.winchForce * -1f);
				this.lastDistance = Vector3.Distance(base.transform.position, this.anchor.position);
			}
			if (Input.GetKeyDown(this.cr.WinchRelease))
			{
				this.DetachHook();
			}
		}
		this.aSource[0].volume = this.v3Velocity;
		if (Input.GetKeyDown(this.cr.WinchRelease) && this.fpsHook.GetComponent<Renderer>().enabled)
		{
			this.DetachHook();
		}
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x00071858 File Offset: 0x0006FA58
	public void DetachHook()
	{
		this.anchorAttached = false;
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = false;
		}
		this.fpsHook.GetComponent<Renderer>().enabled = false;
		if (this.winchBody.GetComponent<Renderer>().enabled)
		{
			if (this.aSource == null)
			{
				this.aSource = base.transform.gameObject.GetComponents<AudioSource>();
			}
			this.aSource[1].Play();
			this.winchHook.GetComponent<Renderer>().enabled = true;
		}
		this.rb2 = null;
		if (this.towObject != null)
		{
			WheelCollider[] componentsInChildren = this.towObject.GetComponentsInChildren<WheelCollider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].brakeTorque = 1000f;
			}
			this.towObject = null;
		}
		base.enabled = false;
	}

	// Token: 0x04001431 RID: 5169
	public car carscript;

	// Token: 0x04001432 RID: 5170
	public car4 carscriptF;

	// Token: 0x04001433 RID: 5171
	[SerializeField]
	public Camera currentCamera;

	// Token: 0x04001434 RID: 5172
	[SerializeField]
	public int winchForce = 10000;

	// Token: 0x04001435 RID: 5173
	[SerializeField]
	private int liftSpeed = 4;

	// Token: 0x04001436 RID: 5174
	[SerializeField]
	private float lowerSpeed = 4f;

	// Token: 0x04001437 RID: 5175
	private Terrain terrain;

	// Token: 0x04001438 RID: 5176
	public Transform fps;

	// Token: 0x04001439 RID: 5177
	public Transform anchor;

	// Token: 0x0400143A RID: 5178
	private LineRenderer lineRenderer;

	// Token: 0x0400143B RID: 5179
	private float lastDistance;

	// Token: 0x0400143C RID: 5180
	private float maxDistance = 20f;

	// Token: 0x0400143D RID: 5181
	private float maxPlayerDistance = 19.5f;

	// Token: 0x0400143E RID: 5182
	public Rigidbody rb;

	// Token: 0x0400143F RID: 5183
	public Rigidbody rb2;

	// Token: 0x04001440 RID: 5184
	private GameObject towObject;

	// Token: 0x04001441 RID: 5185
	private AudioSource[] aSource;

	// Token: 0x04001442 RID: 5186
	private bool playing;

	// Token: 0x04001443 RID: 5187
	public float v3Velocity;

	// Token: 0x04001444 RID: 5188
	public Transform origin;

	// Token: 0x04001445 RID: 5189
	public GameObject fpsHook;

	// Token: 0x04001446 RID: 5190
	public GameObject winchHook;

	// Token: 0x04001447 RID: 5191
	public bool anchorAttached;

	// Token: 0x04001448 RID: 5192
	public GameObject winchBody;

	// Token: 0x04001449 RID: 5193
	public ControlRef cr;

	// Token: 0x0400144A RID: 5194
	public int truckNum;

	// Token: 0x0400144B RID: 5195
	public GameObject truck;

	// Token: 0x0400144C RID: 5196
	public GameObject truck2;

	// Token: 0x0400144D RID: 5197
	public GameObject truck1WCs;

	// Token: 0x0400144E RID: 5198
	public GameObject truck2WCs;
}
