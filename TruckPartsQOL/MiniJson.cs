using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TruckPartsQOL
{
	// A small JSON reader for glTF: objects become Dictionary<string, object>,
	// arrays List<object>, numbers double, plus string/bool/null.
	internal static class MiniJson
	{
		public static object Parse(string json)
		{
			int i = 0;
			object value = ReadValue(json, ref i);
			return value;
		}

		private static void SkipSpace(string s, ref int i)
		{
			while (i < s.Length && char.IsWhiteSpace(s[i]))
			{
				i++;
			}
		}

		private static object ReadValue(string s, ref int i)
		{
			SkipSpace(s, ref i);
			if (i >= s.Length)
			{
				throw new FormatException("Unexpected end of JSON");
			}
			char c = s[i];
			if (c == '{')
			{
				Dictionary<string, object> obj = new Dictionary<string, object>();
				i++;
				SkipSpace(s, ref i);
				if (s[i] == '}')
				{
					i++;
					return obj;
				}
				while (true)
				{
					SkipSpace(s, ref i);
					string key = ReadString(s, ref i);
					SkipSpace(s, ref i);
					if (s[i] != ':')
					{
						throw new FormatException("Expected ':' at " + i);
					}
					i++;
					obj[key] = ReadValue(s, ref i);
					SkipSpace(s, ref i);
					if (s[i] == ',')
					{
						i++;
						continue;
					}
					if (s[i] == '}')
					{
						i++;
						return obj;
					}
					throw new FormatException("Expected ',' or '}' at " + i);
				}
			}
			if (c == '[')
			{
				List<object> list = new List<object>();
				i++;
				SkipSpace(s, ref i);
				if (s[i] == ']')
				{
					i++;
					return list;
				}
				while (true)
				{
					list.Add(ReadValue(s, ref i));
					SkipSpace(s, ref i);
					if (s[i] == ',')
					{
						i++;
						continue;
					}
					if (s[i] == ']')
					{
						i++;
						return list;
					}
					throw new FormatException("Expected ',' or ']' at " + i);
				}
			}
			if (c == '"')
			{
				return ReadString(s, ref i);
			}
			if (s.Length - i >= 4 && string.CompareOrdinal(s, i, "true", 0, 4) == 0)
			{
				i += 4;
				return true;
			}
			if (s.Length - i >= 5 && string.CompareOrdinal(s, i, "false", 0, 5) == 0)
			{
				i += 5;
				return false;
			}
			if (s.Length - i >= 4 && string.CompareOrdinal(s, i, "null", 0, 4) == 0)
			{
				i += 4;
				return null;
			}
			int start = i;
			while (i < s.Length && "+-0123456789.eE".IndexOf(s[i]) >= 0)
			{
				i++;
			}
			return double.Parse(s.Substring(start, i - start), NumberStyles.Float, CultureInfo.InvariantCulture);
		}

		private static string ReadString(string s, ref int i)
		{
			if (s[i] != '"')
			{
				throw new FormatException("Expected string at " + i);
			}
			i++;
			StringBuilder sb = new StringBuilder();
			while (s[i] != '"')
			{
				char c = s[i++];
				if (c != '\\')
				{
					sb.Append(c);
					continue;
				}
				char e = s[i++];
				switch (e)
				{
				case 'n':
					sb.Append('\n');
					break;
				case 't':
					sb.Append('\t');
					break;
				case 'r':
					sb.Append('\r');
					break;
				case 'b':
					sb.Append('\b');
					break;
				case 'f':
					sb.Append('\f');
					break;
				case 'u':
					sb.Append((char)int.Parse(s.Substring(i, 4), NumberStyles.HexNumber));
					i += 4;
					break;
				default:
					sb.Append(e);
					break;
				}
			}
			i++;
			return sb.ToString();
		}
	}
}
