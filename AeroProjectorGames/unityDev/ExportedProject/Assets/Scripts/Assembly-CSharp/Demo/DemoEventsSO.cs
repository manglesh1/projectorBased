using UnityEngine;
using UnityEngine.Events;

namespace Demo
{
	[CreateAssetMenu(menuName = "Demo/Demo Events")]
	public class DemoEventsSO : ScriptableObject
	{
		public event UnityAction OnDemoModeStarted;

		public event UnityAction OnDemoModeStopped;

		public void RaiseDemoModeStarted()
		{
			this.OnDemoModeStarted?.Invoke();
		}

		public void RaiseDemoModeStopped()
		{
			this.OnDemoModeStopped?.Invoke();
		}
	}
}
