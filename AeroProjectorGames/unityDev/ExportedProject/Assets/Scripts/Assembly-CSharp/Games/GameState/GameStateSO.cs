using System.Collections.Generic;
using Detection.Models;
using Games.Models;
using HitEffects;
using UnityEngine;

namespace Games.GameState
{
	[CreateAssetMenu(menuName = "Games/Game State")]
	public class GameStateSO : ScriptableObject
	{
		private GameSO _loadedGame;

		[SerializeField]
		private bool _isTargetDisabled;

		[SerializeField]
		private GameEventsSO gameEvents;

		public string CurrentPlayer { get; set; }

		public GameDifficulties GameDifficulty { get; set; }

		public GameStatus GameStatus { get; set; }

		public GameSO LoadedGame => _loadedGame;

		public HitEffectParticleEnum MissEffect { get; set; }

		public bool IsTargetDisabled => _isTargetDisabled;

		public int CurrentFrame { get; set; }

		public int CurrentRoundIndex => CurrentRound - 1;

		public int CurrentRound { get; set; }

		public NumberOfRounds NumberOfRounds { get; set; }

		public Dictionary<string, List<int?>> RoundScores { get; set; } = new Dictionary<string, List<int?>>();

		public int ThrowsPerTurn { get; set; } = 1;

		public int ThrowsRemaining { get; set; }

		public string Player1Name { get; set; }

		public string Player2Name { get; set; }

		public Sprite Player1Icon { get; set; }

		public Sprite Player2Icon { get; set; }

		public Dictionary<string, List<ScoreToken>> InfiniteScoredGameScores { get; set; } = new Dictionary<string, List<ScoreToken>>();

		public string GameType { get; set; }

		public Dictionary<string, int?> TotalScores { get; set; } = new Dictionary<string, int?>();

		public bool DetectionEnabled { get; set; }

		public GamePhysicsMode GamePhysicsMode { get; set; }

		public GameClickableObjectTypeEnum ClickableObjectType { get; set; }

		public void DisableTarget()
		{
			_isTargetDisabled = true;
			gameEvents.RaiseTargetDisabled();
		}

		public void EnableTarget()
		{
			_isTargetDisabled = false;
			gameEvents.RaiseTargetEnabled();
		}

		public void SetLoadedGame(GameSO game)
		{
			_loadedGame = game;
			DetectionEnabled = game != null && game.DetectionEnabled;
		}
	}
}
