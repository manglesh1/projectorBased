using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(RectTransform))]
public class NorseStrikeThroughController : MonoBehaviour
{
	private readonly Color COLOR_DECEASED_TEXT = new Color(76f / 85f, 67f / 85f, 0.75686276f);

	private void Start()
	{
		LineRenderer component = GetComponent<LineRenderer>();
		Rect rect = GetComponent<RectTransform>().rect;
		component.startWidth = 0.05f;
		component.endWidth = 0.05f;
		component.positionCount = 2;
		component.useWorldSpace = false;
		component.SetPosition(0, new Vector3(0f, 0f, 0f));
		component.SetPosition(1, new Vector3(rect.width, 0f, 0f));
		component.startColor = COLOR_DECEASED_TEXT;
		component.endColor = COLOR_DECEASED_TEXT;
	}
}
