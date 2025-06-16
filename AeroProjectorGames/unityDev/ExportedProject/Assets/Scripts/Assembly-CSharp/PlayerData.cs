using System;

[Serializable]
public class PlayerData
{
	public string PlayerName { get; set; }

	public int GamesWon { get; set; }

	public PlayerData(string userNames)
	{
		PlayerName = userNames;
	}
}
