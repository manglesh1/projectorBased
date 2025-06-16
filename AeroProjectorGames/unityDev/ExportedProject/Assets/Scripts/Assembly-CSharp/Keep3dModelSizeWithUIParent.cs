using System.Collections;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class Keep3dModelSizeWithUIParent : MonoBehaviour
{
	private RectTransform _rectTransform;

	private float _x_CalculatedScale;

	private float _x_Scale;

	private float _x_ScaleDifference;

	private float _y_CalculatedScale;

	private float _y_ScaleDifference;

	private float _y_Scale;

	private float _z_CalculatedScale;

	private float _z_Scale;

	private float _z_ScaleDifference;

	[Tooltip("Connect to the UI Element to resize the object this script is attached")]
	[SerializeField]
	private RectTransform transformToControlResizing;

	[Header("Scale Options")]
	[Space(10f)]
	[Tooltip("Used to keep the aspect ratio when resizing")]
	[SerializeField]
	private bool keepAspectRatio;

	[Header("Padding")]
	[Space(10f)]
	[Tooltip("Add or Subtract size during scaling in the X direction")]
	[SerializeField]
	private float x_Padding;

	[Tooltip("Add or Subtract size during scaling in the Y direction")]
	[SerializeField]
	private float y_Padding;

	[Tooltip("Add or Subtract size during scaling in the Z direction")]
	[SerializeField]
	private float z_Padding;

	[Header("Timed Options")]
	[Space(10f)]
	[Tooltip("Will disable this script after the selected time. Time Starts when script runs")]
	[SerializeField]
	private bool disableWithTimer;

	[Min(0f)]
	[Tooltip("This is the time in seconds this script will disable itself.")]
	[SerializeField]
	private float disableTimeInSeconds;

	public bool KeepAspectRatio
	{
		get
		{
			return keepAspectRatio;
		}
		set
		{
			keepAspectRatio = value;
		}
	}

	public RectTransform TransformToControlResizing
	{
		get
		{
			return transformToControlResizing;
		}
		set
		{
			transformToControlResizing = value;
		}
	}

	private void OnEnable()
	{
		base.gameObject.TryGetComponent<RectTransform>(out _rectTransform);
	}

	private void OnRenderObject()
	{
		ResizeAttachedObjectToGivenTransform();
		CodeThatWontRunInEditor();
	}

	private void AddPadding()
	{
		_x_CalculatedScale -= x_Padding;
		_y_CalculatedScale -= y_Padding;
		_z_CalculatedScale -= z_Padding;
	}

	private void AspectRatioCalculation()
	{
		if (keepAspectRatio)
		{
			_y_ScaleDifference = _rectTransform.localScale.y - _y_Scale;
			_y_ScaleDifference = ((_y_ScaleDifference >= 0f) ? _y_ScaleDifference : (_y_ScaleDifference * -1f));
			_x_CalculatedScale = _rectTransform.localScale.x - _y_ScaleDifference;
		}
		else
		{
			_x_CalculatedScale = _x_Scale;
			_y_CalculatedScale = _y_Scale;
			_z_CalculatedScale = _z_Scale;
		}
	}

	public void UpdateParent(RectTransform newParent)
	{
		_rectTransform = newParent;
	}

	private void CodeThatWontRunInEditor()
	{
		if (Application.IsPlaying(this) && disableWithTimer)
		{
			StartCoroutine(TurnSelfOff());
		}
	}

	private void PreventNegativeScale()
	{
		_x_CalculatedScale = ((_x_CalculatedScale >= 0f) ? _x_CalculatedScale : 0f);
		_y_CalculatedScale = ((_y_CalculatedScale >= 0f) ? _y_CalculatedScale : 0f);
		_z_CalculatedScale = ((_z_CalculatedScale >= 0f) ? _z_CalculatedScale : 0f);
	}

	private void ResizeAttachedObjectToGivenTransform()
	{
		if ((bool)transformToControlResizing)
		{
			_x_Scale = transformToControlResizing.rect.size.x;
			_y_Scale = transformToControlResizing.rect.size.y;
			_z_Scale = _rectTransform.localScale.z;
			AspectRatioCalculation();
			AddPadding();
			PreventNegativeScale();
			_rectTransform.localScale = new Vector3(_x_CalculatedScale, _y_CalculatedScale, _z_CalculatedScale);
		}
	}

	private IEnumerator TurnSelfOff()
	{
		yield return new WaitForSecondsRealtime(0.25f);
		base.enabled = false;
	}
}
