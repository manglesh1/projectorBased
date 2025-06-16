using System;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core.Versioning
{
	public interface IVersion : IComparable<IVersion>, IEquatable<IVersion>, IJsonSerializable
	{
		new string ToString();

		void UpdatePatch();

		void UpdateMinor();

		void UpdateMajor();

		bool IsLower(IVersion compare);

		bool IsHigher(IVersion version);
	}
}
