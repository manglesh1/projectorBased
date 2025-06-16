using System;
using System.Collections;
using System.Collections.Generic;
using Games;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WinAnimations
{
	public class BannerAnimationController : MonoBehaviour
	{
		private string _animationTrigger;

		private bool _playFireworks = true;

		[SerializeField]
		private Animator bannerAnimator;

		[SerializeField]
		private Image bannerImage;

		[SerializeField]
		private List<Sprite> bannerSprites = new List<Sprite>();

		[SerializeField]
		private TMP_Text bannerText;

		[SerializeField]
		private WinAnimationSO winAnimationSettings;

		[SerializeField]
		private ParticleSystem fireworksPS;

		[Header("External References")]
		[SerializeField]
		private GameEventsSO gameEvents;

		private void Awake()
		{
			RectTransform component = fireworksPS.GetComponent<RectTransform>();
			component.localScale = new Vector3(component.rect.size.x * 0.02f, component.localScale.y, component.localScale.z);
		}

		private void OnEnable()
		{
			gameEvents.OnNewGame += DestroySelf;
			gameEvents.OnMainMenu += DestroySelf;
			DetermineAnimationVariationToPlay();
		}

		private void OnDestroy()
		{
			gameEvents.OnNewGame -= DestroySelf;
			gameEvents.OnMainMenu -= DestroySelf;
			StopCoroutine(PlayAnimatin());
		}

		private void DestroySelf()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void DetermineAnimationVariationToPlay()
		{
			try
			{
				int value = RandomNumber(2);
				_playFireworks = Convert.ToBoolean(value);
			}
			catch
			{
				_playFireworks = true;
			}
			bannerImage.sprite = bannerSprites[RandomNumber(bannerSprites.Count)];
			switch (RandomNumber(bannerAnimator.parameterCount))
			{
			case 1:
				_animationTrigger = bannerAnimator.parameters[1].name;
				_playFireworks = false;
				break;
			case 0:
				_animationTrigger = bannerAnimator.parameters[0].name;
				break;
			}
			if (!_playFireworks)
			{
				fireworksPS.Stop();
			}
			StartCoroutine(PlayAnimatin());
		}

		private IEnumerator PlayAnimatin()
		{
			int itemCount = winAnimationSettings.TextToShow.Count;
			for (int i = 0; i < itemCount; i++)
			{
				bannerText.text = winAnimationSettings.TextToShow[i];
				bannerAnimator.SetTrigger(_animationTrigger);
				yield return new WaitForSecondsRealtime(0.5f);
				AnimatorStateInfo currentAnimatorStateInfo = bannerAnimator.GetCurrentAnimatorStateInfo(0);
				if (i + 1 == itemCount)
				{
					fireworksPS.Stop();
				}
				yield return new WaitForSecondsRealtime(currentAnimatorStateInfo.length + 0.5f);
			}
			DestroySelf();
		}

		private int RandomNumber(int maxNumber, int minNumber = 0)
		{
			return UnityEngine.Random.Range(minNumber, maxNumber);
		}
	}
}
