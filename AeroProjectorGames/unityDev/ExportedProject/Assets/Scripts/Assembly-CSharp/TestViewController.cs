using System.Collections.Concurrent;
using Detection.Commands;
using Detection.Models;
using Detection.ScriptableObjects;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TestViewController : MonoBehaviour
{
	[SerializeField]
	private RealSenseCameraSettingsSO cameraSettings;

	[SerializeField]
	private RawImage dotImage;

	[SerializeField]
	private TMP_Text gameBoardLabel;

	[SerializeField]
	private RectTransform gameBoardRectTransform;

	[SerializeField]
	private GetGameBoardCoordinatesFromRealWorldStrategy getGameBoardCoordinates;

	[SerializeField]
	private Image testViewGameboardBackground;

	[SerializeField]
	private TMP_Text realWorldLabel;

	[SerializeField]
	private TMP_Text statusLabel;

	[SerializeField]
	private VectorEventHandler vectorEventHandler;

	private RectTransform _dotRectTransform;

	private Vector2 _rectSize;

	private bool _targetEnabled;

	private readonly ConcurrentStack<GameBoardCoordinateResponse> _workStack;

	private const string DEFAULT_COORDINATES = "(0, 0)";

	private const string STATUS_HIT = "Hit";

	private const string STATUS_NOT_DETECTED = "Object not detected";

	private const string STATUS_OFF_BOARD = "Miss";

	private const string STATUS_UNKNOWN = "Status unknown";

	public TestViewController()
	{
		_workStack = new ConcurrentStack<GameBoardCoordinateResponse>();
	}

	private void VectorEventHandlerOnObjectDetected(Vector3Int vector)
	{
		Vector2Int realWorldCoordinates = new Vector2Int(vector.x, vector.z);
		GameBoardCoordinateResponse item = getGameBoardCoordinates.Execute(_rectSize, realWorldCoordinates, allowOutOfBounds: true);
		_workStack.Push(item);
	}

	private void VectorEventHandlerOnObjectRemoved()
	{
		_workStack.Push(new GameBoardCoordinateResponse());
	}

	private void CameraProcessingProfileChanged()
	{
		if (cameraSettings.SelectedProcessingProfileEnum == ProcessingProfileEnum.PollingProcessingProfile)
		{
			dotImage.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
		}
	}

	private void OnDisable()
	{
		vectorEventHandler.OnObjectDetected -= VectorEventHandlerOnObjectDetected;
		vectorEventHandler.OnObjectRemoved -= VectorEventHandlerOnObjectRemoved;
		cameraSettings.OnProcessingProfileChanged -= CameraProcessingProfileChanged;
		dotImage.enabled = false;
	}

	private void OnEnable()
	{
		vectorEventHandler.OnObjectDetected += VectorEventHandlerOnObjectDetected;
		vectorEventHandler.OnObjectRemoved += VectorEventHandlerOnObjectRemoved;
		cameraSettings.OnProcessingProfileChanged += CameraProcessingProfileChanged;
		Color backgroundColor = SettingsStore.Gameboard.GetBackgroundColor();
		testViewGameboardBackground.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);
		_dotRectTransform = dotImage.GetComponent<RectTransform>();
		_rectSize = gameBoardRectTransform.rect.size;
	}

	private void Start()
	{
		ResetLabels();
	}

	private void Update()
	{
		if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.digit0Key.wasPressedThisFrame)
		{
			_targetEnabled = !_targetEnabled;
		}
		if (!_targetEnabled)
		{
			return;
		}
		_rectSize = gameBoardRectTransform.rect.size;
		if (_workStack.IsEmpty || !_workStack.TryPop(out var result))
		{
			return;
		}
		_workStack.Clear();
		GameBoardStatusEnum status = result.Status;
		if (status - 1 <= GameBoardStatusEnum.Miss)
		{
			dotImage.enabled = true;
			gameBoardLabel.text = result.GameBoardCoordinates.ToString();
			realWorldLabel.text = result.RealWorldCoordinates.ToString();
			if (_dotRectTransform != null)
			{
				_dotRectTransform.anchoredPosition = new Vector2(result.GameBoardCoordinates.x, 0f - (float)result.GameBoardCoordinates.y);
			}
		}
		else
		{
			ResetLabels();
		}
		statusLabel.text = GetStatusText(result.Status);
	}

	private string GetStatusText(GameBoardStatusEnum status)
	{
		switch (status)
		{
		case GameBoardStatusEnum.ObjectNotDetected:
			return "Object not detected";
		case GameBoardStatusEnum.Miss:
			return "Miss";
		case GameBoardStatusEnum.Hit:
			return "Hit";
		default:
			return "Status unknown";
		}
	}

	private void ResetLabels()
	{
		dotImage.enabled = false;
		gameBoardLabel.text = "(0, 0)";
		realWorldLabel.text = "(0, 0)";
		statusLabel.text = "Object not detected";
	}
}
