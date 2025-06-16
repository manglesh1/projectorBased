using System.Collections;
using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Axteroids
{
	public class ShipExplosion : MonoBehaviour
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
		private GameObject particleEffectObject;

		[Header("Animation Timing")]
		[SerializeField]
		private float waitTimeAfterExplosion = 3f;

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

		public void Hit(PointerEventData pointerData, bool isMouseClicked)
		{
			if (!gameState.IsTargetDisabled)
			{
				StopAnimator();
				animationController.PlayAnimation(objectName, scoreValue, pointerData, isMouseClicked);
			}
		}

		public IEnumerator ExplosionAnimation()
		{
			activeObject.SetActive(value: false);
			particleEffectObject.SetActive(value: true);
			yield return new WaitForSeconds(waitTimeAfterExplosion);
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
				activeObject.SetActive(value: true);
				particleEffectObject.SetActive(value: false);
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
