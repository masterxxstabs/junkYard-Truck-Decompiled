using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000136 RID: 310
public class Winch2 : MonoBehaviour
{
	// Token: 0x0600080A RID: 2058 RVA: 0x0006BEF5 File Offset: 0x0006A0F5
	private void Start()
	{
		this.springJoint = this.whatTheRopeIsConnectedTo.GetComponent<SpringJoint>();
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.UpdateSpring();
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x0006BF1A File Offset: 0x0006A11A
	private void FixedUpdate()
	{
		this.UpdateWinch();
		this.DisplayRope();
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0006BF28 File Offset: 0x0006A128
	private void UpdateSpring()
	{
		float num = 7750f;
		float num2 = 0.01f;
		float num3 = (3.1415927f * num2 * num2 * this.ropeLength * num + this.loadMass) * 9.81f / 0.01f;
		this.springJoint.spring = num3 * 100f;
		this.springJoint.damper = num3 * 15f;
		this.springJoint.maxDistance = this.ropeLength;
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x0006BF9C File Offset: 0x0006A19C
	private void DisplayRope()
	{
		float num = 0.02f;
		this.lineRenderer.startWidth = num;
		this.lineRenderer.endWidth = num;
		Vector3 position = this.whatTheRopeIsConnectedTo.position;
		Vector3 position2 = this.whatIsHangingFromTheRope.position;
		Vector3 b = position + this.whatTheRopeIsConnectedTo.up * (-(position - position2).magnitude * 0.1f);
		Vector3 c = position2 + this.whatIsHangingFromTheRope.up * ((position - position2).magnitude * 0.5f);
		BezierCurve.GetBezierCurve(position, b, c, position2, this.allRopeSections);
		Vector3[] array = new Vector3[this.allRopeSections.Count];
		for (int i = 0; i < this.allRopeSections.Count; i++)
		{
			array[i] = this.allRopeSections[i];
		}
		this.lineRenderer.positionCount = array.Length;
		this.lineRenderer.SetPositions(array);
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x0006C0AC File Offset: 0x0006A2AC
	private void UpdateWinch()
	{
		bool flag = false;
		if (Input.GetKey(KeyCode.O) && this.ropeLength < this.maxRopeLength)
		{
			this.ropeLength += this.winchSpeed * Time.deltaTime;
			flag = true;
		}
		else if (Input.GetKey(KeyCode.I) && this.ropeLength > this.minRopeLength)
		{
			this.ropeLength -= this.winchSpeed * Time.deltaTime;
			flag = true;
		}
		if (flag)
		{
			this.ropeLength = Mathf.Clamp(this.ropeLength, this.minRopeLength, this.maxRopeLength);
			this.UpdateSpring();
		}
	}

	// Token: 0x040012D5 RID: 4821
	public Transform whatTheRopeIsConnectedTo;

	// Token: 0x040012D6 RID: 4822
	public Transform whatIsHangingFromTheRope;

	// Token: 0x040012D7 RID: 4823
	private LineRenderer lineRenderer;

	// Token: 0x040012D8 RID: 4824
	public List<Vector3> allRopeSections = new List<Vector3>();

	// Token: 0x040012D9 RID: 4825
	private float ropeLength = 1f;

	// Token: 0x040012DA RID: 4826
	private float minRopeLength = 1f;

	// Token: 0x040012DB RID: 4827
	private float maxRopeLength = 2000f;

	// Token: 0x040012DC RID: 4828
	private float loadMass = 2500f;

	// Token: 0x040012DD RID: 4829
	private float winchSpeed = 13f;

	// Token: 0x040012DE RID: 4830
	private SpringJoint springJoint;
}
