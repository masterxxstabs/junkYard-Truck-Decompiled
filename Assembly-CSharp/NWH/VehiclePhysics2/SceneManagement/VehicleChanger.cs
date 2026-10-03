using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Input;
using UnityEngine;

namespace NWH.VehiclePhysics2.SceneManagement
{
	// Token: 0x020002C5 RID: 709
	public class VehicleChanger : MonoBehaviour
	{
		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06001332 RID: 4914 RVA: 0x000CA5BF File Offset: 0x000C87BF
		// (set) Token: 0x06001333 RID: 4915 RVA: 0x000CA5C6 File Offset: 0x000C87C6
		public static VehicleChanger Instance { get; private set; }

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06001334 RID: 4916 RVA: 0x000CA5CE File Offset: 0x000C87CE
		// (set) Token: 0x06001335 RID: 4917 RVA: 0x000CA5D5 File Offset: 0x000C87D5
		public static VehicleController ActiveVehicleController { get; private set; }

		// Token: 0x06001336 RID: 4918 RVA: 0x000CA5DD File Offset: 0x000C87DD
		private void Awake()
		{
			VehicleChanger.Instance = this;
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x000CA5E8 File Offset: 0x000C87E8
		private void Start()
		{
			if (this.vehicles.Count == 0)
			{
				this.FindVehicles();
			}
			if (this.deactivateAll)
			{
				this.DeactivateAllIncludingActive();
			}
			else
			{
				this.DeactivateAllExceptActive();
			}
			if (this.characterBased && CharacterVehicleChanger.Instance != null)
			{
				this.DeactivateAllIncludingActive();
			}
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x000CA63C File Offset: 0x000C883C
		private void Update()
		{
			if (!this.characterBased)
			{
				bool flag = false;
				try
				{
					flag = UnityEngine.Input.GetButtonDown("ChangeVehicle");
				}
				catch
				{
					flag = false;
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
					Debug.LogWarning("'ChangeVehicle' input binding is not set under Project Settings > Input, falling back to default. Check input section of manual on which input bindings need to be set up for NVP to work properly.");
				}
				if (flag)
				{
					this.NextVehicle();
				}
			}
			if (this.vehicles.Count > 0)
			{
				VehicleChanger.ActiveVehicleController = (this.deactivateAll ? null : this.vehicles[this.currentVehicleIndex]);
			}
			else
			{
				VehicleChanger.ActiveVehicleController = null;
			}
			if (this.deactivateAll)
			{
				for (int i = 0; i < this.vehicles.Count; i++)
				{
					if (this.vehicles[i].IsAwake)
					{
						this.vehicles[i].Sleep();
					}
				}
			}
		}

		// Token: 0x06001339 RID: 4921 RVA: 0x000CA748 File Offset: 0x000C8948
		public void ChangeVehicle(int index)
		{
			this.currentVehicleIndex = index;
			if (this.currentVehicleIndex >= this.vehicles.Count)
			{
				this.currentVehicleIndex = 0;
			}
			this.DeactivateAllExceptActive();
		}

		// Token: 0x0600133A RID: 4922 RVA: 0x000CA774 File Offset: 0x000C8974
		public VehicleController NearestVehicleFrom(GameObject go)
		{
			VehicleController result = null;
			int index = -1;
			float num = float.PositiveInfinity;
			if (this.vehicles.Count > 0)
			{
				for (int i = 0; i < this.vehicles.Count; i++)
				{
					if (this.vehicles[i].gameObject.activeInHierarchy)
					{
						float num2 = Vector3.Distance(go.transform.position, this.vehicles[i].transform.position);
						if (num2 < num)
						{
							index = i;
							num = num2;
						}
					}
				}
				result = this.vehicles[index];
			}
			return result;
		}

		// Token: 0x0600133B RID: 4923 RVA: 0x000CA808 File Offset: 0x000C8A08
		public void ChangeVehicle(VehicleController vc)
		{
			int num = this.vehicles.IndexOf(vc);
			if (num >= 0)
			{
				this.ChangeVehicle(num);
			}
		}

		// Token: 0x0600133C RID: 4924 RVA: 0x000CA82D File Offset: 0x000C8A2D
		public void NextVehicle()
		{
			if (this.vehicles.Count == 1)
			{
				return;
			}
			this.ChangeVehicle(this.currentVehicleIndex + 1);
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x000CA84C File Offset: 0x000C8A4C
		public void PreviousVehicle()
		{
			if (this.vehicles.Count == 1)
			{
				return;
			}
			int index = (this.currentVehicleIndex == 0) ? (this.vehicles.Count - 1) : (this.currentVehicleIndex - 1);
			this.ChangeVehicle(index);
		}

		// Token: 0x0600133E RID: 4926 RVA: 0x000CA890 File Offset: 0x000C8A90
		public void DeactivateAllExceptActive()
		{
			for (int i = 0; i < this.vehicles.Count; i++)
			{
				if (i == this.currentVehicleIndex && !this.deactivateAll)
				{
					this.vehicles[i].Wake();
				}
				else if (this.putOtherVehiclesToSleep)
				{
					this.vehicles[i].Sleep();
				}
			}
		}

		// Token: 0x0600133F RID: 4927 RVA: 0x000CA8F0 File Offset: 0x000C8AF0
		public void DeactivateAllIncludingActive()
		{
			for (int i = 0; i < this.vehicles.Count; i++)
			{
				this.vehicles[i].Sleep();
			}
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x000CA924 File Offset: 0x000C8B24
		public void FindVehicles()
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(this.vehicleTag);
			if (this.vehicles == null)
			{
				this.vehicles = new List<VehicleController>();
			}
			GameObject[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				VehicleController component = array2[i].GetComponent<VehicleController>();
				if (component != null)
				{
					this.vehicles.Add(component);
				}
			}
		}

		// Token: 0x040023A1 RID: 9121
		[Tooltip("Is vehicle changing character based? When true changing vehicles will require getting close to them\r\nto be able to enter, opposed to pressing a button to switch between vehicles.")]
		public bool characterBased;

		// Token: 0x040023A2 RID: 9122
		[Tooltip("    Index of the current vehicle in vehicles list.")]
		public int currentVehicleIndex;

		// Token: 0x040023A3 RID: 9123
		[Tooltip("If true no vehicle will be active. Used when character controller has focus instead of vehicle controller.")]
		public bool deactivateAll;

		// Token: 0x040023A4 RID: 9124
		[Tooltip("    Should the vehicles that the player is currently not using be put to sleep to improve performance?")]
		public bool putOtherVehiclesToSleep = true;

		// Token: 0x040023A5 RID: 9125
		[Tooltip("List of all of the vehicles that can be selected and driven in the scene. If set to 0 script will try to auto-find all the vehicles in the scene with a tag define by VehiclesTag parameter.")]
		[SerializeField]
		public List<VehicleController> vehicles = new List<VehicleController>();

		// Token: 0x040023A6 RID: 9126
		[Tooltip("Tag that the script will search for if vehicles list is empty. Can be left empty if vehicles have already been assigned manually.")]
		public string vehicleTag = "Vehicle";
	}
}
