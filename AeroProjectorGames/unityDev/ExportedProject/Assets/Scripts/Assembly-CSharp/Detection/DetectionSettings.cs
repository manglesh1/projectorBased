using Assets.Scripts.Detection;
using Detection.Models;
using Newtonsoft.Json;
using Settings;
using UnityEngine;

namespace Detection
{
	public class DetectionSettings : BaseResizableAndMovableSettings
	{
		public DetectedCameraEnum DetectedCamera { get; set; }

		public bool DetectionEnabled { get; set; }

		[JsonIgnore]
		public bool IsDetectionOnAndEnabled => DetectedCamera != DetectedCameraEnum.None && DetectionEnabled;

		public bool ObjectRemovalDelay { get; set; }

		public bool EmitterEnabled { get; set; }

		public float Exposure { get; set; }

		public float Gain { get; set; }

		public float LaserPower { get; set; }

		public float MaxDistance { get; set; }

		public float MinDistance { get; set; }

		public decimal DownwardOffset { get; set; }

		public Vector2Int GameboardBottomLeft { get; set; }

		public Vector2Int GameboardBottomRight { get; set; }

		public Vector2Int GameboardTopLeft { get; set; }

		public Vector2Int GameboardTopRight { get; set; }

		public Vector2Int BottomRightROI { get; set; }

		public Vector2Int TopLeftROI { get; set; }

		public override SettingsKey StorageKey => SettingsKey.HitDetection;

		public DetectionSettings()
		{
			SetDefaults();
		}

		public new void Save()
		{
			SettingsStore.Set(this);
		}

		public override void SetDefaults()
		{
			base.Width = 1f;
			base.Height = 1f;
			base.PositionX = 0f;
			base.PositionY = 0f;
			DetectionEnabled = false;
			ObjectRemovalDelay = false;
			EmitterEnabled = false;
			Exposure = 50000f;
			Gain = 16f;
			LaserPower = 50f;
			MaxDistance = 2.4f;
			MinDistance = 1f;
			DownwardOffset = 0m;
			GameboardBottomLeft = DetectionConstants.DEFAULT_GAME_BOARD_BOTTOM_LEFT;
			GameboardTopRight = DetectionConstants.DEFAULT_GAME_BOARD_TOP_RIGHT;
			GameboardTopLeft = DetectionConstants.DEFAULT_GAME_BOARD_TOP_LEFT;
			GameboardBottomRight = DetectionConstants.DEFAULT_GAME_BOARD_BOTTOM_RIGHT;
			BottomRightROI = DetectionConstants.DEFAULT_ROI_BOTTOM_RIGHT;
			TopLeftROI = DetectionConstants.DEFAULT_ROI_TOP_LEFT;
		}
	}
}
