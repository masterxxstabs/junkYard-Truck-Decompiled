using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain.Wheel
{
	// Token: 0x02000283 RID: 643
	[Serializable]
	public class WheelGroup
	{
		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06001165 RID: 4453 RVA: 0x000C1807 File Offset: 0x000BFA07
		public WheelComponent LeftWheel
		{
			get
			{
				if (this.wheels.Count != 0)
				{
					return this.wheels[0];
				}
				return null;
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06001166 RID: 4454 RVA: 0x000C1824 File Offset: 0x000BFA24
		public WheelComponent RightWheel
		{
			get
			{
				if (this.wheels.Count > 1)
				{
					return this.wheels[1];
				}
				return null;
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06001167 RID: 4455 RVA: 0x000C1807 File Offset: 0x000BFA07
		public WheelComponent Wheel
		{
			get
			{
				if (this.wheels.Count != 0)
				{
					return this.wheels[0];
				}
				return null;
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06001168 RID: 4456 RVA: 0x000C1842 File Offset: 0x000BFA42
		public List<WheelComponent> Wheels
		{
			get
			{
				return this.wheels;
			}
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x000C184C File Offset: 0x000BFA4C
		public void Initialize(VehicleController vc)
		{
			this._vc = vc;
			int thisGroupIndex = vc.powertrain.wheelGroups.IndexOf(this);
			this.wheels.Clear();
			foreach (WheelComponent wheel in this.FindWheelsBelongingToGroup(ref vc.powertrain.wheels, thisGroupIndex))
			{
				this.AddWheel(wheel);
			}
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x000C18D0 File Offset: 0x000BFAD0
		public void CalculateCamber()
		{
			if (this.isSolid && this.Wheels.Count == 2)
			{
				WheelComponent wheelComponent = this.Wheels[0];
				WheelComponent wheelComponent2 = this.Wheels[1];
				this._position = (wheelComponent.wheelController.springTravelPoint + wheelComponent2.wheelController.springTravelPoint) / 2f;
				this._direction = this._position - wheelComponent.wheelController.springTravelPoint;
				float num = Vector3.SignedAngle(this._vc.transform.right, this._direction, this._vc.transform.forward);
				num = Mathf.Clamp(num, -25f, 25f);
				wheelComponent.wheelController.SetCamber(num);
				wheelComponent2.wheelController.SetCamber(-num);
				this.camberAtBottom = (this.camberAtTop = num);
				return;
			}
			foreach (WheelComponent wheelComponent3 in this.Wheels)
			{
				wheelComponent3.wheelController.SetCamber(this.camberAtTop, this.camberAtBottom);
			}
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x000C1A18 File Offset: 0x000BFC18
		public void CalculateARB()
		{
			if (this.Wheels.Count != 2)
			{
				return;
			}
			WheelComponent wheelComponent = this.Wheels[0];
			WheelComponent wheelComponent2 = this.Wheels[1];
			if (this.antiRollBarForce > 0f)
			{
				float springTravel = wheelComponent.SpringTravel;
				float springTravel2 = wheelComponent2.SpringTravel;
				float num = wheelComponent.wheelController.spring.length - wheelComponent2.wheelController.spring.length;
				this._arbForce = num * this.antiRollBarForce;
				if (wheelComponent.IsGrounded || wheelComponent2.IsGrounded)
				{
					this._vc.vehicleRigidbody.AddForceAtPosition(wheelComponent.ControllerTransform.up * -this._arbForce, wheelComponent.ControllerTransform.position);
					this._vc.vehicleRigidbody.AddForceAtPosition(wheelComponent2.ControllerTransform.up * this._arbForce, wheelComponent2.ControllerTransform.position);
				}
			}
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x000C1B0F File Offset: 0x000BFD0F
		public void AddWheel(WheelComponent wheel)
		{
			this.Wheels.Add(wheel);
			wheel.wheelGroup = this;
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x000C1B24 File Offset: 0x000BFD24
		public List<WheelComponent> FindWheelsBelongingToGroup(ref List<WheelComponent> wheels, int thisGroupIndex)
		{
			List<WheelComponent> list = new List<WheelComponent>();
			foreach (WheelComponent wheelComponent in wheels)
			{
				if (wheelComponent.wheelGroupSelector.index == thisGroupIndex)
				{
					list.Add(wheelComponent);
				}
			}
			return list;
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x000C1B88 File Offset: 0x000BFD88
		public void RemoveWheel(WheelComponent wheel)
		{
			this.Wheels.Remove(wheel);
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x000C1B98 File Offset: 0x000BFD98
		public void SetWheels(List<WheelComponent> wheels)
		{
			this.wheels = wheels;
			foreach (WheelComponent wheelComponent in wheels)
			{
				wheelComponent.wheelGroup = this;
			}
		}

		// Token: 0x040021B3 RID: 8627
		[Tooltip("Set to positive for Pro-Ackerman steering (inner wheel steers more) or to negative for Anti-Ackerman steering.")]
		[Range(-1f, 1f)]
		[ShowInSettings("Ackerman Percent", -0.5f, 0.5f, 0.05f)]
		public float ackermanPercent = 0.15f;

		// Token: 0x040021B4 RID: 8628
		[Tooltip("Used to reduce roll in the vehicle. Should not exceed max spring force setting. Another way to reduce roll is to adjust center of mass to be lower.")]
		[ShowInSettings("ARB Force", 1000f, 12000f, 1000f)]
		public float antiRollBarForce;

		// Token: 0x040021B5 RID: 8629
		[Tooltip("If set to 1 axle will receive full brake torque as set by Max Torque parameter under Brake section while 0 means no breaking at all.")]
		[Range(0f, 1f)]
		[ShowInSettings("Brake Coefficient", 0f, 1f, 0.1f)]
		public float brakeCoefficient = 1f;

		// Token: 0x040021B6 RID: 8630
		[Tooltip("Camber at the bottom of the spring travel (wheel is at the lowest point).")]
		[Range(-16f, 16f)]
		[ShowInSettings("Wheel Camber At Bottom", -10f, 10f, 0.5f)]
		public float camberAtBottom = 1f;

		// Token: 0x040021B7 RID: 8631
		[Tooltip("Camber at the top of the spring travel (wheel is at the highest point). Set to other than 0 to override WC3D's settings,and set to 0 if you want to use camber settings and curve from WC3D inpector.")]
		[Range(-16f, 16f)]
		[ShowInSettings("Wheel Camber At Top", -10f, 10f, 0.5f)]
		public float camberAtTop = -5f;

		// Token: 0x040021B8 RID: 8632
		[Tooltip("Positive caster means that whe wheel will be angled towards the front of the vehicle while negative  caster will angle the wheel in opposite direction (shopping cart wheel).")]
		[Range(-8f, 8f)]
		[ShowInSettings("Caster Angle", -8f, 8f, 0.5f)]
		public float casterAngle;

		// Token: 0x040021B9 RID: 8633
		[Range(0f, 1f)]
		[ShowInSettings("Handbrake Coefficient", 0f, 1f, 0.1f)]
		[Tooltip("    If set to 1 axle will receive full brake torque when handbrake is used.")]
		public float handbrakeCoefficient;

		// Token: 0x040021BA RID: 8634
		[Tooltip("Setting to true will override camber settings and camber will be calculated from position of the (imaginary) axle object instead.")]
		public bool isSolid;

		// Token: 0x040021BB RID: 8635
		public string name;

		// Token: 0x040021BC RID: 8636
		[Tooltip("Determines what percentage of the steer angle will be applied to the wheel. If set to negative value wheels will turn in direction opposite of input.")]
		[Range(-1f, 1f)]
		[ShowInSettings("Steer Coefficient", -1f, 1f, 0.1f)]
		public float steerCoefficient;

		// Token: 0x040021BD RID: 8637
		[Tooltip("Positive toe angle means that the wheels will face inwards (front of the wheel angled toward longitudinal center of the vehicle).")]
		[Range(-8f, 8f)]
		[ShowInSettings("Toe Angle", -5f, 5f, 0.2f)]
		public float toeAngle;

		// Token: 0x040021BE RID: 8638
		[SerializeField]
		private List<WheelComponent> wheels = new List<WheelComponent>();

		// Token: 0x040021BF RID: 8639
		private float _arbForce;

		// Token: 0x040021C0 RID: 8640
		private Vector3 _direction;

		// Token: 0x040021C1 RID: 8641
		private Vector3 _position;

		// Token: 0x040021C2 RID: 8642
		private VehicleController _vc;
	}
}
