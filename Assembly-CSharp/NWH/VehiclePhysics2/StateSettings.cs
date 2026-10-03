using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
	// Token: 0x02000256 RID: 598
	[CreateAssetMenu(fileName = "NWH Vehicle Physics", menuName = "NWH Vehicle Physics/State Settings", order = 1)]
	[Serializable]
	public class StateSettings : ScriptableObject
	{
		// Token: 0x06000F80 RID: 3968 RVA: 0x000B6ED8 File Offset: 0x000B50D8
		public StateDefinition GetDefinition(string fullComponentTypeName)
		{
			return this.definitions.Find((StateDefinition d) => d.fullName == fullComponentTypeName);
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x000B6F0C File Offset: 0x000B510C
		public void Reload()
		{
			List<string> fullNames = (from t in Assembly.GetAssembly(typeof(VehicleComponent)).GetTypes()
			where !t.IsAbstract && t.IsSubclassOf(typeof(VehicleComponent))
			select t.FullName).ToList<string>();
			foreach (string text in fullNames)
			{
				if (this.GetDefinition(text) == null)
				{
					this.definitions.Add(new StateDefinition(text, true, true, -1));
				}
			}
			this.definitions.RemoveAll((StateDefinition d) => fullNames.All((string n) => n != d.fullName));
			this.definitions = (from d in this.definitions
			orderby d.fullName
			select d).ToList<StateDefinition>();
		}

		// Token: 0x04002016 RID: 8214
		public List<StateDefinition> definitions = new List<StateDefinition>();

		// Token: 0x04002017 RID: 8215
		public List<LOD> LODs = new List<LOD>();
	}
}
