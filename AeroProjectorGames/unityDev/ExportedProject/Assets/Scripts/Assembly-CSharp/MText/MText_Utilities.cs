using UnityEngine;

namespace MText
{
	public class MText_Utilities
	{
		public static MText_UI_List GetParentList(Transform transform)
		{
			if (transform.parent == null)
			{
				return null;
			}
			if ((bool)transform.parent.GetComponent<MText_UI_List>())
			{
				return transform.parent.GetComponent<MText_UI_List>();
			}
			return null;
		}
	}
}
