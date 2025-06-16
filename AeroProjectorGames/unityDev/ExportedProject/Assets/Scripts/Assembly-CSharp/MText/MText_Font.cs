using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MText
{
	[CreateAssetMenu(fileName = "New 3D Font", menuName = "Modular 3d Text/Font/New Font")]
	public class MText_Font : ScriptableObject
	{
		public List<MText_Character> characters = new List<MText_Character>();

		[SerializeField]
		private GameObject fontSet;

		[Tooltip("When set to false, new fontset will be added while keeping the old one")]
		[SerializeField]
		private bool overwriteOldSet = true;

		[Tooltip("A font where each character is spaced equally. If turned on, individual spacing value from list below is ignored")]
		public bool monoSpaceFont;

		public bool useUpperCaseLettersIfLowerCaseIsMissing = true;

		[Tooltip("Word spacing and spacing for unavailable characters")]
		public float emptySpaceSpacing = 1f;

		[Tooltip("Text's character spacing = font's character spacing * text's character spacing")]
		public float characterSpacing = 1f;

		[Space]
		[Tooltip("Avoid recursive references")]
		public List<MText_Font> fallbackFonts = new List<MText_Font>();

		public Vector3 rotationFix;

		public Vector3 positionFix;

		public Vector3 scaleFix;

		public bool enableKerning = true;

		public float kerningMultiplier = 1f;

		public List<MText_KernPairHolder> kernTable = new List<MText_KernPairHolder>();

		public Mesh RetrievePrefab(char c)
		{
			for (int i = 0; i < characters.Count; i++)
			{
				if (c == characters[i].character)
				{
					return MeshPrefab(i);
				}
			}
			if (useUpperCaseLettersIfLowerCaseIsMissing && char.IsLower(c))
			{
				c = char.ToUpper(c);
				for (int j = 0; j < characters.Count; j++)
				{
					if (c == characters[j].character)
					{
						return MeshPrefab(j);
					}
				}
			}
			for (int k = 0; k < fallbackFonts.Count; k++)
			{
				if (fallbackFonts[k] != null)
				{
					Mesh mesh = fallbackFonts[k].RetrievePrefabWithoutFallbackCheck(c);
					if (mesh != null)
					{
						return mesh;
					}
				}
			}
			return null;
		}

		public Mesh RetrievePrefabWithoutFallbackCheck(char c)
		{
			for (int i = 0; i < characters.Count; i++)
			{
				if (c == characters[i].character)
				{
					return MeshPrefab(i);
				}
			}
			if (useUpperCaseLettersIfLowerCaseIsMissing && char.IsLower(c))
			{
				c = char.ToUpper(c);
				for (int j = 0; j < characters.Count; j++)
				{
					if (c == characters[j].character)
					{
						return MeshPrefab(j);
					}
				}
			}
			return null;
		}

		private Mesh MeshPrefab(int i)
		{
			if ((bool)characters[i].prefab)
			{
				if ((bool)characters[i].prefab.GetComponent<MeshFilter>() && (bool)characters[i].prefab.GetComponent<MeshFilter>().sharedMesh)
				{
					return characters[i].prefab.GetComponent<MeshFilter>().sharedMesh;
				}
			}
			else if ((bool)characters[i].meshPrefab)
			{
				return characters[i].meshPrefab;
			}
			return null;
		}

		public float Spacing(char c)
		{
			if (!monoSpaceFont)
			{
				for (int i = 0; i < characters.Count; i++)
				{
					if (c == characters[i].character)
					{
						return characters[i].spacing * characterSpacing;
					}
				}
			}
			return emptySpaceSpacing * characterSpacing;
		}

		public float Spacing(char previousChar, char currentChar)
		{
			if (!enableKerning || kernTable.Count == 0)
			{
				return Spacing(currentChar);
			}
			if (!monoSpaceFont)
			{
				for (int i = 0; i < characters.Count; i++)
				{
					if (currentChar == characters[i].character)
					{
						float num = Kerning(previousChar, currentChar);
						return characters[i].spacing * characterSpacing * num;
					}
				}
			}
			return emptySpaceSpacing * characterSpacing;
		}

		private float Kerning(char previousChar, char currentChar)
		{
			MText_Character mText_Character = Character(previousChar);
			MText_Character mText_Character2 = Character(currentChar);
			if (mText_Character == null || mText_Character2 == null)
			{
				return 1f;
			}
			MText_KernPair mText_KernPair = new MText_KernPair
			{
				left = mText_Character.glyphIndex,
				right = mText_Character2.glyphIndex
			};
			for (int i = 0; i < kernTable.Count; i++)
			{
				if (kernTable[i].kernPair.left == mText_KernPair.left && kernTable[i].kernPair.right == mText_KernPair.right)
				{
					return 1f + (float)kernTable[i].offset * kerningMultiplier * 0.01f;
				}
			}
			return 1f;
		}

		private MText_Character Character(char c)
		{
			for (int i = 0; i < characters.Count; i++)
			{
				if (characters[i].character == c)
				{
					return characters[i];
				}
			}
			return null;
		}

		public void UpdateCharacterList(GameObject prefab)
		{
			fontSet = prefab;
			UpdateCharacterList();
		}

		public void UpdateCharacterList()
		{
			if (overwriteOldSet)
			{
				characters.Clear();
			}
			if ((bool)fontSet)
			{
				foreach (Transform item in fontSet.transform)
				{
					AddCharacter(item.gameObject);
				}
			}
			else
			{
				Debug.LogWarning("Fontset not found on " + base.name);
			}
			AverageSpacing();
		}

		public void AddCharacter(GameObject obj)
		{
			MText_Character mText_Character = new MText_Character();
			if ((bool)obj)
			{
				ProcessName(obj.name, out var character, out var spacing);
				mText_Character.character = character;
				mText_Character.spacing = spacing;
				mText_Character.prefab = obj;
				characters.Add(mText_Character);
			}
		}

		public void AddCharacter(Mesh mesh)
		{
			MText_Character mText_Character = new MText_Character();
			if ((bool)mesh)
			{
				ProcessName(mesh.name, out var character, out var spacing);
				mText_Character.character = character;
				mText_Character.spacing = spacing;
				mText_Character.meshPrefab = mesh;
				characters.Add(mText_Character);
			}
		}

		private void ProcessName(string name, out char character, out float spacing)
		{
			try
			{
				NewMethod(name, out character, out spacing);
			}
			catch
			{
				OldMethod(name, out character, out spacing);
			}
		}

		private void NewMethod(string name, out char character, out float spacing)
		{
			string[] array = name.Split(new char[3] { '_', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			character = Regex.Unescape("\\u" + array[0]).ToCharArray()[0];
			spacing = GetSpacing(array[1]);
		}

		private void OldMethod(string name, out char character, out float spacing)
		{
			if (name.Contains("dot"))
			{
				character = '.';
				spacing = (float)Convert.ToDouble(name.Substring(4));
			}
			else if (name.Contains("forwardSlash"))
			{
				character = '/';
				spacing = GetSpacing(name.Substring(13));
			}
			else if (name.Contains("quotationMark"))
			{
				character = '"';
				spacing = GetSpacing(name.Substring(14));
			}
			else if (name.Contains("multiply"))
			{
				character = '*';
				spacing = GetSpacing(name.Substring(9));
			}
			else if (name.Contains("colon"))
			{
				character = ':';
				spacing = GetSpacing(name.Substring(6));
			}
			else if (name.Contains("lessThan"))
			{
				character = '<';
				spacing = GetSpacing(name.Substring(9));
			}
			else if (name.Contains("moreThan"))
			{
				character = '>';
				spacing = GetSpacing(name.Substring(9));
			}
			else if (name.Contains("questionMark"))
			{
				character = '?';
				spacing = GetSpacing(name.Substring(13));
			}
			else if (name.Contains("slash"))
			{
				character = '/';
				spacing = GetSpacing(name.Substring(6));
			}
			else if (name.Contains("backwardSlash"))
			{
				character = '\\';
				spacing = GetSpacing(name.Substring(14));
			}
			else if (name.Contains("verticalLine"))
			{
				character = '|';
				spacing = GetSpacing(name.Substring(13));
			}
			else
			{
				char[] array = name.ToCharArray();
				character = array[0];
				spacing = GetSpacing(name.Substring(2));
			}
			spacing *= 0.81f;
		}

		private float GetSpacing(string numberString)
		{
			if (float.TryParse(numberString, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
			{
				return result;
			}
			return 1f;
		}

		[ContextMenu("Get average spacing")]
		private void AverageSpacing()
		{
			float num = 0f;
			for (int i = 0; i < characters.Count; i++)
			{
				num += characters[i].spacing;
			}
			emptySpaceSpacing = num / (float)characters.Count;
		}
	}
}
