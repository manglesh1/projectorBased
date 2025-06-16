using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MText
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Modular 3D Text/Buttons")]
	public class MText_UI_Button : MonoBehaviour
	{
		public MText_Settings settings;

		public UnityEvent onClickEvents = new UnityEvent();

		public UnityEvent whileBeingClickedEvents;

		public UnityEvent onSelectEvents;

		public UnityEvent onUnselectEvents;

		public bool interactable = true;

		[Tooltip("Mouse or touch can select this")]
		public bool interactableByMouse = true;

		public Modular3DText text;

		public Renderer background;

		public bool useModules = true;

		public bool useStyles = true;

		public Vector3 normalFontSize = new Vector3(8f, 8f, 1f);

		public Material normalFontMaterial;

		public Material normalBackgroundMaterial;

		public bool applyUnSelectModuleContainers = true;

		public List<MText_ModuleContainer> unSelectModuleContainers = new List<MText_ModuleContainer>();

		public bool applySelectedVisual = true;

		public Vector3 selectedFontSize = new Vector3(8.2f, 8.2f, 8.2f);

		public Material selectedFontMaterial;

		public Material selectedBackgroundMaterial;

		public bool applyOnSelectModuleContainers = true;

		public List<MText_ModuleContainer> onSelectModuleContainers = new List<MText_ModuleContainer>();

		public bool applyPressedVisual = true;

		public Vector3 pressedFontSize = new Vector3(8.5f, 8.5f, 8.5f);

		public Material pressedFontMaterial;

		public Material pressedBackgroundMaterial;

		public bool pressedItemReturnToSelectedVisuaAfterDelay = true;

		public float pressedItemReturnToSelectedVisualTime = 0.15f;

		private float returnToselectedTime;

		public bool applyOnPressModuleContainers = true;

		public List<MText_ModuleContainer> onPressModuleContainers = new List<MText_ModuleContainer>();

		public bool applyOnClickModuleContainers = true;

		public List<MText_ModuleContainer> onClickModuleContainers = new List<MText_ModuleContainer>();

		public Vector3 disabledFontSize = new Vector3(8f, 8f, 8f);

		public Material disabledFontMaterial;

		public Material disabledBackgroundMaterial;

		public bool selectedVisual;

		private void Awake()
		{
			base.enabled = false;
		}

		private void Update()
		{
			Animation();
		}

		private void Animation()
		{
			if (Time.time > returnToselectedTime)
			{
				if (selectedVisual)
				{
					SelectedButtonVisualUpdate();
				}
				else
				{
					UnselectedButtonVisualUpdate();
				}
				base.enabled = false;
			}
		}

		public void SelectButton()
		{
			MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
			if ((bool)parentList)
			{
				int siblingIndex = base.transform.GetSiblingIndex();
				if (parentList.selectedItem != siblingIndex)
				{
					parentList.UnselectItem(parentList.selectedItem);
				}
				parentList.SelectItem(siblingIndex);
			}
			SelectedButtonVisualUpdate();
			SelectedButtonModuleUpdate();
			onSelectEvents.Invoke();
		}

		public void SelectedButtonVisualUpdate()
		{
			selectedVisual = true;
			(bool, bool, MText_UI_List) tuple = ApplyOnSelectStyle();
			if (tuple.Item1)
			{
				ApplyeStyle(tuple.Item3.selectedItemFontSize, tuple.Item3.selectedItemFontMaterial, tuple.Item3.selectedItemBackgroundMaterial);
			}
			else if (tuple.Item2)
			{
				ApplyeStyle(selectedFontSize, selectedFontMaterial, selectedBackgroundMaterial);
			}
		}

		public void SelectedButtonModuleUpdate()
		{
			(bool, bool, MText_UI_List) tuple = ApplyOnSelectModule();
			if (tuple.Item1)
			{
				CallModules(tuple.Item3.onSelectModuleContainers);
			}
			if (tuple.Item2)
			{
				CallModules(onSelectModuleContainers);
			}
		}

		public void UnselectButton()
		{
			UnselectedButtonVisualUpdate();
			UnselectButtonModuleUpdate();
			onUnselectEvents.Invoke();
		}

		public void UnselectedButtonVisualUpdate()
		{
			selectedVisual = false;
			if (ApplyNormalStyle().Item1)
			{
				MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
				ApplyeStyle(parentList.normalItemFontSize, parentList.normalItemFontMaterial, parentList.normalItemBackgroundMaterial);
			}
			else if (ApplyNormalStyle().Item2)
			{
				ApplyeStyle(normalFontSize, normalFontMaterial, normalBackgroundMaterial);
			}
		}

		public void UnselectButtonModuleUpdate()
		{
			(bool, bool, MText_UI_List) tuple = ApplyUnSelectModule();
			if (tuple.Item1)
			{
				CallModules(tuple.Item3.unSelectModuleContainers);
			}
			if (tuple.Item2)
			{
				CallModules(unSelectModuleContainers);
			}
		}

		public void PressButton()
		{
			MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
			if ((bool)parentList)
			{
				parentList.PresstItem(base.transform.GetSiblingIndex());
			}
			PressButtonVisualUpdate();
			onClickEvents.Invoke();
			OnClickButtonModuleUpdate();
		}

		public void PressButtonDontCallList()
		{
			PressButtonVisualUpdate();
			onClickEvents.Invoke();
			OnClickButtonModuleUpdate();
		}

		public void PressButtonVisualUpdate()
		{
			(bool, bool, MText_UI_List) tuple = ApplyPressedStyle();
			if (tuple.Item1)
			{
				ApplyeStyle(tuple.Item3.pressedItemFontSize, tuple.Item3.pressedItemFontMaterial, tuple.Item3.pressedItemBackgroundMaterial);
			}
			else if (tuple.Item2)
			{
				ApplyeStyle(pressedFontSize, pressedFontMaterial, pressedBackgroundMaterial);
				if (pressedItemReturnToSelectedVisuaAfterDelay)
				{
					base.enabled = true;
					returnToselectedTime = Time.time + pressedItemReturnToSelectedVisualTime;
				}
			}
		}

		public void PressButtonClick()
		{
			(bool, bool, MText_UI_List) tuple = ApplyPressedStyle();
			if (tuple.Item1)
			{
				ApplyeStyle(tuple.Item3.pressedItemFontSize, tuple.Item3.pressedItemFontMaterial, tuple.Item3.pressedItemBackgroundMaterial);
			}
			else if (tuple.Item2)
			{
				ApplyeStyle(pressedFontSize, pressedFontMaterial, pressedBackgroundMaterial);
			}
		}

		public void ButtonBeingPressed()
		{
			whileBeingClickedEvents.Invoke();
			OnPressButtonModuleUpdate();
		}

		public void PressedButtonClickStopped()
		{
			if (selectedVisual)
			{
				onClickEvents.Invoke();
				OnClickButtonModuleUpdate();
			}
			if (pressedItemReturnToSelectedVisuaAfterDelay)
			{
				base.enabled = true;
				returnToselectedTime = Time.time + pressedItemReturnToSelectedVisualTime;
			}
			else if (selectedVisual)
			{
				SelectedButtonVisualUpdate();
			}
			else
			{
				UnselectedButtonVisualUpdate();
			}
		}

		public void OnClickButtonModuleUpdate()
		{
			(bool, bool, MText_UI_List) tuple = ApplyOnClickModule();
			if (tuple.Item1)
			{
				CallModules(tuple.Item3.onClickModuleContainers);
			}
			if (tuple.Item2)
			{
				CallModules(onClickModuleContainers);
			}
		}

		public void OnPressButtonModuleUpdate()
		{
			(bool, bool, MText_UI_List) tuple = ApplyOnPresstModule();
			if (tuple.Item1)
			{
				CallModules(tuple.Item3.onPressModuleContainers);
			}
			if (tuple.Item2)
			{
				CallModules(onPressModuleContainers);
			}
		}

		public void DisabledButtonVisualUpdate()
		{
			(bool, MText_UI_List) tuple = ApplyDisabledStyle();
			if (tuple.Item1)
			{
				ApplyeStyle(disabledFontSize, disabledFontMaterial, disabledBackgroundMaterial);
			}
			else
			{
				ApplyeStyle(tuple.Item2.disabledItemFontSize, tuple.Item2.disabledItemFontMaterial, tuple.Item2.disabledItemBackgroundMaterial);
			}
		}

		private void ApplyeStyle(Vector3 fontSize, Material fontMat, Material backgroundMat)
		{
			if ((bool)text)
			{
				text.FontSize = fontSize;
				text.Material = fontMat;
				text.UpdateText();
			}
			if ((bool)background)
			{
				background.material = backgroundMat;
			}
		}

		public void Uninteractable()
		{
			interactable = false;
			DisabledButtonVisualUpdate();
		}

		public void Interactable()
		{
			interactable = true;
			UnselectedButtonVisualUpdate();
		}

		public (bool, bool) ApplyNormalStyle()
		{
			MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customNormalItemVisual)
			{
				return (true, false);
			}
			return (false, true);
		}

		public (bool, bool, MText_UI_List) ApplyOnSelectStyle()
		{
			MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customSelectedItemVisual)
			{
				return (true, false, parentList);
			}
			if (applySelectedVisual)
			{
				return (false, true, parentList);
			}
			return (false, false, parentList);
		}

		public (bool, bool, MText_UI_List) ApplyPressedStyle()
		{
			MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customPressedItemVisual)
			{
				return (true, false, parentList);
			}
			if (applyPressedVisual)
			{
				return (false, true, parentList);
			}
			return (false, false, parentList);
		}

		public (bool, MText_UI_List) ApplyDisabledStyle()
		{
			MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customDisabledItemVisual)
			{
				return (false, parentList);
			}
			return (true, parentList);
		}

		private void CallModules(List<MText_ModuleContainer> moduleContainers)
		{
			if (moduleContainers.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < moduleContainers.Count; i++)
			{
				if ((bool)moduleContainers[i].module)
				{
					StartCoroutine(moduleContainers[i].module.ModuleRoutine(base.gameObject, moduleContainers[i].duration));
				}
			}
		}

		public (bool, bool, MText_UI_List) ApplyUnSelectModule()
		{
			bool item = false;
			bool item2 = false;
			MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
			if ((bool)parentList)
			{
				if (parentList.applyModules && parentList.applyOnSelectModuleContainers)
				{
					item2 = true;
				}
				if (parentList.ignoreChildModules || parentList.ignoreChildUnSelectModuleContainers)
				{
					return (item2, item, parentList);
				}
			}
			if (useModules && applyUnSelectModuleContainers)
			{
				item = true;
			}
			return (item2, item, parentList);
		}

		public (bool, bool, MText_UI_List) ApplyOnPresstModule()
		{
			bool item = false;
			bool item2 = false;
			MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
			if ((bool)parentList)
			{
				if (parentList.applyModules && parentList.applyOnPressModuleContainers)
				{
					item2 = true;
				}
				if (parentList.ignoreChildModules || parentList.ignoreChildOnPressModuleContainers)
				{
					return (item2, item, parentList);
				}
			}
			if (useModules && applyOnPressModuleContainers)
			{
				item = true;
			}
			return (item2, item, parentList);
		}

		public (bool, bool, MText_UI_List) ApplyOnClickModule()
		{
			bool item = false;
			bool item2 = false;
			MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
			if ((bool)parentList)
			{
				if (parentList.applyModules && parentList.applyOnClickModuleContainers)
				{
					item2 = true;
				}
				if (parentList.ignoreChildModules || parentList.ignoreChildOnClickModuleContainers)
				{
					return (item2, item, parentList);
				}
			}
			if (useModules && applyOnPressModuleContainers)
			{
				item = true;
			}
			return (item2, item, parentList);
		}

		public (bool, bool, MText_UI_List) ApplyOnSelectModule()
		{
			bool item = false;
			bool item2 = false;
			MText_UI_List parentList = MText_Utilities.GetParentList(base.transform);
			if ((bool)parentList)
			{
				if (parentList.applyModules && parentList.applyOnSelectModuleContainers)
				{
					item2 = true;
				}
				if (parentList.ignoreChildModules || parentList.ignoreChildOnSelectModuleContainers)
				{
					return (item2, item, parentList);
				}
			}
			if (useModules && applyOnSelectModuleContainers)
			{
				item = true;
			}
			return (item2, item, parentList);
		}

		public void EmptyEffect(List<MText_ModuleContainer> moduleList)
		{
			moduleList.Add(new MText_ModuleContainer
			{
				duration = 0.5f
			});
		}

		public void LoadDefaultSettings()
		{
			if ((bool)settings)
			{
				normalBackgroundMaterial = settings.defaultBackgroundMaterial;
			}
		}
	}
}
