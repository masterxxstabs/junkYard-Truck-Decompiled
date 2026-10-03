using System;
using System.Collections.Generic;
using TSD.uTireRuntime;
using UnityEngine;

namespace TSD.uTireExamples
{
	// Token: 0x02000340 RID: 832
	public class uTireShaderChanger : MonoBehaviour
	{
		// Token: 0x06001561 RID: 5473 RVA: 0x000E001A File Offset: 0x000DE21A
		private void Start()
		{
			this.rt = Shader.Find(this.runtimeShader);
			this.ds = Shader.Find(this.debugShader);
			this.changeShaders(this.changetoRuntime);
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x000E004C File Offset: 0x000DE24C
		private void changeShaders(bool newState)
		{
			List<Renderer> list = new List<Renderer>();
			foreach (Vehicle vehicle in Singleton<uTireManager>.Instance.vehicles)
			{
				foreach (WheelMeshConnection wheelMeshConnection in vehicle.wheels)
				{
					if (wheelMeshConnection.meshRenderer == null)
					{
						if (this.showDebugMessages)
						{
							Debug.Log("wheelMesh is null, make sure to drag it in the inspector");
						}
					}
					else
					{
						list.Add(wheelMeshConnection.meshRenderer);
					}
				}
			}
			Shader shader = Shader.Find(newState ? this.runtimeShader : this.debugShader);
			List<Material> list2 = new List<Material>();
			foreach (Renderer renderer in list)
			{
				foreach (Material material in renderer.sharedMaterials)
				{
					if (!(material == null) && !list2.Contains(material) && (material.shader == this.rt || material.shader == this.ds))
					{
						list2.Add(material);
					}
				}
			}
			foreach (Material material2 in list2)
			{
				material2.shader = shader;
			}
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x000E01E8 File Offset: 0x000DE3E8
		private void OnApplicationQuit()
		{
			if (!this.resetOnQuit)
			{
				return;
			}
			this.changeShaders(!this.changetoRuntime);
		}

		// Token: 0x040025FD RID: 9725
		[Tooltip("If true every material using the deformation shader will switch to the runtime variant on start")]
		public bool changetoRuntime = true;

		// Token: 0x040025FE RID: 9726
		[Tooltip("If ON tessellation shader variant will be used. Warning, Tessellation won't work with GPU Instancing!")]
		public bool tessellation;

		// Token: 0x040025FF RID: 9727
		[Tooltip("If true materials will switch back on quit")]
		public bool resetOnQuit = true;

		// Token: 0x04002600 RID: 9728
		public bool showDebugMessages = true;

		// Token: 0x04002601 RID: 9729
		private string runtimeShader = "TSD/Tire Vertex Deformation";

		// Token: 0x04002602 RID: 9730
		private string runtimeShaderTessellation = "TSD/Tire Vertex Deformation Tessellation";

		// Token: 0x04002603 RID: 9731
		private string debugShader = "TSD/Tire Vertex Deformation Debug";

		// Token: 0x04002604 RID: 9732
		private string debugShaderTessellation = "TSD/Tire Vertex Deformation Debug Tessellation";

		// Token: 0x04002605 RID: 9733
		private Shader rt;

		// Token: 0x04002606 RID: 9734
		private Shader ds;
	}
}
