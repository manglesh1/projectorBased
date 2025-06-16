using System.Collections.Generic;
using Games.CustomComponents;
using UnityEngine;

namespace Blackjack21
{
	public class PlayingCardMaterialHelper : MonoBehaviour
	{
		private const int DECK_COUNT = 52;

		private List<GameObject> _playingCardMasterGameboardReference = new List<GameObject>();

		private List<GameObject> _playingsCardMasterScoreboardReference = new List<GameObject>();

		[Header("Reference Instantiate Points")]
		[SerializeField]
		private GameObject gameboardCardReferenceListObject;

		[SerializeField]
		private GameObject scoreboardCardReferenceListObject;

		[Header("Card Creation Elements")]
		[SerializeField]
		private GameObject playingCardModel;

		[SerializeField]
		private GameObject playingCardWideModel;

		[SerializeField]
		private List<PlayingCard> playingCards;

		private void OnEnable()
		{
			CreateMasterGameboardReference();
			CreateMasterScoreboardReference();
		}

		private void OnDestroy()
		{
			foreach (GameObject item in _playingCardMasterGameboardReference)
			{
				if (item != null)
				{
					Object.Destroy(item);
				}
			}
			foreach (GameObject item2 in _playingsCardMasterScoreboardReference)
			{
				if (item2 != null)
				{
					Object.Destroy(item2);
				}
			}
		}

		private void CreateMasterGameboardReference()
		{
			if (_playingCardMasterGameboardReference.Count == 52)
			{
				return;
			}
			foreach (PlayingCard playingCard in playingCards)
			{
				GameObject item = Object.Instantiate(playingCardModel, gameboardCardReferenceListObject.transform);
				SetPlayingCardProperties(item, playingCard);
				_playingCardMasterGameboardReference.Add(item);
			}
		}

		private void CreateMasterScoreboardReference()
		{
			if (_playingsCardMasterScoreboardReference.Count == 52)
			{
				return;
			}
			foreach (PlayingCard playingCard in playingCards)
			{
				GameObject item = Object.Instantiate(playingCardWideModel, scoreboardCardReferenceListObject.transform);
				SetCardMaterialProperties(item, playingCard.WideFaceMaterial, playingCard.Name);
				_playingsCardMasterScoreboardReference.Add(item);
			}
		}

		public List<GameObject> GetRandomizedPlayingCards(int playingCardCount)
		{
			if (_playingCardMasterGameboardReference.Count != 52)
			{
				CreateMasterGameboardReference();
			}
			List<GameObject> list = new List<GameObject>(_playingCardMasterGameboardReference);
			for (int i = 0; i < playingCardCount; i++)
			{
				int index = Random.Range(0, list.Count);
				GameObject value = list[i];
				list[i] = list[index];
				list[index] = value;
			}
			return list;
		}

		public List<PlayingCard> GetRandomizedPlayingCardMaterial(int playingCardCount)
		{
			List<PlayingCard> list = new List<PlayingCard>(playingCards);
			for (int i = 0; i < playingCardCount; i++)
			{
				int index = Random.Range(0, list.Count);
				PlayingCard value = list[i];
				list[i] = list[index];
				list[index] = value;
			}
			return list;
		}

		public GameObject GetScoreboardTokenReference(string cardName)
		{
			return _playingsCardMasterScoreboardReference.Find((GameObject g) => g.name == cardName);
		}

		private void SetCardMaterialProperties(GameObject playingCardModel, Material playingCardFaceMaterial, string cardName)
		{
			Renderer component = playingCardModel.GetComponent<Renderer>();
			Material[] materials = component.materials;
			materials[1] = playingCardFaceMaterial;
			component.materials = materials;
			playingCardModel.name = cardName;
		}

		public void SetPlayingCardMaterial(GameObject playingCardModel, string materialName)
		{
			PlayingCard playingCardMaterial = playingCards.Find((PlayingCard m) => m.Name == materialName);
			SetPlayingCardProperties(playingCardModel, playingCardMaterial);
		}

		public void SetPlayingCardProperties(GameObject playingCardModel, PlayingCard playingCardMaterial)
		{
			TwoValueScoredButton component = playingCardModel.GetComponent<TwoValueScoredButton>();
			component.Score = ((playingCardMaterial.Score > 0) ? playingCardMaterial.Score : 0);
			component.SecondaryScore = ((playingCardMaterial.AlternativeScore > 0) ? playingCardMaterial.AlternativeScore : 0);
			SetCardMaterialProperties(playingCardModel, playingCardMaterial.FaceMaterial, playingCardMaterial.Name);
		}
	}
}
