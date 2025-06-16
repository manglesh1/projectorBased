using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	[RequireComponent(typeof(Slider))]
	public class RoundedSliderController : MonoBehaviour
	{
		private Slider slider;

		private void HandleValueChanged(float value)
		{
			slider.SetValueWithoutNotify((float)Math.Round(value, 1));
		}

		protected void OnDisable()
		{
			Slider slider = this.slider;
			if (!(slider == null))
			{
				slider.onValueChanged.RemoveListener(HandleValueChanged);
			}
		}

		protected void OnEnable()
		{
			this.slider = base.gameObject.GetComponent<Slider>();
			Slider slider = this.slider;
			if (!(slider == null))
			{
				slider.onValueChanged.AddListener(HandleValueChanged);
			}
		}
	}
}
