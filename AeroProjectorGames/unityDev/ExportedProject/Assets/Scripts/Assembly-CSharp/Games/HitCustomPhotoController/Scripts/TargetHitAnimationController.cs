using System.Collections;
using Games.CustomComponents;
using Games.GameState;
using Games.HitCustomPhotoController.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Games.HitCustomPhotoController.Scripts
{
	public class TargetHitAnimationController : MonoBehaviour
	{
		private GameObject _newTempTarget;

		private GameObject _rockParticleSytem;

		[SerializeField]
		private TargetColorsHolder targetColorsHolder;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private HitCustomPhotoEventsSO hitCustomPhotoEvents;

		[Header("Target Particles")]
		[SerializeField]
		private GameObject blueRockParticleAnimation;

		[SerializeField]
		private GameObject greenRockParticleAnimation;

		[SerializeField]
		private GameObject pinkRockParticleAnimation;

		[SerializeField]
		private GameObject yellowRockParticleAnimation;

		private void OnEnable()
		{
			hitCustomPhotoEvents.OnTargetHit += TargetClicked;
		}

		private void OnDisable()
		{
			hitCustomPhotoEvents.OnTargetHit -= TargetClicked;
			DestroyNewTarget();
			DestroyRockParticleSystem();
		}

		private void CreateAnimationTarget(Transform TargetTransform)
		{
			_newTempTarget = Object.Instantiate(TargetTransform.gameObject, base.transform.root, worldPositionStays: true);
			_newTempTarget.SetActive(value: true);
			_newTempTarget.GetComponent<PolygonCollider2D>().enabled = false;
			_newTempTarget.GetComponent<TargetMovementController>().enabled = false;
		}

		private void CreateRockParticleSystem()
		{
			Color color = _newTempTarget.transform.GetChild(0).gameObject.GetComponent<Image>().color;
			if (color == targetColorsHolder.BlueColor)
			{
				_rockParticleSytem = Object.Instantiate(blueRockParticleAnimation, base.transform.root, worldPositionStays: true);
			}
			else if (color == targetColorsHolder.GreenColor)
			{
				_rockParticleSytem = Object.Instantiate(greenRockParticleAnimation, base.transform.root, worldPositionStays: true);
			}
			else if (color == targetColorsHolder.PinkColor)
			{
				_rockParticleSytem = Object.Instantiate(pinkRockParticleAnimation, base.transform.root, worldPositionStays: true);
			}
			else
			{
				_rockParticleSytem = Object.Instantiate(yellowRockParticleAnimation, base.transform.root, worldPositionStays: true);
			}
			Vector3 position = _newTempTarget.transform.position;
			position.z = -10f;
			_rockParticleSytem.transform.position = position;
			_rockParticleSytem.transform.localScale *= 2f;
			_rockParticleSytem.SetActive(value: false);
		}

		private void DestroyNewTarget()
		{
			if (_newTempTarget != null)
			{
				Object.Destroy(_newTempTarget);
			}
		}

		private void DestroyRockParticleSystem()
		{
			if (_rockParticleSytem != null)
			{
				Object.Destroy(_rockParticleSytem);
			}
		}

		private IEnumerator HitAnimation(Transform TargetTransform, int roundScore)
		{
			gameState.DisableTarget();
			Vector3 originalTransformScale = TargetTransform.localScale;
			Vector3 originalTransformPosition = TargetTransform.localPosition;
			Quaternion originalTransformRotation = TargetTransform.localRotation;
			Color originalColor = TargetTransform.gameObject.GetComponent<Image>().color;
			yield return LerpAnimation(TargetTransform);
			TargetTransform.localScale = originalTransformScale;
			TargetTransform.localPosition = originalTransformPosition;
			TargetTransform.localRotation = originalTransformRotation;
			TargetTransform.gameObject.GetComponent<Image>().color = originalColor;
		}

		private IEnumerator LerpAnimation(Transform TargetTransform)
		{
			TargetTransform.gameObject.SetActive(value: false);
			DestroyNewTarget();
			CreateAnimationTarget(TargetTransform);
			CreateRockParticleSystem();
			yield return null;
			yield return StartCoroutine(PlayScaleUpAnimation(_newTempTarget.transform));
			_newTempTarget.transform.gameObject.GetComponent<Image>().enabled = false;
			_newTempTarget.transform.GetChild(0).gameObject.GetComponent<Image>().enabled = false;
			_rockParticleSytem.SetActive(value: true);
			DestroyNewTarget();
			hitCustomPhotoEvents.RaiseHitAnimationFinished(TargetTransform.GetComponent<ScoredButton>());
		}

		private IEnumerator PlayScaleUpAnimation(Transform TargetTransform)
		{
			Vector3 newScale = TargetTransform.localScale;
			newScale += Vector3.one * 1.5f;
			float animationCulling = 4.75f;
			float currentTime = 0f;
			float duration = 6f;
			while (currentTime < duration - animationCulling)
			{
				TargetTransform.localScale = Vector3.Lerp(TargetTransform.localScale, newScale, currentTime / duration);
				currentTime += Time.deltaTime;
				yield return null;
			}
		}

		public void TargetClicked(ScoredButton scoredButton)
		{
			StartCoroutine(HitAnimation(scoredButton.transform, scoredButton.Score));
		}
	}
}
