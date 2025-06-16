using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using API;
using API.ResponseModels;
using Backgrounds.CustomBackgrounds.ApiResponses;
using Helpers;
using ResizingAndMoving;
using Settings;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Backgrounds
{
	public class BackgroundManager : MonoBehaviour
	{
		private BackgroundSettings _settings;

		[Header("Backgrounds SO")]
		[SerializeField]
		private BackgroundsSO backgrounds;

		[Header("Predefined Background Color References")]
		[SerializeField]
		private List<BackgroundColor> predefinedColors;

		[Header("Predefined Background Image References")]
		[SerializeField]
		private List<Texture2D> predefinedBackgroundImages;

		[Header("Target Dimmer Size and Position")]
		[SerializeField]
		private SizeAndPositionStateSO targetDimmerSizeAndPositionState;

		[Header("Custom Background Refs")]
		[SerializeField]
		private AxcitementApiHandler api;

		[SerializeField]
		private BackgroundEventsSO backgroundEvents;

		[Header("Setting Events")]
		[SerializeField]
		private SettingsEventsSO settingEvents;

		private void OnDisable()
		{
			SizeAndPositionStateSO sizeAndPositionStateSO = targetDimmerSizeAndPositionState;
			sizeAndPositionStateSO.OnReset = (UnityAction)Delegate.Remove(sizeAndPositionStateSO.OnReset, new UnityAction(LoadTargetDimmerSettings));
			backgroundEvents.OnUpdateCustomBackgroundsRequest -= GetCustomBackgroundsApiRequest;
			settingEvents.OnSettingsReloaded -= SettingsReloaded;
		}

		public void OnEnable()
		{
			_settings = SettingsStore.Backgrounds;
			LoadTargetDimmerSettings();
			LoadPredefinedColors();
			LoadPredefinedBackgroundImages();
			SavePredefinedBackgroundsToSettings();
			LoadCustomBackgrounds();
			SizeAndPositionStateSO sizeAndPositionStateSO = targetDimmerSizeAndPositionState;
			sizeAndPositionStateSO.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO.OnReset, new UnityAction(LoadTargetDimmerSettings));
			backgroundEvents.OnUpdateCustomBackgroundsRequest += GetCustomBackgroundsApiRequest;
			settingEvents.OnSettingsReloaded -= SettingsReloaded;
		}

		private void SettingsReloaded()
		{
			LoadTargetDimmerSettings();
		}

		private IEnumerator DownloadAndSaveImage(List<string> urls)
		{
			foreach (string url in urls)
			{
				string filePath = Path.Combine(path2: Path.GetFileNameWithoutExtension(url) + ".png", path1: DataPathHelpers.GetCustomImagesDirectory());
				if (!File.Exists(filePath))
				{
					UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
					yield return www.SendWebRequest();
					byte[] png = DownloadHandlerTexture.GetContent(www).EncodeToPNG();
					if (png.Length != 0)
					{
						File.WriteAllBytes(filePath, png);
					}
				}
			}
			PruneLocalCustomBackgrounds(urls);
			SaveCustomBackgroundsToSettings();
			LoadCustomBackgrounds();
			backgroundEvents.RaiseOnCustomBackgroundsUpdated();
		}

		private void GetCustomBackgroundsApiRequest()
		{
			StartCoroutine(api.GetCustomBackgrounds(HandleCustomBackgroundsApiResponse));
		}

		private void HandleCustomBackgroundsApiResponse(ApiResponse<GetCustomBackgroundsApiResponse> response)
		{
			if (response.Result != UnityWebRequest.Result.Success)
			{
				backgroundEvents.RaiseOnCustomBackgroundsUpdated();
				return;
			}
			bool flag = true;
			foreach (BackgroundImageResponseData datum in response.Data.Data)
			{
				if (!ApiSecretSigner.IsValidData(datum.BlobUrl, datum.DataResponseId, ApiSecretSignerType.GeneralDataKey))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				StartCoroutine(DownloadAndSaveImage(response.Data.Data.Select((BackgroundImageResponseData s) => s.BlobUrl).ToList()));
			}
		}

		private void LoadCustomBackgrounds()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, Texture2D> loadedBackgroundImage in backgrounds.LoadedBackgroundImages)
			{
				KeyValuePair<string, Texture2D> loadedBackground = loadedBackgroundImage;
				if (!SettingsStore.Backgrounds.Backgrounds.Any((BackgroundSetting bg) => bg.Name == loadedBackground.Key))
				{
					list.Add(loadedBackground.Key);
				}
			}
			list.ForEach(backgrounds.RemoveBackground);
			foreach (BackgroundSetting item in SettingsStore.Backgrounds.Backgrounds.FindAll((BackgroundSetting bg) => bg.BackgroundStyle == BackgroundStyleEnum.CustomImage))
			{
				string path = item.Name + ".png";
				if (File.Exists(Path.Combine(DataPathHelpers.GetCustomImagesDirectory(), path)))
				{
					byte[] data = File.ReadAllBytes(Path.Combine(DataPathHelpers.GetCustomImagesDirectory(), path));
					Texture2D texture2D = new Texture2D(2, 2);
					texture2D.LoadImage(data);
					backgrounds.AddBackground(item.Name, texture2D);
				}
			}
		}

		private void LoadTargetDimmerSettings()
		{
			targetDimmerSizeAndPositionState.SetPosition(_settings.TargetDimmer.PositionX, _settings.TargetDimmer.PositionY);
			targetDimmerSizeAndPositionState.SetSize(_settings.TargetDimmer.Width, _settings.TargetDimmer.Height);
		}

		private void LoadPredefinedColors()
		{
			predefinedColors.ForEach(delegate(BackgroundColor color)
			{
				backgrounds.AddColor(color.name, color.color);
			});
		}

		private void LoadPredefinedBackgroundImages()
		{
			predefinedBackgroundImages.ForEach(delegate(Texture2D texture)
			{
				backgrounds.AddBackground(texture.name, texture);
			});
		}

		private void PruneLocalCustomBackgrounds(List<string> urls)
		{
			string[] files = Directory.GetFiles(DataPathHelpers.GetCustomImagesDirectory());
			foreach (string path in files)
			{
				string localFileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
				if (!urls.Any((string u) => u.Contains(localFileNameWithoutExtension)))
				{
					File.Delete(path);
				}
			}
		}

		private void SaveCustomBackgroundsToSettings()
		{
			List<BackgroundSetting> list = SettingsStore.Backgrounds.Backgrounds.FindAll((BackgroundSetting bg) => bg.BackgroundStyle == BackgroundStyleEnum.CustomImage);
			string[] files = Directory.GetFiles(DataPathHelpers.GetCustomImagesDirectory());
			string[] array = files;
			foreach (string path in array)
			{
				string backgroundName = Path.GetFileNameWithoutExtension(path);
				if (!list.Any((BackgroundSetting bg) => bg.Name == backgroundName))
				{
					SettingsStore.Backgrounds.Backgrounds.Add(new BackgroundSetting
					{
						BackgroundStyle = BackgroundStyleEnum.CustomImage,
						Name = Path.GetFileNameWithoutExtension(path)
					});
				}
			}
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				BackgroundSetting customBackgroundSetting = list[num2];
				if (!files.Any((string file) => file.Contains(customBackgroundSetting.Name)))
				{
					SettingsStore.Backgrounds.Backgrounds.Remove(customBackgroundSetting);
				}
			}
			SettingsStore.Backgrounds.Save();
		}

		private void SavePredefinedBackgroundsToSettings()
		{
			List<BackgroundSetting> storedPredefinedColors = SettingsStore.Backgrounds.Backgrounds.FindAll((BackgroundSetting b) => b.BackgroundStyle == BackgroundStyleEnum.PredefinedColor);
			List<BackgroundSetting> storedPredefinedImages = SettingsStore.Backgrounds.Backgrounds.FindAll((BackgroundSetting b) => b.BackgroundStyle == BackgroundStyleEnum.PredefinedImage);
			int i;
			for (i = storedPredefinedColors.Count - 1; i >= 0; i--)
			{
				if (!predefinedColors.Exists((BackgroundColor b) => b.name == storedPredefinedColors[i].Name))
				{
					SettingsStore.Backgrounds.Backgrounds.Remove(storedPredefinedColors[i]);
				}
			}
			int i2;
			for (i2 = storedPredefinedImages.Count - 1; i2 >= 0; i2--)
			{
				if (!predefinedBackgroundImages.Exists((Texture2D b) => b.name == storedPredefinedImages[i2].Name))
				{
					SettingsStore.Backgrounds.Backgrounds.Remove(storedPredefinedImages[i2]);
				}
			}
			foreach (BackgroundColor predefinedColor2 in predefinedColors)
			{
				BackgroundColor predefinedColor = predefinedColor2;
				if (!SettingsStore.Backgrounds.Backgrounds.Exists((BackgroundSetting b) => b.Name == predefinedColor.name))
				{
					SettingsStore.Backgrounds.Backgrounds.Add(new BackgroundSetting
					{
						Alpha = 1f,
						BackgroundStyle = BackgroundStyleEnum.PredefinedColor,
						ColorHexValue = predefinedColor.color.ToHexString(),
						Name = predefinedColor.name
					});
				}
			}
			foreach (Texture2D predefinedBackgroundImage2 in predefinedBackgroundImages)
			{
				Texture2D predefinedBackgroundImage = predefinedBackgroundImage2;
				if (!SettingsStore.Backgrounds.Backgrounds.Exists((BackgroundSetting b) => b.Name == predefinedBackgroundImage.name))
				{
					SettingsStore.Backgrounds.Backgrounds.Add(new BackgroundSetting
					{
						BackgroundStyle = BackgroundStyleEnum.PredefinedImage,
						Name = predefinedBackgroundImage.name
					});
				}
			}
		}
	}
}
