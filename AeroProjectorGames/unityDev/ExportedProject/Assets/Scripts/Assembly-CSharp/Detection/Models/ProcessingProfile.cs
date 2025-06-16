using System;
using UnityEngine;

namespace Detection.Models
{
	[Serializable]
	public class ProcessingProfile
	{
		[SerializeField]
		private ProcessingProfileEnum _profile;

		[SerializeField]
		private RsProcessingProfile _realSenseProfile;

		public ProcessingProfileEnum Profile => _profile;

		public RsProcessingProfile RealSenseProfile => _realSenseProfile;
	}
}
