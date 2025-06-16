using Settings;
using Target;
using UnityEngine;
using UnityEngine.UI;

namespace Admin_Panel
{
	public class AdminStandardTargetSettingsController : MonoBehaviour
	{
		[SerializeField]
		private Toggle fiveFrameGameToggle;

		[SerializeField]
		private Toggle playWinAnimationsToggle;

		[SerializeField]
		private Toggle rotateTargetsToggle;

		[SerializeField]
		private Toggle showKillZoneToggle;

		[SerializeField]
		private Toggle throwsPerTurnEnabled;

		[SerializeField]
		private Toggle useShorterBattleshipAnimationsToggle;

		[SerializeField]
		private Toggle useSixRingTargetsToggle;

		[Header("Settings Elements")]
		private TargetSettings _targetSettings;

		private void OnEnable()
		{
			GetTargetSettings();
			fiveFrameGameToggle.isOn = _targetSettings.FiveFrameGame;
			playWinAnimationsToggle.isOn = _targetSettings.PlayWinAnimations;
			rotateTargetsToggle.isOn = _targetSettings.RotateTargets;
			showKillZoneToggle.isOn = _targetSettings.ShowKillZones;
			throwsPerTurnEnabled.isOn = _targetSettings.ThrowsPerTurnEnabled;
			useShorterBattleshipAnimationsToggle.isOn = _targetSettings.UseShorterBattleshipAnimations;
			useSixRingTargetsToggle.isOn = _targetSettings.UseSixRingTarget;
		}

		private void GetTargetSettings()
		{
			_targetSettings = SettingsStore.Target;
		}

		public void FiveFrameGameValueChange(Toggle newValue)
		{
			_targetSettings.FiveFrameGame = newValue.isOn;
			_targetSettings.Save();
		}

		public void PlayWinAnimationValueChange(Toggle newValue)
		{
			_targetSettings.PlayWinAnimations = newValue.isOn;
			_targetSettings.Save();
		}

		public void RotateTargetValueChange(Toggle newValue)
		{
			_targetSettings.RotateTargets = newValue.isOn;
			_targetSettings.Save();
		}

		public void ShowKillZoneValueChange(Toggle newValue)
		{
			_targetSettings.ShowKillZones = newValue.isOn;
			_targetSettings.Save();
		}

		public void ThrowsPerTurnEnabledValueChange(Toggle newValue)
		{
			_targetSettings.ThrowsPerTurnEnabled = newValue.isOn;
			_targetSettings.Save();
		}

		public void UseShorterBattleshipAnimationsValueChange(Toggle newValue)
		{
			_targetSettings.UseShorterBattleshipAnimations = newValue.isOn;
			_targetSettings.Save();
		}

		public void UseSixRingTargetsValueChange(Toggle newValue)
		{
			_targetSettings.UseSixRingTarget = newValue.isOn;
			_targetSettings.Save();
		}
	}
}
