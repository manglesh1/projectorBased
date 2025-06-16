using System.Collections.Generic;
using Games;
using UnityEngine;

namespace UI.MultiDisplay
{
	public class LoadMultiDisplayScoringObjectController : MonoBehaviour
	{
		private GameObject _helpButtonObject;

		private List<GameObject> _loadedScoringObjects;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayScoringEvents;

		[SerializeField]
		private GameObject helpButtonParent;

		[SerializeField]
		private GameObject scoringDisplayParent;

		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnDisable()
		{
			gameEvents.OnGameLoading -= DestroyScoringObjects;
			gameEvents.OnMainMenu -= DestroyScoringObjects;
			multiDisplayScoringEvents.OnLoadMultiDisplayHelpButtonObject -= ParentHelpButtonObject;
			multiDisplayScoringEvents.OnLoadMultiDisplayScoringObject -= ParentScoringObject;
			multiDisplayScoringEvents.OnUnloadScoringObject -= DestroyScoringObjects;
			DestroyScoringObjects();
		}

		private void OnEnable()
		{
			gameEvents.OnGameLoading += DestroyScoringObjects;
			gameEvents.OnMainMenu += DestroyScoringObjects;
			multiDisplayScoringEvents.OnLoadMultiDisplayHelpButtonObject += ParentHelpButtonObject;
			multiDisplayScoringEvents.OnLoadMultiDisplayScoringObject += ParentScoringObject;
			multiDisplayScoringEvents.OnUnloadScoringObject += DestroyScoringObjects;
			if (_loadedScoringObjects == null)
			{
				_loadedScoringObjects = new List<GameObject>();
			}
		}

		private void DestroyHelpObjects()
		{
			if (_helpButtonObject != null)
			{
				Object.Destroy(_helpButtonObject);
			}
		}

		private void DestroyScoringObjects()
		{
			_loadedScoringObjects.ForEach(Object.Destroy);
			for (int i = 0; i < scoringDisplayParent.transform.childCount; i++)
			{
				Object.Destroy(scoringDisplayParent.transform.GetChild(i).gameObject);
			}
			_loadedScoringObjects.Clear();
			DestroyHelpObjects();
		}

		private void ParentHelpButtonObject(GameObject helpButtonObject)
		{
			_helpButtonObject = helpButtonObject;
			_helpButtonObject.transform.SetParent(helpButtonParent.transform, worldPositionStays: false);
			_helpButtonObject.transform.localScale = Vector3.one;
			RectTransform component = _helpButtonObject.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.anchoredPosition = Vector2.zero;
			component.sizeDelta = Vector2.zero;
		}

		private void ParentScoringObject(GameObject multiDisplayScoringObject)
		{
			_loadedScoringObjects.Add(multiDisplayScoringObject);
			multiDisplayScoringObject.transform.SetParent(scoringDisplayParent.transform, worldPositionStays: false);
			multiDisplayScoringObject.transform.localScale = Vector3.one;
			RectTransform component = multiDisplayScoringObject.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.anchoredPosition = Vector2.zero;
			component.sizeDelta = Vector2.zero;
		}
	}
}
