using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000095 RID: 149
[AddComponentMenu("Enviro/Weather Zone")]
public class EnviroZone : MonoBehaviour
{
	// Token: 0x06000336 RID: 822 RVA: 0x0001D154 File Offset: 0x0001B354
	private void Start()
	{
		if (this.zoneWeatherPresets.Count > 0)
		{
			if (!this.useMeshZone)
			{
				this.zoneCollider = base.gameObject.AddComponent<BoxCollider>();
				this.zoneCollider.isTrigger = true;
			}
			else
			{
				this.zoneMeshCollider = base.gameObject.AddComponent<MeshCollider>();
				this.zoneMeshCollider.sharedMesh = this.zoneMesh;
				this.zoneMeshCollider.convex = true;
				this.zoneMeshCollider.isTrigger = true;
			}
			if (!EnviroSkyMgr.instance.IsDefaultZone(base.gameObject))
			{
				EnviroSkyMgr.instance.RegisterZone(this);
			}
			else
			{
				this.isDefault = true;
			}
			this.UpdateZoneScale();
			this.nextUpdate = EnviroSkyMgr.instance.GetCurrentTimeInHours() + (double)this.WeatherUpdateIntervall;
			this.nextUpdateRealtime = Time.time + this.WeatherUpdateIntervall * 60f;
			return;
		}
		Debug.Log("Please add Weather Prefabs to Zone:" + base.gameObject.name);
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0001D24C File Offset: 0x0001B44C
	public void UpdateZoneScale()
	{
		if (!this.isDefault && !this.useMeshZone)
		{
			this.zoneCollider.size = this.zoneScale;
			return;
		}
		if (!this.isDefault && this.useMeshZone)
		{
			base.transform.localScale = this.zoneScale;
			return;
		}
		if (this.isDefault && !this.useMeshZone)
		{
			this.zoneCollider.size = Vector3.one * (1f / base.transform.localScale.y) * 0.25f;
		}
	}

	// Token: 0x06000338 RID: 824 RVA: 0x0001D2E4 File Offset: 0x0001B4E4
	public void CreateZoneWeatherTypeList()
	{
		for (int i = 0; i < this.zoneWeatherPresets.Count; i++)
		{
			if (this.zoneWeatherPresets[i] == null)
			{
				Debug.Log("Warning! Missing Weather Preset in Zone: " + this.zoneName);
				return;
			}
			bool flag = true;
			for (int j = 0; j < EnviroSkyMgr.instance.GetCurrentWeatherPresetList().Count; j++)
			{
				if (this.zoneWeatherPresets[i] == EnviroSkyMgr.instance.GetCurrentWeatherPresetList()[j])
				{
					flag = false;
					this.zoneWeather.Add(EnviroSkyMgr.instance.GetCurrentWeatherPrefabList()[j]);
				}
			}
			if (flag)
			{
				GameObject gameObject = new GameObject();
				EnviroWeatherPrefab enviroWeatherPrefab = gameObject.AddComponent<EnviroWeatherPrefab>();
				enviroWeatherPrefab.weatherPreset = this.zoneWeatherPresets[i];
				gameObject.name = enviroWeatherPrefab.weatherPreset.Name;
				for (int k = 0; k < enviroWeatherPrefab.weatherPreset.effectSystems.Count; k++)
				{
					if (enviroWeatherPrefab.weatherPreset.effectSystems[k] == null || enviroWeatherPrefab.weatherPreset.effectSystems[k].prefab == null)
					{
						Debug.Log("Warning! Missing Particle System Entry: " + enviroWeatherPrefab.weatherPreset.Name);
						Object.Destroy(gameObject);
						return;
					}
					GameObject gameObject2 = Object.Instantiate<GameObject>(enviroWeatherPrefab.weatherPreset.effectSystems[k].prefab, gameObject.transform);
					gameObject2.transform.localPosition = enviroWeatherPrefab.weatherPreset.effectSystems[k].localPositionOffset;
					gameObject2.transform.localEulerAngles = enviroWeatherPrefab.weatherPreset.effectSystems[k].localRotationOffset;
					ParticleSystem particleSystem = gameObject2.GetComponent<ParticleSystem>();
					if (particleSystem != null)
					{
						enviroWeatherPrefab.effectSystems.Add(particleSystem);
					}
					else
					{
						particleSystem = gameObject2.GetComponentInChildren<ParticleSystem>();
						if (!(particleSystem != null))
						{
							Debug.Log("No Particle System found in prefab in weather preset: " + enviroWeatherPrefab.weatherPreset.Name);
							Object.Destroy(gameObject);
							return;
						}
						enviroWeatherPrefab.effectSystems.Add(particleSystem);
					}
				}
				enviroWeatherPrefab.effectEmmisionRates.Clear();
				gameObject.transform.parent = EnviroSkyMgr.instance.GetVFXHolder().transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				this.zoneWeather.Add(enviroWeatherPrefab);
				EnviroSkyMgr.instance.GetCurrentWeatherPrefabList().Add(enviroWeatherPrefab);
				EnviroSkyMgr.instance.GetCurrentWeatherPresetList().Add(this.zoneWeatherPresets[i]);
			}
		}
		for (int l = 0; l < this.zoneWeather.Count; l++)
		{
			for (int m = 0; m < this.zoneWeather[l].effectSystems.Count; m++)
			{
				this.zoneWeather[l].effectEmmisionRates.Add(EnviroSkyMgr.instance.GetEmissionRate(this.zoneWeather[l].effectSystems[m]));
				EnviroSkyMgr.instance.SetEmissionRate(this.zoneWeather[l].effectSystems[m], 0f);
			}
		}
		if (this.isDefault && EnviroSkyMgr.instance.GetStartWeatherPreset() != null)
		{
			EnviroSkyMgr.instance.ChangeWeatherInstant(EnviroSkyMgr.instance.GetStartWeatherPreset());
			for (int n = 0; n < this.zoneWeather.Count; n++)
			{
				if (this.zoneWeather[n].weatherPreset == EnviroSkyMgr.instance.GetStartWeatherPreset())
				{
					this.currentActiveZoneWeatherPrefab = this.zoneWeather[n];
					this.lastActiveZoneWeatherPrefab = this.zoneWeather[n];
				}
			}
			this.currentActiveZoneWeatherPreset = EnviroSkyMgr.instance.GetStartWeatherPreset();
			this.lastActiveZoneWeatherPreset = EnviroSkyMgr.instance.GetStartWeatherPreset();
		}
		else
		{
			this.currentActiveZoneWeatherPrefab = this.zoneWeather[0];
			this.lastActiveZoneWeatherPrefab = this.zoneWeather[0];
			this.currentActiveZoneWeatherPreset = this.zoneWeatherPresets[0];
			this.lastActiveZoneWeatherPreset = this.zoneWeatherPresets[0];
		}
		this.nextUpdate = EnviroSkyMgr.instance.GetCurrentTimeInHours() + (double)this.WeatherUpdateIntervall;
	}

	// Token: 0x06000339 RID: 825 RVA: 0x0001D764 File Offset: 0x0001B964
	private void BuildNewWeatherList()
	{
		this.curPossibleZoneWeather = new List<EnviroWeatherPrefab>();
		for (int i = 0; i < this.zoneWeather.Count; i++)
		{
			switch (EnviroSkyMgr.instance.GetCurrentSeason())
			{
			case EnviroSeasons.Seasons.Spring:
				if (this.zoneWeather[i].weatherPreset.Spring)
				{
					this.curPossibleZoneWeather.Add(this.zoneWeather[i]);
				}
				break;
			case EnviroSeasons.Seasons.Summer:
				if (this.zoneWeather[i].weatherPreset.Summer)
				{
					this.curPossibleZoneWeather.Add(this.zoneWeather[i]);
				}
				break;
			case EnviroSeasons.Seasons.Autumn:
				if (this.zoneWeather[i].weatherPreset.Autumn)
				{
					this.curPossibleZoneWeather.Add(this.zoneWeather[i]);
				}
				break;
			case EnviroSeasons.Seasons.Winter:
				if (this.zoneWeather[i].weatherPreset.winter)
				{
					this.curPossibleZoneWeather.Add(this.zoneWeather[i]);
				}
				break;
			}
		}
	}

	// Token: 0x0600033A RID: 826 RVA: 0x0001D888 File Offset: 0x0001BA88
	private EnviroWeatherPrefab PossibiltyCheck()
	{
		List<EnviroWeatherPrefab> list = new List<EnviroWeatherPrefab>();
		for (int i = 0; i < this.curPossibleZoneWeather.Count; i++)
		{
			int num = Random.Range(0, 100);
			if (EnviroSkyMgr.instance.GetCurrentSeason() == EnviroSeasons.Seasons.Spring)
			{
				if ((float)num <= this.curPossibleZoneWeather[i].weatherPreset.possibiltyInSpring)
				{
					list.Add(this.curPossibleZoneWeather[i]);
				}
			}
			else if (EnviroSkyMgr.instance.GetCurrentSeason() == EnviroSeasons.Seasons.Summer)
			{
				if ((float)num <= this.curPossibleZoneWeather[i].weatherPreset.possibiltyInSummer)
				{
					list.Add(this.curPossibleZoneWeather[i]);
				}
			}
			else if (EnviroSkyMgr.instance.GetCurrentSeason() == EnviroSeasons.Seasons.Autumn)
			{
				if ((float)num <= this.curPossibleZoneWeather[i].weatherPreset.possibiltyInAutumn)
				{
					list.Add(this.curPossibleZoneWeather[i]);
				}
			}
			else if (EnviroSkyMgr.instance.GetCurrentSeason() == EnviroSeasons.Seasons.Winter && (float)num <= this.curPossibleZoneWeather[i].weatherPreset.possibiltyInWinter)
			{
				list.Add(this.curPossibleZoneWeather[i]);
			}
		}
		if (list.Count > 0)
		{
			EnviroSkyMgr.instance.NotifyZoneWeatherChanged(list[0].weatherPreset, this);
			return list[0];
		}
		return this.currentActiveZoneWeatherPrefab;
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0001D9E0 File Offset: 0x0001BBE0
	private void WeatherUpdate()
	{
		this.nextUpdate = EnviroSkyMgr.instance.GetCurrentTimeInHours() + (double)this.WeatherUpdateIntervall;
		this.nextUpdateRealtime = Time.time + this.WeatherUpdateIntervall * 60f;
		this.BuildNewWeatherList();
		this.lastActiveZoneWeatherPrefab = this.currentActiveZoneWeatherPrefab;
		this.lastActiveZoneWeatherPreset = this.currentActiveZoneWeatherPreset;
		this.currentActiveZoneWeatherPrefab = this.PossibiltyCheck();
		this.currentActiveZoneWeatherPreset = this.currentActiveZoneWeatherPrefab.weatherPreset;
		EnviroSkyMgr.instance.NotifyZoneWeatherChanged(this.currentActiveZoneWeatherPreset, this);
		Debug.Log(this.currentActiveZoneWeatherPreset);
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0001DA74 File Offset: 0x0001BC74
	private IEnumerator CreateWeatherListLate()
	{
		yield return 0;
		this.CreateZoneWeatherTypeList();
		this.init = true;
		yield break;
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0001DA84 File Offset: 0x0001BC84
	private void LateUpdate()
	{
		if (EnviroSkyMgr.instance == null)
		{
			Debug.Log("No EnviroSky instance found!");
			return;
		}
		if (EnviroSkyMgr.instance.IsStarted() && !this.init)
		{
			if (this.zoneWeatherPresets.Count < 1)
			{
				Debug.Log("Zone with no Presets! Please assign at least one preset. Deactivated for now!");
				base.enabled = false;
				return;
			}
			if (this.isDefault)
			{
				this.CreateZoneWeatherTypeList();
				this.init = true;
			}
			else
			{
				base.StartCoroutine(this.CreateWeatherListLate());
			}
		}
		if (this.updateMode == EnviroZone.WeatherUpdateMode.GameTimeHours)
		{
			if (EnviroSkyMgr.instance.GetCurrentTimeInHours() > this.nextUpdate && EnviroSkyMgr.instance.IsAutoWeatherUpdateActive() && EnviroSkyMgr.instance.IsStarted())
			{
				this.WeatherUpdate();
			}
		}
		else if (Time.time > this.nextUpdateRealtime && EnviroSkyMgr.instance.IsAutoWeatherUpdateActive() && EnviroSkyMgr.instance.IsStarted())
		{
			this.WeatherUpdate();
		}
		if (EnviroSkyMgr.instance.Player == null)
		{
			return;
		}
		if (this.isDefault && this.init && !this.useMeshZone)
		{
			this.zoneCollider.center = new Vector3(0f, (EnviroSkyMgr.instance.Player.transform.position.y - base.transform.position.y) / base.transform.lossyScale.y, 0f);
		}
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0001DBE8 File Offset: 0x0001BDE8
	private void OnTriggerEnter(Collider col)
	{
		if (EnviroSkyMgr.instance == null)
		{
			return;
		}
		if (EnviroSkyMgr.instance.GetUseWeatherTag())
		{
			if (col.gameObject.tag == EnviroSkyMgr.instance.GetEnviroSkyTag())
			{
				EnviroSkyMgr.instance.SetCurrentActiveZone(this);
				EnviroSkyMgr.instance.NotifyZoneChanged(this);
				return;
			}
		}
		else if (EnviroSkyMgr.instance.IsEnviroSkyAttached(col.gameObject))
		{
			EnviroSkyMgr.instance.SetCurrentActiveZone(this);
			EnviroSkyMgr.instance.NotifyZoneChanged(this);
		}
	}

	// Token: 0x0600033F RID: 831 RVA: 0x0001DC6C File Offset: 0x0001BE6C
	private void OnTriggerExit(Collider col)
	{
		if (!this.ExitToDefault || EnviroSkyMgr.instance == null)
		{
			return;
		}
		if (EnviroSkyMgr.instance.GetUseWeatherTag())
		{
			if (col.gameObject.tag == EnviroSkyMgr.instance.GetEnviroSkyTag())
			{
				EnviroSkyMgr.instance.SetToZone(0);
				EnviroSkyMgr.instance.NotifyZoneChanged(EnviroSkyMgr.instance.GetZoneByID(0));
				return;
			}
		}
		else if (EnviroSkyMgr.instance.IsEnviroSkyAttached(col.gameObject))
		{
			EnviroSkyMgr.instance.SetToZone(0);
			EnviroSkyMgr.instance.NotifyZoneChanged(EnviroSkyMgr.instance.GetZoneByID(0));
		}
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0001DD0C File Offset: 0x0001BF0C
	private void OnDrawGizmos()
	{
		Gizmos.color = this.zoneGizmoColor;
		if (this.useMeshZone && this.zoneMesh != null)
		{
			Gizmos.DrawMesh(this.zoneMesh);
			return;
		}
		Gizmos.DrawCube(base.transform.position, new Vector3(this.zoneScale.x, this.zoneScale.y, this.zoneScale.z));
	}

	// Token: 0x04000735 RID: 1845
	[Tooltip("Defines the zone name.")]
	public string zoneName;

	// Token: 0x04000736 RID: 1846
	[Tooltip("Uncheck to remove OnTriggerExit call when using overlapping zone layout.")]
	public bool ExitToDefault = true;

	// Token: 0x04000737 RID: 1847
	public List<EnviroWeatherPrefab> zoneWeather = new List<EnviroWeatherPrefab>();

	// Token: 0x04000738 RID: 1848
	public List<EnviroWeatherPrefab> curPossibleZoneWeather;

	// Token: 0x04000739 RID: 1849
	[Header("Zone weather settings:")]
	[Tooltip("Add all weather prefabs for this zone here.")]
	public List<EnviroWeatherPreset> zoneWeatherPresets = new List<EnviroWeatherPreset>();

	// Token: 0x0400073A RID: 1850
	[Tooltip("Shall weather changes occure based on gametime or realtime?")]
	public EnviroZone.WeatherUpdateMode updateMode;

	// Token: 0x0400073B RID: 1851
	[Tooltip("Defines how often (gametime hours or realtime minutes) the system will heck to change the current weather conditions.")]
	public float WeatherUpdateIntervall = 6f;

	// Token: 0x0400073C RID: 1852
	[Header("Zone scaling and gizmo:")]
	[Tooltip("Enable this to use a mesh for zone trigger.")]
	public bool useMeshZone;

	// Token: 0x0400073D RID: 1853
	[Tooltip("Custom Zone Mesh")]
	public Mesh zoneMesh;

	// Token: 0x0400073E RID: 1854
	[Tooltip("Defines the zone scale.")]
	public Vector3 zoneScale = new Vector3(100f, 100f, 100f);

	// Token: 0x0400073F RID: 1855
	[Tooltip("Defines the color of the zone's gizmo in editor mode.")]
	public Color zoneGizmoColor = Color.gray;

	// Token: 0x04000740 RID: 1856
	[Header("Current active weather:")]
	[Tooltip("The current active weather conditions.")]
	public EnviroWeatherPrefab currentActiveZoneWeatherPrefab;

	// Token: 0x04000741 RID: 1857
	public EnviroWeatherPreset currentActiveZoneWeatherPreset;

	// Token: 0x04000742 RID: 1858
	[HideInInspector]
	public EnviroWeatherPrefab lastActiveZoneWeatherPrefab;

	// Token: 0x04000743 RID: 1859
	[HideInInspector]
	public EnviroWeatherPreset lastActiveZoneWeatherPreset;

	// Token: 0x04000744 RID: 1860
	private BoxCollider zoneCollider;

	// Token: 0x04000745 RID: 1861
	private MeshCollider zoneMeshCollider;

	// Token: 0x04000746 RID: 1862
	private double nextUpdate;

	// Token: 0x04000747 RID: 1863
	private float nextUpdateRealtime;

	// Token: 0x04000748 RID: 1864
	public bool init;

	// Token: 0x04000749 RID: 1865
	private bool isDefault;

	// Token: 0x02000395 RID: 917
	public enum WeatherUpdateMode
	{
		// Token: 0x0400274F RID: 10063
		GameTimeHours,
		// Token: 0x04002750 RID: 10064
		RealTimeMinutes
	}
}
