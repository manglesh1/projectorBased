using Detection.Models;
using Games.GameState;
using Games.HitCustomPhotoController.ScriptableObjects;
using Settings;
using UnityEngine;

namespace Games.HitCustomPhotoController.Scripts
{
	public class SpeedSelection : MonoBehaviour
	{
		private bool _multiDisplay;

		[SerializeField]
		private HitCustomPhotoEventsSO hitCustomPhotoEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		private void Start()
		{
			if (!SettingsStore.DetectionSettings.DetectionEnabled || SettingsStore.DetectionSettings.DetectedCamera == DetectedCameraEnum.None)
			{
				SelectedStationary();
			}
		}

		private void RemoveFromGameFlexSpace()
		{
			if (!_multiDisplay)
			{
				gameEvents.RaiseRemoveObjectFromGameFlexSpace(base.gameObject);
			}
		}

		public void SelectedEasy()
		{
			gameState.GameType = "Easy";
			hitCustomPhotoEvents.RaiseChangeGameState(HitCustomPhotoGameStates.Playing);
			RemoveFromGameFlexSpace();
		}

		public void SelectedMedium()
		{
			gameState.GameType = "Medium";
			hitCustomPhotoEvents.RaiseChangeGameState(HitCustomPhotoGameStates.Playing);
			RemoveFromGameFlexSpace();
		}

		public void SelectedHard()
		{
			gameState.GameType = "Hard";
			hitCustomPhotoEvents.RaiseChangeGameState(HitCustomPhotoGameStates.Playing);
			RemoveFromGameFlexSpace();
		}

		public void SelectedStationary()
		{
			gameState.GameType = "Stationary";
			hitCustomPhotoEvents.RaiseChangeGameState(HitCustomPhotoGameStates.Playing);
			RemoveFromGameFlexSpace();
		}

		public void SetMultiDisplay()
		{
			_multiDisplay = true;
		}
	}
}
