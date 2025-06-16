using UnityEngine;

namespace Platform
{
	public class WindowsOnlyController : MonoBehaviour
	{
		private void OnEnable()
		{
			if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
