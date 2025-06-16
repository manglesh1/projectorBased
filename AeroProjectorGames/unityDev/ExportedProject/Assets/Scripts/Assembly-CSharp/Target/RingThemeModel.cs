using System;
using UnityEngine;

namespace Target
{
	[Serializable]
	public class RingThemeModel
	{
		[SerializeField]
		[Range(1f, 8f)]
		public int ringValue;

		[Space]
		[SerializeField]
		public Color ringBorderColor;

		[SerializeField]
		public Color ringColor;

		[Space]
		[SerializeField]
		public Color fontColor;
	}
}
