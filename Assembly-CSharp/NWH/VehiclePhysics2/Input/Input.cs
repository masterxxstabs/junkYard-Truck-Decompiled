using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
	// Token: 0x020002BD RID: 701
	[Serializable]
	public class Input : VehicleComponent
	{
		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060012A5 RID: 4773 RVA: 0x000C8DD9 File Offset: 0x000C6FD9
		// (set) Token: 0x060012A6 RID: 4774 RVA: 0x000C8DE6 File Offset: 0x000C6FE6
		public float Clutch
		{
			get
			{
				return this.states.clutch;
			}
			set
			{
				this.states.clutch = ((value < 0f) ? 0f : ((value > 1f) ? 1f : value));
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x060012A7 RID: 4775 RVA: 0x000C8E12 File Offset: 0x000C7012
		// (set) Token: 0x060012A8 RID: 4776 RVA: 0x000C8E1F File Offset: 0x000C701F
		public bool EngineStartStop
		{
			get
			{
				return this.states.engineStartStop;
			}
			set
			{
				this.states.engineStartStop = (value || this.states.engineStartStop);
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x060012A9 RID: 4777 RVA: 0x000C8E3D File Offset: 0x000C703D
		// (set) Token: 0x060012AA RID: 4778 RVA: 0x000C8E4A File Offset: 0x000C704A
		public bool ExtraLights
		{
			get
			{
				return this.states.extraLights;
			}
			set
			{
				this.states.extraLights = value;
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x060012AB RID: 4779 RVA: 0x000C8E58 File Offset: 0x000C7058
		// (set) Token: 0x060012AC RID: 4780 RVA: 0x000C8E65 File Offset: 0x000C7065
		public bool HighBeamLights
		{
			get
			{
				return this.states.highBeamLights;
			}
			set
			{
				this.states.highBeamLights = value;
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x060012AD RID: 4781 RVA: 0x000C8E73 File Offset: 0x000C7073
		// (set) Token: 0x060012AE RID: 4782 RVA: 0x000C8E80 File Offset: 0x000C7080
		public float Handbrake
		{
			get
			{
				return this.states.handbrake;
			}
			set
			{
				this.states.handbrake = ((value < 0f) ? 0f : ((value > 1f) ? 1f : value));
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x060012AF RID: 4783 RVA: 0x000C8EAC File Offset: 0x000C70AC
		// (set) Token: 0x060012B0 RID: 4784 RVA: 0x000C8EB9 File Offset: 0x000C70B9
		public bool HazardLights
		{
			get
			{
				return this.states.hazardLights;
			}
			set
			{
				this.states.hazardLights = value;
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x060012B1 RID: 4785 RVA: 0x000C8EC7 File Offset: 0x000C70C7
		// (set) Token: 0x060012B2 RID: 4786 RVA: 0x000C8ED4 File Offset: 0x000C70D4
		public float Horizontal
		{
			get
			{
				return this.states.horizontal;
			}
			set
			{
				this.states.horizontal = ((value < -1f) ? -1f : ((value > 1f) ? 1f : value));
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x060012B3 RID: 4787 RVA: 0x000C8F00 File Offset: 0x000C7100
		// (set) Token: 0x060012B4 RID: 4788 RVA: 0x000C8F0D File Offset: 0x000C710D
		public bool Horn
		{
			get
			{
				return this.states.horn;
			}
			set
			{
				this.states.horn = value;
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x060012B5 RID: 4789 RVA: 0x000C8F1B File Offset: 0x000C711B
		// (set) Token: 0x060012B6 RID: 4790 RVA: 0x000C8F28 File Offset: 0x000C7128
		public bool LeftBlinker
		{
			get
			{
				return this.states.leftBlinker;
			}
			set
			{
				this.states.leftBlinker = value;
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x060012B7 RID: 4791 RVA: 0x000C8F36 File Offset: 0x000C7136
		// (set) Token: 0x060012B8 RID: 4792 RVA: 0x000C8F43 File Offset: 0x000C7143
		public bool LowBeamLights
		{
			get
			{
				return this.states.lowBeamLights;
			}
			set
			{
				this.states.lowBeamLights = value;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x060012B9 RID: 4793 RVA: 0x000C8F51 File Offset: 0x000C7151
		// (set) Token: 0x060012BA RID: 4794 RVA: 0x000C8F5E File Offset: 0x000C715E
		public bool RightBlinker
		{
			get
			{
				return this.states.rightBlinker;
			}
			set
			{
				this.states.rightBlinker = value;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x060012BB RID: 4795 RVA: 0x000C8F6C File Offset: 0x000C716C
		// (set) Token: 0x060012BC RID: 4796 RVA: 0x000C8F79 File Offset: 0x000C7179
		public bool ShiftDown
		{
			get
			{
				return this.states.shiftDown;
			}
			set
			{
				this.states.shiftDown = (value || this.states.shiftDown);
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x060012BD RID: 4797 RVA: 0x000C8F97 File Offset: 0x000C7197
		// (set) Token: 0x060012BE RID: 4798 RVA: 0x000C8FA4 File Offset: 0x000C71A4
		public int ShiftInto
		{
			get
			{
				return this.states.shiftInto;
			}
			set
			{
				this.states.shiftInto = value;
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x060012BF RID: 4799 RVA: 0x000C8FB2 File Offset: 0x000C71B2
		// (set) Token: 0x060012C0 RID: 4800 RVA: 0x000C8FBF File Offset: 0x000C71BF
		public bool ShiftUp
		{
			get
			{
				return this.states.shiftUp;
			}
			set
			{
				this.states.shiftUp = (value || this.states.shiftUp);
			}
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060012C1 RID: 4801 RVA: 0x000C8FDD File Offset: 0x000C71DD
		// (set) Token: 0x060012C2 RID: 4802 RVA: 0x000C8FEA File Offset: 0x000C71EA
		public bool TrailerAttachDetach
		{
			get
			{
				return this.states.trailerAttachDetach;
			}
			set
			{
				this.states.trailerAttachDetach = (value || this.states.trailerAttachDetach);
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x060012C3 RID: 4803 RVA: 0x000C9008 File Offset: 0x000C7208
		// (set) Token: 0x060012C4 RID: 4804 RVA: 0x000C9015 File Offset: 0x000C7215
		public float Vertical
		{
			get
			{
				return this.states.vertical;
			}
			set
			{
				this.states.vertical = ((value < -1f) ? -1f : ((value > 1f) ? 1f : value));
			}
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x060012C5 RID: 4805 RVA: 0x000C9041 File Offset: 0x000C7241
		// (set) Token: 0x060012C6 RID: 4806 RVA: 0x000C904E File Offset: 0x000C724E
		public bool CruiseControl
		{
			get
			{
				return this.states.cruiseControl;
			}
			set
			{
				this.states.cruiseControl = value;
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x060012C7 RID: 4807 RVA: 0x000C905C File Offset: 0x000C725C
		// (set) Token: 0x060012C8 RID: 4808 RVA: 0x000C9069 File Offset: 0x000C7269
		public bool Boost
		{
			get
			{
				return this.states.boost;
			}
			set
			{
				this.states.boost = value;
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x060012C9 RID: 4809 RVA: 0x000C9077 File Offset: 0x000C7277
		// (set) Token: 0x060012CA RID: 4810 RVA: 0x000C9084 File Offset: 0x000C7284
		public bool FlipOver
		{
			get
			{
				return this.states.flipOver;
			}
			set
			{
				this.states.flipOver = value;
			}
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x000C9092 File Offset: 0x000C7292
		public override void Initialize()
		{
			this._inputProviders = InputProvider.Instances;
			if (this._inputProviders == null || this._inputProviders.Count == 0)
			{
				Debug.LogWarning("No InputProviders are present in the scene. Make sure that one or more InputProviders are present (DesktopInputProvider, MobileInputProvider, etc.).");
				return;
			}
			this.initialized = true;
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x000C90C6 File Offset: 0x000C72C6
		public void ResetEngineStartStopFlag()
		{
			this.states.engineStartStop = false;
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x000C90D4 File Offset: 0x000C72D4
		public override void Update()
		{
			if (!base.Active || !this.autoSettable)
			{
				return;
			}
			this.Horizontal = this.GetHorizontal();
			this.Vertical = this.GetVertical();
			this.Clutch = this.GetClutch();
			this.Handbrake = this.GetHandbrake();
			this.ShiftInto = this.GetShiftInto();
			bool flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ShiftUp())
					{
						flag = true;
						break;
					}
				}
			}
			this.ShiftUp = flag;
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ShiftDown())
					{
						flag = true;
						break;
					}
				}
			}
			this.ShiftDown = flag;
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.LeftBlinker())
					{
						flag = true;
						break;
					}
				}
			}
			this.LeftBlinker = (flag ? (!this.LeftBlinker) : this.LeftBlinker);
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.RightBlinker())
					{
						flag = true;
						break;
					}
				}
			}
			this.RightBlinker = (flag ? (!this.RightBlinker) : this.RightBlinker);
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.LowBeamLights())
					{
						flag = true;
						break;
					}
				}
			}
			this.LowBeamLights = (flag ? (!this.LowBeamLights) : this.LowBeamLights);
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.HighBeamLights())
					{
						flag = true;
						break;
					}
				}
			}
			this.HighBeamLights = (flag ? (!this.HighBeamLights) : this.HighBeamLights);
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.HazardLights())
					{
						flag = true;
						break;
					}
				}
			}
			this.HazardLights = (flag ? (!this.HazardLights) : this.HazardLights);
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ExtraLights())
					{
						flag = true;
						break;
					}
				}
			}
			this.ExtraLights = (flag ? (!this.ExtraLights) : this.ExtraLights);
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.TrailerAttachDetach())
					{
						flag = true;
						break;
					}
				}
			}
			this.TrailerAttachDetach = flag;
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Horn())
					{
						flag = true;
						break;
					}
				}
			}
			this.Horn = flag;
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.EngineStartStop())
					{
						flag = true;
						break;
					}
				}
			}
			this.EngineStartStop = flag;
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CruiseControl())
					{
						flag = true;
						break;
					}
				}
			}
			this.CruiseControl = flag;
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Boost())
					{
						flag = true;
						break;
					}
				}
			}
			this.Boost = flag;
			flag = false;
			using (List<InputProvider>.Enumerator enumerator = this._inputProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.FlipOver())
					{
						flag = true;
						break;
					}
				}
			}
			this.FlipOver = flag;
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x000C95E8 File Offset: 0x000C77E8
		private int GetShiftInto()
		{
			foreach (InputProvider inputProvider in this._inputProviders)
			{
				if (inputProvider.ShiftInto() > -998)
				{
					return inputProvider.ShiftInto();
				}
			}
			return -999;
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x000C9654 File Offset: 0x000C7854
		private float GetHorizontal()
		{
			float num = 0f;
			foreach (InputProvider inputProvider in this._inputProviders)
			{
				num += inputProvider.Horizontal();
			}
			return num;
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x000C96B0 File Offset: 0x000C78B0
		private float GetVertical()
		{
			float num = 0f;
			foreach (InputProvider inputProvider in this._inputProviders)
			{
				num += inputProvider.Vertical();
			}
			return num;
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x000C970C File Offset: 0x000C790C
		private float GetClutch()
		{
			float num = 0f;
			foreach (InputProvider inputProvider in this._inputProviders)
			{
				num += inputProvider.Clutch();
			}
			return num;
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x000C9768 File Offset: 0x000C7968
		private float GetHandbrake()
		{
			float num = 0f;
			foreach (InputProvider inputProvider in this._inputProviders)
			{
				num += inputProvider.Handbrake();
			}
			return num;
		}

		// Token: 0x060012D4 RID: 4820 RVA: 0x000C97C4 File Offset: 0x000C79C4
		public override void Disable()
		{
			base.Disable();
			this.states.Reset();
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x000C97D7 File Offset: 0x000C79D7
		public void ResetShiftDownFlag()
		{
			this.states.shiftDown = false;
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x000C97E5 File Offset: 0x000C79E5
		public void ResetShiftUpFlag()
		{
			this.states.shiftUp = false;
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x000C97F3 File Offset: 0x000C79F3
		public void ResetTrailerAttachDetachFlag()
		{
			this.states.trailerAttachDetach = false;
		}

		// Token: 0x04002361 RID: 9057
		public bool autoSettable = true;

		// Token: 0x04002362 RID: 9058
		public InputStates states;

		// Token: 0x04002363 RID: 9059
		public Input.ThrottleType throttleType;

		// Token: 0x04002364 RID: 9060
		private List<InputProvider> _inputProviders = new List<InputProvider>();

		// Token: 0x020004CA RID: 1226
		public enum ThrottleType
		{
			// Token: 0x04002C2E RID: 11310
			WForwardSReverse,
			// Token: 0x04002C2F RID: 11311
			WForwardWReverse
		}

		// Token: 0x020004CB RID: 1227
		// (Invoke) Token: 0x06001B34 RID: 6964
		private delegate bool BinaryInputDelegate();
	}
}
