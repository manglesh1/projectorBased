using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Detection.Models
{
	public static class VectorListExtensions
	{
		public static Vector3Int MaxDepth(this IEnumerable<Vector3Int> model)
		{
			Vector3Int[] array = model.ToArray();
			if (array.Length != 0)
			{
				return new Vector3Int(array[0].x, array[0].y, array.Select(delegate(Vector3Int v)
				{
					Vector3Int vector3Int = v;
					return vector3Int.z;
				}).Max());
			}
			return Vector3Int.zero;
		}

		public static Vector3Int MinDepth(this IEnumerable<Vector3Int> model)
		{
			Vector3Int[] frames = model.ToArray();
			if (frames.Length != 0)
			{
				return frames.FirstOrDefault((Vector3Int v) => v.z == frames.Select(delegate(Vector3Int vector3Int2)
				{
					Vector3Int vector3Int = vector3Int2;
					return vector3Int.z;
				}).Min());
			}
			return Vector3Int.zero;
		}

		public static Vector3Int Median(this List<Vector3Int> model)
		{
			Vector3Int[] array = model.Where(delegate(Vector3Int v)
			{
				Vector3Int vector3Int = v;
				return vector3Int.z > 0;
			}).OrderBy(delegate(Vector3Int v)
			{
				Vector3Int vector3Int = v;
				return vector3Int.z;
			}).ToArray();
			if (!array.Any())
			{
				return Vector3Int.zero;
			}
			return array.ElementAt(array.Length / 2);
		}

		public static byte[] ToDepthByteArray(this Vector3Int[] model)
		{
			return model.Select(delegate(Vector3Int p)
			{
				Vector3Int vector3Int = p;
				return (byte)vector3Int.z;
			}).ToArray();
		}
	}
}
