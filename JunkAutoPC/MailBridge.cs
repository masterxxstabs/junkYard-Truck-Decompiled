using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace JunkAutoPC
{
	// Puts JunkAuto into the game's own PC screen ("Advanced Mail Systems"):
	//
	//  * a [ JunkAuto.com ] link under [ Exit ] / [ Save Game ], a copy of the
	//    game's own link so it looks the same, which opens the store;
	//  * JunkAuto's emails as extra rows under the game's emails, copies of the
	//    game's own rows; clicking one shows it where the game shows an email.
	//
	// The game's mail itself is never changed: its emails are a fixed list saved
	// by number, so ours live in our own save file and only borrow its rows' look.
	// Unity UI types are reached by reflection (the game ships that assembly).
	internal static class MailBridge
	{
		private const string LinkName = "JunkAuto link";
		private const string RowPrefix = "JunkAuto mail ";

		private static Type buttonType;
		private static Type textType;

		private static MailScript mail;
		private static GameObject link;
		private static readonly List<GameObject> rows = new List<GameObject>();
		private static int drawnVersion = -1;
		private static int drawnReceived = -1;
		private static bool failed;

		public static void Reset()
		{
			mail = null;
			link = null;
			rows.Clear();
			drawnVersion = -1;
			drawnReceived = -1;
			failed = false;
		}

		// Every frame the PC screen is open on the mail.
		public static void Update(Interactor interactor)
		{
			if (failed || interactor == null || interactor.mail == null)
			{
				return;
			}
			try
			{
				if (!FindTypes())
				{
					failed = true;
					JunkAutoMod.Log("Unity UI isn't loaded; JunkAuto can't add itself to the PC screen.");
					return;
				}
				mail = interactor.mail;
				if (link == null)
				{
					MakeLink(interactor.pcCanvas);
				}
				else
				{
					SetText(link, Orders.Unread > 0 ? "[ JunkAuto.com (" + Orders.Unread + ") ]" : "[ JunkAuto.com ]");
				}
				if (drawnVersion != Orders.MailVersion || drawnReceived != mail.receivedEmails.Count || rows.Exists(r => r == null))
				{
					DrawRows();
				}
			}
			catch (Exception e)
			{
				failed = true;
				JunkAutoMod.Log("Couldn't add JunkAuto to the PC screen: " + e);
			}
		}

		private static bool FindTypes()
		{
			if (buttonType != null && textType != null)
			{
				return true;
			}
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				buttonType = buttonType ?? a.GetType("UnityEngine.UI.Button", false);
				textType = textType ?? a.GetType("UnityEngine.UI.Text", false);
			}
			return buttonType != null && textType != null;
		}

		// --- The link. ---

		private static void MakeLink(GameObject canvas)
		{
			Component exit = FindText(canvas, "EXIT");
			if (exit == null)
			{
				failed = true;
				JunkAutoMod.Log("Couldn't find the PC screen's [ Exit ] link to copy.");
				return;
			}
			Transform exitItem = ClickableOf(exit.transform);
			// Under [ Save Game ] (same spacing as Exit -> Save Game), or under Exit.
			Component save = mail.saveLink != null ? mail.saveLink.GetComponentInChildren(textType, true) : null;
			Transform saveItem = save != null ? ClickableOf(save.transform) : null;
			Vector3 step = saveItem != null ? saveItem.position - exitItem.position : Vector3.down * 40f;
			Vector3 at = (saveItem != null ? saveItem.position : exitItem.position) + step;
			link = Clone(exitItem.gameObject, LinkName, "[ JunkAuto.com ]", JunkAutoMod.OpenStore);
			link.transform.position = at;
		}

		// --- Email rows. ---

		private static void DrawRows()
		{
			drawnVersion = Orders.MailVersion;
			drawnReceived = mail.receivedEmails.Count;
			foreach (GameObject r in rows)
			{
				if (r != null)
				{
					Object.Destroy(r);
				}
			}
			rows.Clear();
			GameObject[] slots = mail.emails;
			if (slots == null || slots.Length < 2 || slots[0] == null || slots[1] == null || Orders.Emails.Count == 0)
			{
				return;
			}
			// Row positions follow the game's: the next free slot down the list.
			Vector3 first = slots[0].transform.position;
			Vector3 step = slots[1].transform.position - slots[0].transform.position;
			int start = Mathf.Min(mail.receivedEmails.Count, slots.Length);
			// Newest first, as many as fit in the game's list (at least a few).
			int room = Mathf.Max(4, slots.Length - start);
			int shown = 0;
			for (int i = Orders.Emails.Count - 1; i >= 0 && shown < room; i--, shown++)
			{
				Email m = Orders.Emails[i];
				string text = Pad(m.sender) + (m.read ? "" : "NEW: ") + m.subject;
				Email mine = m;
				GameObject row = Clone(slots[0], RowPrefix + m.id, text, delegate { Show(mine); });
				row.transform.position = first + step * (start + shown);
				// Rows past the game's own slots (a long inbox) still fit on screen
				// in most cases; clicking a game row there is impossible anyway.
				rows.Add(row);
			}
		}

		// The game lines its sender and subject up with spaces ("Johnny Junks" +
		// 19 spaces): the same here.
		private static string Pad(string sender)
		{
			return sender + new string(' ', Mathf.Max(3, 31 - sender.Length));
		}

		private static void Show(Email m)
		{
			if (mail != null && mail.emailbody != null)
			{
				Component body = mail.emailbody.GetComponent(textType);
				if (body != null)
				{
					Set(body, "text", "From: " + m.sender + "\nSubject: " + m.subject + "\n\n" + m.body);
				}
			}
			if (!m.read)
			{
				m.read = true;
				Orders.MailVersion++;
			}
		}

		// --- Copies of the game's UI. ---

		// A copy of a link/row with new text whose click does only `onClick`: the
		// game's own click calls (load that email, exit the PC...) are switched off,
		// any other calls it makes (showing the email panel) are kept for rows.
		private static GameObject Clone(GameObject original, string name, string text, UnityAction onClick)
		{
			GameObject copy = Object.Instantiate(original, original.transform.parent, false);
			copy.name = name;
			copy.SetActive(true);
			foreach (Component t in copy.GetComponentsInChildren(textType, true))
			{
				Set(t, "text", text);
				Set(t, "horizontalOverflow", 1); // Overflow: don't wrap a longer label
			}
			Component button = copy.GetComponent(buttonType) ?? copy.GetComponentInChildren(buttonType, true);
			if (button != null)
			{
				UnityEventBase click = buttonType.GetProperty("onClick").GetValue(button, null) as UnityEventBase;
				bool isLink = name == LinkName;
				for (int i = 0; click != null && i < click.GetPersistentEventCount(); i++)
				{
					string method = click.GetPersistentMethodName(i);
					if (isLink || method == "LoadEmail")
					{
						click.SetPersistentListenerState(i, UnityEventCallState.Off);
					}
				}
				UnityEvent ev = click as UnityEvent;
				if (ev != null)
				{
					ev.AddListener(onClick);
				}
			}
			copy.transform.SetAsLastSibling();
			return copy;
		}

		// The object that's clicked for a label: its Button, or the label itself.
		private static Transform ClickableOf(Transform label)
		{
			Component b = label.GetComponentInParent(buttonType);
			return b != null ? b.transform : label;
		}

		private static Component FindText(GameObject root, string contains)
		{
			foreach (Component t in root.GetComponentsInChildren(textType, true))
			{
				string s = Get(t, "text") as string;
				if (s != null && s.ToUpperInvariant().Contains(contains) && !t.name.StartsWith("JunkAuto"))
				{
					return t;
				}
			}
			return null;
		}

		private static void SetText(GameObject go, string text)
		{
			foreach (Component t in go.GetComponentsInChildren(textType, true))
			{
				if ((Get(t, "text") as string) != text)
				{
					Set(t, "text", text);
				}
			}
		}

		private static object Get(object o, string prop)
		{
			PropertyInfo p = o.GetType().GetProperty(prop, BindingFlags.Instance | BindingFlags.Public);
			return p != null ? p.GetValue(o, null) : null;
		}

		private static void Set(object o, string prop, object value)
		{
			PropertyInfo p = o.GetType().GetProperty(prop, BindingFlags.Instance | BindingFlags.Public);
			if (p == null || !p.CanWrite)
			{
				return;
			}
			if (p.PropertyType.IsEnum && value is int)
			{
				value = Enum.ToObject(p.PropertyType, (int)value);
			}
			p.SetValue(o, value, null);
		}
	}
}
