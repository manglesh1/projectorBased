using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MText
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Modular 3D Text/Input field")]
	public class Mtext_UI_InputField : MonoBehaviour
	{
		[Tooltip("if not in a list")]
		public bool autoFocusOnGameStart = true;

		public bool interactable = true;

		[SerializeField]
		private int maxCharacter = 20;

		[SerializeField]
		private string typingSymbol = "|";

		[SerializeField]
		private string _text = string.Empty;

		public string placeHolderText = "Enter Text...";

		public Modular3DText textComponent;

		public Renderer background;

		public bool enterKeyEndsInput = true;

		public Material placeHolderTextMat;

		public Material inFocusTextMat;

		public Material inFocusBackgroundMat;

		public Material outOfFocusTextMat;

		public Material outOfFocusBackgroundMat;

		public Material disabledTextMat;

		public Material disabledBackgroundMat;

		private Material currentTextMaterial;

		[SerializeField]
		private AudioClip typeSound;

		[SerializeField]
		private AudioSource audioSource;

		public UnityEvent onInput;

		public UnityEvent onBackspace;

		public UnityEvent onInputEnd;

		public string test;

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				UpdateText(sound: true);
			}
		}

		private void Awake()
		{
			if (!MText_Utilities.GetParentList(base.transform))
			{
				Focus(autoFocusOnGameStart);
			}
		}

		private void Update()
		{
			string inputString = Input.inputString;
			for (int i = 0; i < inputString.Length; i++)
			{
				char c = inputString[i];
				switch (c)
				{
				case '\b':
					if (_text.Length != 0)
					{
						_text = _text.Substring(0, _text.Length - 1);
						UpdateText(sound: true);
						onBackspace.Invoke();
					}
					continue;
				case '\n':
				case '\r':
					if (enterKeyEndsInput)
					{
						InputComplete();
						continue;
					}
					break;
				}
				if (_text.Length < maxCharacter)
				{
					_text += c;
					UpdateText(sound: true);
					onInput.Invoke();
				}
			}
		}

		public void InputComplete()
		{
			onInputEnd.Invoke();
			base.enabled = false;
		}

		public void UpdateText()
		{
			UpdateText(sound: false);
		}

		public void UpdateText(string newText)
		{
			_text = newText;
			UpdateText(sound: false);
		}

		public void UpdateText(int newTextInt)
		{
			_text = newTextInt.ToString();
			UpdateText(sound: false);
		}

		public void UpdateText(float newTextFloat)
		{
			_text = newTextFloat.ToString();
			UpdateText(sound: false);
		}

		public void UpdateText(bool sound)
		{
			if ((bool)textComponent)
			{
				TouchScreenKeyboard.Open(_text);
				if (!string.IsNullOrEmpty(_text))
				{
					textComponent.Material = currentTextMaterial;
					textComponent.Text = _text + typingSymbol;
				}
				else
				{
					textComponent.Material = placeHolderTextMat;
					textComponent.Text = placeHolderText;
				}
				if ((bool)typeSound && sound && (bool)audioSource)
				{
					audioSource.pitch = Random.Range(0.9f, 1.1f);
					audioSource.PlayOneShot(typeSound);
				}
			}
		}

		public void EmptyText()
		{
			_text = string.Empty;
			UpdateText(sound: false);
		}

		public void Select()
		{
			Focus(enable: true);
			Transform parent = base.transform.parent;
			if (!(parent == null))
			{
				MText_UI_List component = parent.GetComponent<MText_UI_List>();
				if (!(component == null))
				{
					component.SelectItem(base.transform.GetSiblingIndex());
				}
			}
		}

		public void Focus(bool enable)
		{
			StartCoroutine(FocusRoutine(enable));
		}

		private IEnumerator FocusRoutine(bool enable)
		{
			yield return null;
			FocusFunction(enable);
		}

		public void Focus(bool enable, bool delay)
		{
			if (!delay)
			{
				FocusFunction(enable);
			}
			else
			{
				FocusRoutine(enable);
			}
		}

		private void FocusFunction(bool enable)
		{
			if (interactable)
			{
				base.enabled = enable;
				if (enable)
				{
					SelectedVisual();
				}
				else
				{
					UnselectedVisual();
				}
			}
			else
			{
				DisableVisual();
			}
			UpdateText(sound: false);
		}

		public void Interactable()
		{
			Focus(enable: false, delay: false);
		}

		public void Uninteractable()
		{
			base.enabled = false;
			DisableVisual();
			UpdateText(sound: false);
		}

		private void SelectedVisual()
		{
			(bool, MText_UI_List) tuple = ApplySelectedStyleFromParent();
			if (tuple.Item1)
			{
				UpdateMaterials(tuple.Item2.selectedItemFontMaterial, tuple.Item2.selectedItemBackgroundMaterial);
			}
			else
			{
				UpdateMaterials(inFocusTextMat, inFocusBackgroundMat);
			}
		}

		private void UnselectedVisual()
		{
			(bool, MText_UI_List) tuple = ApplyNormalStyleFromParent();
			if (tuple.Item1)
			{
				Material textMat = (string.IsNullOrEmpty(_text) ? placeHolderTextMat : tuple.Item2.normalItemFontMaterial);
				UpdateMaterials(textMat, tuple.Item2.normalItemBackgroundMaterial);
			}
			else
			{
				Material textMat2 = (string.IsNullOrEmpty(_text) ? placeHolderTextMat : outOfFocusTextMat);
				UpdateMaterials(textMat2, outOfFocusBackgroundMat);
			}
		}

		public void DisableVisual()
		{
			(bool, MText_UI_List) tuple = ApplyDisabledStyleFromParent();
			if (tuple.Item1)
			{
				Material textMat = (string.IsNullOrEmpty(_text) ? placeHolderTextMat : tuple.Item2.disabledItemFontMaterial);
				UpdateMaterials(textMat, tuple.Item2.disabledItemBackgroundMaterial);
			}
			else
			{
				Material textMat2 = (string.IsNullOrEmpty(_text) ? placeHolderTextMat : disabledTextMat);
				UpdateMaterials(textMat2, disabledBackgroundMat);
			}
		}

		private void UpdateMaterials(Material textMat, Material backgroundMat)
		{
			if ((bool)textComponent)
			{
				textComponent.Material = textMat;
			}
			if ((bool)background)
			{
				background.material = backgroundMat;
			}
			currentTextMaterial = textMat;
		}

		private MText_UI_List GetParentList()
		{
			return MText_Utilities.GetParentList(base.transform);
		}

		public (bool, MText_UI_List) ApplyNormalStyleFromParent()
		{
			MText_UI_List parentList = GetParentList();
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customNormalItemVisual)
			{
				return (true, parentList);
			}
			return (false, parentList);
		}

		public (bool, MText_UI_List) ApplySelectedStyleFromParent()
		{
			MText_UI_List parentList = GetParentList();
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customSelectedItemVisual)
			{
				return (true, parentList);
			}
			return (false, parentList);
		}

		public (bool, MText_UI_List) ApplyDisabledStyleFromParent()
		{
			MText_UI_List parentList = GetParentList();
			if ((bool)parentList && parentList.controlChildVisuals && parentList.customDisabledItemVisual)
			{
				return (true, parentList);
			}
			return (false, parentList);
		}
	}
}
