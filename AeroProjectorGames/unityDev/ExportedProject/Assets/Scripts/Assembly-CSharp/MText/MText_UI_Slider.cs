using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MText
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Modular 3D Text/Slider")]
	public class MText_UI_Slider : MonoBehaviour
	{
		[Serializable]
		public class ValueRange
		{
			public float min;

			public float max = 25f;

			public GameObject icon;

			public bool triggeredAlready;

			public UnityEvent oneTimeEvents;

			public UnityEvent repeatEvents;
		}

		public bool autoFocusOnGameStart = true;

		public bool interactable = true;

		public float minValue;

		public float maxValue = 100f;

		[SerializeField]
		private float currentValue = 50f;

		public MText_UI_SliderHandle handle;

		public Transform progressBar;

		public GameObject progressBarPrefab;

		public Transform background;

		public float backgroundSize = 10f;

		public int directionChoice;

		public bool keyControl = true;

		[Tooltip("How much to change on key press")]
		public float keyStep = 10f;

		public KeyCode scrollUp = KeyCode.RightArrow;

		public KeyCode scrollDown = KeyCode.LeftArrow;

		public bool useEvents = true;

		public UnityEvent onValueChanged;

		[Tooltip("Mouse/touch dragging the slider ended")]
		public UnityEvent sliderDragEnded;

		public Renderer handleGraphic;

		public Material selectedHandleMat;

		public Material unSelectedHandleMat;

		public Material clickedHandleMat;

		public Material disabledHandleMat;

		public bool useValueRangeEvents = true;

		public bool usePercentage = true;

		public List<ValueRange> valueRangeEvents = new List<ValueRange>();

		[HideInInspector]
		[SerializeField]
		private int lastValue;

		public float Value
		{
			get
			{
				return currentValue;
			}
			set
			{
				currentValue = value;
				UpdateValue();
			}
		}

		public float CurrentPercentage()
		{
			return Value / maxValue * 100f;
		}

		private void Awake()
		{
			if (interactable && autoFocusOnGameStart && !MText_Utilities.GetParentList(base.transform))
			{
				Focus(enable: true);
				return;
			}
			DisabledVisual();
			base.enabled = false;
		}

		private void Update()
		{
			if (keyControl)
			{
				if (Input.GetKey(scrollUp))
				{
					IncreaseValue();
				}
				if (Input.GetKey(scrollDown))
				{
					DecreaseValue();
				}
			}
			else
			{
				base.enabled = false;
			}
		}

		public void UpdateValue()
		{
			ValueChanged();
			UpdateGraphic();
		}

		public void UpdateValue(int newValue)
		{
			Value = newValue;
		}

		public void UpdateValue(float newValue)
		{
			Value = newValue;
		}

		public void IncreaseValue()
		{
			float num = Value + keyStep * Time.deltaTime;
			if (num > maxValue)
			{
				num = maxValue;
			}
			Value = num;
		}

		public void IncreaseValue(int amount)
		{
			float num = Value + (float)amount * Time.deltaTime;
			if (num > maxValue)
			{
				num = maxValue;
			}
			Value = num;
		}

		public void IncreaseValue(float amount)
		{
			float num = Value + amount * Time.deltaTime;
			if (num > maxValue)
			{
				num = maxValue;
			}
			Value = num;
		}

		public void DecreaseValue()
		{
			float num = Value - keyStep * Time.deltaTime;
			if (num < minValue)
			{
				num = minValue;
			}
			Value = num;
		}

		public void DecreaseValue(int amount)
		{
			float num = Value - (float)amount * Time.deltaTime;
			if (num < minValue)
			{
				num = minValue;
			}
			Value = num;
		}

		public void DecreaseValue(float amount)
		{
			float num = Value - amount * Time.deltaTime;
			if (num < minValue)
			{
				num = minValue;
			}
			Value = num;
		}

		public void Focus(bool enable)
		{
			base.enabled = enable;
			if (enable)
			{
				SelectedVisual();
			}
			else
			{
				UnSelectedVisual();
			}
		}

		public void SelectedVisual()
		{
			(bool, MText_UI_List) tuple = ApplySelectedStyleFromParent();
			if (tuple.Item1)
			{
				ApplyVisual(tuple.Item2.selectedItemBackgroundMaterial);
			}
			else
			{
				ApplyVisual(selectedHandleMat);
			}
		}

		public void UnSelectedVisual()
		{
			(bool, MText_UI_List) tuple = ApplyNormalStyleFromParent();
			if (tuple.Item1)
			{
				ApplyVisual(tuple.Item2.normalItemBackgroundMaterial);
			}
			else
			{
				ApplyVisual(unSelectedHandleMat);
			}
		}

		public void ClickedVisual()
		{
			(bool, MText_UI_List) tuple = ApplyPressedStyleFromParent();
			if (tuple.Item1)
			{
				ApplyVisual(tuple.Item2.pressedItemBackgroundMaterial);
			}
			else
			{
				ApplyVisual(clickedHandleMat);
			}
		}

		public void DisabledVisual()
		{
			(bool, MText_UI_List) tuple = ApplyDisabledStyleFromParent();
			if (tuple.Item1)
			{
				ApplyVisual(tuple.Item2.disabledItemBackgroundMaterial);
			}
			else
			{
				ApplyVisual(disabledHandleMat);
			}
		}

		private void ApplyVisual(Material handleMaterial)
		{
			if ((bool)handleGraphic)
			{
				handleGraphic.material = handleMaterial;
			}
		}

		public MText_UI_List GetParentList()
		{
			return MText_Utilities.GetParentList(base.transform);
		}

		public (bool, MText_UI_List) ApplyNormalStyleFromParent()
		{
			MText_UI_List parentList = GetParentList();
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customNormalItemVisual)
			{
				return (true, parentList);
			}
			return (false, parentList);
		}

		public (bool, MText_UI_List) ApplySelectedStyleFromParent()
		{
			MText_UI_List parentList = GetParentList();
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customSelectedItemVisual)
			{
				return (true, parentList);
			}
			return (false, parentList);
		}

		public (bool, MText_UI_List) ApplyPressedStyleFromParent()
		{
			MText_UI_List parentList = GetParentList();
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customPressedItemVisual)
			{
				return (true, parentList);
			}
			return (false, parentList);
		}

		public (bool, MText_UI_List) ApplyDisabledStyleFromParent()
		{
			MText_UI_List parentList = GetParentList();
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customDisabledItemVisual)
			{
				return (true, parentList);
			}
			return (false, parentList);
		}

		public void ValueChanged()
		{
			if (useEvents)
			{
				onValueChanged.Invoke();
			}
			if (useValueRangeEvents)
			{
				ValueRangeEvents();
			}
		}

		private void ValueRangeEvents()
		{
			float num = Value;
			if (usePercentage)
			{
				num = CurrentPercentage();
			}
			bool flag = false;
			int i = 0;
			for (int j = 0; j < valueRangeEvents.Count; j++)
			{
				if (num >= valueRangeEvents[j].min && num <= valueRangeEvents[j].max)
				{
					i = j;
					if (lastValue != j)
					{
						flag = true;
					}
					break;
				}
			}
			if (flag && valueRangeEvents.Count > lastValue)
			{
				if ((bool)valueRangeEvents[lastValue].icon)
				{
					valueRangeEvents[lastValue].icon.SetActive(value: false);
				}
				lastValue = i;
			}
			ProcessSelectedValueRange(i, flag);
		}

		private void ProcessSelectedValueRange(int i, bool firstTime)
		{
			if (valueRangeEvents.Count > i)
			{
				if ((bool)valueRangeEvents[i].icon)
				{
					valueRangeEvents[i].icon.SetActive(value: true);
				}
				if (firstTime)
				{
					valueRangeEvents[i].oneTimeEvents.Invoke();
				}
				valueRangeEvents[i].repeatEvents.Invoke();
			}
		}

		public void ValueChangeEnded()
		{
			if (useEvents)
			{
				sliderDragEnded.Invoke();
			}
		}

		public void Uninteractable()
		{
			interactable = false;
			DisabledVisual();
		}

		public void Interactable()
		{
			interactable = true;
			UnSelectedVisual();
		}

		public void NewValueRange()
		{
			ValueRange item = new ValueRange();
			valueRangeEvents.Add(item);
		}

		public void GetCurrentValueFromHandle()
		{
			Value = RangeConvertedValue(handle.transform.localPosition.x, (0f - backgroundSize) / 2f, backgroundSize / 2f, minValue, maxValue);
			UpdateProgressBar();
		}

		public void UpdateGraphic()
		{
			UpdateHandle();
			UpdateProgressBar();
		}

		private void UpdateHandle()
		{
			if ((bool)handle)
			{
				int num = -1;
				if (directionChoice == 1)
				{
					num = 1;
				}
				Vector3 localPosition = handle.transform.localPosition;
				localPosition.x = (float)num * RangeConvertedValue(Value, minValue, maxValue, backgroundSize / 2f, (0f - backgroundSize) / 2f);
				handle.transform.localPosition = localPosition;
			}
		}

		private void UpdateProgressBar()
		{
			if ((bool)progressBar)
			{
				Vector3 localScale = progressBar.localScale;
				localScale.x = (Value - minValue) / (maxValue - minValue) * backgroundSize;
				progressBar.localScale = localScale;
				Vector3 localPosition = progressBar.localPosition;
				localPosition.x = (0f - backgroundSize) / 2f;
				progressBar.localPosition = localPosition;
			}
		}

		private float RangeConvertedValue(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
		{
			return (oldValue - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
		}
	}
}
