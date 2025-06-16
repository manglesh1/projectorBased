using System.Collections;
using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Axteroids
{
	public class AsteroidExplosion : MonoBehaviour
	{
		private const float WAIT_BEFORE_CHECKING_CLIP_LENGTH = 0.5f;

		private RuntimeAnimatorController _animatorController;

		private int _clipCount;

		private IEnumerator _playCoroutine;

		[SerializeField]
		private AxteroidsGameboardController animationController;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private AxteroidsObjectList objectName;

		[SerializeField]
		private int scoreValue;

		[Header("Object Parts")]
		[SerializeField]
		private GameObject activeObject;

		[SerializeField]
		private Animator animator;

		[SerializeField]
		private GameObject objectInParts;

		[SerializeField]
		private GameObject particleEffectObject;

		[Header("Animation Timing")]
		[SerializeField]
		private float particleEffectLength = 3f;

		[Header("Exsplosive Properties")]
		[SerializeField]
		private float expForce;

		[SerializeField]
		private float radius;

		[SerializeField]
		private Rigidbody rigidBody1;

		[SerializeField]
		private Rigidbody rigidBody2;

		[SerializeField]
		private Rigidbody rigidBody3;

		[SerializeField]
		private Rigidbody rigidBody4;

		public int ScoreValue => scoreValue;

		private void OnEnable()
		{
			_animatorController = animator.runtimeAnimatorController;
			_clipCount = _animatorController.animationClips.Length;
			PrepareAnimation();
		}

		private void OnDisable()
		{
			if (_playCoroutine != null)
			{
				StopCoroutine(_playCoroutine);
			}
		}

		public void HitDetected(PointerEventData pointerData, bool isMouseClicked)
		{
			if (!gameState.IsTargetDisabled)
			{
				StopAnimator();
				animationController.PlayAnimation(objectName, scoreValue, pointerData, isMouseClicked);
			}
		}

		private void DisableParticleEffect()
		{
			particleEffectObject.SetActive(value: false);
			objectInParts.SetActive(value: false);
		}

		public IEnumerator ExplosionAnimation()
		{
			Vector3 position = base.transform.position;
			position.z = particleEffectObject.transform.position.z;
			particleEffectObject.transform.position = position;
			activeObject.SetActive(value: false);
			particleEffectObject.SetActive(value: true);
			objectInParts.SetActive(value: true);
			Vector3 rigidBody1_Position = rigidBody1.transform.position;
			Vector3 rigidBody2_Position = rigidBody2.transform.position;
			Vector3 rigidBody3_Position = rigidBody3.transform.position;
			Vector3 rigidBody4_Position = rigidBody4.transform.position;
			rigidBody1.AddExplosionForce(expForce, base.transform.position, radius);
			rigidBody2.AddExplosionForce(expForce, base.transform.position, radius);
			rigidBody3.AddExplosionForce(expForce, base.transform.position, radius);
			rigidBody4.AddExplosionForce(expForce, base.transform.position, radius);
			yield return new WaitForSeconds(particleEffectLength);
			rigidBody1.transform.position = rigidBody1_Position;
			rigidBody2.transform.position = rigidBody2_Position;
			rigidBody3.transform.position = rigidBody3_Position;
			rigidBody4.transform.position = rigidBody4_Position;
			DisableParticleEffect();
		}

		private IEnumerator PlayAnimation()
		{
			animator.Play(0);
			yield return new WaitForSeconds(0.5f);
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.5f);
			PrepareAnimation();
		}

		public void PrepareAnimation()
		{
			if (_playCoroutine != null)
			{
				StopCoroutine(_playCoroutine);
			}
			if (_clipCount != 0)
			{
				DisableParticleEffect();
				activeObject.SetActive(value: true);
				animator.SetInteger("Animation Counter", Random.Range(0, _clipCount));
				animator.enabled = true;
				_playCoroutine = PlayAnimation();
				StartCoroutine(_playCoroutine);
			}
		}

		public void StopAnimator()
		{
			if (_playCoroutine != null)
			{
				StopCoroutine(_playCoroutine);
			}
			animator.enabled = false;
		}
	}
}
