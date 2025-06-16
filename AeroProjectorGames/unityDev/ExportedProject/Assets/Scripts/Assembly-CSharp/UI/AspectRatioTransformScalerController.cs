using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	public class AspectRatioTransformScalerController : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> objectsToScale;

		[SerializeField]
		private Vector3 portraitScale;

		[SerializeField]
		private Vector3 landscapeScale;

		private void OnDisable()
		{
		}

		private void OnEnable()
		{
			SetScale();
		}

		public void SetScale()
		{
			if (Screen.width / Screen.height > 0)
			{
				objectsToScale.ForEach(delegate(GameObject obj)
				{
					obj.transform.localScale = landscapeScale;
				});
			}
			else
			{
				objectsToScale.ForEach(delegate(GameObject obj)
				{
					obj.transform.localScale = portraitScale;
				});
			}
		}
	}
}
