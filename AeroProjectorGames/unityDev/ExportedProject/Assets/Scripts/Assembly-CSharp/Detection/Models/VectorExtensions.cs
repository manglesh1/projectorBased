using UnityEngine;

namespace Detection.Models
{
	public static class VectorExtensions
	{
		public static string ToFormattedString(this Vector2Int model)
		{
			return $"({model.x}, {model.y})";
		}
	}
}
