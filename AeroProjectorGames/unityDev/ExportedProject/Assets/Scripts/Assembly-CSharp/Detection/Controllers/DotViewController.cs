using Detection.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace Detection.Controllers
{
	public class DotViewController : MonoBehaviour
	{
		private RectTransform _dotRect;

		[SerializeField]
		private VectorEventHandler vectorEventHandler;

		[Header("Images")]
		[SerializeField]
		private RawImage dot;

		[Header("Gameboard Objects")]
		[SerializeField]
		private RectTransform gameBoardRectTransform;

		private void OnDisable()
		{
			vectorEventHandler.OnObjectDetected -= HandleObjectDetected;
		}

		private void OnEnable()
		{
			_dotRect = dot.GetComponent<RectTransform>();
			vectorEventHandler.OnObjectDetected += HandleObjectDetected;
		}

		private void HandleObjectDetected(Vector3Int vect)
		{
			_dotRect.anchoredPosition = new Vector3(vect.x, 0f - (float)vect.z, -2f);
		}
	}
}
