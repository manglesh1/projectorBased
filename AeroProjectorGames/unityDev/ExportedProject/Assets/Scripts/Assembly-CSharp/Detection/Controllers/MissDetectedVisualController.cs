using Games;
using HitEffects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Detection.Controllers
{
	public class MissDetectedVisualController : MonoBehaviour
	{
		[SerializeField]
		private HitEffectEventsSO hitEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnDisable()
		{
			gameEvents.OnMissDetected -= HandleMissDetected;
		}

		private void OnEnable()
		{
			gameEvents.OnMissDetected += HandleMissDetected;
		}

		private void HandleMissDetected(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			hitEvents.RaiseHitEffect(screenPoint.Value);
		}
	}
}
