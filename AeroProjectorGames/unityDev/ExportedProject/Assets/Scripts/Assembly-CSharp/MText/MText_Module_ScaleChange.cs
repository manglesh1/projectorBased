using System.Collections;
using UnityEngine;

namespace MText
{
	[CreateAssetMenu(menuName = "Modular 3d Text/Modules/Change Scale")]
	public class MText_Module_ScaleChange : MText_Module
	{
		[Tooltip("If Grow From default is set to false Grow From Value will be used")]
		[SerializeField]
		private bool growFromDefaultScale;

		[Tooltip("If Grow From default is also set to false Grow From Value will be used")]
		[SerializeField]
		private Vector3 growFrom = Vector3.zero;

		[Space]
		[Tooltip("Grow to will be used only if Gro To Original is false")]
		[SerializeField]
		private bool growToOriginal = true;

		[Tooltip("Grow to will be used only if Gro To Original is false")]
		[SerializeField]
		private Vector3 growTo = Vector3.one;

		[Space]
		[Space]
		[Header("Don't forget to assign a animation curve")]
		[SerializeField]
		private AnimationCurve animationCurve;

		public override IEnumerator ModuleRoutine(GameObject obj, float duration)
		{
			Transform tr = obj.transform;
			Vector3 startScale = growFrom;
			if (growFromDefaultScale)
			{
				startScale = tr.localScale;
			}
			Vector3 targetScale = tr.localScale;
			if (!growToOriginal)
			{
				targetScale = growTo;
			}
			float timer = 0f;
			while (timer < duration && (bool)tr)
			{
				float time = timer / duration;
				tr.localScale = Vector3.Lerp(startScale, targetScale, animationCurve.Evaluate(time));
				timer += Time.deltaTime;
				yield return null;
			}
			if ((bool)tr)
			{
				tr.localScale = targetScale;
			}
		}
	}
}
