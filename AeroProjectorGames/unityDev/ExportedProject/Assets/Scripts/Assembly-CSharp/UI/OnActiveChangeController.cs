using UnityEngine;
using UnityEngine.Events;

namespace UI
{
	public class OnActiveChangeController : MonoBehaviour
	{
		public UnityEvent<bool> OnActiveChanged = new UnityEvent<bool>();

		private void OnDisable()
		{
			OnActiveChanged?.Invoke(arg0: false);
		}

		private void OnEnable()
		{
			OnActiveChanged?.Invoke(arg0: true);
		}
	}
}
