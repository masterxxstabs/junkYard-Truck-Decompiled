using System;
using UnityEngine;

// Token: 0x0200009B RID: 155
[AddComponentMenu("Enviro/Utility/Seasons for Meshes")]
public class EnviroMeshSeasons : MonoBehaviour
{
	// Token: 0x0600035D RID: 861 RVA: 0x0001EE24 File Offset: 0x0001D024
	private void Start()
	{
		this.myRenderer = base.GetComponent<MeshRenderer>();
		if (this.myRenderer == null)
		{
			Debug.LogError("Please correct script placement! We need a MeshRenderer to work with!");
			base.enabled = false;
		}
		this.UpdateSeasonMaterial();
		EnviroSkyMgr.instance.OnSeasonChanged += delegate(EnviroSeasons.Seasons season)
		{
			this.UpdateSeasonMaterial();
		};
	}

	// Token: 0x0600035E RID: 862 RVA: 0x0001EE78 File Offset: 0x0001D078
	private void OnEnable()
	{
		if (this.SpringMaterial == null)
		{
			Debug.LogError("Please assign a spring material in Inspector!");
			base.enabled = false;
		}
		if (this.SummerMaterial == null)
		{
			Debug.LogError("Please assign a summer material in Inspector!");
			base.enabled = false;
		}
		if (this.AutumnMaterial == null)
		{
			Debug.LogError("Please assign a autumn material in Inspector!");
			base.enabled = false;
		}
		if (this.WinterMaterial == null)
		{
			Debug.LogError("Please assign a winter material in Inspector!");
			base.enabled = false;
		}
	}

	// Token: 0x0600035F RID: 863 RVA: 0x0001EF04 File Offset: 0x0001D104
	private void UpdateSeasonMaterial()
	{
		switch (EnviroSkyMgr.instance.GetCurrentSeason())
		{
		case EnviroSeasons.Seasons.Spring:
			this.myRenderer.sharedMaterial = this.SpringMaterial;
			return;
		case EnviroSeasons.Seasons.Summer:
			this.myRenderer.sharedMaterial = this.SummerMaterial;
			return;
		case EnviroSeasons.Seasons.Autumn:
			this.myRenderer.sharedMaterial = this.AutumnMaterial;
			return;
		case EnviroSeasons.Seasons.Winter:
			this.myRenderer.sharedMaterial = this.WinterMaterial;
			return;
		default:
			return;
		}
	}

	// Token: 0x04000784 RID: 1924
	public Material SpringMaterial;

	// Token: 0x04000785 RID: 1925
	public Material SummerMaterial;

	// Token: 0x04000786 RID: 1926
	public Material AutumnMaterial;

	// Token: 0x04000787 RID: 1927
	public Material WinterMaterial;

	// Token: 0x04000788 RID: 1928
	private MeshRenderer myRenderer;
}
