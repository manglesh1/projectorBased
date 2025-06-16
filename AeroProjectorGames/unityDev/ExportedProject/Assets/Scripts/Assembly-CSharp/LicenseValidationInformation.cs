using System;
using System.Collections.Generic;
using System.Globalization;

[Serializable]
public class LicenseValidationInformation
{
	public byte[] data;

	public string expires;

	public List<LicensedGames> licensedGames;

	public string licenseKey;

	public int laneNumber;

	public string reason;

	public string refreshToken;

	public bool success;

	public string token;

	public int weeklyFreeTrial;

	public string weeklyFreeTrialValidUntil;

	public DateTime GetExpirationDate()
	{
		DateTime.TryParse(expires, null, DateTimeStyles.AdjustToUniversal, out var result);
		return result;
	}

	public DateTime GetFreeTrialExpirationDate()
	{
		DateTime.TryParse(weeklyFreeTrialValidUntil, null, DateTimeStyles.AdjustToUniversal, out var result);
		return result;
	}
}
