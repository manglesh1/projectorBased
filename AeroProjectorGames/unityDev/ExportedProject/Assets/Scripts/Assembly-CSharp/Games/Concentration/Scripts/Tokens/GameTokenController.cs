using Games.Concentration.SO;
using Games.GameState;
using Settings;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Concentration.Scripts.Tokens
{
	public class GameTokenController : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private Renderer _renderer;

		[Header("Multi Display Child Token")]
		[SerializeField]
		private bool isChild;

		[SerializeField]
		private GameTokenController childToken;

		[SerializeField]
		private GameTokenController parentToken;

		[Header("Token Elements")]
		[SerializeField]
		private GameObject backBorderObject;

		[SerializeField]
		private GameObject correctMatchObject;

		[SerializeField]
		private GameObject failedMatchObject;

		[SerializeField]
		private GameObject frontBorderObject;

		[SerializeField]
		private GameObject particleSystemObject;

		[Header("Scriptable Objects")]
		[SerializeField]
		private ConcentrationGameEventsSO concentrationGameEvents;

		[SerializeField]
		private GameStateSO gameState;

		public GameObject CorrectMatchObject => correctMatchObject;

		public GameObject FailedMatchObject => failedMatchObject;

		public GameObject ParticleSystemObject => particleSystemObject;

		private void OnDisable()
		{
			if (childToken != null)
			{
				childToken.gameObject.SetActive(value: false);
			}
		}

		private void OnEnable()
		{
			backBorderObject.SetActive(value: true);
			frontBorderObject.SetActive(value: true);
			correctMatchObject.SetActive(value: false);
			failedMatchObject.SetActive(value: false);
			particleSystemObject.SetActive(value: false);
			_renderer = GetComponent<Renderer>();
			if (SettingsStore.Interaction.MultiDisplayEnabled && childToken != null)
			{
				childToken.gameObject.SetActive(value: true);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			HandleHitToken(eventData);
		}

		private void Update()
		{
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				if (isChild)
				{
					base.transform.rotation = parentToken.transform.rotation;
					failedMatchObject.SetActive(parentToken.failedMatchObject.activeSelf);
					correctMatchObject.SetActive(parentToken.correctMatchObject.activeSelf);
					particleSystemObject.SetActive(parentToken.particleSystemObject.activeSelf);
				}
				else
				{
					childToken.SetMaterials(_renderer.materials);
				}
			}
		}

		private void HandleHitToken(PointerEventData pointerEventData)
		{
			if (!gameState.IsTargetDisabled && gameState.GameStatus != GameStatus.Finished)
			{
				if (isChild)
				{
					parentToken.SendMessage("OnPointerDown", pointerEventData);
					return;
				}
				gameState.DisableTarget();
				concentrationGameEvents.RaiseTargetClicked(base.gameObject);
			}
		}

		private void OnHitDetected(PointerEventData pointerData)
		{
			HandleHitToken(pointerData);
		}

		private void SetMaterials(Material[] materials)
		{
			_renderer.materials = materials;
		}
	}
}
