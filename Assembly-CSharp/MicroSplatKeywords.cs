using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class MicroSplatKeywords : ScriptableObject
{
	// Token: 0x060005F4 RID: 1524 RVA: 0x00048B16 File Offset: 0x00046D16
	public bool IsKeywordEnabled(string k)
	{
		return this.keywords.Contains(k);
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x00048B24 File Offset: 0x00046D24
	public void EnableKeyword(string k)
	{
		if (!this.IsKeywordEnabled(k))
		{
			this.keywords.Add(k);
		}
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x00048B3B File Offset: 0x00046D3B
	public void DisableKeyword(string k)
	{
		if (this.IsKeywordEnabled(k))
		{
			this.keywords.Remove(k);
		}
	}

	// Token: 0x04000CF4 RID: 3316
	public List<string> keywords = new List<string>();
}
