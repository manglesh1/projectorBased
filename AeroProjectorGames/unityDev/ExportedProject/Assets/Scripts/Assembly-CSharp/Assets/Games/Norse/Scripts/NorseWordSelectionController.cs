using UnityEngine;
using UnityEngine.UI;

namespace Assets.Games.Norse.Scripts
{
	public class NorseWordSelectionController : MonoBehaviour
	{
		[Header("Events")]
		[SerializeField]
		private NorseEventsSO norseEvents;

		[Header("Word Buttons")]
		[SerializeField]
		private Button buttonAxe;

		[SerializeField]
		private Button buttonNorse;

		private const string WORD_AXE = "AXE";

		private const string WORD_NORSE = "NORSE";

		private void OnDisable()
		{
			buttonAxe.onClick.RemoveAllListeners();
			buttonNorse.onClick.RemoveAllListeners();
		}

		private void OnEnable()
		{
			buttonAxe.onClick.AddListener(delegate
			{
				norseEvents.RaiseOnWordSelected("AXE");
			});
			buttonNorse.onClick.AddListener(delegate
			{
				norseEvents.RaiseOnWordSelected("NORSE");
			});
		}
	}
}
