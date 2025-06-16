using UnityEngine;

namespace MText
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	public class MText_TextUpdater : MonoBehaviour
	{
		private bool inPrefabStageOpened;

		private Modular3DText Text => base.gameObject.GetComponent<Modular3DText>();

		[ExecuteAlways]
		private void Awake()
		{
			if ((bool)Text && EmptyText(Text))
			{
				Text.UpdateText();
			}
		}

		private bool EmptyText(Modular3DText text)
		{
			if (string.IsNullOrEmpty(text.Text))
			{
				return false;
			}
			if (text.characterObjectList.Count > 0)
			{
				for (int i = 0; i < text.characterObjectList.Count; i++)
				{
					if ((bool)text.characterObjectList[i])
					{
						return false;
					}
				}
			}
			return !base.gameObject.GetComponent<MeshFilter>() || !(base.gameObject.GetComponent<MeshFilter>().sharedMesh != null);
		}
	}
}
