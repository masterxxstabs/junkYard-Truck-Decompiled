using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace JunkAutoPC
{
	// JunkAuto.com, drawn over the PC screen, in the spirit of RockAuto: a plain
	// white catalog, the logo up top, a blue navigation bar, the vehicle tree on
	// the left (vehicle, then part category) and the parts with prices and
	// "Add to Cart" on the right; a cart with shipping and checkout, and order
	// status. Laid out on a 1280x720 page scaled to the screen.
	internal static class Site
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
		private const float W = 1280f, H = 720f;

		private enum View { Catalog, Cart, Orders }

		private static View view = View.Catalog;
		private static readonly HashSet<string> open = new HashSet<string>();
		private static string vehicle;
		private static string category;
		private static Vector2 treeScroll, listScroll, cartScroll, orderScroll;
		private static readonly List<OrderLine> cart = new List<OrderLine>();
		private static string message = "";
		private static bool messageBad;

		// --- Look. ---

		private static bool styled;
		private static GUIStyle page, tabOn, tabOff, cashStyle, logoJunk, logoAuto, tagline, navBar, navText, navLink;
		private static GUIStyle treeVehicle, treeCategory, treeSelected, small, body, bold, price, header, rowA, rowB, partName, partNo;
		private static GUIStyle addButton, smallButton, orderButton, msgGood, msgBad, title, box;

		private static readonly Color Blue = Hex(0x1f4e8c);
		private static readonly Color LinkBlue = Hex(0x0b3d91);
		private static readonly Color Red = Hex(0xc8102e);

		private static void Style()
		{
			if (styled)
			{
				return;
			}
			styled = true;
			Font font = GUI.skin.label.font;
			page = Fill(Color.white);
			tabOn = Text(Hex(0x33ff66), 14, FontStyle.Bold, TextAnchor.MiddleCenter);
			tabOn.normal.background = Tex(Hex(0x0f2a14));
			tabOff = Text(Hex(0x1fa64a), 14, FontStyle.Normal, TextAnchor.MiddleCenter);
			tabOff.hover.textColor = Hex(0x33ff66);
			cashStyle = Text(Hex(0x33ff66), 14, FontStyle.Normal, TextAnchor.MiddleRight);
			logoJunk = Text(Red, 38, FontStyle.Bold, TextAnchor.MiddleLeft);
			logoAuto = Text(Hex(0x1b1b1b), 38, FontStyle.Bold, TextAnchor.MiddleLeft);
			tagline = Text(Hex(0x666666), 11, FontStyle.Bold, TextAnchor.MiddleLeft);
			navBar = Fill(Blue);
			navText = Text(Color.white, 13, FontStyle.Bold, TextAnchor.MiddleLeft);
			navLink = Text(LinkBlue, 14, FontStyle.Bold, TextAnchor.MiddleCenter);
			navLink.hover.textColor = Red;
			treeVehicle = Text(LinkBlue, 13, FontStyle.Bold, TextAnchor.MiddleLeft);
			treeVehicle.hover.textColor = Red;
			treeCategory = Text(LinkBlue, 12, FontStyle.Normal, TextAnchor.MiddleLeft);
			treeCategory.hover.textColor = Red;
			treeSelected = Text(Color.black, 12, FontStyle.Bold, TextAnchor.MiddleLeft);
			treeSelected.normal.background = Tex(Hex(0xfff3c4));
			small = Text(Hex(0x666666), 11, FontStyle.Normal, TextAnchor.MiddleLeft);
			body = Text(Hex(0x222222), 13, FontStyle.Normal, TextAnchor.MiddleLeft);
			body.wordWrap = true;
			bold = Text(Hex(0x222222), 13, FontStyle.Bold, TextAnchor.MiddleLeft);
			price = Text(Color.black, 14, FontStyle.Bold, TextAnchor.MiddleRight);
			header = Text(Hex(0x333333), 12, FontStyle.Bold, TextAnchor.MiddleLeft);
			header.normal.background = Tex(Hex(0xe4e4e4));
			header.padding = new RectOffset(8, 8, 0, 0);
			rowA = Fill(Color.white);
			rowB = Fill(Hex(0xf5f7fa));
			partName = Text(LinkBlue, 13, FontStyle.Bold, TextAnchor.UpperLeft);
			partNo = Text(Hex(0x777777), 10, FontStyle.Normal, TextAnchor.UpperLeft);
			addButton = Button(Hex(0x3a6ea5), Hex(0x2c5a8c), Color.white, 12);
			smallButton = Button(Hex(0xdddddd), Hex(0xc8c8c8), Hex(0x222222), 12);
			orderButton = Button(Hex(0xf28c28), Hex(0xd9771a), Color.white, 16);
			msgGood = Text(Hex(0x1e7a2e), 13, FontStyle.Bold, TextAnchor.MiddleLeft);
			msgBad = Text(Red, 13, FontStyle.Bold, TextAnchor.MiddleLeft);
			title = Text(Hex(0x1b1b1b), 18, FontStyle.Bold, TextAnchor.MiddleLeft);
			box = Fill(Hex(0xf0f0f0));
			if (font != null)
			{
				page.font = font;
			}
		}

		// --- Drawing. ---

		public static void Draw()
		{
			Style();
			float scale = Mathf.Min(Screen.width / W, Screen.height / H);
			Matrix4x4 old = GUI.matrix;
			GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - W * scale) / 2f, (Screen.height - H * scale) / 2f, 0f), Quaternion.identity, new Vector3(scale, scale, 1f));
			// Behind the page (letterbox) and the page itself.
			GUI.Box(new Rect(-2000f, -2000f, 6000f, 6000f), GUIContent.none, Fill(Color.black));
			GUI.Box(new Rect(0f, 0f, W, H), GUIContent.none, page);

			DrawTabs();
			DrawHeader();
			switch (view)
			{
			case View.Cart:
				DrawCart();
				break;
			case View.Orders:
				DrawOrders();
				break;
			default:
				DrawTree();
				DrawParts();
				break;
			}
			GUI.matrix = old;
		}

		// The PC's own tab strip, in its green terminal look.
		private static void DrawTabs()
		{
			GUI.Box(new Rect(0f, 0f, W, 32f), GUIContent.none, Fill(Color.black));
			int unread = Orders.Unread;
			if (GUI.Button(new Rect(10f, 2f, 150f, 28f), unread > 0 ? "[ Mail (" + unread + ") ]" : "[ Mail ]", tabOff))
			{
				JunkAutoMod.ShowMail();
			}
			GUI.Button(new Rect(165f, 2f, 190f, 28f), "[ JunkAuto.com ]", tabOn);
			GUI.Label(new Rect(W - 330f, 2f, 320f, 28f), "Cash: $" + Money(JunkAutoMod.Cash()), cashStyle);
		}

		private static void DrawHeader()
		{
			GUI.Label(new Rect(22f, 38f, 130f, 50f), "JUNK", logoJunk);
			GUI.Label(new Rect(130f, 38f, 130f, 50f), "AUTO", logoAuto);
			GUI.Label(new Rect(24f, 82f, 400f, 16f), "ALL THE PARTS YOUR RIDE WILL EVER NEED", tagline);
			int count = 0;
			float sub = 0f;
			foreach (OrderLine l in cart)
			{
				count += l.qty;
				sub += l.price * l.qty;
			}
			if (GUI.Button(new Rect(W - 470f, 46f, 120f, 30f), "CATALOG", navLink))
			{
				view = View.Catalog;
				message = "";
			}
			if (GUI.Button(new Rect(W - 350f, 46f, 180f, 30f), "CART (" + count + ")  $" + Money(sub), navLink))
			{
				view = View.Cart;
			}
			if (GUI.Button(new Rect(W - 170f, 46f, 160f, 30f), "ORDER STATUS", navLink))
			{
				view = View.Orders;
				message = "";
			}
			GUI.Box(new Rect(0f, 104f, W, 28f), GUIContent.none, navBar);
			string crumb = view == View.Cart ? "Shopping Cart" : view == View.Orders ? "Order Status" : "Parts Catalog" + (vehicle != null ? "  »  " + vehicle : "") + (category != null ? "  »  " + category : "");
			GUI.Label(new Rect(16f, 104f, W - 32f, 28f), crumb, navText);
		}

		// Vehicle, then category, RockAuto style: [+] to open, [-] to close.
		private static void DrawTree()
		{
			List<Product> all = Catalog.All;
			SortedDictionary<string, SortedDictionary<string, int>> tree = new SortedDictionary<string, SortedDictionary<string, int>>(new VehicleOrder());
			foreach (Product p in all)
			{
				SortedDictionary<string, int> cats;
				if (!tree.TryGetValue(p.vehicle, out cats))
				{
					tree[p.vehicle] = cats = new SortedDictionary<string, int>(StringComparer.Ordinal);
				}
				int n;
				cats.TryGetValue(p.category, out n);
				cats[p.category] = n + 1;
			}
			Rect area = new Rect(10f, 140f, 340f, H - 150f);
			GUI.Box(area, GUIContent.none, box);
			float rows = 0f;
			foreach (KeyValuePair<string, SortedDictionary<string, int>> v in tree)
			{
				rows += 1f + (open.Contains(v.Key) ? v.Value.Count : 0);
			}
			treeScroll = GUI.BeginScrollView(new Rect(area.x + 4f, area.y + 4f, area.width - 8f, area.height - 8f), treeScroll, new Rect(0f, 0f, area.width - 26f, Mathf.Max(area.height - 8f, rows * 24f + 30f)));
			float y = 0f;
			if (tree.Count == 0)
			{
				GUI.Label(new Rect(6f, 4f, 310f, 60f), "The catalog is still loading. Drive past the junkyard once so the warehouse fills up.", body);
			}
			foreach (KeyValuePair<string, SortedDictionary<string, int>> v in tree)
			{
				bool isOpen = open.Contains(v.Key);
				if (GUI.Button(new Rect(4f, y, 310f, 24f), (isOpen ? "[-]  " : "[+]  ") + v.Key, treeVehicle))
				{
					if (isOpen)
					{
						open.Remove(v.Key);
					}
					else
					{
						open.Add(v.Key);
					}
				}
				y += 24f;
				if (!isOpen)
				{
					continue;
				}
				foreach (KeyValuePair<string, int> c in v.Value)
				{
					bool selected = vehicle == v.Key && category == c.Key;
					if (GUI.Button(new Rect(26f, y, 288f, 24f), "  " + c.Key + "  (" + c.Value + ")", selected ? treeSelected : treeCategory))
					{
						vehicle = v.Key;
						category = c.Key;
						listScroll = Vector2.zero;
						message = "";
					}
					y += 24f;
				}
			}
			GUI.EndScrollView();
		}

		private static void DrawParts()
		{
			Rect area = new Rect(360f, 140f, W - 370f, H - 150f);
			if (vehicle == null || category == null)
			{
				GUI.Label(new Rect(area.x + 10f, area.y + 10f, area.width - 20f, 30f), "Welcome to JunkAuto.com", title);
				GUI.Label(new Rect(area.x + 10f, area.y + 46f, area.width - 20f, 120f),
					"Pick your vehicle in the catalog on the left, open a category and add parts to your cart.\n\n" +
					"Every part ships brand new from our warehouse. Orders arrive in about " + Mathf.Max(1, Mathf.RoundToInt(JunkAutoMod.DeliveryMinutes)) + " minute(s), and we email you when they're delivered.", body);
				DrawMessage(new Rect(area.x + 10f, area.y + 180f, area.width - 20f, 24f));
				return;
			}
			List<Product> parts = Catalog.All.FindAll(p => p.vehicle == vehicle && p.category == category);
			GUI.Label(new Rect(area.x, area.y, area.width, 26f), "PART", header);
			GUI.Label(new Rect(area.x + 470f, area.y, 120f, 26f), "PART #", header);
			GUI.Label(new Rect(area.x + 590f, area.y, 110f, 26f), "PRICE", header);
			DrawMessage(new Rect(area.x, area.y + area.height - 24f, area.width, 24f));
			Rect view = new Rect(area.x, area.y + 28f, area.width, area.height - 56f);
			listScroll = GUI.BeginScrollView(view, listScroll, new Rect(0f, 0f, view.width - 20f, Mathf.Max(view.height, parts.Count * 44f)));
			for (int i = 0; i < parts.Count; i++)
			{
				Product p = parts[i];
				float y = i * 44f;
				GUI.Box(new Rect(0f, y, view.width - 20f, 44f), GUIContent.none, i % 2 == 0 ? rowA : rowB);
				GUI.Label(new Rect(8f, y + 6f, 450f, 18f), p.name, partName);
				GUI.Label(new Rect(8f, y + 24f, 450f, 16f), "Ships from the JunkAuto warehouse  ·  new", partNo);
				GUI.Label(new Rect(470f, y + 12f, 120f, 20f), p.partNo, small);
				GUI.Label(new Rect(590f, y + 10f, 100f, 24f), "$" + Money(p.price), price);
				int inCart = InCart(p.key);
				if (GUI.Button(new Rect(715f, y + 9f, 110f, 26f), "Add to Cart", addButton))
				{
					AddToCart(p);
				}
				if (inCart > 0)
				{
					GUI.Label(new Rect(832f, y + 12f, 60f, 20f), inCart + " in cart", small);
				}
			}
			GUI.EndScrollView();
		}

		private static void DrawCart()
		{
			Rect area = new Rect(20f, 140f, W - 40f, H - 150f);
			GUI.Label(new Rect(area.x, area.y, 400f, 30f), "Shopping Cart", title);
			if (cart.Count == 0)
			{
				GUI.Label(new Rect(area.x, area.y + 40f, 600f, 24f), "Your cart is empty. Browse the catalog to add parts.", body);
				DrawMessage(new Rect(area.x, area.y + 76f, area.width, 24f));
				return;
			}
			float top = area.y + 38f;
			GUI.Label(new Rect(area.x, top, 800f, 26f), "PART", header);
			GUI.Label(new Rect(area.x + 560f, top, 100f, 26f), "PRICE", header);
			GUI.Label(new Rect(area.x + 670f, top, 170f, 26f), "QTY", header);
			GUI.Label(new Rect(area.x + 840f, top, 120f, 26f), "TOTAL", header);
			Rect list = new Rect(area.x, top + 28f, 980f, 330f);
			cartScroll = GUI.BeginScrollView(list, cartScroll, new Rect(0f, 0f, list.width - 20f, Mathf.Max(list.height, cart.Count * 38f)));
			for (int i = 0; i < cart.Count; i++)
			{
				OrderLine l = cart[i];
				float y = i * 38f;
				GUI.Box(new Rect(0f, y, list.width - 20f, 38f), GUIContent.none, i % 2 == 0 ? rowA : rowB);
				GUI.Label(new Rect(8f, y + 10f, 540f, 20f), l.name, bold);
				GUI.Label(new Rect(560f, y + 8f, 90f, 22f), "$" + Money(l.price), price);
				if (GUI.Button(new Rect(675f, y + 7f, 26f, 24f), "-", smallButton))
				{
					l.qty--;
				}
				GUI.Label(new Rect(706f, y + 8f, 34f, 22f), l.qty.ToString(Inv), bold);
				if (GUI.Button(new Rect(740f, y + 7f, 26f, 24f), "+", smallButton))
				{
					l.qty++;
				}
				GUI.Label(new Rect(800f, y + 8f, 120f, 22f), "$" + Money(l.price * l.qty), price);
				if (GUI.Button(new Rect(870f, y + 7f, 80f, 24f), "Remove", smallButton))
				{
					l.qty = 0;
				}
			}
			GUI.EndScrollView();
			cart.RemoveAll(l => l.qty <= 0);

			float sub = Subtotal();
			float ship = Shipping(sub);
			Rect sum = new Rect(area.x + 1000f, top, area.width - 1000f, 250f);
			GUI.Box(sum, GUIContent.none, box);
			GUI.Label(new Rect(sum.x + 12f, sum.y + 10f, 200f, 22f), "Order Summary", bold);
			Line(sum, 44f, "Subtotal", "$" + Money(sub));
			Line(sum, 70f, "Shipping", ship > 0f ? "$" + Money(ship) : "FREE");
			Line(sum, 104f, "Total", "$" + Money(sub + ship));
			GUI.Label(new Rect(sum.x + 12f, sum.y + 134f, sum.width - 24f, 34f), JunkAutoMod.FreeShippingOver > 0f ? "Free shipping on orders over $" + Money(JunkAutoMod.FreeShippingOver) + "." : "", small);
			if (GUI.Button(new Rect(sum.x + 12f, sum.y + 176f, sum.width - 24f, 40f), "PLACE ORDER", orderButton))
			{
				Checkout(sub, ship);
			}
			DrawMessage(new Rect(area.x, list.yMax + 12f, area.width, 24f));
		}

		private static void Line(Rect sum, float y, string label, string value)
		{
			GUI.Label(new Rect(sum.x + 12f, sum.y + y, 120f, 22f), label, body);
			GUI.Label(new Rect(sum.x + 120f, sum.y + y, sum.width - 132f, 22f), value, price);
		}

		private static void DrawOrders()
		{
			Rect area = new Rect(20f, 140f, W - 40f, H - 150f);
			GUI.Label(new Rect(area.x, area.y, 400f, 30f), "Order Status", title);
			List<Order> orders = new List<Order>(Orders.All);
			orders.Reverse();
			if (orders.Count == 0)
			{
				GUI.Label(new Rect(area.x, area.y + 40f, 600f, 24f), "No orders yet.", body);
				DrawMessage(new Rect(area.x, area.y + 76f, area.width, 24f));
				return;
			}
			float top = area.y + 38f;
			GUI.Label(new Rect(area.x, top, area.width, 26f), "ORDER", header);
			GUI.Label(new Rect(area.x + 140f, top, 600f, 26f), "ITEMS", header);
			GUI.Label(new Rect(area.x + 760f, top, 120f, 26f), "TOTAL", header);
			GUI.Label(new Rect(area.x + 890f, top, 300f, 26f), "STATUS", header);
			DrawMessage(new Rect(area.x, area.y + area.height - 24f, area.width, 24f));
			Rect list = new Rect(area.x, top + 28f, area.width, area.height - 100f);
			orderScroll = GUI.BeginScrollView(list, orderScroll, new Rect(0f, 0f, list.width - 20f, Mathf.Max(list.height, orders.Count * 40f)));
			for (int i = 0; i < orders.Count; i++)
			{
				Order o = orders[i];
				float y = i * 40f;
				GUI.Box(new Rect(0f, y, list.width - 20f, 40f), GUIContent.none, i % 2 == 0 ? rowA : rowB);
				GUI.Label(new Rect(8f, y + 10f, 130f, 20f), "#" + o.id, bold);
				List<string> names = new List<string>();
				foreach (OrderLine l in o.lines)
				{
					names.Add(l.qty > 1 ? l.qty + " x " + l.name : l.name);
				}
				string items = string.Join(", ", names.ToArray());
				GUI.Label(new Rect(140f, y + 10f, 600f, 20f), items.Length > 80 ? items.Substring(0, 77) + "..." : items, body);
				GUI.Label(new Rect(740f, y + 10f, 120f, 20f), "$" + Money(o.Total), price);
				string status = o.delivered ? "Delivered" : "Shipped  -  arrives in " + Clock(o.secondsLeft);
				GUI.Label(new Rect(890f, y + 10f, 300f, 20f), status, o.delivered ? msgGood : bold);
			}
			GUI.EndScrollView();
		}

		private static void DrawMessage(Rect r)
		{
			if (message.Length > 0)
			{
				GUI.Label(r, message, messageBad ? msgBad : msgGood);
			}
		}

		// --- Cart. ---

		private static int InCart(string key)
		{
			foreach (OrderLine l in cart)
			{
				if (l.key == key)
				{
					return l.qty;
				}
			}
			return 0;
		}

		private static void AddToCart(Product p)
		{
			foreach (OrderLine l in cart)
			{
				if (l.key == p.key)
				{
					l.qty++;
					Say(p.name + " added to your cart.", false);
					return;
				}
			}
			cart.Add(new OrderLine { key = p.key, name = p.name, price = p.price, qty = 1 });
			Say(p.name + " added to your cart.", false);
		}

		private static float Subtotal()
		{
			float s = 0f;
			foreach (OrderLine l in cart)
			{
				s += l.price * l.qty;
			}
			return s;
		}

		private static float Shipping(float subtotal)
		{
			if (JunkAutoMod.FreeShippingOver > 0f && subtotal >= JunkAutoMod.FreeShippingOver)
			{
				return 0f;
			}
			return Mathf.Max(0f, JunkAutoMod.ShippingCost);
		}

		private static void Checkout(float sub, float ship)
		{
			string why;
			Vector3 spot;
			if (!JunkAutoMod.Pay(sub + ship, out why, out spot))
			{
				Say(why, true);
				return;
			}
			Order o = Orders.Place(cart, ship, spot, JunkAutoMod.DeliveryMinutes);
			cart.Clear();
			view = View.Orders;
			Say("Order #" + o.id + " placed! A confirmation is in your mail.", false);
		}

		private static void Say(string text, bool bad)
		{
			message = text;
			messageBad = bad;
		}

		public static void ResetForLevel()
		{
			cart.Clear();
			message = "";
			view = View.Catalog;
		}

		// --- Helpers. ---

		private static string Money(float v)
		{
			return v.ToString("#,0.00", Inv);
		}

		private static string Clock(float seconds)
		{
			int s = Mathf.Max(0, Mathf.CeilToInt(seconds));
			return s / 60 + ":" + (s % 60).ToString("00", Inv);
		}

		private class VehicleOrder : IComparer<string>
		{
			private static int Rank(string v)
			{
				return v == Catalog.Truck ? 0 : v == Catalog.Bike ? 1 : v == "VEHICLES" ? 2 : 3;
			}

			public int Compare(string a, string b)
			{
				int c = Rank(a).CompareTo(Rank(b));
				return c != 0 ? c : string.CompareOrdinal(a, b);
			}
		}

		private static GUIStyle Text(Color color, int size, FontStyle style, TextAnchor anchor)
		{
			GUIStyle s = new GUIStyle(GUI.skin.label);
			s.normal.textColor = color;
			s.hover.textColor = color;
			s.active.textColor = color;
			s.fontSize = size;
			s.fontStyle = style;
			s.alignment = anchor;
			s.wordWrap = false;
			s.clipping = TextClipping.Clip;
			s.padding = new RectOffset(2, 2, 0, 0);
			s.margin = new RectOffset(0, 0, 0, 0);
			return s;
		}

		private static GUIStyle Button(Color bg, Color hover, Color text, int size)
		{
			GUIStyle s = Text(text, size, FontStyle.Bold, TextAnchor.MiddleCenter);
			s.normal.background = Tex(bg);
			s.hover.background = Tex(hover);
			s.active.background = Tex(hover);
			s.hover.textColor = text;
			s.active.textColor = text;
			return s;
		}

		private static readonly Dictionary<Color, GUIStyle> fills = new Dictionary<Color, GUIStyle>();

		private static GUIStyle Fill(Color c)
		{
			GUIStyle s;
			if (!fills.TryGetValue(c, out s))
			{
				s = new GUIStyle();
				s.normal.background = Tex(c);
				fills[c] = s;
			}
			return s;
		}

		private static Texture2D Tex(Color c)
		{
			Texture2D t = new Texture2D(1, 1);
			t.SetPixel(0, 0, c);
			t.Apply();
			t.hideFlags = HideFlags.HideAndDontSave;
			return t;
		}

		private static Color Hex(int rgb)
		{
			return new Color(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f);
		}
	}
}
