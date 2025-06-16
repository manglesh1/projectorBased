using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Games
{
	public class GameToggleController : MonoBehaviour
	{
		private int _gameId;

		private bool _isOn;

		[Header("Display")]
		[SerializeField]
		private TMP_Text gameTitle;

		[SerializeField]
		private Image gameIcon;

		[Header("Toggle Button")]
		[SerializeField]
		private Button gameToggleButton;

		[Header("Toggle Images")]
		[SerializeField]
		private Image toggleOnImage;

		[SerializeField]
		private Image toggleOffImage;

		public bool IsOn => _isOn;

		public int GameId => _gameId;

		public event UnityAction<int, bool> OnToggleChanged;

		private void OnDestroy()
		{
			gameToggleButton.onClick.RemoveListener(ToggleGameSelection);
		}

		private void OnDisable()
		{
			gameToggleButton.onClick.RemoveListener(ToggleGameSelection);
		}

		private void OnEnable()
		{
			gameToggleButton.onClick.AddListener(ToggleGameSelection);
		}

		public void Setup(string title, Sprite sprite, int gameId, bool isOn)
		{
			_isOn = isOn;
			_gameId = gameId;
			gameTitle.text = title;
			gameIcon.sprite = sprite;
			ShowToggleImage();
		}

		public void ToggleGameSelection()
		{
			_isOn = !_isOn;
			ShowToggleImage();
			RaiseToggleChanged();
		}

		public void ToggleGameSelectionWithoutNotify()
		{
			_isOn = !_isOn;
			ShowToggleImage();
		}

		private void RaiseToggleChanged()
		{
			this.OnToggleChanged?.Invoke(_gameId, _isOn);
		}

		private void ShowToggleImage()
		{
			toggleOnImage.gameObject.SetActive(_isOn);
			toggleOffImage.gameObject.SetActive(!_isOn);
		}
	}
}
