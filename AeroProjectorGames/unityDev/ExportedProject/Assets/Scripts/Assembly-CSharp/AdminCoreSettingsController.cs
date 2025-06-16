using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using Admin_Panel.Events;
using ConfirmationModal;
using Helpers;
using ResizingAndMoving;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdminCoreSettingsController : MonoBehaviour
{
	private const string CUSTOM_LOGO_NAME = "custom-logo";

	private const int PIN_MINIMUM_LENGTH = 4;

	private const string PIN_SAVE_MESSAGE = "Pin saved.";

	private const string PIN_VALIDATION_LENGTH_ERROR = "Pin is too short, minimum 4 characters required";

	[SerializeField]
	private TMP_InputField pinTextInput;

	[SerializeField]
	private TMP_Text pinMessage;

	private Sprite _currentLogoSprite;

	private Vector2 _currentImageDefaultSize;

	private Vector2 _previewImageDefaultSize;

	private string _previewImageFileNameString;

	private byte[] _previewImageToSaveByte;

	[SerializeField]
	private Button applyImageButton;

	[SerializeField]
	private Image currentImage;

	[SerializeField]
	private Texture2D defaultLogoTexture;

	[SerializeField]
	private Button getImageButton;

	[SerializeField]
	private Image previewImage;

	[SerializeField]
	private TMP_InputField urlInputField;

	[Header("External References")]
	[SerializeField]
	private Image displayLogo;

	[SerializeField]
	private SizeAndPositionStateSO sizeAndPositionState;

	[Space]
	[Header("Events")]
	[SerializeField]
	private AdminEventsSO adminEvents;

	[SerializeField]
	private ConfirmationModalEventsSO confirmationEvents;

	private void Awake()
	{
		_currentImageDefaultSize = currentImage.rectTransform.sizeDelta;
		_previewImageDefaultSize = _currentImageDefaultSize;
	}

	private void OnEnable()
	{
		adminEvents.OnAdminPinReset += LoadAdminPin;
		SettingsStore.Admin.Save();
		LoadAdminPin();
		GetCurrentLogoSettings();
	}

	private void CheckForExistingLogo()
	{
		string text = Directory.GetFiles(DataPathHelpers.GetApplicationDataPath(), "custom-logo.*").FirstOrDefault();
		if (text != null)
		{
			SettingsStore.Logo.FileNameWithExtension = Path.GetFileName(text);
		}
	}

	private void CleanLogos()
	{
		string[] files = Directory.GetFiles(DataPathHelpers.GetApplicationDataPath(), "custom-logo.*");
		for (int num = files.Length - 1; num >= 0; num--)
		{
			File.Delete(files[num]);
		}
	}

	private Sprite CreateCustomSprite(Texture2D useTexture)
	{
		return Sprite.Create(useTexture, new Rect(0f, 0f, useTexture.width, useTexture.height), new Vector2(0f, 0f));
	}

	private IEnumerator DownloadImage(string webAddress)
	{
		UnityWebRequest uwr = new UnityWebRequest(webAddress)
		{
			downloadHandler = new DownloadHandlerBuffer()
		};
		yield return uwr.SendWebRequest();
		if (uwr.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(uwr.error);
			yield break;
		}
		string[] array = uwr.GetResponseHeaders()["Content-Type"].Split(new string[1] { "/" }, StringSplitOptions.None);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("custom-logo");
		if (array.Length <= 2)
		{
			stringBuilder.Append(".");
			stringBuilder.Append(array[1]);
		}
		byte[] data = uwr.downloadHandler.data;
		_previewImageFileNameString = stringBuilder.ToString();
		_previewImageToSaveByte = data;
		LoadPreviewImage(data);
	}

	private Texture2D GetCurrentImage(string fileName)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Path.Combine(DataPathHelpers.GetApplicationDataPath(), fileName));
		byte[] data = File.ReadAllBytes(stringBuilder.ToString());
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(data);
		return texture2D;
	}

	private void GetCurrentLogoSettings(bool logoIsNew = false)
	{
		CheckForExistingLogo();
		string fileNameWithExtension = SettingsStore.Logo.FileNameWithExtension;
		if (fileNameWithExtension != null && fileNameWithExtension.Length > 0)
		{
			try
			{
				Texture2D useTexture = GetCurrentImage(SettingsStore.Logo.FileNameWithExtension);
				_currentLogoSprite = CreateCustomSprite(useTexture);
			}
			catch
			{
				_currentLogoSprite = CreateCustomSprite(defaultLogoTexture);
			}
		}
		else
		{
			_currentLogoSprite = CreateCustomSprite(defaultLogoTexture);
		}
		if (logoIsNew)
		{
			Vector2 imageSizeToRetainSpriteProportions = RatioSizeHelpersr.GetImageSizeToRetainSpriteProportions(displayLogo, _currentLogoSprite);
			sizeAndPositionState.SetSize(imageSizeToRetainSpriteProportions.x, imageSizeToRetainSpriteProportions.y);
			sizeAndPositionState.RaiseDoneEditing();
		}
		displayLogo.sprite = _currentLogoSprite;
		currentImage.rectTransform.sizeDelta = _currentImageDefaultSize;
		currentImage.rectTransform.sizeDelta = RatioSizeHelpersr.GetImageSizeToRetainSpriteProportions(currentImage, _currentLogoSprite);
		currentImage.sprite = _currentLogoSprite;
	}

	private void LoadAdminPin()
	{
		pinTextInput.text = SettingsStore.Admin.Pin;
		pinMessage.text = string.Empty;
	}

	private void LoadPreviewImage(byte[] loadImage)
	{
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(loadImage);
		Sprite sprite = CreateCustomSprite(texture2D);
		previewImage.rectTransform.sizeDelta = _previewImageDefaultSize;
		previewImage.rectTransform.sizeDelta = RatioSizeHelpersr.GetImageSizeToRetainSpriteProportions(previewImage, sprite);
		previewImage.sprite = sprite;
	}

	public void ExitApplication()
	{
		confirmationEvents.RaiseConfirmationModal(Application.Quit, "Exit the application?");
	}

	public void GetImageFromWeb()
	{
		string text = urlInputField.text.Trim();
		if (text == string.Empty)
		{
			Debug.Log("Add user notification of empty input field");
		}
		else
		{
			StartCoroutine(DownloadImage(text));
		}
	}

	public void ResetCoreSettings()
	{
		_previewImageFileNameString = string.Empty;
		_previewImageToSaveByte = null;
		previewImage.rectTransform.sizeDelta = _previewImageDefaultSize;
		previewImage.sprite = null;
		urlInputField.text = string.Empty;
	}

	public void SaveAdminPin()
	{
		if (pinTextInput.text.Length < 4)
		{
			pinMessage.text = "Pin is too short, minimum 4 characters required";
			return;
		}
		SettingsStore.Admin.Pin = pinTextInput.text;
		SettingsStore.Admin.Save();
		pinMessage.text = "Pin saved.";
	}

	public void SaveCustomLogo()
	{
		if (string.IsNullOrEmpty(_previewImageFileNameString) || _previewImageToSaveByte == null)
		{
			Debug.Log("Add user notification - unable to save logo");
			return;
		}
		string path = Path.Combine(DataPathHelpers.GetApplicationDataPath(), _previewImageFileNameString);
		CleanLogos();
		File.WriteAllBytes(path, _previewImageToSaveByte);
		SettingsStore.Logo.FileNameWithExtension = _previewImageFileNameString;
		SettingsStore.Logo.Save();
		GetCurrentLogoSettings(logoIsNew: true);
		ResetCoreSettings();
	}

	public void ToggleAdminPinVisibility()
	{
		if (pinTextInput.inputType == TMP_InputField.InputType.Password)
		{
			pinTextInput.inputType = TMP_InputField.InputType.Standard;
			pinTextInput.ForceLabelUpdate();
		}
		else
		{
			pinTextInput.inputType = TMP_InputField.InputType.Password;
		}
		pinTextInput.ForceLabelUpdate();
	}
}
