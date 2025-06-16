using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Detection.ScriptableObjects
{
	[CreateAssetMenu(fileName = "RealSenseWizardEvents", menuName = "Detection/Wizard/RealSenseWizardEvents")]
	public class RealSenseWizardEventsSO : ScriptableObject
	{
		public event UnityAction OnCameraPoweredOn;

		public event UnityAction<float> OnMaxValueChanged;

		public event UnityAction<float> OnMinValueChanged;

		public event UnityAction OnRoadBlocked;

		public event UnityAction OnRoadBlockResolved;

		public void RaiseOnCameraPoweredOn()
		{
			this.OnCameraPoweredOn?.Invoke();
		}

		public void RaiseOnMaxValueChanged(float value)
		{
			this.OnMaxValueChanged?.Invoke(value);
		}

		public void RaiseOnMinValueChanged(float value)
		{
			this.OnMinValueChanged?.Invoke(value);
		}

		public void RaiseOnRoadBlocked()
		{
			this.OnRoadBlocked?.Invoke();
		}

		public void RaiseOnRoadBlockResolved()
		{
			this.OnRoadBlockResolved?.Invoke();
		}
	}
}
