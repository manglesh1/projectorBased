using System;
using UnityEngine;
using UnityEngine.UI;

namespace MHLab.Patch.Launcher.Scripts.UI
{
	public sealed class Dialog : MonoBehaviour
	{
		public Text MainMessage;

		public Text DetailsMessage;

		public Button CloseButton;

		public Button ContinueButton;

		private void ClearListeners()
		{
			CloseButton.onClick.RemoveAllListeners();
			ContinueButton.onClick.RemoveAllListeners();
		}

		public void ShowDialog(string main, string detail, Action onClose, Action onContinue)
		{
			ClearListeners();
			MainMessage.text = main;
			DetailsMessage.text = detail;
			CloseButton.onClick.AddListener(delegate
			{
				CloseButton.onClick.RemoveAllListeners();
				onClose?.Invoke();
				base.gameObject.SetActive(value: false);
			});
			CloseButton.gameObject.SetActive(value: true);
			ContinueButton.onClick.AddListener(delegate
			{
				ContinueButton.onClick.RemoveAllListeners();
				onContinue?.Invoke();
				base.gameObject.SetActive(value: false);
			});
			ContinueButton.gameObject.SetActive(value: true);
			base.gameObject.SetActive(value: true);
		}

		public void ShowCloseDialog(string main, string detail, Action onClose)
		{
			ClearListeners();
			MainMessage.text = main;
			DetailsMessage.text = detail;
			CloseButton.onClick.AddListener(delegate
			{
				CloseButton.onClick.RemoveAllListeners();
				onClose?.Invoke();
				base.gameObject.SetActive(value: false);
			});
			CloseButton.gameObject.SetActive(value: true);
			ContinueButton.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: true);
		}

		public void CloseDialog()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
