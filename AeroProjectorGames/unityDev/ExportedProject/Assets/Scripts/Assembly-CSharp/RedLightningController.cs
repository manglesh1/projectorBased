using System.Collections;
using DigitalRuby.ThunderAndLightning;
using ResizingAndMoving;
using UnityEngine;

public class RedLightningController : MonoBehaviour
{
	private LightningBoltShapeConeScript _leftLightningConeScript;

	private LightningBoltShapeConeScript _rightLightningConeScript;

	[SerializeField]
	private GameObject leftLightningCone;

	[SerializeField]
	private GameObject rightLightningCone;

	[SerializeField]
	private SizeAndPositionStateSO gameboardSizeAndPosition;

	private void OnEnable()
	{
		_leftLightningConeScript = leftLightningCone.GetComponent<LightningBoltShapeConeScript>();
		_rightLightningConeScript = rightLightningCone.GetComponent<LightningBoltShapeConeScript>();
		_leftLightningConeScript.Length = 0f;
		_leftLightningConeScript.OuterRadius = 0f;
		_rightLightningConeScript.Length = 0f;
		_rightLightningConeScript.OuterRadius = 0f;
		StartCoroutine(PlayAnimation());
	}

	private IEnumerator PlayAnimation()
	{
		leftLightningCone.SetActive(value: true);
		rightLightningCone.SetActive(value: true);
		float endScale = 1f * gameboardSizeAndPosition.Width;
		float outerRadius = 1.5f * gameboardSizeAndPosition.Height;
		float duration = 0.4f;
		float animationCulling = 0.2f;
		yield return StartCoroutine(ScaleLightning(endScale, outerRadius, duration, animationCulling));
		yield return new WaitForSeconds(0.5f);
		endScale = 0f;
		outerRadius = 0f;
		duration = 0.8f;
		animationCulling = 0.4f;
		yield return StartCoroutine(ScaleLightning(endScale, outerRadius, duration, animationCulling));
		leftLightningCone.SetActive(value: false);
		rightLightningCone.SetActive(value: false);
	}

	private IEnumerator ScaleLightning(float endScale, float outerRadius, float duration, float animationCulling)
	{
		float currentTime = 0f;
		while (currentTime < duration - animationCulling)
		{
			_leftLightningConeScript.Length = Mathf.Lerp(_leftLightningConeScript.Length, endScale, currentTime / duration);
			_rightLightningConeScript.Length = Mathf.Lerp(_rightLightningConeScript.Length, endScale, currentTime / duration);
			_leftLightningConeScript.OuterRadius = Mathf.Lerp(_leftLightningConeScript.OuterRadius, outerRadius, currentTime / duration);
			_rightLightningConeScript.OuterRadius = Mathf.Lerp(_rightLightningConeScript.OuterRadius, outerRadius, currentTime / duration);
			currentTime += Time.deltaTime;
			yield return null;
		}
		_leftLightningConeScript.Length = endScale;
		_rightLightningConeScript.Length = endScale;
		_leftLightningConeScript.OuterRadius = outerRadius;
		_rightLightningConeScript.OuterRadius = outerRadius;
	}
}
