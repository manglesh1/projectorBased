using System;
using System.Runtime.CompilerServices;
using System.Text;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core.Versioning
{
	[Serializable]
	public class Version : IVersion, IComparable<IVersion>, IEquatable<IVersion>, IJsonSerializable
	{
		public int Major;

		public int Minor;

		public int Patch;

		public Version(int major, int minor, int patch)
		{
			if (major < 0)
			{
				throw new ArgumentOutOfRangeException("major");
			}
			if (minor < 0)
			{
				throw new ArgumentOutOfRangeException("minor");
			}
			if (patch < 0)
			{
				throw new ArgumentOutOfRangeException("patch");
			}
			Major = major;
			Minor = minor;
			Patch = patch;
		}

		public Version(string version)
		{
			ParseInternal(version);
		}

		public Version()
		{
			Major = 0;
			Minor = 1;
			Patch = 0;
		}

		public Version(IVersion version)
		{
			Version version2 = version as Version;
			if (version2 != null)
			{
				Major = version2.Major;
				Minor = version2.Minor;
				Patch = version2.Patch;
			}
			else
			{
				Major = 0;
				Minor = 1;
				Patch = 0;
			}
		}

		public int CompareTo(IVersion version)
		{
			Version version2 = version as Version;
			if (version2 == this)
			{
				return 0;
			}
			if (version2 == null)
			{
				return 1;
			}
			if (Major == version2.Major)
			{
				if (Minor == version2.Minor)
				{
					if (Patch == version2.Patch)
					{
						return 0;
					}
					if (Patch <= version2.Patch)
					{
						return -1;
					}
					return 1;
				}
				if (Minor <= version2.Minor)
				{
					return -1;
				}
				return 1;
			}
			if (Major <= version2.Major)
			{
				return -1;
			}
			return 1;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Version);
		}

		public bool Equals(IVersion version)
		{
			Version version2 = version as Version;
			return version2 == this || (version2 != null && Major == version2.Major && Minor == version2.Minor && Patch == version2.Patch);
		}

		public override int GetHashCode()
		{
			return 0 | ((Major & 0xF) << 20) | ((Minor & 0xFF) << 12) | (Patch & 0xFFF);
		}

		public override string ToString()
		{
			return $"{Major}.{Minor}.{Patch}";
		}

		public void UpdatePatch()
		{
			Patch++;
		}

		public void UpdateMinor()
		{
			Minor++;
			Patch = 0;
		}

		public void UpdateMajor()
		{
			Major++;
			Minor = 0;
			Patch = 0;
		}

		public static Version Parse(string input)
		{
			Version version = new Version();
			version.ParseInternal(input);
			return version;
		}

		private void ParseInternal(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				throw new ArgumentNullException("input");
			}
			string[] array = input.Split(new char[1] { '.' });
			Major = int.Parse(array[0]);
			Minor = int.Parse(array[1]);
			Patch = int.Parse(array[2]);
		}

		public bool IsLower(IVersion version)
		{
			return CompareTo(version) < 0;
		}

		public bool IsHigher(IVersion version)
		{
			return CompareTo(version) > 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Version v1, Version v2)
		{
			if (v2 == null)
			{
				return v1 == null;
			}
			return v2 == v1 || v2.Equals(v1);
		}

		public static bool operator !=(Version v1, Version v2)
		{
			return !(v1 == v2);
		}

		public static bool operator <(Version v1, Version v2)
		{
			if (v1 == null)
			{
				return v2 != null;
			}
			return v1.CompareTo(v2) < 0;
		}

		public static bool operator <=(Version v1, Version v2)
		{
			return v1 == null || v1.CompareTo(v2) <= 0;
		}

		public static bool operator >(Version v1, Version v2)
		{
			return v2 < v1;
		}

		public static bool operator >=(Version v1, Version v2)
		{
			return v2 <= v1;
		}

		public string ToJson()
		{
			JsonNode jsonNode = ToJsonNode();
			StringBuilder stringBuilder = new StringBuilder();
			jsonNode.WriteToStringBuilder(stringBuilder, 2, 2, JsonTextMode.Indent);
			return stringBuilder.ToString();
		}

		public JsonNode ToJsonNode()
		{
			return new JsonString($"{Major}.{Minor}.{Patch}");
		}

		public void FromJson(JsonNode node)
		{
			ParseInternal(node.Value);
		}
	}
}
