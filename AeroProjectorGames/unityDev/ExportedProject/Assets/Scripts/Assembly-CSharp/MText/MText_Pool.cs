using System.Collections.Generic;
using UnityEngine;

namespace MText
{
	[DisallowMultipleComponent]
	public class MText_Pool : MonoBehaviour
	{
		public static MText_Pool Instance;

		public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

		public GameObject GetPoolItem(MText_Font font, char c)
		{
			string key = font.name + " " + c;
			if (!poolDictionary.ContainsKey(key))
			{
				Queue<GameObject> value = new Queue<GameObject>();
				poolDictionary.Add(key, value);
				GameObject gameObject = new GameObject();
				Mesh mesh = font.RetrievePrefab(c);
				if ((bool)mesh)
				{
					gameObject.AddComponent<MeshFilter>();
					gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
					gameObject.SetActive(value: false);
					gameObject.transform.SetParent(base.transform);
					gameObject.AddComponent<MText_PoolItem>().key = key;
				}
				else
				{
					gameObject = new GameObject();
				}
				return gameObject;
			}
			if (poolDictionary[key].Count > 0)
			{
				return poolDictionary[key].Dequeue();
			}
			GameObject gameObject2 = new GameObject();
			Mesh mesh2 = font.RetrievePrefab(c);
			if ((bool)mesh2)
			{
				gameObject2.AddComponent<MeshFilter>();
				gameObject2.GetComponent<MeshFilter>().sharedMesh = mesh2;
				gameObject2.SetActive(value: false);
				gameObject2.transform.SetParent(base.transform);
				gameObject2.AddComponent<MText_PoolItem>().key = key;
				gameObject2.name = c.ToString();
			}
			else
			{
				gameObject2.name = "Space";
			}
			return gameObject2;
		}

		public void ReturnPoolItem(GameObject poolItem)
		{
			if ((bool)poolItem)
			{
				if (!poolItem.GetComponent<MText_PoolItem>())
				{
					Object.Destroy(poolItem);
				}
				else if (poolDictionary.ContainsKey(poolItem.GetComponent<MText_PoolItem>().key))
				{
					poolItem.SetActive(value: false);
					poolItem.transform.SetParent(base.transform);
					poolDictionary[poolItem.GetComponent<MText_PoolItem>().key].Enqueue(poolItem);
				}
				else
				{
					Object.Destroy(poolItem);
				}
			}
		}
	}
}
