using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Games._21.Scripts
{
	public class Blackjack21MultiDisplayScoringController : MonoBehaviour
	{
		[SerializeField]
		private GameObject gameboardRoot;

		public void SetupBoard(List<List<PlacedCard>> playingCardPlacement)
		{
			for (int i = 0; i < gameboardRoot.transform.childCount; i++)
			{
				Transform child = gameboardRoot.transform.GetChild(i);
				for (int j = 0; j < child.childCount; j++)
				{
					Transform child2 = child.GetChild(j);
					GameObject scoringCard = playingCardPlacement[i][j].ScoringCard;
					scoringCard.transform.SetParent(child2, worldPositionStays: false);
					scoringCard.transform.localScale = Vector3.one;
					Keep3dModelSizeWithUIParent component = scoringCard.GetComponent<Keep3dModelSizeWithUIParent>();
					component.UpdateParent(child2.GetComponent<RectTransform>());
					component.TransformToControlResizing = child2.GetComponent<RectTransform>();
				}
			}
		}

		public void SelectCard(GameObject scoringCard)
		{
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				scoringCard.transform.parent.GetChild(0).gameObject.SetActive(value: true);
			}
		}

		public void DeselectCard(GameObject scoringCard)
		{
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				scoringCard.transform.parent.GetChild(0).gameObject.SetActive(value: false);
			}
		}
	}
}
