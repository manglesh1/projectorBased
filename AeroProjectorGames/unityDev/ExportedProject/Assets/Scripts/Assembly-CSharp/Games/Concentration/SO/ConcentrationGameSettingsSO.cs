using UnityEngine;

namespace Games.Concentration.SO
{
	[CreateAssetMenu(menuName = "Games/Concentration/Settings")]
	public class ConcentrationGameSettingsSO : ScriptableObject
	{
		public GameDifficulties GameDifficulty { get; set; }

		public bool SamePlayerAfterMatch { get; set; }
	}
}
