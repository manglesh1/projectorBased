using System.Collections.Generic;
using Assets.Scripts.Detection.ScriptableObjects;
using Detection;
using Detection.ScriptableObjects;
using Extensions;
using Settings;
using UnityEngine;
using UnityEngine.UI;

public class RealSenseWizardController : MonoBehaviour
{
	[Header("Navigation Buttons")]
	[SerializeField]
	private Button backButton;

	[SerializeField]
	private Button cancelButton;

	[SerializeField]
	private Button finishButton;

	[SerializeField]
	private Button nextButton;

	[Header("Page Collection")]
	[SerializeField]
	private List<GameObject> contentPages;

	[Header("Events")]
	[SerializeField]
	private RealSenseWizardEventsSO wizardEvents;

	[Header("Scriptable Objects")]
	[SerializeField]
	private RealSenseCameraSettingsSO cameraSettings;

	[SerializeField]
	private CoordinatesSO gameBoardCoordinates;

	[SerializeField]
	private RoiCoordinatesSO roiCoordinates;

	private int _contentPageIndex;

	private CoordinatesSO _originalCoordinates;

	private bool _originalCsEmitterEnabled;

	private float _originalCsExposure;

	private float _originalCsLaserPower;

	private float _originalCsMaxDistance;

	private float _originalCsMinDistance;

	private DetectionSettings _originalDetectionSettings;

	private RoiCoordinatesSO _originalRoiCoordinates;

	private bool _roadBlocked;

	private void backButton_OnClick()
	{
		UpdateNavigationContent(forward: false);
		UpdateNavigationButtons();
	}

	private void cancelButton_OnClick()
	{
		RevertChanges();
		finishButton.onClick?.Invoke();
	}

	private void nextButton_OnClick()
	{
		UpdateNavigationContent(forward: true);
		UpdateNavigationButtons();
	}

	private void wizardEvents_OnCameraPoweredOn()
	{
		SettingsStore.DetectionSettings.DetectionEnabled = true;
	}

	private void wizardEvents_OnMaxValueChanged(float value)
	{
		cameraSettings.MaxDistance = value;
		SettingsStore.DetectionSettings.MaxDistance = value;
	}

	private void wizardEvents_OnMinValueChanged(float value)
	{
		cameraSettings.MinDistance = value;
		SettingsStore.DetectionSettings.MinDistance = value;
	}

	private void wizardEvents_OnRoadBlocked()
	{
		_roadBlocked = true;
		HideNextButton();
	}

	private void wizardEvents_OnRoadBlockResolved()
	{
		_roadBlocked = false;
		ShowNextButton();
	}

	private void BuildUpEvents()
	{
		backButton.onClick.AddListener(backButton_OnClick);
		cancelButton.onClick.AddListener(cancelButton_OnClick);
		nextButton.onClick.AddListener(nextButton_OnClick);
		wizardEvents.OnCameraPoweredOn += wizardEvents_OnCameraPoweredOn;
		wizardEvents.OnMaxValueChanged += wizardEvents_OnMaxValueChanged;
		wizardEvents.OnMinValueChanged += wizardEvents_OnMinValueChanged;
		wizardEvents.OnRoadBlocked += wizardEvents_OnRoadBlocked;
		wizardEvents.OnRoadBlockResolved += wizardEvents_OnRoadBlockResolved;
	}

	private void DisplayFirstPage()
	{
		if (contentPages.Count > 0)
		{
			contentPages[0].SetActive(value: true);
		}
	}

	private void HideAllContent()
	{
		for (int i = 0; i < contentPages.Count; i++)
		{
			contentPages[i].SetActive(value: false);
		}
	}

	private void HideBackButton()
	{
		backButton.gameObject.SetActive(value: false);
	}

	private void HideFinishButton()
	{
		finishButton.gameObject.SetActive(value: false);
	}

	private void HideNextButton()
	{
		nextButton.gameObject.SetActive(value: false);
	}

	private void InitializeOriginalVariables()
	{
		_originalCsEmitterEnabled = cameraSettings.EmitterEnabled;
		_originalCsExposure = cameraSettings.Exposure;
		_originalCsLaserPower = cameraSettings.LaserPower;
		_originalCsMaxDistance = cameraSettings.MaxDistance;
		_originalCsMinDistance = cameraSettings.MinDistance;
		_originalDetectionSettings = SettingsStore.DetectionSettings.SimpleJsonClone();
		_originalCoordinates = ScriptableObject.CreateInstance<CoordinatesSO>();
		_originalCoordinates.BottomLeft = gameBoardCoordinates.BottomLeft;
		_originalCoordinates.BottomRight = gameBoardCoordinates.BottomRight;
		_originalCoordinates.TopLeft = gameBoardCoordinates.TopLeft;
		_originalCoordinates.TopRight = gameBoardCoordinates.TopRight;
		_originalRoiCoordinates = ScriptableObject.CreateInstance<RoiCoordinatesSO>();
		_originalRoiCoordinates.BottomRight = roiCoordinates.BottomRight;
		_originalRoiCoordinates.TopLeft = roiCoordinates.TopLeft;
	}

	private bool IsFirstPage()
	{
		return _contentPageIndex == 0;
	}

	private bool IsLastPage()
	{
		return _contentPageIndex == contentPages.Count - 1;
	}

	private void RevertChanges()
	{
		cameraSettings.EmitterEnabled = _originalCsEmitterEnabled;
		cameraSettings.Exposure = _originalCsExposure;
		cameraSettings.LaserPower = _originalCsLaserPower;
		cameraSettings.MaxDistance = _originalCsMaxDistance;
		cameraSettings.MinDistance = _originalCsMinDistance;
		gameBoardCoordinates.BottomLeft = _originalCoordinates.BottomLeft;
		gameBoardCoordinates.BottomRight = _originalCoordinates.BottomRight;
		gameBoardCoordinates.TopLeft = _originalCoordinates.TopLeft;
		gameBoardCoordinates.TopRight = _originalCoordinates.TopRight;
		roiCoordinates.BottomRight = _originalRoiCoordinates.BottomRight;
		roiCoordinates.TopLeft = _originalRoiCoordinates.TopLeft;
		SettingsStore.DetectionSettings.EmitterEnabled = _originalDetectionSettings.EmitterEnabled;
		SettingsStore.DetectionSettings.Exposure = _originalDetectionSettings.Exposure;
		SettingsStore.DetectionSettings.LaserPower = _originalDetectionSettings.LaserPower;
		SettingsStore.DetectionSettings.DetectionEnabled = _originalDetectionSettings.DetectionEnabled;
		SettingsStore.DetectionSettings.MaxDistance = _originalDetectionSettings.MaxDistance;
		SettingsStore.DetectionSettings.MinDistance = _originalDetectionSettings.MinDistance;
	}

	private void ShowBackButton()
	{
		backButton.gameObject.SetActive(value: true);
	}

	private void ShowFinishButton()
	{
		finishButton.gameObject.SetActive(value: true);
	}

	private void ShowNextButton()
	{
		nextButton.gameObject.SetActive(value: true);
	}

	private void TearDownEvents()
	{
		backButton.onClick.RemoveAllListeners();
		cancelButton.onClick.RemoveAllListeners();
		nextButton.onClick.RemoveAllListeners();
		wizardEvents.OnCameraPoweredOn -= wizardEvents_OnCameraPoweredOn;
		wizardEvents.OnMaxValueChanged -= wizardEvents_OnMaxValueChanged;
		wizardEvents.OnMinValueChanged -= wizardEvents_OnMinValueChanged;
		wizardEvents.OnRoadBlocked -= wizardEvents_OnRoadBlocked;
		wizardEvents.OnRoadBlockResolved -= wizardEvents_OnRoadBlockResolved;
	}

	private void UpdateNavigationButtons()
	{
		if (IsFirstPage())
		{
			HideBackButton();
			HideFinishButton();
			ShowNextButton();
		}
		else
		{
			ShowBackButton();
			HideFinishButton();
			if (!_roadBlocked)
			{
				ShowNextButton();
			}
		}
		if (IsLastPage())
		{
			ShowFinishButton();
			HideNextButton();
		}
	}

	private void UpdateNavigationContent(bool forward)
	{
		contentPages[_contentPageIndex].SetActive(value: false);
		if (forward)
		{
			_contentPageIndex++;
			if (_contentPageIndex >= contentPages.Count)
			{
				_contentPageIndex = contentPages.Count - 1;
			}
		}
		else
		{
			_contentPageIndex--;
			if (_contentPageIndex < 0)
			{
				_contentPageIndex = 0;
			}
		}
		contentPages[_contentPageIndex].SetActive(value: true);
	}

	private void OnDisable()
	{
		TearDownEvents();
	}

	private void OnEnable()
	{
		BuildUpEvents();
		InitializeOriginalVariables();
		HideAllContent();
		DisplayFirstPage();
		_contentPageIndex = 0;
		_roadBlocked = false;
		UpdateNavigationButtons();
	}
}
