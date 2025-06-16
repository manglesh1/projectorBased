using System;
using System.IO;
using System.Text;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core
{
	[Serializable]
	public class BuildDefinitionEntry : IJsonSerializable
	{
		public string RelativePath;

		public long Size;

		public DateTime LastWriting;

		public string Hash;

		public FileAttributes Attributes;

		public JsonNode ToJsonNode()
		{
			JsonObject jsonObject = new JsonObject();
			jsonObject.Add("RelativePath", new JsonString(RelativePath));
			jsonObject.Add("Size", new JsonNumber(Size));
			jsonObject.Add("LastWriting", new JsonString(LastWriting.ToString("u")));
			jsonObject.Add("Hash", new JsonString(Hash));
			jsonObject.Add("Attributes", new JsonNumber((double)Attributes));
			return jsonObject;
		}

		public string ToJson()
		{
			JsonNode jsonNode = ToJsonNode();
			StringBuilder stringBuilder = new StringBuilder();
			jsonNode.WriteToStringBuilder(stringBuilder, 2, 2, JsonTextMode.Indent);
			return stringBuilder.ToString();
		}

		public void FromJson(JsonNode node)
		{
			if (node.HasKey("RelativePath"))
			{
				RelativePath = node["RelativePath"].Value;
			}
			else
			{
				RelativePath = string.Empty;
			}
			if (node.HasKey("Size"))
			{
				Size = node["Size"].AsLong;
			}
			if (node.HasKey("LastWriting"))
			{
				LastWriting = DateTime.Parse(node["LastWriting"].Value);
			}
			else
			{
				LastWriting = DateTime.MinValue;
			}
			if (node.HasKey("Hash"))
			{
				Hash = node["Hash"].Value;
			}
			else
			{
				Hash = string.Empty;
			}
			if (node.HasKey("Attributes"))
			{
				Attributes = (FileAttributes)node["Attributes"].AsInt;
			}
		}
	}
}
