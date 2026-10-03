using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002F8 RID: 760
	[Preserve]
	[ES3Properties(new string[]
	{

	})]
	public class ES3UserType_ParticleSystem : ES3ComponentType
	{
		// Token: 0x060013F5 RID: 5109 RVA: 0x000D3069 File Offset: 0x000D1269
		public ES3UserType_ParticleSystem() : base(typeof(ParticleSystem))
		{
			ES3UserType_ParticleSystem.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x000D3088 File Offset: 0x000D1288
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			ParticleSystem particleSystem = (ParticleSystem)obj;
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x000D3094 File Offset: 0x000D1294
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			ParticleSystem particleSystem = (ParticleSystem)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				reader.Skip();
			}
		}

		// Token: 0x04002464 RID: 9316
		public static ES3Type Instance;
	}
}
