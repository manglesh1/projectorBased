using Assets.Scripts.Detection.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Detection.Controllers
{
	public class RealSenseWizardCameraPowerController : MonoBehaviour
	{
		[Header("Events")]
		[SerializeField]
		private RealSenseWizardEventsSO events;

		private void OnEnable()
		{
			events.RaiseOnCameraPoweredOn();
		}
	}
}
