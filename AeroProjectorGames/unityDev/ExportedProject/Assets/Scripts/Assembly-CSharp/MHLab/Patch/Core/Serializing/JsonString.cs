using System.Text;

namespace MHLab.Patch.Core.Serializing
{
	public class JsonString : JsonNode
	{
		private string m_Data;

		public override JsonNodeType Tag => JsonNodeType.String;

		public override bool IsString => true;

		public override string Value
		{
			get
			{
				return m_Data;
			}
			set
			{
				m_Data = value;
			}
		}

		public override Enumerator GetEnumerator()
		{
			return default(Enumerator);
		}

		public JsonString(string aData)
		{
			m_Data = aData;
		}

		public override JsonNode Clone()
		{
			return new JsonString(m_Data);
		}

		public override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JsonTextMode aMode)
		{
			aSB.Append('"').Append(JsonNode.Escape(m_Data)).Append('"');
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				return true;
			}
			if (obj is string text)
			{
				return m_Data == text;
			}
			JsonString jsonString = obj as JsonString;
			return jsonString != null && m_Data == jsonString.m_Data;
		}

		public override int GetHashCode()
		{
			return m_Data.GetHashCode();
		}

		public override void Clear()
		{
			m_Data = "";
		}
	}
}
