using System;
using System.Globalization;
using System.Text;

namespace MHLab.Patch.Core.Serializing
{
	public class JsonNumber : JsonNode
	{
		private double m_Data;

		public override JsonNodeType Tag => JsonNodeType.Number;

		public override bool IsNumber => true;

		public override string Value
		{
			get
			{
				return m_Data.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
				{
					m_Data = result;
				}
			}
		}

		public override double AsDouble
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

		public override long AsLong
		{
			get
			{
				return (long)m_Data;
			}
			set
			{
				m_Data = value;
			}
		}

		public override ulong AsULong
		{
			get
			{
				return (ulong)m_Data;
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

		public JsonNumber(double aData)
		{
			m_Data = aData;
		}

		public JsonNumber(string aData)
		{
			Value = aData;
		}

		public override JsonNode Clone()
		{
			return new JsonNumber(m_Data);
		}

		public override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JsonTextMode aMode)
		{
			aSB.Append(Value);
		}

		private static bool IsNumeric(object value)
		{
			return value is int || value is uint || value is float || value is double || value is decimal || value is long || value is ulong || value is short || value is ushort || value is sbyte || value is byte;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			JsonNumber jsonNumber = obj as JsonNumber;
			if (jsonNumber != null)
			{
				return m_Data == jsonNumber.m_Data;
			}
			return IsNumeric(obj) && Convert.ToDouble(obj) == m_Data;
		}

		public override int GetHashCode()
		{
			return m_Data.GetHashCode();
		}

		public override void Clear()
		{
			m_Data = 0.0;
		}
	}
}
