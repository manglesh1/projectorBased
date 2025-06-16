using System;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminBackupSettingsController : MonoBehaviour
{
	[Header("Data Commands")]
	[SerializeField]
	private GetBackupSettingsCommand getBackupSettings;

	[SerializeField]
	private GetLastBackupSettingsDateTimeCommand getLastBackupDate;

	[SerializeField]
	private SaveBackupSettingsCommand saveBackupSettings;

	[Header("Settings Commands")]
	[SerializeField]
	private RestoreSettingsCommand restoreSettings;

	[Header("UI Components")]
	[SerializeField]
	private Button backupButton;

	[SerializeField]
	private TMP_Text dateValue;

	[SerializeField]
	private TMP_Text progressText;

	[SerializeField]
	private Button restoreButton;

	private const string BAD_SAVE_RESPONSE = "Failed to save backups. Please try again later.";

	private const string DEFAULT_BACKUP_TIME = "No backups found";

	private const string FORMAT_FULL_DATE_TIME = "MMMM d, yyyy h:mm:ss tt";

	private const string TEXT_LOADING = "Loading data";

	private const string TEXT_BACKUP_FINISHED = "Settings saved successfully";

	private const string TEXT_BACKUP_START = "Saving settings";

	private const string TEXT_RESTORE_ERROR = "Backup not restored";

	private const string TEXT_RESTORE_FINISHED = "Settings restored successfully";

	private const string TEXT_RESTORE_START = "Loading settings";

	private void OnDisable()
	{
		backupButton.onClick.RemoveListener(HandleBackupClick);
		restoreButton.onClick.RemoveListener(HandleRestoreClick);
	}

	private void OnEnable()
	{
		backupButton.onClick.AddListener(HandleBackupClick);
		restoreButton.onClick.AddListener(HandleRestoreClick);
		dateValue.SetText("Loading data");
		progressText.SetText(string.Empty);
		getLastBackupDate.Execute(HandleLastBackupDateResponse);
	}

	private void HandleBackupClick()
	{
		progressText.SetText("Saving settings");
		saveBackupSettings.Execute(HandleSaveBackupSettingsResponse);
	}

	private void HandleRestoreClick()
	{
		progressText.SetText("Loading settings");
		getBackupSettings.Execute(HandleGetBackupSettingsResponse);
	}

	private void HandleGetBackupSettingsResponse(string response)
	{
		if (!string.IsNullOrEmpty(response))
		{
			restoreSettings.Execute(response);
			progressText.SetText("Settings restored successfully");
		}
		else
		{
			progressText.SetText("Backup not restored");
		}
	}

	private void HandleLastBackupDateResponse(DateTime? response)
	{
		SetDateValue(response);
	}

	private void HandleSaveBackupSettingsResponse(bool response, DateTime? lastUpdate)
	{
		if (response)
		{
			SetDateValue(lastUpdate);
			progressText.SetText("Settings saved successfully");
		}
		else
		{
			progressText.SetText("Failed to save backups. Please try again later.");
		}
	}

	private void SetDateValue(DateTime? value)
	{
		if (value.HasValue)
		{
			dateValue.SetText("Last backup: " + value.Value.ToString("MMMM d, yyyy h:mm:ss tt"));
		}
		else
		{
			dateValue.SetText("No backups found");
		}
	}
}
