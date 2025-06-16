using System;
using UnityEngine;

namespace MText
{
	[Serializable]
	public class MText_Character
	{
		public char character;

		public GameObject prefab;

		public Mesh meshPrefab;

		public float spacing = 1f;

		public int glyphIndex;
	}
}
