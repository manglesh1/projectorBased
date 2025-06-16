using Games.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class GameSelectionButton : MonoBehaviour
	{
		[Header("Game Components")]
		[SerializeField]
		private TMP_Text gameNameText;

		[SerializeField]
		private Image gameImage;

		[Header("Free Trial Components")]
		[SerializeField]
		private GameObject weeklyFreeTrial;

		public void SetupGameComponents(GameSO game)
		{
			gameNameText.text = game.GameName;
			gameImage.sprite = game.GameIcon;
		}
	}
}
