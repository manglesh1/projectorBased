using System.Collections;
using Games.Models;
using TMPro;
using UnityEngine;

namespace Games.Axteroids
{
	public class AxteroidsScoreDisplayController : MonoBehaviour
	{
		private int _displayExplosionTime = 4;

		private int _displayObjectTime = 2;

		private GameManager _gameManager;

		private IEnumerator _playScoreAnimationCoroutine;

		private IEnumerator _shipAnimationCoroutine;

		private IEnumerator _tokenAnimationCoroutine;

		private int _tokenTotalScoreValue;

		[SerializeField]
		private GameSO game;

		[SerializeField]
		private TMP_Text scoreValueText;

		[Header("Outline Materials")]
		[SerializeField]
		private Material outlineFill;

		[SerializeField]
		private Material outlineMask;

		[Header("Asteroid Explosion Properties")]
		[SerializeField]
		private float explodingForce;

		[SerializeField]
		private float radius;

		[Header("Green Asteroid Elements")]
		[SerializeField]
		private int asteroidScoreValue = 1;

		[SerializeField]
		private GameObject greenAsteroidActiveGroup;

		[SerializeField]
		private GameObject greenAsteroidExplosionGroup;

		[SerializeField]
		private Rigidbody greenAsteroidRigidBody1;

		[SerializeField]
		private Rigidbody greenAsteroidRigidBody2;

		[SerializeField]
		private Rigidbody greenAsteroidRigidBody3;

		[SerializeField]
		private Rigidbody greenAsteroidRigidBody4;

		[Header("Blue Asteroid Elements")]
		[SerializeField]
		private GameObject blueAsteroidActiveGroup;

		[SerializeField]
		private GameObject blueAsteroidExplosionGroup;

		[SerializeField]
		private Rigidbody blueAsteroidRigidBody1;

		[SerializeField]
		private Rigidbody blueAsteroidRigidBody2;

		[SerializeField]
		private Rigidbody blueAsteroidRigidBody3;

		[SerializeField]
		private Rigidbody blueAsteroidRigidBody4;

		[Header("Ship Elements")]
		[SerializeField]
		private GameObject blockShipActiveGroup;

		[SerializeField]
		private GameObject blockShipExplosionGroup;

		[SerializeField]
		private int blockShipScoreValue = 5;

		[SerializeField]
		private GameObject freighterActiveGroup;

		[SerializeField]
		private GameObject freighterExplosionGroup;

		[SerializeField]
		private int freighterScoreValue = 3;

		[SerializeField]
		private GameObject starshipActiveGroup;

		[SerializeField]
		private GameObject starshipExplosionGroup;

		[SerializeField]
		private int starshipScoreValue = 5;

		[Header("Bomb Elements")]
		[SerializeField]
		private GameObject bombActiveGroup;

		[SerializeField]
		private GameObject bombDiscriptionObject;

		[SerializeField]
		private GameObject bombExplosionGroup;

		[SerializeField]
		private TMP_Text bombScoreValueText;

		[Header("All At Once")]
		[SerializeField]
		private Animator MoveAllGroup_Anim;

		[SerializeField]
		private GameObject blockShipActiveGroup_All;

		[SerializeField]
		private GameObject blockShipExplosionGroup_All;

		[SerializeField]
		private GameObject freighterActiveGroup_All;

		[SerializeField]
		private GameObject freighterExplosionGroup_All;

		[SerializeField]
		private GameObject starshipActiveGroup_All;

		[SerializeField]
		private GameObject starshipExplosionGroup_All;

		[SerializeField]
		private GameObject greenAsteroidActiveGroup_All;

		[SerializeField]
		private GameObject greenAsteroidExplosionGroup_All;

		[SerializeField]
		private Rigidbody greenAsteroidRigidBody1_All;

		[SerializeField]
		private Rigidbody greenAsteroidRigidBody2_All;

		[SerializeField]
		private Rigidbody greenAsteroidRigidBody3_All;

		[SerializeField]
		private Rigidbody greenAsteroidRigidBody4_All;

		[SerializeField]
		private GameObject blueAsteroidActiveGroup_All;

		[SerializeField]
		private GameObject blueAsteroidExplosionGroup_All;

		[SerializeField]
		private Rigidbody blueAsteroidRigidBody1_All;

		[SerializeField]
		private Rigidbody blueAsteroidRigidBody2_All;

		[SerializeField]
		private Rigidbody blueAsteroidRigidBody3_All;

		[SerializeField]
		private Rigidbody blueAsteroidRigidBody4_All;

		private void OnEnable()
		{
			_gameManager = Object.FindObjectOfType<GameManager>();
			Reset();
			_playScoreAnimationCoroutine = PlayScoreAnimation();
			StartCoroutine(_playScoreAnimationCoroutine);
		}

		private void OnDisable()
		{
			if (_playScoreAnimationCoroutine != null)
			{
				StopCoroutine(_playScoreAnimationCoroutine);
			}
			if (_shipAnimationCoroutine != null)
			{
				StopCoroutine(_shipAnimationCoroutine);
			}
			if (_tokenAnimationCoroutine != null)
			{
				StopCoroutine(_tokenAnimationCoroutine);
			}
		}

		private IEnumerator ExplodeAllGameTokens()
		{
			blockShipActiveGroup_All.SetActive(value: true);
			freighterActiveGroup_All.SetActive(value: true);
			starshipActiveGroup_All.SetActive(value: true);
			greenAsteroidActiveGroup_All.SetActive(value: true);
			blueAsteroidActiveGroup_All.SetActive(value: true);
			MoveAllGroup_Anim.enabled = true;
			yield return new WaitForSeconds(1f);
			_tokenAnimationCoroutine = ExplodeSatellite();
			yield return StartCoroutine(_tokenAnimationCoroutine);
			MoveAllGroup_Anim.enabled = false;
			yield return new WaitForSeconds(_displayObjectTime / 2);
			_tokenAnimationCoroutine = ExplodeSingleAsteroid(blueAsteroidActiveGroup_All, blueAsteroidExplosionGroup_All, asteroidScoreValue, blueAsteroidRigidBody1_All, blueAsteroidRigidBody2_All, blueAsteroidRigidBody3_All, blueAsteroidRigidBody4_All);
			yield return StartCoroutine(_tokenAnimationCoroutine);
			_tokenAnimationCoroutine = ExplodeSingleAsteroid(greenAsteroidActiveGroup_All, greenAsteroidExplosionGroup_All, asteroidScoreValue, greenAsteroidRigidBody1_All, greenAsteroidRigidBody2_All, greenAsteroidRigidBody3_All, greenAsteroidRigidBody4_All);
			yield return StartCoroutine(_tokenAnimationCoroutine);
			_tokenAnimationCoroutine = ExplodeSingleShipTokenWithAddiveScore(freighterActiveGroup_All, freighterExplosionGroup_All, freighterScoreValue);
			yield return StartCoroutine(_tokenAnimationCoroutine);
			_tokenAnimationCoroutine = ExplodeSingleShipTokenWithAddiveScore(blockShipActiveGroup_All, blockShipExplosionGroup_All, blockShipScoreValue);
			yield return StartCoroutine(_tokenAnimationCoroutine);
			_tokenAnimationCoroutine = ExplodeSingleShipTokenWithAddiveScore(starshipActiveGroup_All, starshipExplosionGroup, starshipScoreValue);
			yield return StartCoroutine(_tokenAnimationCoroutine);
		}

		private IEnumerator EplodeAsteroidTokens()
		{
			greenAsteroidActiveGroup.SetActive(value: true);
			blueAsteroidActiveGroup.SetActive(value: true);
			yield return new WaitForSeconds(_displayObjectTime);
			greenAsteroidActiveGroup.SetActive(value: false);
			greenAsteroidExplosionGroup.SetActive(value: true);
			blueAsteroidActiveGroup.SetActive(value: false);
			blueAsteroidExplosionGroup.SetActive(value: true);
			greenAsteroidRigidBody1.AddExplosionForce(explodingForce, greenAsteroidExplosionGroup.transform.position, radius);
			greenAsteroidRigidBody2.AddExplosionForce(explodingForce, greenAsteroidExplosionGroup.transform.position, radius);
			greenAsteroidRigidBody3.AddExplosionForce(explodingForce, greenAsteroidExplosionGroup.transform.position, radius);
			greenAsteroidRigidBody4.AddExplosionForce(explodingForce, greenAsteroidExplosionGroup.transform.position, radius);
			blueAsteroidRigidBody1.AddExplosionForce(explodingForce, blueAsteroidExplosionGroup.transform.position, radius);
			blueAsteroidRigidBody2.AddExplosionForce(explodingForce, blueAsteroidExplosionGroup.transform.position, radius);
			blueAsteroidRigidBody3.AddExplosionForce(explodingForce, blueAsteroidExplosionGroup.transform.position, radius);
			blueAsteroidRigidBody4.AddExplosionForce(explodingForce, blueAsteroidExplosionGroup.transform.position, radius);
			scoreValueText.text = asteroidScoreValue.ToString();
			yield return new WaitForSeconds(_displayExplosionTime);
			greenAsteroidExplosionGroup.SetActive(value: false);
			blueAsteroidExplosionGroup.SetActive(value: false);
		}

		private IEnumerator ExplodeSatellite()
		{
			bombActiveGroup.SetActive(value: false);
			bombExplosionGroup.SetActive(value: true);
			bombDiscriptionObject.SetActive(value: false);
			yield return new WaitForSeconds(2f);
		}

		private IEnumerator ExplodeSingleAsteroid(GameObject gameTokenFullGroup, GameObject gameTokenPartsGroup, int gameTokenScoreValue, Rigidbody tokenPartRigidBody1, Rigidbody tokenPartRigidBody2, Rigidbody tokenPartRigidBody3, Rigidbody tokenPartRigidBody4)
		{
			gameTokenFullGroup.SetActive(value: false);
			gameTokenPartsGroup.SetActive(value: true);
			tokenPartRigidBody1.AddExplosionForce(explodingForce, gameTokenPartsGroup.transform.position, radius);
			tokenPartRigidBody2.AddExplosionForce(explodingForce, gameTokenPartsGroup.transform.position, radius);
			tokenPartRigidBody3.AddExplosionForce(explodingForce, gameTokenPartsGroup.transform.position, radius);
			tokenPartRigidBody4.AddExplosionForce(explodingForce, gameTokenPartsGroup.transform.position, radius);
			ShowTotalScoreValue(gameTokenScoreValue);
			yield return new WaitForSeconds(_displayExplosionTime / 2);
			gameTokenPartsGroup.SetActive(value: false);
		}

		private IEnumerator ExplodeSingleShipToken(GameObject gameToken, GameObject tokenParticle, int tokenScoreValue)
		{
			scoreValueText.text = string.Empty;
			gameToken.SetActive(value: true);
			yield return new WaitForSeconds(_displayObjectTime);
			gameToken.SetActive(value: false);
			tokenParticle.SetActive(value: true);
			scoreValueText.text = tokenScoreValue.ToString();
			yield return new WaitForSeconds(_displayExplosionTime);
			tokenParticle.SetActive(value: false);
		}

		private IEnumerator ExplodeSingleShipTokenWithAddiveScore(GameObject gameToken, GameObject tokenParticle, int gameTokenScoreValue)
		{
			gameToken.SetActive(value: false);
			tokenParticle.SetActive(value: true);
			ShowTotalScoreValue(gameTokenScoreValue);
			yield return new WaitForSeconds(_displayExplosionTime / 2);
			tokenParticle.SetActive(value: false);
		}

		private IEnumerator ExplodeTwoShipTokens(GameObject firstGameToken, GameObject firstTokenParticle, GameObject secondGameToken, GameObject secondTokenParticle, int tokenScoreValue)
		{
			scoreValueText.text = string.Empty;
			firstGameToken.SetActive(value: true);
			secondGameToken.SetActive(value: true);
			yield return new WaitForSeconds(_displayObjectTime);
			firstGameToken.SetActive(value: false);
			secondGameToken.SetActive(value: false);
			firstTokenParticle.SetActive(value: true);
			secondTokenParticle.SetActive(value: true);
			scoreValueText.text = tokenScoreValue.ToString();
			yield return new WaitForSeconds(_displayExplosionTime);
			firstTokenParticle.SetActive(value: false);
			secondTokenParticle.SetActive(value: false);
		}

		public void LoadGame()
		{
			outlineFill.renderQueue = 10;
			outlineMask.renderQueue = 10;
			_gameManager.LoadGame(game.GamePrefab, game);
		}

		private IEnumerator PlayScoreAnimation()
		{
			_shipAnimationCoroutine = ShipExplosions();
			yield return StartCoroutine(_shipAnimationCoroutine);
			_shipAnimationCoroutine = ShowSatellite();
			yield return StartCoroutine(_shipAnimationCoroutine);
			_shipAnimationCoroutine = ShowSatellite();
			yield return StartCoroutine(_shipAnimationCoroutine);
			_shipAnimationCoroutine = ExplodeAllGameTokens();
			yield return StartCoroutine(_shipAnimationCoroutine);
			LoadGame();
		}

		private void Reset()
		{
			outlineFill.renderQueue = 3000;
			outlineMask.renderQueue = 10;
			scoreValueText.text = string.Empty;
			bombScoreValueText.text = string.Empty;
			MoveAllGroup_Anim.enabled = false;
			greenAsteroidActiveGroup.SetActive(value: false);
			greenAsteroidExplosionGroup.SetActive(value: false);
			blueAsteroidActiveGroup.SetActive(value: false);
			blueAsteroidExplosionGroup.SetActive(value: false);
			blockShipActiveGroup.SetActive(value: false);
			blockShipExplosionGroup.SetActive(value: false);
			freighterActiveGroup.SetActive(value: false);
			freighterExplosionGroup.SetActive(value: false);
			starshipActiveGroup.SetActive(value: false);
			starshipExplosionGroup.SetActive(value: false);
			bombActiveGroup.SetActive(value: false);
			bombExplosionGroup.SetActive(value: false);
			bombDiscriptionObject.SetActive(value: false);
			blockShipActiveGroup_All.SetActive(value: false);
			blockShipExplosionGroup_All.SetActive(value: false);
			freighterActiveGroup_All.SetActive(value: false);
			freighterExplosionGroup_All.SetActive(value: false);
			starshipActiveGroup_All.SetActive(value: false);
			starshipExplosionGroup_All.SetActive(value: false);
			greenAsteroidActiveGroup_All.SetActive(value: false);
			greenAsteroidExplosionGroup_All.SetActive(value: false);
			blueAsteroidActiveGroup_All.SetActive(value: false);
			blueAsteroidExplosionGroup_All.SetActive(value: false);
		}

		private IEnumerator ShipExplosions()
		{
			_tokenAnimationCoroutine = EplodeAsteroidTokens();
			yield return StartCoroutine(_tokenAnimationCoroutine);
			_tokenAnimationCoroutine = ExplodeSingleShipToken(freighterActiveGroup, freighterExplosionGroup, freighterScoreValue);
			yield return StartCoroutine(_tokenAnimationCoroutine);
			_tokenAnimationCoroutine = ExplodeTwoShipTokens(blockShipActiveGroup, blockShipExplosionGroup, starshipActiveGroup, starshipExplosionGroup, starshipScoreValue);
			yield return StartCoroutine(_tokenAnimationCoroutine);
		}

		private IEnumerator ShowSatellite()
		{
			scoreValueText.text = string.Empty;
			bombActiveGroup.SetActive(value: true);
			yield return new WaitForSeconds(_displayObjectTime / 2);
			bombDiscriptionObject.SetActive(value: true);
			yield return new WaitForSeconds(_displayObjectTime / 2);
		}

		private void ShowTotalScoreValue(int gameTokenScoreValue)
		{
			_tokenTotalScoreValue += gameTokenScoreValue;
			bombScoreValueText.text = _tokenTotalScoreValue.ToString();
		}
	}
}
