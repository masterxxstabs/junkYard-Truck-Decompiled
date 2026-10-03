using System;
using System.Linq;
using UnityEngine;

namespace TSD.uTireSettings
{
	// Token: 0x02000352 RID: 850
	[CreateAssetMenu(fileName = "uTire Default Shader Settings", menuName = "TSD/Tire Default Shader Settings", order = 1)]
	public class uTireDefaultMaterialSettings : ScriptableObject
	{
		// Token: 0x060015C3 RID: 5571 RVA: 0x000E1D55 File Offset: 0x000DFF55
		public void SetSettings(SRPType _srpType, DirectionType _directionType, ShaderType _shaderFeatures, bool _instancing)
		{
			this.sRPType = _srpType;
			this.directionType = _directionType;
			this.shaderFeatures = _shaderFeatures;
			this.instancing = _instancing;
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060015C4 RID: 5572 RVA: 0x000E1D74 File Offset: 0x000DFF74
		public static uTireDefaultMaterialSettings Instance
		{
			get
			{
				if (uTireDefaultMaterialSettings._instance == null)
				{
					Resources.LoadAll("", typeof(uTireDefaultMaterialSettings));
					uTireDefaultMaterialSettings._instance = Resources.FindObjectsOfTypeAll<uTireDefaultMaterialSettings>().FirstOrDefault<uTireDefaultMaterialSettings>();
					Resources.LoadAll("", typeof(uTireDefaultMaterialSettings));
					uTireDefaultMaterialSettings._instance = Resources.FindObjectsOfTypeAll<uTireDefaultMaterialSettings>().FirstOrDefault<uTireDefaultMaterialSettings>();
				}
				return uTireDefaultMaterialSettings._instance;
			}
		}

		// Token: 0x0400265C RID: 9820
		public SRPType sRPType;

		// Token: 0x0400265D RID: 9821
		public DirectionType directionType;

		// Token: 0x0400265E RID: 9822
		public ShaderType shaderFeatures = ShaderType.standard;

		// Token: 0x0400265F RID: 9823
		public bool instancing;

		// Token: 0x04002660 RID: 9824
		private static uTireDefaultMaterialSettings _instance;
	}
}
