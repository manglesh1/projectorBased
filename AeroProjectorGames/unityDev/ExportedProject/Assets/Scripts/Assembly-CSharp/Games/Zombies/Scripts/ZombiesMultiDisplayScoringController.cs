using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Zombies.Scripts
{
	public class ZombiesMultiDisplayScoringController : MonoBehaviour
	{
		private readonly List<GameObject> _targets = new List<GameObject>();

		[Header("Board Layout")]
		[SerializeField]
		private GameObject[] boardLayoutRow1;

		[SerializeField]
		private GameObject[] boardLayoutRow2;

		[SerializeField]
		private GameObject[] boardLayoutRow3;

		[SerializeField]
		private GameObject[] boardLayoutRow4;

		[SerializeField]
		private GameObject[] boardLayoutRow5;

		private void OnDisable()
		{
			Reset();
		}

		private void OnEnable()
		{
		}

		public GameObject AddTarget(GameObject target, int rowIndex, int colIndex)
		{
			GameObject[] array;
			switch (rowIndex)
			{
			case 1:
				array = boardLayoutRow1;
				break;
			case 2:
				array = boardLayoutRow2;
				break;
			case 3:
				array = boardLayoutRow3;
				break;
			case 4:
				array = boardLayoutRow4;
				break;
			case 5:
				array = boardLayoutRow5;
				break;
			default:
				throw new InvalidOperationException($"Unexpected AddTarget value: {rowIndex}");
			}
			GameObject[] array2 = array;
			GameObject gameObject = UnityEngine.Object.Instantiate(target, array2[colIndex - 1].transform);
			if (Screen.width / Screen.height == 0)
			{
				gameObject.transform.localScale = new Vector3(50f, 50f);
			}
			else
			{
				gameObject.transform.localScale = new Vector3(30f, 30f);
			}
			EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(delegate(BaseEventData eventData)
			{
				target.SendMessage("OnPointerDown", eventData);
			});
			eventTrigger.triggers.Add(entry);
			target.AddComponent<OnActiveChangeController>().OnActiveChanged.AddListener(gameObject.SetActive);
			_targets.Add(gameObject);
			return gameObject;
		}

		public void Reset()
		{
			_targets.ForEach(delegate(GameObject obj)
			{
				obj.GetComponent<EventTrigger>().triggers.ForEach(delegate(EventTrigger.Entry t)
				{
					t.callback.RemoveAllListeners();
				});
				UnityEngine.Object.DestroyImmediate(obj);
			});
			_targets.Clear();
		}
	}
}
