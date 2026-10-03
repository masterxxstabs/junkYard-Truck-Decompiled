using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000132 RID: 306
public class PlayAnimation : MonoBehaviour
{
	// Token: 0x060007FA RID: 2042 RVA: 0x0006B898 File Offset: 0x00069A98
	private void Start()
	{
		this.animGameobject = base.gameObject.GetComponent<Animation>();
		foreach (object obj in this.animGameobject)
		{
			AnimationState item = (AnimationState)obj;
			this.animList.Add(item);
		}
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0006B908 File Offset: 0x00069B08
	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && !base.GetComponent<Animation>().isPlaying)
		{
			base.GetComponent<Animation>().clip = this.animList[0].clip;
			base.GetComponent<Animation>().Play();
		}
		if (Input.GetMouseButtonDown(1) && !base.GetComponent<Animation>().isPlaying)
		{
			base.GetComponent<Animation>().clip = this.animList[1].clip;
			base.GetComponent<Animation>().Play();
		}
	}

	// Token: 0x040012C8 RID: 4808
	private Animation animGameobject;

	// Token: 0x040012C9 RID: 4809
	private List<AnimationState> animList = new List<AnimationState>();
}
