using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(ModsMenu.ModsMenuMod), "Mods Menu", "1.0.0", "masterxxstabs")]
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
	public class ModsMenuMod : MelonMod
	{
		private static MelonLogger.Instance log;

		// How long to keep looking for the menu after a scene loads (seconds).
		private const float LookFor = 180f;

		private Type buttonType;
		private Type layoutGroupType;
		private readonly List<Type> textTypes = new List<Type>();

		private float nextLook;
		private float giveUpAt;
		private bool installed;

		private Transform column;
		private GameObject template; // the UPDATES button
		private GameObject modsButton;
		private readonly List<GameObject> pageItems = new List<GameObject>();
		private readonly List<GameObject> hiddenForPage = new List<GameObject>();
		private bool pageOpen;

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			giveUpAt = Time.unscaledTime + LookFor;
		}

		public override void OnSceneWasInitialized(int buildIndex, string sceneName)
		{
			installed = false;
			pageOpen = false;
			pageItems.Clear();
			hiddenForPage.Clear();
			nextLook = 0f;
			// Only menus need it; stop looking in a scene that has none (the level).
			giveUpAt = Time.unscaledTime + LookFor;
		}

		public override void OnUpdate()
		{
			if (pageOpen && Input.GetKeyDown(KeyCode.Escape))
			{
				ClosePage();
			}
			if (installed || Time.unscaledTime < nextLook || Time.unscaledTime > giveUpAt)
			{
				return;
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
				return;
			}
			Component updatesText = FindLabel("UPDATES");
			Component settingsText = FindLabel("SETTINGS");
			if (updatesText == null || settingsText == null)
			{
				return;
			}
			Transform updates, settings;
			if (!Entries(updatesText.transform, settingsText.transform, out updates, out settings))
			{
				return; // not the main menu's button column
			}
			column = updates.parent;
			template = updates.gameObject;
			modsButton = MakeButton("MODS", OpenPage);
			modsButton.name = "MODS (Mods Menu)";
			InsertBefore(modsButton.transform, settings);
			installed = true;
			log.Msg("Added MODS to the main menu.");
		}

		// A visible text in a loaded scene that reads exactly `label`.
		private Component FindLabel(string label)
		{
			foreach (Type type in textTypes)
			{
				foreach (Object o in Resources.FindObjectsOfTypeAll(type))
				{
					Component c = o as Component;
					if (c != null && c.gameObject.scene.IsValid() && c.gameObject.activeInHierarchy && GetText(c).Trim().ToUpperInvariant() == label)
					{
						return c;
					}
				}
			}
			return null;
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
					return Mathf.Abs(entryA.GetSiblingIndex() - entryB.GetSiblingIndex()) <= 3;
				}
			}
			return false;
		}

		// --- Building entries. ---

		private GameObject MakeButton(string label, UnityAction onClick)
		{
			GameObject copy = Object.Instantiate(template, column, false);
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

		// A line on the mods page: a copy of the button in smaller type that does nothing.
		private GameObject MakeLine(string label, float size)
		{
			GameObject line = MakeButton(label, delegate { });
			foreach (Component text in Texts(line))
			{
				ScaleFont(text, size);
			}
			RectTransform rt = line.transform as RectTransform;
			if (rt != null)
			{
				rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y * size);
			}
			return line;
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

		// --- The mods page. ---

		private void OpenPage()
		{
			if (pageOpen)
			{
				return;
			}
			hiddenForPage.Clear();
			foreach (Transform child in column)
			{
				if (child.gameObject.activeSelf)
				{
					hiddenForPage.Add(child.gameObject);
				}
			}
			RectTransform first = template.transform as RectTransform;
			Vector2 firstPos = first != null ? first.anchoredPosition : Vector2.zero;
			Vector2 slot = Slot();
			foreach (GameObject g in hiddenForPage)
			{
				g.SetActive(false);
			}

			List<string> lines = new List<string>();
			foreach (MelonBase melon in MelonMod.RegisteredMelons)
			{
				if (melon == null || melon.Info == null)
				{
					continue;
				}
				string line = melon.Info.Name.ToUpperInvariant() + "  " + melon.Info.Version;
				if (!string.IsNullOrEmpty(melon.Info.Author))
				{
					line += "  -  " + melon.Info.Author;
				}
				lines.Add(line);
			}
			lines.Sort(StringComparer.OrdinalIgnoreCase);

			const float LineSize = 0.5f;
			Vector2 pos = firstPos;
			GameObject header = MakeLine(lines.Count + (lines.Count == 1 ? " MOD INSTALLED" : " MODS INSTALLED"), 0.65f);
			Place(header, ref pos, slot * 0.75f);
			foreach (string line in lines)
			{
				Place(MakeLine(line, LineSize), ref pos, slot * 0.55f);
			}
			pos += slot * 0.35f;
			Place(MakeButton("BACK", ClosePage), ref pos, slot);
			pageOpen = true;
		}

		private void Place(GameObject item, ref Vector2 pos, Vector2 advance)
		{
			pageItems.Add(item);
			item.transform.SetAsLastSibling();
			if (HasLayoutGroup())
			{
				return;
			}
			RectTransform r = item.transform as RectTransform;
			if (r != null)
			{
				r.anchoredPosition = pos;
			}
			pos += advance;
		}

		// The distance between two menu entries (UPDATES to MODS).
		private Vector2 Slot()
		{
			RectTransform a = template.transform as RectTransform;
			RectTransform b = modsButton != null ? modsButton.transform as RectTransform : null;
			if (a != null && b != null && (b.anchoredPosition - a.anchoredPosition).sqrMagnitude > 1f)
			{
				return b.anchoredPosition - a.anchoredPosition;
			}
			return new Vector2(0f, -60f);
		}

		private void ClosePage()
		{
			if (!pageOpen)
			{
				return;
			}
			foreach (GameObject g in pageItems)
			{
				if (g != null)
				{
					Object.Destroy(g);
				}
			}
			pageItems.Clear();
			foreach (GameObject g in hiddenForPage)
			{
				if (g != null)
				{
					g.SetActive(true);
				}
			}
			hiddenForPage.Clear();
			pageOpen = false;
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
