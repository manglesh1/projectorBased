using System;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Utilities.Serializing
{
	public class JsonSerializer : ISerializer
	{
		public string Serialize<TObject>(TObject obj) where TObject : IJsonSerializable
		{
			return JsonUtility.ToJson(obj);
		}

		public TObject Deserialize<TObject>(string data) where TObject : IJsonSerializable
		{
			TObject val = Activator.CreateInstance<TObject>();
			JsonUtility.Parse(val, data);
			return val;
		}

		public TObject DeserializeOn<TObject>(TObject obj, string data) where TObject : IJsonSerializable
		{
			JsonUtility.Parse(obj, data);
			return obj;
		}
	}
}
