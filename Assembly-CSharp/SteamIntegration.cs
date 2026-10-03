using System;
using System.IO;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class SteamIntegration : MonoBehaviour
{
	// Token: 0x06000836 RID: 2102 RVA: 0x0006CD2C File Offset: 0x0006AF2C
	private void Awake()
	{
		if (this.isTitle)
		{
			this.DSAID();
			try
			{
				if (SteamClient.RestartAppIfNecessary(1697880U) && PlayerPrefs.GetInt("CurrentScene") == 0)
				{
					Application.Quit();
				}
			}
			catch (DllNotFoundException arg)
			{
				Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + arg, this);
				Application.Quit();
			}
		}
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0006CD90 File Offset: 0x0006AF90
	private void Start()
	{
		this.connected = true;
		if (this.isTitle)
		{
			try
			{
				SteamClient.Init(1697880U, true);
				this.connected = true;
				Debug.Log("steam connected");
				this.IsThisAchievementUnlocked("ACH_TWIN");
			}
			catch (Exception)
			{
				if (PlayerPrefs.GetInt("CurrentScene") == 0)
				{
					this.connected = false;
					Debug.Log("steam not connected");
					Application.Quit();
				}
			}
			PlayerPrefs.SetInt("CurrentScene", 0);
			return;
		}
		PlayerPrefs.SetInt("CurrentScene", 1);
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0006CE24 File Offset: 0x0006B024
	private void DSAID()
	{
		if (Application.isEditor)
		{
			return;
		}
		if (File.Exists("steam_appid.txt"))
		{
			int num = PlayerPrefs.GetInt("LaunchCount", 0);
			num++;
			PlayerPrefs.SetInt("LaunchCount", num);
			if (num > 5)
			{
				try
				{
					File.Delete("steam_appid.txt");
				}
				catch (Exception)
				{
				}
				File.Exists("steam_appid.txt");
			}
		}
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0006CE90 File Offset: 0x0006B090
	private void OnApplicationQuit()
	{
		try
		{
			SteamClient.Shutdown();
		}
		catch
		{
		}
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x0006CEB8 File Offset: 0x0006B0B8
	private void PrintName()
	{
		Debug.Log(SteamClient.Name);
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x0006CEC4 File Offset: 0x0006B0C4
	public void IsThisAchievementUnlocked(string id)
	{
		Achievement achievement = new Achievement(id);
		Debug.Log("Achievement " + id + " status:" + achievement.State.ToString());
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0006CF00 File Offset: 0x0006B100
	public void UnlockAchievement(string id)
	{
		if (this.connected)
		{
			Achievement achievement = new Achievement(id);
			achievement.Trigger(true);
			Debug.Log("Achievement " + id + " unlocked");
		}
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0006CF3C File Offset: 0x0006B13C
	public void ClearAchievementStatus(string id)
	{
		Achievement achievement = new Achievement(id);
		achievement.Clear();
		Debug.Log("Achievement " + id + " cleared");
	}

	// Token: 0x0400131E RID: 4894
	private bool connected;

	// Token: 0x0400131F RID: 4895
	public bool isTitle;
}
