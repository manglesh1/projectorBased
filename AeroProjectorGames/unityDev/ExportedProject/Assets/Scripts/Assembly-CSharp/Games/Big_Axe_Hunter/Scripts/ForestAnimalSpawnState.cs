using Games.SharedScoringLogic;

namespace Games.Big_Axe_Hunter.Scripts
{
	public struct ForestAnimalSpawnState : IScore
	{
		public int? Animal1 { get; set; }

		public int? Animal2 { get; set; }

		public ViewPosition? ViewPosition { get; set; }

		public bool IsComplete()
		{
			return Animal1.HasValue && Animal2.HasValue && ViewPosition.HasValue;
		}
	}
}
