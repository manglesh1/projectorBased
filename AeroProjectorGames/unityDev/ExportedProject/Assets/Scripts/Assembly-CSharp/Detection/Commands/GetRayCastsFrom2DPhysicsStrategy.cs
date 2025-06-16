using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Detection.Commands
{
	public class GetRayCastsFrom2DPhysicsStrategy : MonoBehaviour
	{
		[SerializeField]
		private PhysicsRaycaster rayCaster;

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
			}
			return list.FirstOrDefault();
		}
	}
}
