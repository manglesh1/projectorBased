using UnityEngine;

namespace UI
{
	public class ToggleGameObject : MonoBehaviour
	{
		[SerializeField]
		private GameObject targetObject;

		public void Toggle()
		{
			targetObject.SetActive(!targetObject.activeSelf);
		}
	}
}
