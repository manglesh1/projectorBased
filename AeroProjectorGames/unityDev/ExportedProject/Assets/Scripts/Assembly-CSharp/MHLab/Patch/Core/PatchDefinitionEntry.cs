using System;
using System.IO;
using System.Text;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core
{
	[Serializable]
	public class PatchDefinitionEntry : IJsonSerializable
	{
		public PatchOperation Operation;

		public string RelativePath;

		public FileAttributes Attributes;

		public DateTime LastWriting;

		public long Size;

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
			jsonObject.Add("Operation", new JsonNumber((double)Operation));
			jsonObject.Add("RelativePath", new JsonString(RelativePath));
			jsonObject.Add("Attributes", new JsonNumber((double)Attributes));
			jsonObject.Add("LastWriting", new JsonString(LastWriting.ToString("u")));
			jsonObject.Add("Size", new JsonNumber(Size));
			return jsonObject;
		}

		public void FromJson(JsonNode node)
		{
			if (node.HasKey("Operation"))
			{
				Operation = (PatchOperation)node["Operation"].AsInt;
			}
			if (node.HasKey("RelativePath"))
			{
				RelativePath = node["RelativePath"].Value;
			}
			if (node.HasKey("Attributes"))
			{
				Attributes = (FileAttributes)node["Attributes"].AsInt;
			}
			if (node.HasKey("LastWriting"))
			{
				LastWriting = DateTime.Parse(node["LastWriting"].Value);
			}
			if (node.HasKey("Size"))
			{
				Size = node["Size"].AsLong;
			}
		}
	}
}
