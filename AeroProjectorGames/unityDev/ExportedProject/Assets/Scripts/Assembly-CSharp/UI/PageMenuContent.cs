using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[Serializable]
	public class PageMenuContent
	{
		public Button MenuButton { get; set; }

		public Button CloseButton { get; set; }

		public List<GameObject> MenuContent { get; set; } = new List<GameObject>();
	}
}
