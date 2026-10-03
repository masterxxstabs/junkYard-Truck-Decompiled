using System;
using UnityEngine;

namespace CalmWater
{
	// Token: 0x0200032F RID: 815
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class CameraDepthMode : MonoBehaviour
	{
		// Token: 0x060014D3 RID: 5331 RVA: 0x000DCE9F File Offset: 0x000DB09F
		private void Start()
		{
			base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x000DCE9F File Offset: 0x000DB09F
		private void OnEnable()
		{
			base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x000DCEAD File Offset: 0x000DB0AD
		private void OnDisable()
		{
			base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
		}
	}
}
