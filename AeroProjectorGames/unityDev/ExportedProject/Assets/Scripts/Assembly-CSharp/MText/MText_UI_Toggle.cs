using UnityEngine;

namespace MText
{
	[DisallowMultipleComponent]
	public class MText_UI_Toggle : MonoBehaviour
	{
		[SerializeField]
		private bool _isOn = true;

		public GameObject activeGraphic;

		public GameObject inactiveGraphic;

		public bool IsOn
		{
			get
			{
				return _isOn;
			}
			set
			{
				_isOn = value;
				VisualUpdate();
			}
		}

		public void Set(bool set)
		{
			IsOn = set;
			VisualUpdate();
		}

		public void Toggle()
		{
			IsOn = !IsOn;
			VisualUpdate();
		}

		public void VisualUpdate()
		{
			if (IsOn)
			{
				ActiveVisualUpdate();
			}
			else
			{
				InactiveVisualUpdate();
			}
		}

		public void ActiveVisualUpdate()
		{
			ToggleGraphic(inactiveGraphic, enable: false);
			ToggleGraphic(activeGraphic, enable: true);
		}

		public void InactiveVisualUpdate()
		{
			ToggleGraphic(inactiveGraphic, enable: true);
			ToggleGraphic(activeGraphic, enable: false);
		}

		private void ToggleGraphic(GameObject graphic, bool enable)
		{
			if ((bool)graphic)
			{
				graphic.SetActive(enable);
			}
		}
	}
}
