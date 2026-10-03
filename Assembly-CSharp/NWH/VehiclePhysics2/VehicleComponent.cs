using System;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
	// Token: 0x02000257 RID: 599
	[Serializable]
	public abstract class VehicleComponent
	{
		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000F83 RID: 3971 RVA: 0x000B704E File Offset: 0x000B524E
		public bool Active
		{
			get
			{
				return this.state.isOn && this.state.isEnabled && this.initialized;
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x000B7072 File Offset: 0x000B5272
		// (set) Token: 0x06000F85 RID: 3973 RVA: 0x000B707A File Offset: 0x000B527A
		public bool Initialized
		{
			get
			{
				return this.initialized;
			}
			set
			{
				this.initialized = value;
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x000B7083 File Offset: 0x000B5283
		// (set) Token: 0x06000F87 RID: 3975 RVA: 0x000B7090 File Offset: 0x000B5290
		public bool IsEnabled
		{
			get
			{
				return this.state.isEnabled;
			}
			private set
			{
				this.state.isEnabled = value;
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000F88 RID: 3976 RVA: 0x000B709E File Offset: 0x000B529E
		// (set) Token: 0x06000F89 RID: 3977 RVA: 0x000B70AB File Offset: 0x000B52AB
		public bool IsOn
		{
			get
			{
				return this.state.isOn;
			}
			set
			{
				this.state.isOn = value;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000F8A RID: 3978 RVA: 0x000B70B9 File Offset: 0x000B52B9
		// (set) Token: 0x06000F8B RID: 3979 RVA: 0x000B70C6 File Offset: 0x000B52C6
		public int LodIndex
		{
			get
			{
				return this.state.lodIndex;
			}
			set
			{
				this.state.lodIndex = value;
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000F8C RID: 3980 RVA: 0x000B70D4 File Offset: 0x000B52D4
		// (set) Token: 0x06000F8D RID: 3981 RVA: 0x000B70DC File Offset: 0x000B52DC
		public VehicleController VehicleController
		{
			get
			{
				return this.vc;
			}
			set
			{
				this.vc = value;
			}
		}

		// Token: 0x06000F8E RID: 3982
		public abstract void Initialize();

		// Token: 0x06000F8F RID: 3983 RVA: 0x000B70E8 File Offset: 0x000B52E8
		public virtual void Awake(VehicleController vc)
		{
			this.vc = vc;
			this.initialized = false;
			this.wasEnabled = false;
			this.fullTypeName = base.GetType().FullName;
			if (this.state == null)
			{
				this.state = new StateDefinition();
			}
			this.state.fullName = this.fullTypeName;
			this.LoadStateFromDefinitionsFile(this.fullTypeName, ref this.state);
		}

		// Token: 0x06000F90 RID: 3984
		public abstract void FixedUpdate();

		// Token: 0x06000F91 RID: 3985
		public abstract void Update();

		// Token: 0x06000F92 RID: 3986 RVA: 0x000B7151 File Offset: 0x000B5351
		public virtual void Enable()
		{
			if (!this.initialized && Application.isPlaying)
			{
				this.Initialize();
			}
			this.state.isEnabled = true;
			this.wasEnabled = true;
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x000B717B File Offset: 0x000B537B
		public virtual void Disable()
		{
			this.state.isEnabled = false;
			this.wasEnabled = false;
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x00002188 File Offset: 0x00000388
		public virtual void OnDrawGizmosSelected(VehicleController vc)
		{
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x000B7190 File Offset: 0x000B5390
		public void LoadStateFromDefinitionsFile(string fullTypeName, ref StateDefinition state)
		{
			if (this.vc.stateSettings == null)
			{
				return;
			}
			StateDefinition definition = this.vc.stateSettings.GetDefinition(fullTypeName);
			if (definition != null)
			{
				state.isOn = definition.isOn;
				state.isEnabled = definition.isEnabled;
				state.lodIndex = definition.lodIndex;
				state.fullName = fullTypeName;
				return;
			}
			Debug.LogWarning("State definition " + fullTypeName + " could not be loaded. Click on 'Refresh' button under Settings > State Settings.");
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x000B720C File Offset: 0x000B540C
		public virtual void CheckState(int lodIndex)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			if (!this.state.isOn)
			{
				if (this.state.isEnabled)
				{
					this.Disable();
				}
				return;
			}
			if (this.state.lodIndex >= 0)
			{
				if (this.state.lodIndex >= lodIndex)
				{
					if (!this.state.isEnabled)
					{
						this.Enable();
					}
				}
				else if (this.state.isEnabled)
				{
					this.Disable();
				}
			}
			else if (this.state.isEnabled && !this.wasEnabled)
			{
				this.Enable();
			}
			else if (!this.state.isEnabled && this.wasEnabled)
			{
				this.Disable();
			}
			this.wasEnabled = this.state.isEnabled;
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x00002188 File Offset: 0x00000388
		public virtual void SetDefaults(VehicleController vc)
		{
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x000B72D7 File Offset: 0x000B54D7
		public void ToggleState()
		{
			if (this.IsEnabled)
			{
				this.Disable();
				return;
			}
			this.Enable();
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x00002188 File Offset: 0x00000388
		public virtual void Validate(VehicleController vc)
		{
		}

		// Token: 0x04002018 RID: 8216
		[Tooltip("    Contains info about component's state.")]
		public StateDefinition state = new StateDefinition();

		// Token: 0x04002019 RID: 8217
		protected string fullTypeName;

		// Token: 0x0400201A RID: 8218
		protected bool initialized;

		// Token: 0x0400201B RID: 8219
		protected VehicleController vc;

		// Token: 0x0400201C RID: 8220
		protected bool wasEnabled;
	}
}
