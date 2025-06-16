using System.Collections;
using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Axteroids
{
	public class BombExplosion : MonoBehaviour
	{
		private const float WAIT_BEFORE_CHECKING_CLIP_LENGTH = 0.5f;

		private RuntimeAnimatorController _animatorController;

		private int _clipCount;

		private IEnumerator _playCoroutine;

		[SerializeField]
		private AxteroidsGameboardController animationController;

		[SerializeField]
		private GameStateSO gameState;

		[Header("Object Parts")]
		[SerializeField]
		private GameObject activeObject;

		[SerializeField]
		private Animator animator;

		[SerializeField]
		private GameObject particleEffectObject;

		[SerializeField]
		private AxteroidsObjectList objectName;

		[SerializeField]
		private int scoreValue;

		[Header("Animation Timing")]
		[SerializeField]
		private float waitTimeAfterExplosion = 3f;

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
				animator.enabled = false;
				animationController.PlayAnimation(objectName, scoreValue, pointerData, isMouseClicked);
			}
		}

		public IEnumerator ExplosionAnimation()
		{
			Vector3 position = base.transform.position;
			position.z = particleEffectObject.transform.position.z;
			particleEffectObject.transform.position = position;
			activeObject.SetActive(value: false);
			particleEffectObject.SetActive(value: true);
			yield return new WaitForSeconds(waitTimeAfterExplosion);
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

		private IEnumerator PlayAnimation()
		{
			animator.Play(0);
			yield return new WaitForSeconds(0.5f);
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.5f);
			PrepareAnimation();
		}
	}
}
