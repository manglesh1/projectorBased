using System;
using System.Text;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core
{
	[Serializable]
	public class UpdaterSafeModeDefinition : IJsonSerializable
	{
		public string ArchiveName;

		public string ExecutableToRun;

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
			jsonObject.Add("ArchiveName", new JsonString(ArchiveName));
			jsonObject.Add("ExecutableToRun", new JsonString(ExecutableToRun));
			return jsonObject;
		}

		public void FromJson(JsonNode node)
		{
			if (node.HasKey("ArchiveName"))
			{
				ArchiveName = node["ArchiveName"].Value;
			}
			if (node.HasKey("ExecutableToRun"))
			{
				ExecutableToRun = node["ExecutableToRun"].Value;
			}
		}
	}
}
