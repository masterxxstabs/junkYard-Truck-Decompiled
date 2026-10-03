using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class SpotLightController : MonoBehaviour
{
	// Token: 0x0600082D RID: 2093 RVA: 0x0006C9F4 File Offset: 0x0006ABF4
	private void Start()
	{
		this.currentRotationY = base.transform.localEulerAngles.y;
		this.rotationTime = Time.time + Random.Range(0.5f, 4f);
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0006CA28 File Offset: 0x0006AC28
	private void Update()
	{
		if (this.player != null && Vector3.Distance(base.transform.position, this.player.position) <= this.detectionRange)
		{
			this.playerDetected = true;
		}
		else
		{
			this.playerDetected = false;
		}
		if (this.playerDetected)
		{
			this.TrackPlayer();
		}
		else if (!this.isPaused)
		{
			this.ScanArea();
		}
		float num = Vector3.Distance(base.transform.position, this.cam.position);
		Vector3 normalized = (this.cam.position - base.transform.position).normalized;
		if (Vector3.Dot(base.transform.forward, normalized) > this.visibilityThreshhold && num < 40f)
		{
			this.lensFlare.enabled = true;
			return;
		}
		this.lensFlare.enabled = false;
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0006CB10 File Offset: 0x0006AD10
	private void TrackPlayer()
	{
		float y = Mathf.Clamp(Quaternion.LookRotation(this.player.position - base.transform.position).eulerAngles.y, this.minRotation, this.maxRotation);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(0f, y, 0f), this.rotationSpeed * Time.deltaTime);
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0006CB94 File Offset: 0x0006AD94
	private void ScanArea()
	{
		this.currentRotationY += this.rotationSpeed * this.currentRotationDirection * Time.deltaTime;
		if (Time.time > this.rotationTime)
		{
			base.StartCoroutine(this.PauseRotation());
			this.currentRotationDirection = (float)(Random.Range(0, 2) * 2 - 1);
		}
		else if (this.currentRotationY > this.maxRotation || Time.time > this.rotationTime)
		{
			this.currentRotationY = this.maxRotation;
			base.StartCoroutine(this.PauseRotation());
			this.currentRotationDirection = -1f;
		}
		else if (this.currentRotationY < this.minRotation || Time.time > this.rotationTime)
		{
			this.currentRotationY = this.minRotation;
			base.StartCoroutine(this.PauseRotation());
			this.currentRotationDirection = 1f;
		}
		base.transform.localRotation = Quaternion.Euler(0f, this.currentRotationY, 0f);
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0006CC92 File Offset: 0x0006AE92
	private IEnumerator PauseRotation()
	{
		this.isPaused = true;
		this.rotationTime = Time.time + Random.Range(0.5f, 4f);
		yield return new WaitForSeconds(this.pauseDuration);
		this.isPaused = false;
		yield break;
	}

	// Token: 0x0400130E RID: 4878
	public Transform player;

	// Token: 0x0400130F RID: 4879
	public float detectionRange = 10f;

	// Token: 0x04001310 RID: 4880
	public float rotationSpeed = 30f;

	// Token: 0x04001311 RID: 4881
	public float pauseDuration = 2f;

	// Token: 0x04001312 RID: 4882
	public float maxRotation = 90f;

	// Token: 0x04001313 RID: 4883
	public float minRotation = -90f;

	// Token: 0x04001314 RID: 4884
	private bool playerDetected;

	// Token: 0x04001315 RID: 4885
	private bool isPaused;

	// Token: 0x04001316 RID: 4886
	private float currentRotationDirection = 1f;

	// Token: 0x04001317 RID: 4887
	private float currentRotationY;

	// Token: 0x04001318 RID: 4888
	private float rotationTime;

	// Token: 0x04001319 RID: 4889
	public LensFlare lensFlare;

	// Token: 0x0400131A RID: 4890
	public Transform cam;

	// Token: 0x0400131B RID: 4891
	public float visibilityThreshhold = 0.2f;
}
