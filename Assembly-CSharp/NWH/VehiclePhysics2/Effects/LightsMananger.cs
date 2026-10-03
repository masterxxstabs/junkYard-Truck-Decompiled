using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002B5 RID: 693
	[Serializable]
	public class LightsMananger : Effect
	{
		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06001273 RID: 4723 RVA: 0x000C6723 File Offset: 0x000C4923
		public bool BlinkerState
		{
			get
			{
				return (int)(Time.realtimeSinceStartup * 2f) % 2 == 0;
			}
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x000C6738 File Offset: 0x000C4938
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.IsEnabled && this.vc != null)
			{
				if (this.brakeLights != null)
				{
					if (this.vc.brakes.IsBraking)
					{
						this.brakeLights.TurnOn();
					}
					else
					{
						this.brakeLights.TurnOff();
					}
				}
				if (this.reverseLights != null)
				{
					if (this.vc.powertrain.transmission.Gear < 0)
					{
						this.reverseLights.TurnOn();
					}
					else
					{
						this.reverseLights.TurnOff();
					}
				}
				if (this.tailLights != null && this.lowBeamLights != null)
				{
					if (this.vc.input.LowBeamLights)
					{
						this.tailLights.TurnOn();
						this.lowBeamLights.TurnOn();
					}
					else
					{
						this.tailLights.TurnOff();
						this.lowBeamLights.TurnOff();
						if (this.highBeamLights != null && this.highBeamLights.On)
						{
							this.highBeamLights.TurnOff();
							this.vc.input.HighBeamLights = false;
						}
					}
				}
				if (this.highBeamLights != null)
				{
					if (this.vc.input.HighBeamLights)
					{
						this.highBeamLights.TurnOn();
						if (this.lowBeamLights != null && this.tailLights != null && !this.vc.input.LowBeamLights)
						{
							this.lowBeamLights.TurnOn();
							this.tailLights.TurnOn();
							this.vc.input.LowBeamLights = true;
						}
					}
					else
					{
						this.highBeamLights.TurnOff();
					}
				}
				if (this.leftBlinkers != null && this.rightBlinkers != null)
				{
					if (this.vc.input.LeftBlinker)
					{
						if (this.vc.input.RightBlinker && this._rightBlinkerWasOn && !this._leftBlinkerWasOn)
						{
							this.vc.input.RightBlinker = false;
							this.rightBlinkers.TurnOff();
						}
						else if (this.BlinkerState)
						{
							this.leftBlinkers.TurnOn();
						}
						else
						{
							this.leftBlinkers.TurnOff();
						}
					}
					if (this.vc.input.RightBlinker)
					{
						if (this.vc.input.LeftBlinker && this._leftBlinkerWasOn && !this._rightBlinkerWasOn)
						{
							this.vc.input.LeftBlinker = false;
							this.leftBlinkers.TurnOff();
						}
						else if (this.BlinkerState)
						{
							this.rightBlinkers.TurnOn();
						}
						else
						{
							this.rightBlinkers.TurnOff();
						}
					}
					this._leftBlinkerWasOn = this.vc.input.LeftBlinker;
					this._rightBlinkerWasOn = this.vc.input.RightBlinker;
					if (this.vc.input.HazardLights)
					{
						if (this.BlinkerState)
						{
							this.leftBlinkers.TurnOn();
							this.rightBlinkers.TurnOn();
						}
						else
						{
							this.leftBlinkers.TurnOff();
							this.rightBlinkers.TurnOff();
						}
					}
					else
					{
						if (!this.vc.input.LeftBlinker)
						{
							this.leftBlinkers.TurnOff();
						}
						if (!this.vc.input.RightBlinker)
						{
							this.rightBlinkers.TurnOff();
						}
					}
				}
				if (this.extraLights != null && this.vc.input.ExtraLights)
				{
					if (this.extraLights.On)
					{
						this.rightBlinkers.TurnOff();
						return;
					}
					this.rightBlinkers.TurnOn();
				}
			}
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x000C6AB8 File Offset: 0x000C4CB8
		public override void Disable()
		{
			base.Disable();
			this.TurnOffAllLights();
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x000C6AC8 File Offset: 0x000C4CC8
		public byte GetByteState()
		{
			byte b = 0;
			if (this.brakeLights.On)
			{
				b |= 1;
			}
			if (this.tailLights.On)
			{
				b |= 2;
			}
			if (this.reverseLights.On)
			{
				b |= 4;
			}
			if (this.lowBeamLights.On)
			{
				b |= 8;
			}
			if (this.highBeamLights.On)
			{
				b |= 16;
			}
			if (this.leftBlinkers.On)
			{
				b |= 32;
			}
			if (this.rightBlinkers.On)
			{
				b |= 64;
			}
			if (this.extraLights.On)
			{
				b |= 128;
			}
			return b;
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x000C6B70 File Offset: 0x000C4D70
		public void SetByteState(byte state)
		{
			if ((state & 1) != 0)
			{
				this.brakeLights.TurnOn();
			}
			else
			{
				this.brakeLights.TurnOff();
			}
			if ((state & 2) != 0)
			{
				this.tailLights.TurnOn();
			}
			else
			{
				this.tailLights.TurnOff();
			}
			if ((state & 4) != 0)
			{
				this.reverseLights.TurnOn();
			}
			else
			{
				this.reverseLights.TurnOff();
			}
			if ((state & 8) != 0)
			{
				this.lowBeamLights.TurnOn();
			}
			else
			{
				this.lowBeamLights.TurnOff();
			}
			if ((state & 16) != 0)
			{
				this.highBeamLights.TurnOn();
			}
			else
			{
				this.highBeamLights.TurnOff();
			}
			if ((state & 32) != 0)
			{
				this.leftBlinkers.TurnOn();
			}
			else
			{
				this.leftBlinkers.TurnOff();
			}
			if ((state & 64) != 0)
			{
				this.rightBlinkers.TurnOn();
				return;
			}
			this.rightBlinkers.TurnOff();
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x000C6C4C File Offset: 0x000C4E4C
		public void SetStatesFromByte(byte state)
		{
			if ((state & 1) != 0)
			{
				this.brakeLights.TurnOn();
			}
			else
			{
				this.brakeLights.TurnOff();
			}
			if ((state & 2) != 0)
			{
				this.tailLights.TurnOn();
			}
			else
			{
				this.tailLights.TurnOff();
			}
			if ((state & 4) != 0)
			{
				this.reverseLights.TurnOn();
			}
			else
			{
				this.reverseLights.TurnOff();
			}
			if ((state & 8) != 0)
			{
				this.lowBeamLights.TurnOn();
			}
			else
			{
				this.lowBeamLights.TurnOff();
			}
			if ((state & 16) != 0)
			{
				this.highBeamLights.TurnOn();
			}
			else
			{
				this.highBeamLights.TurnOff();
			}
			if ((state & 32) != 0)
			{
				this.leftBlinkers.TurnOn();
			}
			else
			{
				this.leftBlinkers.TurnOff();
			}
			if ((state & 64) != 0)
			{
				this.rightBlinkers.TurnOn();
			}
			else
			{
				this.rightBlinkers.TurnOff();
			}
			if ((state & 128) != 0)
			{
				this.extraLights.TurnOn();
				return;
			}
			this.extraLights.TurnOff();
		}

		// Token: 0x0600127B RID: 4731 RVA: 0x000C6D48 File Offset: 0x000C4F48
		public void TurnOffAllLights()
		{
			this.brakeLights.TurnOff();
			this.lowBeamLights.TurnOff();
			this.tailLights.TurnOff();
			this.reverseLights.TurnOff();
			this.highBeamLights.TurnOff();
			this.leftBlinkers.TurnOff();
			this.rightBlinkers.TurnOff();
			this.extraLights.TurnOff();
		}

		// Token: 0x040022F1 RID: 8945
		[FormerlySerializedAs("stopLights")]
		[Tooltip("    Rear lights that will light up when brake is pressed. Always red.")]
		public VehicleLight brakeLights = new VehicleLight();

		// Token: 0x040022F2 RID: 8946
		[Tooltip("    Can be used for any type of special lights.")]
		public VehicleLight extraLights = new VehicleLight();

		// Token: 0x040022F3 RID: 8947
		[FormerlySerializedAs("fullBeams")]
		[Tooltip("    High (full) beam lights.")]
		public VehicleLight highBeamLights = new VehicleLight();

		// Token: 0x040022F4 RID: 8948
		[Tooltip("    Blinkers on the left side of the vehicle.")]
		public VehicleLight leftBlinkers = new VehicleLight();

		// Token: 0x040022F5 RID: 8949
		[FormerlySerializedAs("headLights")]
		[Tooltip("    Low beam lights.")]
		public VehicleLight lowBeamLights = new VehicleLight();

		// Token: 0x040022F6 RID: 8950
		[Tooltip("    Rear Lights that will light up when vehicle is in reverse gear(s). Usually white.")]
		public VehicleLight reverseLights = new VehicleLight();

		// Token: 0x040022F7 RID: 8951
		[Tooltip("    Blinkers on the right side of the vehicle.")]
		public VehicleLight rightBlinkers = new VehicleLight();

		// Token: 0x040022F8 RID: 8952
		[FormerlySerializedAs("rearLights")]
		[Tooltip("    Rear Lights that will light up when headlights are on. Always red.")]
		public VehicleLight tailLights = new VehicleLight();

		// Token: 0x040022F9 RID: 8953
		private bool _leftBlinkerWasOn;

		// Token: 0x040022FA RID: 8954
		private bool _rightBlinkerWasOn;
	}
}
