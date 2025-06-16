using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MText
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MText_UI_ListEditorHelper))]
	[AddComponentMenu("Modular 3D Text/List")]
	public class MText_UI_List : MonoBehaviour
	{
		public int alignmentChoice;

		[Tooltip("List is scrollable with keyboard when focused")]
		[SerializeField]
		private bool autoFocusOnStart = true;

		[Tooltip("Selects first item when focused")]
		[SerializeField]
		private bool autoFocusFirstItem = true;

		public float spacing = 3f;

		[Tooltip("Set Rotation to always 0,0,0")]
		public bool lockRotation = true;

		public int circularListStyle;

		public float spread = 360f;

		public float radius = 5f;

		public bool keyboardControl;

		[SerializeField]
		private KeyCode scrollUp = KeyCode.UpArrow;

		[SerializeField]
		private KeyCode scrollDown = KeyCode.DownArrow;

		[SerializeField]
		private KeyCode pressItemKey = KeyCode.Return;

		public AudioSource audioSource;

		public AudioClip keyScrollingSoundEffect;

		public AudioClip itemSelectionSoundEffect;

		public bool controlChildVisuals;

		public bool customNormalItemVisual;

		public Vector3 normalItemFontSize = new Vector3(10f, 10f, 1f);

		public Material normalItemFontMaterial;

		public Material normalItemBackgroundMaterial;

		public bool customSelectedItemVisual;

		public Vector3 selectedItemFontSize = new Vector3(10.5f, 10.5f, 5f);

		public Material selectedItemFontMaterial;

		public Material selectedItemBackgroundMaterial;

		[SerializeField]
		private Vector3 selectedItemPositionChange = new Vector3(0f, 0f, -0.5f);

		[SerializeField]
		private float selectedItemMoveTime = 0.25f;

		public bool customPressedItemVisual;

		public Vector3 pressedItemFontSize = new Vector3(10.5f, 10.5f, 5f);

		public Material pressedItemFontMaterial;

		public Material pressedItemBackgroundMaterial;

		[SerializeField]
		private Vector3 pressedItemPositionChange = new Vector3(0f, 0f, -0.5f);

		[SerializeField]
		private float pressedItemMoveTime = 0.1f;

		public bool pressedItemReturnToSelectedVisual = true;

		[SerializeField]
		private float pressedItemReturnToSelectedTime = 0.1f;

		public bool customDisabledItemVisual;

		public Vector3 disabledItemFontSize = new Vector3(9f, 9f, 1f);

		public Material disabledItemFontMaterial;

		public Material disabledItemBackgroundMaterial;

		public bool applyModules = true;

		public bool ignoreChildModules;

		public bool ignoreChildUnSelectModuleContainers;

		public bool applyUnSelectModuleContainers = true;

		public List<MText_ModuleContainer> unSelectModuleContainers = new List<MText_ModuleContainer>();

		public bool ignoreChildOnSelectModuleContainers;

		public bool applyOnSelectModuleContainers = true;

		public List<MText_ModuleContainer> onSelectModuleContainers = new List<MText_ModuleContainer>();

		public bool ignoreChildOnPressModuleContainers;

		public bool applyOnPressModuleContainers = true;

		public List<MText_ModuleContainer> onPressModuleContainers = new List<MText_ModuleContainer>();

		public bool ignoreChildOnClickModuleContainers;

		public bool applyOnClickModuleContainers = true;

		public List<MText_ModuleContainer> onClickModuleContainers = new List<MText_ModuleContainer>();

		public int selectedItem;

		private Quaternion expectedRotation = Quaternion.Euler(0f, 0f, 0f);

		private Quaternion originalRotation = Quaternion.Euler(0f, 0f, 0f);

		public float speed = 1f;

		private Vector3 originalPosition = Vector3.zero;

		private Vector3 selectedPosition = Vector3.zero;

		private float startTime;

		private bool selected;

		private bool pressed;

		private float returnToSelectedTime;

		private int counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop;

		private int previousSelection;

		private void Awake()
		{
			SetRequiredFields();
			if (!autoFocusOnStart)
			{
				base.enabled = false;
				return;
			}
			base.enabled = true;
			if (autoFocusFirstItem)
			{
				SelectTheFirstSelectableItem();
			}
			else
			{
				UnselectEverything();
			}
		}

		private void Start()
		{
			originalRotation = base.transform.rotation;
		}

		private void Update()
		{
			if (base.transform.childCount != 0)
			{
				if (keyboardControl)
				{
					PressItemCheckInUpdate();
					ScrollList();
				}
				Animation();
			}
		}

		[ContextMenu("Update List")]
		public void UpdateList()
		{
			if (alignmentChoice == 7)
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			if (alignmentChoice == 0)
			{
				num3 = 0f;
				num4 = 0f - spacing;
			}
			else if (alignmentChoice == 1)
			{
				num3 = (float)base.transform.childCount * spacing - spacing / 2f;
				num4 = 0f - spacing;
			}
			else if (alignmentChoice == 2)
			{
				num3 = ((float)base.transform.childCount * spacing - spacing) / 2f;
				num4 = 0f - spacing;
			}
			else if (alignmentChoice == 3)
			{
				num = 0f;
				num2 = spacing;
			}
			else if (alignmentChoice == 4)
			{
				num = (0f - (float)base.transform.childCount) * spacing + spacing / 2f;
				num2 = spacing;
			}
			else if (alignmentChoice == 5)
			{
				num = (0f - ((float)base.transform.childCount * spacing - spacing)) / 2f;
				num2 = spacing;
			}
			if (alignmentChoice != 6)
			{
				for (int i = 0; i < base.transform.childCount; i++)
				{
					base.transform.GetChild(i).localPosition = new Vector3(num, num3, 0f);
					if (lockRotation)
					{
						base.transform.GetChild(i).localRotation = Quaternion.Euler(0f, 0f, 0f);
					}
					num += num2;
					num3 += num4;
				}
			}
			else
			{
				CircularListStyle();
			}
		}

		public void Focus(bool enable)
		{
			pressed = false;
			selected = false;
			if (!enable)
			{
				UnselectEverything();
				base.enabled = enable;
				return;
			}
			UnselectEverything();
			if (autoFocusFirstItem)
			{
				SelectTheFirstSelectableItem();
			}
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(FocusRoutine());
			}
			else
			{
				base.enabled = true;
			}
		}

		public void Focus(bool enable, bool delay)
		{
			pressed = false;
			selected = false;
			if (!enable)
			{
				UnselectEverything();
				base.enabled = enable;
				return;
			}
			UnselectEverything();
			if (autoFocusFirstItem)
			{
				SelectTheFirstSelectableItem();
			}
			if (delay)
			{
				StartCoroutine(FocusRoutine());
			}
			else
			{
				base.enabled = true;
			}
		}

		public void FocusToggle()
		{
			if (base.enabled)
			{
				Focus(enable: false, delay: true);
			}
			else
			{
				Focus(enable: true, delay: true);
			}
		}

		public void SelectItem(int number)
		{
			if (base.transform.childCount > number)
			{
				selected = true;
				if ((bool)audioSource && (bool)keyScrollingSoundEffect)
				{
					audioSource.PlayOneShot(keyScrollingSoundEffect, UnityEngine.Random.Range(0.5f, 0.75f));
				}
				UpdateList();
				selectedItem = number;
				originalPosition = base.transform.GetChild(selectedItem).localPosition;
				selectedPosition = originalPosition + selectedItemPositionChange;
				startTime = Time.time;
				if (alignmentChoice == 6)
				{
					expectedRotation = originalRotation * Quaternion.Euler(0f, 0f, 360f / (float)base.transform.childCount * (float)number);
				}
			}
		}

		public void AlertSelectedItem(int number)
		{
			if (base.transform.childCount > number)
			{
				Transform child = base.transform.GetChild(number);
				MText_UI_Button component = child.GetComponent<MText_UI_Button>();
				if (component != null)
				{
					component.SelectButton();
				}
				Mtext_UI_InputField component2 = child.GetComponent<Mtext_UI_InputField>();
				if (component2 != null)
				{
					component2.Focus(enable: true);
				}
				MText_UI_Slider component3 = child.GetComponent<MText_UI_Slider>();
				if (component3 != null)
				{
					component3.Focus(enable: true);
				}
				MText_UI_HorizontalSelector component4 = child.GetComponent<MText_UI_HorizontalSelector>();
				if (!(component4 == null))
				{
					component4.Focus(Bool: true);
				}
			}
		}

		public void UnselectItem(int i)
		{
			if (base.transform.childCount <= i)
			{
				return;
			}
			if ((bool)base.transform.GetChild(i).GetComponent<MText_UI_Button>())
			{
				if (base.transform.GetChild(i).GetComponent<MText_UI_Button>().interactable)
				{
					base.transform.GetChild(i).GetComponent<MText_UI_Button>().UnselectButton();
				}
				else
				{
					base.transform.GetChild(i).GetComponent<MText_UI_Button>().Uninteractable();
				}
			}
			if ((bool)base.transform.GetChild(i).GetComponent<Mtext_UI_InputField>())
			{
				base.transform.GetChild(i).GetComponent<Mtext_UI_InputField>().Focus(enable: false);
			}
			if ((bool)base.transform.GetChild(i).gameObject.GetComponent<MText_UI_Slider>())
			{
				if (base.transform.GetChild(i).gameObject.GetComponent<MText_UI_Slider>().interactable)
				{
					MText_UI_Slider component = base.transform.GetChild(i).gameObject.GetComponent<MText_UI_Slider>();
					if (component != null)
					{
						component.Focus(enable: false);
					}
				}
				else
				{
					base.transform.GetChild(i).gameObject.GetComponent<MText_UI_Slider>().Uninteractable();
				}
			}
			if ((bool)base.transform.GetChild(i).gameObject.GetComponent<MText_UI_HorizontalSelector>() && base.transform.GetChild(i).gameObject.GetComponent<MText_UI_HorizontalSelector>().interactable)
			{
				MText_UI_HorizontalSelector component2 = base.transform.GetChild(i).gameObject.GetComponent<MText_UI_HorizontalSelector>();
				if (!(component2 == null))
				{
					component2.Focus(Bool: false);
				}
			}
		}

		public void UnselectEverything()
		{
			selectedItem = base.transform.childCount;
			UpdateList();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				UnselectItem(i);
			}
		}

		public void UnselectEverythingExceptSelected()
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				if (i != selectedItem)
				{
					UnselectItem(i);
				}
			}
		}

		public void PresstItem(int i)
		{
			if (base.transform.childCount > i)
			{
				pressed = true;
				startTime = Time.time;
				if (pressedItemReturnToSelectedVisual)
				{
					returnToSelectedTime = Time.time + pressedItemReturnToSelectedTime + pressedItemMoveTime;
				}
				if ((bool)audioSource && (bool)itemSelectionSoundEffect)
				{
					audioSource.PlayOneShot(itemSelectionSoundEffect);
				}
				AlertPressedUIItem();
			}
		}

		private void SetRequiredFields()
		{
			if (base.transform.childCount > 0)
			{
				originalPosition = base.transform.GetChild(0).localPosition;
			}
			if (alignmentChoice == 6)
			{
				expectedRotation = base.transform.rotation;
			}
			if (pressedItemReturnToSelectedTime == 0f)
			{
				pressedItemReturnToSelectedTime = 0.01f;
			}
			if (pressedItemMoveTime == 0f)
			{
				pressedItemMoveTime = 0.01f;
			}
		}

		private void Animation()
		{
			if (alignmentChoice == 6)
			{
				if (!lockRotation)
				{
					base.transform.rotation = Quaternion.Lerp(base.transform.rotation, expectedRotation, Time.time * speed / 100f);
				}
				if (base.transform.childCount > selectedItem && pressed && pressedItemReturnToSelectedVisual && Time.time > returnToSelectedTime)
				{
					pressed = false;
					MText_UI_Button component = base.transform.GetChild(selectedItem).GetComponent<MText_UI_Button>();
					if (!(component == null))
					{
						component.SelectedButtonVisualUpdate();
					}
				}
			}
			else
			{
				if (alignmentChoice == 7 || base.transform.childCount <= selectedItem)
				{
					return;
				}
				if (pressed)
				{
					float t = (Time.time - startTime) / pressedItemMoveTime;
					base.transform.GetChild(selectedItem).localPosition = Vector3.Slerp(base.transform.GetChild(selectedItem).localPosition, selectedPosition + pressedItemPositionChange, t);
					if (pressedItemReturnToSelectedVisual && Time.time > returnToSelectedTime)
					{
						pressed = false;
						MText_UI_Button component2 = base.transform.GetChild(selectedItem).GetComponent<MText_UI_Button>();
						if (!(component2 == null))
						{
							component2.SelectedButtonVisualUpdate();
						}
					}
				}
				else if (selected)
				{
					float t2 = (Time.time - startTime) / selectedItemMoveTime;
					base.transform.GetChild(selectedItem).localPosition = Vector3.Slerp(base.transform.GetChild(selectedItem).localPosition, selectedPosition, t2);
				}
			}
		}

		private void PressItemCheckInUpdate()
		{
			if (Input.GetKeyDown(pressItemKey) && selected)
			{
				PresstItem(selectedItem);
			}
		}

		private void ScrollList()
		{
			if (Scrolled())
			{
				selected = true;
				SelectItem(selectedItem);
				AlertSelectedItem(selectedItem);
				if (selectedItem != previousSelection)
				{
					UnselectItem(previousSelection);
				}
			}
		}

		private bool Scrolled()
		{
			bool result = false;
			if (Input.GetKeyDown(scrollUp))
			{
				result = true;
				previousSelection = selectedItem;
				selectedItem--;
				if (selectedItem < 0)
				{
					selectedItem = base.transform.childCount - 1;
				}
				while (!IsItemSelectable(selectedItem) && base.transform.childCount > 0 && counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop < base.transform.childCount)
				{
					counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop++;
					selectedItem--;
					if (selectedItem < 0)
					{
						selectedItem = base.transform.childCount - 1;
					}
				}
				counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop = 0;
			}
			else if (Input.GetKeyDown(scrollDown))
			{
				result = true;
				previousSelection = selectedItem;
				selectedItem++;
				if (selectedItem > base.transform.childCount - 1)
				{
					selectedItem = 0;
				}
				while (!IsItemSelectable(selectedItem) && base.transform.childCount > 0 && counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop < base.transform.childCount)
				{
					counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop++;
					selectedItem++;
					if (selectedItem > base.transform.childCount - 1)
					{
						selectedItem = 0;
					}
				}
				counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop = 0;
			}
			return result;
		}

		private void CircularListStyle()
		{
			float num = 0f;
			if (base.transform.childCount > 1)
			{
				num = 0f - spread / 2f + spread / (float)base.transform.childCount / 2f;
			}
			for (int i = 0; i < base.transform.childCount; i++)
			{
				float x = Mathf.Sin(MathF.PI / 180f * num) * radius;
				float y = Mathf.Cos(MathF.PI / 180f * num) * radius;
				base.transform.GetChild(i).localPosition = new Vector3(x, y, 0f);
				if (circularListStyle == 0)
				{
					base.transform.GetChild(i).localRotation = Quaternion.Euler(num - 90f, 90f, 0f);
				}
				else if (circularListStyle == 1)
				{
					base.transform.GetChild(i).localRotation = Quaternion.Euler(num + 90f, 90f, 0f);
				}
				else if (circularListStyle == 2)
				{
					base.transform.GetChild(i).localRotation = Quaternion.Euler(num + 90f, 90f, 90f);
				}
				else if (circularListStyle == 3)
				{
					base.transform.GetChild(i).localRotation = Quaternion.Euler(num - 90f, 90f, 90f);
				}
				else
				{
					Vector3 vector = Vector3.zero - base.transform.GetChild(i).localPosition;
					float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
					base.transform.GetChild(i).localRotation = Quaternion.Euler(new Vector3(0f, 0f, z));
				}
				num += spread / (float)base.transform.childCount;
			}
		}

		private IEnumerator FocusRoutine()
		{
			yield return null;
			base.enabled = true;
		}

		private bool IsItemSelectable(int i)
		{
			if (base.transform.childCount > i)
			{
				Transform child = base.transform.GetChild(i);
				if ((bool)child.GetComponent<MText_UI_Button>())
				{
					return child.GetComponent<MText_UI_Button>().interactable;
				}
				if ((bool)child.GetComponent<Mtext_UI_InputField>())
				{
					return child.GetComponent<Mtext_UI_InputField>().interactable;
				}
				if ((bool)child.gameObject.GetComponent<MText_UI_Slider>())
				{
					return child.GetComponent<MText_UI_Slider>().interactable;
				}
				if ((bool)child.GetComponent<MText_UI_HorizontalSelector>())
				{
					return child.GetComponent<MText_UI_HorizontalSelector>().interactable;
				}
			}
			return false;
		}

		private void AlertPressedUIItem()
		{
			MText_UI_Button component = base.transform.GetChild(selectedItem).GetComponent<MText_UI_Button>();
			if (!(component == null))
			{
				component.PressButtonDontCallList();
			}
		}

		private void SelectTheFirstSelectableItem()
		{
			selected = true;
			if (selectedItem > base.transform.childCount - 1)
			{
				selectedItem = 0;
			}
			while (!IsItemSelectable(selectedItem) && counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop < base.transform.childCount)
			{
				counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop++;
				selectedItem++;
				if (selectedItem > base.transform.childCount - 1)
				{
					selectedItem = 0;
				}
			}
			counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop = 0;
			SelectItem(selectedItem);
			AlertSelectedItem(selectedItem);
		}

		public void CreateEmptyEffect(List<MText_ModuleContainer> moduleList)
		{
			MText_ModuleContainer item = new MText_ModuleContainer
			{
				duration = 0.5f
			};
			moduleList.Add(item);
		}
	}
}
