using System;
using Games.Big_Axe_Hunter.Scripts;
using UnityEngine;

public class BigAxeHunterInitializer : MonoBehaviour
{
	[Header("Location traversal settings")]
	[SerializeField]
	private float animationDuration;

	[SerializeField]
	private AnimationCurve easingCurve;

	[SerializeField]
	private Transform[] focalPositions;

	[SerializeField]
	private Transform[] standingPositions;

	[Header("Dev Setting")]
	[SerializeField]
	private int startingFrameIndex;

	[Header("Game Scriptable Objects")]
	[SerializeField]
	private BigAxeHunterStateSO state;

	private void InitializedStateData()
	{
		if (state == null)
		{
			return;
		}
		if (focalPositions.Length < 10)
		{
			throw new InvalidOperationException("Not enough focal positions defined");
		}
		if (standingPositions.Length < 10)
		{
			throw new InvalidOperationException("Not enough standing positions defined");
		}
		state.AnimationDuration = animationDuration;
		state.EasingCurve = easingCurve;
		state.FocalPoints = new Vector3[10];
		for (int i = 0; i < focalPositions.Length; i++)
		{
			if (focalPositions[i] == null)
			{
				throw new InvalidOperationException($"focalPosition at index {i} is null");
			}
			state.FocalPoints[i] = focalPositions[i].position;
		}
		state.StandingPoints = new Vector3[10];
		for (int j = 0; j < standingPositions.Length; j++)
		{
			if (standingPositions[j] == null)
			{
				throw new InvalidOperationException($"standingPosition at index {j} is null");
			}
			state.StandingPoints[j] = standingPositions[j].position;
		}
		state.StartingFrameIndex = startingFrameIndex;
	}

	private void OnEnable()
	{
		InitializedStateData();
	}
}
