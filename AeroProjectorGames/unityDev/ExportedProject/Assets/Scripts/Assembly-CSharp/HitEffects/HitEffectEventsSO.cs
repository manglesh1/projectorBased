using UnityEngine;
using UnityEngine.Events;

namespace HitEffects
{
	[CreateAssetMenu(menuName = "HitEffects/HitEffectEvents")]
	public class HitEffectEventsSO : ScriptableObject
	{
		public event UnityAction<Vector2> OnHitEffect;

		public void RaiseHitEffect(Vector2 screenPosition)
		{
			this.OnHitEffect?.Invoke(screenPosition);
		}
	}
}
