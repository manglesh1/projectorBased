using System.Collections.Generic;
using System.Linq;
using Detection.Commands;
using Detection.Models;
using Detection.ScriptableObjects;
using Intel.RealSense;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Detection.Controllers
{
	public class PointClickBoundaryController : MonoBehaviour
	{
		[Header("UI Buttons")]
		[SerializeField]
		private Button ButtonBottomLeft;

		[SerializeField]
		private Button ButtonBottomRight;

		[SerializeField]
		private Button ButtonTopLeft;

		[SerializeField]
		private Button ButtonTopRight;

		[Header("UI Labels")]
		[SerializeField]
		private TMP_Text LabelBottomLeft;

		[SerializeField]
		private TMP_Text LabelBottomRight;

		[SerializeField]
		private TMP_Text LabelLiveCoordinates;

		[SerializeField]
		private TMP_Text LabelTopLeft;

		[SerializeField]
		private TMP_Text LabelTopRight;

		[Header("Commands")]
		[SerializeField]
		private GetRealSenseFrameDataCommand GetRealSenseFrameDataCommand;

		[SerializeField]
		private SetBoundaryCornerCommand SetBoundaryCorner;

		[Header("Scriptable Objects")]
		[SerializeField]
		private CameraEventHandler CameraEventHandler;

		[SerializeField]
		private CoordinatesSO GameBoardCoordinates;

		private string _coordinates;

		private Vector2Int _vector;

		private void CameraEventHandler_OnClicked(Vector2Int point)
		{
			DepthFrame depthFrame = GetRealSenseFrameDataCommand.Execute().FirstOrDefault();
			depthFrame.ToVector3IntArray().Where(delegate(Vector3Int v)
			{
				Vector3Int vector3Int2 = v;
				return vector3Int2.z > 0;
			}).OrderBy(delegate(Vector3Int v)
			{
				Vector3Int vector3Int2 = v;
				return vector3Int2.x;
			})
				.ThenBy(delegate(Vector3Int v)
				{
					Vector3Int vector3Int2 = v;
					return vector3Int2.y;
				})
				.ToList();
			Vector3Int vector3Int;
			if (depthFrame != null)
			{
				List<Vector3Int> list = new List<Vector3Int>(7);
				for (int num = 0; num < 5; num++)
				{
					for (int num2 = 0; num2 < 5; num2++)
					{
						int x = point.x + num;
						int y = point.y + num2;
						list.Add(new Vector3Int(x, y, depthFrame.GetDistanceInMillimeters(x, y)));
					}
				}
				vector3Int = list.FirstOrDefault((Vector3Int v) => v.z > 0);
			}
			else
			{
				vector3Int = Vector3Int.zero;
			}
			_vector = new Vector2Int(vector3Int.x, vector3Int.z);
			_coordinates = vector3Int.ToString();
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
			CameraEventHandler.OnClicked -= CameraEventHandler_OnClicked;
		}

		private void OnEnable()
		{
			ButtonBottomLeft.onClick.AddListener(ButtonBottomLeftClicked);
			ButtonBottomRight.onClick.AddListener(ButtonBottomRightClicked);
			ButtonTopLeft.onClick.AddListener(ButtonTopLeftClicked);
			ButtonTopRight.onClick.AddListener(ButtonTopRightClicked);
			GameBoardCoordinates.OnBottomLeftChanged += HandleBottomLeftChanged;
			GameBoardCoordinates.OnBottomRightChanged += HandleBottomRightChanged;
			GameBoardCoordinates.OnTopLeftChanged += HandleTopLeftChanged;
			GameBoardCoordinates.OnTopRightChanged += HandleTopRightChanged;
			CameraEventHandler.OnClicked += CameraEventHandler_OnClicked;
		}

		private void Start()
		{
			LabelTopLeft.text = GameBoardCoordinates.TopLeft.ToString();
			LabelTopRight.text = GameBoardCoordinates.TopRight.ToString();
			LabelBottomLeft.text = GameBoardCoordinates.BottomLeft.ToString();
			LabelBottomRight.text = GameBoardCoordinates.BottomRight.ToString();
		}

		private void Update()
		{
			LabelLiveCoordinates.text = _coordinates;
		}

		private void ButtonBottomLeftClicked()
		{
			if (_vector != Vector2Int.zero)
			{
				SetBoundaryCorner.Execute(BoundaryCornerEnum.BottomLeft);
				ResetVectorCoordinates();
			}
		}

		private void ButtonBottomRightClicked()
		{
			if (_vector != Vector2Int.zero)
			{
				SetBoundaryCorner.Execute(BoundaryCornerEnum.BottomRight);
				ResetVectorCoordinates();
			}
		}

		private void ButtonTopLeftClicked()
		{
			if (_vector != Vector2Int.zero)
			{
				SetBoundaryCorner.Execute(BoundaryCornerEnum.TopLeft);
				ResetVectorCoordinates();
			}
		}

		private void ButtonTopRightClicked()
		{
			if (_vector != Vector2Int.zero)
			{
				SetBoundaryCorner.Execute(BoundaryCornerEnum.TopRight);
				ResetVectorCoordinates();
			}
		}

		private void HandleTopRightChanged(Vector2Int value)
		{
			LabelTopRight.text = value.ToString();
		}

		private void HandleBottomLeftChanged(Vector2Int value)
		{
			LabelBottomLeft.text = value.ToString();
		}

		private void HandleBottomRightChanged(Vector2Int value)
		{
			LabelBottomRight.text = value.ToString();
		}

		private void HandleTopLeftChanged(Vector2Int value)
		{
			LabelTopLeft.text = value.ToString();
		}

		private void ResetVectorCoordinates()
		{
			_vector = Vector2Int.zero;
			_coordinates = Vector3Int.zero.ToString();
		}
	}
}
