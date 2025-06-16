using System;
using System.Text;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core
{
	[Serializable]
	public class BuildDefinition : IJsonSerializable
	{
		public BuildDefinitionEntry[] Entries;

		public string ToJson()
		{
			JsonNode jsonNode = ToJsonNode();
			StringBuilder stringBuilder = new StringBuilder();
			jsonNode.WriteToStringBuilder(stringBuilder, 2, 2, JsonTextMode.Indent);
			return stringBuilder.ToString();
		}

		public JsonNode ToJsonNode()
		{
			JsonObject jsonObject = new JsonObject();
			JsonArray jsonArray = new JsonArray();
			BuildDefinitionEntry[] entries = Entries;
			for (int i = 0; i < entries.Length; i++)
			{
				JsonNode aItem = entries[i].ToJsonNode();
				jsonArray.Add(aItem);
			}
			jsonObject.Add("Entries", jsonArray);
			return jsonObject;
		}

		public void FromJson(JsonNode node)
		{
			if (node.HasKey("Entries"))
			{
				JsonArray asArray = node["Entries"].AsArray;
				Entries = new BuildDefinitionEntry[asArray.Count];
				for (int i = 0; i < asArray.Count; i++)
				{
					JsonNode node2 = asArray[i];
					BuildDefinitionEntry buildDefinitionEntry = new BuildDefinitionEntry();
					buildDefinitionEntry.FromJson(node2);
					Entries[i] = buildDefinitionEntry;
				}
			}
			else
			{
				Entries = new BuildDefinitionEntry[0];
			}
		}
	}
}
