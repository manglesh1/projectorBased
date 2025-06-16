using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
	public class ToggleMaterial : MonoBehaviour
	{
		private int currentIndex = -1;

		[FormerlySerializedAs("onMaterial")]
		[SerializeField]
		private Material[] onMaterials;

		[FormerlySerializedAs("offShader")]
		[SerializeField]
		private Material offMaterial;

		[SerializeField]
		private Image targetObject;

		public void Toggle()
		{
			int num = ((currentIndex == onMaterials.Length - 1) ? (-1) : (++currentIndex));
			currentIndex = num;
			targetObject.material = ((currentIndex == -1) ? offMaterial : onMaterials[currentIndex]);
		}
	}
}
