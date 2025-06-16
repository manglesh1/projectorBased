using UnityEngine;

namespace Admin.Demo
{
	[CreateAssetMenu(menuName = "Admin/Demo")]
	public class DemoSO : ScriptableObject
	{
		[SerializeField]
		private bool demoIsRunning;

		[SerializeField]
		private bool useDemo;

		[SerializeField]
		private int waitBeforeDemoTimeInMinutes = 30;

		public bool DemoIsRunning
		{
			get
			{
				return demoIsRunning;
			}
			set
			{
				demoIsRunning = value;
			}
		}

		public bool UseDemo
		{
			get
			{
				return useDemo;
			}
			set
			{
				useDemo = value;
			}
		}

		public int WaitBeforeDemoTimeInMinutes
		{
			get
			{
				return waitBeforeDemoTimeInMinutes;
			}
			set
			{
				waitBeforeDemoTimeInMinutes = value;
			}
		}
	}
}
