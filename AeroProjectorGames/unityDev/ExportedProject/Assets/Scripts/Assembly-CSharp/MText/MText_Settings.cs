using UnityEngine;

namespace MText
{
	[CreateAssetMenu(menuName = "Modular 3d Text/Settings")]
	public class MText_Settings : ScriptableObject
	{
		public enum MeshExportStyle
		{
			exportAsObj = 0,
			exportAsMeshAsset = 1
		}

		public enum CharInputStyle
		{
			CharacterRange = 0,
			UnicodeRange = 1,
			CustomCharacters = 2,
			UnicodeSequence = 3
		}

		[HideInInspector]
		public string selectedTab = "Getting Started";

		public Color tabSelectedColor = Color.white;

		public Color tabUnselectedColor = new Color(0.9f, 0.9f, 0.9f);

		[Space]
		public Color gridItemColor = new Color(0.9f, 0.9f, 0.9f);

		[Header("Text default settings")]
		public MText_Font defaultFont;

		public Vector3 defaultTextSize = new Vector3(8f, 8f, 2f);

		public Material defaultTextMaterial;

		public Material defaultBackgroundMaterial;

		public bool autoCreateInEditorMode = true;

		public bool autoCreateInPlayMode = true;

		[HideInInspector]
		public int vertexDensity = 1;

		[HideInInspector]
		public float sizeXY = 1f;

		[HideInInspector]
		public float sizeZ = 1f;

		[HideInInspector]
		public float smoothingAngle = 30f;

		[HideInInspector]
		public MeshExportStyle meshExportStyle;

		[Header("Inspector field size")]
		public float smallHorizontalFieldSize = 72.5f;

		public float normalltHorizontalFieldSize = 100f;

		public float largeHorizontalFieldSize = 132.5f;

		public float extraLargeHorizontalFieldSize = 150f;

		public CharInputStyle charInputStyle;

		public char startChar = '!';

		public char endChar = '~';

		public string startUnicode = "0021";

		[HideInInspector]
		public string endUnicode = "007E";

		[HideInInspector]
		[TextArea(10, 99)]
		public string customCharacters = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

		[HideInInspector]
		[TextArea(10, 99)]
		public string unicodeSequence = "\\u0021-\\u007E";
	}
}
