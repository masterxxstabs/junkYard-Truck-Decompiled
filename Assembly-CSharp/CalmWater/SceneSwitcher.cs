using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CalmWater
{
	// Token: 0x0200032E RID: 814
	public class SceneSwitcher : MonoBehaviour
	{
		// Token: 0x060014D1 RID: 5329 RVA: 0x000DCE97 File Offset: 0x000DB097
		public void SwitchLevel(string level)
		{
			SceneManager.LoadScene(level);
		}
	}
}
