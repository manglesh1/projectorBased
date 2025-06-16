using System.IO;
using Admin_Panel;
using Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class SafetyVideoPlayer : MonoBehaviour
{
	private SafetyVideoSettings _safetyVideoSettings;

	[SerializeField]
	private GameObject closeVideoButtonObject;

	[SerializeField]
	private RawImage videoPlayerBackground;

	[SerializeField]
	private RawImage videoPlayerImage;

	[SerializeField]
	private VideoPlayer videoPlayerComponent;

	[Header("External References")]
	[SerializeField]
	private AdminManagerScript adminManagerScript;

	public UnityEvent SafetyVideoFinishedPlaying = new UnityEvent();

	private void Awake()
	{
		adminManagerScript.AdminPanelOpened.AddListener(ShowCloseButton);
	}

	private void OnEnable()
	{
		ShowCloseButton();
		videoPlayerImage.enabled = false;
		videoPlayerBackground.enabled = false;
		videoPlayerComponent.loopPointReached += VideoFinished;
	}

	private void OnDisable()
	{
		videoPlayerComponent.loopPointReached -= VideoFinished;
	}

	private void Update()
	{
		if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.vKey.wasPressedThisFrame && !SettingsStore.SafetyVideo.IsDisabled)
		{
			PlaySafetyVideo();
		}
	}

	public void PlaySafetyVideo()
	{
		_safetyVideoSettings = SettingsStore.SafetyVideo;
		string fileLocationAndName = _safetyVideoSettings.FileLocationAndName;
		if (File.Exists(fileLocationAndName))
		{
			videoPlayerImage.enabled = true;
			videoPlayerBackground.enabled = true;
			videoPlayerComponent.url = fileLocationAndName;
			videoPlayerComponent.Play();
			if (adminManagerScript.CurrentAdminState == AdminPanelStates.AdminPanel)
			{
				ShowCloseButton(isShown: true);
			}
			else
			{
				ShowCloseButton();
			}
		}
		else
		{
			SafetyVideoFinishedPlaying?.Invoke();
		}
	}

	private void ShowCloseButton(bool isShown = false)
	{
		if (videoPlayerImage.IsActive())
		{
			closeVideoButtonObject.SetActive(isShown);
		}
		else
		{
			closeVideoButtonObject.SetActive(value: false);
		}
	}

	private void VideoFinished(VideoPlayer vp)
	{
		videoPlayerComponent.Stop();
		videoPlayerImage.enabled = false;
		videoPlayerBackground.enabled = false;
		SafetyVideoFinishedPlaying?.Invoke();
	}

	public void StopVideoButtonClicked()
	{
		VideoFinished(null);
		ShowCloseButton();
	}
}
