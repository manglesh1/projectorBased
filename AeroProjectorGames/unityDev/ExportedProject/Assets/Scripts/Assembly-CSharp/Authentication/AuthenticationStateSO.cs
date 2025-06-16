using System;
using System.Collections.Generic;
using Games;
using UnityEngine;

namespace Authentication
{
	[CreateAssetMenu(menuName = "Authentication/Authentication Info")]
	public class AuthenticationStateSO : ScriptableObject
	{
		private const int OFFLINE_WINDOW = 5;

		[SerializeField]
		private GameEventsSO gameEvents;

		private List<LicensedGames> _licensedGames = new List<LicensedGames>();

		public AuthenticationStatus AuthenticationStatus { get; set; }

		public string BearerToken { get; set; }

		public int DaysRemaining
		{
			get
			{
				int num = TokenExpiration.Subtract(DateTime.UtcNow).Days;
				if (num < 0)
				{
					num = 0;
				}
				return num;
			}
		}

		public bool InOfflineWindow => TokenExpiration <= DateTime.UtcNow.AddDays(5.0);

		public int LaneNumber { get; set; }

		public List<LicensedGames> LicensedGames
		{
			get
			{
				return _licensedGames;
			}
			set
			{
				_licensedGames = value;
				gameEvents.RaiseGameLicensedListUpdated();
			}
		}

		public string LicenseKey { get; set; }

		public DateTime TokenExpiration { get; set; }

		public void LoadAuthInfoFromLicenseValidation(LicenseValidationInformation licenseValidationInformation)
		{
			BearerToken = licenseValidationInformation.token;
			LaneNumber = licenseValidationInformation.laneNumber;
			LicensedGames = licenseValidationInformation.licensedGames;
			LicenseKey = licenseValidationInformation.licenseKey;
			TokenExpiration = licenseValidationInformation.GetExpirationDate();
		}
	}
}
