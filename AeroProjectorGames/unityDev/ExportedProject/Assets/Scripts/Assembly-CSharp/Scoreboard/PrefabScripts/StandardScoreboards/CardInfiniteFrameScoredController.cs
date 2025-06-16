using System;
using System.Collections;
using System.Collections.Generic;
using Games;
using Games.GameState;
using Helpers;
using Players;
using TMPro;
using UnityEngine;

namespace Scoreboard.PrefabScripts.StandardScoreboards
{
	public class CardInfiniteFrameScoredController : MonoBehaviour
	{
		private const string TOKEN_REFERNCE_OBJECT = "ScoreboardTokenReference List";

		private AnimationHelper _animationHelper = new AnimationHelper();

		private bool _isWinner;

		private PlayerData _player;

		private GameObject _tokenReferenceMasterObject;

		private List<GameObject> _scoreBoardCardsList = new List<GameObject>();

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private GameObject cardPlacementContainer;

		[SerializeField]
		private ParticleSystem explosionPS;

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
			_tokenReferenceMasterObject = GameObject.Find("ScoreboardTokenReference List");
			gameEvents.OnGameOver += HandleUpdatePlayerHighlight;
			gameEvents.OnRemoveScoreboardPlayerToken += HandleRemovePlayerToken;
			gameEvents.OnResetPlayerScore += HandleResetPlayerScore;
			gameEvents.OnUpdatePlayersScoreboard += HandleUpdatePlayerScoreboard;
			gameEvents.OnUpdatePlayerTurn += HandleUpdatePlayerHighlight;
			gameEvents.OnUpdateScoreboard += HandleUpdateScoreboard;
			gameEvents.OnUpdateScoreboardWithCallback += HandleUpdateScoreboardWithCallback;
			gameEvents.OnUpdateTwoPlayersScores += HandleUpdateTwoPlayersScores;
			gameEvents.OnCustomAfterUndo += Undo;
		}

		private void OnDestroy()
		{
			gameEvents.OnGameOver -= HandleUpdatePlayerHighlight;
			gameEvents.OnRemoveScoreboardPlayerToken -= HandleRemovePlayerToken;
			gameEvents.OnResetPlayerScore -= HandleResetPlayerScore;
			gameEvents.OnUpdatePlayersScoreboard -= HandleUpdatePlayerScoreboard;
			gameEvents.OnUpdatePlayerTurn -= HandleUpdatePlayerHighlight;
			gameEvents.OnUpdateScoreboard -= HandleUpdateScoreboard;
			gameEvents.OnUpdateScoreboardWithCallback -= HandleUpdateScoreboardWithCallback;
			gameEvents.OnUpdateTwoPlayersScores -= HandleUpdateTwoPlayersScores;
			gameEvents.OnCustomAfterUndo -= Undo;
		}

		private void AddToHeldCards(GameObject hitCard, bool isUndo = false, Action callback = null)
		{
			if (!(cardPlacementContainer == null) && _scoreBoardCardsList.FindIndex((GameObject g) => g.name.Contains(hitCard.name)) == -1)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(hitCard, cardPlacementContainer.GetComponent<RectTransform>());
				gameObject.name = hitCard.name;
				gameObject.GetComponent<Keep3dModelSizeWithUIParent>().enabled = false;
				float height = cardPlacementContainer.GetComponent<RectTransform>().rect.height;
				float width = cardPlacementContainer.GetComponent<RectTransform>().rect.width;
				float num = gameObject.gameObject.GetComponent<RectTransform>().localScale.y - height;
				num = ((num >= 0f) ? num : (num * -1f));
				float num2 = gameObject.gameObject.GetComponent<RectTransform>().localScale.x - num;
				Vector3 vector = new Vector3(num2 - 10f, height - 10f, 0f);
				gameObject.gameObject.GetComponent<RectTransform>().localScale = vector;
				gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, -1f);
				vector.x *= 1.75f;
				gameObject.GetComponent<RectTransform>().sizeDelta = vector;
				gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 180f, 0f);
				if (!isUndo)
				{
					gameObject.GetComponent<MeshRenderer>().enabled = false;
					StartCoroutine(ScaleUpAndBackDownOnClick_ScoreBoard(gameObject, hitCard, callback));
				}
				_scoreBoardCardsList.Add(gameObject);
			}
		}

		private void DestroyToken(GameObject token, bool playAnimation = true)
		{
			if (playAnimation)
			{
				UnityEngine.Object.Instantiate(explosionPS).transform.position = token.transform.position;
			}
			UnityEngine.Object.DestroyImmediate(token);
		}

		private void HandleRemovePlayerToken(string removePlayerName, string removeTokenName)
		{
			if (_player.PlayerName == removePlayerName)
			{
				DestroyToken(_scoreBoardCardsList.Find((GameObject g) => g.name == removeTokenName));
				_scoreBoardCardsList.RemoveAll((GameObject g) => g == null);
			}
		}

		private void HandleResetPlayerScore(string resetPlayerName)
		{
			if (_player.PlayerName == resetPlayerName)
			{
				ResetTokens(_player.PlayerName);
				UpdatePlayerScore(isUndo: false);
			}
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

		private void HandleUpdatePlayerScoreboard(string forPlayerName)
		{
			if (_player.PlayerName == forPlayerName)
			{
				UpdatePlayerScore(isUndo: false);
			}
		}

		private void HandleUpdateScoreboard()
		{
			ResetTokens(_player.PlayerName, playAnimation: false);
			UpdatePlayerScore(isUndo: false);
		}

		private void HandleUpdateScoreboardWithCallback(string forPlayerName, Action callback)
		{
			if (gameState.CurrentPlayer == forPlayerName)
			{
				UpdatePlayerScore(isUndo: false, callback);
			}
		}

		private void HandleUpdateTwoPlayersScores(string restPlayerName, string updatePlayerName, string removeTokenName, int? updatePlayerScore)
		{
			HandleResetPlayerScore(restPlayerName);
		}

		private void ResetTokens(string resetPlayerName, bool playAnimation = true)
		{
			if (_player.PlayerName != resetPlayerName)
			{
				return;
			}
			foreach (GameObject scoreBoardCards in _scoreBoardCardsList)
			{
				DestroyToken(scoreBoardCards, playAnimation);
			}
			_scoreBoardCardsList.Clear();
		}

		private IEnumerator ScaleUpAndBackDownOnClick_ScoreBoard(GameObject playingCard, GameObject hitObject, Action callback = null)
		{
			yield return new WaitForEndOfFrame();
			if (!(playingCard == null))
			{
				playingCard.GetComponent<MeshRenderer>().enabled = true;
				RectTransform component = playingCard.GetComponent<RectTransform>();
				Vector3 localPosition = component.localPosition;
				Vector3 localScale = component.localScale;
				component.localPosition = new Vector3(hitObject.transform.localPosition.x, hitObject.transform.localPosition.y, -400f);
				component.localScale = new Vector3(400f, 400f, localScale.z);
				component.eulerAngles = new Vector3(0f, 180f, 250f);
				Vector3 endLocalPosition = localPosition;
				Vector3 endScale = localScale;
				Quaternion endRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
				float duration = 6f;
				float animationCulling = 5f;
				yield return StartCoroutine(_animationHelper.LerpLocalAnimation(component, endScale, endLocalPosition, endRotation, duration, animationCulling));
				callback?.Invoke();
			}
		}

		private void Undo()
		{
			ResetTokens(_player.PlayerName, playAnimation: false);
			UpdatePlayerScore(isUndo: true);
		}

		private void UpdatePlayerScore(bool isUndo, Action callback = null)
		{
			if (gameState.InfiniteScoredGameScores[_player.PlayerName].Count != 0)
			{
				foreach (ScoreToken item in gameState.InfiniteScoredGameScores[_player.PlayerName])
				{
					if (item.Token == null)
					{
						if (_tokenReferenceMasterObject == null)
						{
							_tokenReferenceMasterObject = GameObject.Find("ScoreboardTokenReference List");
						}
						item.Token = _tokenReferenceMasterObject.transform.Find(item.TokenName).gameObject;
						item.Token.SetActive(value: true);
					}
					AddToHeldCards(item.Token, isUndo, callback);
				}
			}
			totalScore.text = gameState.TotalScores[_player.PlayerName].ToString();
		}
	}
}
