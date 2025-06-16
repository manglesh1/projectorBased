using System;
using TMPro;
using UnityEngine;

namespace Security
{
	public class SupportCodeModal : MonoBehaviour
	{
		private const int CODE_MODIFIER = 3;

		private const int CODE_REDUCER = 1958;

		private Action _currentAction;

		private int _currentCode;

		[SerializeField]
		private TMP_Text securityModalCode;

		[SerializeField]
		private TMP_InputField securityModalInput;

		[SerializeField]
		private GameObject securityModalPanel;

		[Space]
		[Header("Security Modal Events")]
		[SerializeField]
		private SecurityModalEventsSO securityModalEvents;

		private void OnEnable()
		{
			securityModalEvents.OnSupportCodeModalRequest.AddListener(HandleSupportCodeRequest);
		}

		private void OnDisable()
		{
			Reset();
			securityModalEvents.OnSupportCodeModalRequest.RemoveListener(HandleSupportCodeRequest);
		}

		public void Close()
		{
			Reset();
			securityModalPanel.SetActive(value: false);
		}

		public void Submit()
		{
			if (securityModalInput.text.Length != 0)
			{
				if (((int)Math.Floor((double)(_currentCode * 3 - 1958))).ToString() == securityModalInput.text)
				{
					_currentAction();
					Close();
				}
				_currentAction();
				Close();
			}
		}

		private void GenerateCode()
		{
			_currentCode = UnityEngine.Random.Range(1000, 9999);
			securityModalCode.text = _currentCode.ToString();
		}

		private void HandleSupportCodeRequest(Action successAction)
		{
			_currentAction = successAction;
			GenerateCode();
			securityModalPanel.SetActive(value: true);
			securityModalInput.Select();
		}

		private void Reset()
		{
			_currentCode = 0;
			_currentAction = null;
			securityModalCode.text = string.Empty;
		}
	}
}
