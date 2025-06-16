using System.Collections.Generic;
using Players;
using UnityEngine;

namespace Games.HitCustomPhotoController.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Games/Hit Custom Photo/Game Session")]
	public class HitCustomPhotoGameSessionSO : ScriptableObject
	{
		[SerializeField]
		private List<Texture2D> customPlayerTextures = new List<Texture2D>();

		[SerializeField]
		private string gameSessionID;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private List<string> teamNames = new List<string>();

		private bool AreTeamsTheSame()
		{
			if (teamNames.Count == 0 || teamNames.Count != playerState.players.Count)
			{
				return false;
			}
			for (int i = 0; i < teamNames.Count; i++)
			{
				if (teamNames[i] == null)
				{
					return false;
				}
				if (teamNames[i] != playerState.players[i].PlayerName)
				{
					return false;
				}
			}
			return true;
		}

		public bool CheckCurrentGameSession()
		{
			return !IsTextureCountZero() && !IsTexturesNull() && AreTeamsTheSame();
		}

		public void EndGameSession()
		{
			customPlayerTextures.Clear();
			gameSessionID = string.Empty;
			teamNames = new List<string>();
		}

		public string GetGameSessionID()
		{
			return gameSessionID;
		}

		public Texture2D GetPlayerTextureByIndex(int index)
		{
			return customPlayerTextures[index];
		}

		private bool IsTextureCountZero()
		{
			return customPlayerTextures.Count == 0;
		}

		private bool IsTexturesNull()
		{
			foreach (Texture2D customPlayerTexture in customPlayerTextures)
			{
				if (customPlayerTexture == null)
				{
					return true;
				}
			}
			return false;
		}

		public void SetGameSessionID(string GameSessionID)
		{
			gameSessionID = GameSessionID;
		}

		public void SetPlayerTextures(List<Texture2D> playerTextures)
		{
			customPlayerTextures = playerTextures;
		}

		public void SetTeamRequestModel(List<string> TeamNames)
		{
			teamNames = TeamNames;
		}
	}
}
