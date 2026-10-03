using System;
using System.Collections.Generic;
using ch.sycoforge.Decal;
using ch.sycoforge.Decal.Projectors;
using UnityEngine;

// Token: 0x02000142 RID: 322
[RequireComponent(typeof(EasyDecal))]
public class CandidateFilter : MonoBehaviour
{
	// Token: 0x06000849 RID: 2121 RVA: 0x0006D6A4 File Offset: 0x0006B8A4
	private void Start()
	{
		this.decal = base.GetComponent<EasyDecal>();
		ch.sycoforge.Decal.Projectors.Projector projector = this.decal.Projector;
		if (projector != null && projector is BoxProjector)
		{
			(projector as BoxProjector).OnCandidatesProcessed += this.bp_OnCandidatesProcessed;
		}
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0006D6EC File Offset: 0x0006B8EC
	private void bp_OnCandidatesProcessed(List<Collider> colliders)
	{
		List<Collider> list = new List<Collider>();
		foreach (Collider collider in colliders)
		{
			if (!collider.gameObject.Equals(this.ExclusiveReceiver))
			{
				list.Add(collider);
			}
		}
		foreach (Collider item in list)
		{
			colliders.Remove(item);
		}
	}

	// Token: 0x04001347 RID: 4935
	public GameObject ExclusiveReceiver;

	// Token: 0x04001348 RID: 4936
	private EasyDecal decal;
}
