using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Cricket.Logic.Gameboard.Animations
{
	public class InnerBullseyeRotateAllRingsController : MonoBehaviour
	{
		private const float ANIMATION_DURATION = 3f;

		private const float ROTATION_AMOUNT = 540f;

		private const float ROTATION_SPEED = 4f;

		[Header("Rotation Animation Elements")]
		[SerializeField]
		private List<GameObject> leftRotatingObjects = new List<GameObject>();

		[SerializeField]
		private List<GameObject> rightRotatingObjects = new List<GameObject>();

		[Header("Other Elements")]
		[SerializeField]
		private CricketGameboardAnimationOrchestrator gameboardAnimationOrchestrator;

		private void OnEnable()
		{
			StartCoroutine(StartGameboardAnimation());
		}

		private IEnumerator StartGameboardAnimation()
		{
			yield return LerpInnerBullseyeAnimation();
			yield return null;
			gameboardAnimationOrchestrator.HandleEndPlayCricketGameboardAnimation();
		}

		private IEnumerator LerpInnerBullseyeAnimation()
		{
			float currentTime = 0f;
			Quaternion startRotation = leftRotatingObjects[0].transform.rotation;
			Vector3 startVector = leftRotatingObjects[0].transform.localEulerAngles;
			while (currentTime < 3f)
			{
				leftRotatingObjects[0].transform.localEulerAngles = Vector3.Lerp(startVector, new Vector3(0f, 0f, -540f), currentTime / 4f);
				leftRotatingObjects[1].transform.localEulerAngles = Vector3.Lerp(startVector, new Vector3(0f, 0f, -540f), currentTime / 4f);
				rightRotatingObjects[0].transform.localEulerAngles = Vector3.Lerp(startVector, new Vector3(0f, 0f, 540f), currentTime / 4f);
				rightRotatingObjects[1].transform.localEulerAngles = Vector3.Lerp(startVector, new Vector3(0f, 0f, 540f), currentTime / 4f);
				currentTime += Time.deltaTime;
				yield return null;
			}
			leftRotatingObjects[0].transform.localEulerAngles = startVector;
			leftRotatingObjects[1].transform.rotation = startRotation;
			rightRotatingObjects[0].transform.rotation = startRotation;
			rightRotatingObjects[1].transform.rotation = startRotation;
		}
	}
}
