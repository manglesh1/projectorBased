using System.Collections;
using Games.GameState;
using UnityEngine;

namespace Games.HitCustomPhotoController.Scripts
{
	public class TargetMovementController : MonoBehaviour
	{
		private const float EASY_MAX_VELOCITY = 0.2f;

		private const float MEDIUM_MAX_VELOCITY = 0.5f;

		private const float HARD_MAX_VELOCITY = 0.8f;

		private const float STATIONARY_MAX_VELOCITY = 0f;

		private const float STATIONARY_MOVEMENT_TIME = 0.5f;

		private bool _isTargetStationary;

		private Vector3 _lastVelocity;

		private float _maxVelocity;

		private IEnumerator _stationarySetupCoroutine;

		private Rigidbody2D _targetRB;

		[SerializeField]
		private GameStateSO gameState;

		[Header("Rotation Elements")]
		[SerializeField]
		private RuntimeAnimatorController clockwiseRotationController;

		[SerializeField]
		private RuntimeAnimatorController counterClockwiseRotationController;

		[SerializeField]
		private Animator targetAnimator;

		private void OnCollisionEnter2D(Collision2D collision)
		{
			float magnitude = _lastVelocity.magnitude;
			Vector3 vector = Vector3.Reflect(_lastVelocity.normalized, collision.contacts[0].normal);
			_targetRB.velocity = vector * Mathf.Max(magnitude, 0f);
		}

		private void OnEnable()
		{
			base.gameObject.GetComponent<PolygonCollider2D>().enabled = true;
			RandomizeRotation();
			if (_isTargetStationary)
			{
				_stationarySetupCoroutine = StationarySetupPhase();
				StartCoroutine(_stationarySetupCoroutine);
			}
		}

		private void OnDisable()
		{
			if (_stationarySetupCoroutine != null)
			{
				StopCoroutine(_stationarySetupCoroutine);
			}
		}

		private void Start()
		{
			_targetRB = GetComponent<Rigidbody2D>();
			_isTargetStationary = false;
			if (gameState.GameType == "Easy")
			{
				_maxVelocity = 0.2f;
			}
			else if (gameState.GameType == "Medium")
			{
				_maxVelocity = 0.5f;
			}
			else if (gameState.GameType == "Hard")
			{
				_maxVelocity = 0.8f;
			}
			else
			{
				_isTargetStationary = true;
				_stationarySetupCoroutine = StationarySetupPhase();
				StartCoroutine(_stationarySetupCoroutine);
			}
			AddForceToTarget();
		}

		private void Update()
		{
			if (_maxVelocity != 0f)
			{
				_lastVelocity = _targetRB.velocity;
				if (_targetRB.velocity.magnitude <= _maxVelocity / 2f)
				{
					AddForceToTarget();
				}
				CheckForMaxVelocity();
			}
			else
			{
				_targetRB.velocity = Vector2.zero;
			}
		}

		private void AddForceToTarget()
		{
			int randomNumber = GetRandomNumber();
			int randomNumber2 = GetRandomNumber();
			Vector2 force = new Vector2(randomNumber, randomNumber2);
			_targetRB.AddForce(force);
		}

		private void CheckForMaxVelocity()
		{
			if (_targetRB.velocity.x > _maxVelocity)
			{
				_targetRB.velocity = new Vector2(_maxVelocity, _targetRB.velocity.y);
			}
			if (_targetRB.velocity.y > _maxVelocity)
			{
				_targetRB.velocity = new Vector2(_targetRB.velocity.x, _maxVelocity);
			}
		}

		private int GetRandomNumber()
		{
			int num = 19;
			int num2 = 50;
			int num3 = -19;
			int minInclusive = -50;
			int num4 = 100;
			int num5 = num2;
			for (int i = 0; i <= num4; i++)
			{
				num5 = Random.Range(minInclusive, num2);
				if (num5 < num3 || num5 > num)
				{
					break;
				}
				if (i == num4)
				{
					num5 = num2;
				}
			}
			return num5;
		}

		private void RandomizeRotation()
		{
			if (Random.Range(0, 2) == 0)
			{
				targetAnimator.runtimeAnimatorController = counterClockwiseRotationController;
			}
			else
			{
				targetAnimator.runtimeAnimatorController = clockwiseRotationController;
			}
			targetAnimator.speed = Random.Range(0.6f, 1.5f);
		}

		private IEnumerator StationarySetupPhase()
		{
			_maxVelocity = 0.8f;
			yield return new WaitForSeconds(0.5f);
			_maxVelocity = 0f;
		}
	}
}
