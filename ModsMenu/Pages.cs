using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace ModsMenu
{
	// The pages MODS opens, drawn in the menu's own button column with copies of
	// the UPDATES button:
	//
	//   Mods page      one line per installed mod; a mod with settings opens its
	//                  settings page.
	//   Settings page  every MelonPreferences entry of that mod, one per line as
	//                  "NAME: VALUE". Click a setting to change it: on/off flips,
	//                  a key waits for the next key press, a choice steps to the
	//                  next option, a number or text is typed (Enter keeps it,
	//                  Esc cancels). Changes save to UserData/MelonPreferences.cfg
	//                  straight away.
	//
	// Any mod that keeps its settings in MelonPreferences shows up with no changes
	// of its own: a mod's settings are the categories named like the mod (its
	// name, namespace or assembly, ignoring case, spaces and punctuation). Hidden
	// categories and entries are left out.
	public partial class ModsMenuMod
	{
		private enum Page { None, Mods, Settings }

		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		private MelonPreferences_Entry<int> linesPerPage;
		private MelonPreferences_Entry<float> textScale;
		private MelonPreferences_Entry<float> panelWidth;

		private Page page = Page.None;
		private readonly List<GameObject> pageItems = new List<GameObject>();
		private readonly List<GameObject> hiddenForPage = new List<GameObject>();
		// The panel the pages are drawn on, in the middle of the screen.
		private RectTransform panel;
		private float lineHeight;
		private readonly List<KeyValuePair<RectTransform, Vector2>> rows = new List<KeyValuePair<RectTransform, Vector2>>(); // (row, (height, gap after))

		// Settings page state.
		private string pageTitle = "";
		private List<object> pageCategories = new List<object>();
		private int scroll;
		private string info = "";
		private object editing;          // the entry being typed into or bound
		private bool capturingKey;
		private int captureFrame;
		private string buffer = "";
		private GameObject editingLine;

		private void InitPrefs()
		{
			MelonPreferences_Category c = MelonPreferences.CreateCategory("ModsMenu", "Mods Menu");
			linesPerPage = c.CreateEntry("LinesPerPage", 8, "Settings per page", "How many settings a mod's page shows before NEXT PAGE.");
			textScale = c.CreateEntry("TextScale", 1f, "Text size", "Size of the mods and settings pages' text (1 = normal; try 0.8 to 1.4).");
			panelWidth = c.CreateEntry("PanelWidth", 0.7f, "Panel width", "Width of the mods and settings panel as a share of the screen (0.4 to 0.95).");
		}

		private void ResetPages()
		{
			page = Page.None;
			pageItems.Clear();
			hiddenForPage.Clear();
			rows.Clear();
			panel = null;
			editing = null;
		}

		// --- Opening and closing. ---

		private void OpenPage()
		{
			if (page != Page.None)
			{
				return;
			}
			hiddenForPage.Clear();
			MakePanel();
			foreach (Transform child in column)
			{
				if (child.gameObject.activeSelf)
				{
					hiddenForPage.Add(child.gameObject);
				}
			}
			foreach (GameObject g in hiddenForPage)
			{
				g.SetActive(false);
			}
			ShowMods();
		}

		private void ClosePage()
		{
			ClearItems();
			if (panel != null)
			{
				Object.Destroy(panel.gameObject);
				panel = null;
			}
			foreach (GameObject g in hiddenForPage)
			{
				if (g != null)
				{
					g.SetActive(true);
				}
			}
			hiddenForPage.Clear();
			page = Page.None;
			editing = null;
		}

		private void ClearItems()
		{
			foreach (GameObject g in pageItems)
			{
				if (g != null)
				{
					// Out of the layout now; destroyed at the end of the frame.
					g.SetActive(false);
					Object.Destroy(g);
				}
			}
			pageItems.Clear();
			rows.Clear();
			editingLine = null;
		}

		// --- Keyboard: editing, Esc, scrolling. ---

		private void UpdatePages()
		{
			if (page == Page.None)
			{
				return;
			}
			if (editing != null)
			{
				UpdateEditing();
				return;
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (page == Page.Settings)
				{
					ShowMods();
				}
				else
				{
					ClosePage();
				}
				return;
			}
			if (page == Page.Settings)
			{
				float wheel = Input.mouseScrollDelta.y;
				if (wheel < 0f)
				{
					Scroll(1);
				}
				else if (wheel > 0f)
				{
					Scroll(-1);
				}
			}
		}

		private void UpdateEditing()
		{
			if (capturingKey)
			{
				if (Time.frameCount <= captureFrame)
				{
					return; // the click that started it
				}
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					StopEditing("Cancelled.");
					return;
				}
				foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
				{
					if (key != KeyCode.None && Input.GetKeyDown(key))
					{
						Apply(editing, key);
						return;
					}
				}
				return;
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				StopEditing("Cancelled.");
				return;
			}
			bool changed = false;
			foreach (char ch in Input.inputString)
			{
				if (ch == '\b')
				{
					if (buffer.Length > 0)
					{
						buffer = buffer.Substring(0, buffer.Length - 1);
						changed = true;
					}
				}
				else if (ch == '\n' || ch == '\r')
				{
					Commit();
					return;
				}
				else if (!char.IsControl(ch) && buffer.Length < 200)
				{
					buffer += ch;
					changed = true;
				}
			}
			if (changed && editingLine != null)
			{
				SetLineText(editingLine, EntryName(editing) + ": " + buffer + "_");
			}
		}

		// --- Mods page. ---

		private void ShowMods()
		{
			ClearItems();
			page = Page.Mods;
			editing = null;
			Dictionary<MelonBase, List<object>> owned;
			List<object> unclaimed;
			MatchCategories(out owned, out unclaimed);

			List<MelonBase> mods = new List<MelonBase>();
			foreach (MelonBase melon in MelonMod.RegisteredMelons)
			{
				if (melon != null && melon.Info != null)
				{
					mods.Add(melon);
				}
			}
			mods.Sort((a, b) => string.Compare(a.Info.Name, b.Info.Name, StringComparison.OrdinalIgnoreCase));

			Place(Line(mods.Count + (mods.Count == 1 ? " MOD INSTALLED" : " MODS INSTALLED"), TitleSize, null), 0.25f);
			Place(Line("CLICK A MOD TO CHANGE ITS SETTINGS", InfoSize, null), 0.45f);
			foreach (MelonBase melon in mods)
			{
				string text = melon.Info.Name.ToUpperInvariant() + "  " + melon.Info.Version;
				if (!string.IsNullOrEmpty(melon.Info.Author))
				{
					text += "  -  " + melon.Info.Author;
				}
				List<object> cats;
				UnityAction click = null;
				if (owned.TryGetValue(melon, out cats) && cats.Count > 0)
				{
					int count = 0;
					foreach (object cat in cats)
					{
						count += VisibleEntries(cat).Count;
					}
					text += "   >  " + count + (count == 1 ? " SETTING" : " SETTINGS");
					string title = melon.Info.Name;
					List<object> mine = cats;
					click = delegate { ShowSettings(title, mine); };
				}
				else
				{
					text += "   (NO SETTINGS)";
				}
				Place(Line(text, LineSize, click), 0.1f);
			}
			// Settings no installed mod's name matches (a plugin's, or a mod that
			// names its category differently): still reachable.
			foreach (object cat in unclaimed)
			{
				string title = CategoryName(cat);
				List<object> one = new List<object> { cat };
				Place(Line("SETTINGS: " + title.ToUpperInvariant() + "   >", LineSize, delegate { ShowSettings(title, one); }), 0.1f);
			}
			Gap(0.4f);
			Place(Line("BACK", ButtonSize, ClosePage), 0f);
			Finish();
		}

		// Each visible category goes to the mod whose name, namespace or assembly
		// it's named after.
		private void MatchCategories(out Dictionary<MelonBase, List<object>> owned, out List<object> unclaimed)
		{
			owned = new Dictionary<MelonBase, List<object>>();
			unclaimed = new List<object>();
			Dictionary<string, MelonBase> byKey = new Dictionary<string, MelonBase>();
			foreach (MelonBase melon in MelonMod.RegisteredMelons)
			{
				if (melon == null || melon.Info == null)
				{
					continue;
				}
				Type type = melon.GetType();
				string typeName = type.Name;
				foreach (string key in new[] { melon.Info.Name, type.Namespace, type.Assembly.GetName().Name, typeName, typeName.EndsWith("Mod") ? typeName.Substring(0, typeName.Length - 3) : typeName })
				{
					string k = Key(key);
					if (k.Length > 0 && !byKey.ContainsKey(k))
					{
						byKey[k] = melon;
					}
				}
			}
			foreach (object cat in Categories())
			{
				if (IsHidden(cat) || VisibleEntries(cat).Count == 0)
				{
					continue;
				}
				MelonBase owner;
				if (byKey.TryGetValue(Key(Get<string>(cat, "Identifier")), out owner) || byKey.TryGetValue(Key(Get<string>(cat, "DisplayName")), out owner))
				{
					List<object> list;
					if (!owned.TryGetValue(owner, out list))
					{
						owned[owner] = list = new List<object>();
					}
					list.Add(cat);
				}
				else
				{
					unclaimed.Add(cat);
				}
			}
		}

		// "Junkyard ATV" / "JunkyardATV" / "junkyard_atv" -> "junkyardatv"
		private static string Key(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return "";
			}
			char[] chars = new char[s.Length];
			int n = 0;
			foreach (char ch in s)
			{
				if (char.IsLetterOrDigit(ch))
				{
					chars[n++] = char.ToLowerInvariant(ch);
				}
			}
			return new string(chars, 0, n);
		}

		// --- Settings page. ---

		private void ShowSettings(string title, List<object> categories)
		{
			pageTitle = title;
			pageCategories = categories;
			scroll = 0;
			info = "CLICK A SETTING TO CHANGE IT";
			DrawSettings();
		}

		private void DrawSettings()
		{
			ClearItems();
			page = Page.Settings;
			List<object> entries = new List<object>();
			foreach (object cat in pageCategories)
			{
				entries.AddRange(VisibleEntries(cat));
			}
			int per = Mathf.Clamp(linesPerPage.Value, 3, 30);
			int pages = Mathf.Max(1, (entries.Count + per - 1) / per);
			scroll = Mathf.Clamp(scroll, 0, pages - 1);

			Place(Line(pageTitle.ToUpperInvariant() + (pages > 1 ? "  (" + (scroll + 1) + "/" + pages + ")" : ""), TitleSize, null), 0.25f);
			Place(Line(info, InfoSize, null), 0.45f);
			for (int i = scroll * per; i < entries.Count && i < (scroll + 1) * per; i++)
			{
				object entry = entries[i];
				GameObject line = null;
				line = Line(EntryName(entry) + ": " + ValueText(entry), LineSize, delegate { Edit(entry, line); });
				Place(line, 0.1f);
			}
			Gap(0.3f);
			if (pages > 1)
			{
				if (scroll < pages - 1)
				{
					Place(Line("NEXT PAGE  >", LineSize, delegate { Scroll(1); }), 0.1f);
				}
				if (scroll > 0)
				{
					Place(Line("<  PREVIOUS PAGE", LineSize, delegate { Scroll(-1); }), 0.1f);
				}
			}
			Place(Line("RESET THESE TO DEFAULTS", LineSize, ResetAll), 0.1f);
			Gap(0.4f);
			Place(Line("BACK", ButtonSize, ShowMods), 0f);
			Finish();
		}

		private void Scroll(int by)
		{
			if (page != Page.Settings || editing != null)
			{
				return;
			}
			scroll += by;
			DrawSettings();
		}

		private void ResetAll()
		{
			foreach (object cat in pageCategories)
			{
				foreach (object entry in VisibleEntries(cat))
				{
					Call(entry, "ResetToDefault");
				}
			}
			Save();
			info = "SET BACK TO DEFAULTS";
			DrawSettings();
		}

		// Clicked a setting.
		private void Edit(object entry, GameObject line)
		{
			if (editing != null)
			{
				return;
			}
			Type type = ValueType(entry);
			object value = Get<object>(entry, "BoxedValue");
			info = Describe(entry);
			if (type == typeof(bool))
			{
				Apply(entry, !(value is bool && (bool)value));
				return;
			}
			if (type == typeof(KeyCode))
			{
				editing = entry;
				editingLine = line;
				capturingKey = true;
				captureFrame = Time.frameCount;
				SetLineText(line, EntryName(entry) + ": PRESS A KEY (ESC CANCELS)");
				return;
			}
			if (type != null && type.IsEnum)
			{
				Array options = Enum.GetValues(type);
				int at = Array.IndexOf(options, value);
				Apply(entry, options.GetValue((at + 1) % options.Length));
				return;
			}
			if (type == typeof(string) || IsNumber(type))
			{
				editing = entry;
				editingLine = line;
				capturingKey = false;
				buffer = Format(value);
				SetLineText(line, EntryName(entry) + ": " + buffer + "_");
				return;
			}
			info = "THIS KIND OF SETTING CAN ONLY BE CHANGED IN MELONPREFERENCES.CFG";
			DrawSettings();
		}

		private void Commit()
		{
			object entry = editing;
			Type type = ValueType(entry);
			object value;
			try
			{
				value = type == typeof(string) ? buffer : Convert.ChangeType(buffer.Trim(), type, Inv);
			}
			catch (Exception)
			{
				StopEditing("\"" + buffer + "\" ISN'T A VALID " + (IsNumber(type) ? "NUMBER" : "VALUE"));
				return;
			}
			Apply(entry, value);
		}

		private void Apply(object entry, object value)
		{
			editing = null;
			try
			{
				Set(entry, "BoxedValue", value);
				Save();
				info = Describe(entry);
			}
			catch (Exception e)
			{
				Exception inner = e is TargetInvocationException && e.InnerException != null ? e.InnerException : e;
				info = "COULDN'T SET IT: " + inner.Message.ToUpperInvariant();
			}
			DrawSettings();
		}

		private void StopEditing(string message)
		{
			editing = null;
			info = message.ToUpperInvariant();
			DrawSettings();
		}

		private static void Save()
		{
			try
			{
				MelonPreferences.Save();
			}
			catch (Exception e)
			{
				log.Warning("Couldn't save preferences: " + e.Message);
			}
		}

		// --- Showing values. ---

		private static string EntryName(object entry)
		{
			string name = Get<string>(entry, "DisplayName");
			if (string.IsNullOrEmpty(name))
			{
				name = Get<string>(entry, "Identifier") ?? "?";
			}
			return name.ToUpperInvariant();
		}

		private static string Describe(object entry)
		{
			string d = Get<string>(entry, "Description");
			if (string.IsNullOrEmpty(d))
			{
				return EntryName(entry);
			}
			d = d.Replace('\n', ' ').Trim();
			return (d.Length > 90 ? d.Substring(0, 87) + "..." : d).ToUpperInvariant();
		}

		private static string ValueText(object entry)
		{
			object v = Get<object>(entry, "BoxedValue");
			if (v is bool)
			{
				return (bool)v ? "ON" : "OFF";
			}
			string s = Format(v);
			if (v is string)
			{
				s = s.Length > 34 ? s.Substring(0, 31) + "..." : s;
				return s.Length == 0 ? "(EMPTY)" : s;
			}
			return s.ToUpperInvariant();
		}

		private static string Format(object v)
		{
			if (v == null)
			{
				return "";
			}
			if (v is float)
			{
				return ((float)v).ToString("0.####", Inv);
			}
			if (v is double)
			{
				return ((double)v).ToString("0.####", Inv);
			}
			IFormattable f = v as IFormattable;
			return f != null ? f.ToString(null, Inv) : v.ToString();
		}

		private static bool IsNumber(Type t)
		{
			return t == typeof(int) || t == typeof(float) || t == typeof(double) || t == typeof(long) || t == typeof(short) || t == typeof(byte) || t == typeof(uint) || t == typeof(ulong) || t == typeof(ushort) || t == typeof(sbyte) || t == typeof(decimal);
		}

		// --- The panel and its lines. ---

		// Sizes relative to the menu's own buttons (times the TextScale setting).
		private const float TitleSize = 0.85f;
		private const float InfoSize = 0.5f;
		private const float LineSize = 0.62f;
		private const float ButtonSize = 0.85f;

		// A dark panel in the middle of the screen, on the menu's canvas, so the
		// pages read well over the busy background whatever the column looks like.
		private void MakePanel()
		{
			Transform canvas = column;
			if (canvasType != null)
			{
				Component c = column.GetComponentInParent(canvasType);
				if (c != null)
				{
					PropertyInfo root = canvasType.GetProperty("rootCanvas");
					Component top = root != null ? root.GetValue(c, null) as Component : null;
					canvas = (top ?? c).transform;
				}
			}
			GameObject go = new GameObject("Mods Menu Panel", typeof(RectTransform));
			go.layer = column.gameObject.layer;
			panel = (RectTransform)go.transform;
			panel.SetParent(canvas, false);
			panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(0.5f, 0.5f);
			panel.anchoredPosition = Vector2.zero;
			panel.SetAsLastSibling();
			if (imageType != null)
			{
				// Also catches clicks so nothing behind the panel reacts.
				Component image = go.AddComponent(imageType);
				PropertyInfo color = imageType.GetProperty("color");
				if (color != null)
				{
					color.SetValue(image, new Color(0f, 0f, 0f, 0.82f), null);
				}
			}
			RectTransform t = template.transform as RectTransform;
			lineHeight = t != null && t.rect.height > 1f ? t.rect.height : 60f;
		}

		private float PanelWidth()
		{
			RectTransform canvas = panel.parent as RectTransform;
			float screen = canvas != null && canvas.rect.width > 1f ? canvas.rect.width : 1920f;
			return screen * Mathf.Clamp(panelWidth.Value, 0.4f, 0.95f);
		}

		private float Scale()
		{
			return Mathf.Clamp(textScale.Value, 0.4f, 2.5f);
		}

		// A line: a copy of the menu button (same font, hover and click sound),
		// centered on the panel. Without a click it's just text.
		private GameObject Line(string label, float size, UnityAction onClick)
		{
			size *= Scale();
			GameObject line = MakeButton(label, onClick ?? delegate { }, panel);
			RectTransform rt = line.transform as RectTransform;
			if (rt != null)
			{
				rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
				rt.localScale = Vector3.one;
				rt.localRotation = Quaternion.identity;
			}
			foreach (Component text in Texts(line))
			{
				ScaleFont(text, size);
				CenterText(text);
			}
			rows.Add(new KeyValuePair<RectTransform, Vector2>(rt, new Vector2(lineHeight * size, 0f)));
			return line;
		}

		// Text fills its line and sits in the middle, on one line.
		private static void CenterText(Component text)
		{
			RectTransform rt = text.transform as RectTransform;
			if (rt != null)
			{
				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.one;
				rt.pivot = new Vector2(0.5f, 0.5f);
				rt.offsetMin = rt.offsetMax = Vector2.zero;
				rt.anchoredPosition = Vector2.zero;
			}
			Type type = text.GetType();
			PropertyInfo align = type.GetProperty("alignment", BindingFlags.Instance | BindingFlags.Public);
			if (align != null && align.CanWrite && align.PropertyType.IsEnum)
			{
				// TextMeshPro: Center; UI Text: MiddleCenter.
				foreach (string name in new[] { "Center", "MiddleCenter" })
				{
					if (Enum.IsDefined(align.PropertyType, name))
					{
						align.SetValue(text, Enum.Parse(align.PropertyType, name), null);
						break;
					}
				}
			}
			PropertyInfo wrap = type.GetProperty("enableWordWrapping", BindingFlags.Instance | BindingFlags.Public);
			if (wrap != null && wrap.CanWrite)
			{
				wrap.SetValue(text, false, null);
			}
			PropertyInfo overflow = type.GetProperty("horizontalOverflow", BindingFlags.Instance | BindingFlags.Public);
			if (overflow != null && overflow.CanWrite && overflow.PropertyType.IsEnum && Enum.IsDefined(overflow.PropertyType, "Overflow"))
			{
				overflow.SetValue(text, Enum.Parse(overflow.PropertyType, "Overflow"), null);
			}
		}

		private void SetLineText(GameObject line, string text)
		{
			foreach (Component c in Texts(line))
			{
				SetText(c, text);
			}
		}

		// Add the line just made, with `gap` (in line heights) after it.
		private void Place(GameObject item, float gap)
		{
			pageItems.Add(item);
			int i = rows.Count - 1;
			if (i >= 0)
			{
				rows[i] = new KeyValuePair<RectTransform, Vector2>(rows[i].Key, new Vector2(rows[i].Value.x, lineHeight * gap * Scale()));
			}
		}

		private void Gap(float lines)
		{
			int i = rows.Count - 1;
			if (i >= 0)
			{
				rows[i] = new KeyValuePair<RectTransform, Vector2>(rows[i].Key, new Vector2(rows[i].Value.x, rows[i].Value.y + lineHeight * lines * Scale()));
			}
		}

		// Stack the lines top to bottom, centered, and fit the panel around them.
		private void Finish()
		{
			float width = PanelWidth();
			float pad = lineHeight * 0.5f * Scale();
			float total = 0f;
			for (int i = 0; i < rows.Count; i++)
			{
				total += rows[i].Value.x + (i < rows.Count - 1 ? rows[i].Value.y : 0f);
			}
			panel.sizeDelta = new Vector2(width, total + pad * 2f);
			float y = total / 2f;
			foreach (KeyValuePair<RectTransform, Vector2> row in rows)
			{
				if (row.Key != null)
				{
					row.Key.sizeDelta = new Vector2(width - pad * 2f, row.Value.x);
					row.Key.anchoredPosition = new Vector2(0f, y - row.Value.x / 2f);
				}
				y -= row.Value.x + row.Value.y;
			}
			panel.SetAsLastSibling();
		}

		// --- MelonPreferences by reflection: Categories and Entries are fields in
		// some MelonLoader versions and properties in others. ---

		private static IEnumerable<object> Categories()
		{
			IEnumerable list = Static(typeof(MelonPreferences), "Categories") as IEnumerable;
			List<object> result = new List<object>();
			if (list != null)
			{
				foreach (object c in list)
				{
					result.Add(c);
				}
			}
			return result;
		}

		private static List<object> VisibleEntries(object category)
		{
			List<object> result = new List<object>();
			IEnumerable list = Get<object>(category, "Entries") as IEnumerable;
			if (list == null)
			{
				return result;
			}
			foreach (object e in list)
			{
				if (e != null && !IsHidden(e))
				{
					result.Add(e);
				}
			}
			return result;
		}

		private static string CategoryName(object category)
		{
			string name = Get<string>(category, "DisplayName");
			return string.IsNullOrEmpty(name) ? Get<string>(category, "Identifier") ?? "?" : name;
		}

		private static bool IsHidden(object o)
		{
			object v = Get<object>(o, "IsHidden");
			return v is bool && (bool)v;
		}

		private static Type ValueType(object entry)
		{
			MethodInfo m = entry.GetType().GetMethod("GetReflectedType", Type.EmptyTypes);
			if (m != null)
			{
				return m.Invoke(entry, null) as Type;
			}
			object v = Get<object>(entry, "BoxedValue");
			return v != null ? v.GetType() : null;
		}

		private static T Get<T>(object o, string name)
		{
			if (o == null)
			{
				return default(T);
			}
			const BindingFlags F = BindingFlags.Instance | BindingFlags.Public;
			PropertyInfo p = o.GetType().GetProperty(name, F);
			if (p != null)
			{
				return p.GetValue(o, null) is T ? (T)p.GetValue(o, null) : default(T);
			}
			FieldInfo f = o.GetType().GetField(name, F);
			if (f != null)
			{
				object v = f.GetValue(o);
				return v is T ? (T)v : default(T);
			}
			return default(T);
		}

		private static void Set(object o, string name, object value)
		{
			PropertyInfo p = o.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
			p.SetValue(o, value, null);
		}

		private static void Call(object o, string name)
		{
			MethodInfo m = o.GetType().GetMethod(name, Type.EmptyTypes);
			if (m != null)
			{
				m.Invoke(o, null);
			}
		}

		private static object Static(Type type, string name)
		{
			const BindingFlags F = BindingFlags.Static | BindingFlags.Public;
			PropertyInfo p = type.GetProperty(name, F);
			if (p != null)
			{
				return p.GetValue(null, null);
			}
			FieldInfo f = type.GetField(name, F);
			return f != null ? f.GetValue(null) : null;
		}
	}
}
