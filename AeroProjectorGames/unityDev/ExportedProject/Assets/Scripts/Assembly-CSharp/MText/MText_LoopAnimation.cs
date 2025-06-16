using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MText
{
	[RequireComponent(typeof(Modular3DText))]
	public class MText_LoopAnimation : MonoBehaviour
	{
		private enum TargetType
		{
			letters = 0,
			words = 1
		}

		[SerializeField]
		private Vector2 duration = new Vector2(1f, 2f);

		[SerializeField]
		private TargetType targetType;

		[Space]
		[SerializeField]
		private Material focusedMaterial;

		[HideInInspector]
		public List<GameObject> targetLetterList = new List<GameObject>();

		public List<List<GameObject>> targetWordsList = new List<List<GameObject>>();

		private Modular3DText Modular3DText => GetComponent<Modular3DText>();

		private void OnEnable()
		{
			UpdateTargetList();
			if (targetType == TargetType.letters)
			{
				StartCoroutine(LetterAnimationRoutine());
			}
			else
			{
				StartCoroutine(WordAnimationRoutine());
			}
		}

		public void UpdateTargetList()
		{
			targetLetterList.Clear();
			targetWordsList.Clear();
			if (targetType == TargetType.letters)
			{
				for (int i = 0; i < Modular3DText.characterObjectList.Count; i++)
				{
					targetLetterList.Add(Modular3DText.characterObjectList[i]);
				}
				return;
			}
			List<GameObject> list = new List<GameObject>();
			for (int j = 0; j < Modular3DText.characterObjectList.Count; j++)
			{
				if (Modular3DText.characterObjectList[j].name == "space")
				{
					targetWordsList.Add(list);
					list = new List<GameObject>();
				}
				else
				{
					list.Add(Modular3DText.characterObjectList[j]);
				}
			}
			if (list.Count > 0)
			{
				targetWordsList.Add(list);
			}
		}

		private IEnumerator LetterAnimationRoutine()
		{
			yield return null;
			for (int i = 0; i < targetLetterList.Count; i++)
			{
				Focus(targetLetterList[i]);
				yield return new WaitForSeconds(Random.Range(duration.x, duration.y));
				UnFocus(targetLetterList[i]);
			}
			StartCoroutine(LetterAnimationRoutine());
		}

		private IEnumerator WordAnimationRoutine()
		{
			yield return null;
			for (int i = 0; i < targetWordsList.Count; i++)
			{
				for (int j = 0; j < targetWordsList[i].Count; j++)
				{
					Focus(targetWordsList[i][j]);
				}
				yield return new WaitForSeconds(Random.Range(duration.x, duration.y));
				for (int k = 0; k < targetWordsList[i].Count; k++)
				{
					UnFocus(targetWordsList[i][k]);
				}
			}
			StartCoroutine(WordAnimationRoutine());
		}

		private void Focus(GameObject target)
		{
			if ((bool)target && (bool)focusedMaterial && (bool)target.GetComponent<Renderer>())
			{
				target.GetComponent<Renderer>().material = focusedMaterial;
			}
		}

		private void UnFocus(GameObject target)
		{
			if ((bool)target && (bool)target.GetComponent<Renderer>())
			{
				target.GetComponent<Renderer>().material = Modular3DText.Material;
			}
		}
	}
}
