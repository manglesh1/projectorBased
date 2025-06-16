using UnityEngine;

namespace Games.HitCustomPhotoController.Scripts
{
	public class TargetColorsHolder : MonoBehaviour
	{
		[Header("Target Colors")]
		[SerializeField]
		private Color blueColor;

		[SerializeField]
		private Color greenColor;

		[SerializeField]
		private Color pinkColor;

		[SerializeField]
		private Color yellowColor;

		public Color BlueColor => blueColor;

		public Color GreenColor => greenColor;

		public Color PinkColor => pinkColor;

		public Color YellowColor => yellowColor;
	}
}
