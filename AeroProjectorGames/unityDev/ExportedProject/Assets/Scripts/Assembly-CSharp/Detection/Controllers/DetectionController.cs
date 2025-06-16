using System.Collections.Concurrent;
using Detection.Commands;
using Detection.Models;
using Games;
using Games.GameState;
using HitEffects;
using Scoreboard;
using Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Detection.Controllers
{
	public class DetectionController : MonoBehaviour
	{
		[SerializeField]
		private Image dotImage;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private GetGameBoardCoordinatesFromRealWorldStrategy getGameBoardCoordinates;

		[SerializeField]
		private HitEffectEventsSO hitEffectsEvents;

		[SerializeField]
		private RectTransform gameBoardRectTransform;

		[SerializeField]
		private RayCastFactory rayCastFactory;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardEvents;

		[SerializeField]
		private SendClickEventFactory sendEventFactory;

		[SerializeField]
		private VectorEventHandler vectorEventHandler;

		private Camera _camera;

		private RectTransform _dotRectTransform;

		private EventSystem _eventSystem;

		private Vector2 _rectSize;

		private readonly ConcurrentStack<GameBoardCoordinateResponse> _workStack;

		public DetectionController()
		{
			_workStack = new ConcurrentStack<GameBoardCoordinateResponse>();
		}

		private void VectorEventHandlerOnOnObjectDetected(Vector3Int vector)
		{
			if (!gameState.IsTargetDisabled)
			{
				switch (SettingsStore.DetectionSettings.DetectedCamera)
				{
				case DetectedCameraEnum.OakD:
					_workStack.Push(new GameBoardCoordinateResponse
					{
						Status = GameBoardStatusEnum.Hit,
						GameBoardCoordinates = new Vector2Int(vector.x, vector.z)
					});
					break;
				case DetectedCameraEnum.RealSense:
				{
					Vector2Int realWorldCoordinates = new Vector2Int(vector.x, vector.z);
					GameBoardCoordinateResponse item = getGameBoardCoordinates.Execute(_rectSize, realWorldCoordinates);
					_workStack.Push(item);
					break;
				}
				}
			}
		}

		private void VectorEventHandlerOnOnObjectRemoved()
		{
			_workStack.Push(new GameBoardCoordinateResponse());
		}

		protected void OnDisable()
		{
			vectorEventHandler.OnObjectDetected -= VectorEventHandlerOnOnObjectDetected;
			vectorEventHandler.OnObjectRemoved -= VectorEventHandlerOnOnObjectRemoved;
		}

		private void OnEnable()
		{
			vectorEventHandler.OnObjectDetected += VectorEventHandlerOnOnObjectDetected;
			vectorEventHandler.OnObjectRemoved += VectorEventHandlerOnOnObjectRemoved;
			_camera = Camera.main;
			_dotRectTransform = dotImage.GetComponent<RectTransform>();
			_eventSystem = EventSystem.current;
			_rectSize = gameBoardRectTransform.rect.size;
		}

		private void Update()
		{
			_rectSize = gameBoardRectTransform.rect.size;
			if (_workStack.IsEmpty || !_workStack.TryPop(out var result))
			{
				return;
			}
			_workStack.Clear();
			if (!gameState.DetectionEnabled)
			{
				return;
			}
			switch (result.Status)
			{
			case GameBoardStatusEnum.Hit:
			{
				if (_dotRectTransform != null)
				{
					_dotRectTransform.anchoredPosition = new Vector2(result.GameBoardCoordinates.x, 0f - (float)result.GameBoardCoordinates.y);
				}
				RectTransform dotRectTransform = _dotRectTransform;
				Vector3 vector = ((dotRectTransform != null) ? dotRectTransform.position : Vector3.zero);
				Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(_camera, vector);
				PointerEventData pointerEventData = new PointerEventData(_eventSystem)
				{
					position = vector2
				};
				RaycastResult rayCasts = rayCastFactory.GetRayCasts(pointerEventData, vector);
				if (rayCasts.gameObject == null)
				{
					gameEvents.RaiseMissDetected(pointerEventData, vector2);
				}
				else
				{
					sendEventFactory.SendClick(rayCasts.gameObject, pointerEventData);
				}
				break;
			}
			case GameBoardStatusEnum.Miss:
				gameEvents.RaiseMissDetected();
				break;
			}
		}
	}
}
