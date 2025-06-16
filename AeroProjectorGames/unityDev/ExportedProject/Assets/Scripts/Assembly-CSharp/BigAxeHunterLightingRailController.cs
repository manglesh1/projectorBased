using System.Collections;
using Games;
using Games.Big_Axe_Hunter.Scripts;
using Games.GameState;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class BigAxeHunterLightingRailController : MonoBehaviour
{
	[Header("Game Stuff")]
	[SerializeField]
	private BigAxeHunterStateSO bahState;

	[SerializeField]
	private BigAxeHunterEventsSO bahEvents;

	[SerializeField]
	private GameEventsSO gameEvents;

	[SerializeField]
	private GameStateSO gameState;

	private int _currentFrame;

	private int _frameStep;

	private Vector3 BuildStandingPoint(Vector3 position)
	{
		return new Vector3(position.x, base.transform.position.y, position.z);
	}

	private void HandleNewRound()
	{
		_currentFrame += _frameStep;
		if (_currentFrame < bahState.StandingPoints.Length)
		{
			StartCoroutine(MoveCamera(bahState.StandingPoints[_currentFrame], bahState.FocalPoints[_currentFrame]));
		}
	}

	private void HandleUndo()
	{
		if (_currentFrame != gameState.CurrentFrame)
		{
			_currentFrame -= _frameStep;
			base.transform.position = BuildStandingPoint(bahState.StandingPoints[_currentFrame]);
			base.transform.rotation = Quaternion.LookRotation(bahState.FocalPoints[_currentFrame] - base.transform.position);
			bahEvents.RaiseCameraMovedTo((ViewPosition)_currentFrame);
		}
	}

	private IEnumerator MoveCamera(Vector3 standingLocation, Vector3 focusLocation)
	{
		Vector3 standingVector = BuildStandingPoint(standingLocation);
		float timeElapsed = 0f;
		while (timeElapsed < bahState.AnimationDuration)
		{
			float time = timeElapsed / bahState.AnimationDuration;
			float t = bahState.EasingCurve.Evaluate(time);
			base.transform.position = Vector3.Lerp(base.transform.position, standingVector, t);
			Quaternion b = Quaternion.LookRotation(focusLocation - base.transform.position);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, t);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
	}

	private void OnDisable()
	{
		gameEvents.OnNewRound -= HandleNewRound;
		bahEvents.OnUndo -= HandleUndo;
	}

	private void OnEnable()
	{
		gameEvents.OnNewRound += HandleNewRound;
		bahEvents.OnUndo += HandleUndo;
		_frameStep = ((gameState.NumberOfRounds != NumberOfRounds.FiveFrames) ? 1 : 2);
	}

	private void Start()
	{
		base.transform.position = BuildStandingPoint(bahState.StandingPoints[bahState.StartingFrameIndex]);
		base.transform.rotation = Quaternion.LookRotation(bahState.FocalPoints[bahState.StartingFrameIndex] - base.transform.position);
		_currentFrame = bahState.StartingFrameIndex;
	}
}
