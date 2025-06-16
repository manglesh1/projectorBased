using Games.Hit_Custom_Photo.Scriptable_Objects;
using Settings;
using UI.MultiDisplay;
using UnityEngine;

namespace Games.HitCustomPhotoController.Scripts
{
	public class HitCustomPhotoMultiDisplayScoringController : MonoBehaviour
	{
		private GameObject _loadedGroup;

		[Header("Game Objects")]
		[SerializeField]
		private GameObject checkGameSessionGroup;

		[SerializeField]
		private GameObject getUserImagesGroup;

		[SerializeField]
		private GameObject speedSelectionGroup;

		[Header("Multi-Display Scoring Events")]
		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayScoringEvents;

		[Header("Multi-Display Game Events")]
		[SerializeField]
		private HitCustomPhotoMultiDisplayEventsSO gameMultiDisplayEvents;

		private void Awake()
		{
			gameMultiDisplayEvents.OnMultiDisplayStateChangeRequest += HandleMultiDisplayStateChange;
		}

		private void OnDisable()
		{
			gameMultiDisplayEvents.OnMultiDisplayStateChangeRequest -= HandleMultiDisplayStateChange;
			DestroyGroup();
		}

		private void DestroyGroup()
		{
			if (_loadedGroup != null)
			{
				Object.DestroyImmediate(_loadedGroup);
			}
		}

		private void HandleMultiDisplayStateChange(HitCustomPhotoGameStates state)
		{
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				DestroyGroup();
				switch (state)
				{
				case HitCustomPhotoGameStates.CheckExistingSession:
					_loadedGroup = Object.Instantiate(checkGameSessionGroup);
					_loadedGroup.GetComponent<ContinueSessionMenu>().SetMultiDisplay();
					break;
				case HitCustomPhotoGameStates.SpeedSelection:
					_loadedGroup = Object.Instantiate(speedSelectionGroup);
					_loadedGroup.GetComponent<SpeedSelection>().SetMultiDisplay();
					break;
				case HitCustomPhotoGameStates.WaitingOnUserImages:
					_loadedGroup = Object.Instantiate(getUserImagesGroup);
					break;
				}
				if (_loadedGroup != null)
				{
					_loadedGroup.SetActive(value: true);
					multiDisplayScoringEvents.RaiseLoadScoringObject(_loadedGroup);
				}
			}
		}
	}
}
