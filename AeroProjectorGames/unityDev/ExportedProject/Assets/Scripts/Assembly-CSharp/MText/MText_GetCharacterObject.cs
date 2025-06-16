using UnityEngine;

namespace MText
{
	public class MText_GetCharacterObject
	{
		public static GameObject GetObject(char c, Modular3DText text)
		{
			GameObject gameObject = null;
			if (text.pooling && (bool)text.pool)
			{
				gameObject = text.pool.GetPoolItem(text.Font, c);
			}
			else
			{
				Mesh mesh = text.Font.RetrievePrefab(c);
				if ((bool)mesh)
				{
					gameObject = new GameObject();
					gameObject.AddComponent<MeshFilter>();
					gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
					gameObject.name = c.ToString();
				}
			}
			if (gameObject == null)
			{
				gameObject = new GameObject
				{
					name = "space"
				};
			}
			gameObject.SetActive(value: true);
			return gameObject;
		}
	}
}
