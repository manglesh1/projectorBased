using System;
using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.BottleBash
{
	public class TokenController : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private bool _loaded;

		private TokenMatchManager _tokenMatchManager;

		private BottleBashTokens _tokens;

		[SerializeField]
		private GameObject particleEffect;

		[SerializeField]
		private Rigidbody2D rigidBody;

		[SerializeField]
		private int tokenScore = 1;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameStateSO gameState;

		public BottleBashTokens Tokens => _tokens;

		public int TokenScore => tokenScore;

		public void OnPointerDown(PointerEventData eventData)
		{
			TokenHit();
		}

		private void OnHitDetected(PointerEventData pointerData)
		{
			TokenHit();
		}

		private void EnsureLoaded()
		{
			if (!_loaded)
			{
				throw new ArgumentException("Token has not been initialized properly. Call the init function with the proper values first");
			}
		}

		public void ExplodeSelf()
		{
			EnsureLoaded();
			particleEffect.SetActive(value: true);
			particleEffect.transform.SetParent(base.gameObject.transform.parent.transform.parent.transform);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public void Init(BottleBashTokens tokens, TokenMatchManager manager)
		{
			_tokens = tokens;
			_tokenMatchManager = manager;
			_loaded = true;
		}

		private void TokenHit()
		{
			EnsureLoaded();
			if (!gameState.IsTargetDisabled && gameState.GameStatus != GameStatus.Finished && !(rigidBody.velocity.y < 0f))
			{
				gameState.DisableTarget();
				_tokenMatchManager.DetermineMatches(_tokens);
			}
		}
	}
}
