using System.Collections;
using Games;
using Games.Big_Axe_Hunter.Scripts;
using Games.GameState;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BigAxeHunterCameraRailController : MonoBehaviour
{
	[Header("State & Events")]
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

	private void HandleNewRound()
	{
		bahEvents.RaiseCameraMovingAwayFrom((ViewPosition)_currentFrame);
		SetCurrentFrame(_currentFrame + _frameStep);
		if (_currentFrame < bahState.StandingPoints.Length)
		{
			StartCoroutine(MoveCamera(bahState.StandingPoints[_currentFrame], bahState.FocalPoints[_currentFrame]));
		}
	}

	private void HandleUndo()
	{
		if (_currentFrame != gameState.CurrentFrame)
		{
			SetCurrentFrame(_currentFrame - _frameStep);
			base.transform.position = bahState.StandingPoints[_currentFrame];
			base.transform.rotation = Quaternion.LookRotation(bahState.FocalPoints[_currentFrame] - base.transform.position);
			bahEvents.RaiseCameraMovedTo((ViewPosition)_currentFrame);
		}
	}

	private IEnumerator MoveCamera(Vector3 standingLocation, Vector3 focusLocation)
	{
		float timeElapsed = 0f;
		while (timeElapsed < bahState.AnimationDuration)
		{
			float time = timeElapsed / bahState.AnimationDuration;
			float t = bahState.EasingCurve.Evaluate(time);
			base.transform.position = Vector3.Lerp(base.transform.position, standingLocation, t);
			Quaternion b = Quaternion.LookRotation(focusLocation - base.transform.position);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, t);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		bahEvents.RaiseCameraMovedTo((ViewPosition)_currentFrame);
	}

	private void SetCurrentFrame(int frameIndex)
	{
		_currentFrame = frameIndex;
		bahState.CurrentViewPosition = (ViewPosition)frameIndex;
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
		base.transform.position = bahState.StandingPoints[bahState.StartingFrameIndex];
		base.transform.rotation = Quaternion.LookRotation(bahState.FocalPoints[bahState.StartingFrameIndex] - base.transform.position);
		SetCurrentFrame(bahState.StartingFrameIndex);
		bahEvents.RaiseCameraMovedTo((ViewPosition)_currentFrame);
	}
}
