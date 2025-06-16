using System.Collections;
using System.Collections.Generic;
using Demo;
using Detection.Commands;
using Games;
using Scoreboard;
using Scoreboard.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Admin.Demo
{
	public class DemoGameController : MonoBehaviour
	{
		private const int TARGETS_TO_HIT_PER_GAME = 5;

		private IEnumerator _handleDemoEnumerator;

		private IEnumerator _playCodeGeneratedTargetsDemoEnumerator;

		private IEnumerator _playParentObjectsChangeEachFrameDemoEnumerator;

		private IEnumerator _playAlwaysSameTargetsDemoEnumerator;

		[SerializeField]
		private DemoEventsSO demoEvents;

		[SerializeField]
		private DemoSO demoManager;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[SerializeField]
		private SendClickEventFactory sendEventFactory;

		[Header("Demo Timers")]
		[Tooltip("Time when game first loads and first target click")]
		[SerializeField]
		private float newGameTimer = 4f;

		[Tooltip("Time between each target click")]
		[SerializeField]
		private float betweenTargetsTimer = 4f;

		[Tooltip("Time after last target click before returning to the main menu")]
		[SerializeField]
		private float afterAllTargetsTimer = 5f;

		[Header("Settings")]
		[SerializeField]
		private bool hasButtonSelection;

		[SerializeField]
		private bool hasSecondButtonSelection;

		[SerializeField]
		private TypeOfTargets targetType;

		[SerializeField]
		private TargetReactions targetReactions;

		[Header("Button Selection Objects")]
		[SerializeField]
		private List<GameObject> buttonSelection = new List<GameObject>();

		[SerializeField]
		private List<GameObject> secondButtonParents = new List<GameObject>();

		[Tooltip("List of parent objects that could contain a game target")]
		[SerializeField]
		private List<GameObject> parentsOfTargets = new List<GameObject>();

		[Tooltip("List of objects that are the game targets")]
		[SerializeField]
		private List<GameObject> targetsToActivate = new List<GameObject>();

		private void OnEnable()
		{
			if (demoManager.DemoIsRunning)
			{
				_handleDemoEnumerator = HandleDemo();
				StartCoroutine(_handleDemoEnumerator);
			}
		}

		private void OnDisable()
		{
			StopAllCoroutine();
		}

		private void Update()
		{
			bool flag = Touchscreen.current != null && Touchscreen.current.press.wasPressedThisFrame;
			if ((Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || flag) && demoManager.DemoIsRunning)
			{
				demoManager.DemoIsRunning = false;
				gameEvents.RaiseRemoveAllFromGameFlexSpace();
				scoreboardLoader.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(null, "", ScoreboardMessageStyle.Normal));
				scoreboardLoader.RaiseUnloadScoreboardRequest();
				gameEvents.RaiseMainMenu();
				demoEvents.RaiseDemoModeStopped();
			}
		}

		private int CheckForActiveGameObject(int indexToUse)
		{
			if (targetsToActivate[indexToUse].activeSelf)
			{
				return indexToUse;
			}
			for (int i = 0; i < targetsToActivate.Count; i++)
			{
				if (targetsToActivate[i].activeSelf)
				{
					return i;
				}
			}
			return indexToUse;
		}

		private int CheckForActiveImageComponent(int indexToUse)
		{
			if (targetsToActivate[indexToUse].GetComponent<Image>().enabled)
			{
				return indexToUse;
			}
			for (int i = 0; i < targetsToActivate.Count; i++)
			{
				if (targetsToActivate[i].GetComponent<Image>().enabled)
				{
					return i;
				}
			}
			return indexToUse;
		}

		private int CheckForNotActiveChild(int indexToUse, GameObject parentTarget)
		{
			if (parentTarget.transform.GetChild(indexToUse).gameObject.activeSelf)
			{
				return indexToUse;
			}
			for (int i = 0; i < parentTarget.transform.childCount; i++)
			{
				if (parentTarget.transform.GetChild(i).gameObject.activeSelf)
				{
					return i;
				}
			}
			return -1;
		}

		private void ClickButton()
		{
			new PointerEventData(EventSystem.current);
			int randomNumber = GetRandomNumber(buttonSelection.Count);
			buttonSelection[randomNumber].GetComponent<Button>().onClick.Invoke();
		}

		private void ClickChildTarget()
		{
			PointerEventData pointerData = new PointerEventData(EventSystem.current);
			GameObject childTarget = GetChildTarget();
			if (!(childTarget == null))
			{
				sendEventFactory.SendClick(childTarget, pointerData);
			}
		}

		private void ClickCodeGeneratedButton()
		{
			new PointerEventData(EventSystem.current);
			List<GameObject> secondButtonSelectionParentObjects = GetSecondButtonSelectionParentObjects();
			List<GameObject> generatedTargets = GetGeneratedTargets(secondButtonSelectionParentObjects);
			int randomNumber = GetRandomNumber(generatedTargets.Count);
			generatedTargets[randomNumber].GetComponent<Button>().onClick.Invoke();
		}

		private void ClickCodeGeneratedTarget(bool withColliders)
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			List<GameObject> generatedTargets = GetGeneratedTargets(parentsOfTargets);
			int randomNumber = GetRandomNumber(generatedTargets.Count);
			GameObject gameObject = generatedTargets[randomNumber];
			if (withColliders)
			{
				Collider componentInChildren = gameObject.GetComponentInChildren<Collider>();
				if (componentInChildren != null)
				{
					sendEventFactory.SendClick(componentInChildren.gameObject, pointerEventData);
					return;
				}
				Collider2D componentInChildren2 = gameObject.GetComponentInChildren<Collider2D>();
				if (componentInChildren2 != null)
				{
					Vector2 vector = componentInChildren2.bounds.center;
					pointerEventData.position = RectTransformUtility.WorldToScreenPoint(Camera.main, vector);
					sendEventFactory.SendClick(componentInChildren2.gameObject, pointerEventData);
				}
			}
			else
			{
				sendEventFactory.SendClick(gameObject, pointerEventData);
			}
		}

		private void ClickTarget()
		{
			PointerEventData pointerData = new PointerEventData(EventSystem.current);
			int num = GetRandomNumber(targetsToActivate.Count);
			if (targetReactions == TargetReactions.ImageComponentDisabled)
			{
				num = CheckForActiveImageComponent(num);
			}
			if (targetReactions == TargetReactions.GameObjectDisabled)
			{
				num = CheckForActiveGameObject(num);
			}
			GameObject proxiedObject = targetsToActivate[num];
			sendEventFactory.SendClick(proxiedObject, pointerData);
		}

		private GameObject GetChildTarget()
		{
			foreach (GameObject parentsOfTarget in parentsOfTargets)
			{
				if (parentsOfTarget.activeSelf)
				{
					int randomNumber = GetRandomNumber(parentsOfTarget.transform.childCount);
					randomNumber = CheckForNotActiveChild(randomNumber, parentsOfTarget);
					if (randomNumber >= 0)
					{
						return parentsOfTarget.transform.GetChild(randomNumber).gameObject;
					}
				}
			}
			return null;
		}

		private List<GameObject> GetGeneratedTargets(List<GameObject> parentObjects)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject parentObject in parentObjects)
			{
				if (parentObject.transform.childCount == 0)
				{
					continue;
				}
				for (int i = 0; i < parentObject.transform.childCount; i++)
				{
					GameObject gameObject = parentObject.transform.GetChild(i).gameObject;
					if (gameObject.activeSelf)
					{
						list.Add(gameObject);
					}
				}
			}
			return list;
		}

		private int GetRandomNumber(int MaxNumber)
		{
			return Random.Range(0, MaxNumber);
		}

		private List<GameObject> GetSecondButtonSelectionParentObjects()
		{
			List<GameObject> list = new List<GameObject>();
			GameObject[] array = GameObject.FindGameObjectsWithTag("ParentSelection");
			foreach (GameObject secondButtonParent in secondButtonParents)
			{
				GameObject[] array2 = array;
				foreach (GameObject gameObject in array2)
				{
					if (gameObject.name.Contains(secondButtonParent.name) && gameObject.activeSelf)
					{
						list.Add(gameObject);
						break;
					}
				}
			}
			return list;
		}

		private IEnumerator HandleDemo()
		{
			if (hasButtonSelection)
			{
				yield return new WaitForSeconds(2f);
				ClickButton();
			}
			if (hasSecondButtonSelection)
			{
				yield return new WaitForSeconds(2f);
				ClickCodeGeneratedButton();
			}
			yield return new WaitForSeconds(newGameTimer);
			switch (targetType)
			{
			case TypeOfTargets.AlwaysSameTargets:
				_playAlwaysSameTargetsDemoEnumerator = PlayAlwaysSameTargetsDemo();
				yield return StartCoroutine(_playAlwaysSameTargetsDemoEnumerator);
				break;
			case TypeOfTargets.ParentObjectsChangeEachFrame:
				_playParentObjectsChangeEachFrameDemoEnumerator = PlayParentObjectsChangeEachFrameDemo();
				yield return StartCoroutine(_playParentObjectsChangeEachFrameDemoEnumerator);
				break;
			case TypeOfTargets.TargetsAreCodeGenerated:
				_playCodeGeneratedTargetsDemoEnumerator = PlayCodeGeneratedTargetsDemo(withCollidiers: false);
				yield return StartCoroutine(_playCodeGeneratedTargetsDemoEnumerator);
				break;
			case TypeOfTargets.TargetsAreCodeGeneratedWithColliders:
				_playCodeGeneratedTargetsDemoEnumerator = PlayCodeGeneratedTargetsDemo(withCollidiers: true);
				yield return StartCoroutine(_playCodeGeneratedTargetsDemoEnumerator);
				break;
			}
			yield return new WaitForSeconds(afterAllTargetsTimer);
			gameEvents.RaiseMainMenu();
		}

		private IEnumerator PlayAlwaysSameTargetsDemo()
		{
			ClickTarget();
			for (int i = 0; i < 4; i++)
			{
				yield return new WaitForSeconds(betweenTargetsTimer);
				ClickTarget();
			}
		}

		private IEnumerator PlayCodeGeneratedTargetsDemo(bool withCollidiers)
		{
			ClickCodeGeneratedTarget(withCollidiers);
			for (int i = 0; i < 4; i++)
			{
				yield return new WaitForSeconds(betweenTargetsTimer);
				ClickCodeGeneratedTarget(withCollidiers);
			}
		}

		private IEnumerator PlayParentObjectsChangeEachFrameDemo()
		{
			ClickChildTarget();
			for (int i = 0; i < 4; i++)
			{
				yield return new WaitForSeconds(betweenTargetsTimer);
				ClickChildTarget();
			}
		}

		private void StopAllCoroutine()
		{
			if (_handleDemoEnumerator != null)
			{
				StopCoroutine(_handleDemoEnumerator);
			}
			if (_playCodeGeneratedTargetsDemoEnumerator != null)
			{
				StopCoroutine(_playCodeGeneratedTargetsDemoEnumerator);
			}
			if (_playParentObjectsChangeEachFrameDemoEnumerator != null)
			{
				StopCoroutine(_playParentObjectsChangeEachFrameDemoEnumerator);
			}
			if (_playAlwaysSameTargetsDemoEnumerator != null)
			{
				StopCoroutine(_playAlwaysSameTargetsDemoEnumerator);
			}
		}
	}
}
