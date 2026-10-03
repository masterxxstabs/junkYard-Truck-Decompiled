using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200008F RID: 143
[AddComponentMenu("Enviro/Vegetation Growth Object")]
public class EnviroVegetationInstance : MonoBehaviour
{
	// Token: 0x06000318 RID: 792 RVA: 0x0001C480 File Offset: 0x0001A680
	private void Start()
	{
		EnviroSkyMgr.instance.RegisterVegetationInstance(this);
		this.currentSeason = EnviroSkyMgr.instance.GetCurrentSeason();
		this.maxAgeInHours = EnviroSkyMgr.instance.GetInHours(this.Age.maxAgeHours, this.Age.maxAgeDays, this.Age.maxAgeYears);
		EnviroSkyMgr.instance.OnSeasonChanged += delegate(EnviroSeasons.Seasons season)
		{
			this.SetSeason();
		};
		if (this.Age.randomStartAge)
		{
			this.Age.startAgeinHours = Random.Range(0f, (float)this.maxAgeInHours);
			this.Age.randomStartAge = false;
		}
		this.Birth(0, this.Age.startAgeinHours);
	}

	// Token: 0x06000319 RID: 793 RVA: 0x0001C538 File Offset: 0x0001A738
	private void OnEnable()
	{
		if (this.GrowStages.Count < 1)
		{
			Debug.LogError("Please setup GrowStages!");
			base.enabled = false;
		}
		for (int i = 0; i < this.GrowStages.Count; i++)
		{
			if (this.GrowStages[i].GrowGameobjectAutumn == null || this.GrowStages[i].GrowGameobjectSpring == null || this.GrowStages[i].GrowGameobjectSummer == null || this.GrowStages[i].GrowGameobjectWinter == null)
			{
				Debug.LogError("One ore more GrowStages missing GrowPrefabs!");
				base.enabled = false;
			}
		}
	}

	// Token: 0x0600031A RID: 794 RVA: 0x0001C5F2 File Offset: 0x0001A7F2
	private void SetSeason()
	{
		this.currentSeason = EnviroSkyMgr.instance.GetCurrentSeason();
		this.VegetationChange();
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0001C60C File Offset: 0x0001A80C
	public void KeepVariablesClear()
	{
		this.GrowStages[0].minAgePercent = 0f;
		for (int i = 0; i < this.GrowStages.Count; i++)
		{
			if (this.GrowStages[i].minAgePercent > 100f)
			{
				this.GrowStages[i].minAgePercent = 100f;
			}
		}
	}

	// Token: 0x0600031C RID: 796 RVA: 0x0001C673 File Offset: 0x0001A873
	public void UpdateInstance()
	{
		if (this.reBirth)
		{
			this.Birth(0, 0f);
		}
		if (this.shrink)
		{
			this.ShrinkAndDeactivate();
		}
		if (this.canGrow)
		{
			this.UpdateGrowth();
		}
	}

	// Token: 0x0600031D RID: 797 RVA: 0x0001C6A8 File Offset: 0x0001A8A8
	public void UpdateGrowth()
	{
		this.ageInHours = EnviroSkyMgr.instance.GetCurrentTimeInHours() - this.Age.birthdayInHours;
		this.KeepVariablesClear();
		if (!this.stay)
		{
			if (this.currentStage + 1 < this.GrowStages.Count)
			{
				if (this.maxAgeInHours * (double)(this.GrowStages[this.currentStage + 1].minAgePercent / 100f) <= this.ageInHours && this.ageInHours > 0.0)
				{
					this.currentStage++;
					this.VegetationChange();
					return;
				}
				if (this.GrowStages[this.currentStage].growAction == EnviroVegetationStage.GrowState.Grow)
				{
					this.CalculateScale();
					return;
				}
			}
			else if (!this.stay)
			{
				if (this.ageInHours > this.maxAgeInHours)
				{
					if (!this.Age.Loop)
					{
						this.stay = true;
						return;
					}
					this.currentVegetationObject.SetActive(false);
					if (this.DeadPrefab != null)
					{
						this.DeadPrefabLoop();
						return;
					}
					this.Birth(this.Age.LoopFromGrowStage, 0f);
					return;
				}
				else if (this.GrowStages[this.currentStage].growAction == EnviroVegetationStage.GrowState.Grow)
				{
					this.CalculateScale();
				}
			}
		}
	}

	// Token: 0x0600031E RID: 798 RVA: 0x0001C7F0 File Offset: 0x0001A9F0
	private void DeadPrefabLoop()
	{
		this.stay = true;
		Object.Instantiate<GameObject>(this.DeadPrefab, base.transform.position, base.transform.rotation).transform.localScale = this.currentVegetationObject.transform.localScale;
		this.Birth(this.Age.LoopFromGrowStage, 0f);
		this.stay = false;
	}

	// Token: 0x0600031F RID: 799 RVA: 0x0001C85C File Offset: 0x0001AA5C
	private IEnumerator BirthColliders()
	{
		Collider[] colliders = this.currentVegetationObject.GetComponentsInChildren<Collider>();
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = false;
		}
		yield return new WaitForSeconds(10f);
		for (int j = 0; j < colliders.Length; j++)
		{
			colliders[j].enabled = true;
		}
		yield break;
	}

	// Token: 0x06000320 RID: 800 RVA: 0x0001C86C File Offset: 0x0001AA6C
	private void CalculateScale()
	{
		if (this.rescale)
		{
			this.currentVegetationObject.transform.localScale = this.minScale;
			this.rescale = false;
		}
		double num = this.ageInHours / this.maxAgeInHours * (double)this.GrowSpeedMod;
		this.currentVegetationObject.transform.localScale = this.minScale + new Vector3((float)num, (float)num, (float)num);
		if (this.currentVegetationObject.transform.localScale.y > this.maxScale.y)
		{
			this.currentVegetationObject.transform.localScale = this.maxScale;
		}
		if (this.currentVegetationObject.transform.localScale.y < this.minScale.y)
		{
			this.currentVegetationObject.transform.localScale = this.minScale;
		}
	}

	// Token: 0x06000321 RID: 801 RVA: 0x0001C94C File Offset: 0x0001AB4C
	public void Birth(int stage, float startAge)
	{
		this.Age.birthdayInHours = EnviroSkyMgr.instance.GetCurrentTimeInHours() - (double)startAge;
		startAge = 0f;
		this.ageInHours = 0.0;
		this.currentStage = stage;
		this.rescale = true;
		this.reBirth = false;
		this.VegetationChange();
		base.StartCoroutine(this.BirthColliders());
	}

	// Token: 0x06000322 RID: 802 RVA: 0x0001C9B0 File Offset: 0x0001ABB0
	private void SeasonAction()
	{
		if (this.Seasons.seasonAction == EnviroVegetationSeasons.SeasonAction.SpawnDeadPrefab)
		{
			if (this.DeadPrefab != null)
			{
				Object.Instantiate<GameObject>(this.DeadPrefab, base.transform.position, base.transform.rotation).transform.localScale = this.currentVegetationObject.transform.localScale;
			}
			this.currentVegetationObject.SetActive(false);
			return;
		}
		if (this.Seasons.seasonAction == EnviroVegetationSeasons.SeasonAction.Deactivate)
		{
			this.shrink = true;
			return;
		}
		if (this.Seasons.seasonAction == EnviroVegetationSeasons.SeasonAction.Destroy)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000323 RID: 803 RVA: 0x0001CA50 File Offset: 0x0001AC50
	private void CheckSeason(bool update)
	{
		if (!update && this.canGrow)
		{
			this.SeasonAction();
			this.canGrow = false;
			return;
		}
		if (update && !this.canGrow)
		{
			this.canGrow = true;
			this.reBirth = true;
			return;
		}
		if (!update && !this.canGrow)
		{
			this.SeasonAction();
		}
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0001CAA4 File Offset: 0x0001ACA4
	private void ShrinkAndDeactivate()
	{
		if (this.currentVegetationObject.transform.localScale.y > this.minScale.y)
		{
			this.currentVegetationObject.transform.localScale = new Vector3(this.currentVegetationObject.transform.localScale.x - 0.1f * Time.deltaTime, this.currentVegetationObject.transform.localScale.y - 0.1f * Time.deltaTime, this.currentVegetationObject.transform.localScale.z - 0.1f * Time.deltaTime);
			return;
		}
		this.shrink = false;
		this.currentVegetationObject.SetActive(false);
	}

	// Token: 0x06000325 RID: 805 RVA: 0x0001CB60 File Offset: 0x0001AD60
	public void VegetationChange()
	{
		this.canGrow = true;
		if (this.currentVegetationObject != null)
		{
			this.currentVegetationObject.SetActive(false);
		}
		switch (this.currentSeason)
		{
		case EnviroSeasons.Seasons.Spring:
			this.currentVegetationObject = this.GrowStages[this.currentStage].GrowGameobjectSpring;
			this.CalculateScale();
			this.currentVegetationObject.SetActive(true);
			if (!this.Seasons.GrowInSpring)
			{
				this.CheckSeason(false);
				return;
			}
			if (this.Seasons.GrowInSpring)
			{
				this.CheckSeason(true);
				return;
			}
			break;
		case EnviroSeasons.Seasons.Summer:
			this.currentVegetationObject = this.GrowStages[this.currentStage].GrowGameobjectSummer;
			this.CalculateScale();
			this.currentVegetationObject.SetActive(true);
			if (!this.Seasons.GrowInSummer)
			{
				this.CheckSeason(false);
				return;
			}
			if (this.Seasons.GrowInSummer)
			{
				this.CheckSeason(true);
				return;
			}
			break;
		case EnviroSeasons.Seasons.Autumn:
			this.currentVegetationObject = this.GrowStages[this.currentStage].GrowGameobjectAutumn;
			this.CalculateScale();
			this.currentVegetationObject.SetActive(true);
			if (!this.Seasons.GrowInAutumn)
			{
				this.CheckSeason(false);
				return;
			}
			if (this.Seasons.GrowInAutumn)
			{
				this.CheckSeason(true);
				return;
			}
			break;
		case EnviroSeasons.Seasons.Winter:
			this.currentVegetationObject = this.GrowStages[this.currentStage].GrowGameobjectWinter;
			this.CalculateScale();
			this.currentVegetationObject.SetActive(true);
			if (!this.Seasons.GrowInWinter)
			{
				this.CheckSeason(false);
				return;
			}
			if (this.Seasons.GrowInWinter)
			{
				this.CheckSeason(true);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0001CD11 File Offset: 0x0001AF11
	private void LateUpdate()
	{
		if (this.GrowStages[this.currentStage].billboard && this.canGrow)
		{
			base.transform.rotation = Camera.main.transform.rotation;
		}
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0001CD4D File Offset: 0x0001AF4D
	private void OnDrawGizmos()
	{
		Gizmos.color = this.GizmoColor;
		Gizmos.DrawCube(base.transform.position, new Vector3(this.GizmoSize, this.GizmoSize, this.GizmoSize));
	}

	// Token: 0x040006D2 RID: 1746
	[HideInInspector]
	public int id;

	// Token: 0x040006D3 RID: 1747
	public EnviroVegetationAge Age;

	// Token: 0x040006D4 RID: 1748
	public EnviroVegetationSeasons Seasons;

	// Token: 0x040006D5 RID: 1749
	public List<EnviroVegetationStage> GrowStages = new List<EnviroVegetationStage>();

	// Token: 0x040006D6 RID: 1750
	public Vector3 minScale = new Vector3(0.1f, 0.1f, 0.1f);

	// Token: 0x040006D7 RID: 1751
	public Vector3 maxScale = new Vector3(1f, 1f, 1f);

	// Token: 0x040006D8 RID: 1752
	public float GrowSpeedMod = 1f;

	// Token: 0x040006D9 RID: 1753
	public GameObject DeadPrefab;

	// Token: 0x040006DA RID: 1754
	public Color GizmoColor = new Color(255f, 0f, 0f, 255f);

	// Token: 0x040006DB RID: 1755
	public float GizmoSize = 0.5f;

	// Token: 0x040006DC RID: 1756
	private EnviroSeasons.Seasons currentSeason;

	// Token: 0x040006DD RID: 1757
	private double ageInHours;

	// Token: 0x040006DE RID: 1758
	private double maxAgeInHours;

	// Token: 0x040006DF RID: 1759
	private int currentStage;

	// Token: 0x040006E0 RID: 1760
	private GameObject currentVegetationObject;

	// Token: 0x040006E1 RID: 1761
	private bool stay;

	// Token: 0x040006E2 RID: 1762
	private bool reBirth;

	// Token: 0x040006E3 RID: 1763
	private bool rescale = true;

	// Token: 0x040006E4 RID: 1764
	private bool canGrow = true;

	// Token: 0x040006E5 RID: 1765
	private bool shrink;
}
