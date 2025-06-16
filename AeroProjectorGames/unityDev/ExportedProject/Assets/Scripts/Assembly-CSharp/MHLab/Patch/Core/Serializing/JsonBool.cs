using System.Text;

namespace MHLab.Patch.Core.Serializing
{
	public class JsonBool : JsonNode
	{
		private bool m_Data;

		public override JsonNodeType Tag => JsonNodeType.Boolean;

		public override bool IsBoolean => true;

		public override string Value
		{
			get
			{
				return m_Data.ToString();
			}
			set
			{
				if (bool.TryParse(value, out var result))
				{
					m_Data = result;
				}
			}
		}

		public override bool AsBool
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

		public JsonBool(bool aData)
		{
			m_Data = aData;
		}

		public JsonBool(string aData)
		{
			Value = aData;
		}

		public override JsonNode Clone()
		{
			return new JsonBool(m_Data);
		}

		public override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JsonTextMode aMode)
		{
			aSB.Append(m_Data ? "true" : "false");
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is bool && m_Data == (bool)obj;
		}

		public override int GetHashCode()
		{
			return m_Data.GetHashCode();
		}

		public override void Clear()
		{
			m_Data = false;
		}
	}
}
