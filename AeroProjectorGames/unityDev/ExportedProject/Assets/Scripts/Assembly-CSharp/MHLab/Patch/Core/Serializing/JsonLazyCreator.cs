using System.Text;

namespace MHLab.Patch.Core.Serializing
{
	public class JsonLazyCreator : JsonNode
	{
		private JsonNode m_Node;

		private string m_Key;

		public override JsonNodeType Tag => JsonNodeType.None;

		public override JsonNode this[int aIndex]
		{
			get
			{
				return new JsonLazyCreator(this);
			}
			set
			{
				Set(new JsonArray()).Add(value);
			}
		}

		public override JsonNode this[string aKey]
		{
			get
			{
				return new JsonLazyCreator(this, aKey);
			}
			set
			{
				Set(new JsonObject()).Add(aKey, value);
			}
		}

		public override int AsInt
		{
			get
			{
				Set(new JsonNumber(0.0));
				return 0;
			}
			set
			{
				Set(new JsonNumber(value));
			}
		}

		public override float AsFloat
		{
			get
			{
				Set(new JsonNumber(0.0));
				return 0f;
			}
			set
			{
				Set(new JsonNumber(value));
			}
		}

		public override double AsDouble
		{
			get
			{
				Set(new JsonNumber(0.0));
				return 0.0;
			}
			set
			{
				Set(new JsonNumber(value));
			}
		}

		public override long AsLong
		{
			get
			{
				if (JsonNode.longAsString)
				{
					Set(new JsonString("0"));
				}
				else
				{
					Set(new JsonNumber(0.0));
				}
				return 0L;
			}
			set
			{
				if (JsonNode.longAsString)
				{
					Set(new JsonString(value.ToString()));
				}
				else
				{
					Set(new JsonNumber(value));
				}
			}
		}

		public override ulong AsULong
		{
			get
			{
				if (JsonNode.longAsString)
				{
					Set(new JsonString("0"));
				}
				else
				{
					Set(new JsonNumber(0.0));
				}
				return 0uL;
			}
			set
			{
				if (JsonNode.longAsString)
				{
					Set(new JsonString(value.ToString()));
				}
				else
				{
					Set(new JsonNumber(value));
				}
			}
		}

		public override bool AsBool
		{
			get
			{
				Set(new JsonBool(aData: false));
				return false;
			}
			set
			{
				Set(new JsonBool(value));
			}
		}

		public override JsonArray AsArray => Set(new JsonArray());

		public override JsonObject AsObject => Set(new JsonObject());

		public override Enumerator GetEnumerator()
		{
			return default(Enumerator);
		}

		public JsonLazyCreator(JsonNode aNode)
		{
			m_Node = aNode;
			m_Key = null;
		}

		public JsonLazyCreator(JsonNode aNode, string aKey)
		{
			m_Node = aNode;
			m_Key = aKey;
		}

		private T Set<T>(T aVal) where T : JsonNode
		{
			if (m_Key == null)
			{
				m_Node.Add(aVal);
			}
			else
			{
				m_Node.Add(m_Key, aVal);
			}
			m_Node = null;
			return aVal;
		}

		public override void Add(JsonNode aItem)
		{
			Set(new JsonArray()).Add(aItem);
		}

		public override void Add(string aKey, JsonNode aItem)
		{
			Set(new JsonObject()).Add(aKey, aItem);
		}

		public static bool operator ==(JsonLazyCreator a, object b)
		{
			return b == null || a == b;
		}

		public static bool operator !=(JsonLazyCreator a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return obj == null || this == obj;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JsonTextMode aMode)
		{
			aSB.Append("null");
		}
	}
}
