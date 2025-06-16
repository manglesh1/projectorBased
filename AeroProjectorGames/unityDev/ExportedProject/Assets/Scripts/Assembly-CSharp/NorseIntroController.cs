using System.Collections;
using Assets.Games.Norse.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class NorseIntroController : MonoBehaviour
{
	[Header("Events")]
	[SerializeField]
	private NorseEventsSO norseEvents;

	[Header("UI Rows")]
	[SerializeField]
	private GameObject row1;

	[SerializeField]
	private GameObject row2;

	[SerializeField]
	private GameObject row3;

	[SerializeField]
	private GameObject row4;

	[SerializeField]
	private GameObject row5;

	[SerializeField]
	private GameObject row6;

	[Header("UI Buttons")]
	[SerializeField]
	private Button closeButton;

	private void HandleCloseButtonClick()
	{
		norseEvents.RaiseOnIntroCompleted();
	}

	private void HideAllRows()
	{
		row1.SetActive(value: false);
		row2.SetActive(value: false);
		row3.SetActive(value: false);
		row4.SetActive(value: false);
		row5.SetActive(value: false);
		row6.SetActive(value: false);
	}

	private IEnumerator ShowRows()
	{
		row1.SetActive(value: true);
		yield return new WaitForSeconds(1f);
		row2.SetActive(value: true);
		yield return new WaitForSeconds(1f);
		row3.SetActive(value: true);
		yield return new WaitForSeconds(1f);
		row4.SetActive(value: true);
		yield return new WaitForSeconds(1f);
		row5.SetActive(value: true);
		yield return new WaitForSeconds(1f);
		row6.SetActive(value: true);
	}

	private void OnDisable()
	{
		closeButton.onClick.RemoveAllListeners();
	}

	private void OnEnable()
	{
		closeButton.onClick.AddListener(HandleCloseButtonClick);
		HideAllRows();
		StartCoroutine(ShowRows());
	}
}
