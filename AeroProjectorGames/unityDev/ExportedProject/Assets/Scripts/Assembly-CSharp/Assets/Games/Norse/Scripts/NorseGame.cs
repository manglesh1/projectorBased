using System.Collections.Generic;
using Games;
using Games.GameState;
using Players;
using Scoreboard;
using Settings;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Games.Norse.Scripts
{
	public class NorseGame : MonoBehaviour
	{
		private const string GAME_WORD_AXE = "AXE";

		private const string GAME_WORD_NORSE = "NORSE";

		private HashSet<Vector2Int> _availableCells;

		private Button _helpButton;

		private NorseScoreboardBackgroundTimer _helpTimer;

		private int _runningAnimations;

		private bool _viewHelp;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[SerializeField]
		private NorseEventsSO norseEvents;

		[SerializeField]
		private NorseStateSO norseState;

		[Header("Scoring Strategy")]
		[SerializeField]
		private NorseScoringLogic scoringLogic;

		[Header("Display Views")]
		[SerializeField]
		private GameObject gameBoardView;

		[SerializeField]
		private GameObject wordSelectionView;

		[Header("UI Elements")]
		[SerializeField]
		private GameObject bubblePopParticles;

		[SerializeField]
		private GameObject grid;

		[SerializeField]
		private Button phantomIntroCloseButton;

		[SerializeField]
		private GameObject powerUpAnimation;

		[SerializeField]
		private GameObject powerUpCircle;

		[SerializeField]
		private GameObject powerUpHitParticles;

		[SerializeField]
		private GameObject targetAnimation;

		[SerializeField]
		private GameObject targetCircle;

		[Header("Power Up Sprites")]
		[SerializeField]
		private Sprite imageAttack;

		[SerializeField]
		private Sprite imageExtraLife;

		[SerializeField]
		private Sprite imageProtect;

		[SerializeField]
		private Sprite imageReverse;

		[SerializeField]
		private Sprite imageSteal;

		[SerializeField]
		private Sprite imageUnknown;

		[Header("Multi Display Support")]
		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

		[SerializeField]
		private GameObject multiDisplayHelpButton;

		[SerializeField]
		private GameObject multiDisplayIntro;

		[SerializeField]
		private GameObject multiDisplayView;

		private void DecrementAnimationCounter()
		{
			_runningAnimations--;
			if (_runningAnimations <= 0)
			{
				_runningAnimations = 0;
				gameState.EnableTarget();
			}
		}

		private Rect GetParentRect()
		{
			return GetComponentInParent<RectTransform>().rect;
		}

		private float GetObjectRadius(GameObject obj)
		{
			Vector3 localScale = obj.GetComponent<Transform>().localScale;
			Vector2 size = obj.GetComponent<SpriteRenderer>().sprite.rect.size;
			float num = localScale.x / 100f;
			return size.x * num / 2f;
		}

		private Vector3 GetPowerUpLocation()
		{
			return new Vector3(norseState.PowerUpLocationX, norseState.PowerUpLocationY, 10f);
		}

		private Sprite GetPowerUpSprite()
		{
			switch (norseState.SelectedPowerUp)
			{
			case NorsePowerUpEnum.Attack:
				return imageAttack;
			case NorsePowerUpEnum.ExtraLife:
				return imageExtraLife;
			case NorsePowerUpEnum.Protect:
				return imageProtect;
			case NorsePowerUpEnum.Reverse:
				return imageReverse;
			case NorsePowerUpEnum.Steal:
				return imageSteal;
			default:
				return imageUnknown;
			}
		}

		private Vector2 GetRandomAvailablePosition()
		{
			float num = GetObjectRadius(powerUpCircle) + 50f;
			Rect parentRect = GetParentRect();
			for (int i = 0; i < 10000; i++)
			{
				Vector2 vector = new Vector2(Random.Range(parentRect.xMin + num, parentRect.xMax - num), Random.Range(parentRect.yMin + num, parentRect.yMax - num));
				if (IsValidPlacement(vector, num))
				{
					return vector;
				}
			}
			return Vector2.negativeInfinity;
		}

		private Vector3 GetTargetLocation()
		{
			return new Vector3(norseState.TargetLocationX, norseState.TargetLocationY, 10f);
		}

		private Vector2 GetTargetPointInBounds(Vector2 point)
		{
			float objectRadius = GetObjectRadius(targetCircle);
			Rect rect = grid.GetComponent<RectTransform>().rect;
			Vector2 result = default(Vector2);
			if (point.x - objectRadius < rect.xMin)
			{
				result.x = rect.xMin + objectRadius;
			}
			else if (point.x + objectRadius > rect.xMax)
			{
				result.x = rect.xMax - objectRadius;
			}
			else
			{
				result.x = point.x;
			}
			if (point.y - objectRadius < rect.yMin)
			{
				result.y = rect.yMin + objectRadius;
			}
			else if (point.y + objectRadius > rect.yMax)
			{
				result.y = rect.yMax - objectRadius;
			}
			else
			{
				result.y = point.y;
			}
			return result;
		}

		private void HandleGameOver()
		{
			gameState.GameStatus = GameStatus.Finished;
			gameState.DisableTarget();
			gameEvents.RaiseGameOver();
			gameEvents.RaiseWinAnimation();
		}

		private void HandleIntroClose()
		{
			norseEvents.RaiseOnIntroCompleted();
			norseEvents.RaiseOnWordSelected("AXE");
		}

		private void HandleMiss()
		{
			if (!gameState.IsTargetDisabled)
			{
				scoringLogic.AddHistory();
				if (norseState.IsCommandSet)
				{
					scoringLogic.RecordMiss(gameState.CurrentPlayer);
				}
				HandleNextPlayer();
			}
		}

		private void HandleNextPlayer()
		{
			scoringLogic.NextPlayer();
			if (gameState.CurrentPlayer == norseState.CommandingPlayer)
			{
				ResetState();
			}
			else if (norseState.IsCommandSet)
			{
				SetPowerUpLocation(scoringLogic.GetRandomPowerUp(), GetRandomAvailablePosition(), runAnimation: true);
			}
			if (scoringLogic.HasPlayerWon())
			{
				scoringLogic.SetCommander(gameState.CurrentPlayer);
			}
			gameEvents.RaiseUpdateScoreboard();
		}

		private void HandleNewGame()
		{
			gameState.GameStatus = GameStatus.InProgress;
			gameState.EnableTarget();
			norseState.DirectionIsReversed = false;
			LoadPlayerState();
			ResetState();
			scoringLogic.Initialize();
			gameEvents.RaiseUpdateScoreboard();
		}

		private void HandleUndo()
		{
			scoringLogic.UndoHistory();
			StopPowerUpAnimations();
			StopTargetAnimations();
			if (norseState.IsCommandSet)
			{
				SetCommandedLocation(new Vector2(norseState.TargetLocationX, norseState.TargetLocationY), runAnimation: false);
				SetPowerUpLocation(norseState.SelectedPowerUp, new Vector2(norseState.PowerUpLocationX, norseState.PowerUpLocationY), runAnimation: false);
			}
			else
			{
				targetCircle.SetActive(value: false);
				powerUpCircle.SetActive(value: false);
			}
			gameEvents.RaiseUpdateScoreboard();
		}

		private void HandleUpdateScoreboard()
		{
			if (scoringLogic.HasPlayerWon())
			{
				HandleGameOver();
			}
			else
			{
				gameState.EnableTarget();
			}
		}

		private void IncrementAnimationCounter()
		{
			_runningAnimations++;
			gameState.DisableTarget();
		}

		private bool IsValidPlacement(Vector2 pos, float radius)
		{
			Vector2 b = new Vector2(targetCircle.transform.localPosition.x, targetCircle.transform.localPosition.y);
			float num = Vector2.Distance(pos, b);
			float objectRadius = GetObjectRadius(targetCircle);
			float num2 = radius + objectRadius;
			return num >= num2;
		}

		private void LoadPlayerState()
		{
			norseState.Players.Clear();
			foreach (string currentPlayerName in playerState.CurrentPlayerNames)
			{
				norseState.Players.Add(currentPlayerName, new NorsePlayerState());
			}
		}

		private void OnBubbleBurst()
		{
			bubblePopParticles.transform.localPosition = GetPowerUpLocation();
			bubblePopParticles.SetActive(value: true);
			ParticleSystem component = bubblePopParticles.GetComponent<ParticleSystem>();
			if (component != null)
			{
				component.Play();
			}
			DecrementAnimationCounter();
		}

		private void OnBubbleGrown()
		{
			Vector3 localPosition = new Vector3(norseState.PowerUpLocationX, norseState.PowerUpLocationY, 10f);
			powerUpCircle.transform.localPosition = localPosition;
			powerUpCircle.SetActive(value: true);
			SetPowerUpSprite();
		}

		private void OnGameBoardHit(Vector2 point)
		{
			if (!gameState.IsTargetDisabled)
			{
				gameState.DisableTarget();
				scoringLogic.AddHistory();
				powerUpCircle.SetActive(value: false);
				if (norseState.IsCommandSet)
				{
					scoringLogic.RecordMiss(gameState.CurrentPlayer);
				}
				else
				{
					SetCommandedLocation(point, runAnimation: true);
					scoringLogic.SetCommander(gameState.CurrentPlayer);
				}
				HandleNextPlayer();
			}
		}

		private void OnHelpClicked()
		{
			_viewHelp = !_viewHelp;
			if (_viewHelp)
			{
				norseEvents.RaiseOnHelpOpened();
			}
			else
			{
				norseEvents.RaiseOnHelpClosed();
			}
			NorseScoreboardBackgroundTimer helpTimer = _helpTimer;
			if (!(helpTimer == null))
			{
				helpTimer.StartTimer(10f, OnHelpTimerCompleted);
			}
		}

		private void OnHelpClosed()
		{
			gameState.EnableTarget();
		}

		private void OnHelpOpened()
		{
			gameState.DisableTarget();
		}

		private void OnHelpTimerCompleted()
		{
			if (_viewHelp)
			{
				OnHelpClicked();
			}
		}

		private void OnHideGamePieces()
		{
			powerUpAnimation.SetActive(value: false);
			powerUpCircle.SetActive(value: false);
			targetAnimation.SetActive(value: false);
			targetCircle.SetActive(value: false);
			grid.GetComponent<Collider2D>().enabled = true;
		}

		private void OnIntroCompleted()
		{
			multiDisplayIntro.SetActive(value: false);
			multiDisplayHelpButton.SetActive(value: false);
			ShowWordSelection(visible: true);
		}

		private void OnIntroStarted()
		{
			gameBoardView.SetActive(value: false);
			multiDisplayIntro.SetActive(value: true);
			multiDisplayHelpButton.SetActive(value: false);
			wordSelectionView.SetActive(value: false);
		}

		private void OnMissDetected(PointerEventData eventData, Vector2? localPoint)
		{
			HandleMiss();
		}

		private void OnPowerUpHit()
		{
			if (!gameState.IsTargetDisabled)
			{
				gameState.DisableTarget();
				scoringLogic.AddHistory();
				scoringLogic.ExecutePowerUp();
				powerUpCircle.SetActive(value: false);
				powerUpHitParticles.transform.localPosition = GetPowerUpLocation();
				powerUpHitParticles.SetActive(value: true);
				ParticleSystem component = powerUpHitParticles.GetComponent<ParticleSystem>();
				if (component != null)
				{
					component.Play();
				}
				HandleNextPlayer();
			}
		}

		private void OnTargetGrown()
		{
			targetAnimation.SetActive(value: false);
			if (norseState.IsCommandSet)
			{
				targetCircle.transform.localPosition = GetTargetLocation();
				targetCircle.SetActive(value: true);
			}
			DecrementAnimationCounter();
		}

		private void OnTargetHit()
		{
			if (!gameState.IsTargetDisabled)
			{
				gameState.DisableTarget();
				scoringLogic.AddHistory();
				powerUpCircle.SetActive(value: false);
				targetCircle.SetActive(value: false);
				targetAnimation.SetActive(value: true);
				targetAnimation.GetComponent<Animator>().Play("Base Layer.Bounce", 0, 0f);
				IncrementAnimationCounter();
				HandleNextPlayer();
			}
		}

		private void OnWordSelected(string word)
		{
			ShowWordSelection(visible: false);
			norseState.GameWord = word;
		}

		private void ResetState()
		{
			norseState.TargetLocationX = 0f;
			norseState.TargetLocationY = 0f;
			norseState.CommandingPlayer = string.Empty;
			norseState.IsCommandSet = false;
			powerUpCircle.SetActive(value: false);
			targetCircle.SetActive(value: false);
			grid.GetComponent<Collider2D>().enabled = true;
		}

		private void SetCommandedLocation(Vector2 point, bool runAnimation)
		{
			Vector2 targetPointInBounds = GetTargetPointInBounds(point);
			norseState.TargetLocationX = targetPointInBounds.x;
			norseState.TargetLocationY = targetPointInBounds.y;
			Vector3 localPosition = new Vector3(targetPointInBounds.x, targetPointInBounds.y, 10f);
			targetCircle.transform.localPosition = localPosition;
			if (norseState.IsCommandSet && !runAnimation)
			{
				targetCircle.transform.localPosition = localPosition;
				targetCircle.SetActive(value: true);
			}
			if (runAnimation)
			{
				targetAnimation.transform.localPosition = localPosition;
				targetAnimation.SetActive(value: true);
				targetAnimation.GetComponent<Animator>().Play("Base Layer.Grow", 0, 0f);
				IncrementAnimationCounter();
			}
			grid.GetComponent<Collider2D>().enabled = false;
		}

		private void SetPowerUpLocation(NorsePowerUpEnum powerUp, Vector2 point, bool runAnimation)
		{
			norseState.PowerUpLocationX = point.x;
			norseState.PowerUpLocationY = point.y;
			norseState.SelectedPowerUp = powerUp;
			norseEvents.RaiseOnPowerUpSelected();
			Vector3 localPosition = new Vector3(point.x, point.y, 10f);
			powerUpCircle.transform.localPosition = localPosition;
			if (norseState.IsCommandSet && !runAnimation)
			{
				SetPowerUpSprite();
				powerUpCircle.SetActive(value: true);
			}
			else
			{
				powerUpCircle.SetActive(value: false);
			}
			if (runAnimation)
			{
				powerUpAnimation.transform.localPosition = localPosition;
				powerUpAnimation.SetActive(value: true);
				powerUpAnimation.GetComponent<Animator>().Play("Base Layer.Grow", 0, 0f);
				IncrementAnimationCounter();
			}
		}

		private void SetPowerUpSprite()
		{
			SpriteRenderer component = powerUpCircle.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				component.sprite = GetPowerUpSprite();
			}
		}

		private void ShowWordSelection(bool visible)
		{
			wordSelectionView.SetActive(visible);
			gameBoardView.SetActive(!visible);
			multiDisplayHelpButton.SetActive(!visible);
			if (visible)
			{
				norseEvents.RaiseOnWordSelection();
			}
			else
			{
				norseEvents.RaiseOnGameStarted();
			}
		}

		private void StopPowerUpAnimations()
		{
			powerUpAnimation.GetComponent<Animator>().Play("Base Layer.Empty");
		}

		private void StopTargetAnimations()
		{
			targetAnimation.GetComponent<Animator>().Play("Base Layer.Empty");
		}

		private void OnDisable()
		{
			GameObject gameObject = multiDisplayHelpButton;
			if (gameObject != null)
			{
				Button component = gameObject.GetComponent<Button>();
				if (component != null)
				{
					component.onClick.RemoveAllListeners();
				}
			}
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= OnMissDetected;
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnUndo -= HandleUndo;
			gameEvents.OnUpdateScoreboard -= HandleUpdateScoreboard;
			norseEvents.OnBubbleBurst -= OnBubbleBurst;
			norseEvents.OnBubbleGrown -= OnBubbleGrown;
			norseEvents.OnGameBoardHit -= OnGameBoardHit;
			norseEvents.OnHelpClosed -= OnHelpClosed;
			norseEvents.OnHelpOpened -= OnHelpOpened;
			norseEvents.OnHideGamePieces -= OnHideGamePieces;
			norseEvents.OnIntroCompleted -= OnIntroCompleted;
			norseEvents.OnIntroStarted -= OnIntroStarted;
			norseEvents.OnPowerUpHit -= OnPowerUpHit;
			norseEvents.OnTargetGrown -= OnTargetGrown;
			norseEvents.OnTargetHit -= OnTargetHit;
			norseEvents.OnWordSelected -= OnWordSelected;
			phantomIntroCloseButton.onClick.RemoveAllListeners();
			NorseScoreboardBackgroundTimer helpTimer = _helpTimer;
			if (!(helpTimer == null))
			{
				helpTimer.StopTimer();
			}
		}

		private void OnEnable()
		{
			norseState.GameWord = "NORSE";
			gameState.NumberOfRounds = NumberOfRounds.Infinite;
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Norse);
			Button component = multiDisplayHelpButton.GetComponent<Button>();
			if (component != null)
			{
				component.onClick.AddListener(OnHelpClicked);
			}
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += OnMissDetected;
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnUndo += HandleUndo;
			gameEvents.OnUpdateScoreboard += HandleUpdateScoreboard;
			norseEvents.OnBubbleBurst += OnBubbleBurst;
			norseEvents.OnBubbleGrown += OnBubbleGrown;
			norseEvents.OnGameBoardHit += OnGameBoardHit;
			norseEvents.OnHelpClosed += OnHelpClosed;
			norseEvents.OnHelpOpened += OnHelpOpened;
			norseEvents.OnHideGamePieces += OnHideGamePieces;
			norseEvents.OnIntroCompleted += OnIntroCompleted;
			norseEvents.OnIntroStarted += OnIntroStarted;
			norseEvents.OnPowerUpHit += OnPowerUpHit;
			norseEvents.OnTargetGrown += OnTargetGrown;
			norseEvents.OnTargetHit += OnTargetHit;
			norseEvents.OnWordSelected += OnWordSelected;
			phantomIntroCloseButton.onClick.AddListener(HandleIntroClose);
			_runningAnimations = 0;
			ResetState();
			norseEvents.RaiseOnIntroStarted();
		}

		private void Start()
		{
			_helpButton = multiDisplayHelpButton.GetComponent<Button>();
			_helpTimer = GetComponent<NorseScoreboardBackgroundTimer>();
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayView);
				multiDisplayEvents.RaiseLoadMultiDisplayHelpButtonObject(multiDisplayHelpButton);
			}
			else
			{
				multiDisplayView.SetActive(value: false);
			}
			HandleNewGame();
		}
	}
}
