using System;
using System.Collections;
using System.IO;
using System.Text;
using Helpers;
using Settings;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminSafetyVideoController : MonoBehaviour
{
	private const float BYTES_TO_MEGABYTES = 1048576f;

	private const string SAFETY_VIDEO_NAME = "safetyVideo";

	private const string EXCEEDS_MAX_FILE_SIZE_MESSAGE = "File exceeds 250mb, Select a different video";

	private const float MAX_FILE_SIZE_IN_MEGABYTES = 250f;

	private const string SUCCESS_MESSAGE = "Video successfully added";

	private string _fileLocationAndName;

	private SafetyVideoSettings _safetyVideoSettings;

	[SerializeField]
	private Toggle isDisabledToggle;

	[SerializeField]
	private TMP_Text userMessageText;

	private void OnEnable()
	{
		FileBrowser.SetFilters(false, new FileBrowser.Filter("Video", ".ogv", ".webm"));
		FileBrowser.SetDefaultFilter(".ogv");
		userMessageText.text = string.Empty;
		GetSafetyVideoSettings();
	}

	private void CleanSafetyVideos()
	{
		string[] files = Directory.GetFiles(DataPathHelpers.GetApplicationDataPath(), "safetyVideo.*");
		for (int num = files.Length - 1; num >= 0; num--)
		{
			File.Delete(files[num]);
		}
	}

	private void GetSafetyVideoSettings()
	{
		_safetyVideoSettings = SettingsStore.SafetyVideo;
		isDisabledToggle.isOn = _safetyVideoSettings.IsDisabled;
		_fileLocationAndName = _safetyVideoSettings.FileLocationAndName;
	}

	private void SaveSafetyVideoSettings()
	{
		_safetyVideoSettings.IsDisabled = isDisabledToggle.isOn;
		_safetyVideoSettings.FileLocationAndName = _fileLocationAndName;
		_safetyVideoSettings.Save();
	}

	private IEnumerator ShowFileExplorerAndGetFile()
	{
		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, allowMultiSelection: false, "", "", "Select Safety Video");
		if (FileBrowser.Success)
		{
			float num = (float)new FileInfo(Path.Combine(FileBrowserHelpers.GetDirectoryName(FileBrowser.Result[0]), FileBrowserHelpers.GetFilename(FileBrowser.Result[0]))).Length / 1048576f;
			if (250f < num)
			{
				userMessageText.text = "File exceeds 250mb, Select a different video";
				yield break;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("safetyVideo");
			stringBuilder.Append(".");
			stringBuilder.Append(FileBrowserHelpers.GetFilename(FileBrowser.Result[0]).Split(new string[1] { "." }, StringSplitOptions.None)[1]);
			string text = Path.Combine(DataPathHelpers.GetApplicationDataPath(), stringBuilder.ToString());
			CleanSafetyVideos();
			byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
			FileBrowserHelpers.WriteBytesToFile(text, bytes);
			_fileLocationAndName = text;
			SaveSafetyVideoSettings();
			userMessageText.text = "Video successfully added";
		}
	}

	public void SetSafetyVideo()
	{
		StartCoroutine(ShowFileExplorerAndGetFile());
	}

	public void ToggleDisabled()
	{
		SaveSafetyVideoSettings();
	}
}
