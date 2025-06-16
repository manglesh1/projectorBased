using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Cricket.Logic.Gameboard.Animations
{
	public class TwiceAroundTheBoardAnimationController : MonoBehaviour
	{
		private const float COLOR_DURATION = 0.025f;

		private const int FULL_ROTATION_COUNT = 18;

		private const int ROTATION_EULER_ANGLES = 20;

		private CricketGameboardHitModel StartsectionModel;

		[Header("Existing Elements")]
		[SerializeField]
		private List<GameObject> exisitingSections = new List<GameObject>();

		[Header("Animation Elements")]
		[SerializeField]
		private List<GameObject> hitHighlight = new List<GameObject>();

		[SerializeField]
		private GameObject hitHighlightGroup;

		[SerializeField]
		private List<SpriteRenderer> sectionColors = new List<SpriteRenderer>();

		[SerializeField]
		private List<GameObject> sectionObjects = new List<GameObject>();

		[Header("Other Elements")]
		[SerializeField]
		private CricketGameboardAnimationOrchestrator gameboardAnimationOrchestrator;

		private void OnEnable()
		{
			StartsectionModel = gameboardAnimationOrchestrator.StartsectionModel;
			StartCoroutine(StartGameboardAnimation());
		}

		private void DetermineHightlightedSection()
		{
			DisableHitHighlightObjects();
			switch (StartsectionModel.Modifier)
			{
			case GameboardRingModifier.SingleThin:
				hitHighlight[3].SetActive(value: true);
				break;
			case GameboardRingModifier.SingleWide:
				hitHighlight[1].SetActive(value: true);
				break;
			case GameboardRingModifier.Double:
				hitHighlight[0].SetActive(value: true);
				break;
			case GameboardRingModifier.Triple:
				hitHighlight[2].SetActive(value: true);
				break;
			}
		}

		private Quaternion DetermineStartingSection()
		{
			foreach (GameObject exisitingSection in exisitingSections)
			{
				for (int i = 0; i < exisitingSection.transform.childCount; i++)
				{
					if (exisitingSection.name == StartsectionModel.GameObjectName)
					{
						return exisitingSection.transform.rotation;
					}
				}
			}
			return new Quaternion(0f, 0f, 0f, 0f);
		}

		private void DisableHitHighlightObjects()
		{
			hitHighlight.ForEach(delegate(GameObject g)
			{
				g.SetActive(value: false);
			});
		}

		private IEnumerator StartGameboardAnimation()
		{
			Quaternion allObjectInitialRotation = DetermineStartingSection();
			SetAllObjectInitialRotation(allObjectInitialRotation);
			DetermineHightlightedSection();
			for (int i = 0; i < 36; i++)
			{
				yield return LerpOneSectionColor(Color.green, Color.blue, 0.025f, sectionColors[0]);
				yield return LerpOneSectionColor(Color.blue, Color.white, 0.025f, sectionColors[0]);
				RotateObject(0);
			}
			yield return LerpOneSectionColor(Color.green, Color.blue, 0.25f, sectionColors[0]);
			yield return LerpOneSectionColor(Color.blue, Color.white, 0.25f, sectionColors[0]);
			yield return null;
			gameboardAnimationOrchestrator.HandleEndPlayCricketGameboardAnimation();
		}

		private void RotateObject(int objectIndex)
		{
			Quaternion rotation = sectionObjects[objectIndex].transform.rotation;
			rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z - 20f);
			sectionObjects[objectIndex].transform.rotation = rotation;
		}

		private void SetAllObjectInitialRotation(Quaternion setRotation)
		{
			hitHighlightGroup.transform.rotation = setRotation;
			sectionObjects[0].transform.rotation = setRotation;
			sectionObjects[1].transform.rotation = setRotation;
			sectionObjects[2].transform.rotation = setRotation;
		}

		private IEnumerator LerpOneSectionColor(Color startingColor, Color endingColor, float duration, SpriteRenderer boardSection)
		{
			float currentTime = 0f;
			boardSection.gameObject.SetActive(value: true);
			while (currentTime < duration)
			{
				boardSection.color = Color.Lerp(startingColor, endingColor, currentTime / duration);
				currentTime += Time.deltaTime;
				yield return null;
			}
			boardSection.gameObject.SetActive(value: false);
		}
	}
}
