namespace Backgrounds
{
	public class BackgroundSetting
	{
		private const float DEFAULT_ALPHA = 0.8f;

		public float Alpha { get; set; } = 0.8f;

		public BackgroundStyleEnum BackgroundStyle { get; set; }

		public string Name { get; set; }

		public string ColorHexValue { get; set; }
	}
}
