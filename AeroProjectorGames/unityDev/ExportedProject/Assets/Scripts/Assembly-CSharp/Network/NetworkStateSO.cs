using UnityEngine;

namespace Network
{
	[CreateAssetMenu(menuName = "Network/Network State")]
	public class NetworkStateSO : ScriptableObject
	{
		public NetworkStatus Status { get; set; }
	}
}
