using UnityEngine;
using UnityEngine.Events;

namespace Games.Standard_Target.SO
{
	[CreateAssetMenu(menuName = "Games/Standard Target/Game Events")]
	public class StandardTargetGameEventsSO : ScriptableObject
	{
		public event UnityAction OnStartGameplay;

		public void RaiseStartGameplay()
		{
			this.OnStartGameplay?.Invoke();
		}
	}
}
