using Games.GameState;
using Games.SharedScoringLogic.Standard;
using Players;
using Scoreboard;
using Scoreboard.PrefabScripts;
using Settings;
using UnityEngine;

namespace Games.Axteroids
{
	public class AxteroidsGame : MonoBehaviour
	{
		private const int BOMB_FRAMES_1 = 1;

		private const int BOMB_FRAMES_2 = 3;

		private const int BOMB_FRAMES_3 = 5;

		private const int BOMB_FRAMES_4 = 7;

		private const int BOMB_FRAMES_5 = 9;

		private bool _killZones;

		private ScoreboardController _scoreboardController;

		[SerializeField]
		private GameObject killzoneObject;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[Header("Scoring Strategy")]
		[SerializeField]
		private StandardRoundBasedScoringLogic scoringLogic;

		private void OnEnable()
		{
			gameState.NumberOfRounds = (SettingsStore.Target.FiveFrameGame ? NumberOfRounds.FiveFrames : NumberOfRounds.TenFrames);
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Standard);
			_killZones = SettingsStore.Target.ShowKillZones;
			SetActiveTarget();
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnNewRound += SetActiveTarget;
			gameEvents.OnScoreChange += HandleScoreChange;
			gameEvents.OnMiss += HandleMiss;
			scoringLogic.OnUndoComplete += SetActiveTarget;
			scoringLogic.OnFrameUpdateComplete += HandleOnFrameChange;
		}

		protected void OnDisable()
		{
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnNewRound -= SetActiveTarget;
			gameEvents.OnScoreChange -= HandleScoreChange;
			gameEvents.OnMiss -= HandleMiss;
			scoringLogic.OnUndoComplete -= SetActiveTarget;
			scoringLogic.OnFrameUpdateComplete -= HandleOnFrameChange;
		}

		private void HandleOnFrameChange()
		{
			SetActiveTarget();
		}

		private void HandleScoreChange(int? score)
		{
			scoringLogic.RecordScore(new StandardScoreModel(score.GetValueOrDefault()));
		}

		private void HandleMiss()
		{
			HandleScoreChange(null);
		}

		private void HandleNewGame()
		{
			gameState.CurrentFrame = 0;
			SetActiveTarget();
		}

		private void SetActiveTarget()
		{
			killzoneObject.SetActive(IsKillZoneRound());
		}

		private bool IsKillZoneRound()
		{
			int currentFrame = gameState.CurrentFrame;
			bool killZones = _killZones;
			switch (currentFrame)
			{
			case 1:
				if (killZones)
				{
					return true;
				}
				break;
			case 3:
				if (killZones)
				{
					return true;
				}
				break;
			case 5:
				if (killZones)
				{
					return true;
				}
				break;
			case 7:
				if (killZones)
				{
					return true;
				}
				break;
			case 9:
				if (killZones)
				{
					return true;
				}
				break;
			}
			return false;
		}
	}
}
