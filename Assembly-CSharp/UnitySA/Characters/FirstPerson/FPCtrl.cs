using System;
using UnityEngine;
using UnitySA.Utility;

namespace UnitySA.Characters.FirstPerson
{
	// Token: 0x020001A9 RID: 425
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(AudioSource))]
	public class FPCtrl : MonoBehaviour
	{
		// Token: 0x06000A79 RID: 2681 RVA: 0x0008C8C4 File Offset: 0x0008AAC4
		private void Start()
		{
			this.m_CharacterController = base.GetComponent<CharacterController>();
			this.m_Camera = Camera.main;
			this.m_OriginalCameraPosition = this.m_Camera.transform.localPosition;
			this.m_FovKick.Setup(this.m_Camera);
			this.m_HeadBob.Setup(this.m_Camera, this.m_StepInterval);
			this.m_StepCycle = 0f;
			this.m_NextStep = this.m_StepCycle / 2f;
			this.m_Jumping = false;
			this.m_MouseLook.Init(base.transform, this.m_Camera.transform);
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x0008C968 File Offset: 0x0008AB68
		private void Update()
		{
			this.RotateView();
			if (!this.m_Jump)
			{
				this.m_Jump = Input.GetButtonDown("Jump");
			}
			if (!this.m_PreviouslyGrounded && this.m_CharacterController.isGrounded)
			{
				base.StartCoroutine(this.m_JumpBob.DoBobCycle());
				this.m_MoveDir.y = 0f;
				this.m_Jumping = false;
			}
			if (!this.m_CharacterController.isGrounded && !this.m_Jumping && this.m_PreviouslyGrounded)
			{
				this.m_MoveDir.y = 0f;
			}
			this.m_PreviouslyGrounded = this.m_CharacterController.isGrounded;
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x0008CA10 File Offset: 0x0008AC10
		private void FixedUpdate()
		{
			float num;
			this.GetInput(out num);
			Vector3 vector = base.transform.forward * this.m_Input.y + base.transform.right * this.m_Input.x;
			RaycastHit raycastHit;
			Physics.SphereCast(base.transform.position, this.m_CharacterController.radius, Vector3.down, out raycastHit, this.m_CharacterController.height / 2f, -1, QueryTriggerInteraction.Ignore);
			vector = Vector3.ProjectOnPlane(vector, raycastHit.normal).normalized;
			this.m_MoveDir.x = vector.x * num;
			this.m_MoveDir.z = vector.z * num;
			if (this.m_CharacterController.isGrounded)
			{
				this.m_MoveDir.y = -this.m_StickToGroundForce;
				if (this.m_Jump)
				{
					this.m_MoveDir.y = this.m_JumpSpeed;
					this.m_Jump = false;
					this.m_Jumping = true;
				}
			}
			else
			{
				this.m_MoveDir += Physics.gravity * this.m_GravityMultiplier * Time.fixedDeltaTime;
			}
			this.m_CollisionFlags = this.m_CharacterController.Move(this.m_MoveDir * Time.fixedDeltaTime);
			this.ProgressStepCycle(num);
			this.UpdateCameraPosition(num);
			this.m_MouseLook.UpdateCursorLock();
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x0008CB80 File Offset: 0x0008AD80
		private void ProgressStepCycle(float speed)
		{
			if (this.m_CharacterController.velocity.sqrMagnitude > 0f && (this.m_Input.x != 0f || this.m_Input.y != 0f))
			{
				this.m_StepCycle += (this.m_CharacterController.velocity.magnitude + speed * (this.m_IsWalking ? 1f : this.m_RunstepLenghten)) * Time.fixedDeltaTime;
			}
			if (this.m_StepCycle <= this.m_NextStep)
			{
				return;
			}
			this.m_NextStep = this.m_StepCycle + this.m_StepInterval;
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x0008CC2C File Offset: 0x0008AE2C
		private void UpdateCameraPosition(float speed)
		{
			if (!this.m_UseHeadBob)
			{
				return;
			}
			Vector3 localPosition;
			if (this.m_CharacterController.velocity.magnitude > 0f && this.m_CharacterController.isGrounded)
			{
				this.m_Camera.transform.localPosition = this.m_HeadBob.DoHeadBob(this.m_CharacterController.velocity.magnitude + speed * (this.m_IsWalking ? 1f : this.m_RunstepLenghten));
				localPosition = this.m_Camera.transform.localPosition;
				localPosition.y = this.m_Camera.transform.localPosition.y - this.m_JumpBob.Offset();
			}
			else
			{
				localPosition = this.m_Camera.transform.localPosition;
				localPosition.y = this.m_OriginalCameraPosition.y - this.m_JumpBob.Offset();
			}
			this.m_Camera.transform.localPosition = localPosition;
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x0008CD30 File Offset: 0x0008AF30
		private void GetInput(out float speed)
		{
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			bool isWalking = this.m_IsWalking;
			this.m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
			speed = (this.m_IsWalking ? this.m_WalkSpeed : this.m_RunSpeed);
			this.m_Input = new Vector2(axis, axis2);
			if (this.m_Input.sqrMagnitude > 1f)
			{
				this.m_Input.Normalize();
			}
			if (this.m_IsWalking != isWalking && this.m_UseFovKick && this.m_CharacterController.velocity.sqrMagnitude > 0f)
			{
				base.StopAllCoroutines();
				base.StartCoroutine((!this.m_IsWalking) ? this.m_FovKick.FOVKickUp() : this.m_FovKick.FOVKickDown());
			}
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x0008CE07 File Offset: 0x0008B007
		private void RotateView()
		{
			this.m_MouseLook.LookRotation(base.transform, this.m_Camera.transform);
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x0008CE28 File Offset: 0x0008B028
		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
			if (this.m_CollisionFlags == CollisionFlags.Below)
			{
				return;
			}
			if (attachedRigidbody == null || attachedRigidbody.isKinematic)
			{
				return;
			}
			attachedRigidbody.AddForceAtPosition(this.m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
		}

		// Token: 0x04001C70 RID: 7280
		[SerializeField]
		private bool m_IsWalking;

		// Token: 0x04001C71 RID: 7281
		[SerializeField]
		private float m_WalkSpeed;

		// Token: 0x04001C72 RID: 7282
		[SerializeField]
		private float m_RunSpeed;

		// Token: 0x04001C73 RID: 7283
		[SerializeField]
		[Range(0f, 1f)]
		private float m_RunstepLenghten;

		// Token: 0x04001C74 RID: 7284
		[SerializeField]
		private float m_JumpSpeed;

		// Token: 0x04001C75 RID: 7285
		[SerializeField]
		private float m_StickToGroundForce;

		// Token: 0x04001C76 RID: 7286
		[SerializeField]
		private float m_GravityMultiplier;

		// Token: 0x04001C77 RID: 7287
		[SerializeField]
		private MLook m_MouseLook;

		// Token: 0x04001C78 RID: 7288
		[SerializeField]
		private bool m_UseFovKick;

		// Token: 0x04001C79 RID: 7289
		[SerializeField]
		private FOVZoom m_FovKick = new FOVZoom();

		// Token: 0x04001C7A RID: 7290
		[SerializeField]
		private bool m_UseHeadBob;

		// Token: 0x04001C7B RID: 7291
		[SerializeField]
		private CurveCtrlBob m_HeadBob = new CurveCtrlBob();

		// Token: 0x04001C7C RID: 7292
		[SerializeField]
		private LerpCtrlBob m_JumpBob = new LerpCtrlBob();

		// Token: 0x04001C7D RID: 7293
		[SerializeField]
		private float m_StepInterval;

		// Token: 0x04001C7E RID: 7294
		private Camera m_Camera;

		// Token: 0x04001C7F RID: 7295
		private bool m_Jump;

		// Token: 0x04001C80 RID: 7296
		private float m_YRotation;

		// Token: 0x04001C81 RID: 7297
		private Vector2 m_Input;

		// Token: 0x04001C82 RID: 7298
		private Vector3 m_MoveDir = Vector3.zero;

		// Token: 0x04001C83 RID: 7299
		private CharacterController m_CharacterController;

		// Token: 0x04001C84 RID: 7300
		private CollisionFlags m_CollisionFlags;

		// Token: 0x04001C85 RID: 7301
		private bool m_PreviouslyGrounded;

		// Token: 0x04001C86 RID: 7302
		private Vector3 m_OriginalCameraPosition;

		// Token: 0x04001C87 RID: 7303
		private float m_StepCycle;

		// Token: 0x04001C88 RID: 7304
		private float m_NextStep;

		// Token: 0x04001C89 RID: 7305
		private bool m_Jumping;
	}
}
