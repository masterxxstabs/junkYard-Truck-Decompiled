using System.Collections.Generic;
using UnityEngine;

namespace JunkyardATV
{
	// A drivable ATV: four WheelColliders (like the game's golf cart) driven by a
	// copy of the game's 250 engine.
	public class AtvVehicle : MonoBehaviour
	{
		public static readonly List<AtvVehicle> All = new List<AtvVehicle>();

		public const float TopSpeed = 19.5f; // m/s, ~70 km/h
		private const float MaxWheelTorque = 720f;
		private const float BrakeTorque = 1100f;
		private const float AntiRoll = 5500f;

		public Engine250 Engine { get; private set; }

		public Transform Seat { get; private set; }

		public bool Running { get; private set; }

		public float Rpm { get; private set; }

		public float SpeedKmh
		{
			get { return body != null ? Vector3.Dot(body.velocity, transform.forward) * 3.6f : 0f; }
		}

		// Input, set by the mod while someone rides it.
		[System.NonSerialized]
		public float throttle;

		[System.NonSerialized]
		public float steer;

		[System.NonSerialized]
		public bool handbrake;

		[System.NonSerialized]
		public bool ridden;

		private Rigidbody body;
		private AtvLayout layout;
		private readonly WheelCollider[] wheels = new WheelCollider[4];
		private readonly Quaternion[] wheelRest = new Quaternion[4];
		private Quaternion handlebarRest;
		private Transform engineMount;
		private Transform fuelInlet;
		private float steerAngle;
		private float nextEngineCheck;
		private EngineSound sound;

		public static AtvVehicle Create(Vector3 position, Quaternion rotation, string engineState)
		{
			GameObject go = new GameObject("JunkyardATV");
			go.transform.position = position;
			go.transform.rotation = rotation;
			AtvVehicle atv = go.AddComponent<AtvVehicle>();
			atv.Setup(engineState);
			return atv;
		}

		private void OnEnable()
		{
			All.Add(this);
		}

		private void OnDisable()
		{
			All.Remove(this);
		}

		private void Setup(string engineState)
		{
			AtvConfig cfg = AtvMod.Config;
			body = gameObject.AddComponent<Rigidbody>();
			body.mass = cfg.mass;
			body.drag = 0.05f;
			body.angularDrag = 0.6f;
			body.interpolation = RigidbodyInterpolation.Interpolate;

			Seat = new GameObject("Seat").transform;
			Seat.SetParent(transform, false);
			engineMount = new GameObject("EngineMount").transform;
			engineMount.SetParent(transform, false);
			// The game's fuel nozzle fills a trigger with this name (fluid type 17);
			// FuelPatch points it at this ATV's engine.
			GameObject inlet = new GameObject("250FuelInlet");
			inlet.transform.SetParent(transform, false);
			BoxCollider inletBox = inlet.AddComponent<BoxCollider>();
			inletBox.isTrigger = true;
			inletBox.size = new Vector3(0.25f, 0.25f, 0.25f);
			fuelInlet = inlet.transform;

			BuildLook(cfg);

			Engine = AtvEngine.Fit(engineMount, transform);
			if (Engine != null && !string.IsNullOrEmpty(engineState))
			{
				AtvEngine.Restore(Engine, engineState);
			}
			sound = new EngineSound(gameObject);
		}

		// The model, wheels and colliders. Re-run by fit mode when the fit changes.
		internal void BuildLook(AtvConfig cfg)
		{
			Transform old = transform.Find("Look");
			if (old != null)
			{
				old.SetParent(null);
				Destroy(old.gameObject);
			}
			GameObject look = new GameObject("Look");
			look.transform.SetParent(transform, false);
			layout = AtvModel.Build(look.transform, cfg);
			if (layout.handlebars != null)
			{
				handlebarRest = layout.handlebars.localRotation;
			}

			// Body collider: the body's box, lifted clear of the ground under the
			// wheels so it doesn't scrape.
			Bounds b = layout.body;
			float bottom = Mathf.Max(b.min.y, layout.wheelCenters[0].y - layout.wheelRadius * 0.3f);
			GameObject hull = new GameObject("Hull");
			hull.transform.SetParent(look.transform, false);
			BoxCollider box = hull.AddComponent<BoxCollider>();
			box.center = new Vector3(b.center.x, (bottom + b.max.y) / 2f, b.center.z);
			box.size = new Vector3(b.size.x * 0.9f, Mathf.Max(0.1f, b.max.y - bottom), b.size.z * 0.95f);

			// Wheels: the collider sits above the wheel's center by half the
			// suspension travel, so at rest the wheel shows where the model has it.
			const float travel = 0.18f;
			for (int i = 0; i < 4; i++)
			{
				GameObject w = new GameObject("Collider" + i);
				w.transform.SetParent(look.transform, false);
				w.transform.localPosition = layout.wheelCenters[i] + Vector3.up * travel * 0.5f;
				WheelCollider wc = w.AddComponent<WheelCollider>();
				wc.radius = layout.wheelRadius;
				wc.mass = 15f;
				wc.suspensionDistance = travel;
				wc.forceAppPointDistance = 0.1f;
				JointSpring spring = new JointSpring();
				spring.spring = 18000f;
				spring.damper = 2200f;
				spring.targetPosition = 0.5f;
				wc.suspensionSpring = spring;
				WheelFrictionCurve forward = wc.forwardFriction;
				forward.extremumSlip = 0.4f;
				forward.extremumValue = 1.2f;
				forward.asymptoteSlip = 0.8f;
				forward.asymptoteValue = 0.8f;
				forward.stiffness = 1.4f;
				wc.forwardFriction = forward;
				WheelFrictionCurve side = wc.sidewaysFriction;
				side.extremumSlip = 0.25f;
				side.extremumValue = 1.1f;
				side.asymptoteSlip = 0.5f;
				side.asymptoteValue = 0.75f;
				side.stiffness = 1.6f;
				wc.sidewaysFriction = side;
				wheels[i] = wc;
				if (layout.wheels[i] != null)
				{
					// Wheel pivots belong to the ATV, not the model, once built.
					layout.wheels[i].SetParent(look.transform, true);
					wheelRest[i] = layout.wheels[i].localRotation;
				}
			}
			// Low center of mass between the wheels: quads roll easily otherwise.
			body.centerOfMass = new Vector3(b.center.x, layout.wheelCenters[0].y, (layout.wheelCenters[0].z + layout.wheelCenters[2].z) / 2f);
			ApplyPoints(cfg);
		}

		// Seat, engine and fuel inlet positions (cheap: fit mode calls this live).
		internal void ApplyPoints(AtvConfig cfg)
		{
			Seat.localPosition = AtvConfig.IsAuto(cfg.seat) ? layout.seat : cfg.seat;
			engineMount.localPosition = AtvConfig.IsAuto(cfg.engine) ? layout.engine : cfg.engine;
			fuelInlet.localPosition = AtvConfig.IsAuto(cfg.fuelInlet) ? layout.fuelInlet : cfg.fuelInlet;
		}

		public Vector3 PointOf(string which)
		{
			switch (which)
			{
			case "seat":
				return Seat.localPosition;
			case "engine":
				return engineMount.localPosition;
			default:
				return fuelInlet.localPosition;
			}
		}

		public Transform Look
		{
			get { return transform.Find("Look"); }
		}

		// --- Engine. ---

		// Electric start: the engine decides (fuel, missing or broken parts).
		public bool TryStart(out string why)
		{
			why = null;
			if (Engine == null)
			{
				why = "There's no engine fitted.";
				return false;
			}
			if (AtvEngine.GetFloat(Engine, "newFuelLevel") <= 1f)
			{
				why = "Out of fuel.";
				sound.Crank();
				return false;
			}
			Engine.Refresh();
			if (!Engine.canRun)
			{
				why = "It cranks but won't start. Check the engine parts.";
				sound.Crank();
				return false;
			}
			Running = true;
			Rpm = 1600f;
			nextEngineCheck = Time.time + 8f;
			sound.Crank();
			return true;
		}

		public void Stop()
		{
			Running = false;
		}

		private float PowerFactor
		{
			get
			{
				int division = Engine != null && Engine.db != null ? Mathf.Max(1, Engine.db.powerDivision) : 1;
				return 1f / division;
			}
		}

		private void FixedUpdate()
		{
			if (body == null || wheels[0] == null)
			{
				return;
			}
			float dt = Time.fixedDeltaTime;
			float forwardSpeed = Vector3.Dot(body.velocity, transform.forward);
			float speedRatio = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / TopSpeed);

			if (Running)
			{
				// Fuel use and wear, like the dirt bike (Dirtbike.FixedUpdate).
				float fuel = AtvEngine.GetFloat(Engine, "newFuelLevel") - (0.3f + 0.9f * Mathf.Abs(throttle)) * dt;
				AtvEngine.SetFloat(Engine, "newFuelLevel", fuel);
				if (Time.time >= nextEngineCheck)
				{
					nextEngineCheck = Time.time + 8f;
					Engine.DegradeEngine();
					Engine.Refresh();
				}
				if (fuel <= 1f || !Engine.canRun)
				{
					Running = false;
				}
			}

			// Drive: forward torque fades toward top speed; back key brakes while
			// rolling forward, reverses when nearly stopped.
			float drive = 0f;
			float brake = 0f;
			if (ridden)
			{
				if (throttle > 0f)
				{
					drive = Running ? throttle * MaxWheelTorque * PowerFactor * (1f - speedRatio * speedRatio) : 0f;
				}
				else if (throttle < 0f)
				{
					if (forwardSpeed > 1f)
					{
						brake = BrakeTorque * -throttle;
					}
					else if (Running)
					{
						drive = throttle * MaxWheelTorque * 0.4f * PowerFactor * (1f - Mathf.Clamp01(-forwardSpeed / 5f));
					}
				}
				else
				{
					brake = 40f; // engine braking
				}
				if (handbrake)
				{
					brake = BrakeTorque * 1.5f;
				}
			}
			else
			{
				brake = BrakeTorque * 2f; // parked
			}
			for (int i = 0; i < 4; i++)
			{
				wheels[i].motorTorque = drive / 4f;
				wheels[i].brakeTorque = brake;
			}

			// Steering: less lock at speed.
			float maxLock = Mathf.Lerp(32f, 12f, speedRatio);
			steerAngle = Mathf.MoveTowards(steerAngle, steer * maxLock, 140f * dt);
			wheels[AtvModel.FL].steerAngle = steerAngle;
			wheels[AtvModel.FR].steerAngle = steerAngle;

			AntiRollBar(wheels[AtvModel.FL], wheels[AtvModel.FR]);
			AntiRollBar(wheels[AtvModel.RL], wheels[AtvModel.RR]);

			// Engine speed: a CVT, so it follows throttle and road speed.
			float target = Running ? Mathf.Lerp(1600f, 9000f, Mathf.Max(Mathf.Abs(throttle) * 0.55f, speedRatio)) : 0f;
			Rpm = Mathf.MoveTowards(Rpm, target, (Running ? 9000f : 6000f) * dt);
		}

		private void AntiRollBar(WheelCollider left, WheelCollider right)
		{
			WheelHit hit;
			float travelL = 1f;
			float travelR = 1f;
			bool groundedL = left.GetGroundHit(out hit);
			if (groundedL)
			{
				travelL = (-left.transform.InverseTransformPoint(hit.point).y - left.radius) / left.suspensionDistance;
			}
			bool groundedR = right.GetGroundHit(out hit);
			if (groundedR)
			{
				travelR = (-right.transform.InverseTransformPoint(hit.point).y - right.radius) / right.suspensionDistance;
			}
			float force = (travelL - travelR) * AntiRoll;
			if (groundedL)
			{
				body.AddForceAtPosition(left.transform.up * -force, left.transform.position);
			}
			if (groundedR)
			{
				body.AddForceAtPosition(right.transform.up * force, right.transform.position);
			}
		}

		private void Update()
		{
			for (int i = 0; i < 4; i++)
			{
				Transform visual = layout != null ? layout.wheels[i] : null;
				if (visual == null || wheels[i] == null)
				{
					continue;
				}
				Vector3 pos;
				Quaternion rot;
				wheels[i].GetWorldPose(out pos, out rot);
				visual.position = pos;
				visual.rotation = rot * wheelRest[i];
			}
			if (layout != null && layout.handlebars != null)
			{
				layout.handlebars.localRotation = Quaternion.AngleAxis(steerAngle * 0.8f, Vector3.up) * handlebarRest;
			}
			if (sound != null)
			{
				sound.Update(Running, Rpm, throttle);
			}
		}

		public string SaveState()
		{
			return Engine != null ? AtvEngine.Save(Engine) : "";
		}
	}

	// The dirt bike's engine sound: its MotoSound has one looping clip per RPM band
	// (EngineNote2: clip, min/peak/max RPM, pitch reference). The ATV copies those
	// and drives them with its own RPM; it borrows the bike's start sounds too.
	internal class EngineSound
	{
		private readonly List<AudioSource> sources = new List<AudioSource>();
		private readonly List<float[]> bands = new List<float[]>(); // min, peak, max, pitchRef
		private readonly AudioSource crank;
		private AudioClip[] crankClips;

		public EngineSound(GameObject atv)
		{
			crank = atv.AddComponent<AudioSource>();
			Configure(crank);
			foreach (Dirtbike bike in Resources.FindObjectsOfTypeAll<Dirtbike>())
			{
				if (bike == null || !bike.gameObject.scene.IsValid() || bike.engineSound == null)
				{
					continue;
				}
				crankClips = bike.audioClips;
				SMPScripts.MotoSound moto = bike.engineSound.GetComponent<SMPScripts.MotoSound>();
				if (moto == null || moto.engineNotes == null)
				{
					continue;
				}
				foreach (SMPScripts.EngineNote2 note in moto.engineNotes)
				{
					if (note == null || note.source == null || note.source.clip == null)
					{
						continue;
					}
					AudioSource s = atv.AddComponent<AudioSource>();
					Configure(s);
					s.clip = note.source.clip;
					s.loop = true;
					s.volume = 0f;
					sources.Add(s);
					bands.Add(new[] { note.minRPM, note.peakRPM, note.maxRPM, Mathf.Max(1f, note.pitchReferenceRPM) });
				}
				break;
			}
			if (sources.Count == 0)
			{
				AtvMod.Log("Couldn't find the dirt bike's engine sounds; the ATV will be quiet.");
			}
		}

		private static void Configure(AudioSource s)
		{
			s.playOnAwake = false;
			s.spatialBlend = 1f;
			s.minDistance = 3f;
			s.maxDistance = 120f;
			s.dopplerLevel = 0.3f;
		}

		public void Crank()
		{
			if (crankClips != null && crankClips.Length > 0)
			{
				crank.PlayOneShot(crankClips[Random.Range(0, crankClips.Length)]);
			}
		}

		// Same mix as EngineNote2.SetPitchAndGetVolumeForRPM.
		public void Update(bool running, float rpm, float throttle)
		{
			for (int i = 0; i < sources.Count; i++)
			{
				AudioSource s = sources[i];
				if (!running)
				{
					if (s.isPlaying)
					{
						s.Stop();
					}
					continue;
				}
				float[] b = bands[i];
				s.pitch = rpm / b[3];
				float volume = rpm < b[0] || rpm > b[2] ? 0f : rpm < b[1] ? Mathf.InverseLerp(b[0], b[1], rpm) : Mathf.InverseLerp(b[2], b[1], rpm);
				s.volume = volume * (0.6f + 0.4f * Mathf.Abs(throttle));
				if (!s.isPlaying)
				{
					s.Play();
				}
			}
		}
	}
}
