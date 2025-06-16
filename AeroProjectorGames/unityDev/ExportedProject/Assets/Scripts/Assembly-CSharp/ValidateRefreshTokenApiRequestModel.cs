using System;
using Detection.Models;
using Settings;
using UnityEngine;

[Serializable]
public class ValidateRefreshTokenApiRequestModel
{
	public string clientPlatform;

	public string clientVersion;

	public bool hasDetection;

	public bool hasTouchScreen;

	public string licenseKey;

	public string refreshToken;

	public bool usesDetection;

	public bool usesTouchScreen;

	public ValidateRefreshTokenApiRequestModel(string key, string token, string platform, string version)
	{
		clientPlatform = platform;
		clientVersion = version;
		hasDetection = SettingsStore.DetectionSettings.DetectedCamera > DetectedCameraEnum.None;
		hasTouchScreen = Display.displays.Length > 1;
		licenseKey = key;
		refreshToken = token;
		usesDetection = SettingsStore.DetectionSettings.DetectionEnabled && SettingsStore.DetectionSettings.DetectedCamera > DetectedCameraEnum.None;
		usesTouchScreen = SettingsStore.Interaction.MultiDisplayEnabled && Display.displays.Length > 1;
	}
}
