using System.Collections.Generic;
using UnityEngine;

namespace FurnitureTweaks
{
	// The furniture's shapes. Each piece stands on its origin (the floor), with
	// its front facing +Z (toward you when it's placed).
	internal static class Models
	{
		// --- Parts shelf: a steel pallet rack, blue uprights, orange beams, four
		// levels of wooden decking, three spots on each. ---

		public const float ShelfWidth = 1.8f;
		public const float ShelfDepth = 0.6f;
		public static readonly float[] ShelfLevels = { 0.14f, 0.64f, 1.14f, 1.64f };
		public static readonly float[] ShelfSpots = { -0.56f, 0f, 0.56f };
		// Room above each level, up to the next one's beams.
		public const float ShelfClearance = 0.42f;

		public static void Shelf(GameObject root)
		{
			MeshKit k = new MeshKit();
			Material blue = Looks.RackBlue(), orange = Looks.RackOrange(), wood = Looks.Wood(), steel = Looks.Steel();
			float hx = 0.87f, hz = 0.27f, h = 2.0f, post = 0.07f;
			foreach (float x in new[] { -hx, hx })
			{
				foreach (float z in new[] { -hz, hz })
				{
					k.Box(blue, new Vector3(x, h / 2f, z), new Vector3(post, h, post), true);
					k.Box(steel, new Vector3(x, 0.005f, z), new Vector3(0.13f, 0.01f, 0.13f));
					// Anchor bolts in the foot plates.
					k.Cylinder(steel, new Vector3(x + 0.045f, 0.015f, z), 0.008f, 0.012f, Vector3.up, 8);
				}
				// The frame's bracing between the front and back posts: rails top and
				// bottom, a zigzag of diagonals between.
				k.Box(blue, new Vector3(x, 0.08f, 0f), new Vector3(0.03f, 0.03f, hz * 2f));
				k.Box(blue, new Vector3(x, h - 0.06f, 0f), new Vector3(0.03f, 0.03f, hz * 2f));
				float y0 = 0.08f;
				int bays = 4;
				float bay = (h - 0.14f) / bays;
				for (int b = 0; b < bays; b++)
				{
					float ya = y0 + b * bay, yb = ya + bay;
					float za = b % 2 == 0 ? -hz : hz;
					Vector3 a = new Vector3(x, ya, za), c = new Vector3(x, yb, -za);
					Vector3 d = c - a;
					Quaternion rot = Quaternion.FromToRotation(Vector3.up, d.normalized);
					k.Box(blue, (a + c) / 2f, new Vector3(0.025f, d.magnitude, 0.025f), rot);
				}
				// Rows of slots punched up the posts' fronts (dark squares).
				for (float y = 0.2f; y < h - 0.1f; y += 0.075f)
				{
					k.Box(Looks.Black(), new Vector3(x, y, hz + post / 2f + 0.0005f), new Vector3(0.018f, 0.03f, 0.001f));
					k.Box(Looks.Black(), new Vector3(x, y, -hz - post / 2f - 0.0005f), new Vector3(0.018f, 0.03f, 0.001f));
				}
			}
			float span = hx * 2f - post;
			foreach (float level in ShelfLevels)
			{
				// Front and back beams, their tops level with the deck's underside.
				foreach (float z in new[] { -hz, hz })
				{
					k.Box(orange, new Vector3(0f, level - 0.045f, z), new Vector3(span, 0.09f, 0.045f), true);
					// The beam's end clips on the posts.
					foreach (float x in new[] { -hx + post / 2f + 0.02f, hx - post / 2f - 0.02f })
					{
						k.Box(steel, new Vector3(x, level - 0.045f, z + Mathf.Sign(z) * 0.024f), new Vector3(0.04f, 0.11f, 0.004f));
					}
				}
				// Deck: boards across the depth, small gaps between.
				int boards = 5;
				float bw = (hz * 2f + 0.045f) / boards;
				for (int i = 0; i < boards; i++)
				{
					float z = -hz - 0.0225f + bw * (i + 0.5f);
					k.Box(wood, new Vector3(0f, level + 0.011f, z), new Vector3(span - 0.01f, 0.022f, bw - 0.012f));
				}
				k.Solid(new Vector3(0f, level + 0.011f, 0f), new Vector3(span, 0.022f, hz * 2f + 0.045f));
				// A support bar under the middle of the deck.
				k.Box(steel, new Vector3(0f, level - 0.015f, 0f), new Vector3(0.04f, 0.03f, hz * 2f));
			}
			k.Build(root);
		}

		// Where parts sit on the shelf (on the deck, in the shelf's frame).
		public static List<Vector3> ShelfSlots()
		{
			List<Vector3> slots = new List<Vector3>();
			foreach (float level in ShelfLevels)
			{
				foreach (float x in ShelfSpots)
				{
					slots.Add(new Vector3(x, level + 0.022f, 0f));
				}
			}
			return slots;
		}

		// --- Safe: a heavy enamel box, a door with a brass pinstripe, a dial and
		// a spoked handle. ---

		public static readonly Vector3 SafeCashOut = new Vector3(0.18f, 0.55f, 0.62f);

		public static void Safe(GameObject root)
		{
			MeshKit k = new MeshKit();
			Material body = Looks.SafeBody(), brass = Looks.Brass(), chrome = Looks.Chrome(), black = Looks.Black(), steel = Looks.Steel();
			float w = 0.66f, d = 0.62f, feet = 0.06f, h = 0.89f;
			float cy = feet + h / 2f;
			foreach (float x in new[] { -0.26f, 0.26f })
			{
				foreach (float z in new[] { -0.24f, 0.24f })
				{
					k.Box(black, new Vector3(x, feet / 2f, z), new Vector3(0.09f, feet, 0.09f));
				}
			}
			k.Box(body, new Vector3(0f, cy, 0f), new Vector3(w, h, d), true);
			// A raised band round the top and bottom edges.
			k.Box(body, new Vector3(0f, feet + 0.02f, 0f), new Vector3(w + 0.016f, 0.04f, d + 0.016f));
			k.Box(body, new Vector3(0f, feet + h - 0.024f, 0f), new Vector3(w + 0.016f, 0.04f, d + 0.016f));
			// The door: a thick slab proud of the front, the dark gap round it.
			float dw = 0.54f, dh = 0.74f, front = d / 2f;
			k.Box(black, new Vector3(0f, cy, front + 0.004f), new Vector3(dw + 0.018f, dh + 0.018f, 0.008f));
			k.Box(body, new Vector3(0f, cy, front + 0.026f), new Vector3(dw, dh, 0.044f));
			// Pinstripe inset on the door.
			float inset = 0.035f, line = 0.006f;
			float lz = front + 0.0485f;
			k.Box(brass, new Vector3(0f, cy + dh / 2f - inset, lz), new Vector3(dw - inset * 2f, line, 0.001f));
			k.Box(brass, new Vector3(0f, cy - dh / 2f + inset, lz), new Vector3(dw - inset * 2f, line, 0.001f));
			k.Box(brass, new Vector3(-dw / 2f + inset, cy, lz), new Vector3(line, dh - inset * 2f, 0.001f));
			k.Box(brass, new Vector3(dw / 2f - inset, cy, lz), new Vector3(line, dh - inset * 2f, 0.001f));
			// Hinges on the left.
			foreach (float y in new[] { cy - 0.24f, cy + 0.24f })
			{
				k.Cylinder(steel, new Vector3(-dw / 2f - 0.012f, y, front + 0.03f), 0.02f, 0.13f, Vector3.up, 14);
				k.Cylinder(steel, new Vector3(-dw / 2f - 0.012f, y + 0.075f, front + 0.03f), 0.008f, 0.02f, Vector3.up, 8);
			}
			// The combination dial: a chrome bezel, the brass dial with its ticks, a knob.
			Vector3 dial = new Vector3(0.05f, cy + 0.17f, front + 0.048f);
			k.Cylinder(chrome, dial + new Vector3(0f, 0f, 0.004f), 0.078f, 0.008f, Vector3.forward, 32);
			k.Cylinder(brass, dial + new Vector3(0f, 0f, 0.017f), 0.064f, 0.022f, Vector3.forward, 32);
			k.Cylinder(brass, dial + new Vector3(0f, 0f, 0.04f), 0.03f, 0.03f, Vector3.forward, 20);
			for (int i = 0; i < 20; i++)
			{
				float a = i / 20f * Mathf.PI * 2f;
				Vector3 at = dial + new Vector3(Mathf.Cos(a) * 0.052f, Mathf.Sin(a) * 0.052f, 0.0285f);
				k.Box(black, at, new Vector3(0.003f, i % 5 == 0 ? 0.016f : 0.009f, 0.001f), Quaternion.Euler(0f, 0f, a * Mathf.Rad2Deg - 90f));
			}
			// The index mark above the dial.
			k.Box(brass, dial + new Vector3(0f, 0.09f, 0f), new Vector3(0.008f, 0.014f, 0.004f));
			// Handle: a hub and three spokes with ball ends.
			Vector3 hub = new Vector3(0.05f, cy - 0.12f, front + 0.06f);
			k.Cylinder(chrome, hub + new Vector3(0f, 0f, -0.006f), 0.03f, 0.03f, Vector3.forward, 20);
			for (int i = 0; i < 3; i++)
			{
				float a = (90f + i * 120f) * Mathf.Deg2Rad;
				Vector3 dir = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f);
				Quaternion rot = Quaternion.FromToRotation(Vector3.up, dir);
				k.Box(chrome, hub + dir * 0.07f + new Vector3(0f, 0f, 0.012f), new Vector3(0.014f, 0.11f, 0.014f), rot);
				k.Cylinder(chrome, hub + dir * 0.13f + new Vector3(0f, 0f, 0.012f), 0.016f, 0.03f, Vector3.forward, 14);
			}
			// Maker's plate.
			k.Box(brass, new Vector3(0.05f, cy + dh / 2f - 0.08f, front + 0.049f), new Vector3(0.2f, 0.045f, 0.003f));
			k.Box(black, new Vector3(0.05f, cy + dh / 2f - 0.08f, front + 0.0508f), new Vector3(0.17f, 0.006f, 0.0005f));
			k.Build(root);
		}

		// --- Storage crate: a slatted wooden crate on skids, steel corners,
		// handles at the ends and a lid that opens on a hinge at the back. ---

		public const float CrateWidth = 1.0f;
		public const float CrateHeight = 0.62f;
		public const float CrateDepth = 0.62f;
		// Where stored things are kept (out of sight, inside it).
		public static readonly Vector3 CrateInside = new Vector3(0f, 0.32f, 0f);

		public static GameObject Crate(GameObject root)
		{
			MeshKit k = new MeshKit();
			Material wood = Looks.Wood(), dark = Looks.DarkWood(), steel = Looks.Steel();
			float hw = CrateWidth / 2f, hd = CrateDepth / 2f, top = CrateHeight;
			// Skids.
			foreach (float z in new[] { -hd + 0.08f, hd - 0.08f })
			{
				k.Box(dark, new Vector3(0f, 0.04f, z), new Vector3(CrateWidth, 0.08f, 0.09f));
			}
			k.Box(wood, new Vector3(0f, 0.09f, 0f), new Vector3(CrateWidth - 0.02f, 0.02f, CrateDepth - 0.02f));
			// The inside (seen through the gaps): dark boards.
			k.Box(dark, new Vector3(0f, (top + 0.1f) / 2f, -hd + 0.03f), new Vector3(CrateWidth - 0.06f, top - 0.12f, 0.01f));
			k.Box(dark, new Vector3(0f, (top + 0.1f) / 2f, hd - 0.03f), new Vector3(CrateWidth - 0.06f, top - 0.12f, 0.01f));
			k.Box(dark, new Vector3(-hw + 0.03f, (top + 0.1f) / 2f, 0f), new Vector3(0.01f, top - 0.12f, CrateDepth - 0.06f));
			k.Box(dark, new Vector3(hw - 0.03f, (top + 0.1f) / 2f, 0f), new Vector3(0.01f, top - 0.12f, CrateDepth - 0.06f));
			// Slats: four a side, gaps between.
			int slats = 4;
			float y0 = 0.1f, slatH = 0.105f, gap = (top - 0.02f - y0 - slatH * slats) / (slats - 1);
			for (int i = 0; i < slats; i++)
			{
				float y = y0 + slatH / 2f + i * (slatH + gap);
				k.Box(wood, new Vector3(0f, y, hd - 0.01f), new Vector3(CrateWidth, slatH, 0.02f));
				k.Box(wood, new Vector3(0f, y, -hd + 0.01f), new Vector3(CrateWidth, slatH, 0.02f));
				k.Box(wood, new Vector3(hw - 0.01f, y, 0f), new Vector3(0.02f, slatH, CrateDepth - 0.04f));
				k.Box(wood, new Vector3(-hw + 0.01f, y, 0f), new Vector3(0.02f, slatH, CrateDepth - 0.04f));
			}
			// Frame boards over the slats: corner battens and a diagonal on the long sides.
			float bt = 0.022f, bwid = 0.07f, fy = (top + y0) / 2f, fh = top - y0 - 0.02f;
			foreach (float z in new[] { hd + bt / 2f, -hd - bt / 2f })
			{
				k.Box(dark, new Vector3(-hw + bwid / 2f, fy, z), new Vector3(bwid, fh, bt));
				k.Box(dark, new Vector3(hw - bwid / 2f, fy, z), new Vector3(bwid, fh, bt));
				k.Box(dark, new Vector3(0f, y0 + 0.035f, z), new Vector3(CrateWidth - bwid * 2f, bwid, bt));
				k.Box(dark, new Vector3(0f, top - 0.055f, z), new Vector3(CrateWidth - bwid * 2f, bwid, bt));
				Vector3 a = new Vector3(-hw + bwid, y0 + bwid, z), c = new Vector3(hw - bwid, top - 0.02f - bwid, z);
				Vector3 d = c - a;
				k.Box(dark, (a + c) / 2f, new Vector3(bwid * 0.8f, d.magnitude, bt), Quaternion.FromToRotation(Vector3.up, d.normalized));
			}
			foreach (float x in new[] { hw + bt / 2f, -hw - bt / 2f })
			{
				k.Box(dark, new Vector3(x, fy, -hd + bwid / 2f), new Vector3(bt, fh, bwid));
				k.Box(dark, new Vector3(x, fy, hd - bwid / 2f), new Vector3(bt, fh, bwid));
				// End handles: a steel bar on two standoffs.
				float hy = top - 0.17f;
				k.Cylinder(steel, new Vector3(x + Mathf.Sign(x) * 0.045f, hy, 0f), 0.012f, 0.22f, Vector3.forward, 10);
				foreach (float z in new[] { -0.1f, 0.1f })
				{
					k.Cylinder(steel, new Vector3(x + Mathf.Sign(x) * 0.024f, hy, z), 0.01f, 0.045f, Vector3.right, 8);
				}
			}
			// Steel corner caps at the top and bottom corners.
			foreach (float x in new[] { -hw, hw })
			{
				foreach (float z in new[] { -hd, hd })
				{
					foreach (float y in new[] { y0 + 0.03f, top - 0.05f })
					{
						k.Box(steel, new Vector3(x, y, z), new Vector3(0.075f, 0.06f, 0.075f));
					}
				}
			}
			k.Solid(new Vector3(0f, top / 2f, 0f), new Vector3(CrateWidth + 0.05f, top, CrateDepth + 0.05f));
			k.Build(root);

			// The lid, on a hinge along the back's top edge.
			GameObject lid = new GameObject("Lid");
			lid.transform.SetParent(root.transform, false);
			lid.transform.localPosition = new Vector3(0f, top, -hd - bt);
			MeshKit l = new MeshKit();
			int planks = 3;
			float pd = (CrateDepth + bt * 2f) / planks;
			for (int i = 0; i < planks; i++)
			{
				l.Box(wood, new Vector3(0f, 0.011f, pd * (i + 0.5f)), new Vector3(CrateWidth + bt * 2f, 0.022f, pd - 0.008f));
			}
			foreach (float x in new[] { -hw + 0.12f, hw - 0.12f })
			{
				l.Box(dark, new Vector3(x, 0.033f, (CrateDepth + bt * 2f) / 2f), new Vector3(0.07f, 0.022f, CrateDepth));
			}
			foreach (float x in new[] { -0.3f, 0.3f })
			{
				l.Box(steel, new Vector3(x, 0.012f, 0.015f), new Vector3(0.09f, 0.026f, 0.04f));
			}
			l.Build(lid);
			return lid;
		}
	}
}
