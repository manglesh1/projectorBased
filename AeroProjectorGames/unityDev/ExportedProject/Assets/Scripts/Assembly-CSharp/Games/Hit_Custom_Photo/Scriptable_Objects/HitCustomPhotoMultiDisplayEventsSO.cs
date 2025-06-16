using Games.HitCustomPhotoController.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Games.Hit_Custom_Photo.Scriptable_Objects
{
	[CreateAssetMenu(menuName = "Games/Hit Custom Photo/MultiDisplay Events")]
	public class HitCustomPhotoMultiDisplayEventsSO : ScriptableObject
	{
		public event UnityAction<string, string> OnDeniedPlayersMessageChanged;

		public event UnityAction<GettingGamePhotoStates> OnGettingPhotosStateChanged;

		public event UnityAction<string, string, string> OnGameSessionMessageChanged;

		public event UnityAction<HitCustomPhotoGameStates> OnMultiDisplayStateChangeRequest;

		public event UnityAction<bool> OnQrCodeEnvironmentSet;

		public void RaiseDeniedPlayersMessageChanged(string group1Message, string group2Message)
		{
			this.OnDeniedPlayersMessageChanged?.Invoke(group1Message, group2Message);
		}

		public void RaiseGettingPhotosStateChanged(GettingGamePhotoStates state)
		{
			this.OnGettingPhotosStateChanged?.Invoke(state);
		}

		public void RaiseGameSessionMessageChanged(string visitUrl, string gameSessionId, string shortCodeText)
		{
			this.OnGameSessionMessageChanged?.Invoke(visitUrl, gameSessionId, visitUrl);
		}

		public void RaiseMultiDisplayStateChangeRequest(HitCustomPhotoGameStates state)
		{
			this.OnMultiDisplayStateChangeRequest?.Invoke(state);
		}

		public void RaiseSetQrCodeEnvironment(bool isProd)
		{
			this.OnQrCodeEnvironmentSet?.Invoke(isProd);
		}
	}
}
