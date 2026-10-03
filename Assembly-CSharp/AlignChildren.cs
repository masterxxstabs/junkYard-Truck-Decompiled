using System;
using UnityEngine;

// Token: 0x02000009 RID: 9
[ExecuteInEditMode]
public class AlignChildren : MonoBehaviour
{
	// Token: 0x06000019 RID: 25 RVA: 0x000025A0 File Offset: 0x000007A0
	private void Update()
	{
		if (!this.alignNow || Application.isPlaying)
		{
			return;
		}
		Transform transform = base.transform;
		int childCount = transform.childCount;
		for (int i = 0; i < childCount - 1; i++)
		{
			Transform child = transform.GetChild(i);
			Vector3 forward = transform.GetChild(i + 1).position - child.position;
			child.rotation = Quaternion.LookRotation(forward, Vector3.up);
		}
		this.alignNow = false;
	}

	// Token: 0x04000018 RID: 24
	public bool alignNow;
}
