using System;
using System.Text;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core
{
	[Serializable]
	public class UpdaterDefinition : IJsonSerializable
	{
		public UpdaterDefinitionEntry[] Entries;

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
			for (int i = 0; i < Entries.Length; i++)
			{
				UpdaterDefinitionEntry updaterDefinitionEntry = Entries[i];
				jsonArray.Add(updaterDefinitionEntry.ToJsonNode());
			}
			jsonObject.Add("Entries", jsonArray);
			return jsonObject;
		}

		public void FromJson(JsonNode node)
		{
			if (!node.HasKey("Entries"))
			{
				Entries = Array.Empty<UpdaterDefinitionEntry>();
				return;
			}
			node = node["Entries"];
			if (node.IsArray)
			{
				JsonArray asArray = node.AsArray;
				Entries = new UpdaterDefinitionEntry[asArray.Count];
				for (int i = 0; i < asArray.Count; i++)
				{
					JsonNode node2 = asArray[i];
					UpdaterDefinitionEntry updaterDefinitionEntry = new UpdaterDefinitionEntry();
					updaterDefinitionEntry.FromJson(node2);
					Entries[i] = updaterDefinitionEntry;
				}
			}
			else
			{
				Entries = Array.Empty<UpdaterDefinitionEntry>();
			}
		}
	}
}
