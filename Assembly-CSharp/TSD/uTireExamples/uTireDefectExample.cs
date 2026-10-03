using System;
using System.Collections;
using TSD.uTireRuntime;
using UnityEngine;

namespace TSD.uTireExamples
{
	// Token: 0x02000341 RID: 833
	public class uTireDefectExample : MonoBehaviour
	{
		// Token: 0x06001565 RID: 5477 RVA: 0x000E0258 File Offset: 0x000DE458
		private void Awake()
		{
			uTireDefectExample.instance = this;
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x000E0260 File Offset: 0x000DE460
		private void Start()
		{
			this.vehicle = uTireManager.GetVehicle(this.vehiclesRigidbody);
			this.initSFX();
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x000E0279 File Offset: 0x000DE479
		public void SetVehicle(Vehicle newVehicle)
		{
			this.vehicle = newVehicle;
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x000E0284 File Offset: 0x000DE484
		private void initSFX()
		{
			GameObject gameObject = new GameObject("Audio Source");
			this.deflateTransform = gameObject.transform;
			this.deflateTransform.SetParent(base.transform);
			this.deflateSFXSource = gameObject.AddComponent<AudioSource>();
			this.deflateSFXSource.spatialBlend = 1f;
			this.deflateSFXSource.clip = this.deflateSFX;
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x000E02E6 File Offset: 0x000DE4E6
		private void setActiveWheels(Rigidbody m_vehicleRigidbody)
		{
			this.vehicle = uTireManager.GetVehicle(m_vehicleRigidbody);
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x000E02F4 File Offset: 0x000DE4F4
		public IEnumerator changePressure(Object m_wc)
		{
			if (m_wc == null)
			{
				yield break;
			}
			WheelMeshConnection wmc = uTireManager.GetWheelMeshConnection(m_wc);
			float t = 0f;
			float targetPressure = (Mathf.Round(wmc.tirePressure) > 0f) ? 0f : 1f;
			float startPressure = wmc.tirePressure;
			if (this.useAudio && !this.deflateSFXSource.isPlaying)
			{
				this.deflateTransform.position = wmc.iWheel.wheelTransform.position;
				this.deflateSFXSource.Play();
			}
			while (t < 1f)
			{
				t += Time.deltaTime * this.deflateSpeed;
				wmc.SetPressure(Mathf.Lerp(startPressure, targetPressure, t));
				yield return new WaitForEndOfFrame();
			}
			if (this.useAudio)
			{
				this.deflateSFXSource.Stop();
			}
			yield break;
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x000E030A File Offset: 0x000DE50A
		public void ToggleState()
		{
			this.state = !this.state;
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x000E031C File Offset: 0x000DE51C
		private void OnGUI()
		{
			if (this.vehicle == null || !this.state)
			{
				return;
			}
			GUI.Box(new Rect(10f, 30f, 160f, 166f), "");
			GUILayout.BeginArea(new Rect(10f, 30f, 160f, 400f));
			GUILayout.Label("Inflate/Deflate", Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Front Left", Array.Empty<GUILayoutOption>()))
			{
				base.StartCoroutine(this.changePressure(this.vehicle.wheels[0].iWheel.wheelObject));
			}
			if (GUILayout.Button("Front Right", Array.Empty<GUILayoutOption>()))
			{
				base.StartCoroutine(this.changePressure(this.vehicle.wheels[1].iWheel.wheelObject));
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Rear Left", Array.Empty<GUILayoutOption>()))
			{
				base.StartCoroutine(this.changePressure(this.vehicle.wheels[2].iWheel.wheelObject));
			}
			if (GUILayout.Button("Rear Right", Array.Empty<GUILayoutOption>()))
			{
				base.StartCoroutine(this.changePressure(this.vehicle.wheels[3].iWheel.wheelObject));
			}
			GUILayout.EndHorizontal();
			this.vehicle.SetPressureMultiplier(GUILayout.HorizontalSlider((float)Math.Round((double)this.vehicle.tirePressureMultiplier, 1), 0f, 2f, Array.Empty<GUILayoutOption>()));
			if (GUILayout.Button("Toggle Tire Deformation", Array.Empty<GUILayoutOption>()))
			{
				Singleton<uTireManager>.Instance.updateMaterial = !Singleton<uTireManager>.Instance.updateMaterial;
				if (!Singleton<uTireManager>.Instance.updateMaterial)
				{
					this.savedTirePressure = Singleton<uTireManager>.Instance.vehicles[0].tirePressureMultiplier;
					foreach (Vehicle vehicle in Singleton<uTireManager>.Instance.vehicles)
					{
						vehicle.tirePressureMultiplier = 2f;
						Singleton<uTireManager>.Instance.updateWheels();
					}
				}
				foreach (Vehicle vehicle2 in Singleton<uTireManager>.Instance.vehicles)
				{
					vehicle2.tirePressureMultiplier = this.savedTirePressure;
				}
			}
			GUILayout.Box("Vehicle Turn: " + this.vehicle.wheels[0].turnAngle, Array.Empty<GUILayoutOption>());
			GUILayout.Box("Vehicle Speed Ratio: " + this.vehicle.speedRatio, Array.Empty<GUILayoutOption>());
			GUILayout.EndArea();
		}

		// Token: 0x04002607 RID: 9735
		public Rigidbody vehiclesRigidbody;

		// Token: 0x04002608 RID: 9736
		public float deflateSpeed = 1f;

		// Token: 0x04002609 RID: 9737
		public bool useAudio;

		// Token: 0x0400260A RID: 9738
		public AudioClip deflateSFX;

		// Token: 0x0400260B RID: 9739
		private Transform deflateTransform;

		// Token: 0x0400260C RID: 9740
		private AudioSource deflateSFXSource;

		// Token: 0x0400260D RID: 9741
		private Vehicle vehicle;

		// Token: 0x0400260E RID: 9742
		private float savedTirePressure;

		// Token: 0x0400260F RID: 9743
		public static uTireDefectExample instance;

		// Token: 0x04002610 RID: 9744
		private bool state = true;
	}
}
