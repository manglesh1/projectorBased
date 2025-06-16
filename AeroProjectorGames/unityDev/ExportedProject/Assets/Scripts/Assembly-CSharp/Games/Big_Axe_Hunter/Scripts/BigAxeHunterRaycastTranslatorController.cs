using System;
using Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Games.Big_Axe_Hunter.Scripts
{
	public class BigAxeHunterRaycastTranslatorController : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private Camera _cam;

		private RawImage _r;

		[SerializeField]
		private BigAxeHunterEventsSO bahEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnDisable()
		{
			bahEvents.OnSendGameboardCameraToGame -= AssignCamera;
		}

		private void OnEnable()
		{
			bahEvents.OnSendGameboardCameraToGame += AssignCamera;
			_r = GetComponent<RawImage>();
		}

		private void AssignCamera(Camera cam)
		{
			_cam = cam;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_cam == null)
			{
				throw new ArgumentException("Camera was not provided for raycast translation.");
			}
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_r.rectTransform, eventData.position, Camera.main, out var localPoint))
			{
				return;
			}
			Rect rect = _r.rectTransform.rect;
			float num = (localPoint.x - rect.x) / rect.width;
			float num2 = (localPoint.y - rect.y) / rect.height;
			float x = Mathf.Clamp01(num);
			float y = Mathf.Clamp01(num2);
			Vector3 vector = new Vector3(num * (float)_cam.pixelWidth, num2 * (float)_cam.pixelHeight);
			Ray ray = _cam.ScreenPointToRay(vector);
			_cam.ViewportPointToRay(new Vector3(x, y));
			if (!Physics.SphereCast(ray, 0.05f, out var hitInfo))
			{
				if (SettingsStore.DetectionSettings.IsDetectionOnAndEnabled)
				{
					gameEvents.RaiseMissDetected(eventData);
				}
			}
			else
			{
				hitInfo.transform.GetComponent<IPointerDownHandler>()?.OnPointerDown(new PointerEventData(EventSystem.current)
				{
					position = vector
				});
			}
		}
	}
}
