using System;
using System.Collections.Generic;

namespace Games.Word_Whack.Scripts
{
	[Serializable]
	public class WordWhackGameHistoryState
	{
		public Dictionary<char, WordWhackCharacterStateEnum> CharacterMap { get; set; }

		public string PlayersTurn { get; set; }
	}
}
