using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;

namespace MText
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MText_TextUpdater))]
	[AddComponentMenu("Modular 3D Text/3D Text")]
	public class Modular3DText : MonoBehaviour
	{
		[FormerlySerializedAs("text")]
		[TextArea]
		[SerializeField]
		private string _text = string.Empty;

		[SerializeField]
		private List<string> lineList = new List<string>();

		public List<string> oldLineList = new List<string>();

		public List<GameObject> characterObjectList = new List<GameObject>();

		[Tooltip("only prefabs need mesh to be saved")]
		public bool autoSaveMesh;

		[FormerlySerializedAs("font")]
		[SerializeField]
		private MText_Font _font;

		[FormerlySerializedAs("material")]
		[SerializeField]
		private Material _material;

		[FormerlySerializedAs("fontSize")]
		[SerializeField]
		private Vector3 _fontSize = new Vector3(8f, 8f, 1f);

		public int textDirection = 1;

		[FormerlySerializedAs("characterSpacingInput")]
		[SerializeField]
		private float _characterSpacing = 1f;

		[FormerlySerializedAs("lineSpacingInput")]
		[SerializeField]
		private float _lineSpacing = 1f;

		[FormerlySerializedAs("capitalize")]
		[SerializeField]
		private bool _capitalize;

		[FormerlySerializedAs("lowercase")]
		[SerializeField]
		private bool _lowercase;

		[FormerlySerializedAs("layoutStyle")]
		[SerializeField]
		private int _layoutStyle;

		[SerializeField]
		private TextAnchor _textAnchor;

		public bool alignCenter = true;

		public bool alignLeft;

		public bool alignRight;

		public bool alignTop;

		public bool alignMiddle = true;

		public bool alignBottom;

		[FormerlySerializedAs("circularAlignmentRadius")]
		[SerializeField]
		private float _circularAlignmentRadius = 5f;

		[FormerlySerializedAs("circularAlignmentSpreadAmount")]
		[SerializeField]
		private float _circularAlignmentSpreadAmount = 360f;

		[FormerlySerializedAs("circularAlignmentAngle")]
		[SerializeField]
		private Vector3 _circularAlignmentAngle = new Vector3(0f, 0f, 0f);

		[FormerlySerializedAs("height")]
		[SerializeField]
		private float _height = 2f;

		[FormerlySerializedAs("length")]
		[SerializeField]
		private float _width = 15f;

		private float adjustedWidth;

		public float depth = 1f;

		public List<MText_ModuleContainer> typingEffects = new List<MText_ModuleContainer>();

		public List<MText_ModuleContainer> deletingEffects = new List<MText_ModuleContainer>();

		public bool customDeleteAfterDuration;

		public float deleteAfter = 1f;

		[Tooltip("When text is updated, old characters are moved to their correct position if their position is moved by something like module.")]
		public bool repositionOldCharacters = true;

		public bool reApplyModulesToOldCharacters;

		[Tooltip("Pooling increases performence if you are changing lots of text when game is running.")]
		public bool pooling;

		public MText_Pool pool;

		public bool combineMeshInEditor;

		public bool dontCombineInEditorAnyway;

		[Tooltip("There is no reason to turn this on unless you really need this for something. \nOtherwise, wasted resource on combining")]
		public bool combineMeshDuringRuntime;

		[Tooltip("Don't let letters show up in hierarchy in play mode. They are still there but not visible.")]
		public bool hideLettersInHierarchyInPlayMode;

		[Tooltip("If combine mesh is turned off")]
		public bool hideLettersInHierarchyInEditMode;

		[Tooltip("Breaks prefab connection while saving prefab location, can replace prefab at that location with a click")]
		public bool canBreakOutermostPrefab;

		public string assetPath = string.Empty;

		[SerializeField]
		private List<string> meshPaths = new List<string>();

		private int charInOneLine;

		private float x;

		private float y;

		private float z;

		private bool createChilds;

		public bool updateTextOncePerFrame = true;

		private bool runningRoutine;

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				if (_text != value)
				{
					_text = value;
					SetTextDirty();
				}
			}
		}

		private string ProcessedText => GetProcessedText();

		public MText_Font Font
		{
			get
			{
				return _font;
			}
			set
			{
				if (_font != value)
				{
					_font = value;
					oldLineList.Clear();
					SetTextDirty();
				}
			}
		}

		public Material Material
		{
			get
			{
				return _material;
			}
			set
			{
				if (_material != value)
				{
					_material = value;
					SetTextDirty();
				}
			}
		}

		public Vector3 FontSize
		{
			get
			{
				return _fontSize;
			}
			set
			{
				if (_fontSize != value)
				{
					_fontSize = value;
					SetTextDirty();
				}
			}
		}

		public float CharacterSpacing
		{
			get
			{
				return _characterSpacing;
			}
			set
			{
				if (_characterSpacing != value)
				{
					_characterSpacing = value;
					SetTextDirty();
				}
			}
		}

		private float ModifiedCharacterSpacing => CharacterSpacing * 0.1f * FontSize.x;

		public float LineSpacing
		{
			get
			{
				return _lineSpacing;
			}
			set
			{
				if (_lineSpacing != value)
				{
					_lineSpacing = value;
					SetTextDirty();
				}
			}
		}

		private float ModifiedLineSpacing => LineSpacing * 0.13f * FontSize.y;

		public bool Capitalize
		{
			get
			{
				return _capitalize;
			}
			set
			{
				if (_capitalize != value)
				{
					_capitalize = value;
					SetTextDirty();
				}
			}
		}

		public bool LowerCase
		{
			get
			{
				return _lowercase;
			}
			set
			{
				if (_lowercase != value)
				{
					_lowercase = value;
					SetTextDirty();
				}
			}
		}

		public int LayoutStyle
		{
			get
			{
				return _layoutStyle;
			}
			set
			{
				_layoutStyle = value;
				SetTextDirty();
			}
		}

		public TextAnchor TextAnchor
		{
			get
			{
				return _textAnchor;
			}
			set
			{
				if (_textAnchor != value)
				{
					switch (value)
					{
					case TextAnchor.UpperLeft:
						alignTop = true;
						alignMiddle = false;
						alignBottom = false;
						alignLeft = true;
						alignCenter = false;
						alignRight = false;
						break;
					case TextAnchor.UpperCenter:
						alignTop = true;
						alignMiddle = false;
						alignBottom = false;
						alignLeft = false;
						alignCenter = true;
						alignRight = false;
						break;
					case TextAnchor.UpperRight:
						alignTop = true;
						alignMiddle = false;
						alignBottom = false;
						alignLeft = false;
						alignCenter = false;
						alignRight = true;
						break;
					case TextAnchor.MiddleLeft:
						alignTop = false;
						alignMiddle = true;
						alignBottom = false;
						alignLeft = true;
						alignCenter = false;
						alignRight = false;
						break;
					case TextAnchor.MiddleCenter:
						alignTop = false;
						alignMiddle = true;
						alignBottom = false;
						alignLeft = false;
						alignCenter = true;
						alignRight = false;
						break;
					case TextAnchor.MiddleRight:
						alignTop = false;
						alignMiddle = true;
						alignBottom = false;
						alignLeft = false;
						alignCenter = false;
						alignRight = true;
						break;
					case TextAnchor.LowerLeft:
						alignTop = false;
						alignMiddle = false;
						alignBottom = true;
						alignLeft = true;
						alignCenter = false;
						alignRight = false;
						break;
					case TextAnchor.LowerCenter:
						alignTop = false;
						alignMiddle = false;
						alignBottom = true;
						alignLeft = false;
						alignCenter = true;
						alignRight = false;
						break;
					case TextAnchor.LowerRight:
						alignTop = false;
						alignMiddle = false;
						alignBottom = true;
						alignLeft = false;
						alignCenter = false;
						alignRight = true;
						break;
					}
					_textAnchor = value;
					SetTextDirty();
				}
			}
		}

		public float CircularAlignmentRadius
		{
			get
			{
				return _circularAlignmentRadius;
			}
			set
			{
				if (_circularAlignmentRadius != value)
				{
					_circularAlignmentRadius = value;
					SetTextDirty();
				}
			}
		}

		public float CircularAlignmentSpreadAmount
		{
			get
			{
				return _circularAlignmentSpreadAmount;
			}
			set
			{
				if (_circularAlignmentSpreadAmount != value)
				{
					_circularAlignmentSpreadAmount = value;
					SetTextDirty();
				}
			}
		}

		public Vector3 CircularAlignmentAngle
		{
			get
			{
				return _circularAlignmentAngle;
			}
			set
			{
				if (_circularAlignmentAngle != value)
				{
					_circularAlignmentAngle = value;
					SetTextDirty();
				}
			}
		}

		public float Height
		{
			get
			{
				return _height;
			}
			set
			{
				if (_height != value)
				{
					_height = value;
					SetTextDirty();
				}
			}
		}

		public float Width
		{
			get
			{
				return _width;
			}
			set
			{
				if (_width != value)
				{
					_width = value;
					SetTextDirty();
				}
			}
		}

		private void OnEnable()
		{
			runningRoutine = false;
		}

		private void SetTextDirty()
		{
			if (base.gameObject.activeInHierarchy && updateTextOncePerFrame)
			{
				if (!runningRoutine)
				{
					runningRoutine = true;
					StartCoroutine(UpdateRoutine());
				}
			}
			else
			{
				UpdateText();
			}
		}

		private IEnumerator UpdateRoutine()
		{
			yield return new WaitForEndOfFrame();
			UpdateText();
			runningRoutine = false;
		}

		private string GetProcessedText()
		{
			if (Capitalize)
			{
				return Text.ToUpper();
			}
			if (LowerCase)
			{
				return Text.ToLower();
			}
			return Text;
		}

		public void UpdateText(string newText)
		{
			if (!Font)
			{
				Debug.Log("No font assigned on " + base.gameObject.name, base.gameObject);
				return;
			}
			Text = newText;
			UpdateText();
		}

		public void UpdateText(float number)
		{
			if (!Font)
			{
				Debug.Log("No font assigned on " + base.gameObject.name, base.gameObject);
				return;
			}
			Text = number.ToString();
			UpdateText();
		}

		public void UpdateText(int number)
		{
			if (!Font)
			{
				Debug.Log("No font assigned on " + base.gameObject.name, base.gameObject);
				return;
			}
			Text = number.ToString();
			UpdateText();
		}

		[ContextMenu("Update")]
		public void UpdateText()
		{
			UpdateTextBase();
		}

		private void UpdateTextBase()
		{
			if (!Font)
			{
				return;
			}
			FixInvalidInputs();
			if (LayoutStyle == 0)
			{
				charInOneLine = CharacterInOneLineUpdate();
				if (charInOneLine < 1)
				{
					charInOneLine = 1;
				}
			}
			x = 0f;
			createChilds = ShouldItCreateChild();
			SplitStuff();
			int startingFrom = 0;
			if (createChilds)
			{
				if ((bool)GetComponent<MeshRenderer>())
				{
					startingFrom = 0;
					DestroyMeshRenderAndMeshFilter();
				}
				else
				{
					startingFrom = CompareNewTextWithOld();
				}
			}
			oldLineList = new List<string>(lineList);
			DeleteReplacedChars(startingFrom);
			if (LayoutStyle == 0)
			{
				GetPositionAtStart();
			}
			CheckIfPoolExistsAndRequired();
			if (LayoutStyle == 0)
			{
				if (repositionOldCharacters)
				{
					PositionOldChars(startingFrom);
				}
				CreateNewChars(startingFrom);
			}
			else
			{
				CircularListProcessOldChars(startingFrom);
				CreateNewCharsForCircularList(startingFrom);
			}
			if (LayoutStyle == 1)
			{
				CircularPositioning();
			}
			if (!createChilds)
			{
				CombineMeshes();
			}
		}

		private void DestroyMeshRenderAndMeshFilter()
		{
			UnityEngine.Object.Destroy(GetComponent<MeshRenderer>());
			UnityEngine.Object.Destroy(GetComponent<MeshFilter>());
		}

		public bool ShouldItCreateChild()
		{
			bool result = false;
			if (!combineMeshDuringRuntime)
			{
				result = true;
			}
			return result;
		}

		private void CheckIfPoolExistsAndRequired()
		{
			if (pooling && !pool)
			{
				pool = MText_Pool.Instance;
				if (!pool)
				{
					CreatePool();
				}
			}
		}

		private void CreatePool()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "Modular 3D Text Pool";
			gameObject.AddComponent<MText_Pool>();
			pool = gameObject.GetComponent<MText_Pool>();
			MText_Pool.Instance = pool;
		}

		private void SplitStuff()
		{
			string pattern = "([ \r\n])";
			string[] wordArray = Regex.Split(ProcessedText, pattern);
			List<string> wordList = RemoveSpacesFromWordArray(wordArray).ToList();
			GetLineList(wordList);
		}

		private string[] RemoveSpacesFromWordArray(string[] wordArray)
		{
			List<string> list = new List<string>();
			foreach (string text in wordArray)
			{
				if (text != " ")
				{
					list.Add(text);
				}
			}
			return list.ToArray();
		}

		private void GetLineList(List<string> wordList)
		{
			lineList = new List<string>();
			if (LayoutStyle == 0)
			{
				GetLinearLineList(wordList);
			}
			else
			{
				GetCircularLineList(wordList);
			}
		}

		private void GetLinearLineList(List<string> wordList)
		{
			float num = 0f;
			string text = "";
			for (int i = 0; i < wordList.Count; i++)
			{
				float num2 = TotalSpacingRequiredFor(wordList[i]);
				if (wordList[i].Contains("\n"))
				{
					lineList.Add(text);
					text = ((!(wordList[i] == "\n")) ? wordList[i] : "");
					num = 0f;
					num2 = 0f;
				}
				else if (num + num2 < adjustedWidth)
				{
					text = ((num == 0f) ? (text + wordList[i]) : (text + " " + wordList[i]));
				}
				else if (num2 > adjustedWidth)
				{
					if (Font.monoSpaceFont)
					{
						int num3 = 0;
						int num4 = Mathf.CeilToInt(wordList[i].Length / charInOneLine);
						for (int j = 0; j < num4; j++)
						{
							string item = wordList[i].Substring(num3, charInOneLine);
							lineList.Add(item);
							num3 += charInOneLine;
						}
						text = wordList[i].Substring(num3);
						num = TotalSpacingRequiredFor(text);
					}
					else
					{
						if (text != string.Empty)
						{
							lineList.Add(text);
						}
						char[] array = wordList[i].ToCharArray();
						text = string.Empty;
						num = 0f;
						for (int k = 0; k < array.Length; k++)
						{
							float num5 = ((k != 0) ? (Font.Spacing(array[k - 1], array[k]) * ModifiedCharacterSpacing) : (Font.Spacing(array[k]) * ModifiedCharacterSpacing));
							if (num + num5 <= adjustedWidth)
							{
								num += num5;
								text += array[k];
							}
							else
							{
								lineList.Add(text);
								text = array[k].ToString();
								num = num5;
							}
						}
					}
					num2 = 0f;
				}
				else
				{
					string text2 = text;
					if (text2 != "")
					{
						lineList.Add(text2);
					}
					num = 0f;
					text = wordList[i];
				}
				num += num2;
				if (i == wordList.Count - 1)
				{
					lineList.Add(text);
				}
			}
		}

		private void GetCircularLineList(List<string> wordList)
		{
			float num = 0f;
			string text = "";
			float num2 = MathF.PI * 2f * CircularAlignmentRadius * (CircularAlignmentSpreadAmount / 360f);
			for (int i = 0; i < wordList.Count; i++)
			{
				float num3 = TotalSpacingRequiredFor(wordList[i]);
				if (wordList[i].Contains("\n"))
				{
					lineList.Add(text);
					text = ((!(wordList[i] == "\n")) ? wordList[i] : "");
					num = 0f;
					num3 = 0f;
				}
				else if (num + num3 < num2)
				{
					text = ((num == 0f) ? (text + wordList[i]) : (text + " " + wordList[i]));
				}
				else if (num3 > num2)
				{
					if (Font.monoSpaceFont)
					{
						int num4 = 0;
						float num5 = num2 / ModifiedCharacterSpacing;
						int num6 = Mathf.CeilToInt((float)wordList[i].Length / num5);
						for (int j = 0; j < num6; j++)
						{
							string item = wordList[i].Substring(num4, charInOneLine);
							lineList.Add(item);
							num4 += charInOneLine;
						}
						text = wordList[i].Substring(num4);
						num = TotalSpacingRequiredFor(text);
					}
					else
					{
						if (text != string.Empty)
						{
							lineList.Add(text);
						}
						char[] array = wordList[i].ToCharArray();
						text = string.Empty;
						num = 0f;
						for (int k = 0; k < array.Length; k++)
						{
							float num7 = ((k != 0) ? (Font.Spacing(array[k - 1], array[k]) * ModifiedCharacterSpacing) : (Font.Spacing(array[k]) * ModifiedCharacterSpacing));
							if (num + num7 <= num2)
							{
								num += num7;
								text += array[k];
							}
							else
							{
								lineList.Add(text);
								text = array[k].ToString();
								num = num7;
							}
						}
					}
					num3 = 0f;
				}
				else
				{
					string text2 = text;
					if (text2 != "")
					{
						lineList.Add(text2);
					}
					num = 0f;
					text = wordList[i];
				}
				num += num3;
				if (i == wordList.Count - 1)
				{
					lineList.Add(text);
				}
			}
		}

		private void FixInvalidInputs()
		{
			if (Width != 0f)
			{
				adjustedWidth = Width;
			}
			else
			{
				adjustedWidth = 10f;
			}
			if (LayoutStyle == 1)
			{
				adjustedWidth = 50f * (CircularAlignmentSpreadAmount / 360f);
			}
		}

		private int CompareNewTextWithOld()
		{
			int num = 0;
			for (int i = 0; i < lineList.Count; i++)
			{
				if (oldLineList.Count <= i)
				{
					return num;
				}
				char[] array = lineList[i].ToCharArray();
				char[] array2 = oldLineList[i].ToCharArray();
				if (array.Length == 0 || array2.Length == 0)
				{
					return num;
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (j >= array2.Length)
					{
						return num;
					}
					if (array[j] != array2[j])
					{
						return num;
					}
					num++;
				}
			}
			return num;
		}

		private void DeleteReplacedChars(int startingFrom)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = startingFrom; i < characterObjectList.Count; i++)
			{
				if (i >= startingFrom)
				{
					list.Add(characterObjectList[i]);
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			foreach (GameObject item in list)
			{
				DestroyObject(item);
				characterObjectList.Remove(item);
			}
		}

		private void DeleteAllChildObjects()
		{
			if (characterObjectList.Count == 0)
			{
				return;
			}
			for (int i = 0; i < characterObjectList.Count; i++)
			{
				if (!characterObjectList[i])
				{
					return;
				}
				UnityEngine.Object.Destroy(characterObjectList[i]);
			}
			characterObjectList.Clear();
		}

		private void DestroyObject(GameObject obj)
		{
			if ((bool)obj)
			{
				if (characterObjectList.Contains(obj))
				{
					characterObjectList.Remove(obj);
				}
				if (base.gameObject.activeInHierarchy)
				{
					StartCoroutine(RunTimeDestroyObjectRoutine(obj));
				}
				else
				{
					RunTimeDestroyObjectOnDisabledText(obj);
				}
			}
		}

		private IEnumerator RunTimeDestroyObjectRoutine(GameObject obj)
		{
			float num = 0f;
			obj.transform.SetParent(null);
			if (obj.name != "space")
			{
				if (base.gameObject.activeInHierarchy)
				{
					for (int i = 0; i < deletingEffects.Count; i++)
					{
						if ((bool)deletingEffects[i].module)
						{
							StartCoroutine(deletingEffects[i].module.ModuleRoutine(obj, deletingEffects[i].duration));
							if (deletingEffects[i].duration > num)
							{
								num = deletingEffects[i].duration;
							}
						}
					}
				}
				if (!customDeleteAfterDuration)
				{
					if (deletingEffects.Count > 0)
					{
						yield return new WaitForSeconds(num);
					}
				}
				else
				{
					yield return new WaitForSeconds(deleteAfter);
				}
			}
			if (pooling && (bool)pool)
			{
				pool.ReturnPoolItem(obj);
			}
			else
			{
				UnityEngine.Object.Destroy(obj);
			}
		}

		private void RunTimeDestroyObjectOnDisabledText(GameObject obj)
		{
			if (pooling && (bool)pool)
			{
				pool.ReturnPoolItem(obj);
			}
			else
			{
				UnityEngine.Object.Destroy(obj);
			}
		}

		private void PositionOldChars(int startingFrom)
		{
			int num = 0;
			for (int i = 0; i < lineList.Count; i++)
			{
				if (lineList.Count > i)
				{
					x = StartingX(lineList[i]);
				}
				char[] array = lineList[i].ToCharArray();
				for (int j = 0; j < array.Length; j++)
				{
					if (num >= startingFrom)
					{
						break;
					}
					float num2 = ((j != 0) ? Font.Spacing(array[j - 1], array[j]) : Font.Spacing(array[0]));
					float num3 = ModifiedCharacterSpacing * (num2 / 2f) * (float)textDirection;
					x += num3;
					if (characterObjectList.Count > num)
					{
						PrepareCharacter(characterObjectList[num]);
						if (reApplyModulesToOldCharacters)
						{
							ApplyEffects(characterObjectList[num]);
						}
					}
					x += num3;
					num++;
				}
				y -= ModifiedLineSpacing;
			}
		}

		private void GetPositionAtStart()
		{
			x = StartingX(lineList[0]);
			y = StartingY();
			z = 0f;
		}

		private float StartingX(string myString)
		{
			if (alignCenter)
			{
				if (!Font.monoSpaceFont)
				{
					return (0f - (TotalSpacingRequiredFor(myString) - ModifiedCharacterSpacing * Font.emptySpaceSpacing) / 2f) * (float)textDirection;
				}
				return (0f - (float)myString.Length * ModifiedCharacterSpacing / 2f) * (float)textDirection;
			}
			if (alignLeft)
			{
				return (0f - adjustedWidth) / 2f * (float)textDirection;
			}
			if (!Font.monoSpaceFont)
			{
				return (adjustedWidth / 2f - (TotalSpacingRequiredFor(myString) - ModifiedCharacterSpacing * Font.emptySpaceSpacing)) * (float)textDirection;
			}
			return (adjustedWidth / 2f - (float)myString.Length * ModifiedCharacterSpacing) * (float)textDirection;
		}

		private float TotalSpacingRequiredFor(string myString)
		{
			char[] array = myString.ToCharArray();
			float num = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				num = ((i != 0) ? (num + ModifiedCharacterSpacing * Font.Spacing(array[i - 1], array[i])) : (num + ModifiedCharacterSpacing * Font.Spacing(array[0])));
			}
			return num + ModifiedCharacterSpacing * Font.emptySpaceSpacing;
		}

		private float StartingY()
		{
			if (alignTop)
			{
				return Height / 2f - ModifiedLineSpacing / 2f;
			}
			if (!alignMiddle)
			{
				return (0f - Height) / 2f + (float)lineList.Count * ModifiedLineSpacing - ModifiedLineSpacing / 2f;
			}
			if (lineList.Count == 1)
			{
				return 0f;
			}
			return (float)lineList.Count / 2f * ModifiedLineSpacing - ModifiedLineSpacing / 2f;
		}

		private int CharacterInOneLineUpdate()
		{
			return Mathf.FloorToInt(adjustedWidth / ModifiedCharacterSpacing);
		}

		private void CircularPositioning()
		{
			float num = 0f;
			if (lineList.Count > 0)
			{
				num = ((lineList[0].Length != 1) ? ((0f - CircularAlignmentSpreadAmount / 2f + CircularAlignmentSpreadAmount / (float)lineList[0].Length / 2f) * (float)textDirection) : 0f);
			}
			int num2 = 0;
			int num3 = 0;
			float num4 = 0f;
			for (int i = 0; i < characterObjectList.Count; i++)
			{
				if (num2 > lineList[num3].Length - 1)
				{
					num4 += ModifiedLineSpacing;
					num2 = 0;
					num3++;
					num = 0f;
					if (lineList.Count > num3 && lineList[num3].Length > 1)
					{
						num = (0f - CircularAlignmentSpreadAmount / 2f + CircularAlignmentSpreadAmount / (float)lineList[num3].Length / 2f) * (float)textDirection;
					}
				}
				num2++;
				float num5 = Mathf.Sin(MathF.PI / 180f * num) * CircularAlignmentRadius;
				float num6 = Mathf.Cos(MathF.PI / 180f * num) * CircularAlignmentRadius;
				if ((bool)characterObjectList[i])
				{
					characterObjectList[i].transform.localPosition = new Vector3(num5, num4, num6);
					characterObjectList[i].transform.localRotation = Quaternion.Euler(CircularAlignmentAngle.x, num - CircularAlignmentAngle.y, CircularAlignmentAngle.z);
				}
				if (lineList.Count > num3)
				{
					num += CircularAlignmentSpreadAmount / (float)lineList[num3].Length * (float)textDirection;
				}
			}
		}

		private void CircularListProcessOldChars(int startingFrom)
		{
			int num = 0;
			for (int i = 0; i < lineList.Count; i++)
			{
				string text = lineList[i];
				foreach (char c in text)
				{
					if (num >= startingFrom || characterObjectList.Count >= num)
					{
						break;
					}
					if ((bool)characterObjectList[i])
					{
						ApplyStyle(characterObjectList[num]);
						if (reApplyModulesToOldCharacters)
						{
							ApplyEffects(characterObjectList[num]);
						}
					}
					num++;
				}
			}
		}

		private void CreateNewCharsForCircularList(int startingFrom)
		{
			int num = 0;
			for (int i = 0; i < lineList.Count; i++)
			{
				string text = lineList[i];
				foreach (char c in text)
				{
					if (num >= startingFrom)
					{
						CreateCharForCircularList(c);
					}
					num++;
				}
			}
		}

		private void CreateCharForCircularList(char c)
		{
			if ((bool)this)
			{
				GameObject gameObject = MText_GetCharacterObject.GetObject(c, this);
				gameObject.transform.SetParent(base.transform);
				ApplyStyle(gameObject);
				AddCharacterToList(gameObject);
				ApplyEffects(gameObject);
			}
		}

		private void CreateNewChars(int startingFrom)
		{
			int num = 0;
			for (int i = 0; i < lineList.Count; i++)
			{
				if (num + lineList[i].Length > startingFrom)
				{
					x = StartingX(lineList[i]);
					y = StartingY() - ModifiedLineSpacing * (float)i;
					string text;
					if (num > startingFrom)
					{
						text = lineList[i];
					}
					else
					{
						if (Font.monoSpaceFont)
						{
							x += ModifiedCharacterSpacing * (float)(startingFrom - num) * (float)textDirection;
						}
						else
						{
							char[] array = lineList[i].Substring(0, startingFrom - num).ToCharArray();
							for (int j = 0; j < array.Length; j++)
							{
								if (j == 0)
								{
									x += ModifiedCharacterSpacing * Font.Spacing(array[0]) * (float)textDirection;
								}
								else
								{
									x += ModifiedCharacterSpacing * Font.Spacing(array[j - 1], array[j]) * (float)textDirection;
								}
							}
						}
						text = lineList[i].Substring(startingFrom - num);
					}
					for (int k = 0; k < text.Length; k++)
					{
						float num2 = ((k != 0) ? Font.Spacing(text[k - 1], text[k]) : Font.Spacing(text[0]));
						float num3 = ModifiedCharacterSpacing * (num2 / 2f) * (float)textDirection;
						x += num3;
						CreateThisChar(text[k]);
						x += num3;
					}
				}
				num += lineList[i].Length;
			}
		}

		private void CreateThisChar(char c)
		{
			GameObject obj = MText_GetCharacterObject.GetObject(c, this);
			AddCharacterToList(obj);
			PrepareCharacter(obj);
			ApplyEffects(obj);
		}

		private void PrepareCharacter(GameObject obj)
		{
			if ((bool)this && (bool)obj)
			{
				obj.transform.SetParent(base.transform);
				obj.transform.localPosition = new Vector3(x + Font.positionFix.x, y + Font.positionFix.y, z + Font.positionFix.z);
				obj.transform.localRotation = Quaternion.Euler(Font.rotationFix.x, Font.rotationFix.y, Font.rotationFix.z);
				ApplyStyle(obj);
			}
		}

		private void CreateAndPrepareCharacter(char c, float myX, float myY, float myZ, Transform tr)
		{
			if ((bool)tr)
			{
				GameObject gameObject = MText_GetCharacterObject.GetObject(c, this);
				if ((bool)gameObject)
				{
					AddCharacterToList(gameObject);
					gameObject.transform.SetParent(tr);
					gameObject.transform.localPosition = new Vector3(myX + Font.positionFix.x, myY + Font.positionFix.y, myZ + Font.positionFix.z);
					gameObject.transform.localRotation = Quaternion.Euler(Font.rotationFix.x, Font.rotationFix.y, Font.rotationFix.z);
					ApplyStyle(gameObject);
				}
			}
		}

		private void AddCharacterToList(GameObject obj)
		{
			characterObjectList.Add(obj);
		}

		private void ApplyEffects(GameObject obj)
		{
			if (!base.gameObject.activeInHierarchy || !(obj.name != "space"))
			{
				return;
			}
			for (int i = 0; i < typingEffects.Count; i++)
			{
				if ((bool)typingEffects[i].module)
				{
					StartCoroutine(typingEffects[i].module.ModuleRoutine(obj, typingEffects[i].duration));
				}
			}
		}

		private void ApplyStyle(GameObject obj)
		{
			if ((bool)obj.GetComponent<MText_Letter>() && (bool)obj.GetComponent<MText_Letter>().model)
			{
				obj.GetComponent<MText_Letter>().model.material = Material;
			}
			if ((bool)obj.GetComponent<MeshFilter>())
			{
				if (!obj.GetComponent<MeshRenderer>())
				{
					obj.AddComponent<MeshRenderer>();
				}
				obj.GetComponent<MeshRenderer>().material = Material;
			}
			obj.transform.localScale = new Vector3(FontSize.x, FontSize.y, FontSize.z / 2f);
			SetLayer(obj);
		}

		private void SetLayer(GameObject obj)
		{
			if ((bool)obj)
			{
				obj.layer = base.gameObject.layer;
			}
		}

		private void AddToList(GameObject combinedMeshHolder)
		{
			characterObjectList.Add(combinedMeshHolder);
		}

		public void EmptyEffect(List<MText_ModuleContainer> moduleList)
		{
			moduleList.Add(new MText_ModuleContainer
			{
				duration = 0.5f
			});
		}

		public void NewEffect(List<MText_ModuleContainer> moduleList, MText_Module newModule)
		{
			moduleList.Add(new MText_ModuleContainer
			{
				duration = 0.5f,
				module = newModule
			});
		}

		public void ClearAllEffects()
		{
			typingEffects.Clear();
			deletingEffects.Clear();
		}

		[ContextMenu("Combine meshes")]
		private void CombineMeshes()
		{
			if (!this || !base.gameObject)
			{
				return;
			}
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			Vector3 localScale = base.transform.localScale;
			base.transform.rotation = Quaternion.identity;
			base.transform.position = Vector3.zero;
			Vector3 lossyScale = base.transform.lossyScale;
			if (lossyScale != base.transform.localScale)
			{
				lossyScale = new Vector3(1f / lossyScale.x * localScale.x, 1f / lossyScale.y * localScale.y, 1f / lossyScale.z * localScale.z);
				base.transform.localScale = lossyScale;
			}
			else
			{
				base.transform.localScale = Vector3.one;
			}
			if ((bool)GetComponent<MeshFilter>())
			{
				GetComponent<MeshFilter>().mesh = null;
			}
			List<List<MeshFilter>> list = new List<List<MeshFilter>>();
			int num = 0;
			List<MeshFilter> item = new List<MeshFilter>();
			list.Add(item);
			int num2 = 0;
			for (int i = 0; i < characterObjectList.Count; i++)
			{
				if ((bool)characterObjectList[i] && (bool)characterObjectList[i].GetComponent<MeshFilter>())
				{
					if (num2 + characterObjectList[i].GetComponent<MeshFilter>().sharedMesh.vertices.Length < 65535)
					{
						num2 += characterObjectList[i].GetComponent<MeshFilter>().sharedMesh.vertices.Length;
						list[num].Add(characterObjectList[i].GetComponent<MeshFilter>());
						continue;
					}
					num2 = 0;
					List<MeshFilter> item2 = new List<MeshFilter>();
					list.Add(item2);
					num++;
					num2 += characterObjectList[i].GetComponent<MeshFilter>().sharedMesh.vertices.Length;
					list[num].Add(characterObjectList[i].GetComponent<MeshFilter>());
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				MeshFilter[] array = list[j].ToArray();
				CombineInstance[] array2 = new CombineInstance[array.Length];
				for (int k = 0; k < array.Length; k++)
				{
					array2[k].mesh = array[k].sharedMesh;
					array2[k].transform = array[k].transform.localToWorldMatrix;
				}
				if (!GetComponent<MeshFilter>())
				{
					base.gameObject.AddComponent<MeshFilter>();
				}
				List<CombineInstance> list2 = new List<CombineInstance>();
				for (int l = 0; l < array2.Length; l++)
				{
					if (array2[l].mesh != null)
					{
						list2.Add(array2[l]);
					}
				}
				array2 = list2.ToArray();
				Mesh mesh = new Mesh();
				mesh.CombineMeshes(array2);
				if (j == 0)
				{
					GetComponent<MeshFilter>().mesh = mesh;
					if (!GetComponent<MeshRenderer>())
					{
						base.gameObject.AddComponent<MeshRenderer>();
					}
					GetComponent<MeshRenderer>().material = Material;
				}
				else
				{
					GameObject gameObject = new GameObject();
					gameObject.name = "Combined mesh 2";
					gameObject.transform.SetParent(base.transform);
					gameObject.transform.rotation = Quaternion.identity;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.AddComponent<MeshFilter>();
					gameObject.GetComponent<MeshFilter>().mesh = mesh;
					gameObject.AddComponent<MeshRenderer>();
					gameObject.GetComponent<MeshRenderer>().material = Material;
				}
			}
			base.transform.position = position;
			base.transform.rotation = rotation;
			base.transform.localScale = localScale;
			DeleteAllChildObjects();
		}

		public bool DoesStyleInheritFromAParent()
		{
			return (bool)base.transform.parent && (bool)base.transform.parent.gameObject.GetComponent<MText_UI_Button>() && base.transform.parent.gameObject.GetComponent<MText_UI_Button>().text == this;
		}
	}
}
