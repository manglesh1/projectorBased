using System.Collections;
using UnityEngine;

namespace MText
{
	[CreateAssetMenu(menuName = "Modular 3d Text/Modules/Add Rigidbody")]
	public class MText_Module_AddRigidBody : MText_Module
	{
		[SerializeField]
		private bool enableGravity;

		[SerializeField]
		private bool addRandomForce;

		[SerializeField]
		private float horizontalForcePower = 1f;

		[SerializeField]
		private float verticalForcePower = 1f;

		[SerializeField]
		private Vector3 forceDirectionMinimum = Vector3.zero;

		[SerializeField]
		private Vector3 forceDirectionMaximum = new Vector3(360f, 360f, 360f);

		[SerializeField]
		private PhysicMaterial physicMaterial;

		public override IEnumerator ModuleRoutine(GameObject obj, float delay)
		{
			if (!obj.GetComponent<MeshFilter>())
			{
				yield break;
			}
			if ((bool)obj.GetComponent<Rigidbody>())
			{
				obj.GetComponent<Rigidbody>().isKinematic = true;
			}
			yield return new WaitForSeconds(delay);
			if ((bool)obj)
			{
				if (!obj.GetComponent<BoxCollider>())
				{
					obj.AddComponent<BoxCollider>();
				}
				if ((bool)physicMaterial)
				{
					obj.GetComponent<BoxCollider>().material = physicMaterial;
				}
				if (!obj.GetComponent<Rigidbody>())
				{
					obj.AddComponent<Rigidbody>();
				}
				obj.GetComponent<Rigidbody>().isKinematic = false;
				obj.GetComponent<Rigidbody>().useGravity = enableGravity;
				if (addRandomForce)
				{
					obj.GetComponent<Rigidbody>().AddForce(new Vector3(horizontalForcePower * Random.Range(forceDirectionMinimum.x, forceDirectionMaximum.x), verticalForcePower * Random.Range(forceDirectionMinimum.y, forceDirectionMaximum.y), horizontalForcePower * Random.Range(forceDirectionMinimum.z, forceDirectionMaximum.z)));
				}
			}
		}
	}
}
