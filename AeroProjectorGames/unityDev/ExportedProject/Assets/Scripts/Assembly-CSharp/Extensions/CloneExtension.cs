using Newtonsoft.Json;

namespace Extensions
{
	public static class CloneExtension
	{
		public static T SimpleJsonClone<T>(this T objectToClone)
		{
			string value = JsonConvert.SerializeObject(objectToClone);
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				ObjectCreationHandling = ObjectCreationHandling.Replace
			};
			return JsonConvert.DeserializeObject<T>(value, settings);
		}
	}
}
