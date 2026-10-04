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

		private Page page = Page.None;
		private readonly List<GameObject> pageItems = new List<GameObject>();
		private readonly List<GameObject> hiddenForPage = new List<GameObject>();
		private Vector2 firstPos;
		private Vector2 slot;

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
		}

		private void ResetPages()
		{
			page = Page.None;
			pageItems.Clear();
			hiddenForPage.Clear();
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
			RectTransform first = template.transform as RectTransform;
			firstPos = first != null ? first.anchoredPosition : Vector2.zero;
			slot = Slot();
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

			Vector2 pos = firstPos;
			Place(Line(mods.Count + (mods.Count == 1 ? " MOD INSTALLED" : " MODS INSTALLED"), 0.65f, null), ref pos, slot * 0.75f);
			Place(Line("CLICK A MOD TO CHANGE ITS SETTINGS", 0.38f, null), ref pos, slot * 0.5f);
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
				Place(Line(text, 0.5f, click), ref pos, slot * 0.55f);
			}
			// Settings no installed mod's name matches (a plugin's, or a mod that
			// names its category differently): still reachable.
			foreach (object cat in unclaimed)
			{
				string title = CategoryName(cat);
				List<object> one = new List<object> { cat };
				Place(Line("SETTINGS: " + title.ToUpperInvariant() + "   >", 0.5f, delegate { ShowSettings(title, one); }), ref pos, slot * 0.55f);
			}
			pos += slot * 0.35f;
			Place(Button("BACK", ClosePage), ref pos, slot);
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

			Vector2 pos = firstPos;
			Place(Line(pageTitle.ToUpperInvariant() + (pages > 1 ? "  (" + (scroll + 1) + "/" + pages + ")" : ""), 0.65f, null), ref pos, slot * 0.7f);
			Place(Line(info, 0.38f, null), ref pos, slot * 0.5f);
			for (int i = scroll * per; i < entries.Count && i < (scroll + 1) * per; i++)
			{
				object entry = entries[i];
				GameObject line = null;
				line = Line(EntryName(entry) + ": " + ValueText(entry), 0.5f, delegate { Edit(entry, line); });
				Place(line, ref pos, slot * 0.55f);
			}
			pos += slot * 0.2f;
			if (pages > 1)
			{
				if (scroll < pages - 1)
				{
					Place(Line("NEXT PAGE  >", 0.5f, delegate { Scroll(1); }), ref pos, slot * 0.55f);
				}
				if (scroll > 0)
				{
					Place(Line("<  PREVIOUS PAGE", 0.5f, delegate { Scroll(-1); }), ref pos, slot * 0.55f);
				}
			}
			Place(Line("RESET THESE TO DEFAULTS", 0.5f, ResetAll), ref pos, slot * 0.55f);
			pos += slot * 0.2f;
			Place(Button("BACK", ShowMods), ref pos, slot);
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

		// --- Lines. ---

		// A line of the page: a copy of the menu button in smaller type. Without a
		// click it's just text (its hover look stays, it does nothing).
		private GameObject Line(string label, float size, UnityAction onClick)
		{
			GameObject line = MakeButton(label, onClick ?? delegate { });
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

		private GameObject Button(string label, UnityAction onClick)
		{
			return MakeButton(label, onClick);
		}

		private void SetLineText(GameObject line, string text)
		{
			foreach (Component c in Texts(line))
			{
				SetText(c, text);
			}
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
