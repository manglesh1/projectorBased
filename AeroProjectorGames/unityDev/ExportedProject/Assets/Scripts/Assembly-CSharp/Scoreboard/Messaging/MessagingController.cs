using System;
using Games;
using UnityEngine;

namespace Scoreboard.Messaging
{
	public class MessagingController : MonoBehaviour
	{
		private ScoreboardMessageRequest _currentRequest;

		private MessageStyleAnimationController _currentController;

		[Header("Scoreboard Loader Events")]
		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoaderEvents;

		[Header("Message Style Containers")]
		[SerializeField]
		private MessageStyleAnimationController normalStyle;

		[Header("Game Events")]
		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnDisable()
		{
			normalStyle.OnAnimationComplete -= HandleAnimationComplete;
			scoreboardLoaderEvents.OnScoreboardMessageRequest -= HandleScoreboardMessageRequest;
			scoreboardLoaderEvents.OnUnloadScoreboardRequest -= HandleAnimationComplete;
			gameEvents.OnMainMenu -= Reset;
			gameEvents.OnGameLoading -= Reset;
		}

		private void OnEnable()
		{
			_currentRequest = null;
			normalStyle.gameObject.SetActive(value: false);
			normalStyle.OnAnimationComplete += HandleAnimationComplete;
			scoreboardLoaderEvents.OnScoreboardMessageRequest += HandleScoreboardMessageRequest;
			gameEvents.OnMainMenu += Reset;
			gameEvents.OnGameLoading += Reset;
		}

		private void CleanUp()
		{
			_currentRequest = null;
			_currentController = null;
		}

		private void HandleScoreboardMessageRequest(ScoreboardMessageRequest request)
		{
			_currentRequest = request;
			_currentController = GetMessageStylePanel(request.MessageStyle).GetComponent<MessageStyleAnimationController>();
			_currentController.SetMessage(request.MessageText);
			_currentController.Activate();
		}

		private void HandleAnimationComplete()
		{
			_currentRequest?.MessageCompleteAction?.Invoke();
			Reset();
			scoreboardLoaderEvents.RaiseScoreboardMessageFinished();
		}

		private void Reset()
		{
			normalStyle.SetMessage(string.Empty);
			normalStyle.gameObject.SetActive(value: false);
			CleanUp();
		}

		private MessageStyleAnimationController GetMessageStylePanel(ScoreboardMessageStyle style)
		{
			if (style == ScoreboardMessageStyle.Normal)
			{
				return normalStyle;
			}
			throw new InvalidOperationException($"Unexpected GetMessageStylePanel value: {style}");
		}
	}
}
