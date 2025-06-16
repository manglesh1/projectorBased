namespace MHLab.Patch.Core.Serializing
{
	public interface IJsonSerializable
	{
		string ToJson();

		JsonNode ToJsonNode();

		void FromJson(JsonNode node);
	}
}
