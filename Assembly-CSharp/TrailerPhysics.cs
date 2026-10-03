using System;
using UnityEngine;

// Token: 0x02000157 RID: 343
public class TrailerPhysics : MonoBehaviour
{
	// Token: 0x06000893 RID: 2195 RVA: 0x0006F674 File Offset: 0x0006D874
	private void Update()
	{
		if (this.rb.velocity.sqrMagnitude > this.maxVel)
		{
			this.rb.velocity *= 0.5f;
		}
	}

	// Token: 0x040013D3 RID: 5075
	public Rigidbody rb;

	// Token: 0x040013D4 RID: 5076
	public float maxVel;

	// Token: 0x040013D5 RID: 5077
	public Rigidbody truck1rb;

	// Token: 0x040013D6 RID: 5078
	public float maxRelativeVelocity = 10f;
}
