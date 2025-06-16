using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MText
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Modular 3D Text/Horizontal Selector")]
	public class MText_UI_HorizontalSelector : MonoBehaviour
	{
		[Tooltip("The 3D text used to show the current value")]
		public Modular3DText text;

		[Tooltip("If keyboard control is enabled, selected = you can control via selected. \nThis value will be controlled by list, if it is in one")]
		public bool selected;

		[Tooltip("If keyboard control is enabled, selected = you can control via selected\nOr selected/deselected in a List")]
		public bool interactable = true;

		[Tooltip("Available options for horizontal selector")]
		public List<string> options = new List<string>(new string[3] { "Option 1", "Option 2", "Option 3" });

		[SerializeField]
		private int value;

		public bool keyboardControl = true;

		public KeyCode increaseKey = KeyCode.LeftArrow;

		public KeyCode decreaseKey = KeyCode.RightArrow;

		public AudioClip valueChangeSoundEffect;

		public AudioSource audioSource;

		public UnityEvent onSelectEvent;

		public UnityEvent onValueChangedEvent;

		public UnityEvent onValueIncreasedEvent;

		public UnityEvent onValueDecreasedEvent;

		public int Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
				UpdateText();
				onValueChangedEvent.Invoke();
			}
		}

		private void Awake()
		{
			if (selected && keyboardControl)
			{
				base.enabled = true;
			}
			else
			{
				base.enabled = false;
			}
		}

		private void Update()
		{
			if (!keyboardControl)
			{
				base.enabled = false;
			}
			else if (Input.GetKeyDown(increaseKey))
			{
				Decrease();
			}
			else if (Input.GetKeyDown(decreaseKey))
			{
				Increase();
			}
		}

		public void Increase()
		{
			value++;
			if (value >= options.Count)
			{
				value = 0;
			}
			UpdateText();
			onValueChangedEvent.Invoke();
			onValueIncreasedEvent.Invoke();
			if ((bool)audioSource && (bool)valueChangeSoundEffect)
			{
				audioSource.PlayOneShot(valueChangeSoundEffect);
			}
		}

		public void Decrease()
		{
			value--;
			if (value < 0)
			{
				value = options.Count - 1;
			}
			UpdateText();
			onValueChangedEvent.Invoke();
			onValueDecreasedEvent.Invoke();
			if ((bool)audioSource && (bool)valueChangeSoundEffect)
			{
				audioSource.PlayOneShot(valueChangeSoundEffect);
			}
		}

		public void UpdateText()
		{
			if (options.Count != 0 && value >= 0 && options.Count > value)
			{
				if ((bool)text)
				{
					text.UpdateText(options[value]);
				}
				else
				{
					Debug.LogError("No text is attached to Horizontal selector: " + base.gameObject.name, base.gameObject);
				}
			}
		}

		public void Focus(bool Bool)
		{
			selected = Bool;
			if (selected && interactable)
			{
				onSelectEvent.Invoke();
				if (keyboardControl)
				{
					base.enabled = true;
				}
			}
			else
			{
				base.enabled = false;
			}
		}
	}
}
