using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Detection.Commands
{
	public class GetRayCastsFromGraphicsStrategy : MonoBehaviour
	{
		[SerializeField]
		private GraphicRaycaster rayCaster;

		private void OnDisable()
		{
		}

		private void OnEnable()
		{
		}

		public RaycastResult GetRayCasts(PointerEventData pointerData, Vector3 localPoint)
		{
			List<RaycastResult> list = new List<RaycastResult>();
			try
			{
				rayCaster.Raycast(pointerData, list);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				throw;
			}
			return list.FirstOrDefault();
		}
	}
}
