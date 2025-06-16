using System.Collections;
using Admin.Demo;
using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Games.Axteroids
{
	public class AxteroidsGameboardController : MonoBehaviour
	{
		private const int PARTICLE_Z_POSITION = 10;

		private readonly Vector2 SCREENPOINT_DEFAULT_VALUE = Vector2.negativeInfinity;

		private IEnumerator _playProperAnimationCoroutine;

		private IEnumerator _playMouseClickAnimationCoroutine;

		private Vector3 _worldPositionOfHitPoint;

		[SerializeField]
		private DemoSO demoManager;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[Header("Animation Timings")]
		[SerializeField]
		private float mouseClickAnimationlength = 1f;

		[SerializeField]
		private float waitTimeBetweenAllExplosions = 1f;

		[Header("Animations")]
		[SerializeField]
		private GameObject mouseClickAnimation;

		[Header("Object Animation Scripts")]
		[SerializeField]
		private ShipExplosion blockShipHit;

		[SerializeField]
		private AsteroidExplosion blueAsteroidHit;

		[SerializeField]
		private BombExplosion bombHit;

		[SerializeField]
		private ShipExplosion freighterHit;

		[SerializeField]
		private AsteroidExplosion greenAsteroidHit;

		[SerializeField]
		private ShipExplosion starshipHit;

		private void OnDisable()
		{
			if (_playProperAnimationCoroutine != null)
			{
				StopCoroutine(_playProperAnimationCoroutine);
			}
		}

		private void AnimationFinished(int scoreValue)
		{
			gameState.EnableTarget();
			gameEvents.RaiseScoreChange(scoreValue);
		}

		private int CalculateBombScoreValue()
		{
			return blockShipHit.ScoreValue + blueAsteroidHit.ScoreValue + freighterHit.ScoreValue + greenAsteroidHit.ScoreValue + starshipHit.ScoreValue;
		}

		private IEnumerator DestroyAllObjects()
		{
			yield return new WaitForSeconds(0.25f);
			StartCoroutine(greenAsteroidHit.ExplosionAnimation());
			yield return new WaitForSeconds(waitTimeBetweenAllExplosions);
			StartCoroutine(blueAsteroidHit.ExplosionAnimation());
			yield return new WaitForSeconds(waitTimeBetweenAllExplosions);
			StartCoroutine(freighterHit.ExplosionAnimation());
			yield return new WaitForSeconds(waitTimeBetweenAllExplosions);
			StartCoroutine(starshipHit.ExplosionAnimation());
			yield return new WaitForSeconds(waitTimeBetweenAllExplosions);
			yield return StartCoroutine(blockShipHit.ExplosionAnimation());
		}

		private void GetWorldPositionFromMousePosition()
		{
			Vector2 vector = Mouse.current.position.ReadValue();
			Vector3 position = new Vector3(vector.x, vector.y, 10f);
			_worldPositionOfHitPoint = Camera.main.ScreenToWorldPoint(position);
		}

		public void PlayAnimation(AxteroidsObjectList objectName, int scoreValue, PointerEventData pointerData, bool isMouseClicked)
		{
			gameState.DisableTarget();
			if (isMouseClicked)
			{
				GetWorldPositionFromMousePosition();
			}
			else
			{
				if (pointerData == null)
				{
					AnimationFinished(scoreValue);
					return;
				}
				Vector3 position = new Vector3(pointerData.position.x, pointerData.position.y, 10f);
				_worldPositionOfHitPoint = Camera.main.ScreenToWorldPoint(position);
			}
			_playProperAnimationCoroutine = PlayProperAnimations(objectName, scoreValue);
			StartCoroutine(_playProperAnimationCoroutine);
		}

		private IEnumerator PlayMouseClickAnimation()
		{
			mouseClickAnimation.transform.position = _worldPositionOfHitPoint;
			mouseClickAnimation.SetActive(value: true);
			yield return new WaitForSeconds(mouseClickAnimationlength);
			mouseClickAnimation.SetActive(value: false);
		}

		private IEnumerator PlayProperAnimations(AxteroidsObjectList objectName, int scoreValue)
		{
			if (!demoManager.DemoIsRunning)
			{
				_playMouseClickAnimationCoroutine = PlayMouseClickAnimation();
				yield return StartCoroutine(_playMouseClickAnimationCoroutine);
			}
			switch (objectName)
			{
			case AxteroidsObjectList.GreenAsteroid:
				yield return StartCoroutine(greenAsteroidHit.ExplosionAnimation());
				greenAsteroidHit.PrepareAnimation();
				break;
			case AxteroidsObjectList.BlueAsteroid:
				yield return StartCoroutine(blueAsteroidHit.ExplosionAnimation());
				blueAsteroidHit.PrepareAnimation();
				break;
			case AxteroidsObjectList.Freighter:
				yield return StartCoroutine(freighterHit.ExplosionAnimation());
				freighterHit.PrepareAnimation();
				break;
			case AxteroidsObjectList.Starship:
				yield return StartCoroutine(starshipHit.ExplosionAnimation());
				starshipHit.PrepareAnimation();
				break;
			case AxteroidsObjectList.BlockShip:
				yield return StartCoroutine(blockShipHit.ExplosionAnimation());
				blockShipHit.PrepareAnimation();
				break;
			case AxteroidsObjectList.Bomb:
				yield return StartCoroutine(bombHit.ExplosionAnimation());
				yield return StartCoroutine(StopAllObjectAnimations());
				yield return StartCoroutine(DestroyAllObjects());
				StartAllObjectAnimations();
				scoreValue = CalculateBombScoreValue();
				break;
			}
			AnimationFinished(scoreValue);
		}

		private void StartAllObjectAnimations()
		{
			greenAsteroidHit.PrepareAnimation();
			blueAsteroidHit.PrepareAnimation();
			freighterHit.PrepareAnimation();
			blockShipHit.PrepareAnimation();
			starshipHit.PrepareAnimation();
			bombHit.PrepareAnimation();
		}

		private IEnumerator StopAllObjectAnimations()
		{
			blockShipHit.StopAnimator();
			starshipHit.StopAnimator();
			freighterHit.StopAnimator();
			greenAsteroidHit.StopAnimator();
			blueAsteroidHit.StopAnimator();
			yield return null;
		}
	}
}
