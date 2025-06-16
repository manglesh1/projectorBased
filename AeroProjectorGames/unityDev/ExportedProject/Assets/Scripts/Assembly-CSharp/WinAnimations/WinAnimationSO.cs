using System.Collections.Generic;
using UnityEngine;

namespace WinAnimations
{
	[CreateAssetMenu(menuName = "Animations/Win Animation Settings")]
	public class WinAnimationSO : ScriptableObject
	{
		public List<string> TextToShow { get; set; }
	}
}
