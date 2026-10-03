using System;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Dark
{
	// Token: 0x02000313 RID: 787
	public class GamepadChecker : MonoBehaviour
	{
		// Token: 0x06001446 RID: 5190 RVA: 0x000D9FD0 File Offset: 0x000D81D0
		private void Start()
		{
			this.checkerScript = base.gameObject.GetComponent<GamepadChecker>();
			if (!this.alwaysSearch)
			{
				this.checkerScript.enabled = false;
			}
			else
			{
				this.checkerScript.enabled = true;
				Debug.Log("Always Search is on. Input device will be updated in case of disconnecting/connecting.");
			}
			for (int i = 0; i < this.gamepadObjects.Count; i++)
			{
				this.gamepadObjects[i].SetActive(true);
			}
			for (int j = 0; j < this.gamepadObjects.Count; j++)
			{
				this.gamepadObjects[j].SetActive(false);
			}
			for (int k = 0; k < this.keyboardObjects.Count; k++)
			{
				this.keyboardObjects[k].SetActive(true);
			}
			for (int l = 0; l < this.keyboardObjects.Count; l++)
			{
				this.keyboardObjects[l].SetActive(false);
			}
			this.SwitchToKeyboard();
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x000DA0C0 File Offset: 0x000D82C0
		private void Update()
		{
			string[] joystickNames = Input.GetJoystickNames();
			for (int i = 0; i < joystickNames.Length; i++)
			{
				if (joystickNames[i].Length >= 1)
				{
					this.GamepadConnected = 1;
				}
				else if (joystickNames[i].Length == 0)
				{
					this.GamepadConnected = 0;
				}
			}
			if (this.GamepadConnected == 1 && !this.gamepadEnabled)
			{
				this.SwitchToController();
				return;
			}
			if (this.GamepadConnected == 0 && this.gamepadEnabled)
			{
				this.SwitchToKeyboard();
			}
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x000DA134 File Offset: 0x000D8334
		public void SwitchToController()
		{
			for (int i = 0; i < this.keyboardObjects.Count; i++)
			{
				this.keyboardObjects[i].SetActive(false);
			}
			for (int j = 0; j < this.gamepadObjects.Count; j++)
			{
				this.gamepadObjects[j].SetActive(true);
			}
			this.gamepadEnabled = true;
			this.eventSystem.SetActive(false);
			this.virtualCursor.SetActive(true);
			Debug.Log("Gamepad detected. Switching to gamepad input.");
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x000DA1BC File Offset: 0x000D83BC
		public void SwitchToKeyboard()
		{
			for (int i = 0; i < this.keyboardObjects.Count; i++)
			{
				this.keyboardObjects[i].SetActive(true);
			}
			for (int j = 0; j < this.gamepadObjects.Count; j++)
			{
				this.gamepadObjects[j].SetActive(false);
			}
			this.gamepadEnabled = false;
			this.virtualCursor.SetActive(false);
			this.eventSystem.SetActive(true);
			Debug.Log("No gamepad detected. Switching to keyboard input.");
		}

		// Token: 0x040024A2 RID: 9378
		[Header("RESOURCES")]
		public GameObject virtualCursor;

		// Token: 0x040024A3 RID: 9379
		public GameObject eventSystem;

		// Token: 0x040024A4 RID: 9380
		[Header("OBJECTS")]
		[Tooltip("Objects in this list will be active when gamepad is un-plugged.")]
		public List<GameObject> keyboardObjects = new List<GameObject>();

		// Token: 0x040024A5 RID: 9381
		[Tooltip("Objects in this list will be active when gamepad is plugged.")]
		public List<GameObject> gamepadObjects = new List<GameObject>();

		// Token: 0x040024A6 RID: 9382
		[Header("SETTINGS")]
		[Tooltip("Always update input device. If you turn off this feature, you won't able to change the input device after start, but it might increase the performance.")]
		public bool alwaysSearch;

		// Token: 0x040024A7 RID: 9383
		private GamepadChecker checkerScript;

		// Token: 0x040024A8 RID: 9384
		private int GamepadConnected;

		// Token: 0x040024A9 RID: 9385
		private Vector3 startMousePos;

		// Token: 0x040024AA RID: 9386
		private Vector3 startPos;

		// Token: 0x040024AB RID: 9387
		private bool gamepadEnabled;
	}
}
