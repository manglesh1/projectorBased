using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[Serializable]
	public class TabButton
	{
		public Button ExistingButton { get; set; }

		public int Index { get; set; }

		public string Name { get; set; }

		public List<GameObject> TabContents { get; set; } = new List<GameObject>();

		public bool WindowsOnly { get; set; }
	}
}
