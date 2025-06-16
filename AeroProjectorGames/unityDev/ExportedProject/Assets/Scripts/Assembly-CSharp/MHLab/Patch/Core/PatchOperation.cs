using System;

namespace MHLab.Patch.Core
{
	[Serializable]
	public enum PatchOperation
	{
		Unchanged = 0,
		Deleted = 1,
		Updated = 2,
		ChangedAttributes = 3,
		Added = 4
	}
}
