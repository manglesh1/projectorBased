using UnityEngine;

public class AdminBackgroundScaler : MonoBehaviour
{
	private const float SCALE_BUFFER = 1.5f;

	private void OnEnable()
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2((float)Screen.width * 1.5f, (float)Screen.height * 1.5f);
	}
}
