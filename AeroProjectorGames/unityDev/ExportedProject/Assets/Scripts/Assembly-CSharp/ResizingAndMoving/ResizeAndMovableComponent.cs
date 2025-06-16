using System;
using UnityEngine;
using UnityEngine.Events;

namespace ResizingAndMoving
{
	public class ResizeAndMovableComponent : MonoBehaviour
	{
		private GameObject _loadedOverlay;

		[SerializeField]
		private GameObject highlightArea;

		[SerializeField]
		private SizeAndPositionStateSO sizeAndPositionState;

		[SerializeField]
		private ResizeMethod resizeMethod;

		private void OnEnable()
		{
			RemoveHighlight();
			UpdatePosition();
			UpdateSize();
			SizeAndPositionStateSO sizeAndPositionStateSO = sizeAndPositionState;
			sizeAndPositionStateSO.OnDoneEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO.OnDoneEditing, new UnityAction(RemoveHighlight));
			SizeAndPositionStateSO sizeAndPositionStateSO2 = sizeAndPositionState;
			sizeAndPositionStateSO2.OnEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO2.OnEditing, new UnityAction(HighlightArea));
			SizeAndPositionStateSO sizeAndPositionStateSO3 = sizeAndPositionState;
			sizeAndPositionStateSO3.OnPositionChange = (UnityAction)Delegate.Combine(sizeAndPositionStateSO3.OnPositionChange, new UnityAction(UpdatePosition));
			SizeAndPositionStateSO sizeAndPositionStateSO4 = sizeAndPositionState;
			sizeAndPositionStateSO4.OnSizeChange = (UnityAction)Delegate.Combine(sizeAndPositionStateSO4.OnSizeChange, new UnityAction(UpdateSize));
		}

		private void OnDisable()
		{
			SizeAndPositionStateSO sizeAndPositionStateSO = sizeAndPositionState;
			sizeAndPositionStateSO.OnDoneEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO.OnDoneEditing, new UnityAction(RemoveHighlight));
			SizeAndPositionStateSO sizeAndPositionStateSO2 = sizeAndPositionState;
			sizeAndPositionStateSO2.OnEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO2.OnEditing, new UnityAction(HighlightArea));
			SizeAndPositionStateSO sizeAndPositionStateSO3 = sizeAndPositionState;
			sizeAndPositionStateSO3.OnPositionChange = (UnityAction)Delegate.Remove(sizeAndPositionStateSO3.OnPositionChange, new UnityAction(UpdatePosition));
			SizeAndPositionStateSO sizeAndPositionStateSO4 = sizeAndPositionState;
			sizeAndPositionStateSO4.OnSizeChange = (UnityAction)Delegate.Remove(sizeAndPositionStateSO4.OnSizeChange, new UnityAction(UpdateSize));
		}

		private void HighlightArea()
		{
			if (highlightArea != null)
			{
				_loadedOverlay = UnityEngine.Object.Instantiate(highlightArea, base.gameObject.transform);
				Canvas canvas = _loadedOverlay.AddComponent<Canvas>();
				canvas.overrideSorting = true;
				canvas.sortingLayerName = "Edit Size and Position";
				canvas.sortingOrder = -1;
			}
		}

		private void RemoveHighlight()
		{
			if (highlightArea != null)
			{
				UnityEngine.Object.Destroy(_loadedOverlay);
			}
		}

		private void UpdatePosition()
		{
			GetComponent<RectTransform>().anchoredPosition = new Vector2(sizeAndPositionState.PositionX, sizeAndPositionState.PositionY);
		}

		private void UpdateSize()
		{
			switch (resizeMethod)
			{
			case ResizeMethod.RectTransform:
			{
				RectTransform component2 = GetComponent<RectTransform>();
				component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeAndPositionState.Width);
				component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeAndPositionState.Height);
				break;
			}
			case ResizeMethod.Scale:
				base.gameObject.transform.localScale = new Vector3(sizeAndPositionState.Width, sizeAndPositionState.Height, base.gameObject.transform.localScale.z);
				break;
			case ResizeMethod.ScaleRectTransform:
			{
				RectTransform component = GetComponent<RectTransform>();
				float x = sizeAndPositionState.Width / component.rect.width;
				float y = sizeAndPositionState.Height / component.rect.height;
				component.localScale = new Vector3(x, y, component.localScale.z);
				break;
			}
			}
		}
	}
}
