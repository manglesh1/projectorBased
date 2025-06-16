using System;
using System.Text;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core.Client
{
	[Serializable]
	public class SettingsOverride : IJsonSerializable
	{
		public bool DebugMode { get; set; }

		public bool PatcherUpdaterSafeMode { get; set; }

		public string RemoteUrl { get; set; }

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
			jsonObject.Add("DebugMode", new JsonBool(DebugMode));
			jsonObject.Add("PatcherUpdaterSafeMode", new JsonBool(PatcherUpdaterSafeMode));
			return jsonObject;
		}

		public void FromJson(JsonNode node)
		{
			if (node.HasKey("DebugMode"))
			{
				DebugMode = node["DebugMode"].AsBool;
			}
			if (node.HasKey("PatcherUpdaterSafeMode"))
			{
				PatcherUpdaterSafeMode = node["PatcherUpdaterSafeMode"].AsBool;
			}
			if (node.HasKey("RemoteUrl"))
			{
				RemoteUrl = node["RemoteUrl"].Value;
			}
		}
	}
}
