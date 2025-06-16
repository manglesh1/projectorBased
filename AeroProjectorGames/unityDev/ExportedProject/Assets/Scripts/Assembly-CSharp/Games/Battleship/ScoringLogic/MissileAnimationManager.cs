using System.Collections;
using System.Text;
using Settings;
using Target;
using UnityEngine;

namespace Games.Battleship.ScoringLogic
{
	public class MissileAnimationManager : MonoBehaviour
	{
		private const string ANIMATION_TRIGGER_PREFIX = "Trigger Cell ";

		private const float WAIT_FOR_ANIMATION_TO_START = 0.5f;

		[Header("Missile Animation Elements")]
		[SerializeField]
		private Animator missileAnimator;

		[Header("Missile Animation Controller")]
		[SerializeField]
		private RuntimeAnimatorController normalAnimtionController;

		[SerializeField]
		private RuntimeAnimatorController shortAnimtionController;

		[Header("Missile Objects")]
		[SerializeField]
		private GameObject missileFireObject;

		[SerializeField]
		private GameObject missileObject;

		[SerializeField]
		private GameObject missileSmokeObject;

		[Header("Settings Elements")]
		private TargetSettings _targetSettings;

		private void OnEnable()
		{
			_targetSettings = SettingsStore.Target;
			if (_targetSettings.UseShorterBattleshipAnimations)
			{
				missileAnimator.runtimeAnimatorController = shortAnimtionController;
			}
			else
			{
				missileAnimator.runtimeAnimatorController = normalAnimtionController;
			}
			missileFireObject.SetActive(value: false);
			missileObject.SetActive(value: false);
			missileSmokeObject.SetActive(value: false);
		}

		public IEnumerator PlayMissileAnimation(int cellPosition)
		{
			missileFireObject.SetActive(value: true);
			missileObject.SetActive(value: true);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Trigger Cell ");
			stringBuilder.Append(cellPosition);
			missileAnimator.SetTrigger(stringBuilder.ToString());
			yield return new WaitForSeconds(0.5f);
			float length = missileAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length - 0.5f);
			missileFireObject.SetActive(value: false);
			missileObject.SetActive(value: false);
		}
	}
}
