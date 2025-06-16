using UnityEngine;
using UnityEngine.Events;

namespace UI.MultiDisplay
{
	[CreateAssetMenu(menuName = "Displays/Multi-Display Scoring Events")]
	public class MultiDisplayScoringEventsSO : ScriptableObject
	{
		public event UnityAction<GameObject> OnLoadMultiDisplayHelpButtonObject;

		public event UnityAction<GameObject> OnLoadMultiDisplayScoringObject;

		public event UnityAction OnUnloadScoringObject;

		public void RaiseLoadMultiDisplayHelpButtonObject(GameObject helpButtonObject)
		{
			this.OnLoadMultiDisplayHelpButtonObject?.Invoke(helpButtonObject);
		}

		public void RaiseLoadScoringObject(GameObject multiDisplayScoringObject)
		{
			this.OnLoadMultiDisplayScoringObject?.Invoke(multiDisplayScoringObject);
		}

		public void RaiseUnloadScoringObject()
		{
			this.OnUnloadScoringObject?.Invoke();
		}
	}
}
