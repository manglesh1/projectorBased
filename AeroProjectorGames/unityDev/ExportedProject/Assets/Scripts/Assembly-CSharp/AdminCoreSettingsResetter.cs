using UnityEngine;

public class AdminCoreSettingsResetter : MonoBehaviour
{
	[SerializeField]
	private AdminCoreSettingsController adminCoreSettingsController;

	private void OnEnable()
	{
		adminCoreSettingsController.ResetCoreSettings();
	}
}
