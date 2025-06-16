using System;
using System.Collections.Generic;
using Admin_Panel.Events;
using ResizingAndMoving;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Games.IATF.Ring_Target.Scripts
{
	public class IATFAdminController : MonoBehaviour
	{
		private SizeAndPositionStateSO _selectedSizeAndPositionState;

		private readonly List<GameObject> _loadedControls = new List<GameObject>();

		private Dictionary<IATFComponent, Button> _tabMap = new Dictionary<IATFComponent, Button>();

		private Dictionary<IATFComponent, SizeAndPositionStateSO> _sizeAndPositionMap = new Dictionary<IATFComponent, SizeAndPositionStateSO>();

		[Header("Admin Control Component Tab Panel")]
		[SerializeField]
		private GameObject tabPanel;

		[Header("Admin Panels")]
		[SerializeField]
		private GameObject topPanel;

		[SerializeField]
		private GameObject centerPanel;

		[SerializeField]
		private GameObject rightPanel;

		[SerializeField]
		private GameObject leftPanel;

		[Space(1f)]
		[Header("Prefabs")]
		[SerializeField]
		private GameObject sizeControlsPrefab;

		[SerializeField]
		private GameObject positionControlsPrefab;

		[Space(1f)]
		[Header("Tab Objects")]
		[SerializeField]
		private Button ring1Tab;

		[SerializeField]
		private Button ring2Tab;

		[SerializeField]
		private Button ring3Tab;

		[SerializeField]
		private Button clutch1Tab;

		[SerializeField]
		private Button clutch2Tab;

		[Space(1f)]
		[Header("Component Resizing SOs")]
		[SerializeField]
		private SizeAndPositionStateSO clutch1SizeAndPosition;

		[SerializeField]
		private SizeAndPositionStateSO clutch2SizeAndPosition;

		[SerializeField]
		private SizeAndPositionStateSO ring1SizeAndPosition;

		[SerializeField]
		private SizeAndPositionStateSO ring2SizeAndPosition;

		[SerializeField]
		private SizeAndPositionStateSO ring3SizeAndPosition;

		[Space(1f)]
		[Header("References")]
		[SerializeField]
		private AdminEventsSO adminEvents;

		protected void OnEnable()
		{
			HideAdminControls();
			_sizeAndPositionMap.Add(IATFComponent.Clutch1, clutch1SizeAndPosition);
			_sizeAndPositionMap.Add(IATFComponent.Clutch2, clutch2SizeAndPosition);
			_sizeAndPositionMap.Add(IATFComponent.Ring1, ring1SizeAndPosition);
			_sizeAndPositionMap.Add(IATFComponent.Ring2, ring2SizeAndPosition);
			_sizeAndPositionMap.Add(IATFComponent.Ring3, ring3SizeAndPosition);
			foreach (KeyValuePair<IATFComponent, SizeAndPositionStateSO> item in _sizeAndPositionMap)
			{
				SizeAndPositionStateSO value = item.Value;
				value.OnDoneEditing = (UnityAction)Delegate.Combine(value.OnDoneEditing, new UnityAction(DoneEditing));
			}
			_tabMap.Add(IATFComponent.Clutch1, clutch1Tab);
			_tabMap.Add(IATFComponent.Clutch2, clutch2Tab);
			_tabMap.Add(IATFComponent.Ring1, ring1Tab);
			_tabMap.Add(IATFComponent.Ring2, ring2Tab);
			_tabMap.Add(IATFComponent.Ring3, ring3Tab);
			foreach (KeyValuePair<IATFComponent, Button> component in _tabMap)
			{
				component.Value.onClick.AddListener(delegate
				{
					ShowControlsForTab(component.Key);
				});
			}
			adminEvents.OnAdminPanelOpened += ShowTabs;
			adminEvents.OnAdminPanelClosed += HideAdminControls;
		}

		protected void OnDisable()
		{
			adminEvents.OnAdminPanelOpened -= ShowTabs;
			adminEvents.OnAdminPanelClosed -= HideAdminControls;
			foreach (KeyValuePair<IATFComponent, Button> item in _tabMap)
			{
				item.Value.onClick.RemoveAllListeners();
			}
			foreach (KeyValuePair<IATFComponent, SizeAndPositionStateSO> item2 in _sizeAndPositionMap)
			{
				SizeAndPositionStateSO value = item2.Value;
				value.OnDoneEditing = (UnityAction)Delegate.Remove(value.OnDoneEditing, new UnityAction(DoneEditing));
			}
		}

		private void DestroyCurrentControls()
		{
			_loadedControls.ForEach(UnityEngine.Object.DestroyImmediate);
			_loadedControls.Clear();
		}

		private void DoneEditing()
		{
			DestroyCurrentControls();
			ShowTabs();
		}

		private void HideAdminControls()
		{
			tabPanel.SetActive(value: false);
			DestroyCurrentControls();
		}

		private void HideTabs()
		{
			tabPanel.SetActive(value: false);
		}

		private void ShowControlsForTab(IATFComponent component)
		{
			DestroyCurrentControls();
			switch (component)
			{
			case IATFComponent.Ring1:
			case IATFComponent.Ring2:
			case IATFComponent.Ring3:
				_loadedControls.Add(UnityEngine.Object.Instantiate(sizeControlsPrefab, topPanel.transform));
				break;
			case IATFComponent.Clutch1:
				_loadedControls.Add(UnityEngine.Object.Instantiate(sizeControlsPrefab, centerPanel.transform));
				_loadedControls.Add(UnityEngine.Object.Instantiate(positionControlsPrefab, rightPanel.transform));
				break;
			case IATFComponent.Clutch2:
				_loadedControls.Add(UnityEngine.Object.Instantiate(sizeControlsPrefab, centerPanel.transform));
				_loadedControls.Add(UnityEngine.Object.Instantiate(positionControlsPrefab, leftPanel.transform));
				break;
			}
			_selectedSizeAndPositionState = _sizeAndPositionMap[component];
			_loadedControls.ForEach(delegate(GameObject control)
			{
				control.GetComponent<IATFResizeAndPositionController>().selectedSizeAndPositionState = _selectedSizeAndPositionState;
			});
			HideTabs();
		}

		private void ShowTabs()
		{
			tabPanel.SetActive(value: true);
		}
	}
}
