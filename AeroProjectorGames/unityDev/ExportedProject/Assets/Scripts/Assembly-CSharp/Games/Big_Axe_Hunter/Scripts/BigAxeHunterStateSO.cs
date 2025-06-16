using System;
using UnityEngine;

namespace Games.Big_Axe_Hunter.Scripts
{
	[CreateAssetMenu(menuName = "Games/BigAxeHunter/BigAxeHunter State")]
	public class BigAxeHunterStateSO : ScriptableObject
	{
		public float AnimationDuration { get; set; }

		public ViewPosition CurrentViewPosition { get; set; }

		public AnimationCurve EasingCurve { get; set; }

		public Vector3[] FocalPoints { get; set; }

		public Vector3[] StandingPoints { get; set; }

		public int StartingFrameIndex { get; set; }

		public void Reset()
		{
			AnimationDuration = 0f;
			CurrentViewPosition = ViewPosition.Position1;
			EasingCurve = null;
			FocalPoints = Array.Empty<Vector3>();
			StandingPoints = Array.Empty<Vector3>();
			StartingFrameIndex = 0;
		}
	}
}
