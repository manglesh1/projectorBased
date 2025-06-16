namespace MHLab.Patch.Core.Serializing
{
	public static class JsonUtility
	{
		public static TObject Parse<TObject>(TObject obj, string json) where TObject : IJsonSerializable
		{
			obj.FromJson(JsonNode.Parse(json));
			return obj;
		}

		public static string ToJson<TObject>(TObject obj) where TObject : IJsonSerializable
		{
			return obj.ToJson();
		}
	}
}
