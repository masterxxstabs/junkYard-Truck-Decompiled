using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Cameras;
using NWH.VehiclePhysics2.Input;
using NWH.WheelController3D;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.SetupWizard
{
	// Token: 0x02000278 RID: 632
	public class VehicleSetupWizard : MonoBehaviour
	{
		// Token: 0x060010A4 RID: 4260 RVA: 0x000BD4C4 File Offset: 0x000BB6C4
		public void RunSetup()
		{
			Debug.Log("======== VEHICLE SETUP START ========");
			if (base.transform.localScale != Vector3.one)
			{
				Debug.LogWarning("Scale of a parent object should be [1,1,1] for Rigidbody and VehicleController to function properly.");
				return;
			}
			base.gameObject.tag = "Vehicle";
			if (this.bodyMeshGameObject != null && this.bodyMeshGameObject.GetComponent<MeshCollider>() == null)
			{
				Debug.Log("Adding MeshCollider to body mesh object " + this.bodyMeshGameObject.name);
				this.bodyMeshGameObject.AddComponent<MeshCollider>().convex = true;
				Debug.Log("Setting layer of body collider to default layer 'Ignore Raycast' to prevent wheels from detecting the vehicle itself. If you wish to use some other layer check Ignore Layer settings (WheelController inspector).");
				this.bodyMeshGameObject.layer = 2;
			}
			if (base.GetComponent<Rigidbody>() == null)
			{
				Debug.Log("Adding Rigidbody to " + base.name);
				base.gameObject.AddComponent<Rigidbody>();
			}
			foreach (GameObject gameObject in this.wheelGameObjects)
			{
				string text = gameObject.name + "_WheelController";
				Debug.Log("Creating new WheelController object " + text);
				if (!base.transform.Find(text))
				{
					GameObject gameObject2 = new GameObject(text);
					gameObject2.transform.SetParent(base.transform);
					gameObject2.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
					gameObject2.transform.position += base.transform.up * 0.2f;
					Debug.Log("   |-> Adding WheelController to " + gameObject2.name);
					WheelController wheelController = gameObject2.AddComponent<WheelController>();
					wheelController.Visual = gameObject;
					MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
					if (component != null)
					{
						float y = component.bounds.extents.y;
						if (y < 0.05f || y > 1f)
						{
							Debug.LogWarning("Detected unusual wheel radius. Adjust WheelController's radius field manually.");
						}
						Debug.Log(string.Format("   |-> Setting radius to {0}", y));
						wheelController.wheel.radius = y;
						float num = component.bounds.extents.x * 2f;
						if (num < 0.02f || num > 1f)
						{
							Debug.LogWarning("Detected unusual wheel width. Adjust WheelController's width field manually.");
						}
						Debug.Log(string.Format("   |-> Setting width to {0}", num));
						wheelController.wheel.width = num;
					}
					else
					{
						Debug.LogWarning("Radius and width could not be auto configured. Wheel " + gameObject.name + " does not contain a MeshFilter.");
					}
				}
			}
			VehicleController vehicleController = base.GetComponent<VehicleController>();
			if (vehicleController == null)
			{
				Debug.Log("Adding VehicleController to " + base.name);
				vehicleController = base.gameObject.AddComponent<VehicleController>();
				vehicleController.SetDefaults();
			}
			if (this.addCamera)
			{
				Debug.Log("Adding CameraChanger.");
				GameObject gameObject3 = new GameObject("Cameras");
				gameObject3.transform.SetParent(base.transform);
				gameObject3.AddComponent<CameraChanger>();
				Debug.Log("Adding a camera follow.");
				GameObject gameObject4 = new GameObject("Vehicle Camera");
				gameObject4.transform.SetParent(gameObject3.transform);
				gameObject4.transform.SetPositionAndRotation(vehicleController.transform.position, vehicleController.transform.rotation);
				gameObject4.AddComponent<Camera>().fieldOfView = 80f;
				gameObject4.AddComponent<AudioListener>();
				CameraFollow cameraFollow = gameObject4.AddComponent<CameraFollow>();
				cameraFollow.target = vehicleController;
				cameraFollow.tag = "MainCamera";
			}
			if (this.addCharacterEnterExitPoints)
			{
				Debug.Log("Adding enter/exit points.");
				GameObject gameObject5 = new GameObject("LeftEnterExitPoint");
				GameObject gameObject6 = new GameObject("RightEnterExitPoint");
				gameObject5.transform.SetParent(base.transform);
				gameObject6.transform.SetParent(base.transform);
				gameObject5.transform.position = base.transform.position + base.transform.right;
				gameObject6.transform.position = base.transform.position - base.transform.right;
				gameObject5.tag = "EnterExitPoint";
				gameObject6.tag = "EnterExitPoint";
			}
			if (this.addInputProvider)
			{
				if (Object.FindObjectsOfType<InputProvider>().Length != 0)
				{
					Debug.LogWarning("InputProvider already present in scene. Skipping.");
				}
				else
				{
					Debug.Log("Adding input provider (DesktopInputProvider) to object 'VehicleSceneManager'");
					new GameObject("VehicleSceneManager").AddComponent<DesktopInputProvider>();
				}
			}
			Debug.Log("Validating setup.");
			vehicleController.Validate();
			Debug.Log("Setup done. Removing Wizard.");
			Debug.Log("======== VEHICLE SETUP END ========");
			Object.DestroyImmediate(this);
		}

		// Token: 0x040020FD RID: 8445
		public bool addCamera = true;

		// Token: 0x040020FE RID: 8446
		public bool addCharacterEnterExitPoints = true;

		// Token: 0x040020FF RID: 8447
		[FormerlySerializedAs("bodyMeshGO")]
		[FormerlySerializedAs("bodyMesh")]
		public bool addCollider;

		// Token: 0x04002100 RID: 8448
		public bool addInputProvider = true;

		// Token: 0x04002101 RID: 8449
		public GameObject bodyMeshGameObject;

		// Token: 0x04002102 RID: 8450
		public List<GameObject> wheelGameObjects = new List<GameObject>();

		// Token: 0x04002103 RID: 8451
		private GameObject _cameraParent;

		// Token: 0x04002104 RID: 8452
		private GameObject _wheelControllerParent;

		// Token: 0x04002105 RID: 8453
		private GameObject _wheelParent;
	}
}
