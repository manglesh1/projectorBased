using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Detection.Models;
using Detection.ScriptableObjects;
using Intel.RealSense;
using Settings;
using UnityEngine;

namespace Detection.Cameras
{
	public class RealSenseEdgeDevice : RsFrameProvider
	{
		private const int DEFAULT_GAIN = 16;

		[SerializeField]
		private RealSenseCameraSettingsSO settings;

		private bool _initialized;

		private Thread _worker;

		private readonly AutoResetEvent _stopEvent;

		private Pipeline _pipeline;

		private Sensor _sensor;

		public RsConfiguration DeviceConfiguration = new RsConfiguration
		{
			mode = RsConfiguration.Mode.Live,
			RequestedSerialNumber = string.Empty,
			Profiles = new RsVideoStreamRequest[1]
			{
				new RsVideoStreamRequest
				{
					Stream = Stream.Depth,
					StreamIndex = 0,
					Width = 1280,
					Height = 720,
					Format = Format.Z16,
					Framerate = 30
				}
			}
		};

		[Tooltip("Threading mode of operation, Multithreads or Unitythread")]
		public ProcessModeEnum processMode;

		public override event Action<Frame> OnNewSample;

		public override event Action<PipelineProfile> OnStart;

		public override event Action OnStop;

		public RealSenseEdgeDevice()
		{
			_stopEvent = new AutoResetEvent(initialState: false);
		}

		private void HandleEmitterEnabledChanged()
		{
			if (_sensor.Options.Supports(Option.EmitterEnabled))
			{
				_sensor.Options[Option.EmitterEnabled].Value = GetBooleanFloat(settings.EmitterEnabled);
			}
		}

		private void HandleExposureChanged()
		{
			if (_sensor.Options.Supports(Option.Exposure))
			{
				_sensor.Options[Option.Exposure].Value = settings.Exposure;
			}
		}

		private void HandleGainChanged()
		{
			if (_sensor.Options.Supports(Option.Gain))
			{
				_sensor.Options[Option.Gain].Value = 16f;
			}
		}

		private void HandleLaserPowerChanged()
		{
			if (_sensor.Options.Supports(Option.LaserPower))
			{
				_sensor.Options[Option.LaserPower].Value = settings.LaserPower;
			}
		}

		private void OnDestroy()
		{
			OnStop = null;
			if (base.ActiveProfile != null)
			{
				base.ActiveProfile.Dispose();
				base.ActiveProfile = null;
			}
			if (_pipeline != null)
			{
				_pipeline.Dispose();
				_pipeline = null;
			}
		}

		private void OnDisable()
		{
			DisableSettingEvents();
			OnNewSample = null;
			if (_worker != null)
			{
				_stopEvent.Set();
				_worker.Join();
			}
			if (base.Streaming && OnStop != null)
			{
				OnStop();
			}
			if (base.ActiveProfile != null)
			{
				base.ActiveProfile.Dispose();
				base.ActiveProfile = null;
			}
			if (_pipeline != null)
			{
				_pipeline.Dispose();
				_pipeline = null;
			}
			base.Streaming = false;
		}

		private void OnEnable()
		{
			if (SettingsStore.DetectionSettings.DetectedCamera != DetectedCameraEnum.RealSense || !SettingsStore.DetectionSettings.DetectionEnabled)
			{
				base.enabled = false;
				return;
			}
			_pipeline = new Pipeline();
			using Config cfg = DeviceConfiguration.ToPipelineConfig();
			base.ActiveProfile = _pipeline.Start(cfg);
			AdvancedDevice.FromDevice(base.ActiveProfile.Device).JsonConfiguration = "{\"aux-param-autoexposure-setpoint\":\"1536\",\"aux-param-colorcorrection1\":\"0.298828\",\"aux-param-colorcorrection10\":\"0\",\"aux-param-colorcorrection11\":\"0\",\"aux-param-colorcorrection12\":\"0\",\"aux-param-colorcorrection2\":\"0.293945\",\"aux-param-colorcorrection3\":\"0.293945\",\"aux-param-colorcorrection4\":\"0.114258\",\"aux-param-colorcorrection5\":\"0\",\"aux-param-colorcorrection6\":\"0\",\"aux-param-colorcorrection7\":\"0\",\"aux-param-colorcorrection8\":\"0\",\"aux-param-colorcorrection9\":\"0\",\"aux-param-depthclampmax\":\"65536\",\"aux-param-depthclampmin\":\"0\",\"aux-param-disparityshift\":\"0\",\"controls-autoexposure-auto\":\"True\",\"controls-autoexposure-manual\":\"8500\",\"controls-color-autoexposure-auto\":\"True\",\"controls-color-autoexposure-manual\":\"156\",\"controls-color-backlight-compensation\":\"0\",\"controls-color-brightness\":\"0\",\"controls-color-contrast\":\"50\",\"controls-color-gain\":\"64\",\"controls-color-gamma\":\"300\",\"controls-color-hue\":\"0\",\"controls-color-power-line-frequency\":\"3\",\"controls-color-saturation\":\"64\",\"controls-color-sharpness\":\"50\",\"controls-color-white-balance-auto\":\"True\",\"controls-color-white-balance-manual\":\"4600\",\"controls-depth-gain\":\"72\",\"controls-laserpower\":\"150\",\"controls-laserstate\":\"off\",\"ignoreSAD\":\"0\",\"param-autoexposure-setpoint\":\"1536\",\"param-censusenablereg-udiameter\":\"9\",\"param-censusenablereg-vdiameter\":\"9\",\"param-censususize\":\"9\",\"param-censusvsize\":\"9\",\"param-depthclampmax\":\"65536\",\"param-depthclampmin\":\"0\",\"param-depthunits\":\"1000\",\"param-disableraucolor\":\"0\",\"param-disablesadcolor\":\"0\",\"param-disablesadnormalize\":\"0\",\"param-disablesloleftcolor\":\"0\",\"param-disableslorightcolor\":\"0\",\"param-disparitymode\":\"0\",\"param-disparityshift\":\"0\",\"param-lambdaad\":\"800\",\"param-lambdacensus\":\"26\",\"param-leftrightthreshold\":\"24\",\"param-maxscorethreshb\":\"2047\",\"param-medianthreshold\":\"500\",\"param-minscorethresha\":\"1\",\"param-neighborthresh\":\"7\",\"param-raumine\":\"1\",\"param-rauminn\":\"1\",\"param-rauminnssum\":\"3\",\"param-raumins\":\"1\",\"param-rauminw\":\"1\",\"param-rauminwesum\":\"3\",\"param-regioncolorthresholdb\":\"0.0499022\",\"param-regioncolorthresholdg\":\"0.0499022\",\"param-regioncolorthresholdr\":\"0.0499022\",\"param-regionshrinku\":\"3\",\"param-regionshrinkv\":\"1\",\"param-robbinsmonrodecrement\":\"10\",\"param-robbinsmonroincrement\":\"10\",\"param-rsmdiffthreshold\":\"4\",\"param-rsmrauslodiffthreshold\":\"1\",\"param-rsmremovethreshold\":\"0.375\",\"param-scanlineedgetaub\":\"72\",\"param-scanlineedgetaug\":\"72\",\"param-scanlineedgetaur\":\"72\",\"param-scanlinep1\":\"60\",\"param-scanlinep1onediscon\":\"105\",\"param-scanlinep1twodiscon\":\"70\",\"param-scanlinep2\":\"342\",\"param-scanlinep2onediscon\":\"190\",\"param-scanlinep2twodiscon\":\"130\",\"param-secondpeakdelta\":\"325\",\"param-texturecountthresh\":\"24\",\"param-texturedifferencethresh\":\"126\",\"param-usersm\":\"0\",\"param-zunits\":\"1000\",\"stream-depth-format\":\"Z16\",\"stream-fps\":\"30\",\"stream-height\":\"720\",\"stream-width\":\"1280\"}";
			DeviceConfiguration.Profiles = base.ActiveProfile.Streams.Select(RsVideoStreamRequest.FromProfile).ToArray();
			InitializeSensor();
			EnableSettingEvents();
			if (processMode == ProcessModeEnum.Multithread)
			{
				_stopEvent.Reset();
				_worker = new Thread(WaitForFrames);
				_worker.IsBackground = true;
				_worker.Start();
			}
			StartCoroutine(WaitAndStart());
		}

		private void Update()
		{
			if (!base.Streaming || processMode != ProcessModeEnum.UnityThread || !_pipeline.PollForFrames(out var result))
			{
				return;
			}
			using (result)
			{
				RaiseSampleEvent(result);
			}
		}

		private IEnumerator WaitAndStart()
		{
			yield return new WaitForEndOfFrame();
			base.Streaming = true;
			if (OnStart != null)
			{
				OnStart(base.ActiveProfile);
			}
		}

		private void DisableSettingEvents()
		{
			settings.OnEmitterEnabledChanged -= HandleEmitterEnabledChanged;
			settings.OnExposureChanged -= HandleExposureChanged;
			settings.OnGainChanged -= HandleGainChanged;
			settings.OnLaserPowerChanged -= HandleLaserPowerChanged;
		}

		private void EnableSettingEvents()
		{
			settings.OnEmitterEnabledChanged += HandleEmitterEnabledChanged;
			settings.OnExposureChanged += HandleExposureChanged;
			settings.OnGainChanged += HandleGainChanged;
			settings.OnLaserPowerChanged += HandleLaserPowerChanged;
		}

		private float GetBooleanFloat(bool flag)
		{
			if (!flag)
			{
				return 0f;
			}
			return 1f;
		}

		private void InitializeSensor()
		{
			Device device = base.ActiveProfile.Device;
			_sensor = device.Sensors[0];
			_sensor.Options[Option.ErrorPollingEnabled].Value = GetBooleanFloat(flag: true);
			_sensor.Options[Option.EnableAutoExposure].Value = GetBooleanFloat(flag: false);
			HandleExposureChanged();
			HandleGainChanged();
			HandleLaserPowerChanged();
			HandleEmitterEnabledChanged();
		}

		private void RaiseSampleEvent(Frame frame)
		{
			OnNewSample?.Invoke(frame);
		}

		private void WaitForFrames()
		{
			while (!_stopEvent.WaitOne(0))
			{
				try
				{
					using FrameSet frameSet = _pipeline.WaitForFrames();
					RaiseSampleEvent(frameSet);
					frameSet.Dispose();
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.Message);
				}
			}
		}
	}
}
