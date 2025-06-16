using System.Collections;
using UnityEngine;

namespace MText
{
	[CreateAssetMenu(menuName = "Modular 3d Text/Modules/Remove Rigidbody")]
	public class MText_Module_RemoveRigidBody : MText_Module
	{
		public override IEnumerator ModuleRoutine(GameObject obj, float delay)
		{
			if ((bool)obj)
			{
				if ((bool)obj.GetComponent<BoxCollider>())
				{
					Object.Destroy(obj.GetComponent<Rigidbody>());
				}
				if ((bool)obj.GetComponent<Rigidbody>())
				{
					Object.Destroy(obj.GetComponent<Rigidbody>());
				}
			}
			yield return null;
		}
	}
}
