using System.Collections.Generic;
using UnityEngine;

namespace TruckPartsQOL
{
	// Bolts like the game's own, so a fitted part is bolted down and taken off the
	// vanilla way: with the ratchet (tool 2), scroll on each bolt to tighten or
	// loosen it. The game's ratchet code does the turning; it only needs what its
	// own bolts have:
	//
	//   a child named "bolt" on the "Bolts" layer, with a Renderer (it glows when
	//   aimed at), a BoxCollider, an AudioSource and a BoltScript (its turn count,
	//   -1 to 11, 10 = tight), under a parent holding a `durability` (numBolts).
	//
	// The bolts sit under a "Bolts" holder rather than the part itself, so the
	// game never takes the part for one of its own slot parts.
	//
	// A part is fitted with its bolts out (loose). It can be taken off once every
	// bolt is all the way out, and a part that's barely bolted falls off when the
	// vehicle gets going.
	public class PartBolts : MonoBehaviour
	{
		public const int Tight = 10;

		// Where the bolts go, in the part's space: position of the bolt head's
		// seat, and the direction the bolt goes in (into the surface, usually -Z).
		public List<Vector3> seats = new List<Vector3>();
		public List<Vector3> directions = new List<Vector3>();

		// Public (serialized) so copies of a part (the store's templates, a cloned
		// vehicle) point at their own bolts.
		public Transform holder;
		public List<BoltScript> bolts = new List<BoltScript>();

		private static Mesh hexMesh;
		private static Material boltMaterial;
		private static int boltsLayer = -2;

		public int Count
		{
			get { return bolts.Count; }
		}

		public void Build()
		{
			if (holder != null || seats.Count == 0)
			{
				return;
			}
			if (boltsLayer == -2)
			{
				boltsLayer = LayerMask.NameToLayer("Bolts");
				if (boltsLayer < 0)
				{
					TruckPartsQOLMod.Log("The game has no \"Bolts\" layer, so parts fit without bolts.");
				}
			}
			if (boltsLayer < 0)
			{
				// The ratchet couldn't reach them: no bolts (parts just fit and come off).
				return;
			}
			holder = new GameObject("Bolts").transform;
			holder.SetParent(transform, false);
			durability d = holder.gameObject.AddComponent<durability>();
			d.numBolts = seats.Count;
			d.health = 100f;
			for (int i = 0; i < seats.Count; i++)
			{
				GameObject bolt = new GameObject("bolt");
				bolt.transform.SetParent(holder, false);
				bolt.transform.localRotation = Quaternion.LookRotation(directions[i], Mathf.Abs(Vector3.Dot(directions[i], Vector3.up)) > 0.9f ? Vector3.forward : Vector3.up);
				bolt.AddComponent<MeshFilter>().sharedMesh = HexMesh();
				bolt.AddComponent<MeshRenderer>().material = new Material(Material());
				BoxCollider box = bolt.AddComponent<BoxCollider>();
				box.size = new Vector3(0.02f, 0.02f, 0.014f);
				box.center = new Vector3(0f, 0f, -0.004f);
				AudioSource audio = bolt.AddComponent<AudioSource>();
				audio.playOnAwake = false;
				audio.spatialBlend = 1f;
				BoltScript script = bolt.AddComponent<BoltScript>();
				// Its Start() pulls a loose bolt out by its turn count; positions are
				// set here instead, so keep it from running.
				script.enabled = false;
				script.boltturns = Tight;
				bolt.layer = boltsLayer;
				bolts.Add(script);
			}
			Place();
		}

		// Barely bolted (under 30%) and the vehicle gets going: it falls off.
		private void FixedUpdate()
		{
			if (bolts.Count == 0 || Tightness >= 0.3f)
			{
				return;
			}
			TruckPart part = GetComponent<TruckPart>();
			if (part == null || !part.installed || transform.parent == null)
			{
				return;
			}
			Rigidbody body = transform.parent.GetComponentInParent<Rigidbody>();
			if (body == null || body.velocity.sqrMagnitude < 16f)
			{
				return;
			}
			Vector3 velocity = body.GetPointVelocity(transform.position);
			part.Remove();
			Rigidbody own = GetComponent<Rigidbody>();
			if (own != null)
			{
				own.velocity = velocity;
			}
			TruckPartsQOLMod.Notify("Your " + part.DisplayName + " fell off. Tighten its bolts with the ratchet (tool 2) next time.");
		}

		public bool IsBolt(Collider c)
		{
			return holder != null && c != null && c.transform.parent == holder;
		}

		// Shown (and aimable) only while the part is fitted.
		public void Show(bool shown)
		{
			foreach (BoltScript b in bolts)
			{
				if (b == null)
				{
					continue;
				}
				b.GetComponent<Renderer>().enabled = shown;
				Collider c = b.GetComponent<Collider>();
				c.enabled = shown;
				c.isTrigger = false;
			}
		}

		public void SetAll(int turns)
		{
			foreach (BoltScript b in bolts)
			{
				b.boltturns = turns;
			}
			Place();
		}

		public string Save()
		{
			List<string> turns = new List<string>();
			foreach (BoltScript b in bolts)
			{
				turns.Add(b.boltturns.ToString());
			}
			return string.Join(",", turns.ToArray());
		}

		public void Load(string saved)
		{
			string[] turns = saved.Split(',');
			for (int i = 0; i < bolts.Count && i < turns.Length; i++)
			{
				int t;
				if (int.TryParse(turns[i], out t))
				{
					bolts[i].boltturns = Mathf.Clamp(t, -1, 11);
				}
			}
			Place();
		}

		// 0 = every bolt out, 1 = every bolt tight.
		public float Tightness
		{
			get
			{
				if (bolts.Count == 0)
				{
					return 1f;
				}
				float sum = 0f;
				foreach (BoltScript b in bolts)
				{
					sum += Mathf.Clamp(b.boltturns, 0, Tight);
				}
				return sum / (bolts.Count * Tight);
			}
		}

		public int TightCount
		{
			get
			{
				int n = 0;
				foreach (BoltScript b in bolts)
				{
					if (b.boltturns >= Tight)
					{
						n++;
					}
				}
				return n;
			}
		}

		// The game's own rule: a part comes off once its bolts' turns add up to
		// less than one.
		public bool AllOut
		{
			get
			{
				int sum = 0;
				foreach (BoltScript b in bolts)
				{
					sum += b.boltturns;
				}
				return sum < 1;
			}
		}

		// A bolt sits 2 mm further out per turn short of tight, like the game's.
		private void Place()
		{
			for (int i = 0; i < bolts.Count; i++)
			{
				Transform t = bolts[i].transform;
				t.localPosition = seats[i] - directions[i].normalized * (0.002f * (Tight - bolts[i].boltturns));
				t.localRotation = Quaternion.LookRotation(directions[i], Mathf.Abs(Vector3.Dot(directions[i], Vector3.up)) > 0.9f ? Vector3.forward : Vector3.up) * Quaternion.Euler(0f, 0f, -30f * bolts[i].boltturns);
			}
		}

		// A hex bolt head with a washer, its shank along +Z (into the surface).
		private static Mesh HexMesh()
		{
			if (hexMesh != null)
			{
				return hexMesh;
			}
			List<Vector3> v = new List<Vector3>();
			List<int> t = new List<int>();
			AddPrism(v, t, 6, 0.0065f, -0.0055f, -0.0015f, 30f);  // head
			AddPrism(v, t, 16, 0.0085f, -0.0015f, 0f, 0f);       // washer
			AddPrism(v, t, 10, 0.003f, 0f, 0.012f, 0f);          // shank
			hexMesh = new Mesh();
			hexMesh.name = "TruckPartsQOL bolt";
			hexMesh.SetVertices(v);
			hexMesh.SetTriangles(t, 0);
			hexMesh.RecalculateNormals();
			hexMesh.RecalculateBounds();
			return hexMesh;
		}

		// A closed n-sided prism along Z from z0 to z1 (flat-shaded: own vertices per face).
		private static void AddPrism(List<Vector3> v, List<int> t, int sides, float radius, float z0, float z1, float phase)
		{
			Vector3[] ring = new Vector3[sides];
			for (int i = 0; i < sides; i++)
			{
				float a = (phase + 360f * i / sides) * Mathf.Deg2Rad;
				ring[i] = new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0f);
			}
			Vector3 back = new Vector3(0f, 0f, z0), front = new Vector3(0f, 0f, z1);
			for (int i = 0; i < sides; i++)
			{
				Vector3 a = ring[i], b = ring[(i + 1) % sides];
				Quad(v, t, a + back, b + back, b + front, a + front);
				Tri(v, t, back, b + back, a + back);    // outer cap (faces -Z)
				Tri(v, t, front, a + front, b + front); // inner cap (faces +Z)
			}
		}

		private static void Quad(List<Vector3> v, List<int> t, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
		{
			Tri(v, t, a, b, c);
			Tri(v, t, a, c, d);
		}

		private static void Tri(List<Vector3> v, List<int> t, Vector3 a, Vector3 b, Vector3 c)
		{
			int i = v.Count;
			v.Add(a);
			v.Add(b);
			v.Add(c);
			t.Add(i);
			t.Add(i + 1);
			t.Add(i + 2);
		}

		private static Material Material()
		{
			if (boltMaterial == null)
			{
				boltMaterial = ModelBuilder.NewMaterial();
				boltMaterial.color = new Color(0.62f, 0.63f, 0.66f);
				if (boltMaterial.HasProperty("_Metallic"))
				{
					boltMaterial.SetFloat("_Metallic", 0.85f);
				}
				if (boltMaterial.HasProperty("_Glossiness"))
				{
					boltMaterial.SetFloat("_Glossiness", 0.55f);
				}
			}
			return boltMaterial;
		}
	}
}
