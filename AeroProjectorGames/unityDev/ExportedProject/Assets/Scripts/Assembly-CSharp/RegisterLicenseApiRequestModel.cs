using System;

[Serializable]
public class RegisterLicenseApiRequestModel
{
	public string licenseKey;

	public RegisterLicenseApiRequestModel(string key)
	{
		licenseKey = key;
	}
}
