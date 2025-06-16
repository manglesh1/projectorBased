using System;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Word_Whack.Scripts
{
	public class WordWhackPhraseController : MonoBehaviour
	{
		private const int MAX_CHARACTERS = 17;

		private List<WordWhackPhraseCharacterController> _characters;

		[Header("Character Prefab")]
		[SerializeField]
		private WordWhackPhraseCharacterController characterPrefab;

		[Header("Character Container")]
		[SerializeField]
		private Transform characterContainer;

		public int SpaceAvailable => 17 - _characters.Count;

		private void OnDisable()
		{
			_characters.ForEach(delegate(WordWhackPhraseCharacterController c)
			{
				UnityEngine.Object.DestroyImmediate(c.gameObject);
			});
		}

		private void OnEnable()
		{
			if (_characters == null)
			{
				_characters = new List<WordWhackPhraseCharacterController>();
			}
		}

		public void AddCharacter(char character, int index)
		{
			if (_characters.Count > 17)
			{
				throw new InvalidOperationException("You have exceeded the maximum number of characters allowed.");
			}
			WordWhackPhraseCharacterController wordWhackPhraseCharacterController = UnityEngine.Object.Instantiate(characterPrefab, characterContainer);
			wordWhackPhraseCharacterController.SetCharacter(character, index);
			_characters.Add(wordWhackPhraseCharacterController);
		}
	}
}
