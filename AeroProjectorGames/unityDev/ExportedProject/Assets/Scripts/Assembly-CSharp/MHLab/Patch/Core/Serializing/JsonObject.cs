using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHLab.Patch.Core.Serializing
{
	public class JsonObject : JsonNode
	{
		private Dictionary<string, JsonNode> m_Dict = new Dictionary<string, JsonNode>();

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

		public override JsonNodeType Tag => JsonNodeType.Object;

		public override bool IsObject => true;

		public override JsonNode this[string aKey]
		{
			get
			{
				if (m_Dict.ContainsKey(aKey))
				{
					return m_Dict[aKey];
				}
				return new JsonLazyCreator(this, aKey);
			}
			set
			{
				if (value == null)
				{
					value = JsonNull.CreateOrGet();
				}
				if (m_Dict.ContainsKey(aKey))
				{
					m_Dict[aKey] = value;
				}
				else
				{
					m_Dict.Add(aKey, value);
				}
			}
		}

		public override JsonNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= m_Dict.Count)
				{
					return null;
				}
				return m_Dict.ElementAt(aIndex).Value;
			}
			set
			{
				if (value == null)
				{
					value = JsonNull.CreateOrGet();
				}
				if (aIndex >= 0 && aIndex < m_Dict.Count)
				{
					string key = m_Dict.ElementAt(aIndex).Key;
					m_Dict[key] = value;
				}
			}
		}

		public override int Count => m_Dict.Count;

		public override IEnumerable<JsonNode> Children
		{
			get
			{
				foreach (KeyValuePair<string, JsonNode> item in m_Dict)
				{
					yield return item.Value;
				}
			}
		}

		public override Enumerator GetEnumerator()
		{
			return new Enumerator(m_Dict.GetEnumerator());
		}

		public override void Add(string aKey, JsonNode aItem)
		{
			if (aItem == null)
			{
				aItem = JsonNull.CreateOrGet();
			}
			if (aKey == null)
			{
				m_Dict.Add(Guid.NewGuid().ToString(), aItem);
			}
			else if (m_Dict.ContainsKey(aKey))
			{
				m_Dict[aKey] = aItem;
			}
			else
			{
				m_Dict.Add(aKey, aItem);
			}
		}

		public override JsonNode Remove(string aKey)
		{
			if (!m_Dict.ContainsKey(aKey))
			{
				return null;
			}
			JsonNode result = m_Dict[aKey];
			m_Dict.Remove(aKey);
			return result;
		}

		public override JsonNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_Dict.Count)
			{
				return null;
			}
			KeyValuePair<string, JsonNode> keyValuePair = m_Dict.ElementAt(aIndex);
			m_Dict.Remove(keyValuePair.Key);
			return keyValuePair.Value;
		}

		public override JsonNode Remove(JsonNode aNode)
		{
			try
			{
				KeyValuePair<string, JsonNode> keyValuePair = m_Dict.Where(delegate(KeyValuePair<string, JsonNode> k)
				{
					KeyValuePair<string, JsonNode> keyValuePair2 = k;
					return keyValuePair2.Value == aNode;
				}).First();
				m_Dict.Remove(keyValuePair.Key);
				return aNode;
			}
			catch
			{
				return null;
			}
		}

		public override void Clear()
		{
			m_Dict.Clear();
		}

		public override JsonNode Clone()
		{
			JsonObject jsonObject = new JsonObject();
			foreach (KeyValuePair<string, JsonNode> item in m_Dict)
			{
				jsonObject.Add(item.Key, item.Value.Clone());
			}
			return jsonObject;
		}

		public override bool HasKey(string aKey)
		{
			return m_Dict.ContainsKey(aKey);
		}

		public override JsonNode GetValueOrDefault(string aKey, JsonNode aDefault)
		{
			if (m_Dict.TryGetValue(aKey, out var value))
			{
				return value;
			}
			return aDefault;
		}

		public override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JsonTextMode aMode)
		{
			aSB.Append('{');
			bool flag = true;
			if (inline)
			{
				aMode = JsonTextMode.Compact;
			}
			foreach (KeyValuePair<string, JsonNode> item in m_Dict)
			{
				if (!flag)
				{
					aSB.Append(',');
				}
				flag = false;
				if (aMode == JsonTextMode.Indent)
				{
					aSB.AppendLine();
				}
				if (aMode == JsonTextMode.Indent)
				{
					aSB.Append(' ', aIndent + aIndentInc);
				}
				aSB.Append('"').Append(JsonNode.Escape(item.Key)).Append('"');
				if (aMode == JsonTextMode.Compact)
				{
					aSB.Append(':');
				}
				else
				{
					aSB.Append(" : ");
				}
				item.Value.WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
			}
			if (aMode == JsonTextMode.Indent)
			{
				aSB.AppendLine().Append(' ', aIndent);
			}
			aSB.Append('}');
		}
	}
}
