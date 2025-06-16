using System.Collections.Generic;
using Authentication;
using Detection.Models;
using Games.Models;
using Settings;
using UnityEngine;

namespace Games
{
	public class AvailableGamesManager : MonoBehaviour
	{
		[Header("Core Game Elements")]
		[SerializeField]
		private List<GameSO> coreGames;

		[Header("Free Detection Games")]
		[Tooltip("Free detection games always get set DetectionEnabled = true")]
		[SerializeField]
		private List<GameSO> freeDetectionGames;

		[Header("Paid Game Elements")]
		[SerializeField]
		private List<GameSO> paidGames;

		[Header("External References")]
		[SerializeField]
		private AuthenticationStateSO authenticationStateInfo;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private SettingsEventsSO settingsEvents;

		[SerializeField]
		private ViewableGamesSO viewableGames;

		private void OnEnable()
		{
			gameEvents.OnGameLicensedListUpdated += GetAllUserVisbleGames;
			settingsEvents.OnSettingsReloaded += ReloadGameVisibility;
			SetCoreGamesAvailableByDefault();
			SetGameDetectionSettings();
		}

		private void OnDisable()
		{
			gameEvents.OnGameLicensedListUpdated -= GetAllUserVisbleGames;
			settingsEvents.OnSettingsReloaded -= ReloadGameVisibility;
		}

		private void GetAllUserVisbleGames()
		{
			List<GameSO> list = new List<GameSO>();
			List<LicensedGames> licensedGames = authenticationStateInfo.LicensedGames;
			foreach (GameSO coreGame in coreGames)
			{
				list.Add(coreGame);
			}
			foreach (GameSO game in paidGames)
			{
				if (licensedGames.FindIndex((LicensedGames g) => g.gameId == game.GameId) != -1)
				{
					list.Add(game);
				}
			}
			if (SettingsStore.DetectionSettings.DetectionEnabled && SettingsStore.DetectionSettings.DetectedCamera != DetectedCameraEnum.None)
			{
				foreach (GameSO freeDetectionGame in freeDetectionGames)
				{
					freeDetectionGame.DetectionEnabled = true;
					list.Add(freeDetectionGame);
				}
			}
			viewableGames.UserAvailableGames = list;
			gameEvents.RaiseViewableGamesUpdated();
		}

		private void ReloadGameVisibility()
		{
			viewableGames.LoadSettings();
			GetAllUserVisbleGames();
		}

		private void SetGameDetectionSettings()
		{
			coreGames.ForEach(delegate(GameSO game)
			{
				game.DetectionEnabled = SettingsStore.GameAccessVisibility.DetectionEnabledForGameId.Contains(game.GameId);
			});
			paidGames.ForEach(delegate(GameSO game)
			{
				game.DetectionEnabled = SettingsStore.GameAccessVisibility.DetectionEnabledForGameId.Contains(game.GameId);
			});
		}

		private void SetCoreGamesAvailableByDefault()
		{
			viewableGames.UserAvailableGames = coreGames;
		}
	}
}
