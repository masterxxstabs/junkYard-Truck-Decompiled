using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkAutoPC
{
	internal class OrderLine
	{
		public string key;
		public string name;
		public float price;
		public int qty;
	}

	internal class Order
	{
		public int id;
		public float secondsLeft;     // of play until it arrives
		public bool delivered;
		public Vector3 spot;          // where it's dropped off
		public float shipping;
		public List<OrderLine> lines = new List<OrderLine>();

		public float Total
		{
			get
			{
				float t = shipping;
				foreach (OrderLine l in lines)
				{
					t += l.price * l.qty;
				}
				return t;
			}
		}

		public int Count
		{
			get
			{
				int n = 0;
				foreach (OrderLine l in lines)
				{
					n += l.qty;
				}
				return n;
			}
		}
	}

	internal class Email
	{
		public int id;
		public bool read;
		public string sender;
		public string subject;
		public string body;
	}

	// Orders and JunkAuto emails, saved with the game's save slots in
	// UserData/JunkAutoPC/slotN.txt (auto.txt), like our other mods.
	internal static class Orders
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
		public const int AutoSlot = 4;
		private const int KeepDelivered = 30;
		private const int KeepEmails = 40;

		public static readonly List<Order> All = new List<Order>();
		public static readonly List<Email> Emails = new List<Email>();
		public static int NextOrderId = 1001;
		private static int nextEmailId = 1;

		// Bumped whenever the inbox changes, so the game's mail list is redrawn.
		public static int MailVersion;

		public static int Unread
		{
			get
			{
				int n = 0;
				foreach (Email e in Emails)
				{
					if (!e.read)
					{
						n++;
					}
				}
				return n;
			}
		}

		public static void Clear()
		{
			All.Clear();
			Emails.Clear();
			NextOrderId = 1001;
			nextEmailId = 1;
			MailVersion++;
		}

		// --- Placing and delivering. ---

		public static Order Place(List<OrderLine> cart, float shipping, Vector3 spot, float minutes)
		{
			Order o = new Order();
			o.id = NextOrderId++;
			o.secondsLeft = Mathf.Max(0f, minutes) * 60f;
			o.spot = spot;
			o.shipping = shipping;
			foreach (OrderLine l in cart)
			{
				o.lines.Add(new OrderLine { key = l.key, name = l.name, price = l.price, qty = l.qty });
			}
			All.Add(o);
			StringBuilder body = new StringBuilder();
			body.Append("Thank you for your order!\n\nOrder #").Append(o.id).Append("\n");
			AppendLines(body, o);
			body.Append("\nEstimated delivery: about ").Append(Mathf.Max(1, Mathf.RoundToInt(minutes))).Append(minutes < 1.5f ? " minute" : " minutes").Append(".\nWe'll email you when it arrives.\n\nJunkAuto.com\nAll the parts your ride will ever need");
			Mail("Order #" + o.id + " confirmed", body.ToString());
			return o;
		}

		// Called every frame in the level.
		public static void Tick(float dt)
		{
			foreach (Order o in All)
			{
				if (o.delivered)
				{
					continue;
				}
				o.secondsLeft -= dt;
				if (o.secondsLeft <= 0f)
				{
					Deliver(o);
				}
			}
			Trim();
		}

		private static void Deliver(Order o)
		{
			o.delivered = true;
			o.secondsLeft = 0f;
			Vector3 spot = JunkAutoMod.DeliverToPlayer ? JunkAutoMod.InFrontOfPlayer() : o.spot;
			int n = 0;
			List<string> missing = new List<string>();
			foreach (OrderLine l in o.lines)
			{
				Product p = Catalog.Find(l.key);
				for (int i = 0; i < l.qty; i++)
				{
					if (p == null || p.prefab == null)
					{
						missing.Add(l.name);
						break;
					}
					Spawn(p, l.price, spot + Offset(n++));
				}
			}
			StringBuilder body = new StringBuilder();
			body.Append("Your order #").Append(o.id).Append(" has been delivered.\n\n");
			AppendLines(body, o);
			body.Append(JunkAutoMod.DeliverToPlayer ? "\nThe driver left it right in front of you." : "\nThe driver left it where you placed the order, by the computer.");
			if (missing.Count > 0)
			{
				body.Append("\n\nSorry, these couldn't be found in the warehouse: ").Append(string.Join(", ", missing.ToArray())).Append(".");
				JunkAutoMod.Log("Order #" + o.id + ": no longer in the catalog: " + string.Join(", ", missing.ToArray()));
			}
			body.Append("\n\nThanks for shopping at JunkAuto.com!");
			Mail("Order #" + o.id + " delivered", body.ToString());
			JunkAutoMod.Log("Delivered order #" + o.id + " (" + n + " item(s)).");
		}

		// The same as the Parts Store's own delivery: a brand new part, full price.
		private static void Spawn(Product p, float price, Vector3 at)
		{
			GameObject item = Object.Instantiate(p.prefab, at, Quaternion.identity);
			item.name = p.prefab.name;
			item.SetActive(true);
			PickUp pick = item.GetComponent<PickUp>();
			if (pick != null)
			{
				pick.price = price;
				pick.thisDurability = 100f;
				pick.pickable = true;
			}
		}

		// Spread a big order out on a small grid instead of one pile.
		private static Vector3 Offset(int i)
		{
			int ring = i % 9;
			int layer = i / 9;
			float x = (ring % 3 - 1) * 0.45f;
			float z = (ring / 3 - 1) * 0.45f;
			return new Vector3(x, 0.25f + layer * 0.4f, z);
		}

		private static void AppendLines(StringBuilder sb, Order o)
		{
			foreach (OrderLine l in o.lines)
			{
				sb.Append("  ").Append(l.qty).Append(" x ").Append(l.name).Append("   $").Append((l.price * l.qty).ToString("0.00", Inv)).Append("\n");
			}
			if (o.shipping > 0f)
			{
				sb.Append("  Shipping   $").Append(o.shipping.ToString("0.00", Inv)).Append("\n");
			}
			sb.Append("  Total   $").Append(o.Total.ToString("0.00", Inv)).Append("\n");
		}

		public static void Mail(string subject, string body)
		{
			Emails.Add(new Email { id = nextEmailId++, sender = "JunkAuto.com", subject = subject, body = body });
			MailVersion++;
			JunkAutoMod.Toast("New email from JunkAuto.com: " + subject);
		}

		private static void Trim()
		{
			int delivered = 0;
			for (int i = All.Count - 1; i >= 0; i--)
			{
				if (All[i].delivered && ++delivered > KeepDelivered)
				{
					All.RemoveAt(i);
				}
			}
			if (Emails.Count > KeepEmails)
			{
				Emails.RemoveRange(0, Emails.Count - KeepEmails);
				MailVersion++;
			}
		}

		// --- Saving. ---

		private static string File(int slot)
		{
			string dir = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"), "JunkAutoPC");
			return Path.Combine(dir, slot == AutoSlot ? "auto.txt" : "slot" + slot + ".txt");
		}

		public static void Save(int slot)
		{
			try
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("NEXT;").Append(NextOrderId).Append(';').Append(nextEmailId).Append('\n');
				foreach (Order o in All)
				{
					List<string> lines = new List<string>();
					foreach (OrderLine l in o.lines)
					{
						lines.Add(E(l.key) + "~" + E(l.name) + "~" + F(l.price) + "~" + l.qty);
					}
					sb.Append(string.Join(";", new[]
					{
						"ORDER", o.id.ToString(Inv), F(o.secondsLeft), o.delivered ? "1" : "0", F(o.spot.x), F(o.spot.y), F(o.spot.z), F(o.shipping), string.Join("^", lines.ToArray())
					})).Append('\n');
				}
				foreach (Email m in Emails)
				{
					sb.Append(string.Join(";", new[] { "MAIL", m.id.ToString(Inv), m.read ? "1" : "0", E(m.sender), E(m.subject), E(m.body) })).Append('\n');
				}
				string path = File(slot);
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				string temp = path + ".tmp";
				System.IO.File.WriteAllText(temp, sb.ToString());
				if (System.IO.File.Exists(path))
				{
					System.IO.File.Delete(path);
				}
				System.IO.File.Move(temp, path);
			}
			catch (Exception e)
			{
				JunkAutoMod.Log("Saving JunkAuto orders failed: " + e.Message);
			}
		}

		public static void Load(int slot)
		{
			Clear();
			if (slot <= 0 || !System.IO.File.Exists(File(slot)))
			{
				return;
			}
			foreach (string line in System.IO.File.ReadAllLines(File(slot)))
			{
				string[] f = line.Split(';');
				try
				{
					if (f[0] == "NEXT" && f.Length >= 3)
					{
						NextOrderId = int.Parse(f[1], Inv);
						nextEmailId = int.Parse(f[2], Inv);
					}
					else if (f[0] == "ORDER" && f.Length >= 9)
					{
						Order o = new Order();
						o.id = int.Parse(f[1], Inv);
						o.secondsLeft = P(f[2]);
						o.delivered = f[3] == "1";
						o.spot = new Vector3(P(f[4]), P(f[5]), P(f[6]));
						o.shipping = P(f[7]);
						foreach (string item in f[8].Split(new[] { '^' }, StringSplitOptions.RemoveEmptyEntries))
						{
							string[] g = item.Split('~');
							o.lines.Add(new OrderLine { key = U(g[0]), name = U(g[1]), price = P(g[2]), qty = int.Parse(g[3], Inv) });
						}
						All.Add(o);
					}
					else if (f[0] == "MAIL" && f.Length >= 6)
					{
						Emails.Add(new Email { id = int.Parse(f[1], Inv), read = f[2] == "1", sender = U(f[3]), subject = U(f[4]), body = U(f[5]) });
					}
				}
				catch (Exception e)
				{
					JunkAutoMod.Log("Skipping a bad JunkAuto save line: " + e.Message);
				}
			}
			MailVersion++;
			JunkAutoMod.Log("Restored " + All.Count + " order(s) and " + Emails.Count + " email(s).");
		}

		public static void Delete(int slot)
		{
			try
			{
				if (System.IO.File.Exists(File(slot)))
				{
					System.IO.File.Delete(File(slot));
				}
			}
			catch (Exception e)
			{
				JunkAutoMod.Log("Couldn't delete JunkAuto data: " + e.Message);
			}
		}

		private static string E(string s)
		{
			return Uri.EscapeDataString(s ?? "");
		}

		private static string U(string s)
		{
			return Uri.UnescapeDataString(s);
		}

		private static string F(float v)
		{
			return v.ToString("R", Inv);
		}

		private static float P(string s)
		{
			return float.Parse(s, NumberStyles.Float, Inv);
		}
	}
}
