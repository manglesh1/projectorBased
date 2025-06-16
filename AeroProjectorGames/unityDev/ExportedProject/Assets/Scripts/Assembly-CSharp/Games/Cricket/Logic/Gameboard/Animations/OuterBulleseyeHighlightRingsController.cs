using System.Collections;
using UnityEngine;

namespace Games.Cricket.Logic.Gameboard.Animations
{
	public class OuterBulleseyeHighlightRingsController : MonoBehaviour
	{
		private const float ANIMATION_DURATION = 3f;

		[Header("Other Elements")]
		[SerializeField]
		private CricketGameboardAnimationOrchestrator gameboardAnimationOrchestrator;

		private void OnEnable()
		{
			StartCoroutine(StartGameboardAnimation());
		}

		private IEnumerator StartGameboardAnimation()
		{
			yield return new WaitForSeconds(3f);
			yield return null;
			gameboardAnimationOrchestrator.HandleEndPlayCricketGameboardAnimation();
		}
	}
}
