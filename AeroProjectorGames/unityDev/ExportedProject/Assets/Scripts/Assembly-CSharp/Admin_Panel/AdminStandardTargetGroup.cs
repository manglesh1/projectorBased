using System.Collections.Generic;
using Games;
using Games.Models;
using UnityEngine;

namespace Admin_Panel
{
	public class AdminStandardTargetGroup : MonoBehaviour
	{
		[Header("Toggle Item")]
		[SerializeField]
		private GameObject battleshipSettingsGroup;

		[Header("ViewableGames")]
		[SerializeField]
		private GameSO battleshipGameSO;

		[SerializeField]
		private ViewableGamesSO viewableGames;

		private List<GameSO> _licensedGames = new List<GameSO>();

		private void OnEnable()
		{
			battleshipSettingsGroup.gameObject.SetActive(value: false);
			_licensedGames = viewableGames.GetLicensedGames();
			foreach (GameSO licensedGame in _licensedGames)
			{
				if (battleshipGameSO.GameId == licensedGame.GameId)
				{
					battleshipSettingsGroup.gameObject.SetActive(value: true);
				}
			}
		}
	}
}
