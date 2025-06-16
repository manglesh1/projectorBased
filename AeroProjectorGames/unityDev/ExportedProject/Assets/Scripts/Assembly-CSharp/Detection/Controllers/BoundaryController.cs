using Detection.Commands;
using Detection.Models;
using Detection.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Detection.Controllers
{
	public class BoundaryController : MonoBehaviour
	{
		private const int MAX_X = 1248;

		private const int MAX_Y = 10000;

		private const int MIN_X = 0;

		private const int MIN_Y = 0;

		[Header("Boundary Buttons")]
		[SerializeField]
		private Button ButtonBottomLeft;

		[SerializeField]
		private Button ButtonBottomRight;

		[SerializeField]
		private Button ButtonTopLeft;

		[SerializeField]
		private Button ButtonTopRight;

		[Header("Boundary Labels")]
		[SerializeField]
		private TMP_Text LabelLiveCoordinates;

		[SerializeField]
		private TMP_Text LabelStatus;

		[Header("Boundary Coordinate Inputs")]
		[SerializeField]
		private TMP_InputField InputBottomLeftX;

		[SerializeField]
		private TMP_InputField InputBottomLeftY;

		[SerializeField]
		private TMP_InputField InputBottomRightX;

		[SerializeField]
		private TMP_InputField InputBottomRightY;

		[SerializeField]
		private TMP_InputField InputTopLeftX;

		[SerializeField]
		private TMP_InputField InputTopLeftY;

		[SerializeField]
		private TMP_InputField InputTopRightX;

		[SerializeField]
		private TMP_InputField InputTopRightY;

		[Header("Commands")]
		[SerializeField]
		private SetBoundaryCornerCommand SetBoundaryCornerCommand;

		[SerializeField]
		private VectorEventHandler VectorHandler;

		[Header("Settings")]
		[SerializeField]
		private CoordinatesSO GameBoardCoordinates;

		private string _coordinates;

		private void VectorHandler_OnObjectDetected(Vector3Int vector)
		{
			_coordinates = new Vector2Int(vector.x, vector.z).ToString();
		}

		private void VectorHandler_OnObjectRemoved()
		{
			_coordinates = Vector2Int.zero.ToString();
		}

		private void OnDisable()
		{
			ButtonBottomLeft.onClick.RemoveListener(ButtonBottomLeftClicked);
			ButtonBottomRight.onClick.RemoveListener(ButtonBottomRightClicked);
			ButtonTopLeft.onClick.RemoveListener(ButtonTopLeftClicked);
			ButtonTopRight.onClick.RemoveListener(ButtonTopRightClicked);
			GameBoardCoordinates.OnBottomLeftChanged -= HandleBottomLeftChanged;
			GameBoardCoordinates.OnBottomRightChanged -= HandleBottomRightChanged;
			GameBoardCoordinates.OnTopLeftChanged -= HandleTopLeftChanged;
			GameBoardCoordinates.OnTopRightChanged -= HandleTopRightChanged;
			InputBottomLeftX.onValueChanged.RemoveListener(HandleBottomLeftXChanged);
			InputBottomLeftY.onValueChanged.RemoveListener(HandleBottomLeftYChanged);
			InputBottomRightX.onValueChanged.RemoveListener(HandleBottomRightXChanged);
			InputBottomRightY.onValueChanged.RemoveListener(HandleBottomRightYChanged);
			InputTopLeftX.onValueChanged.RemoveListener(HandleTopLeftXChanged);
			InputTopLeftY.onValueChanged.RemoveListener(HandleTopLeftYChanged);
			InputTopRightX.onValueChanged.RemoveListener(HandleTopRightXChanged);
			InputTopRightY.onValueChanged.RemoveListener(HandleTopRightYChanged);
			VectorHandler.OnObjectDetected -= VectorHandler_OnObjectDetected;
			VectorHandler.OnObjectRemoved -= VectorHandler_OnObjectRemoved;
		}

		private void OnEnable()
		{
			InputBottomLeftX.SetTextWithoutNotify(GameBoardCoordinates.BottomLeft.x.ToString());
			InputBottomLeftY.SetTextWithoutNotify(GameBoardCoordinates.BottomLeft.y.ToString());
			InputBottomRightX.SetTextWithoutNotify(GameBoardCoordinates.BottomRight.x.ToString());
			InputBottomRightY.SetTextWithoutNotify(GameBoardCoordinates.BottomRight.y.ToString());
			InputTopLeftX.SetTextWithoutNotify(GameBoardCoordinates.TopLeft.x.ToString());
			InputTopLeftY.SetTextWithoutNotify(GameBoardCoordinates.TopLeft.y.ToString());
			InputTopRightX.SetTextWithoutNotify(GameBoardCoordinates.TopRight.x.ToString());
			InputTopRightY.SetTextWithoutNotify(GameBoardCoordinates.TopRight.y.ToString());
			ButtonBottomLeft.onClick.AddListener(ButtonBottomLeftClicked);
			ButtonBottomRight.onClick.AddListener(ButtonBottomRightClicked);
			ButtonTopLeft.onClick.AddListener(ButtonTopLeftClicked);
			ButtonTopRight.onClick.AddListener(ButtonTopRightClicked);
			GameBoardCoordinates.OnBottomLeftChanged += HandleBottomLeftChanged;
			GameBoardCoordinates.OnBottomRightChanged += HandleBottomRightChanged;
			GameBoardCoordinates.OnTopLeftChanged += HandleTopLeftChanged;
			GameBoardCoordinates.OnTopRightChanged += HandleTopRightChanged;
			InputBottomLeftX.onValueChanged.AddListener(HandleBottomLeftXChanged);
			InputBottomLeftY.onValueChanged.AddListener(HandleBottomLeftYChanged);
			InputBottomRightX.onValueChanged.AddListener(HandleBottomRightXChanged);
			InputBottomRightY.onValueChanged.AddListener(HandleBottomRightYChanged);
			InputTopLeftX.onValueChanged.AddListener(HandleTopLeftXChanged);
			InputTopLeftY.onValueChanged.AddListener(HandleTopLeftYChanged);
			InputTopRightX.onValueChanged.AddListener(HandleTopRightXChanged);
			InputTopRightY.onValueChanged.AddListener(HandleTopRightYChanged);
			VectorHandler.OnObjectDetected += VectorHandler_OnObjectDetected;
			VectorHandler.OnObjectRemoved += VectorHandler_OnObjectRemoved;
			LabelStatus.text = string.Empty;
		}

		private void Update()
		{
			LabelLiveCoordinates.text = _coordinates;
		}

		private void ButtonBottomLeftClicked()
		{
			SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.BottomLeft);
		}

		private void ButtonBottomRightClicked()
		{
			SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.BottomRight);
		}

		private void ButtonTopLeftClicked()
		{
			SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.TopLeft);
		}

		private void ButtonTopRightClicked()
		{
			SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.TopRight);
		}

		private void HandleBottomLeftChanged(Vector2Int value)
		{
			InputBottomLeftX.text = value.x.ToString();
			InputBottomLeftY.text = value.y.ToString();
		}

		private void HandleBottomLeftXChanged(string value)
		{
			if (int.TryParse(value, out var result))
			{
				if (result < 0)
				{
					result = 0;
					InputBottomLeftX.SetTextWithoutNotify(result.ToString());
				}
				if (result > 1248)
				{
					result = 1248;
					InputBottomLeftX.SetTextWithoutNotify(result.ToString());
				}
				SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.BottomLeft, new Vector2Int(result, GameBoardCoordinates.BottomLeft.y));
			}
		}

		private void HandleBottomLeftYChanged(string value)
		{
			if (int.TryParse(value, out var result))
			{
				if (result < 0)
				{
					result = 0;
					InputBottomLeftY.SetTextWithoutNotify(result.ToString());
				}
				if (result > 10000)
				{
					result = 10000;
					InputBottomLeftY.SetTextWithoutNotify(result.ToString());
				}
				SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.BottomLeft, new Vector2Int(GameBoardCoordinates.BottomLeft.x, result));
			}
		}

		private void HandleBottomRightChanged(Vector2Int value)
		{
			InputBottomRightX.text = value.x.ToString();
			InputBottomRightY.text = value.y.ToString();
		}

		private void HandleBottomRightXChanged(string value)
		{
			if (int.TryParse(value, out var result))
			{
				if (result < 0)
				{
					result = 0;
					InputBottomRightX.SetTextWithoutNotify(result.ToString());
				}
				if (result > 1248)
				{
					result = 1248;
					InputBottomRightX.SetTextWithoutNotify(result.ToString());
				}
				SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.BottomRight, new Vector2Int(result, GameBoardCoordinates.BottomRight.y));
			}
		}

		private void HandleBottomRightYChanged(string value)
		{
			if (int.TryParse(value, out var result))
			{
				if (result < 0)
				{
					result = 0;
					InputBottomRightY.SetTextWithoutNotify(result.ToString());
				}
				if (result > 10000)
				{
					result = 10000;
					InputBottomRightY.SetTextWithoutNotify(result.ToString());
				}
				SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.BottomRight, new Vector2Int(GameBoardCoordinates.BottomRight.x, result));
			}
		}

		private void HandleTopLeftChanged(Vector2Int value)
		{
			InputTopLeftX.text = value.x.ToString();
			InputTopLeftY.text = value.y.ToString();
		}

		private void HandleTopLeftXChanged(string value)
		{
			if (int.TryParse(value, out var result))
			{
				if (result < 0)
				{
					result = 0;
					InputTopLeftX.SetTextWithoutNotify(result.ToString());
				}
				if (result > 1248)
				{
					result = 1248;
					InputTopLeftX.SetTextWithoutNotify(result.ToString());
				}
				SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.TopLeft, new Vector2Int(result, GameBoardCoordinates.TopLeft.y));
			}
		}

		private void HandleTopLeftYChanged(string value)
		{
			if (int.TryParse(value, out var result))
			{
				if (result < 0)
				{
					result = 0;
					InputTopLeftY.SetTextWithoutNotify(result.ToString());
				}
				if (result > 10000)
				{
					result = 10000;
					InputTopLeftY.SetTextWithoutNotify(result.ToString());
				}
				SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.TopLeft, new Vector2Int(GameBoardCoordinates.TopLeft.x, result));
			}
		}

		private void HandleTopRightChanged(Vector2Int value)
		{
			InputTopRightX.text = value.x.ToString();
			InputTopRightY.text = value.y.ToString();
		}

		private void HandleTopRightXChanged(string value)
		{
			if (int.TryParse(value, out var result))
			{
				if (result < 0)
				{
					result = 0;
					InputTopRightX.SetTextWithoutNotify(result.ToString());
				}
				if (result > 1248)
				{
					result = 1248;
					InputTopRightX.SetTextWithoutNotify(result.ToString());
				}
				SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.TopRight, new Vector2Int(result, GameBoardCoordinates.TopRight.y));
			}
		}

		private void HandleTopRightYChanged(string value)
		{
			if (int.TryParse(value, out var result))
			{
				if (result < 0)
				{
					result = 0;
					InputTopRightY.SetTextWithoutNotify(result.ToString());
				}
				if (result > 10000)
				{
					result = 10000;
					InputTopRightY.SetTextWithoutNotify(result.ToString());
				}
				SetBoundaryCornerCommand.Execute(BoundaryCornerEnum.TopRight, new Vector2Int(GameBoardCoordinates.TopRight.x, result));
			}
		}
	}
}
