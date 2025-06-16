using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Detection.Controllers
{
	public class DetectionCanvasController : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text calibrationStatusText;

		[SerializeField]
		private RawImage dotImage;

		[SerializeField]
		private GameObject touchPointBottomLeft;

		[SerializeField]
		private GameObject touchPointBottomRight;

		[SerializeField]
		private GameObject touchPointTopLeft;

		[SerializeField]
		private GameObject touchPointTopRight;

		private void HideFields()
		{
			calibrationStatusText.text = string.Empty;
			dotImage.enabled = false;
			touchPointBottomLeft.SetActive(value: false);
			touchPointBottomRight.SetActive(value: false);
			touchPointTopLeft.SetActive(value: false);
			touchPointTopRight.SetActive(value: false);
		}

		private void OnDisable()
		{
			HideFields();
		}

		private void OnEnable()
		{
			HideFields();
		}
	}
}
