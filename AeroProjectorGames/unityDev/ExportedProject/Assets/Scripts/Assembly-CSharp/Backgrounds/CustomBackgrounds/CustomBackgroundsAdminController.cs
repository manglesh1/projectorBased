using UnityEngine;
using UnityEngine.UI;

namespace Backgrounds.CustomBackgrounds
{
	public class CustomBackgroundsAdminController : MonoBehaviour
	{
		[SerializeField]
		private Button closeButton;

		[SerializeField]
		private BackgroundEventsSO backgroundEvents;

		private void OnDisable()
		{
			backgroundEvents.OnCustomBackgroundsUpdated -= Close;
		}

		private void OnEnable()
		{
			GetBackgrounds();
			backgroundEvents.OnCustomBackgroundsUpdated += Close;
		}

		private void GetBackgrounds()
		{
			backgroundEvents.RaiseUpdateCustomBackgroundsRequest();
		}

		private void Close()
		{
			closeButton.onClick.Invoke();
		}
	}
}
