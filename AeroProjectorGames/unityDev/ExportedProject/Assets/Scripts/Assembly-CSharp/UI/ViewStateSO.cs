using UnityEngine;

namespace UI
{
	[CreateAssetMenu(menuName = "Views/View State SO")]
	public class ViewStateSO : ScriptableObject
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
