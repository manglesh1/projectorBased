using UnityEngine;
using UnityEngine.Events;

namespace Games.SecondCameraGameboard
{
	[CreateAssetMenu(menuName = "Games/Second Gameboard Events")]
	public class SecondCameraGameboardEventsSO : ScriptableObject
	{
		public event UnityAction<GameObject> OnLoadGameObjectRequest;

		public event UnityAction OnUnloadRequest;

		public void RaiseLoadGameObjectRequest(GameObject go)
		{
			this.OnLoadGameObjectRequest?.Invoke(go);
		}

		public void RaiseUnloadRequest()
		{
			this.OnUnloadRequest?.Invoke();
		}
	}
}
