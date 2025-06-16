using Games;
using UnityEngine;

namespace Scoreboard
{
	public class GameFlexSpaceController : MonoBehaviour
	{
		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnEnable()
		{
			gameEvents.OnAddObjectToGameFlexSpace += AddChildObject;
			gameEvents.OnMainMenu += DestroyAllChildObjects;
			gameEvents.OnNewGame += DestroyAllChildObjects;
			gameEvents.OnRemoveObjectFromGameFlexSpace += RemoveChildObject;
			gameEvents.OnRemoveAllFromGameFlexSpace += DestroyAllChildObjects;
		}

		private void OnDisable()
		{
			gameEvents.OnAddObjectToGameFlexSpace -= AddChildObject;
			gameEvents.OnMainMenu -= DestroyAllChildObjects;
			gameEvents.OnNewGame -= DestroyAllChildObjects;
			gameEvents.OnRemoveObjectFromGameFlexSpace -= RemoveChildObject;
			gameEvents.OnRemoveAllFromGameFlexSpace -= DestroyAllChildObjects;
		}

		private void AddChildObject(GameObject objectToAdd)
		{
			objectToAdd.transform.SetParent(base.transform, worldPositionStays: false);
			objectToAdd.SetActive(value: true);
		}

		private void DestroyAllChildObjects()
		{
			foreach (object item in base.transform)
			{
				Object.Destroy(((Transform)item).gameObject);
			}
		}

		private void RemoveChildObject(GameObject objectToRemove)
		{
			Transform transform = base.transform.Find(objectToRemove.name);
			if (transform != null)
			{
				Object.Destroy(transform.gameObject);
			}
		}
	}
}
