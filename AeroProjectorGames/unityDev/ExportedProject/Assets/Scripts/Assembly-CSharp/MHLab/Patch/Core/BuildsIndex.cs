using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHLab.Patch.Core.Serializing;
using MHLab.Patch.Core.Versioning;

namespace MHLab.Patch.Core
{
	[Serializable]
	public class BuildsIndex : IJsonSerializable
	{
		public List<IVersion> AvailableBuilds;

		public IVersion GetLast()
		{
			if (AvailableBuilds == null || AvailableBuilds.Count == 0)
			{
				return null;
			}
			return AvailableBuilds.Last();
		}

		public IVersion GetFirst()
		{
			if (AvailableBuilds == null || AvailableBuilds.Count == 0)
			{
				return null;
			}
			return AvailableBuilds.First();
		}

		public bool Contains(IVersion version)
		{
			for (int i = 0; i < AvailableBuilds.Count; i++)
			{
				if (AvailableBuilds[i].Equals(version))
				{
					return true;
				}
			}
			return false;
		}

		public JsonNode ToJsonNode()
		{
			JsonObject jsonObject = new JsonObject();
			JsonArray jsonArray = new JsonArray();
			foreach (IVersion availableBuild in AvailableBuilds)
			{
				jsonArray.Add(new JsonString(availableBuild.ToString()));
			}
			jsonObject.Add("AvailableBuilds", jsonArray);
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
			AvailableBuilds = new List<IVersion>();
			JsonNode jsonNode = node["AvailableBuilds"];
			if (!(jsonNode != null) || !jsonNode.IsArray)
			{
				return;
			}
			foreach (JsonNode child in jsonNode.AsArray.Children)
			{
				if (child != null && child.IsString)
				{
					try
					{
						MHLab.Patch.Core.Versioning.Version item = new MHLab.Patch.Core.Versioning.Version(child.Value);
						AvailableBuilds.Add(item);
					}
					catch
					{
					}
				}
			}
		}
	}
}
