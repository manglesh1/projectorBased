using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	public class PointerEnterExitVisibilityController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		[SerializeField]
		private GameObject target;

		private void OnEnable()
		{
			target.SetActive(value: false);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			target.SetActive(value: true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			target.SetActive(value: false);
		}
	}
}
