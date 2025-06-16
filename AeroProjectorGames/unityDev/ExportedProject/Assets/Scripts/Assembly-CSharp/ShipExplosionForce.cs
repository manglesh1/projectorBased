using System.Collections.Generic;
using UnityEngine;

public class ShipExplosionForce : MonoBehaviour
{
	private Vector3 startPosition = new Vector3(0f, 0f, 90f);

	[Tooltip("Transforms for objects to reset back to 0 before explosion")]
	[SerializeField]
	private List<Transform> shipPartsTransform;

	[Header("Explosion Elements")]
	[SerializeField]
	private float blastPower = 200f;

	[SerializeField]
	private float blastRadius = 150f;

	private void OnEnable()
	{
		foreach (Transform item in shipPartsTransform)
		{
			item.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			item.position = startPosition;
		}
		Vector3 position = base.transform.position;
		Collider[] array = Physics.OverlapSphere(position, blastRadius);
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody component = array[i].GetComponent<Rigidbody>();
			if (component != null)
			{
				component.AddExplosionForce(blastPower, position, blastRadius, 10f);
			}
		}
	}
}
