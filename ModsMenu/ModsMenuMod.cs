using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(ModsMenu.ModsMenuMod), "Mods Menu", "1.2.0", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace ModsMenu
{
	// Adds MODS to the main menu, between UPDATES and SETTINGS. The button is a
	// copy of the game's own UPDATES button (same font, hover animation and click
	// sound), and the mods page is built from copies of it too, in the same column:
	// one line per installed MelonLoader mod, then BACK.
	//
	// The menu is Unity UI (Michsky Dark UI kit) and the game ships its UI and
	// TextMeshPro assemblies itself, so those types are reached by reflection
	// instead of compiling against them.
	public partial class ModsMenuMod : MelonMod
	{
		private static MelonLogger.Instance log;

		// How long to keep looking for the menu after a scene loads (seconds).
		private const float LookFor = 180f;

		private Type buttonType;
		private Type layoutGroupType;
		private Type canvasType;
		private Type imageType;
		private readonly List<Type> textTypes = new List<Type>();

		private float nextLook;
		private float giveUpAt;
		private float dumpAt;
		private bool installed;
		private string sceneName = "";
		private string lastStatus = "";

		private const string ModsButtonName = "MODS (Mods Menu)";

		private Transform column;
		private GameObject template; // the UPDATES button
		private GameObject modsButton;

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			giveUpAt = Time.unscaledTime + LookFor;
			dumpAt = Time.unscaledTime + DumpAfter;
			InitPrefs();
			log.Msg("Loaded; looking for the main menu.");
		}

		// If the button isn't in after this long (seconds), write what the UI looks
		// like to UserData/ModsMenu/ so it can be worked out from the file.
		private const float DumpAfter = 10f;

		public override void OnSceneWasInitialized(int buildIndex, string name)
		{
			sceneName = name;
			lastStatus = "";
			dumpAt = Time.unscaledTime + DumpAfter;
			installed = false;
			ResetPages();
			nextLook = 0f;
			// Only menus need it; stop looking in a scene that has none (the level).
			giveUpAt = Time.unscaledTime + LookFor;
		}

		public override void OnUpdate()
		{
			UpdatePages();
			if (installed || Time.unscaledTime < nextLook || Time.unscaledTime > giveUpAt)
			{
				return;
			}
			if (dumpAt > 0f && Time.unscaledTime > dumpAt)
			{
				dumpAt = 0f;
				Dump();
			}
			// The menu can appear after a splash screen: keep looking a few times a second.
			nextLook = Time.unscaledTime + 0.5f;
			try
			{
				TryInstall();
			}
			catch (Exception e)
			{
				installed = true; // don't spam the log every half second
				log.Error("Couldn't add the MODS button: " + e);
			}
		}

		// --- Finding the menu. ---

		private bool FindTypes()
		{
			if (buttonType != null)
			{
				return true;
			}
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type t;
				if ((t = asm.GetType("UnityEngine.UI.Button")) != null)
				{
					buttonType = t;
				}
				if ((t = asm.GetType("UnityEngine.UI.LayoutGroup")) != null)
				{
					layoutGroupType = t;
				}
				if ((t = asm.GetType("UnityEngine.Canvas")) != null)
				{
					canvasType = t;
				}
				if ((t = asm.GetType("UnityEngine.UI.Image")) != null)
				{
					imageType = t;
				}
				if ((t = asm.GetType("UnityEngine.UI.Text")) != null)
				{
					textTypes.Add(t);
				}
				if ((t = asm.GetType("TMPro.TMP_Text")) != null)
				{
					textTypes.Add(t);
				}
			}
			return buttonType != null && textTypes.Count > 0;
		}

		private void TryInstall()
		{
			if (!FindTypes())
			{
				Status("Unity UI isn't loaded (no UnityEngine.UI.Button / text types found).");
				return;
			}
			List<Component> texts = VisibleTexts();
			List<Component> updatesLabels = texts.FindAll(c => Clean(GetText(c)) == "UPDATES");
			List<Component> settingsLabels = texts.FindAll(c => Clean(GetText(c)) == "SETTINGS");
			if (updatesLabels.Count == 0 || settingsLabels.Count == 0)
			{
				Status("Scene '" + sceneName + "': " + updatesLabels.Count + " UPDATES and " + settingsLabels.Count + " SETTINGS labels among " + texts.Count + " texts.");
				return;
			}
			// Dark UI keeps every panel active (it fades them), so the Updates and
			// Settings panels' own titles count as visible too: try every pair and
			// take the one whose entries sit in the same column. The panels may sit
			// side by side too, so of all such pairs take the smallest entries:
			// buttons, not whole panels.
			Transform bestUpdates = null, bestSettings = null;
			int bestSize = int.MaxValue;
			foreach (Component u in updatesLabels)
			{
				foreach (Component st in settingsLabels)
				{
					Transform updates, settings;
					if (!Entries(u.transform, st.transform, out updates, out settings))
					{
						continue;
					}
					int size = updates.GetComponentsInChildren<Transform>(true).Length + settings.GetComponentsInChildren<Transform>(true).Length;
					if (size < bestSize)
					{
						bestSize = size;
						bestUpdates = updates;
						bestSettings = settings;
					}
				}
			}
			if (bestUpdates != null)
			{
				Install(bestUpdates, bestSettings);
				return;
			}
			Status("Scene '" + sceneName + "': found UPDATES (" + Paths(updatesLabels) + ") and SETTINGS (" + Paths(settingsLabels) + ") but not in one column.");
		}

		private void Install(Transform updates, Transform settings)
		{
			column = updates.parent;
			template = updates.gameObject;
			// Already there (the menu can report its scene as loaded twice): keep it.
			Transform existing = column.Find(ModsButtonName);
			if (existing != null)
			{
				modsButton = existing.gameObject;
				installed = true;
				return;
			}
			modsButton = MakeButton("MODS", OpenPage);
			modsButton.name = ModsButtonName;
			InsertBefore(modsButton.transform, settings);
			installed = true;
			log.Msg("Added MODS to the main menu (" + PathOf(column) + ", " + (HasLayoutGroup() ? "layout group" : "placed by hand") + ").");
		}

		// Log what's in the way, once per change rather than twice a second.
		private void Status(string status)
		{
			if (status != lastStatus)
			{
				lastStatus = status;
				log.Msg(status);
			}
		}

		private List<Component> VisibleTexts()
		{
			List<Component> found = new List<Component>();
			foreach (Type type in textTypes)
			{
				foreach (Object o in Resources.FindObjectsOfTypeAll(type))
				{
					Component c = o as Component;
					if (c != null && c.gameObject.scene.IsValid() && c.gameObject.activeInHierarchy)
					{
						found.Add(c);
					}
				}
			}
			return found;
		}

		// "<b>Updates</b> " -> "UPDATES"
		private static string Clean(string text)
		{
			return Regex.Replace(Regex.Replace(text ?? "", "<[^>]*>", ""), "\\s+", " ").Trim().ToUpperInvariant();
		}

		private static string PathOf(Transform t)
		{
			string path = t.name;
			for (Transform p = t.parent; p != null; p = p.parent)
			{
				path = p.name + "/" + path;
			}
			return path;
		}

		private static string Paths(List<Component> list)
		{
			List<string> paths = new List<string>();
			foreach (Component c in list)
			{
				paths.Add(PathOf(c.transform));
			}
			return string.Join("; ", paths.ToArray());
		}

		// The column both labels' entries sit in, and each label's entry in it
		// (the child of the column holding the label: its button, or a holder
		// around the button). The two must be close: same column, next to each
		// other or one entry apart, so a SETTINGS somewhere else doesn't match.
		private bool Entries(Transform a, Transform b, out Transform entryA, out Transform entryB)
		{
			entryA = entryB = null;
			Component button = a.GetComponentInParent(buttonType);
			Transform start = button != null ? button.transform : a;
			for (Transform t = start; t != null && t.parent != null; t = t.parent)
			{
				if (b.IsChildOf(t.parent) && !b.IsChildOf(t))
				{
					entryA = t;
					entryB = b;
					while (entryB.parent != t.parent)
					{
						entryB = entryB.parent;
					}
					// Next to each other, give or take a separator or two.
					return Mathf.Abs(entryA.GetSiblingIndex() - entryB.GetSiblingIndex()) <= 4;
				}
			}
			return false;
		}

		// --- Building entries. ---

		private GameObject MakeButton(string label, UnityAction onClick, Transform parent = null)
		{
			GameObject copy = Object.Instantiate(template, parent ?? column, false);
			copy.SetActive(true);
			foreach (Component text in Texts(copy))
			{
				SetText(text, label);
			}
			Component button = copy.GetComponent(buttonType) ?? copy.GetComponentInChildren(buttonType, true);
			if (button != null)
			{
				// A fresh click event: the copy must not also do what UPDATES does.
				PropertyInfo onClickProp = buttonType.GetProperty("onClick");
				UnityEvent click = (UnityEvent)Activator.CreateInstance(onClickProp.PropertyType);
				click.AddListener(onClick);
				onClickProp.SetValue(button, click, null);
			}
			// Anything else on the copy that reacts to clicks the way UPDATES did.
			foreach (Component c in copy.GetComponentsInChildren<Component>(true))
			{
				if (c != null && c.GetType().Name == "EventTrigger")
				{
					Object.Destroy(c);
				}
			}
			return copy;
		}

		// Put `item` where `before` is and move `before` and what follows down a slot.
		private void InsertBefore(Transform item, Transform before)
		{
			item.SetSiblingIndex(before.GetSiblingIndex());
			if (HasLayoutGroup())
			{
				return; // the layout places it
			}
			RectTransform a = template.transform as RectTransform;
			RectTransform b = before as RectTransform;
			RectTransform r = item as RectTransform;
			if (a == null || b == null || r == null)
			{
				return;
			}
			// One slot: from UPDATES to the entry after it.
			Vector2 step = b.anchoredPosition - a.anchoredPosition;
			Vector2 start = b.anchoredPosition;
			foreach (Transform sibling in column)
			{
				RectTransform s = sibling as RectTransform;
				if (s == null || s == r || s == a)
				{
					continue;
				}
				// At or past `before` along the column's direction.
				if (Vector2.Dot(s.anchoredPosition - start, step) >= -0.01f)
				{
					s.anchoredPosition += step;
				}
			}
			r.anchoredPosition = start;
		}

		private bool HasLayoutGroup()
		{
			return layoutGroupType != null && column.GetComponent(layoutGroupType) != null;
		}

		// --- Diagnostics. ---

		// Every UI object in the loaded scenes, with its components and text, to
		// UserData/ModsMenu/ui_<scene>.txt.
		private void Dump()
		{
			try
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("Mods Menu UI dump, scene '" + sceneName + "', status: " + lastStatus);
				int lines = 0;
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					foreach (GameObject root in SceneManager.GetSceneAt(i).GetRootGameObjects())
					{
						if (root.GetComponentInChildren<RectTransform>(true) != null)
						{
							DumpObject(root.transform, 0, sb, ref lines);
						}
					}
				}
				string dir = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"), "ModsMenu");
				Directory.CreateDirectory(dir);
				string file = Path.Combine(dir, "ui_" + Regex.Replace(sceneName, "[^A-Za-z0-9_-]", "_") + ".txt");
				File.WriteAllText(file, sb.ToString());
				log.Msg("MODS isn't in yet; wrote the menu layout to " + file);
			}
			catch (Exception e)
			{
				log.Warning("Couldn't write the UI dump: " + e.Message);
			}
		}

		private void DumpObject(Transform t, int depth, StringBuilder sb, ref int lines)
		{
			if (lines++ > 20000)
			{
				return;
			}
			sb.Append(' ', depth * 2).Append(t.gameObject.activeSelf ? "+ " : "- ").Append(t.name).Append("  [");
			bool first = true;
			string text = null;
			foreach (Component c in t.GetComponents<Component>())
			{
				if (c == null)
				{
					continue;
				}
				sb.Append(first ? "" : ", ").Append(c.GetType().Name);
				first = false;
				foreach (Type type in textTypes)
				{
					if (type.IsInstanceOfType(c))
					{
						text = GetText(c);
					}
				}
			}
			sb.Append(']');
			RectTransform rt = t as RectTransform;
			if (rt != null)
			{
				sb.Append("  pos ").Append(rt.anchoredPosition.ToString("F0")).Append(" size ").Append(rt.sizeDelta.ToString("F0"));
			}
			if (text != null)
			{
				sb.Append("  text \"").Append(text.Replace("\n", "\\n")).Append('"');
			}
			sb.AppendLine();
			foreach (Transform child in t)
			{
				DumpObject(child, depth + 1, sb, ref lines);
			}
		}

		// --- Text by reflection (UnityEngine.UI.Text or TextMeshPro). ---

		private List<Component> Texts(GameObject root)
		{
			List<Component> found = new List<Component>();
			foreach (Type type in textTypes)
			{
				foreach (Component c in root.GetComponentsInChildren(type, true))
				{
					found.Add(c);
				}
			}
			return found;
		}

		private static string GetText(Component c)
		{
			PropertyInfo p = c.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
			object v = p != null ? p.GetValue(c, null) : null;
			return v as string ?? "";
		}

		private static void SetText(Component c, string text)
		{
			PropertyInfo p = c.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
			if (p != null && p.CanWrite)
			{
				p.SetValue(c, text, null);
			}
		}

		private static void ScaleFont(Component c, float factor)
		{
			// TextMeshPro may size text to fit its box; fix the size instead.
			PropertyInfo auto = c.GetType().GetProperty("enableAutoSizing", BindingFlags.Instance | BindingFlags.Public);
			if (auto != null && auto.CanWrite)
			{
				auto.SetValue(c, false, null);
			}
			PropertyInfo p = c.GetType().GetProperty("fontSize", BindingFlags.Instance | BindingFlags.Public);
			if (p == null || !p.CanWrite)
			{
				return;
			}
			if (p.PropertyType == typeof(int))
			{
				p.SetValue(c, Mathf.Max(1, Mathf.RoundToInt((int)p.GetValue(c, null) * factor)), null);
			}
			else if (p.PropertyType == typeof(float))
			{
				p.SetValue(c, (float)p.GetValue(c, null) * factor, null);
			}
		}
	}
}
