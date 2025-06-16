using Authentication;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
	public class NetworkStatusModalController : MonoBehaviour
	{
		private const string DAYS_REMAINING = "days remaining";

		private const string NETWORK_STATUS_TEXT = "You do not have an internet connection";

		private const string OK = "Ok";

		private const string SNOOZE = "Snooze";

		[Header("References")]
		[SerializeField]
		private AuthenticationStateSO authState;

		[SerializeField]
		private NetworkStatusEventsSO networkEvents;

		[Space]
		[Header("UI elements")]
		[SerializeField]
		private TMP_Text messageText;

		[SerializeField]
		private GameObject modalPanel;

		[SerializeField]
		private Button snoozeButton;

		[SerializeField]
		private TMP_Text snoozeButtonText;

		protected void OnEnable()
		{
			networkEvents.OnStatusChange += HandleNetworkStatusChange;
			networkEvents.OnDisplayNetworkStatusModal += HandleDisplayNetworkStatusModal;
			snoozeButton.onClick.AddListener(Deactivate);
		}

		protected void OnDisable()
		{
			networkEvents.OnStatusChange -= HandleNetworkStatusChange;
			networkEvents.OnDisplayNetworkStatusModal -= HandleDisplayNetworkStatusModal;
			snoozeButton.onClick.RemoveListener(Deactivate);
		}

		private void Activate()
		{
			if (authState.InOfflineWindow)
			{
				snoozeButtonText.text = "Snooze";
				messageText.text = string.Format("{0} \n {1} {2}", "You do not have an internet connection", authState.DaysRemaining, "days remaining");
				modalPanel.SetActive(value: true);
			}
		}

		private void Deactivate()
		{
			if (modalPanel.activeSelf)
			{
				modalPanel.SetActive(value: false);
			}
		}

		private void HandleDisplayNetworkStatusModal()
		{
			snoozeButtonText.text = "Ok";
			messageText.text = "You do not have an internet connection";
			modalPanel.SetActive(value: true);
		}

		private void HandleNetworkStatusChange(NetworkStatus newStatus)
		{
			switch (newStatus)
			{
			case NetworkStatus.Offline:
				Activate();
				break;
			case NetworkStatus.Online:
				Deactivate();
				break;
			}
		}
	}
}
