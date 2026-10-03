using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class DriverDoorOpen : MonoBehaviour
{
	// Token: 0x06000194 RID: 404 RVA: 0x000115A8 File Offset: 0x0000F7A8
	private void Start()
	{
		this._animator = base.GetComponent<Animator>();
	}

	// Token: 0x06000195 RID: 405 RVA: 0x000115B6 File Offset: 0x0000F7B6
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			this._animator.SetBool("open", true);
		}
	}

	// Token: 0x04000484 RID: 1156
	private Animator _animator;
}
