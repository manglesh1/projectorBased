using System.Text;

namespace MHLab.Patch.Core.Serializing
{
	public class JsonNull : JsonNode
	{
		private static JsonNull m_StaticInstance = new JsonNull();

		public static bool reuseSameInstance = true;

		public override JsonNodeType Tag => JsonNodeType.NullValue;

		public override bool IsNull => true;

		public override string Value
		{
			get
			{
				return "null";
			}
			set
			{
			}
		}

		public override bool AsBool
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public static JsonNull CreateOrGet()
		{
			if (reuseSameInstance)
			{
				return m_StaticInstance;
			}
			return new JsonNull();
		}

		private JsonNull()
		{
		}

		public override Enumerator GetEnumerator()
		{
			return default(Enumerator);
		}

		public override JsonNode Clone()
		{
			return CreateOrGet();
		}

		public override bool Equals(object obj)
		{
			return this == obj || obj is JsonNull;
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
