using System;
using TMPro;
using UnityEngine;

namespace ConfirmationModal
{
	public class ConfirmationModalController : MonoBehaviour
	{
		private const string DEFAULT_MESSAGE = "ARE YOU SURE?";

		private Action _currentAction;

		[SerializeField]
		private TMP_Text confirmationModalText;

		[SerializeField]
		private GameObject confirmationModalPanel;

		[Space]
		[Header("Confirmation Modal Events")]
		[SerializeField]
		private ConfirmationModalEventsSO confirmationEvents;

		private void OnEnable()
		{
			confirmationEvents.OnConfirmationRequest.AddListener(HandleConfirmationRequest);
		}

		private void OnDisable()
		{
			Reset();
			confirmationEvents.OnConfirmationRequest.RemoveListener(HandleConfirmationRequest);
		}

		public void Close()
		{
			Reset();
			confirmationModalPanel.SetActive(value: false);
		}

		public void Submit()
		{
			_currentAction();
			Close();
		}

		private void HandleConfirmationRequest(Action successAction, string confirmationMessage)
		{
			_currentAction = successAction;
			confirmationModalText.text = confirmationMessage;
			confirmationModalPanel.SetActive(value: true);
		}

		private void Reset()
		{
			confirmationModalPanel.SetActive(value: false);
			_currentAction = null;
			confirmationModalText.text = "ARE YOU SURE?";
		}
	}
}
