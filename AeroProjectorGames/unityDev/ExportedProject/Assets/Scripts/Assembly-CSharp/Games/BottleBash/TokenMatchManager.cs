using System.Collections.Generic;
using Games.GameState;
using Settings;
using UnityEngine;

namespace Games.BottleBash
{
	public class TokenMatchManager : MonoBehaviour
	{
		private const float LOOP_MAX_ITERATIONS = 25f;

		private const float RAYCAST_LENGTH = 2f;

		private List<BottleBashTokens> _matchingTokens = new List<BottleBashTokens>();

		private string _spriteNameToMatch;

		private List<BottleBashTokens> _tokensToCheckForMatches = new List<BottleBashTokens>();

		private int _currentTurnTotalScore;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[Header("External Reference")]
		[SerializeField]
		private BottleBashSaveStates bottleBashSaveStates;

		private void AddTokenToListToCheck(BottleBashTokens token)
		{
			token.GameToken.GetComponent<Collider2D>().enabled = false;
			_tokensToCheckForMatches.Add(token);
		}

		private void CheckAdditionalTokens()
		{
			for (int i = 0; (float)i < 25f; i++)
			{
				if (_tokensToCheckForMatches.Count <= 0)
				{
					break;
				}
				FindAllAdjacentTokens(_tokensToCheckForMatches[0]);
				MoveFirstTokenFromCheckToMatching();
			}
		}

		private void DestroyAllAdjacentMatchingTokens()
		{
			foreach (BottleBashTokens matchingToken in _matchingTokens)
			{
				TokenController component = matchingToken.GameToken.GetComponent<TokenController>();
				DetermineScore(component.TokenScore);
				component.ExplodeSelf();
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					matchingToken.ScoringToken.GetComponent<TokenController>().ExplodeSelf();
				}
			}
		}

		public void DetermineMatches(BottleBashTokens tokenHit)
		{
			bottleBashSaveStates.SaveCurrentGameboardState();
			ResetValues();
			_spriteNameToMatch = GetTokenSpriteName(tokenHit.GameToken);
			tokenHit.GameToken.GetComponent<Collider2D>().enabled = false;
			_matchingTokens.Add(tokenHit);
			FindAllAdjacentTokens(tokenHit);
			CheckAdditionalTokens();
			DestroyAllAdjacentMatchingTokens();
			gameState.EnableTarget();
			gameEvents.RaiseScoreChange(_currentTurnTotalScore);
		}

		private void DetermineScore(int tokenScore)
		{
			_currentTurnTotalScore += tokenScore;
		}

		private void FindAllAdjacentTokens(BottleBashTokens tokenToUse)
		{
			RaycastFromToken(tokenToUse.GameToken, Vector2.left);
			RaycastFromToken(tokenToUse.GameToken, Vector2.right);
			RaycastFromToken(tokenToUse.GameToken, Vector2.up);
			RaycastFromToken(tokenToUse.GameToken, Vector2.down);
		}

		private string GetTokenSpriteName(GameObject token)
		{
			if (token.TryGetComponent<SpriteRenderer>(out var component))
			{
				return component.name;
			}
			return string.Empty;
		}

		private void MoveFirstTokenFromCheckToMatching()
		{
			_matchingTokens.Add(_tokensToCheckForMatches[0]);
			_tokensToCheckForMatches.RemoveAt(0);
		}

		private void RaycastFromToken(GameObject token, Vector2 rayDirection)
		{
			RaycastHit2D raycastHit2D = Physics2D.Raycast(new Vector2(token.transform.position.x, token.transform.position.y), rayDirection, 2f);
			if (raycastHit2D.collider != null && _spriteNameToMatch == GetTokenSpriteName(raycastHit2D.collider.gameObject))
			{
				AddTokenToListToCheck(raycastHit2D.collider.gameObject.GetComponent<TokenController>().Tokens);
			}
		}

		private void ResetValues()
		{
			_matchingTokens.Clear();
			_tokensToCheckForMatches.Clear();
			_spriteNameToMatch = string.Empty;
			_currentTurnTotalScore = 0;
		}
	}
}
