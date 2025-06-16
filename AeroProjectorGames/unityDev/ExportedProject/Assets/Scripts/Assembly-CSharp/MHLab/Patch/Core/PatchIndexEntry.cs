using System;
using System.Text;
using MHLab.Patch.Core.Serializing;
using MHLab.Patch.Core.Versioning;

namespace MHLab.Patch.Core
{
	[Serializable]
	public class PatchIndexEntry : IJsonSerializable
	{
		public IVersion From;

		public IVersion To;

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
		}
	}
}
