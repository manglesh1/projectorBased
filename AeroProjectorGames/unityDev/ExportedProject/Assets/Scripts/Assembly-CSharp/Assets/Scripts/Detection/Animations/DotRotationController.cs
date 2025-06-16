using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Detection.Animations
{
	[RequireComponent(typeof(Image))]
	public class DotRotationController : MonoBehaviour
	{
		private const float ANGLE = -90f;

		private static readonly Vector3 AXIS = Vector3.forward;

		private const float DURATION = 1f;

		private Image _image;

		private IEnumerator Rotate()
		{
			while (true)
			{
				float elapsed = 0f;
				Quaternion startRotation = _image.transform.rotation;
				Quaternion endRotation = Quaternion.AngleAxis(-90f, AXIS) * startRotation;
				while (elapsed < 1f)
				{
					float t = elapsed / 1f;
					_image.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
					elapsed += Time.deltaTime;
					yield return null;
				}
				_image.transform.rotation = endRotation;
				yield return null;
			}
		}

		private void OnDisable()
		{
			StopCoroutine(Rotate());
		}

		private void OnEnable()
		{
			_image = GetComponent<Image>();
			StartCoroutine(Rotate());
		}
	}
}
