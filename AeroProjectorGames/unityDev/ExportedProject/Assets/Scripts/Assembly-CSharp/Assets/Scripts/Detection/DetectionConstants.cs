using UnityEngine;

namespace Assets.Scripts.Detection
{
	public static class DetectionConstants
	{
		public const int DEFAULT_HEIGHT = 1;

		public const int DEFAULT_WIDTH = 1;

		public const float DEFAULT_POSITION_X = 0f;

		public const float DEFAULT_POSITION_Y = 0f;

		public const bool DEFAULT_DETECTION_ENABLED = false;

		public const bool DEFAULT_OBJECT_REMOVAL_DELAY = false;

		public const bool DEFAULT_EMITTER_ENABLED = false;

		public const int DEFAULT_EXPOSURE = 50000;

		public const int DEFAULT_GAIN = 16;

		public const int DEFAULT_LASER_POWER = 50;

		public const float DEFAULT_MAX_DISTANCE = 2.4f;

		public const float DEFAULT_MIN_DISTANCE = 1f;

		public const int DEFAULT_DOWNWARD_OFFSET = 0;

		public static readonly Vector2Int DEFAULT_GAME_BOARD_BOTTOM_LEFT = new Vector2Int(50, 2000);

		public static readonly Vector2Int DEFAULT_GAME_BOARD_TOP_RIGHT = new Vector2Int(1200, 1000);

		public static readonly Vector2Int DEFAULT_GAME_BOARD_TOP_LEFT = new Vector2Int(50, 1000);

		public static readonly Vector2Int DEFAULT_GAME_BOARD_BOTTOM_RIGHT = new Vector2Int(1200, 2000);

		public static readonly Vector2Int DEFAULT_ROI_BOTTOM_RIGHT = new Vector2Int(1200, 361);

		public static readonly Vector2Int DEFAULT_ROI_TOP_LEFT = new Vector2Int(50, 356);
	}
}
