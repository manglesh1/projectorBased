using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NorseScoreboardBackgroundTimer : MonoBehaviour
{
	private Coroutine timerCoroutine;

	public void StartTimer(float duration, UnityAction onTimerComplete)
	{
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
		}
		timerCoroutine = StartCoroutine(TimerCoroutine(duration, onTimerComplete));
	}

	public void StopTimer()
	{
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
			timerCoroutine = null;
		}
	}

	private IEnumerator TimerCoroutine(float duration, UnityAction onTimerComplete)
	{
		yield return new WaitForSeconds(duration);
		onTimerComplete?.Invoke();
		timerCoroutine = null;
	}
}
