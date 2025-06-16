using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Scoreboard.Messaging
{
	public class MessageStyleAnimationController : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text messageObject;

		[SerializeField]
		private float runtimeInSeconds = 2f;

		public event UnityAction OnAnimationComplete;

		private void OnEnable()
		{
			StartCoroutine(AnimationManager());
		}

		public void Activate()
		{
			base.gameObject.SetActive(value: true);
		}

		public void SetMessage(string message)
		{
			messageObject.text = message;
		}

		private IEnumerator AnimationManager()
		{
			yield return new WaitForSecondsRealtime(runtimeInSeconds);
			this.OnAnimationComplete?.Invoke();
			base.gameObject.SetActive(value: false);
		}
	}
}
