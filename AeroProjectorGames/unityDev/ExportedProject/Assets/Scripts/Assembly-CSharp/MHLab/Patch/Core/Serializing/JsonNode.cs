using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MHLab.Patch.Core.Serializing
{
	public abstract class JsonNode
	{
		public struct Enumerator
		{
			private enum Type
			{
				None = 0,
				Array = 1,
				Object = 2
			}

			private Type type;

			private Dictionary<string, JsonNode>.Enumerator m_Object;

			private List<JsonNode>.Enumerator m_Array;

			public bool IsValid => type > Type.None;

			public KeyValuePair<string, JsonNode> Current
			{
				get
				{
					if (type == Type.Array)
					{
						return new KeyValuePair<string, JsonNode>(string.Empty, m_Array.Current);
					}
					if (type == Type.Object)
					{
						return m_Object.Current;
					}
					return new KeyValuePair<string, JsonNode>(string.Empty, null);
				}
			}

			public Enumerator(List<JsonNode>.Enumerator aArrayEnum)
			{
				type = Type.Array;
				m_Object = default(Dictionary<string, JsonNode>.Enumerator);
				m_Array = aArrayEnum;
			}

			public Enumerator(Dictionary<string, JsonNode>.Enumerator aDictEnum)
			{
				type = Type.Object;
				m_Object = aDictEnum;
				m_Array = default(List<JsonNode>.Enumerator);
			}

			public bool MoveNext()
			{
				if (type == Type.Array)
				{
					return m_Array.MoveNext();
				}
				return type == Type.Object && m_Object.MoveNext();
			}
		}

		public struct ValueEnumerator
		{
			private Enumerator m_Enumerator;

			public JsonNode Current => m_Enumerator.Current.Value;

			public ValueEnumerator(List<JsonNode>.Enumerator aArrayEnum)
				: this(new Enumerator(aArrayEnum))
			{
			}

			public ValueEnumerator(Dictionary<string, JsonNode>.Enumerator aDictEnum)
				: this(new Enumerator(aDictEnum))
			{
			}

			public ValueEnumerator(Enumerator aEnumerator)
			{
				m_Enumerator = aEnumerator;
			}

			public bool MoveNext()
			{
				return m_Enumerator.MoveNext();
			}

			public ValueEnumerator GetEnumerator()
			{
				return this;
			}
		}

		public struct KeyEnumerator
		{
			private Enumerator m_Enumerator;

			public string Current => m_Enumerator.Current.Key;

			public KeyEnumerator(List<JsonNode>.Enumerator aArrayEnum)
				: this(new Enumerator(aArrayEnum))
			{
			}

			public KeyEnumerator(Dictionary<string, JsonNode>.Enumerator aDictEnum)
				: this(new Enumerator(aDictEnum))
			{
			}

			public KeyEnumerator(Enumerator aEnumerator)
			{
				m_Enumerator = aEnumerator;
			}

			public bool MoveNext()
			{
				return m_Enumerator.MoveNext();
			}

			public KeyEnumerator GetEnumerator()
			{
				return this;
			}
		}

		public class LinqEnumerator : IEnumerator<KeyValuePair<string, JsonNode>>, IDisposable, IEnumerator, IEnumerable<KeyValuePair<string, JsonNode>>, IEnumerable
		{
			private JsonNode m_Node;

			private Enumerator m_Enumerator;

			public KeyValuePair<string, JsonNode> Current => m_Enumerator.Current;

			object IEnumerator.Current => m_Enumerator.Current;

			internal LinqEnumerator(JsonNode aNode)
			{
				m_Node = aNode;
				if (m_Node != null)
				{
					m_Enumerator = m_Node.GetEnumerator();
				}
			}

			public bool MoveNext()
			{
				return m_Enumerator.MoveNext();
			}

			public void Dispose()
			{
				m_Node = null;
				m_Enumerator = default(Enumerator);
			}

			public IEnumerator<KeyValuePair<string, JsonNode>> GetEnumerator()
			{
				return new LinqEnumerator(m_Node);
			}

			public void Reset()
			{
				if (m_Node != null)
				{
					m_Enumerator = m_Node.GetEnumerator();
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new LinqEnumerator(m_Node);
			}
		}

		public static bool forceASCII = false;

		public static bool longAsString = false;

		public static bool allowLineComments = true;

		[ThreadStatic]
		private static StringBuilder m_EscapeBuilder;

		public abstract JsonNodeType Tag { get; }

		public virtual JsonNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual JsonNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual string Value
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public virtual int Count => 0;

		public virtual bool IsNumber => false;

		public virtual bool IsString => false;

		public virtual bool IsBoolean => false;

		public virtual bool IsNull => false;

		public virtual bool IsArray => false;

		public virtual bool IsObject => false;

		public virtual bool Inline
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public virtual IEnumerable<JsonNode> Children
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<JsonNode> DeepChildren
		{
			get
			{
				foreach (JsonNode jsonNode in Children)
				{
					foreach (JsonNode deepChild in jsonNode.DeepChildren)
					{
						yield return deepChild;
					}
				}
			}
		}

		public IEnumerable<KeyValuePair<string, JsonNode>> Linq => new LinqEnumerator(this);

		public KeyEnumerator Keys => new KeyEnumerator(GetEnumerator());

		public ValueEnumerator Values => new ValueEnumerator(GetEnumerator());

		public virtual double AsDouble
		{
			get
			{
				double result = 0.0;
				if (double.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				Value = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public virtual int AsInt
		{
			get
			{
				return (int)AsDouble;
			}
			set
			{
				AsDouble = value;
			}
		}

		public virtual float AsFloat
		{
			get
			{
				return (float)AsDouble;
			}
			set
			{
				AsDouble = value;
			}
		}

		public virtual bool AsBool
		{
			get
			{
				bool result = false;
				if (bool.TryParse(Value, out result))
				{
					return result;
				}
				return !string.IsNullOrEmpty(Value);
			}
			set
			{
				Value = (value ? "true" : "false");
			}
		}

		public virtual long AsLong
		{
			get
			{
				long result = 0L;
				if (long.TryParse(Value, out result))
				{
					return result;
				}
				return 0L;
			}
			set
			{
				Value = value.ToString();
			}
		}

		public virtual ulong AsULong
		{
			get
			{
				ulong result = 0uL;
				if (ulong.TryParse(Value, out result))
				{
					return result;
				}
				return 0uL;
			}
			set
			{
				Value = value.ToString();
			}
		}

		public virtual JsonArray AsArray => this as JsonArray;

		public virtual JsonObject AsObject => this as JsonObject;

		internal static StringBuilder EscapeBuilder
		{
			get
			{
				if (m_EscapeBuilder == null)
				{
					m_EscapeBuilder = new StringBuilder();
				}
				return m_EscapeBuilder;
			}
		}

		public virtual void Add(string aKey, JsonNode aItem)
		{
		}

		public virtual void Add(JsonNode aItem)
		{
			Add("", aItem);
		}

		public virtual JsonNode Remove(string aKey)
		{
			return null;
		}

		public virtual JsonNode Remove(int aIndex)
		{
			return null;
		}

		public virtual JsonNode Remove(JsonNode aNode)
		{
			return aNode;
		}

		public virtual void Clear()
		{
		}

		public virtual JsonNode Clone()
		{
			return null;
		}

		public virtual bool HasKey(string aKey)
		{
			return false;
		}

		public virtual JsonNode GetValueOrDefault(string aKey, JsonNode aDefault)
		{
			return aDefault;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			WriteToStringBuilder(stringBuilder, 0, 0, JsonTextMode.Compact);
			return stringBuilder.ToString();
		}

		public virtual string ToString(int aIndent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			WriteToStringBuilder(stringBuilder, 0, aIndent, JsonTextMode.Indent);
			return stringBuilder.ToString();
		}

		public abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JsonTextMode aMode);

		public abstract Enumerator GetEnumerator();

		public static implicit operator JsonNode(string s)
		{
			if (s != null)
			{
				return new JsonString(s);
			}
			return JsonNull.CreateOrGet();
		}

		public static implicit operator string(JsonNode d)
		{
			if (!(d == null))
			{
				return d.Value;
			}
			return null;
		}

		public static implicit operator JsonNode(double n)
		{
			return new JsonNumber(n);
		}

		public static implicit operator double(JsonNode d)
		{
			if (!(d == null))
			{
				return d.AsDouble;
			}
			return 0.0;
		}

		public static implicit operator JsonNode(float n)
		{
			return new JsonNumber(n);
		}

		public static implicit operator float(JsonNode d)
		{
			if (!(d == null))
			{
				return d.AsFloat;
			}
			return 0f;
		}

		public static implicit operator JsonNode(int n)
		{
			return new JsonNumber(n);
		}

		public static implicit operator int(JsonNode d)
		{
			if (!(d == null))
			{
				return d.AsInt;
			}
			return 0;
		}

		public static implicit operator JsonNode(long n)
		{
			if (longAsString)
			{
				return new JsonString(n.ToString());
			}
			return new JsonNumber(n);
		}

		public static implicit operator long(JsonNode d)
		{
			if (!(d == null))
			{
				return d.AsLong;
			}
			return 0L;
		}

		public static implicit operator JsonNode(ulong n)
		{
			if (longAsString)
			{
				return new JsonString(n.ToString());
			}
			return new JsonNumber(n);
		}

		public static implicit operator ulong(JsonNode d)
		{
			if (!(d == null))
			{
				return d.AsULong;
			}
			return 0uL;
		}

		public static implicit operator JsonNode(bool b)
		{
			return new JsonBool(b);
		}

		public static implicit operator bool(JsonNode d)
		{
			return !(d == null) && d.AsBool;
		}

		public static implicit operator JsonNode(KeyValuePair<string, JsonNode> aKeyValue)
		{
			return aKeyValue.Value;
		}

		public static bool operator ==(JsonNode a, object b)
		{
			if (a == b)
			{
				return true;
			}
			bool flag = a is JsonNull || a == null || a is JsonLazyCreator;
			bool flag2 = b is JsonNull || b == null || b is JsonLazyCreator;
			return (flag && flag2) || (!flag && a.Equals(b));
		}

		public static bool operator !=(JsonNode a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static string Escape(string aText)
		{
			StringBuilder escapeBuilder = EscapeBuilder;
			escapeBuilder.Length = 0;
			if (escapeBuilder.Capacity < aText.Length + aText.Length / 10)
			{
				escapeBuilder.Capacity = aText.Length + aText.Length / 10;
			}
			foreach (char c in aText)
			{
				switch (c)
				{
				case '\b':
					escapeBuilder.Append("\\b");
					continue;
				case '\t':
					escapeBuilder.Append("\\t");
					continue;
				case '\n':
					escapeBuilder.Append("\\n");
					continue;
				case '\f':
					escapeBuilder.Append("\\f");
					continue;
				case '\r':
					escapeBuilder.Append("\\r");
					continue;
				case '\\':
					escapeBuilder.Append("\\\\");
					continue;
				case '"':
					escapeBuilder.Append("\\\"");
					continue;
				}
				if (c < ' ' || (forceASCII && c > '\u007f'))
				{
					ushort num = c;
					escapeBuilder.Append("\\u").Append(num.ToString("X4"));
				}
				else
				{
					escapeBuilder.Append(c);
				}
			}
			string result = escapeBuilder.ToString();
			escapeBuilder.Length = 0;
			return result;
		}

		private static JsonNode ParseElement(string token, bool quoted)
		{
			if (quoted)
			{
				return token;
			}
			if (token.Length <= 5)
			{
				string text = token.ToLower();
				if (text == "false" || text == "true")
				{
					return text == "true";
				}
				if (text == "null")
				{
					return JsonNull.CreateOrGet();
				}
			}
			if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
			{
				return result;
			}
			return token;
		}

		public static JsonNode Parse(string aJSON)
		{
			Stack<JsonNode> stack = new Stack<JsonNode>();
			JsonNode jsonNode = null;
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			string aKey = "";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (; i < aJSON.Length; i++)
			{
				switch (aJSON[i])
				{
				case '\n':
				case '\r':
					flag3 = true;
					break;
				case '\t':
				case ' ':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
					}
					break;
				case '/':
					if (allowLineComments && !flag && i + 1 < aJSON.Length && aJSON[i + 1] == '/')
					{
						while (++i < aJSON.Length && aJSON[i] != '\n' && aJSON[i] != '\r')
						{
						}
					}
					else
					{
						stringBuilder.Append(aJSON[i]);
					}
					break;
				case ',':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
						break;
					}
					if (stringBuilder.Length > 0 || flag2)
					{
						jsonNode.Add(aKey, ParseElement(stringBuilder.ToString(), flag2));
					}
					aKey = "";
					stringBuilder.Length = 0;
					flag2 = false;
					break;
				case '"':
					flag = !flag;
					flag2 = flag2 || flag;
					break;
				case '[':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
						break;
					}
					stack.Push(new JsonArray());
					if (jsonNode != null)
					{
						jsonNode.Add(aKey, stack.Peek());
					}
					aKey = "";
					stringBuilder.Length = 0;
					jsonNode = stack.Peek();
					flag3 = false;
					break;
				case '\\':
					i++;
					if (flag)
					{
						char c = aJSON[i];
						switch (c)
						{
						case 'b':
							stringBuilder.Append('\b');
							break;
						case 'f':
							stringBuilder.Append('\f');
							break;
						case 'n':
							stringBuilder.Append('\n');
							break;
						case 'r':
							stringBuilder.Append('\r');
							break;
						case 't':
							stringBuilder.Append('\t');
							break;
						case 'u':
						{
							string s = aJSON.Substring(i + 1, 4);
							stringBuilder.Append((char)int.Parse(s, NumberStyles.AllowHexSpecifier));
							i += 4;
							break;
						}
						default:
							stringBuilder.Append(c);
							break;
						}
					}
					break;
				case ':':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
						break;
					}
					aKey = stringBuilder.ToString();
					stringBuilder.Length = 0;
					flag2 = false;
					break;
				case '{':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
						break;
					}
					stack.Push(new JsonObject());
					if (jsonNode != null)
					{
						jsonNode.Add(aKey, stack.Peek());
					}
					aKey = "";
					stringBuilder.Length = 0;
					jsonNode = stack.Peek();
					flag3 = false;
					break;
				case ']':
				case '}':
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
						break;
					}
					if (stack.Count == 0)
					{
						throw new Exception("JSON Parse: Too many closing brackets");
					}
					stack.Pop();
					if (stringBuilder.Length > 0 || flag2)
					{
						jsonNode.Add(aKey, ParseElement(stringBuilder.ToString(), flag2));
					}
					if (jsonNode != null)
					{
						jsonNode.Inline = !flag3;
					}
					flag2 = false;
					aKey = "";
					stringBuilder.Length = 0;
					if (stack.Count > 0)
					{
						jsonNode = stack.Peek();
					}
					break;
				case '\ufeff':
					break;
				default:
					stringBuilder.Append(aJSON[i]);
					break;
				}
			}
			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			if (jsonNode == null)
			{
				return ParseElement(stringBuilder.ToString(), flag2);
			}
			return jsonNode;
		}
	}
}
