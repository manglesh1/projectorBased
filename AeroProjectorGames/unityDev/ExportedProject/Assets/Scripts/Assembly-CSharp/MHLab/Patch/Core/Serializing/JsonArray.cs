using System.Collections.Generic;
using System.Text;

namespace MHLab.Patch.Core.Serializing
{
	public class JsonArray : JsonNode
	{
		private List<JsonNode> m_List = new List<JsonNode>();

		private bool inline;

		public override bool Inline
		{
			get
			{
				return inline;
			}
			set
			{
				inline = value;
			}
		}

		public override JsonNodeType Tag => JsonNodeType.Array;

		public override bool IsArray => true;

		public override JsonNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= m_List.Count)
				{
					return new JsonLazyCreator(this);
				}
				return m_List[aIndex];
			}
			set
			{
				if (value == null)
				{
					value = JsonNull.CreateOrGet();
				}
				if (aIndex < 0 || aIndex >= m_List.Count)
				{
					m_List.Add(value);
				}
				else
				{
					m_List[aIndex] = value;
				}
			}
		}

		public override JsonNode this[string aKey]
		{
			get
			{
				return new JsonLazyCreator(this);
			}
			set
			{
				if (value == null)
				{
					value = JsonNull.CreateOrGet();
				}
				m_List.Add(value);
			}
		}

		public override int Count => m_List.Count;

		public override IEnumerable<JsonNode> Children
		{
			get
			{
				foreach (JsonNode item in m_List)
				{
					yield return item;
				}
			}
		}

		public override Enumerator GetEnumerator()
		{
			return new Enumerator(m_List.GetEnumerator());
		}

		public override void Add(string aKey, JsonNode aItem)
		{
			if (aItem == null)
			{
				aItem = JsonNull.CreateOrGet();
			}
			m_List.Add(aItem);
		}

		public override JsonNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_List.Count)
			{
				return null;
			}
			JsonNode result = m_List[aIndex];
			m_List.RemoveAt(aIndex);
			return result;
		}

		public override JsonNode Remove(JsonNode aNode)
		{
			m_List.Remove(aNode);
			return aNode;
		}

		public override void Clear()
		{
			m_List.Clear();
		}

		public override JsonNode Clone()
		{
			JsonArray jsonArray = new JsonArray();
			jsonArray.m_List.Capacity = m_List.Capacity;
			foreach (JsonNode item in m_List)
			{
				if (item != null)
				{
					jsonArray.Add(item.Clone());
				}
				else
				{
					jsonArray.Add(null);
				}
			}
			return jsonArray;
		}

		public override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JsonTextMode aMode)
		{
			aSB.Append('[');
			int count = m_List.Count;
			if (inline)
			{
				aMode = JsonTextMode.Compact;
			}
			for (int i = 0; i < count; i++)
			{
				if (i > 0)
				{
					aSB.Append(',');
				}
				if (aMode == JsonTextMode.Indent)
				{
					aSB.AppendLine();
				}
				if (aMode == JsonTextMode.Indent)
				{
					aSB.Append(' ', aIndent + aIndentInc);
				}
				m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
			}
			if (aMode == JsonTextMode.Indent)
			{
				aSB.AppendLine().Append(' ', aIndent);
			}
			aSB.Append(']');
		}
	}
}
