using System;
using System.Collections.Generic;
using System.Text;
using MHLab.Patch.Core.Serializing;
using MHLab.Patch.Core.Versioning;

namespace MHLab.Patch.Core
{
	[Serializable]
	public class PatchDefinition : IJsonSerializable
	{
		public IVersion From;

		public IVersion To;

		public string Hash;

		public long TotalSize;

		public List<PatchDefinitionEntry> Entries;

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
			jsonObject.Add("From", new JsonString(From.ToString()));
			jsonObject.Add("To", new JsonString(To.ToString()));
			jsonObject.Add("Hash", new JsonString(Hash));
			jsonObject.Add("TotalSize", new JsonNumber(TotalSize));
			JsonArray jsonArray = new JsonArray();
			for (int i = 0; i < Entries.Count; i++)
			{
				PatchDefinitionEntry patchDefinitionEntry = Entries[i];
				jsonArray.Add(patchDefinitionEntry.ToJsonNode());
			}
			jsonObject.Add("Entries", jsonArray);
			return jsonObject;
		}

		public void FromJson(JsonNode node)
		{
			if (node.HasKey("From"))
			{
				From = new MHLab.Patch.Core.Versioning.Version(node["From"].Value);
			}
			if (node.HasKey("To"))
			{
				To = new MHLab.Patch.Core.Versioning.Version(node["To"].Value);
			}
			if (node.HasKey("Hash"))
			{
				Hash = node["Hash"].Value;
			}
			if (node.HasKey("TotalSize"))
			{
				TotalSize = node["TotalSize"].AsLong;
			}
			Entries = new List<PatchDefinitionEntry>();
			if (node.HasKey("Entries"))
			{
				JsonArray asArray = node["Entries"].AsArray;
				for (int i = 0; i < asArray.Count; i++)
				{
					JsonNode node2 = asArray[i];
					PatchDefinitionEntry patchDefinitionEntry = new PatchDefinitionEntry();
					patchDefinitionEntry.FromJson(node2);
					Entries.Add(patchDefinitionEntry);
				}
			}
		}
	}
}
