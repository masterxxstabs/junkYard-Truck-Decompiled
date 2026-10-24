using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FurnitureTweaks
{
	// A crate's grid on screen, Tarkov style: each part a tile of its size with a
	// picture of it. Drag a tile to move it (R turns it while you drag); let go
	// of it outside the window, or right-click it, to take it out into your hands.
	// The open key or the X closes it; so does walking away.
	internal static class CrateWindow
	{
		private static StorageCrate crate;
		private static int openedFrame;

		// Dragging a tile.
		private static StorageCrate.Slot dragging;
		private static bool dragTurned;
		private static Vector2 grabOffset;    // from the tile's top-left, in cells

		private static Rect window, grid;
		private static float cell;
		private static StorageCrate.Slot hover;

		// What the window turned off while it's open.
		private static Behaviour fpc;
		private static Behaviour interactor;
		private static int eventMask;

		private static GUIStyle title, small, tiny, footer;
		private static Texture2D white;

		public static bool Open
		{
			get { return crate != null; }
		}

		public static bool IsOpen(StorageCrate c)
		{
			return crate != null && crate == c;
		}

		public static void Show(StorageCrate c)
		{
			crate = c;
			openedFrame = Time.frameCount;
			dragging = null;
			Interactor i = Object.FindObjectOfType<Interactor>();
			interactor = i;
			FieldInfo f = i != null ? AccessTools.Field(i.GetType(), "fpc") : null;
			fpc = f != null ? f.GetValue(i) as Behaviour : null;
			// Looking around and the game's clicking stop while you sort the crate.
			if (fpc != null)
			{
				fpc.enabled = false;
			}
			if (interactor != null)
			{
				interactor.enabled = false;
			}
			Camera cam = Camera.main;
			if (cam != null)
			{
				eventMask = cam.eventMask;
				cam.eventMask = 0;
			}
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		public static void Close()
		{
			if (crate == null)
			{
				return;
			}
			crate = null;
			dragging = null;
			if (interactor != null)
			{
				interactor.enabled = true;
			}
			if (fpc != null)
			{
				fpc.enabled = true;
				MethodInfo lockMouse = AccessTools.Method(fpc.GetType(), "LockMouse");
				if (lockMouse != null)
				{
					try
					{
						lockMouse.Invoke(fpc, null);
					}
					catch (Exception)
					{
					}
				}
			}
			Camera cam = Camera.main;
			if (cam != null)
			{
				cam.eventMask = eventMask;
			}
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		public static void Update()
		{
			if (crate == null)
			{
				return;
			}
			Camera cam = Camera.main;
			Component i = interactor;
			if (cam == null || i == null || (crate.transform.position - i.transform.position).sqrMagnitude > 4f * 4f)
			{
				Close();
				return;
			}
			crate.Tidy();
			// Something else may lock the cursor again.
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			// Pictures of the parts, a couple a frame.
			int made = 0;
			foreach (StorageCrate.Slot s in crate.slots)
			{
				if (s.item != null && !Icons.Has(s.item) && made++ < 2)
				{
					Icons.Get(s.item, s.w, s.h);
				}
			}
		}

		// --- Drawing and the mouse. ---

		public static void Draw()
		{
			if (crate == null)
			{
				return;
			}
			Styles();
			cell = Mathf.Clamp(Mathf.Round(Screen.height * 0.07f), 40f, 84f);
			float pad = 14f, head = 34f, foot = 30f;
			float w = StorageCrate.Cols * cell + pad * 2f, h = StorageCrate.Rows * cell + head + foot + pad * 2f;
			window = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);
			grid = new Rect(window.x + pad, window.y + pad + head, StorageCrate.Cols * cell, StorageCrate.Rows * cell);
			Event e = Event.current;
			Vector2 mouse = e.mousePosition;

			// Frame.
			Fill(new Rect(window.x - 2, window.y - 2, window.width + 4, window.height + 4), new Color(0.32f, 0.33f, 0.3f, 1f));
			Fill(window, new Color(0.07f, 0.075f, 0.08f, 0.97f));
			GUI.Label(new Rect(window.x + pad, window.y + 8f, window.width - 80f, 24f), "STORAGE CRATE   <color=#9a9a8a>" + crate.Count + " parts · " + crate.UsedCells + " / " + StorageCrate.Cols * StorageCrate.Rows + " cells</color>", title);
			Rect x = new Rect(window.xMax - pad - 26f, window.y + 8f, 26f, 22f);
			if (GUI.Button(x, "X"))
			{
				Close();
				return;
			}

			// Cells.
			Fill(grid, new Color(0.04f, 0.045f, 0.05f, 1f));
			Color line = new Color(0.2f, 0.21f, 0.22f, 1f);
			for (int c = 0; c <= StorageCrate.Cols; c++)
			{
				Fill(new Rect(grid.x + c * cell, grid.y, 1f, grid.height), line);
			}
			for (int r = 0; r <= StorageCrate.Rows; r++)
			{
				Fill(new Rect(grid.x, grid.y + r * cell, grid.width, 1f), line);
			}

			// Tiles.
			hover = null;
			foreach (StorageCrate.Slot s in crate.slots)
			{
				if (s.item == null || s == dragging || s.x < 0)
				{
					continue;
				}
				Rect r = new Rect(grid.x + s.x * cell, grid.y + s.y * cell, s.W * cell, s.H * cell);
				if (r.Contains(mouse) && dragging == null)
				{
					hover = s;
				}
				Tile(s, r, s.turned, hover == s, 1f);
			}

			// The tile being dragged, and where it would go.
			if (dragging != null)
			{
				int tw = dragTurned ? dragging.h : dragging.w, th = dragTurned ? dragging.w : dragging.h;
				Vector2 topLeft = mouse - grabOffset * cell;
				int gx = Mathf.RoundToInt((topLeft.x - grid.x) / cell), gy = Mathf.RoundToInt((topLeft.y - grid.y) / cell);
				if (window.Contains(mouse))
				{
					bool fits = crate.Fits(gx, gy, tw, th, dragging);
					Rect ghost = new Rect(grid.x + gx * cell, grid.y + gy * cell, tw * cell, th * cell);
					ghost = Clip(ghost, grid);
					Fill(ghost, fits ? new Color(0.25f, 0.6f, 0.25f, 0.45f) : new Color(0.7f, 0.2f, 0.2f, 0.45f));
				}
				else
				{
					GUI.Label(new Rect(mouse.x + 14f, mouse.y + 14f, 200f, 20f), "Let go: take it out", small);
				}
				Tile(dragging, new Rect(topLeft.x, topLeft.y, tw * cell, th * cell), dragTurned, true, 0.85f);
			}

			// Hover: what it is.
			if (hover != null && hover.item != null)
			{
				string tip = Items.Label(hover.item) + "  ·  " + Mathf.RoundToInt(hover.item.thisDurability) + "%  ·  " + hover.w + " x " + hover.h;
				Vector2 size = small.CalcSize(new GUIContent(tip));
				Rect tr = new Rect(mouse.x + 16f, mouse.y + 18f, size.x + 12f, 22f);
				Fill(tr, new Color(0f, 0f, 0f, 0.85f));
				GUI.Label(new Rect(tr.x + 6f, tr.y + 2f, size.x, 20f), tip, small);
			}

			GUI.Label(new Rect(window.x + pad, window.yMax - foot - 4f, window.width - pad * 2f, foot), "Drag to move · [R] while dragging turns it · right-click, or drag it out of the window, to take it · [" + FurnitureMod.TakeKey + "] or X closes", footer);

			HandleInput(e, mouse);
		}

		private static void HandleInput(Event e, Vector2 mouse)
		{
			if (e.type == EventType.KeyDown && Time.frameCount != openedFrame)
			{
				if (e.keyCode == KeyCode.R && dragging != null)
				{
					// Turn it about the middle of the tile.
					dragTurned = !dragTurned;
					int tw = dragTurned ? dragging.h : dragging.w, th = dragTurned ? dragging.w : dragging.h;
					grabOffset = new Vector2(tw / 2f, th / 2f);
					e.Use();
					return;
				}
				if (e.keyCode == FurnitureMod.TakeKey && dragging == null)
				{
					e.Use();
					Close();
					return;
				}
			}
			if (e.type == EventType.MouseDown && hover != null)
			{
				if (e.button == 1)
				{
					StorageCrate c = crate;
					StorageCrate.Slot s = hover;
					e.Use();
					Close();
					c.TakeOut(s, Camera.main);
					return;
				}
				if (e.button == 0)
				{
					dragging = hover;
					dragTurned = hover.turned;
					grabOffset = new Vector2((mouse.x - (grid.x + hover.x * cell)) / cell, (mouse.y - (grid.y + hover.y * cell)) / cell);
					e.Use();
					return;
				}
			}
			if (e.type == EventType.MouseUp && e.button == 0 && dragging != null)
			{
				StorageCrate.Slot s = dragging;
				dragging = null;
				e.Use();
				if (!window.Contains(mouse))
				{
					StorageCrate c = crate;
					Close();
					c.TakeOut(s, Camera.main);
					return;
				}
				int tw = dragTurned ? s.h : s.w, th = dragTurned ? s.w : s.h;
				Vector2 topLeft = mouse - grabOffset * cell;
				int gx = Mathf.RoundToInt((topLeft.x - grid.x) / cell), gy = Mathf.RoundToInt((topLeft.y - grid.y) / cell);
				if (crate.Fits(gx, gy, tw, th, s))
				{
					s.x = gx;
					s.y = gy;
					s.turned = dragTurned;
				}
			}
		}

		// One part's tile: its picture (turned with the tile), its name and wear.
		private static void Tile(StorageCrate.Slot s, Rect r, bool turned, bool lit, float alpha)
		{
			Rect inner = new Rect(r.x + 1f, r.y + 1f, r.width - 1f, r.height - 1f);
			Fill(inner, new Color(0.16f, 0.18f, 0.19f, 0.92f * alpha));
			Fill(new Rect(inner.x, inner.y, inner.width, 1f), new Color(0.42f, 0.44f, 0.42f, alpha));
			if (lit)
			{
				Fill(inner, new Color(1f, 1f, 1f, 0.06f));
			}
			Texture2D icon = Icons.Get(s.item, s.w, s.h, false);
			if (icon != null)
			{
				Rect pic = new Rect(inner.x + 3f, inner.y + 3f, inner.width - 6f, inner.height - 6f);
				Color was = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, alpha);
				if (turned)
				{
					// The picture is drawn for the unturned tile; turn it a quarter
					// about the tile's middle.
					Matrix4x4 m = GUI.matrix;
					Vector2 mid = pic.center;
					GUIUtility.RotateAroundPivot(90f, mid);
					GUI.DrawTexture(new Rect(mid.x - pic.height / 2f, mid.y - pic.width / 2f, pic.height, pic.width), icon, ScaleMode.ScaleToFit);
					GUI.matrix = m;
				}
				else
				{
					GUI.DrawTexture(pic, icon, ScaleMode.ScaleToFit);
				}
				GUI.color = was;
			}
			string name = Items.Label(s.item);
			GUI.Label(new Rect(inner.x + 4f, inner.y + 1f, inner.width - 6f, 16f), name, tiny);
			float d = s.item.thisDurability;
			string wear = Mathf.RoundToInt(d) + "%";
			Color wc = d > 60f ? new Color(0.55f, 0.85f, 0.5f) : d > 25f ? new Color(0.9f, 0.8f, 0.35f) : new Color(0.9f, 0.4f, 0.35f);
			tiny.normal.textColor = wc;
			GUI.Label(new Rect(inner.x + 2f, inner.yMax - 16f, inner.width - 6f, 16f), wear, Right(tiny));
			tiny.normal.textColor = new Color(0.85f, 0.85f, 0.8f);
		}

		private static GUIStyle right;

		private static GUIStyle Right(GUIStyle s)
		{
			if (right == null)
			{
				right = new GUIStyle(s);
				right.alignment = TextAnchor.LowerRight;
			}
			right.normal.textColor = s.normal.textColor;
			return right;
		}

		private static Rect Clip(Rect r, Rect to)
		{
			float x0 = Mathf.Max(r.x, to.x), y0 = Mathf.Max(r.y, to.y);
			float x1 = Mathf.Min(r.xMax, to.xMax), y1 = Mathf.Min(r.yMax, to.yMax);
			return new Rect(x0, y0, Mathf.Max(0f, x1 - x0), Mathf.Max(0f, y1 - y0));
		}

		private static void Fill(Rect r, Color c)
		{
			if (white == null)
			{
				white = Texture2D.whiteTexture;
			}
			Color was = GUI.color;
			GUI.color = c;
			GUI.DrawTexture(r, white);
			GUI.color = was;
		}

		private static void Styles()
		{
			if (title != null)
			{
				return;
			}
			title = new GUIStyle(GUI.skin.label);
			title.fontSize = 17;
			title.fontStyle = FontStyle.Bold;
			title.richText = true;
			title.normal.textColor = new Color(0.86f, 0.86f, 0.8f);
			small = new GUIStyle(GUI.skin.label);
			small.fontSize = 13;
			small.normal.textColor = new Color(0.9f, 0.9f, 0.85f);
			tiny = new GUIStyle(GUI.skin.label);
			tiny.fontSize = 11;
			tiny.clipping = TextClipping.Clip;
			tiny.wordWrap = false;
			tiny.normal.textColor = new Color(0.85f, 0.85f, 0.8f);
			footer = new GUIStyle(small);
			footer.fontSize = 12;
			footer.wordWrap = true;
			footer.normal.textColor = new Color(0.6f, 0.6f, 0.55f);
		}
	}

	// Pictures of parts for the crate's tiles: the part's own meshes and
	// materials, copied onto a layer of their own far below the world, filmed
	// from the side that shows its two longest sides, long side across.
	internal static class Icons
	{
		private const int Layer = 31;
		private const int PixelsPerCell = 64;
		private static readonly Dictionary<int, Texture2D> made = new Dictionary<int, Texture2D>();

		public static void Reset()
		{
			foreach (Texture2D t in made.Values)
			{
				if (t != null)
				{
					Object.Destroy(t);
				}
			}
			made.Clear();
		}

		public static bool Has(PickUp p)
		{
			return p != null && made.ContainsKey(p.GetInstanceID());
		}

		// The picture (made now unless `make` is off, then null until it's made).
		public static Texture2D Get(PickUp p, int w, int h, bool make = true)
		{
			if (p == null)
			{
				return null;
			}
			Texture2D t;
			if (made.TryGetValue(p.GetInstanceID(), out t) || !make)
			{
				return t;
			}
			try
			{
				t = Render(p.transform, w, h);
			}
			catch (Exception e)
			{
				FurnitureMod.Log("Couldn't draw " + p.name + ": " + e.Message);
				t = null;
			}
			made[p.GetInstanceID()] = t;
			return t;
		}

		private static float Nz(float v)
		{
			return Mathf.Abs(v) < 0.0001f ? 1f : v;
		}

		private static Texture2D Render(Transform part, int w, int h)
		{
			GameObject rig = new GameObject("FurnitureTweaks_IconRig");
			rig.transform.position = new Vector3(0f, -3000f, 0f);
			rig.transform.rotation = Quaternion.identity;
			// Posed in the part's own units (its scale on the rig).
			rig.transform.localScale = part.lossyScale;
			Camera cam = null;
			RenderTexture rt = null;
			RenderTexture was = RenderTexture.active;
			try
			{
				// The part's visible meshes, posed as they are on the part.
				int copied = 0;
				foreach (MeshFilter mf in part.GetComponentsInChildren<MeshFilter>(true))
				{
					MeshRenderer mr = mf.GetComponent<MeshRenderer>();
					if (mf.sharedMesh == null || mr == null || !mr.enabled || !Items.ShownUnder(mf.transform, part))
					{
						continue;
					}
					GameObject c = new GameObject("m");
					c.layer = Layer;
					c.transform.SetParent(rig.transform, false);
					c.transform.localPosition = part.InverseTransformPoint(mf.transform.position);
					c.transform.localRotation = Quaternion.Inverse(part.rotation) * mf.transform.rotation;
					Vector3 ps = part.lossyScale, ms = mf.transform.lossyScale;
					c.transform.localScale = new Vector3(ms.x / Nz(ps.x), ms.y / Nz(ps.y), ms.z / Nz(ps.z));
					c.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
					MeshRenderer r = c.AddComponent<MeshRenderer>();
					r.sharedMaterials = mr.sharedMaterials;
					r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					r.receiveShadows = false;
					copied++;
				}
				if (copied == 0)
				{
					return null;
				}
				Bounds b = Items.LocalBounds(part);
				Vector3 size = Vector3.Scale(b.size, new Vector3(Mathf.Abs(part.lossyScale.x), Mathf.Abs(part.lossyScale.y), Mathf.Abs(part.lossyScale.z)));
				// Long side across, the next one up, filmed along the shortest.
				Vector3[] axes = { Vector3.right, Vector3.up, Vector3.forward };
				float[] len = { size.x, size.y, size.z };
				int[] order = { 0, 1, 2 };
				Array.Sort((float[])len.Clone(), order);
				Vector3 across = axes[order[2]], up = axes[order[1]], view = axes[order[0]];
				Vector3 center = rig.transform.TransformPoint(b.center);
				float depth = Mathf.Max(size.x, Mathf.Max(size.y, size.z));

				int pw = Mathf.Clamp(w * PixelsPerCell, 32, 640), ph = Mathf.Clamp(h * PixelsPerCell, 32, 512);
				rt = RenderTexture.GetTemporary(pw, ph, 24, RenderTextureFormat.ARGB32);
				rt.antiAliasing = 1;
				GameObject camGo = new GameObject("cam");
				camGo.transform.SetParent(rig.transform, false);
				cam = camGo.AddComponent<Camera>();
				cam.enabled = false;
				cam.orthographic = true;
				cam.clearFlags = CameraClearFlags.SolidColor;
				cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
				cam.cullingMask = 1 << Layer;
				cam.nearClipPlane = 0.01f;
				cam.farClipPlane = depth * 4f + 2f;
				cam.allowHDR = false;
				cam.allowMSAA = false;
				cam.targetTexture = rt;
				cam.aspect = pw / (float)ph;
				// Seen at a slight angle, so it doesn't look flat.
				Quaternion tilt = Quaternion.AngleAxis(18f, across) * Quaternion.AngleAxis(-12f, up);
				Vector3 look = tilt * -view;
				camGo.transform.position = center - look * (depth * 2f + 1f);
				camGo.transform.rotation = Quaternion.LookRotation(look, up);
				float across2 = Vector3.Scale(size, across).magnitude, up2 = Vector3.Scale(size, up).magnitude;
				cam.orthographicSize = Mathf.Max(up2 / 2f, across2 / 2f / cam.aspect) * 1.12f + 0.005f;
				// Its own light, from over the camera's shoulder.
				GameObject lightGo = new GameObject("light");
				lightGo.transform.SetParent(rig.transform, false);
				lightGo.transform.rotation = Quaternion.LookRotation(look + Vector3.down * 0.6f + camGo.transform.right * 0.4f);
				Light light = lightGo.AddComponent<Light>();
				light.type = LightType.Directional;
				light.intensity = 1.1f;
				light.cullingMask = 1 << Layer;
				cam.Render();
				RenderTexture.active = rt;
				Texture2D t = new Texture2D(pw, ph, TextureFormat.RGBA32, false);
				t.ReadPixels(new Rect(0, 0, pw, ph), 0, 0);
				t.Apply(false, true);
				return t;
			}
			finally
			{
				RenderTexture.active = was;
				if (cam != null)
				{
					cam.targetTexture = null;
				}
				if (rt != null)
				{
					RenderTexture.ReleaseTemporary(rt);
				}
				Object.DestroyImmediate(rig);
			}
		}
	}
}
