using System;
using System.Collections;
using System.Collections.Generic;
using Games.GameState;
using TMPro;
using Target;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Standard_Target
{
	public class RingController : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private SpriteRenderer _renderer;

		private Color _startingColor;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private int scoreValue;

		[SerializeField]
		private GameStateSO gameState;

		[Space(2f)]
		[Header("Bullseye Specific")]
		[SerializeField]
		private bool bullseye;

		[SerializeField]
		private bool useBackgroundColor = true;

		[SerializeField]
		private List<SpriteRenderer> bullseyeRingsToAnimate;

		[Header("External References")]
		[SerializeField]
		private StandardTargetColorController targetColorController;

		public event EventHandler AnimationFinishedEvent;

		private void OnEnable()
		{
			_renderer = base.gameObject.GetComponent<SpriteRenderer>();
			RingThemeModel ringSettings = targetColorController.GetRingSettings(scoreValue);
			SpriteRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				if (spriteRenderer.gameObject.name == "Border")
				{
					spriteRenderer.color = ringSettings.ringBorderColor;
				}
			}
			TMP_Text componentInChildren = base.transform.GetComponentInChildren<TMP_Text>();
			if (componentInChildren != null)
			{
				componentInChildren.color = ringSettings.fontColor;
			}
			_renderer.color = ringSettings.ringColor;
			_startingColor = _renderer.color;
			AnimationFinishedEvent += AnimationFinished;
		}

		private void OnDisable()
		{
			AnimationFinishedEvent -= AnimationFinished;
			_renderer.color = _startingColor;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			Hit();
		}

		private void Hit()
		{
			if (!gameState.IsTargetDisabled)
			{
				gameState.DisableTarget();
				StartCoroutine(AnimateHitEffects());
			}
		}

		private void OnHitDetected(PointerEventData pointerEventData)
		{
			Hit();
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
			this.AnimationFinishedEvent?.Invoke(this, EventArgs.Empty);
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

		private void AnimationFinished(object sender, EventArgs args)
		{
			gameState.EnableTarget();
			gameEvents.RaiseScoreChange(scoreValue);
		}
	}
}
