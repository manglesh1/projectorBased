using TMPro;
using UnityEngine;

namespace Games.Cricket.Scoreboard
{
	public class CricketCurrentThrowDisplayController : MonoBehaviour
	{
		private bool _firstUpdateOccured;

		[Header("Context")]
		[SerializeField]
		private CricketContextReadOnlySO context;

		[SerializeField]
		private CricketScoreboardEventsSO scoreboardEvents;

		[Header("UI Object")]
		[SerializeField]
		private TMP_Text currentThrow;

		[SerializeField]
		private ParticleSystem modifiedEffect;

		private void OnEnable()
		{
			scoreboardEvents.OnUpdateScoreboard += HandleScoreboardUpdate;
		}

		private void OnDisable()
		{
			scoreboardEvents.OnUpdateScoreboard -= HandleScoreboardUpdate;
		}

		private void HandleScoreboardUpdate()
		{
			currentThrow.text = context.ThrowsRemaining.ToString();
			if ((context.ThrowsRemaining != 3 || context.IsSinglePlayer) && _firstUpdateOccured)
			{
				modifiedEffect.Play();
			}
			if (!_firstUpdateOccured)
			{
				_firstUpdateOccured = true;
			}
		}
	}
}
