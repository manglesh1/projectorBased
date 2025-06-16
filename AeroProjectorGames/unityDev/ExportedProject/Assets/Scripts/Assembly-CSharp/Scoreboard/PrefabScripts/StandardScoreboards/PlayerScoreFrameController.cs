using System.Collections.Generic;
using Games;
using Games.GameState;
using Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scoreboard.PrefabScripts.StandardScoreboards
{
	public class PlayerScoreFrameController : MonoBehaviour
	{
		private bool _editingFrame;

		private int _editingFrameIndex;

		private PlayerData _player;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private TextMeshProUGUI playerName;

		[SerializeField]
		private int playerIndex;

		[SerializeField]
		private GameObject playerHightlight;

		[SerializeField]
		private List<TextMeshProUGUI> frames = new List<TextMeshProUGUI>(10);

		[SerializeField]
		private TextMeshProUGUI totalScore;

		private void OnEnable()
		{
			_player = playerState.players[playerIndex];
			playerName.text = _player.PlayerName;
			bool flag = _player.PlayerName == playerState.players[0].PlayerName;
			playerHightlight.SetActive(flag);
			for (int i = 0; i < frames.Count; i++)
			{
				TextMeshProUGUI textMeshProUGUI = frames[i];
				int frameValue = i;
				textMeshProUGUI.transform.parent.GetComponent<Button>().onClick.AddListener(delegate
				{
					EditScore(frameValue);
				});
				textMeshProUGUI.transform.parent.GetChild(0).gameObject.SetActive(flag && i == gameState.CurrentRound - 1);
				textMeshProUGUI.text = " ";
			}
			gameEvents.OnUpdateScoreboard += UpdateScoreboard;
			gameEvents.OnCancelScoreEdit += CancelEditing;
		}

		private void OnDestroy()
		{
			frames.ForEach(delegate(TextMeshProUGUI frame)
			{
				frame.transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
			});
			gameEvents.OnUpdateScoreboard -= UpdateScoreboard;
			gameEvents.OnCancelScoreEdit -= CancelEditing;
		}

		private void CancelEditing()
		{
			if (_editingFrame)
			{
				GameObject gameObject = frames[_editingFrameIndex].transform.parent.GetChild(0).gameObject;
				gameObject.SetActive(value: false);
				gameObject.GetComponent<Image>().color = Color.blue;
				frames[_editingFrameIndex].color = Color.white;
				_editingFrame = false;
			}
		}

		private void EditScore(int frameIndex)
		{
			if (gameState.RoundScores[_player.PlayerName][frameIndex].HasValue && !gameState.IsTargetDisabled)
			{
				_editingFrame = true;
				_editingFrameIndex = frameIndex;
				GameObject gameObject = frames[frameIndex].transform.parent.GetChild(0).gameObject;
				gameObject.SetActive(value: true);
				gameObject.GetComponent<Image>().color = Color.yellow;
				frames[frameIndex].color = Color.black;
				gameEvents.RaiseBeginScoreEdit(_player.PlayerName, frameIndex);
			}
		}

		private void UpdateScoreboard()
		{
			CancelEditing();
			UpdateScoresAndFrameHighlight();
			UpdatePlayerTotal();
		}

		private void UpdateScoresAndFrameHighlight()
		{
			bool flag = gameState.CurrentPlayer == _player.PlayerName;
			if (gameState.GameStatus != GameStatus.Finished)
			{
				playerHightlight.SetActive(flag);
			}
			else
			{
				playerHightlight.SetActive(value: false);
			}
			for (int i = 0; i < frames.Count; i++)
			{
				TextMeshProUGUI textMeshProUGUI = frames[i];
				textMeshProUGUI.transform.parent.GetChild(0).gameObject.SetActive(flag && i == gameState.CurrentFrame);
				if (gameState.RoundScores.ContainsKey(_player.PlayerName))
				{
					int? num = 0;
					textMeshProUGUI.text = (gameState.RoundScores[_player.PlayerName][i].HasValue ? num.GetValueOrDefault().ToString() : null) ?? " ";
				}
				else
				{
					Debug.Log("Should never hit this");
				}
			}
		}

		private void UpdatePlayerTotal()
		{
			int num = 0;
			foreach (int? item in gameState.RoundScores[_player.PlayerName])
			{
				if (item.HasValue)
				{
					num += item.Value;
				}
			}
			totalScore.text = num.ToString();
		}
	}
}
