using System;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x0200034B RID: 843
	public class uTirePressureDebugger : MonoBehaviour
	{
		// Token: 0x0600159D RID: 5533 RVA: 0x000E10BE File Offset: 0x000DF2BE
		private void Start()
		{
			if (this.cam == null)
			{
				this.cam = Camera.main;
			}
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x000E10DC File Offset: 0x000DF2DC
		private void Update()
		{
			if (Input.GetKeyDown(this.toggleDebugger))
			{
				this.state = !this.state;
			}
			if (Input.GetKeyDown(this.pauseTimeKey))
			{
				Time.timeScale = ((Time.timeScale > 0f) ? 0f : 1f);
			}
			if (Input.GetKeyDown(this.slowTime))
			{
				Time.timeScale = ((Time.timeScale == 0.3f) ? 1f : 0.3f);
			}
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x000E115C File Offset: 0x000DF35C
		private void OnGUI()
		{
			if (!this.state)
			{
				return;
			}
			this.centeredStyle = GUI.skin.GetStyle("Label");
			this.centeredStyle.alignment = TextAnchor.MiddleCenter;
			foreach (Vehicle vehicle in Singleton<uTireManager>.Instance.vehicles)
			{
				foreach (WheelMeshConnection wheelMeshConnection in vehicle.wheels)
				{
					GUI.color = Color.red;
					if (wheelMeshConnection.flatness < 0.6f)
					{
						GUI.color = Color.yellow;
					}
					if (wheelMeshConnection.flatness < 0.3f)
					{
						GUI.color = Color.green;
					}
					Vector3 vector = this.cam.WorldToScreenPoint(wheelMeshConnection.meshRenderer.transform.position);
					if (wheelMeshConnection.meshRenderer.isVisible)
					{
						Rect position = new Rect(vector.x - 30f, (float)Screen.height - vector.y, 60f, 20f);
						GUI.Box(position, "");
						GUI.Label(position, string.Format("{0:0.00}", wheelMeshConnection.flatness), this.centeredStyle);
					}
				}
				Rect position2 = new Rect(5f, (float)(Screen.height - 65), 180f, 60f);
				GUI.Box(position2, "");
				GUI.color = Color.white;
				GUI.Label(position2, string.Format("Press {0} to pause/start time \nPress {1} to slow down time", this.pauseTimeKey.ToString(), this.slowTime.ToString()));
			}
		}

		// Token: 0x060015A0 RID: 5536 RVA: 0x000E1320 File Offset: 0x000DF520
		public void toggleState()
		{
			this.state = !this.state;
		}

		// Token: 0x04002636 RID: 9782
		public Camera cam;

		// Token: 0x04002637 RID: 9783
		public KeyCode toggleDebugger = KeyCode.F5;

		// Token: 0x04002638 RID: 9784
		public KeyCode pauseTimeKey = KeyCode.Q;

		// Token: 0x04002639 RID: 9785
		public KeyCode slowTime = KeyCode.R;

		// Token: 0x0400263A RID: 9786
		private GUIStyle centeredStyle;

		// Token: 0x0400263B RID: 9787
		private bool state = true;
	}
}
