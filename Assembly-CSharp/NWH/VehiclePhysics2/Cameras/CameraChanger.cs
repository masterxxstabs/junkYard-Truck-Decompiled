using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Input;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Cameras
{
	// Token: 0x020002C6 RID: 710
	public class CameraChanger : MonoBehaviour
	{
		// Token: 0x06001342 RID: 4930 RVA: 0x000CA9A4 File Offset: 0x000C8BA4
		private void Awake()
		{
			this._vehicleController = base.GetComponentInParent<VehicleController>();
			if (this._vehicleController == null)
			{
				Debug.LogError("None of the parent objects of CameraChanger contain VehicleController.");
			}
			this._vehicleController.onWake.AddListener(new UnityAction(this.OnVehicleWake));
			this._vehicleController.onSleep.AddListener(new UnityAction(this.OnVehicleSleep));
			if (this._vehicleController == null)
			{
				Debug.Log("None of the parents of camera changer contain VehicleController component. Make sure that the camera changer is amongst the children of VehicleController object.");
			}
			if (this.autoFindCameras)
			{
				this.vehicleCameras = new List<GameObject>();
				foreach (Camera camera in base.GetComponentsInChildren<Camera>(true))
				{
					this.vehicleCameras.Add(camera.gameObject);
				}
			}
			if (this.vehicleCameras.Count == 0)
			{
				Debug.LogWarning("No cameras could. Either add cameras manually or add them as children to the game object this script is attached to.");
			}
			if (!this._vehicleController.IsAwake || this._vehicleController.multiplayerInstanceType == VehicleController.MultiplayerInstanceType.Remote)
			{
				this.DisableAllCameras();
				return;
			}
			this.EnableCurrentDisableOthers();
			this.CheckIfInside();
		}

		// Token: 0x06001343 RID: 4931 RVA: 0x000CAAA8 File Offset: 0x000C8CA8
		private void Update()
		{
			if (this._vehicleController.IsAwake && this._vehicleController.multiplayerInstanceType == VehicleController.MultiplayerInstanceType.Local && InputProvider.Instances.Count > 0)
			{
				bool flag = false;
				using (List<InputProvider>.Enumerator enumerator = InputProvider.Instances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.ChangeCamera())
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					this.NextCamera();
					this.CheckIfInside();
				}
			}
		}

		// Token: 0x06001344 RID: 4932 RVA: 0x000CAB38 File Offset: 0x000C8D38
		private void EnableCurrentDisableOthers()
		{
			int count = this.vehicleCameras.Count;
			for (int i = 0; i < count; i++)
			{
				if (!(this.vehicleCameras[i] == null))
				{
					if (i == this.currentCameraIndex)
					{
						this.vehicleCameras[i].SetActive(true);
						AudioListener component = this.vehicleCameras[i].GetComponent<AudioListener>();
						if (component != null)
						{
							component.enabled = true;
						}
					}
					else
					{
						this.vehicleCameras[i].SetActive(false);
						AudioListener component2 = this.vehicleCameras[i].GetComponent<AudioListener>();
						if (component2 != null)
						{
							component2.enabled = false;
						}
					}
				}
			}
		}

		// Token: 0x06001345 RID: 4933 RVA: 0x000CABEC File Offset: 0x000C8DEC
		private void DisableAllCameras()
		{
			int count = this.vehicleCameras.Count;
			for (int i = 0; i < count; i++)
			{
				this.vehicleCameras[i].SetActive(false);
				AudioListener component = this.vehicleCameras[i].GetComponent<AudioListener>();
				if (component != null)
				{
					component.enabled = true;
				}
			}
		}

		// Token: 0x06001346 RID: 4934 RVA: 0x000CAC45 File Offset: 0x000C8E45
		public void NextCamera()
		{
			if (this.vehicleCameras.Count <= 0)
			{
				return;
			}
			this.currentCameraIndex++;
			if (this.currentCameraIndex >= this.vehicleCameras.Count)
			{
				this.currentCameraIndex = 0;
			}
			this.EnableCurrentDisableOthers();
		}

		// Token: 0x06001347 RID: 4935 RVA: 0x000CAC84 File Offset: 0x000C8E84
		public void PreviousCamera()
		{
			if (this.vehicleCameras.Count <= 0)
			{
				return;
			}
			this.currentCameraIndex--;
			if (this.currentCameraIndex < 0)
			{
				this.currentCameraIndex = this.vehicleCameras.Count - 1;
			}
			this.EnableCurrentDisableOthers();
		}

		// Token: 0x06001348 RID: 4936 RVA: 0x000CACD0 File Offset: 0x000C8ED0
		private void CheckIfInside()
		{
			if (this.vehicleCameras.Count == 0 || this.vehicleCameras[this.currentCameraIndex] == null)
			{
				return;
			}
			this._cis = this.vehicleCameras[this.currentCameraIndex].gameObject.GetComponent<CameraInsideVehicle>();
			if (this._cis != null && this._cis.isInsideVehicle)
			{
				this._vehicleController.soundManager.insideVehicle = true;
				return;
			}
			this._vehicleController.soundManager.insideVehicle = false;
		}

		// Token: 0x06001349 RID: 4937 RVA: 0x000CAD63 File Offset: 0x000C8F63
		private void OnVehicleWake()
		{
			this.EnableCurrentDisableOthers();
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x000CAD6B File Offset: 0x000C8F6B
		private void OnVehicleSleep()
		{
			this.DisableAllCameras();
		}

		// Token: 0x040023A9 RID: 9129
		[Tooltip("    If true vehicleCameras list will be filled through cameraTag.")]
		public bool autoFindCameras = true;

		// Token: 0x040023AA RID: 9130
		[Tooltip("    Index of the camera from vehicle cameras list that will be active first.")]
		public int currentCameraIndex;

		// Token: 0x040023AB RID: 9131
		[Tooltip("List of cameras that the changer will cycle through. Leave empty if you want cameras to be automatically detected. To be detected cameras need to have camera tag and be children of the object this script is attached to.")]
		public List<GameObject> vehicleCameras = new List<GameObject>();

		// Token: 0x040023AC RID: 9132
		private CameraInsideVehicle _cis;

		// Token: 0x040023AD RID: 9133
		private int _previousCamera;

		// Token: 0x040023AE RID: 9134
		private VehicleController _vehicleController;
	}
}
