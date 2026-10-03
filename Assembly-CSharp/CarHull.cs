using System;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class CarHull : MonoBehaviour
{
	// Token: 0x0600048D RID: 1165 RVA: 0x0002FCDC File Offset: 0x0002DEDC
	private void Awake()
	{
		base.GetComponent<ImpactDeformable>().OnDeformForce += this.CarHull_OnDeformForce;
		ImpactDeformable[] bumpers = this.Bumpers;
		for (int i = 0; i < bumpers.Length; i++)
		{
			bumpers[i].OnDeformForce += this.CarHull_OnDeformForce;
		}
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x0002FD2C File Offset: 0x0002DF2C
	private void OnDisable()
	{
		base.GetComponent<ImpactDeformable>().OnDeformForce -= this.CarHull_OnDeformForce;
		ImpactDeformable[] bumpers = this.Bumpers;
		for (int i = 0; i < bumpers.Length; i++)
		{
			bumpers[i].OnDeformForce += this.CarHull_OnDeformForce;
		}
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0002FD7C File Offset: 0x0002DF7C
	private void CarHull_OnDeformForce(ImpactDeformable impactDeformable, Vector3 point, Vector3 force)
	{
		if (!this.Audio.isPlaying)
		{
			this.Audio.pitch = Random.Range(0.2f, 1f);
			this.Audio.Play();
		}
		this.Audio.volume = Mathf.Max(force.magnitude * 5f, this.Audio.volume);
	}

	// Token: 0x04000958 RID: 2392
	public AudioSource Audio;

	// Token: 0x04000959 RID: 2393
	public ImpactDeformable[] Bumpers;
}
