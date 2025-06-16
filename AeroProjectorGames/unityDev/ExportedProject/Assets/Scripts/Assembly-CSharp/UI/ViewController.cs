using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	public class ViewController : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> viewPanels;

		[SerializeField]
		private ViewEventsSO viewEvents;

		private void OnDisable()
		{
			viewEvents.OnOpenView -= Show;
			viewEvents.OnCloseView -= Hide;
		}

		private void OnEnable()
		{
			viewEvents.OnOpenView += Show;
			viewEvents.OnCloseView += Hide;
		}

		private void Show()
		{
			viewPanels.ForEach(delegate(GameObject panel)
			{
				panel.SetActive(value: true);
			});
		}

		private void Hide()
		{
			viewPanels.ForEach(delegate(GameObject panel)
			{
				panel.SetActive(value: false);
			});
		}
	}
}
