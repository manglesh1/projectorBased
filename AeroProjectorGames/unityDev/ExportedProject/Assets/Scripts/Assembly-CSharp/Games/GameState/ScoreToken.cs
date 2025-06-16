using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Games.GameState
{
	[Serializable]
	public class ScoreToken
	{
		public int AlternateScoreValue { get; set; }

		public int ScoreValue { get; set; }

		public string TokenName { get; set; }

		[JsonIgnore]
		public GameObject Token { get; set; }
	}
}
