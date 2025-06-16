using System.Collections.Generic;
using UnityEngine;

namespace Games.HitCustomPhotoController.Scripts
{
	public class TargetPlacementSelection : MonoBehaviour
	{
		[SerializeField]
		private List<PositionOptionTransforms> targetPositionOptions = new List<PositionOptionTransforms>();

		private void OnEnable()
		{
			HideTargetOptionsGameObjects();
		}

		public PositionOptionTransforms GetTargetPositions()
		{
			new List<PositionOptionTransforms>();
			RandomizeTargetPositionRotation();
			int index = Random.Range(0, targetPositionOptions.Count);
			return targetPositionOptions[index];
		}

		private void HideTargetOptionsGameObjects()
		{
			foreach (PositionOptionTransforms targetPositionOption in targetPositionOptions)
			{
				targetPositionOption.gameObject.SetActive(value: false);
			}
		}

		private void RandomizeTargetPositionRotation()
		{
			switch (Random.Range(0, 4))
			{
			case 1:
				base.transform.Rotate(new Vector3(0f, 0f, 90f));
				break;
			case 2:
				base.transform.Rotate(new Vector3(0f, 0f, 180f));
				break;
			case 3:
				base.transform.Rotate(new Vector3(0f, 0f, 270f));
				break;
			default:
				base.transform.Rotate(new Vector3(0f, 0f, 0f));
				break;
			}
		}
	}
}
