using Games.Concentration.Scoreboard;
using Games.GameState;
using Players;
using TMPro;
using UnityEngine;

namespace Games.Concentration.ScoringLogic
{
	public class ConcentrationFrameScoredController : MonoBehaviour
	{
		private const string TOKEN_REFERNCE_OBJECT = "Concentration Tokens";

		private bool _isWinner;

		private PlayerData _player;

		private GameObject _tokenReferenceMasterObject;

		[Header("Scriptable Objects")]
		[SerializeField]
		private ConcentrationScoreboardEventsSO concentrationScoreboardEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		[Header("UI Elements")]
		[SerializeField]
		private GameObject tokenPlacementContainer;

		[SerializeField]
		private GameObject playerHightlight;

		[SerializeField]
		private int playerIndex;

		[SerializeField]
		private TextMeshProUGUI playerName;

		[SerializeField]
		private TextMeshProUGUI totalScore;

		private void OnEnable()
		{
			_player = playerState.players[playerIndex];
			playerName.text = _player.PlayerName;
			bool active = _player.PlayerName == playerState.players[0].PlayerName;
			playerHightlight.SetActive(active);
			_tokenReferenceMasterObject = GameObject.Find("Concentration Tokens");
			AtEnableAddListeners();
		}

		private void OnDestroy()
		{
			AtDisableRemoveListeners();
		}

		private void AtEnableAddListeners()
		{
			concentrationScoreboardEvents.OnUpdateScoreboardRemovingStolenTokenFromPlayer += HandleUpdateScoreboardAndRemoveToken;
			concentrationScoreboardEvents.OnUpdateScoreboardWithStandardScore += HandleUpdateScoreboardWithStandardScore;
			concentrationScoreboardEvents.OnUpdateScoreboardWithStealScore += HandleUpdateScoreboardWithStealScore;
			gameEvents.OnUpdatePlayerTurn += HandleUpdatePlayerHighlight;
			gameEvents.OnCustomAfterUndo += HandleUndo;
		}

		private void AtDisableRemoveListeners()
		{
			concentrationScoreboardEvents.OnUpdateScoreboardRemovingStolenTokenFromPlayer -= HandleUpdateScoreboardAndRemoveToken;
			concentrationScoreboardEvents.OnUpdateScoreboardWithStandardScore -= HandleUpdateScoreboardWithStandardScore;
			concentrationScoreboardEvents.OnUpdateScoreboardWithStealScore -= HandleUpdateScoreboardWithStealScore;
			gameEvents.OnUpdatePlayerTurn -= HandleUpdatePlayerHighlight;
			gameEvents.OnCustomAfterUndo -= HandleUndo;
		}

		private void AddToken(GameObject gameToken)
		{
			RectTransform component = tokenPlacementContainer.GetComponent<RectTransform>();
			GameObject gameObject = Object.Instantiate(gameToken, component);
			gameObject.SetActive(value: true);
			gameObject.GetComponent<Animator>().enabled = false;
			gameObject.transform.rotation = Quaternion.Euler(new Vector3(-90f, 180f, 0f));
			float height = component.rect.height;
			float width = component.rect.width;
			float num = gameObject.gameObject.GetComponent<RectTransform>().localScale.y - height;
			float num2 = ((num >= 0f) ? num : (num * -1f));
			Vector3 localScale = new Vector3(2000f, 2000f, 2000f);
			gameObject.gameObject.GetComponent<RectTransform>().localScale = localScale;
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 10f);
			Vector3 vector = gameObject.GetComponent<RectTransform>().sizeDelta;
			vector.y *= 0.5f;
			gameObject.GetComponent<RectTransform>().sizeDelta = vector;
		}

		private void AddStolenToken(GameObject stolenGameToken)
		{
			RectTransform component = tokenPlacementContainer.GetComponent<RectTransform>();
			stolenGameToken.transform.SetParent(component);
		}

		private void DestroyAllGameTokens()
		{
			for (int num = tokenPlacementContainer.transform.GetComponentInChildren<Transform>().childCount; num > 0; num--)
			{
				Object.DestroyImmediate(tokenPlacementContainer.transform.GetComponentInChildren<Transform>().GetChild(num - 1).gameObject);
			}
		}

		private GameObject GetCardToRemove()
		{
			int childCount = tokenPlacementContainer.transform.childCount;
			return tokenPlacementContainer.transform.GetChild(childCount - 1).gameObject;
		}

		private void HandleUndo()
		{
			DestroyAllGameTokens();
			if (_tokenReferenceMasterObject == null)
			{
				_tokenReferenceMasterObject = GameObject.Find("Concentration Tokens");
			}
			foreach (ScoreToken item in gameState.InfiniteScoredGameScores[_player.PlayerName])
			{
				if (!string.IsNullOrEmpty(item.TokenName))
				{
					item.Token = _tokenReferenceMasterObject.transform.Find(item.TokenName).gameObject;
					AddToken(item.Token);
				}
			}
			totalScore.text = gameState.TotalScores[_player.PlayerName].ToString();
			HandleUpdatePlayerHighlight();
		}

		private void HandleUpdatePlayerHighlight()
		{
			bool active = gameState.CurrentPlayer == _player.PlayerName;
			if (gameState.GameStatus != GameStatus.Finished)
			{
				playerHightlight.SetActive(active);
			}
			else
			{
				playerHightlight.SetActive(_isWinner);
			}
		}

		private void HandleUpdateScoreboardAndRemoveToken(string playerToStealFrom, int tokenScoreValue)
		{
			if (_player.PlayerName == playerToStealFrom)
			{
				totalScore.text = gameState.TotalScores[_player.PlayerName].ToString();
				GameObject cardToRemove = GetCardToRemove();
				concentrationScoreboardEvents.RaiseRecordScoreWithStolenToken(cardToRemove, tokenScoreValue);
			}
		}

		private void HandleUpdateScoreboardWithStandardScore(string forPlayerName, GameObject gameToken)
		{
			if (_player.PlayerName == forPlayerName)
			{
				if (gameToken != null)
				{
					AddToken(gameToken);
				}
				totalScore.text = gameState.TotalScores[_player.PlayerName].ToString();
			}
		}

		private void HandleUpdateScoreboardWithStealScore(string forPlayerName, GameObject stolenGameToken)
		{
			if (_player.PlayerName == forPlayerName)
			{
				if (stolenGameToken != null)
				{
					AddStolenToken(stolenGameToken);
				}
				totalScore.text = gameState.TotalScores[_player.PlayerName].ToString();
			}
		}
	}
}
