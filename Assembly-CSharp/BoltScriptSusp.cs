using System;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class BoltScriptSusp : MonoBehaviour
{
	// Token: 0x0600005F RID: 95 RVA: 0x00004D34 File Offset: 0x00002F34
	private void Start()
	{
		this.thisWC.Translate(Vector3.up * ((float)this.boltTurns * 0.005f * -1f));
		JointSpring suspensionSpring = default(JointSpring);
		suspensionSpring.spring = (float)(35000 + this.boltTurns * -1 * 1000);
		suspensionSpring.damper = (float)(4500 + this.boltTurns * 40);
		suspensionSpring.targetPosition = 0.5f;
		this.wc.suspensionSpring = suspensionSpring;
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00004DBC File Offset: 0x00002FBC
	public void Tighten()
	{
		if (this.boltTurns < 25)
		{
			this.boltTurns++;
			this.thisWC.Translate(Vector3.up * -0.005f);
			JointSpring suspensionSpring = default(JointSpring);
			suspensionSpring.spring = (float)(35000 + this.boltTurns * -1 * 1000);
			suspensionSpring.damper = (float)(4500 + this.boltTurns * 40);
			suspensionSpring.targetPosition = 0.5f;
			this.wc.suspensionSpring = suspensionSpring;
		}
	}

	// Token: 0x06000061 RID: 97 RVA: 0x00004E50 File Offset: 0x00003050
	public void Loosen()
	{
		if (this.boltTurns > -25)
		{
			this.boltTurns--;
			this.thisWC.Translate(Vector3.up * 0.005f);
			JointSpring suspensionSpring = default(JointSpring);
			suspensionSpring.spring = (float)(35000 + this.boltTurns * -1 * 1000);
			suspensionSpring.damper = (float)(4500 + this.boltTurns * 40);
			suspensionSpring.targetPosition = 0.5f;
			this.wc.suspensionSpring = suspensionSpring;
		}
	}

	// Token: 0x040000E4 RID: 228
	public int boltTurns;

	// Token: 0x040000E5 RID: 229
	public Transform thisWC;

	// Token: 0x040000E6 RID: 230
	public WheelCollider wc;
}
