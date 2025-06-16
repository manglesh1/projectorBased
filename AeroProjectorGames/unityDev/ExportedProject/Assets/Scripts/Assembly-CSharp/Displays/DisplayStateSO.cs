using UnityEngine;

namespace Displays
{
	[CreateAssetMenu(menuName = "Displays/Display State SO")]
	public class DisplayStateSO : ScriptableObject
	{
		public bool MultiDisplayEnabled { get; private set; }

		public void SetMultiDisplayEnabled(bool enabled)
		{
			MultiDisplayEnabled = enabled;
		}
	}
}
