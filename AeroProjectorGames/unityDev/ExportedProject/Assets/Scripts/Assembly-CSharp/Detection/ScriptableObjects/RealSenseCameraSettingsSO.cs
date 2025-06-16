using System;
using System.Collections.Generic;
using System.Linq;
using Detection.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Detection.ScriptableObjects
{
	[CreateAssetMenu(fileName = "Data", menuName = "Detection/ScriptableObjects/RealSenseCameraSettingsSO", order = 1)]
	public class RealSenseCameraSettingsSO : ScriptableObject
	{
		[SerializeField]
		private bool emitterEnabled;

		[SerializeField]
		private float exposure;

		[SerializeField]
		private float gain;

		[SerializeField]
		private float laserPower;

		[SerializeField]
		private float maxDistance;

		[SerializeField]
		private float minDistance;

		[SerializeField]
		private List<ProcessingProfile> _processingProfiles;

		[SerializeField]
		private ProcessingProfileEnum _selectedProcessingProfile;

		public bool EmitterEnabled
		{
			get
			{
				return emitterEnabled;
			}
			set
			{
				emitterEnabled = value;
				HandleChange(this.OnEmitterEnabledChanged);
			}
		}

		public float Exposure
		{
			get
			{
				return exposure;
			}
			set
			{
				exposure = value;
				HandleChange(this.OnExposureChanged);
			}
		}

		public float Gain
		{
			get
			{
				return gain;
			}
			set
			{
				gain = value;
				HandleChange(this.OnGainChanged);
			}
		}

		public float LaserPower
		{
			get
			{
				return laserPower;
			}
			set
			{
				laserPower = value;
				HandleChange(this.OnLaserPowerChanged);
			}
		}

		public float MaxDistance
		{
			get
			{
				return maxDistance;
			}
			set
			{
				maxDistance = value;
				HandleChange(this.OnMaxDistanceChanged);
			}
		}

		public float MinDistance
		{
			get
			{
				return minDistance;
			}
			set
			{
				minDistance = value;
				HandleChange(this.OnMinDistanceChanged);
			}
		}

		public RsProcessingProfile SelectedProcessingProfile => _processingProfiles.First((ProcessingProfile p) => p.Profile == _selectedProcessingProfile).RealSenseProfile;

		public ProcessingProfileEnum SelectedProcessingProfileEnum => _selectedProcessingProfile;

		public event UnityAction OnChanged;

		public event UnityAction OnEmitterEnabledChanged;

		public event UnityAction OnExposureChanged;

		public event UnityAction OnGainChanged;

		public event UnityAction OnLaserPowerChanged;

		public event UnityAction OnMaxDistanceChanged;

		public event UnityAction OnMinDistanceChanged;

		public event UnityAction OnProcessingProfileChanged;

		private void OnEnable()
		{
			if (_processingProfiles.Count < 1)
			{
				throw new ArgumentException("Must have at least one processing profile set in the Camera Settings SO");
			}
			if (!_processingProfiles.Exists((ProcessingProfile p) => p.Profile == _selectedProcessingProfile))
			{
				_selectedProcessingProfile = _processingProfiles.First().Profile;
			}
		}

		public void SetProcessingProfile(ProcessingProfileEnum profile)
		{
			_selectedProcessingProfile = profile;
			HandleChange(this.OnProcessingProfileChanged);
		}

		private void HandleChange(UnityAction optionalEvent = null)
		{
			optionalEvent?.Invoke();
			this.OnChanged?.Invoke();
		}
	}
}
