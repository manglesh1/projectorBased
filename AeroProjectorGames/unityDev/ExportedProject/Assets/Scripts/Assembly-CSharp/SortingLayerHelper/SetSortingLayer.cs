using UnityEngine;

namespace SortingLayerHelper
{
	public class SetSortingLayer : MonoBehaviour
	{
		[SerializeField]
		private SortingLayerDrawer sortingLayer;

		public void Start()
		{
			Canvas component = GetComponent<Canvas>();
			if (component != null)
			{
				component.sortingLayerID = sortingLayer.id;
				return;
			}
			SpriteRenderer component2 = GetComponent<SpriteRenderer>();
			if (component2 != null)
			{
				component2.sortingLayerID = sortingLayer.id;
			}
		}
	}
}
