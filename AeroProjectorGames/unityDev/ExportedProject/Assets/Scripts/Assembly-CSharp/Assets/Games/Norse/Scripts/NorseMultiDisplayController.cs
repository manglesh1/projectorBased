using Games;
using UnityEngine;

namespace Assets.Games.Norse.Scripts
{
	public class NorseMultiDisplayController : MonoBehaviour
	{
		[Header("Events")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private NorseEventsSO norseEvents;

		[Header("State")]
		[SerializeField]
		private NorseStateSO norseState;

		[Header("Views")]
		[SerializeField]
		private GameObject multiDisplayGameboard;

		[SerializeField]
		private GameObject multiDisplayHelp;

		[SerializeField]
		private GameObject multiDisplayWords;

		[Header("Power Up Sprites")]
		[SerializeField]
		private Sprite imageAttack;

		[SerializeField]
		private Sprite imageExtraLife;

		[SerializeField]
		private Sprite imageProtect;

		[SerializeField]
		private Sprite imageReverse;

		[SerializeField]
		private Sprite imageSteal;

		[SerializeField]
		private Sprite imageUnknown;

		[Header("UI Elements")]
		[SerializeField]
		private GameObject grid;

		[SerializeField]
		private GameObject powerUpCircle;

		[SerializeField]
		private GameObject targetCircle;

		private Sprite GetPowerUpSprite()
		{
			return norseState.SelectedPowerUp switch
			{
				NorsePowerUpEnum.Attack => imageAttack, 
				NorsePowerUpEnum.ExtraLife => imageExtraLife, 
				NorsePowerUpEnum.Protect => imageProtect, 
				NorsePowerUpEnum.Reverse => imageReverse, 
				NorsePowerUpEnum.Steal => imageSteal, 
				_ => imageUnknown, 
			};
		}

		private void OnGameStarted()
		{
			multiDisplayGameboard.SetActive(value: true);
			multiDisplayWords.SetActive(value: false);
			grid.SetActive(value: true);
			powerUpCircle.SetActive(value: false);
			targetCircle.SetActive(value: false);
		}

		private void OnHelpClosed()
		{
			multiDisplayGameboard.SetActive(value: true);
			multiDisplayHelp.SetActive(value: false);
			multiDisplayWords.SetActive(value: false);
		}

		private void OnHelpOpened()
		{
			multiDisplayGameboard.SetActive(value: false);
			multiDisplayHelp.SetActive(value: true);
			multiDisplayWords.SetActive(value: false);
		}

		private void OnPowerUpSelected()
		{
			powerUpCircle.SetActive(value: true);
			SpriteRenderer component = powerUpCircle.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				component.sprite = GetPowerUpSprite();
			}
		}

		private void OnUpdateScoreboard()
		{
			if (multiDisplayGameboard.activeInHierarchy)
			{
				if (norseState.IsCommandSet)
				{
					grid.SetActive(value: false);
					targetCircle.SetActive(value: true);
				}
				else
				{
					grid.SetActive(value: true);
					powerUpCircle.SetActive(value: false);
					targetCircle.SetActive(value: false);
				}
			}
		}

		private void OnWordSelection()
		{
			multiDisplayWords.SetActive(value: true);
			multiDisplayGameboard.SetActive(value: false);
		}

		private void OnDisable()
		{
			gameEvents.OnUpdateScoreboard -= OnUpdateScoreboard;
			norseEvents.OnGameStarted -= OnGameStarted;
			norseEvents.OnHelpClosed -= OnHelpClosed;
			norseEvents.OnHelpOpened -= OnHelpOpened;
			norseEvents.OnPowerUpSelected -= OnPowerUpSelected;
			norseEvents.OnWordSelection -= OnWordSelection;
		}

		private void OnEnable()
		{
			gameEvents.OnUpdateScoreboard += OnUpdateScoreboard;
			norseEvents.OnGameStarted += OnGameStarted;
			norseEvents.OnHelpClosed += OnHelpClosed;
			norseEvents.OnHelpOpened += OnHelpOpened;
			norseEvents.OnPowerUpSelected += OnPowerUpSelected;
			norseEvents.OnWordSelection += OnWordSelection;
		}
	}
}
