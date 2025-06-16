using System.Collections;
using System.Collections.Generic;
using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Games.Standard_Target
{
	public class ScoringRingController : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private SpriteRenderer _renderer;

		private Color _startingColor;

		[SerializeField]
		private GameStateSO gameState;

		[Space(2f)]
		[Header("Bullseye Specific")]
		[SerializeField]
		private bool bullseye;

		[SerializeField]
		private List<SpriteRenderer> bullseyeRingsToAnimate;

		public event UnityAction OnPointerDownTriggered;

		private void OnDisable()
		{
			_renderer.color = _startingColor;
		}

		private void OnEnable()
		{
			_renderer = base.gameObject.GetComponent<SpriteRenderer>();
			_startingColor = _renderer.color;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!gameState.IsTargetDisabled)
			{
				StartCoroutine(AnimateHitEffects());
				this.OnPointerDownTriggered?.Invoke();
			}
		}

		private IEnumerator AnimateHitEffects()
		{
			if (bullseye)
			{
				yield return BullseyeFlash();
				yield return RingExplosionAnimation();
				yield return LerpColor(Color.blue, Color.yellow, 0.02f);
				yield return LerpColor(Color.yellow, Color.magenta, 0.2f);
				yield return LerpColor(Color.magenta, Color.green, 0.2f);
				yield return LerpColor(Color.green, Color.blue, 0.2f);
				yield return LerpColor(Color.blue, _startingColor, 0.2f);
			}
			else
			{
				yield return LerpColor(Color.green, Color.blue, 0.2f);
				yield return LerpColor(Color.blue, _startingColor, 0.5f);
			}
			_renderer.color = _startingColor;
		}

		private IEnumerator RingExplosionAnimation()
		{
			foreach (SpriteRenderer r in bullseyeRingsToAnimate)
			{
				Color originalColor = r.color;
				float duration = 0.04f;
				float currentTime = 0f;
				while (currentTime < duration)
				{
					r.color = Color.Lerp(r.color, Color.blue, currentTime / duration);
					currentTime += Time.deltaTime;
					yield return null;
				}
				duration = 0.04f;
				currentTime = 0f;
				while (currentTime < duration)
				{
					r.color = Color.Lerp(r.color, Color.green, currentTime / duration);
					currentTime += Time.deltaTime;
					yield return null;
				}
				duration = 0.04f;
				currentTime = 0f;
				while (currentTime < duration)
				{
					r.color = Color.Lerp(r.color, originalColor, currentTime / duration);
					currentTime += Time.deltaTime;
					yield return null;
				}
				r.color = originalColor;
			}
			for (int i = bullseyeRingsToAnimate.Count - 2; i >= 0; i--)
			{
				SpriteRenderer r2 = bullseyeRingsToAnimate[i];
				Color originalColor2 = r2.color;
				float currentTime2 = 0.04f;
				float duration2 = 0f;
				while (duration2 < currentTime2)
				{
					r2.color = Color.Lerp(r2.color, Color.blue, duration2 / currentTime2);
					duration2 += Time.deltaTime;
					yield return null;
				}
				currentTime2 = 0.04f;
				duration2 = 0f;
				while (duration2 < currentTime2)
				{
					r2.color = Color.Lerp(r2.color, Color.green, duration2 / currentTime2);
					duration2 += Time.deltaTime;
					yield return null;
				}
				currentTime2 = 0.04f;
				duration2 = 0f;
				while (duration2 < currentTime2)
				{
					r2.color = Color.Lerp(r2.color, originalColor2, duration2 / currentTime2);
					duration2 += Time.deltaTime;
					yield return null;
				}
				r2.color = originalColor2;
			}
		}

		private IEnumerator BullseyeFlash()
		{
			yield return LerpColor(_startingColor, Color.yellow, 0.02f);
			yield return LerpColor(Color.yellow, Color.magenta, 0.2f);
			yield return LerpColor(Color.magenta, Color.green, 0.2f);
			yield return LerpColor(Color.green, Color.blue, 0.2f);
		}

		private IEnumerator LerpColor(Color startingColor, Color endingColor, float duration)
		{
			float currentTime = 0f;
			while (currentTime < duration)
			{
				_renderer.color = Color.Lerp(startingColor, endingColor, currentTime / duration);
				currentTime += Time.deltaTime;
				yield return null;
			}
		}
	}
}
