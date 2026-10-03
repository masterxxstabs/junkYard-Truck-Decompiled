using System;
using UnityEngine;

// Token: 0x0200011B RID: 283
public class ExampleWheelController : MonoBehaviour
{
	// Token: 0x06000788 RID: 1928 RVA: 0x0006224A File Offset: 0x0006044A
	private void Start()
	{
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
		this.m_Rigidbody.maxAngularVelocity = 100f;
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x00062268 File Offset: 0x00060468
	private void Update()
	{
		if (Input.GetKey(KeyCode.UpArrow))
		{
			this.m_Rigidbody.AddRelativeTorque(new Vector3(-1f * this.acceleration, 0f, 0f), ForceMode.Acceleration);
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			this.m_Rigidbody.AddRelativeTorque(new Vector3(1f * this.acceleration, 0f, 0f), ForceMode.Acceleration);
		}
		float value = -this.m_Rigidbody.angularVelocity.x / 100f;
		if (this.motionVectorRenderer)
		{
			this.motionVectorRenderer.material.SetFloat(ExampleWheelController.Uniforms._MotionAmount, Mathf.Clamp(value, -0.25f, 0.25f));
		}
	}

	// Token: 0x04001139 RID: 4409
	public float acceleration;

	// Token: 0x0400113A RID: 4410
	public Renderer motionVectorRenderer;

	// Token: 0x0400113B RID: 4411
	private Rigidbody m_Rigidbody;

	// Token: 0x02000411 RID: 1041
	private static class Uniforms
	{
		// Token: 0x04002911 RID: 10513
		internal static readonly int _MotionAmount = Shader.PropertyToID("_MotionAmount");
	}
}
