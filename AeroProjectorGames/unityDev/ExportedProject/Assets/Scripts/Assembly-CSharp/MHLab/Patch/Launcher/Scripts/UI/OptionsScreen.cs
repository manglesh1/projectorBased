using UnityEngine;

namespace MHLab.Patch.Launcher.Scripts.UI
{
	public sealed class OptionsScreen : MonoBehaviour
	{
		public void OpenOptions()
		{
			base.gameObject.SetActive(value: true);
			Debug.Log("Opened");
		}

		public void CloseOptions()
		{
			base.gameObject.SetActive(value: false);
			Debug.Log("Closed");
		}
	}
}
