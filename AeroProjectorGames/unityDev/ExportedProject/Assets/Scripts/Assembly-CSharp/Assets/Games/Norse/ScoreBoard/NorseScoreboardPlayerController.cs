using Assets.Games.Norse.Scripts;
using Games.GameState;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Games.Norse.ScoreBoard
{
	public class NorseScoreboardPlayerController : MonoBehaviour
	{
		[Header("UI Elements")]
		[SerializeField]
		private Image backgroundImage;

		[SerializeField]
		private GameObject imageLeader;

		[SerializeField]
		private GameObject imageShield;

		[SerializeField]
		private TMP_Text playerMisses;

		[SerializeField]
		private TMP_Text playerName;

		[SerializeField]
		private GameObject strikeThrough;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private NorseStateSO norseState;

		private bool hasProtect;

		private bool isLeader;

		private int missCount;

		private string player;

		private readonly Color COLOR_ACTIVE = new Color(0.1254902f, 12f / 85f, 0.22745098f);

		private readonly Color COLOR_DEFAULT = new Color(0f, 0f, 0f);

		private readonly Color COLOR_DECEASED = new Color(0.61960787f, 16f / 51f, 0.23137255f);

		private readonly Color COLOR_DECEASED_TEXT = new Color(76f / 85f, 67f / 85f, 0.75686276f);

		public void SetLeader(bool leader)
		{
			isLeader = leader;
		}

		public void SetMissCount(int count)
		{
			missCount = count;
		}

		public void SetPlayerName(string pName)
		{
			player = pName;
			playerName.text = player;
		}

		public void SetProtected(bool isProtected)
		{
			hasProtect = isProtected;
		}

		public void SetVisible(bool visible)
		{
			base.gameObject.SetActive(visible);
		}

		public void UpdateUI()
		{
			imageLeader.SetActive(isLeader);
			imageShield.SetActive(hasProtect);
			if (missCount > norseState.GameWord.Length)
			{
				missCount = norseState.GameWord.Length;
			}
			playerMisses.text = norseState.GameWord.Substring(0, missCount);
			bool flag = missCount >= norseState.GameWord.Length;
			if (flag)
			{
				backgroundImage.color = COLOR_DECEASED;
			}
			else if (gameState.CurrentPlayer.Equals(player ?? string.Empty))
			{
				backgroundImage.color = COLOR_ACTIVE;
			}
			else if (isLeader)
			{
				backgroundImage.color = COLOR_DEFAULT;
			}
			else
			{
				backgroundImage.color = COLOR_DEFAULT;
			}
			if (flag)
			{
				playerName.color = COLOR_DECEASED_TEXT;
				playerMisses.color = COLOR_DECEASED_TEXT;
			}
			else
			{
				playerName.color = Color.white;
				playerMisses.color = Color.white;
			}
			strikeThrough.SetActive(flag);
		}

		private void Awake()
		{
			backgroundImage.color = COLOR_DEFAULT;
		}
	}
}
