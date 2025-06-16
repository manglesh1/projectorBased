using Games.Standard_Target.SO;
using Target;
using UnityEngine;

namespace Games.Standard_Target
{
	public class MultiDisplayStandardTargetColorSelectionMenuController : MonoBehaviour
	{
		[SerializeField]
		private StandardTargetColorController standardTargetColorController;

		[Header("Scriptable Objects")]
		[SerializeField]
		private StandardTargetGameEventsSO standardTargetGameEvents;

		public void SetPlayerSelectedTargetColor(TargetColorEnum selectedColor)
		{
			standardTargetColorController.SetPlayerSelectedTargetColor(selectedColor);
			standardTargetGameEvents.RaiseStartGameplay();
		}

		public void DefaultColorSelectedByPlayer()
		{
			SetPlayerSelectedTargetColor(TargetColorEnum.Default);
		}

		public void NeonColorSelectedByPlayer()
		{
			SetPlayerSelectedTargetColor(TargetColorEnum.Neon);
		}

		public void RedAndWhiteColorSelectedByPlayer()
		{
			SetPlayerSelectedTargetColor(TargetColorEnum.RedAndWhite);
		}
	}
}
