using UnityEngine;

namespace Extensions
{
	public static class GameObjectExtensions
	{
		private const int HIDDEN_COORDS = 9999;

		private const int HIDDEN_VALUE_CHECK = -5000;

		public static void Hide(this GameObject go)
		{
			Vector3 position = new Vector3(go.transform.position.x - 9999f, go.transform.position.y, go.transform.position.z);
			go.transform.position = position;
		}

		public static bool IsHidden(this GameObject go)
		{
			return go.transform.position.x < -5000f;
		}

		public static void Show(this GameObject go)
		{
			Vector3 position = new Vector3(go.transform.position.x + 9999f, go.transform.position.y, go.transform.position.z);
			go.transform.position = position;
		}
	}
}
