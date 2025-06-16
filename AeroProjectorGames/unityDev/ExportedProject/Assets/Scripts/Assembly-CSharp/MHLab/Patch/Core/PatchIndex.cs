using System;
using System.Collections.Generic;
using System.Text;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core
{
	[Serializable]
	public class PatchIndex : IJsonSerializable
	{
		public List<PatchIndexEntry> Patches;

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
			foreach (PatchIndexEntry patch in Patches)
			{
				jsonArray.Add(patch.ToJsonNode());
			}
			jsonObject.Add("Patches", jsonArray);
			return jsonObject;
		}

		public void FromJson(JsonNode node)
		{
			if (node.HasKey("Patches"))
			{
				JsonArray asArray = node["Patches"].AsArray;
				Patches = new List<PatchIndexEntry>(asArray.Count);
				for (int i = 0; i < asArray.Count; i++)
				{
					JsonNode node2 = asArray[i];
					PatchIndexEntry patchIndexEntry = new PatchIndexEntry();
					patchIndexEntry.FromJson(node2);
					Patches.Add(patchIndexEntry);
				}
			}
			else
			{
				Patches = new List<PatchIndexEntry>();
			}
		}
	}
}
