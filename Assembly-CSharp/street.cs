using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000188 RID: 392
public class street : MonoBehaviour
{
	// Token: 0x040019A6 RID: 6566
	public AudioMixer streetMixer;

	// Token: 0x040019A7 RID: 6567
	public gate leftGate;

	// Token: 0x040019A8 RID: 6568
	public gate rightGate;

	// Token: 0x040019A9 RID: 6569
	public float maxIntensity;

	// Token: 0x040019AA RID: 6570
	public float maxBounce;

	// Token: 0x040019AB RID: 6571
	private Light mainLight;

	// Token: 0x040019AC RID: 6572
	private float openingFactor;

	// Token: 0x040019AD RID: 6573
	private float lowPass;
}
