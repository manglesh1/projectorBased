using UnityEngine;

namespace MText
{
	[DisallowMultipleComponent]
	public class MText_UI_RaycastSelector : MonoBehaviour
	{
		[Tooltip("If not assigned, it will automatically get Camera.main on Start")]
		public Camera myCamera;

		[SerializeField]
		private LayerMask UILayer = -1;

		[SerializeField]
		private float maxDistance = 5000f;

		[Space]
		[Tooltip("True = How normal UI works. It toggles if clicking a inputfield enables it and clicking somewhere else disables it")]
		public bool onlyOneTargetFocusedAtOnce = true;

		[Tooltip("Unhovering mouse from a Btn will unselect it")]
		public bool unselectBtnOnUnhover = true;

		private Transform currentTarget;

		private Transform clickedTarget;

		private bool dragging;

		private void Start()
		{
			if (!myCamera)
			{
				myCamera = Camera.main;
			}
		}

		private void Update()
		{
			if (!myCamera)
			{
				return;
			}
			if (dragging)
			{
				Dragging(currentTarget);
				DetectDragEnd();
				return;
			}
			Transform transform = RaycastCheck();
			if (transform != currentTarget)
			{
				UnSelectOldTarget(currentTarget);
			}
			if ((bool)transform)
			{
				if (transform != currentTarget)
				{
					SelectNewTarget(transform);
				}
				if (Input.GetMouseButtonDown(0))
				{
					PressTarget(transform);
					dragging = true;
				}
			}
			currentTarget = transform;
		}

		private Transform RaycastCheck()
		{
			if (Physics.Raycast(myCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo, maxDistance, UILayer))
			{
				return hitInfo.transform;
			}
			return null;
		}

		private void PressTarget(Transform hit)
		{
			if (onlyOneTargetFocusedAtOnce)
			{
				UnFocusPreviouslySelectedItems(hit);
			}
			PressInputString(hit);
			PressButton(hit);
			PressSlider(hit);
		}

		private void PressInputString(Transform hit)
		{
			Mtext_UI_InputField component = hit.gameObject.GetComponent<Mtext_UI_InputField>();
			if (InteractWithInputString(component))
			{
				component.Select();
				clickedTarget = hit;
			}
		}

		private void PressSlider(Transform hit)
		{
			MText_UI_SliderHandle component = hit.gameObject.GetComponent<MText_UI_SliderHandle>();
			if (InteractWithSlider(component))
			{
				hit.gameObject.GetComponent<MText_UI_SliderHandle>().slider.ClickedVisual();
			}
		}

		private void PressButton(Transform hit)
		{
			MText_UI_Button component = hit.gameObject.GetComponent<MText_UI_Button>();
			if (InteractWithButton(component))
			{
				component.PressButtonClick();
			}
		}

		private void UnFocusPreviouslySelectedItems(Transform hit)
		{
			if (hit != clickedTarget && (bool)clickedTarget && (bool)clickedTarget.gameObject.GetComponent<Mtext_UI_InputField>() && clickedTarget.gameObject.GetComponent<Mtext_UI_InputField>().interactable)
			{
				clickedTarget.gameObject.GetComponent<Mtext_UI_InputField>().Focus(enable: false);
			}
		}

		private void SelectNewTarget(Transform hit)
		{
			SelectButton(hit);
			SelectSlider(hit);
		}

		private void SelectSlider(Transform hit)
		{
			MText_UI_SliderHandle component = hit.gameObject.GetComponent<MText_UI_SliderHandle>();
			if (InteractWithSlider(component))
			{
				component.slider.SelectedVisual();
			}
		}

		private void SelectButton(Transform hit)
		{
			MText_UI_Button component = hit.gameObject.GetComponent<MText_UI_Button>();
			if (InteractWithButton(component))
			{
				component.SelectButton();
			}
		}

		private void UnSelectOldTarget(Transform hit)
		{
			if ((bool)hit)
			{
				UnselectButton(hit);
				UnselectSlider(hit);
			}
		}

		private void UnselectSlider(Transform hit)
		{
			MText_UI_SliderHandle component = hit.gameObject.GetComponent<MText_UI_SliderHandle>();
			if (InteractWithSlider(component))
			{
				component.slider.UnSelectedVisual();
			}
		}

		private void UnselectButton(Transform hit)
		{
			MText_UI_Button component = hit.gameObject.GetComponent<MText_UI_Button>();
			if (!InteractWithButton(component))
			{
				return;
			}
			if (!unselectBtnOnUnhover)
			{
				component.UnselectButton();
				return;
			}
			MText_UI_List parentList = MText_Utilities.GetParentList(component.transform);
			if (!parentList)
			{
				component.UnselectButton();
			}
			else
			{
				parentList.UnselectEverything();
			}
		}

		private void Dragging(Transform hit)
		{
			DragSlider(hit);
			DragButton(hit);
		}

		private void DragSlider(Transform hit)
		{
			MText_UI_SliderHandle component = hit.gameObject.GetComponent<MText_UI_SliderHandle>();
			if (InteractWithSlider(component))
			{
				Vector3 vector = myCamera.WorldToScreenPoint(hit.position);
				Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, vector.z);
				Vector3 position2 = myCamera.ScreenToWorldPoint(position);
				Vector3 localPosition = new Vector3(hit.parent.InverseTransformPoint(position2).x, 0f, 0f);
				float backgroundSize = component.slider.backgroundSize;
				localPosition.x = Mathf.Clamp(localPosition.x, (0f - backgroundSize) / 2f, backgroundSize / 2f);
				hit.localPosition = localPosition;
				component.slider.GetCurrentValueFromHandle();
				component.slider.ValueChanged();
			}
		}

		private void DragButton(Transform hit)
		{
			MText_UI_Button component = hit.gameObject.GetComponent<MText_UI_Button>();
			if (InteractWithButton(component))
			{
				component.ButtonBeingPressed();
			}
		}

		private void DetectDragEnd()
		{
			if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
			{
				dragging = false;
				DragEnded(currentTarget, RaycastCheck());
			}
			if (Input.GetMouseButtonUp(0) && dragging)
			{
				dragging = false;
				DragEnded(currentTarget, RaycastCheck());
			}
		}

		private void DragEnded(Transform hit, Transform currentTarget)
		{
			DragEndOnSlider(hit);
			DragEndOnButton(hit, currentTarget);
		}

		private void DragEndOnSlider(Transform hit)
		{
			MText_UI_SliderHandle component = hit.gameObject.GetComponent<MText_UI_SliderHandle>();
			if (InteractWithSlider(component))
			{
				component.slider.ValueChangeEnded();
			}
		}

		private void DragEndOnButton(Transform hit, Transform currentTarget)
		{
			MText_UI_Button component = hit.gameObject.GetComponent<MText_UI_Button>();
			if (InteractWithButton(component))
			{
				if (currentTarget != hit)
				{
					component.selectedVisual = false;
				}
				component.PressedButtonClickStopped();
			}
		}

		private bool InteractWithButton(MText_UI_Button button)
		{
			return (bool)button && button.interactable && button.interactableByMouse;
		}

		private bool InteractWithSlider(MText_UI_SliderHandle sliderHandle)
		{
			return (bool)sliderHandle && (bool)sliderHandle.slider && sliderHandle.slider.interactable;
		}

		private bool InteractWithInputString(Mtext_UI_InputField inputString)
		{
			return (bool)inputString && inputString.interactable;
		}
	}
}
