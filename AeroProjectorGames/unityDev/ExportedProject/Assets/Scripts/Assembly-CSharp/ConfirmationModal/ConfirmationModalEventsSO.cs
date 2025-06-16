using System;
using UnityEngine;
using UnityEngine.Events;

namespace ConfirmationModal
{
	[CreateAssetMenu(menuName = "Modals/Confirmation Modal Events")]
	public class ConfirmationModalEventsSO : ScriptableObject
	{
		public UnityEvent<Action, string> OnConfirmationRequest = new UnityEvent<Action, string>();

		public void RaiseConfirmationModal(Action confirmationAction, string confirmationMessage)
		{
			OnConfirmationRequest.Invoke(confirmationAction, confirmationMessage);
		}
	}
}
