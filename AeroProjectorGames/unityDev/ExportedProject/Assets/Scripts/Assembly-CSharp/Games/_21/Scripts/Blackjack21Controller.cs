using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blackjack21;
using Games.CustomComponents;
using Games.GameState;
using Games._21.ScoringLogic;
using Helpers;
using Players;
using Scoreboard;
using Settings;
using UI.MultiDisplay;
using UnityEngine;

namespace Games._21.Scripts
{
	public class Blackjack21Controller : MonoBehaviour
	{
		private const int PLAYING_CARD_GAMBOARD_COUNT = 30;

		private const float SCALE_SIZE_VALUE = 1.2f;

		private const float SCORING_CARD_SCALE_SIZE_VALUE = 1.2f;

		private const int STOLE_CARD_PLAYER_DEFAULT_SCORE = -1;

		private const int WINNING_SCORE = 21;

		private readonly Quaternion FLIPPED_CARD_ROTATION = Quaternion.Euler(0f, 180f, 0f);

		private readonly Quaternion DEAL_ROTATION = Quaternion.Euler(new Vector3(0f, 0f, -405f));

		private List<Coroutine> _dealCardsOneAtATimeRoutine = new List<Coroutine>();

		private List<Coroutine> _dealCardsRoutine = new List<Coroutine>();

		private List<Coroutine> _dealCardColumnRoutine = new List<Coroutine>();

		private List<Coroutine> _flipCardsOneAtATimeRoutine = new List<Coroutine>();

		private List<Coroutine> _flipCardColumnRoutine = new List<Coroutine>();

		private List<Coroutine> _lerpRotationChangeFaceRoutine = new List<Coroutine>();

		private List<Coroutine> _lerpRotationRoutine = new List<Coroutine>();

		private List<Coroutine> _lerpScaleRoutine = new List<Coroutine>();

		private List<Coroutine> _prepareToDealRoutine = new List<Coroutine>();

		private List<Coroutine> _scaleUpAndBackDownOnClickRoutine = new List<Coroutine>();

		private AnimationHelper _animationHelper = new AnimationHelper();

		private bool _areAllCardsFlipped;

		private int _cardsDealtCounter;

		private int _cardFlipMaterialCounter;

		private List<List<RectTransform>> _gameboardCardPlacement = new List<List<RectTransform>>();

		private Vector3 _originalSize = Vector3.zero;

		private Vector3 _scoringCardOriginalSize = Vector3.zero;

		private bool _onFirstRender = true;

		private List<GameObject> _playingCards;

		private List<List<PlacedCard>> _playingCardPlacement = new List<List<PlacedCard>>();

		private Stack<List<List<string>>> _playingCardPlacementHistory = new Stack<List<List<string>>>();

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayScoringEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[Header("Scoring Strategy")]
		[SerializeField]
		private TwentyOneScoringLogic scoringLogic;

		[Space]
		[Header("Gameboard Placement Elements")]
		[SerializeField]
		private GameObject dealPosition;

		[SerializeField]
		private GameObject gameBoardParent;

		[Space]
		[Header("Animation Elements")]
		[SerializeField]
		private ParticleSystem explosionPS;

		[Space]
		[Header("External References")]
		[SerializeField]
		private PlayingCardMaterialHelper cardMaterialHelper;

		[SerializeField]
		private Blackjack21MultiDisplayScoringController multiDiplayController;

		private void OnEnable()
		{
			gameState.NumberOfRounds = NumberOfRounds.InfiniteScored;
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.TwentyOne);
			gameEvents.OnGameOver += StopAnimations;
			gameEvents.OnNewGame += ResetBoard;
			gameEvents.OnNewRound += FlipAllCards;
			scoringLogic.OnSaveGameStateBegin += ConvertAndSaveToStringList;
			scoringLogic.OnUndoComplete += HandleUndoComplete;
			GetGameBoardPlacementRectTransforms();
			DestoryPlayTokensAndResetBoard();
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayScoringEvents.RaiseLoadScoringObject(multiDiplayController.gameObject);
			}
		}

		private void OnDisable()
		{
			gameEvents.OnGameOver -= StopAnimations;
			gameEvents.OnNewGame -= ResetBoard;
			gameEvents.OnNewRound -= FlipAllCards;
			scoringLogic.OnSaveGameStateBegin -= ConvertAndSaveToStringList;
			scoringLogic.OnUndoComplete -= HandleUndoComplete;
			_playingCardPlacementHistory.Clear();
			DestoryPlayTokensAndResetBoard();
			gameState.EnableTarget();
		}

		private void OnRenderObject()
		{
			if (!_onFirstRender)
			{
				return;
			}
			foreach (List<PlacedCard> item in _playingCardPlacement)
			{
				for (int i = 0; i < item.Count; i++)
				{
					item[i].GameboardCard.transform.position = dealPosition.transform.position;
					item[i].GameboardCard.transform.rotation = DEAL_ROTATION;
				}
			}
			_dealCardsOneAtATimeRoutine.Add(StartCoroutine(DealCardsOneAtATime()));
			_onFirstRender = false;
		}

		private bool CheckIfPlayerOwns(GameObject playingCardModel)
		{
			return gameState.InfiniteScoredGameScores.Values.Any((List<ScoreToken> value) => value.Any((ScoreToken token) => token.TokenName == playingCardModel.name));
		}

		private bool CheckIfMaterialIsOnAPlayerOwnedCard(PlayingCard playingCardMaterial)
		{
			return gameState.InfiniteScoredGameScores.Values.Any((List<ScoreToken> value) => value.Any((ScoreToken token) => token.TokenName == playingCardMaterial.Name));
		}

		private void ConvertAndSaveToStringList()
		{
			List<List<string>> list = new List<List<string>>();
			int num = 0;
			foreach (List<PlacedCard> item in _playingCardPlacement)
			{
				list.Add(new List<string>());
				foreach (PlacedCard item2 in item)
				{
					list[num].Add(item2.GameboardCard.name);
				}
				num++;
			}
			_playingCardPlacementHistory.Push(list);
		}

		private void CreateAndPlacePlayingCardsOnGameboard()
		{
			_playingCards = cardMaterialHelper.GetRandomizedPlayingCards(30);
			_playingCardPlacement.Clear();
			if (_gameboardCardPlacement.Count == 0)
			{
				GetGameBoardPlacementRectTransforms();
			}
			int num = 0;
			for (int i = 0; i < _gameboardCardPlacement.Count; i++)
			{
				_playingCardPlacement.Add(new List<PlacedCard>());
				foreach (RectTransform item in _gameboardCardPlacement[i])
				{
					GameObject newPlayingCard = UnityEngine.Object.Instantiate(_playingCards[num], item.transform);
					newPlayingCard.name = _playingCards[num].name;
					GameObject gameObject = UnityEngine.Object.Instantiate(newPlayingCard);
					PlacedCard placedCard = new PlacedCard(newPlayingCard, gameObject);
					SetPlayingCardListener(placedCard);
					TwoValueScoredButton component = gameObject.GetComponent<TwoValueScoredButton>();
					component.onClick.RemoveAllListeners();
					component.onClick.AddListener(delegate
					{
						newPlayingCard.GetComponent<TwoValueScoredButton>().onClick.Invoke();
					});
					gameObject.gameObject.SetActive(SettingsStore.Interaction.MultiDisplayEnabled);
					_playingCardPlacement[i].Add(placedCard);
					num++;
				}
			}
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDiplayController.SetupBoard(_playingCardPlacement);
			}
		}

		private ScoreToken CreateNewToken(GameObject hitToken)
		{
			return new ScoreToken
			{
				AlternateScoreValue = hitToken.GetComponent<TwoValueScoredButton>().SecondaryScore,
				ScoreValue = hitToken.GetComponent<TwoValueScoredButton>().Score,
				Token = cardMaterialHelper.GetScoreboardTokenReference(hitToken.name),
				TokenName = hitToken.name
			};
		}

		private void HandleUndoComplete()
		{
			if (_areAllCardsFlipped && gameState.CurrentRound == 1)
			{
				_areAllCardsFlipped = false;
			}
			if (_playingCardPlacementHistory.Count == 0)
			{
				return;
			}
			List<List<string>> list = _playingCardPlacementHistory.Pop();
			for (int i = 0; i < _playingCardPlacement.Count; i++)
			{
				for (int j = 0; j < _playingCardPlacement[i].Count; j++)
				{
					if (list[i][j] != _playingCardPlacement[i][j].GameboardCard.name)
					{
						cardMaterialHelper.SetPlayingCardMaterial(_playingCardPlacement[i][j].GameboardCard, list[i][j]);
						SetPlayingCardListener(_playingCardPlacement[i][j]);
					}
					if (CheckIfPlayerOwns(_playingCardPlacement[i][j].GameboardCard))
					{
						_playingCardPlacement[i][j].GameboardCard.transform.localScale = ((_playingCardPlacement[i][j].GameboardCard.transform.localScale == _originalSize) ? (_playingCardPlacement[i][j].GameboardCard.transform.localScale / 1.2f) : _playingCardPlacement[i][j].GameboardCard.transform.localScale);
						_playingCardPlacement[i][j].GameboardCard.transform.rotation = FLIPPED_CARD_ROTATION;
						_playingCardPlacement[i][j].ScoringCard.transform.localScale = ((_playingCardPlacement[i][j].ScoringCard.transform.localScale == _scoringCardOriginalSize) ? (_playingCardPlacement[i][j].ScoringCard.transform.localScale / 1.2f) : _playingCardPlacement[i][j].ScoringCard.transform.localScale);
						_playingCardPlacement[i][j].ScoringCard.transform.rotation = FLIPPED_CARD_ROTATION;
						multiDiplayController.SelectCard(_playingCardPlacement[i][j].ScoringCard);
					}
					else
					{
						_playingCardPlacement[i][j].GameboardCard.transform.localScale = _originalSize;
						_playingCardPlacement[i][j].GameboardCard.transform.rotation = ((gameState.CurrentRound == 1) ? Quaternion.Euler(Vector3.zero) : FLIPPED_CARD_ROTATION);
						_playingCardPlacement[i][j].ScoringCard.transform.localScale = _scoringCardOriginalSize;
						_playingCardPlacement[i][j].ScoringCard.transform.rotation = ((gameState.CurrentRound == 1) ? Quaternion.Euler(Vector3.zero) : FLIPPED_CARD_ROTATION);
						multiDiplayController.DeselectCard(_playingCardPlacement[i][j].ScoringCard);
					}
				}
			}
		}

		private void DestoryPlayTokensAndResetBoard(bool resetBoard = true)
		{
			foreach (List<PlacedCard> item in _playingCardPlacement)
			{
				foreach (PlacedCard item2 in item)
				{
					if (item2 != null)
					{
						UnityEngine.Object.Destroy(item2.GameboardCard);
						UnityEngine.Object.Destroy(item2.ScoringCard);
					}
				}
			}
			_playingCardPlacement.Clear();
			if (resetBoard)
			{
				ResetBoard();
			}
		}

		private void GetGameBoardPlacementRectTransforms()
		{
			_gameboardCardPlacement = new List<List<RectTransform>>();
			for (int i = 0; i < gameBoardParent.transform.childCount; i++)
			{
				_gameboardCardPlacement.Add(new List<RectTransform>());
				foreach (RectTransform item in gameBoardParent.transform.GetChild(i))
				{
					_gameboardCardPlacement[i].Add(item);
				}
			}
		}

		private string GetPlayerWhoOwnsTheCard(GameObject playingCardModel)
		{
			return gameState.InfiniteScoredGameScores.FirstOrDefault((KeyValuePair<string, List<ScoreToken>> value) => value.Value.Any((ScoreToken token) => token.TokenName == playingCardModel.name)).Key;
		}

		private void PlayExplosion(GameObject obj)
		{
			UnityEngine.Object.Instantiate(explosionPS).transform.position = obj.transform.position;
		}

		private void ResetBoard()
		{
			gameState.DisableTarget();
			StopAnimations();
			_areAllCardsFlipped = false;
			_playingCardPlacementHistory.Clear();
			if (_onFirstRender)
			{
				CreateAndPlacePlayingCardsOnGameboard();
			}
			else
			{
				StartCoroutine(DealCardsOneAtATime(reverseDeal: true));
			}
		}

		private void ResetCardSize(string cardName)
		{
			foreach (List<PlacedCard> item in _playingCardPlacement)
			{
				foreach (PlacedCard item2 in item)
				{
					if (cardName.Contains(item2.GameboardCard.name))
					{
						PlayExplosion(item2.GameboardCard);
						_lerpScaleRoutine.Add(StartCoroutine(PlayResetCardAnimation(item2.GameboardCard.GetComponent<RectTransform>())));
						_lerpScaleRoutine.Add(StartCoroutine(PlayResetCardAnimation(item2.ScoringCard.GetComponent<RectTransform>(), scoringCard: true)));
						multiDiplayController.DeselectCard(item2.ScoringCard);
					}
				}
			}
		}

		private void ResetPlayingCardsAndPlaceNewRandomMaterial()
		{
			List<PlayingCard> randomizedPlayingCardMaterial = cardMaterialHelper.GetRandomizedPlayingCardMaterial(30);
			int num = 0;
			foreach (List<PlacedCard> item in _playingCardPlacement)
			{
				foreach (PlacedCard item2 in item)
				{
					if (item2.GameboardCard.transform.localScale == _originalSize)
					{
						cardMaterialHelper.SetPlayingCardProperties(item2.GameboardCard.gameObject, randomizedPlayingCardMaterial[num]);
						cardMaterialHelper.SetPlayingCardProperties(item2.ScoringCard.gameObject, randomizedPlayingCardMaterial[num]);
						SetPlayingCardListener(item2);
						num++;
					}
				}
			}
			StartCoroutine(DealCardsOneAtATime());
		}

		private void SetPlayingCardListener(PlacedCard placedCard)
		{
			placedCard.GameboardCard.GetComponent<Keep3dModelSizeWithUIParent>().TransformToControlResizing = placedCard.GameboardCard.transform.parent.GetComponent<RectTransform>();
			TwoValueScoredButton component = placedCard.GameboardCard.GetComponent<TwoValueScoredButton>();
			component.onClick.RemoveAllListeners();
			component.onClick.AddListener(delegate
			{
				CardHitHandler(placedCard);
			});
		}

		private void StopAnimations()
		{
			StopCoroutine(_dealCardsOneAtATimeRoutine);
			StopCoroutine(_dealCardsRoutine);
			StopCoroutine(_dealCardColumnRoutine);
			StopCoroutine(_flipCardsOneAtATimeRoutine);
			StopCoroutine(_flipCardColumnRoutine);
			StopCoroutine(_prepareToDealRoutine);
			StopCoroutine(_lerpRotationChangeFaceRoutine);
			StopCoroutine(_lerpRotationRoutine);
			StopCoroutine(_scaleUpAndBackDownOnClickRoutine);
		}

		private void StopCoroutine(List<Coroutine> coroutineList)
		{
			foreach (Coroutine coroutine in coroutineList)
			{
				if (coroutine != null)
				{
					StopCoroutine(coroutine);
				}
			}
		}

		private IEnumerator DealCardsOneAtATime(bool reverseDeal = false)
		{
			Blackjack21Controller blackjack21Controller = this;
			blackjack21Controller._cardsDealtCounter = 0;
			yield return new WaitForSecondsRealtime(0.5f);
			if (reverseDeal)
			{
				yield return blackjack21Controller.StartCoroutine(blackjack21Controller.FlipAllCardsToStartingRotation());
				int i = blackjack21Controller._playingCardPlacement.Count - 1;
				while (0 <= i)
				{
					yield return new WaitForSecondsRealtime(0.2f);
					blackjack21Controller._dealCardColumnRoutine.Add(blackjack21Controller.StartCoroutine(blackjack21Controller.DealCardColumn(blackjack21Controller._playingCardPlacement[i], reverseDeal)));
					int num = i - 1;
					i = num;
				}
				yield return new WaitForSecondsRealtime(4.5f);
				blackjack21Controller.ResetPlayingCardsAndPlaceNewRandomMaterial();
			}
			else
			{
				foreach (List<PlacedCard> col in blackjack21Controller._playingCardPlacement)
				{
					blackjack21Controller._dealCardColumnRoutine.Add(blackjack21Controller.StartCoroutine(blackjack21Controller.DealCardColumn(col)));
					yield return new WaitForSecondsRealtime(0.75f);
				}
			}
			blackjack21Controller.ConvertAndSaveToStringList();
		}

		private IEnumerator DealCardColumn(List<PlacedCard> col, bool reverseDeal = false)
		{
			Blackjack21Controller blackjack21Controller = this;
			if (reverseDeal)
			{
				foreach (PlacedCard placedCard in col)
				{
					blackjack21Controller._prepareToDealRoutine.Add(blackjack21Controller.StartCoroutine(blackjack21Controller.PrepareToDeal(placedCard.GameboardCard)));
					yield return new WaitForSecondsRealtime(0.75f);
				}
				yield break;
			}
			int i = col.Count - 1;
			while (0 <= i)
			{
				yield return new WaitForSecondsRealtime(0.2f);
				blackjack21Controller._dealCardsRoutine.Add(blackjack21Controller.StartCoroutine(blackjack21Controller.DealCards(col[i].GameboardCard)));
				int num = i - 1;
				i = num;
			}
		}

		private IEnumerator DealCards(GameObject playingCard)
		{
			Blackjack21Controller blackjack21Controller = this;
			Vector3 zero = Vector3.zero;
			Quaternion endRotation = Quaternion.Euler(Vector3.zero);
			float duration = 5f;
			float animationCulling = 4f;
			yield return blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpLocalAnimation(playingCard.GetComponent<RectTransform>(), playingCard.transform.localScale, zero, endRotation, duration, animationCulling));
			blackjack21Controller._cardsDealtCounter++;
			if (blackjack21Controller._cardsDealtCounter == 30)
			{
				blackjack21Controller.gameState.EnableTarget();
			}
		}

		private IEnumerator PrepareToDeal(GameObject playingCard)
		{
			Blackjack21Controller blackjack21Controller = this;
			Vector3 position = blackjack21Controller.dealPosition.transform.position;
			Vector3 originalSize = blackjack21Controller._originalSize;
			Quaternion dealRotation = blackjack21Controller.DEAL_ROTATION;
			if (!(playingCard.transform.position == position))
			{
				float duration = 5f;
				float animationCulling = 4.35f;
				yield return blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpWorldAnimation(playingCard.GetComponent<RectTransform>(), originalSize, position, dealRotation, duration, animationCulling));
			}
		}

		private IEnumerator FlipAllCardsToStartingRotation()
		{
			Blackjack21Controller blackjack21Controller = this;
			float duration = 0.03f;
			float culling = 0.02f;
			Quaternion defaultRotation = Quaternion.Euler(0f, 0f, 0f);
			foreach (List<PlacedCard> placedCardList in blackjack21Controller._playingCardPlacement)
			{
				foreach (PlacedCard card in placedCardList)
				{
					if (card.GameboardCard.transform.rotation != defaultRotation)
					{
						yield return blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpWorldAnimation(card.GameboardCard.GetComponent<RectTransform>(), card.GameboardCard.transform.localScale, card.GameboardCard.transform.position, defaultRotation, duration, culling));
					}
					if (card.ScoringCard.transform.rotation != defaultRotation)
					{
						yield return blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpWorldAnimation(card.ScoringCard.GetComponent<RectTransform>(), blackjack21Controller._scoringCardOriginalSize, card.ScoringCard.transform.position, defaultRotation, duration, culling));
						blackjack21Controller.multiDiplayController.DeselectCard(card.ScoringCard);
					}
				}
			}
		}

		private void FlipAllCards()
		{
			if (gameState.GameStatus != GameStatus.Finished)
			{
				gameState.DisableTarget();
				if (gameState.CurrentRound != 1 && !_areAllCardsFlipped)
				{
					_flipCardsOneAtATimeRoutine.Add(StartCoroutine(FlipCardsOneAtATime()));
					_areAllCardsFlipped = true;
				}
				if (gameState.CurrentRound != 2 && _areAllCardsFlipped)
				{
					List<PlayingCard> randomizedPlayingCardMaterial = cardMaterialHelper.GetRandomizedPlayingCardMaterial(30);
					_cardFlipMaterialCounter = 0;
					_flipCardsOneAtATimeRoutine.Add(StartCoroutine(FlipCardsOneAtATime(mixCards: true, randomizedPlayingCardMaterial)));
				}
			}
		}

		private IEnumerator FlipCardsOneAtATime(bool mixCards = false, List<PlayingCard> materialList = null)
		{
			Blackjack21Controller blackjack21Controller = this;
			if (mixCards)
			{
				foreach (List<PlacedCard> col in blackjack21Controller._playingCardPlacement)
				{
					blackjack21Controller._flipCardColumnRoutine.Add(blackjack21Controller.StartCoroutine(blackjack21Controller.FlipCardColumn(col, mixCards, materialList)));
					yield return new WaitForSecondsRealtime(0.1f);
				}
				yield return new WaitForSecondsRealtime(2f);
			}
			else
			{
				foreach (List<PlacedCard> col2 in blackjack21Controller._playingCardPlacement)
				{
					blackjack21Controller._flipCardColumnRoutine.Add(blackjack21Controller.StartCoroutine(blackjack21Controller.FlipCardColumn(col2)));
					yield return new WaitForSecondsRealtime(0.1f);
				}
				yield return new WaitForSecondsRealtime(1f);
			}
			blackjack21Controller.gameState.EnableTarget();
		}

		private IEnumerator FlipCardColumn(List<PlacedCard> col, bool mixCards = false, List<PlayingCard> materialList = null)
		{
			Blackjack21Controller blackjack21Controller1 = this;
			if (mixCards)
			{
				foreach (PlacedCard placedCard in col)
				{
					Blackjack21Controller blackjack21Controller2 = blackjack21Controller1;
					PlacedCard card = placedCard;
					if (card.GameboardCard.GetComponent<RectTransform>().localScale == blackjack21Controller1._originalSize || blackjack21Controller1._originalSize == Vector3.zero)
					{
						int index = 0;
						while (index < 30 && blackjack21Controller1.CheckIfMaterialIsOnAPlayerOwnedCard(materialList[blackjack21Controller1._cardFlipMaterialCounter]))
						{
							blackjack21Controller1._cardFlipMaterialCounter++;
							int num = index + 1;
							index = num;
						}
						blackjack21Controller1._lerpRotationChangeFaceRoutine.Add(blackjack21Controller1.StartCoroutine(blackjack21Controller1.LerpRotationChangeFace(card.GameboardCard.GetComponent<RectTransform>(), materialList[blackjack21Controller1._cardFlipMaterialCounter], delegate
						{
							blackjack21Controller2.SetPlayingCardListener(card);
						})));
						blackjack21Controller1._lerpRotationChangeFaceRoutine.Add(blackjack21Controller1.StartCoroutine(blackjack21Controller1.LerpRotationChangeFace(card.ScoringCard.GetComponent<RectTransform>(), materialList[blackjack21Controller1._cardFlipMaterialCounter])));
						blackjack21Controller1._cardFlipMaterialCounter++;
					}
					yield return new WaitForSecondsRealtime(0.1f);
				}
				yield break;
			}
			foreach (PlacedCard placedCard2 in col)
			{
				blackjack21Controller1._lerpRotationRoutine.Add(blackjack21Controller1.StartCoroutine(blackjack21Controller1.LerpRotation(placedCard2.GameboardCard.GetComponent<RectTransform>())));
				blackjack21Controller1._lerpRotationRoutine.Add(blackjack21Controller1.StartCoroutine(blackjack21Controller1.LerpRotation(placedCard2.ScoringCard.GetComponent<RectTransform>())));
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}

		private IEnumerator LerpRotationChangeFace(RectTransform playingCardTransform, PlayingCard newMaterial, Action callback = null)
		{
			Blackjack21Controller blackjack21Controller = this;
			Quaternion endRotation1 = Quaternion.Euler(Vector3.zero);
			float duration = 5f;
			float animationCulling = 4.5f;
			yield return blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpLocalAnimation(playingCardTransform, playingCardTransform.transform.localScale, playingCardTransform.transform.localPosition, endRotation1, duration, animationCulling));
			blackjack21Controller.cardMaterialHelper.SetPlayingCardProperties(playingCardTransform.gameObject, newMaterial);
			Quaternion endRotation2 = ((!(playingCardTransform.rotation != blackjack21Controller.FLIPPED_CARD_ROTATION)) ? playingCardTransform.rotation : blackjack21Controller.FLIPPED_CARD_ROTATION);
			yield return blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpLocalAnimation(playingCardTransform, playingCardTransform.transform.localScale, playingCardTransform.transform.localPosition, endRotation2, duration, animationCulling));
			callback?.Invoke();
		}

		private IEnumerator LerpRotation(RectTransform playingCardTransform)
		{
			Blackjack21Controller blackjack21Controller = this;
			Quaternion.Euler(Vector3.zero);
			Quaternion endRotation = ((!(playingCardTransform.rotation != blackjack21Controller.FLIPPED_CARD_ROTATION)) ? playingCardTransform.rotation : blackjack21Controller.FLIPPED_CARD_ROTATION);
			float duration = 5f;
			float animationCulling = 4.5f;
			yield return blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpLocalAnimation(playingCardTransform, playingCardTransform.transform.localScale, playingCardTransform.transform.localPosition, endRotation, duration, animationCulling));
		}

		private void MatchPlacedCardsRotation()
		{
			foreach (List<PlacedCard> item in _playingCardPlacement)
			{
				foreach (PlacedCard item2 in item)
				{
					if (item2.ScoringCard.transform.rotation != item2.GameboardCard.transform.rotation)
					{
						_lerpRotationRoutine.Add(StartCoroutine(_animationHelper.LerpWorldAnimation(item2.ScoringCard.GetComponent<RectTransform>(), item2.ScoringCard.transform.localScale, item2.ScoringCard.transform.position, item2.GameboardCard.transform.rotation, 2f, 0.5f)));
					}
				}
			}
		}

		private IEnumerator PlayResetCardAnimation(RectTransform playingCardTransform, bool scoringCard = false)
		{
			Blackjack21Controller blackjack21Controller = this;
			Vector3 startPosition = playingCardTransform.localPosition;
			Vector3 endScale1 = playingCardTransform.localScale * 3f;
			Vector3 endLocalPosition1 = playingCardTransform.localPosition - new Vector3(0f, 1000f, 0f);
			Quaternion endRotation = Quaternion.Euler(0f, 0f, -1000f);
			float duration1 = 5f;
			float animationCulling1 = 4.5f;
			if (!scoringCard)
			{
				yield return blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpLocalAnimation(playingCardTransform, endScale1, endLocalPosition1, endRotation, duration1, animationCulling1));
			}
			Vector3 endScale2;
			Vector3 endLocalPosition2;
			if (scoringCard)
			{
				endScale2 = blackjack21Controller._scoringCardOriginalSize;
				endLocalPosition2 = playingCardTransform.localPosition;
			}
			else
			{
				endScale2 = blackjack21Controller._originalSize;
				endLocalPosition2 = startPosition;
			}
			Quaternion flippedCardRotation = blackjack21Controller.FLIPPED_CARD_ROTATION;
			float duration2 = 2f;
			float animationCulling2 = 1f;
			yield return blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpLocalAnimation(playingCardTransform, endScale2, endLocalPosition2, flippedCardRotation, duration2, animationCulling2));
		}

		private int CalculateAlternateScoresIntoTotal(int totalScore, ScoreToken hitToken, string playerName)
		{
			foreach (ScoreToken item in gameState.InfiniteScoredGameScores[playerName])
			{
				if (!(item.TokenName == hitToken.TokenName))
				{
					if (totalScore <= 21)
					{
						return totalScore;
					}
					if (item.AlternateScoreValue > 0)
					{
						totalScore -= item.ScoreValue;
						totalScore += item.AlternateScoreValue;
					}
				}
			}
			return totalScore;
		}

		private void CallAfterAnimation(GameObject hitCard, int playerScore, string currentOwner, int stoleCardPlayerScore)
		{
			CheckForWinningScore(playerScore);
			if (string.IsNullOrEmpty(currentOwner))
			{
				scoringLogic.RecordScore(new TwentyOneScoreModel
				{
					ScoreToken = CreateNewToken(hitCard),
					TotalScore = playerScore
				});
				return;
			}
			gameEvents.RaiseMoveToken(new MoveToken
			{
				FromPlayer = currentOwner,
				FromPlayerScore = stoleCardPlayerScore,
				ToPlayer = gameState.CurrentPlayer,
				ToPlayerScore = playerScore,
				TokenName = hitCard.name
			});
		}

		private void CardAnimationActions(PlacedCard playingCard, Action afterAnimationCallback)
		{
			playingCard.GameboardCard.GetComponent<Keep3dModelSizeWithUIParent>().enabled = false;
			playingCard.ScoringCard.GetComponent<Keep3dModelSizeWithUIParent>().enabled = false;
			if (_originalSize == Vector3.zero)
			{
				_originalSize = _playingCardPlacement[0][0].GameboardCard.GetComponent<RectTransform>().localScale;
			}
			if (_scoringCardOriginalSize == Vector3.zero)
			{
				_scoringCardOriginalSize = _playingCardPlacement[0][0].ScoringCard.GetComponent<RectTransform>().localScale;
			}
			_scaleUpAndBackDownOnClickRoutine.Add(StartCoroutine(ScaleUpAndBackDownOnClick_GameBoard(playingCard, afterAnimationCallback)));
		}

		public void CardHitHandler(PlacedCard placedCard)
		{
			if (gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				return;
			}
			TwoValueScoredButton scoredButton = placedCard.GameboardCard.GetComponent<TwoValueScoredButton>();
			ConvertAndSaveToStringList();
			gameState.DisableTarget();
			int playerScore = 0;
			int stoleCardPlayerScore = -1;
			string cardOwner = GetPlayerWhoOwnsTheCard(scoredButton.gameObject);
			if (!string.IsNullOrEmpty(cardOwner) && cardOwner == gameState.CurrentPlayer)
			{
				HandlePlayerHittingOwnCard(scoredButton, cardOwner);
				return;
			}
			playerScore = GetNewTotalScore(scoredButton, gameState.CurrentPlayer);
			stoleCardPlayerScore = ((!string.IsNullOrEmpty(cardOwner)) ? GetNewTotalScore(scoredButton, cardOwner, removeToken: true) : (-1));
			if (CheckForBustScore(playerScore))
			{
				HandlePlayerScoreBust(cardOwner, scoredButton.gameObject.name, stoleCardPlayerScore);
				return;
			}
			if (stoleCardPlayerScore != -1)
			{
				gameEvents.RaiseRemoveScoreboardPlayerToken(cardOwner, scoredButton.gameObject.name);
			}
			CardAnimationActions(placedCard, delegate
			{
				CallAfterAnimation(scoredButton.gameObject, playerScore, cardOwner, stoleCardPlayerScore);
			});
		}

		private bool CheckForBustScore(int newTotalScore)
		{
			if (newTotalScore <= 21)
			{
				return false;
			}
			foreach (ScoreToken item in gameState.InfiniteScoredGameScores[gameState.CurrentPlayer])
			{
				ResetCardSize(item.TokenName);
			}
			gameState.EnableTarget();
			return true;
		}

		private void CheckForWinningScore(int playerScore)
		{
			if (playerScore == 21)
			{
				gameEvents.RaiseGameOver();
				gameEvents.RaiseWinAnimationForPlayer(gameState.CurrentPlayer);
			}
		}

		private int GetPrimaryScoreTotal(ScoreToken hitToken, string playerName)
		{
			int num = 0;
			foreach (ScoreToken item in gameState.InfiniteScoredGameScores[playerName])
			{
				if (!(item.TokenName == hitToken.TokenName))
				{
					num += item.ScoreValue;
				}
			}
			return num;
		}

		private int GetNewTotalScore(TwoValueScoredButton scoredButton, string playerName, bool removeToken = false)
		{
			ScoreToken hitToken = CreateNewToken(scoredButton.gameObject);
			int num = GetPrimaryScoreTotal(hitToken, playerName);
			if (!removeToken)
			{
				num += scoredButton.Score;
			}
			int num2 = CalculateAlternateScoresIntoTotal(num, hitToken, playerName);
			if (scoredButton.SecondaryScore > 0 && num2 > 21 && !removeToken)
			{
				num2 = num2 - scoredButton.Score + scoredButton.SecondaryScore;
			}
			return num2;
		}

		private bool HandlePlayerHittingOwnCard(TwoValueScoredButton scoredButton, string cardOwner)
		{
			ResetCardSize(scoredButton.gameObject.name);
			int newTotalScore = GetNewTotalScore(scoredButton, cardOwner, removeToken: true);
			CheckForWinningScore(newTotalScore);
			gameEvents.RaiseRemovePlayerToken(gameState.CurrentPlayer, scoredButton.gameObject.name, newTotalScore);
			return true;
		}

		private void HandlePlayerScoreBust(string cardOwner, string tokenName, int stoleCardPlayerScore)
		{
			if (stoleCardPlayerScore != -1)
			{
				ResetCardSize(tokenName);
				gameEvents.RaiseUpdateTwoPlayersScores(gameState.CurrentPlayer, cardOwner, tokenName, stoleCardPlayerScore);
			}
			else
			{
				gameEvents.RaiseResetPlayerScore(gameState.CurrentPlayer);
			}
		}

		public IEnumerator ScaleUpAndBackDownOnClick_GameBoard(PlacedCard playingCard, Action afterAnimationCallback)
		{
			Blackjack21Controller blackjack21Controller = this;
			blackjack21Controller.multiDiplayController.SelectCard(playingCard.ScoringCard);
			RectTransform playingCardTransform = playingCard.GameboardCard.GetComponent<RectTransform>();
			Vector3 startPosition = playingCardTransform.localPosition;
			Vector3 startSize = playingCardTransform.localScale;
			Quaternion rotation = playingCardTransform.rotation;
			Vector3 endLocalPosition1 = new Vector3(startPosition.x, startPosition.y, -700f);
			Vector3 endScale1 = new Vector3(1800f, 1800f, startSize.z);
			Quaternion endRotation = Quaternion.Euler(0f, 180f, UnityEngine.Random.Range(-360, 360));
			Quaternion finalRotation = ((rotation == blackjack21Controller.FLIPPED_CARD_ROTATION) ? rotation : blackjack21Controller.FLIPPED_CARD_ROTATION);
			float duration = 10f;
			float animationCulling = 8f;
			blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpLocalAnimation(playingCard.ScoringCard.GetComponent<RectTransform>(), playingCard.ScoringCard.transform.localScale, playingCard.ScoringCard.transform.localPosition, finalRotation, duration, animationCulling));
			yield return blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpLocalAnimation(playingCardTransform, endScale1, endLocalPosition1, endRotation, duration, animationCulling));
			Vector3 endLocalPosition2 = startPosition;
			Vector3 endScale2 = ((startSize == blackjack21Controller._originalSize) ? (startSize / 1.2f) : startSize);
			Vector3 endScale3 = ((playingCard.ScoringCard.transform.localScale == blackjack21Controller._scoringCardOriginalSize) ? (playingCard.ScoringCard.transform.localScale / 1.2f) : playingCard.ScoringCard.transform.localScale);
			blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpLocalAnimation(playingCardTransform, endScale2, endLocalPosition2, finalRotation, duration, animationCulling));
			blackjack21Controller.StartCoroutine(blackjack21Controller._animationHelper.LerpLocalAnimation(playingCard.ScoringCard.GetComponent<RectTransform>(), endScale3, endLocalPosition2, finalRotation, duration, animationCulling));
			afterAnimationCallback();
		}
	}
}
