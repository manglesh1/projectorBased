using System.Collections;
using UnityEngine;

namespace MText
{
	[CreateAssetMenu(menuName = "Modular 3d Text/Modules/Change Position")]
	public class MText_Module_PositionChange : MText_Module
	{
		[Header("This Module always put a single frame delay")]
		[Space]
		[SerializeField]
		private float delayBeforeStarting;

		[SerializeField]
		private bool useLocalPosition;

		[Space]
		[Space]
		[Tooltip("If start From default is set to false 'Start From' Value will be used")]
		[SerializeField]
		private bool startFromDefaultPosition = true;

		[Tooltip("If Grow From default is also set to false Grow From Value will be used")]
		[SerializeField]
		private Vector3 startFrom = Vector3.zero;

		[Space]
		[Space]
		[Tooltip("Move to will be used only if Move To Original is false")]
		[SerializeField]
		private bool moveToOriginal;

		[Tooltip("If set to true, move to will be used to add position instead of literally moving to new position")]
		[SerializeField]
		private bool addMoveToValue;

		[Tooltip("Move to will be used only if Move To Original is false")]
		[SerializeField]
		private Vector3 moveTo = Vector3.zero;

		[Space]
		[Space]
		[Header("Don't forget to assign a animation curve")]
		[SerializeField]
		private AnimationCurve animationCurve;

		public override IEnumerator ModuleRoutine(GameObject obj, float duration)
		{
			obj.SetActive(value: false);
			yield return null;
			if (!obj)
			{
				yield break;
			}
			Transform tr = obj.transform;
			Vector3 startPosition = startFrom;
			if (startFromDefaultPosition)
			{
				startPosition = ((!useLocalPosition) ? tr.position : tr.localPosition);
			}
			Vector3 targetPosition = moveTo;
			if (moveToOriginal)
			{
				targetPosition = ((!useLocalPosition) ? tr.position : tr.localPosition);
			}
			else if (addMoveToValue)
			{
				if (useLocalPosition)
				{
					targetPosition += tr.localPosition;
				}
				else
				{
					targetPosition += tr.position;
				}
			}
			yield return new WaitForSeconds(delayBeforeStarting);
			obj.SetActive(value: true);
			float timer = 0f;
			while (timer < duration && (bool)tr)
			{
				float time = timer / duration;
				if (useLocalPosition)
				{
					tr.localPosition = Vector3.Lerp(startPosition, targetPosition, animationCurve.Evaluate(time));
				}
				else
				{
					tr.position = Vector3.Lerp(startPosition, targetPosition, animationCurve.Evaluate(time));
				}
				timer += Time.deltaTime;
				yield return null;
			}
			if ((bool)tr)
			{
				if (useLocalPosition)
				{
					tr.localPosition = targetPosition;
				}
				else
				{
					tr.position = targetPosition;
				}
			}
		}
	}
}
