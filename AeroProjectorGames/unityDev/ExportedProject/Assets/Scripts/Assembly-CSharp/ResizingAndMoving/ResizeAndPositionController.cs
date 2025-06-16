using System;
using System.Collections.Generic;
using System.Linq;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace ResizingAndMoving
{
	public class ResizeAndPositionController : MonoBehaviour
	{
		private const int MAIN_MENU_INDEX = 0;

		private const int SCOREBOARD_DROPDOWN_INDEX = 3;

		private const string DETECTION_SETUP_KEY = "Detection Setup";

		private const string EDIT_POSITIONS_KEY = "Position & Size";

		private const string GLOBAL_BACKGROUND = "Global Background";

		private const string GAMEBOARD_KEY = "Gameboard";

		private const string LOGO_KEY = "Logo";

		private const string MAIN_MENU_KEY = "Main Menu";

		private const string SAFETY_VIDEO_KEY = "Safety Video";

		private const string SCOREBOARD_KEY = "Scoreboard";

		private const string TIMER_KEY = "Timer";

		private ColorBlock _defaultColor;

		private ColorBlock _highlightColor;

		private Dictionary<string, SizeAndPositionStateSO> _sizeAndPositionStates;

		private SizeAndPositionStateSO _selectedSizeAndPositionState;

		[SerializeField]
		private GameObject fontSettings;

		[SerializeField]
		private GameObject positionButtons;

		[SerializeField]
		private GameObject sizeButtons;

		[Header("Buttons")]
		[SerializeField]
		private List<Button> _selectionButtons;

		[Header("Size Toggle")]
		[SerializeField]
		private Toggle largeIncrementToggle;

		[Header("Zoom Settings")]
		[SerializeField]
		private Camera mainCamera;

		[Header("State Components")]
		[SerializeField]
		private SizeAndPositionStateSO detectionSetupSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO editPositionsSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO globalBackgroundSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO gameboardSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO logoSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO mainMenuSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO safetyVideoSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO scoreboardSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO timerSizeAndPositionState;

		private void Awake()
		{
			_defaultColor = _selectionButtons.First().colors;
			_highlightColor = _defaultColor;
			_highlightColor.normalColor = _defaultColor.pressedColor;
			_highlightColor.selectedColor = _defaultColor.pressedColor;
		}

		private void OnEnable()
		{
			ResetZoom();
			fontSettings.SetActive(value: false);
			_selectionButtons.ForEach(delegate(Button btn)
			{
				btn.onClick.AddListener(delegate
				{
					SelectComponent(btn);
				});
			});
			_sizeAndPositionStates = new Dictionary<string, SizeAndPositionStateSO>
			{
				{ "Detection Setup", detectionSetupSizeAndPositionState },
				{ "Position & Size", editPositionsSizeAndPositionState },
				{ "Global Background", globalBackgroundSizeAndPositionState },
				{ "Gameboard", gameboardSizeAndPositionState },
				{ "Logo", logoSizeAndPositionState },
				{ "Main Menu", mainMenuSizeAndPositionState },
				{ "Safety Video", safetyVideoSizeAndPositionState },
				{ "Scoreboard", scoreboardSizeAndPositionState },
				{ "Timer", timerSizeAndPositionState }
			};
			SelectComponent(_selectionButtons[0]);
		}

		private void OnDisable()
		{
			_selectionButtons.ForEach(delegate(Button btn)
			{
				btn.onClick.RemoveAllListeners();
			});
			foreach (KeyValuePair<string, SizeAndPositionStateSO> sizeAndPositionState in _sizeAndPositionStates)
			{
				largeIncrementToggle.isOn = false;
				sizeAndPositionState.Value.SetLargeIncrements(large: false);
				sizeAndPositionState.Value.RaiseDoneEditing();
			}
		}

		public void ResetAll()
		{
			foreach (KeyValuePair<string, SizeAndPositionStateSO> sizeAndPositionState in _sizeAndPositionStates)
			{
				sizeAndPositionState.Value.RaiseReset();
			}
			ResetZoom();
		}

		public void DecrementHeight()
		{
			_selectedSizeAndPositionState.DecrementHeight();
		}

		public void IncrementHeight()
		{
			_selectedSizeAndPositionState.IncrementHeight();
		}

		public void DecrementWidth()
		{
			_selectedSizeAndPositionState.DecrementWidth();
		}

		public void IncrementWidth()
		{
			_selectedSizeAndPositionState.IncrementWidth();
		}

		public void DecrementSize()
		{
			_selectedSizeAndPositionState.Decrement();
		}

		public void IncrementSize()
		{
			_selectedSizeAndPositionState.Increment();
		}

		public void MoveDown()
		{
			_selectedSizeAndPositionState.MoveDown();
		}

		public void MoveUp()
		{
			_selectedSizeAndPositionState.MoveUp();
		}

		public void MoveLeft()
		{
			_selectedSizeAndPositionState.MoveLeft();
		}

		public void MoveRight()
		{
			_selectedSizeAndPositionState.MoveRight();
		}

		public void ResetComponent()
		{
			_selectedSizeAndPositionState.RaiseReset();
		}

		public void ToggleMovementSize()
		{
			foreach (KeyValuePair<string, SizeAndPositionStateSO> sizeAndPositionState in _sizeAndPositionStates)
			{
				sizeAndPositionState.Value.SetLargeIncrements(largeIncrementToggle.isOn);
			}
		}

		private void ResetZoom()
		{
			Zoom(delegate
			{
				mainCamera.orthographicSize = 5f;
			});
		}

		private void SelectComponent(Button selectedButton)
		{
			if (selectedButton.name == "Scoreboard")
			{
				fontSettings.SetActive(value: true);
			}
			else
			{
				fontSettings.SetActive(value: false);
			}
			ShowComponentControls();
			if (_selectedSizeAndPositionState != null)
			{
				_selectedSizeAndPositionState.RaiseDoneEditing();
			}
			_selectedSizeAndPositionState = _sizeAndPositionStates[selectedButton.name];
			_selectedSizeAndPositionState.RaiseEditing();
			_selectionButtons.ForEach(delegate(Button btn)
			{
				btn.colors = _defaultColor;
			});
			selectedButton.colors = _highlightColor;
		}

		private void ShowComponentControls()
		{
			positionButtons.SetActive(value: true);
			sizeButtons.SetActive(value: true);
		}

		private void Zoom(Action zoomAction)
		{
			zoomAction?.Invoke();
			SettingsStore.GlobalViewSettings.OrthographicSize = mainCamera.orthographicSize;
		}
	}
}
