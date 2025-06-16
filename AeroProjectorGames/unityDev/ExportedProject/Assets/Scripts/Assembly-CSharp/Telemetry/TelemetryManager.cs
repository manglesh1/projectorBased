using System.Collections;
using System.Collections.Generic;
using System.IO;
using API;
using API.ResponseModels;
using Admin.Demo;
using Authentication;
using Games;
using Helpers;
using Newtonsoft.Json;
using Stats;
using Telemetry.API;
using Telemetry.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace Telemetry
{
	public class TelemetryManager : MonoBehaviour
	{
		private string FILE_NAME = "stats.json";

		private int _currentGameId;

		private string _filePath;

		private bool _loaded;

		private JsonSerializerSettings _serializerSettings;

		private bool _sendingInProgress;

		private TelemetryCollection _telemetry;

		[SerializeField]
		private AxcitementApiHandler api;

		[SerializeField]
		private AuthenticationStateSO authState;

		[SerializeField]
		private TelemetryEventsSO telemetryEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private DemoSO demo;

		private TelemetryCollection Telemetry
		{
			get
			{
				if (_telemetry == null)
				{
					_telemetry = new TelemetryCollection();
				}
				return _telemetry;
			}
		}

		private void OnDisable()
		{
			gameEvents.OnGameOver -= HandleGameCompletedCollection;
			gameEvents.OnMainMenu -= HandleMainMenu;
			gameEvents.OnNewGame -= HandleNewGame;
			telemetryEvents.OnGameCompletedTelemetry -= HandleGameCompletedCollection;
			telemetryEvents.OnGameStartedTelemetry -= HandleGameStartedCollection;
		}

		private void OnEnable()
		{
			_serializerSettings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			};
			LoadFile();
			gameEvents.OnGameOver += HandleGameCompletedCollection;
			gameEvents.OnMainMenu += HandleMainMenu;
			gameEvents.OnNewGame += HandleNewGame;
			telemetryEvents.OnGameCompletedTelemetry += HandleGameCompletedCollection;
			telemetryEvents.OnGameStartedTelemetry += HandleGameStartedCollection;
		}

		private void Start()
		{
			StartCoroutine(SendTelemetry());
		}

		private IEnumerator SendTelemetry()
		{
			while (Application.isPlaying)
			{
				_sendingInProgress = true;
				GamesPlayedStatsApiRequest gamesPlayedStatsApiRequest = new GamesPlayedStatsApiRequest();
				foreach (KeyValuePair<int, TelemetryGamesPlayedStats> keyValuePair in Telemetry.GamesPlayed)
				{
					if (keyValuePair.Value.GamesCompleted > 0 && keyValuePair.Value.GamesStarted > 0)
					{
						gamesPlayedStatsApiRequest.Data.Add(new GamesPlayedStatsDetailsApiRequest
						{
							LicenseKey = authState.LicenseKey,
							GameId = keyValuePair.Value.GameId,
							AddGamesCreatedCount = keyValuePair.Value.GamesStarted,
							AddTotalCompletedCount = keyValuePair.Value.GamesCompleted
						});
					}
				}
				if (gamesPlayedStatsApiRequest.Data.Count > 0)
				{
					StartCoroutine(api.SendGamesPlayedStats(gamesPlayedStatsApiRequest, delegate(ApiResponse<BoolApiResponse> response)
					{
						if (response.Result == UnityWebRequest.Result.Success && response.Data.Success)
						{
							Telemetry.GamesPlayed.Clear();
							SaveFile();
						}
					}));
				}
				_sendingInProgress = false;
				yield return new WaitForSecondsRealtime(21600f);
			}
		}

		private void LoadFile()
		{
			_filePath = DataPathHelpers.GetManagedFilePath(Application.persistentDataPath, FILE_NAME);
			if (File.Exists(_filePath))
			{
				try
				{
					_telemetry = JsonConvert.DeserializeObject<TelemetryCollection>(File.ReadAllText(_filePath), _serializerSettings);
				}
				catch
				{
					File.Delete(_filePath);
					_telemetry = new TelemetryCollection();
				}
			}
			else
			{
				_telemetry = new TelemetryCollection();
			}
			_loaded = true;
		}

		private void SaveFile()
		{
			if (!_loaded)
			{
				LoadFile();
			}
			File.WriteAllText(_filePath, JsonConvert.SerializeObject(Telemetry, _serializerSettings));
		}

		private void HandleGameStartedCollection(int gameId)
		{
			if (gameId != 0 && !demo.DemoIsRunning)
			{
				_currentGameId = gameId;
				IncrementGamesStarted();
				SaveFile();
			}
		}

		private void HandleGameCompletedCollection()
		{
			if (_currentGameId != 0)
			{
				if (Telemetry.GamesPlayed.TryGetValue(_currentGameId, out var value))
				{
					value.GamesCompleted++;
				}
				else
				{
					Telemetry.GamesPlayed.Add(_currentGameId, new TelemetryGamesPlayedStats
					{
						GameId = _currentGameId,
						GamesCompleted = 1,
						GamesStarted = 1
					});
				}
				if (!_sendingInProgress)
				{
					SaveFile();
				}
			}
		}

		private void HandleMainMenu()
		{
			_currentGameId = 0;
		}

		private void HandleNewGame()
		{
			IncrementGamesStarted();
			SaveFile();
		}

		private void IncrementGamesStarted()
		{
			if (Telemetry.GamesPlayed.TryGetValue(_currentGameId, out var value))
			{
				value.GamesStarted++;
				return;
			}
			Telemetry.GamesPlayed.Add(_currentGameId, new TelemetryGamesPlayedStats
			{
				GameId = _currentGameId,
				GamesCompleted = 0,
				GamesStarted = 1
			});
		}
	}
}
