using System.Collections.Generic;
using UnityEngine;

namespace Games.HitCustomPhotoController.Scripts
{
	public class PositionOptionTransforms : MonoBehaviour
	{
		[SerializeField]
		private List<Transform> targetpositions = new List<Transform>();

		public List<Transform> TargetPositions => targetpositions;
	}
}
