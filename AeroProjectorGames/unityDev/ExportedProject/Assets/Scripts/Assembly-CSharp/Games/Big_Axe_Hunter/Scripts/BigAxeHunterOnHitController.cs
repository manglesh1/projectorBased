using UnityEngine;

namespace Games.Big_Axe_Hunter.Scripts
{
	public class BigAxeHunterOnHitController : MonoBehaviour
	{
		private const string DIE_ANIMATION = "Die";

		private const string RUN_ANIMATION = "Run";

		[SerializeField]
		private BigAxeHunterEventsSO bigAxeHunterEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private Animator animalAnimator;

		private void OnDisable()
		{
			bigAxeHunterEvents.OnHit -= HandleHit;
			gameEvents.OnMiss -= HandleMiss;
		}

		private void OnEnable()
		{
			bigAxeHunterEvents.OnHit += HandleHit;
			gameEvents.OnMiss += HandleMiss;
		}

		private void HandleMiss()
		{
			animalAnimator.Play("Run");
		}

		private void HandleHit(int score)
		{
			animalAnimator.Play("Die");
		}
	}
}
