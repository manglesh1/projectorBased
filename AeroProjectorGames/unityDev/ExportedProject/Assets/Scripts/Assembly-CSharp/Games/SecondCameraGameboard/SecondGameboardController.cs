using UnityEngine;

namespace Games.SecondCameraGameboard
{
	public class SecondGameboardController : MonoBehaviour
	{
		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private SecondCameraGameboardEventsSO secondCameraGameboardEvents;

		private void OnDisable()
		{
			secondCameraGameboardEvents.OnLoadGameObjectRequest -= OnLoadGameObjectRequest;
			gameEvents.OnMainMenu -= CleanupChildren;
		}

		private void OnEnable()
		{
			secondCameraGameboardEvents.OnLoadGameObjectRequest += OnLoadGameObjectRequest;
			gameEvents.OnMainMenu += CleanupChildren;
		}

		private void CleanupChildren()
		{
			for (int i = 0; i < base.gameObject.transform.childCount; i++)
			{
				Object.Destroy(base.transform.GetChild(i).gameObject);
			}
		}

		private void OnLoadGameObjectRequest(GameObject go)
		{
			if (!(go == null))
			{
				CleanupChildren();
				Object.Instantiate(go, base.gameObject.transform, worldPositionStays: false);
			}
		}
	}
}
