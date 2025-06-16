using UI;
using UnityEngine;

namespace Detection
{
	[CreateAssetMenu(menuName = "Detection/UI/Detection UI State")]
	public class DetectionUIStateSO : ScriptableObject
	{
		private PanelVisibilityEnum _panelVisibilityEnum;

		public PanelVisibilityEnum PanelVisibilityEnum => _panelVisibilityEnum;

		public void SetOpen()
		{
			_panelVisibilityEnum = PanelVisibilityEnum.Open;
		}

		public void SetClosed()
		{
			_panelVisibilityEnum = PanelVisibilityEnum.Closed;
		}
	}
}
