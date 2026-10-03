using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Input;
using NWH.VehiclePhysics2.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2
{
	// Token: 0x02000251 RID: 593
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VehicleChanger))]
	public class CharacterVehicleChanger : MonoBehaviour
	{
		// Token: 0x06000F66 RID: 3942 RVA: 0x000B5EF6 File Offset: 0x000B40F6
		public CharacterVehicleChanger(VehicleController nearestVehicle)
		{
			this._nearestVehicle = nearestVehicle;
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x000B5F26 File Offset: 0x000B4126
		private void Awake()
		{
			CharacterVehicleChanger.Instance = this;
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x000B5F2E File Offset: 0x000B412E
		private void Start()
		{
			if (base.enabled)
			{
				VehicleChanger.Instance.characterBased = true;
				VehicleChanger.Instance.deactivateAll = true;
			}
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x000B5F50 File Offset: 0x000B4150
		private void Update()
		{
			if (!this._insideVehicle)
			{
				this.nearVehicle = false;
				if (!this.characterObject.activeSelf)
				{
					this.characterObject.SetActive(true);
				}
				this._enterExitPoints = GameObject.FindGameObjectsWithTag(this.enterExitTag);
				this._nearestEnterExitPoint = null;
				float num = float.PositiveInfinity;
				foreach (GameObject gameObject in this._enterExitPoints)
				{
					float num2 = Vector3.SqrMagnitude(this.characterObject.transform.position - gameObject.transform.position);
					if (num2 < num)
					{
						num = num2;
						this._nearestEnterExitPoint = gameObject;
					}
				}
				if (this._nearestEnterExitPoint == null)
				{
					return;
				}
				if (Vector3.Magnitude(Vector3.ProjectOnPlane(this._nearestEnterExitPoint.transform.position - this.characterObject.transform.position, Vector3.up)) < this.enterDistance)
				{
					this.nearVehicle = true;
					this._nearestVehicle = this._nearestEnterExitPoint.GetComponentInParent<VehicleController>();
				}
			}
			bool flag = false;
			using (List<InputProvider>.Enumerator enumerator = InputProvider.Instances.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ChangeVehicle())
					{
						flag = true;
						break;
					}
				}
			}
			if (InputProvider.Instances.Count > 0 && flag)
			{
				this.EnterExitVehicle();
			}
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x000B60C0 File Offset: 0x000B42C0
		public void EnterExitVehicle()
		{
			if (this.nearVehicle && !this._insideVehicle && this._nearestVehicle.Speed < this.maxEnterExitVehicleSpeed)
			{
				this.characterObject.SetActive(false);
				VehicleChanger.Instance.deactivateAll = false;
				this._relativeEnterPosition = this._nearestVehicle.transform.InverseTransformPoint(this.characterObject.transform.position);
				this._insideVehicle = true;
				VehicleChanger.Instance.ChangeVehicle(this._nearestVehicle);
				this.nearVehicle = false;
				return;
			}
			if (this._insideVehicle && this._nearestVehicle.Speed < this.maxEnterExitVehicleSpeed)
			{
				VehicleChanger.Instance.DeactivateAllIncludingActive();
				VehicleChanger.Instance.deactivateAll = true;
				this._insideVehicle = false;
				this.characterObject.transform.position = this._nearestVehicle.transform.TransformPoint(this._relativeEnterPosition);
				this.characterObject.SetActive(true);
			}
		}

		// Token: 0x04001FE7 RID: 8167
		public static CharacterVehicleChanger Instance;

		// Token: 0x04001FE8 RID: 8168
		[FormerlySerializedAs("characterControllerObject")]
		[Tooltip("    Game object representing a character. Can also be another vehicle.")]
		public GameObject characterObject;

		// Token: 0x04001FE9 RID: 8169
		[Range(0.2f, 3f)]
		[Tooltip("    Maximum distance at which the character will be able to enter the vehicle.")]
		public float enterDistance = 2f;

		// Token: 0x04001FEA RID: 8170
		[Tooltip("Tag of the object representing the point from which the enter distance will be measured. Useful if you want to enable you character to enter only when near the door.")]
		public string enterExitTag = "EnterExitPoint";

		// Token: 0x04001FEB RID: 8171
		[Tooltip("    Maximum speed at which the character will be able to enter / exit the vehicle.")]
		public float maxEnterExitVehicleSpeed = 2f;

		// Token: 0x04001FEC RID: 8172
		[Tooltip("    True when character can enter the vehicle.")]
		public bool nearVehicle;

		// Token: 0x04001FED RID: 8173
		private bool _insideVehicle;

		// Token: 0x04001FEE RID: 8174
		private GameObject _nearestEnterExitObject;

		// Token: 0x04001FEF RID: 8175
		private VehicleController _nearestVehicle;

		// Token: 0x04001FF0 RID: 8176
		private Vector3 _relativeEnterPosition;

		// Token: 0x04001FF1 RID: 8177
		private GameObject[] _enterExitPoints;

		// Token: 0x04001FF2 RID: 8178
		private GameObject _nearestEnterExitPoint;
	}
}
