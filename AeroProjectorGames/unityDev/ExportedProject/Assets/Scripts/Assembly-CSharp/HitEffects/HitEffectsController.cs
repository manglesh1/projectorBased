using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

namespace HitEffects
{
	public class HitEffectsController : MonoBehaviour
	{
		private const float WAIT_FOR_ANIMATION = 2f;

		private Animator _animator;

		private ParticleSystem _currentEffect;

		[Header("Particles")]
		[Description("Add particles to match the HitEffectParticleenum")]
		[SerializeField]
		private GameObject redLightningParticle;

		[Header("Hit Effect Events")]
		[SerializeField]
		private HitEffectEventsSO hitEffectEvents;

		private void OnDisable()
		{
			hitEffectEvents.OnHitEffect -= HandleHitEffect;
		}

		private void OnEnable()
		{
			hitEffectEvents.OnHitEffect += HandleHitEffect;
			_animator = redLightningParticle.GetComponent<Animator>();
		}

		private void HandleHitEffect(Vector2 screenPosition)
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(screenPosition);
			redLightningParticle.gameObject.transform.position = new Vector3(vector.x, vector.y, 1f);
			redLightningParticle.gameObject.SetActive(value: true);
			StartCoroutine(PlayAnimation());
		}

		private IEnumerator PlayAnimation()
		{
			yield return new WaitForSeconds(2f);
			redLightningParticle.gameObject.SetActive(value: false);
		}

		private GameObject GetEffect(HitEffectParticleEnum hitEffect)
		{
			if (hitEffect == HitEffectParticleEnum.RedLightning)
			{
				return redLightningParticle;
			}
			throw new ArgumentOutOfRangeException("hitEffect", hitEffect, null);
		}
	}
}
