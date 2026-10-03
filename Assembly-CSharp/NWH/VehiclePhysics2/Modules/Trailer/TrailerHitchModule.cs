using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace NWH.VehiclePhysics2.Modules.Trailer
{
	// Token: 0x02000289 RID: 649
	[Serializable]
	public class TrailerHitchModule : VehicleModule
	{
		// Token: 0x170001BF RID: 447
		// (get) Token: 0x0600118D RID: 4493 RVA: 0x000C2372 File Offset: 0x000C0572
		// (set) Token: 0x0600118E RID: 4494 RVA: 0x000C237A File Offset: 0x000C057A
		public TrailerModule Trailer
		{
			get
			{
				return this._trailer;
			}
			set
			{
				this._trailer = value;
			}
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x000C2383 File Offset: 0x000C0583
		public override void Initialize()
		{
			this.FindSceneTrailerModules(ref this._trailerModules);
			this.initialized = true;
			if (this.attachOnPlay)
			{
				this.AttachTrailer(this.vc);
			}
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x000C23AC File Offset: 0x000C05AC
		public override void FixedUpdate()
		{
			if (this._trailer == null && this.attachmentPoint != null)
			{
				if (this._trailerModules.Count > 0)
				{
					this._nearestTrailerModule = null;
					this.trailerInRange = false;
					using (List<TrailerModule>.Enumerator enumerator = this._trailerModules.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TrailerModule trailerModule = enumerator.Current;
							if (trailerModule.Active && (trailerModule.attachmentPoint.transform.position - this.attachmentPoint.transform.position).sqrMagnitude < this.attachDistanceThreshold)
							{
								this._nearestTrailerModule = trailerModule;
								this.trailerInRange = true;
								break;
							}
						}
						goto IL_C5;
					}
				}
				this._nearestTrailerModule = null;
				this.trailerInRange = false;
			}
			IL_C5:
			if (this._nearestTrailerModule != null && this._trailer == null && this.vc.input.TrailerAttachDetach)
			{
				this.AttachTrailer(this.vc);
			}
			else if (this._trailer != null && this.vc.input.TrailerAttachDetach)
			{
				this.DetachTrailer(this.vc);
			}
			if (this._trailer != null && this._configurableJoint == null)
			{
				this.DetachTrailer(this.vc);
			}
			this.vc.input.ResetTrailerAttachDetachFlag();
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x000C2518 File Offset: 0x000C0718
		public override void Update()
		{
			if (this._trailer != null && this._trailer.VehicleController != null && this._trailer.VehicleController.input.Active)
			{
				this._trailer.VehicleController.input.states = this.vc.input.states;
			}
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x000C257C File Offset: 0x000C077C
		public override void Enable()
		{
			base.Enable();
			if (this.vc != null)
			{
				this.vc.powertrain.engine.powerModifiers.Add(new EngineComponent.PowerModifier(this.NoTrailerPowerModifier));
			}
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x000C25B8 File Offset: 0x000C07B8
		public override void Disable()
		{
			base.Disable();
			if (this.vc != null)
			{
				this.vc.powertrain.engine.powerModifiers.Remove(new EngineComponent.PowerModifier(this.NoTrailerPowerModifier));
			}
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x000C25F5 File Offset: 0x000C07F5
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.Trailer;
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x000C25F8 File Offset: 0x000C07F8
		public float NoTrailerPowerModifier()
		{
			if (!this.attached)
			{
				return 1f;
			}
			return this.noTrailerPowerCoefficient;
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x000C260E File Offset: 0x000C080E
		public void SyncTrailers()
		{
			this.FindSceneTrailerModules(ref this._trailerModules);
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x000C261C File Offset: 0x000C081C
		private void AttachTrailer(VehicleController vc)
		{
			if (this._nearestTrailerModule != null)
			{
				this._trailer = this._nearestTrailerModule;
				if (this._trailer == null)
				{
					return;
				}
				VehicleController vehicleController = this._trailer.VehicleController;
				vehicleController.vehicleTransform.position = vehicleController.transform.position - (this._trailer.attachmentPoint.transform.position - this.attachmentPoint.transform.position);
				this._configurableJoint = vehicleController.gameObject.GetComponent<ConfigurableJoint>();
				if (this._configurableJoint == null)
				{
					this._configurableJoint = vehicleController.gameObject.AddComponent<ConfigurableJoint>();
				}
				this._configurableJoint.connectedBody = vc.vehicleRigidbody;
				this._configurableJoint.anchor = vehicleController.transform.InverseTransformPoint(this.attachmentPoint.transform.position);
				this._configurableJoint.xMotion = ConfigurableJointMotion.Locked;
				this._configurableJoint.yMotion = ConfigurableJointMotion.Locked;
				this._configurableJoint.zMotion = ConfigurableJointMotion.Locked;
				this._configurableJoint.angularZMotion = (this.useHingeJoint ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free);
				this._configurableJoint.enableCollision = true;
				this._configurableJoint.breakForce = this.breakForce;
				vc.input.ResetTrailerAttachDetachFlag();
				if (vc.effectsManager.lightsManager.IsEnabled)
				{
					vehicleController.effectsManager.lightsManager.Enable();
				}
				else
				{
					vehicleController.effectsManager.lightsManager.Disable();
				}
				this.attached = true;
				this._trailer.OnAttach(this);
				this.onTrailerAttach.Invoke();
			}
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x000C27B8 File Offset: 0x000C09B8
		private void DetachTrailer(VehicleController vc)
		{
			if (!this.detachable || this._trailer == null || this._trailer.VehicleController == null)
			{
				return;
			}
			this.attached = false;
			if (this._configurableJoint != null)
			{
				Object.Destroy(this._configurableJoint);
				this._configurableJoint = null;
			}
			this._trailer.OnDetach();
			this._trailer = null;
			vc.input.ResetTrailerAttachDetachFlag();
			this.onTrailerDetach.Invoke();
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x000C2838 File Offset: 0x000C0A38
		private void FindSceneTrailerModules(ref List<TrailerModule> trailerModules)
		{
			trailerModules = new List<TrailerModule>();
			TrailerModuleWrapper[] array = Object.FindObjectsOfType<TrailerModuleWrapper>();
			for (int i = 0; i < array.Length; i++)
			{
				TrailerModule module = array[i].module;
				if (module != null && this.vc.gameObject.scene == SceneManager.GetActiveScene())
				{
					trailerModules.Add(module);
				}
			}
		}

		// Token: 0x040021D1 RID: 8657
		[Tooltip("    Maximum distance between towing vehicle's attachment point and trailer's attachment point.")]
		public float attachDistanceThreshold = 0.5f;

		// Token: 0x040021D2 RID: 8658
		[Tooltip("True if object is trailer and is attached to a towing vehicle and also true if towing vehicle and has trailer\r\nattached.")]
		public bool attached;

		// Token: 0x040021D3 RID: 8659
		[Tooltip("If the vehicle is a trailer, this is the object placed at the point at which it will connect to the towing vehicle. If the vehicle is towing, this is the object placed at point at which trailer will be coneected.")]
		public Transform attachmentPoint;

		// Token: 0x040021D4 RID: 8660
		[Tooltip("    If a trailer is in range when the scene is started it will be attached.")]
		public bool attachOnPlay;

		// Token: 0x040021D5 RID: 8661
		[Tooltip("    Breaking force of the generated joint.")]
		public float breakForce = float.PositiveInfinity;

		// Token: 0x040021D6 RID: 8662
		[Tooltip("    Can the trailer be detached once it is attached?")]
		public bool detachable = true;

		// Token: 0x040021D7 RID: 8663
		[Tooltip("Power reduction that will be applied when vehicle has no trailer to avoid wheel spin when controlled with a binary controller.")]
		public float noTrailerPowerCoefficient = 1f;

		// Token: 0x040021D8 RID: 8664
		public UnityEvent onTrailerAttach;

		// Token: 0x040021D9 RID: 8665
		public UnityEvent onTrailerDetach;

		// Token: 0x040021DA RID: 8666
		[Tooltip("    Is trailer's attachment point close enough to be attached to the towing vehicle?")]
		public bool trailerInRange;

		// Token: 0x040021DB RID: 8667
		[Tooltip("    Use for articulated busses and equipment where rotation around vertical axis is not wanted.")]
		public bool useHingeJoint;

		// Token: 0x040021DC RID: 8668
		[NonSerialized]
		private ConfigurableJoint _configurableJoint;

		// Token: 0x040021DD RID: 8669
		[NonSerialized]
		private TrailerModule _nearestTrailerModule;

		// Token: 0x040021DE RID: 8670
		private TrailerModule _trailer;

		// Token: 0x040021DF RID: 8671
		[NonSerialized]
		private List<TrailerModule> _trailerModules = new List<TrailerModule>();
	}
}
