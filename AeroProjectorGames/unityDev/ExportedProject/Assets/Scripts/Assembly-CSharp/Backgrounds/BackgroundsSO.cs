using System.Collections.Generic;
using UnityEngine;

namespace Backgrounds
{
	[CreateAssetMenu(menuName = "Backgrounds/Runtime Background Container")]
	public class BackgroundsSO : ScriptableObject
	{
		private Dictionary<string, Color> _loadedBackgroundColors = new Dictionary<string, Color>();

		private Dictionary<string, Texture2D> _loadedBackgroundImages = new Dictionary<string, Texture2D>();

		public IReadOnlyDictionary<string, Color> LoadedBackgroundColors => _loadedBackgroundColors;

		public IReadOnlyDictionary<string, Texture2D> LoadedBackgroundImages => _loadedBackgroundImages;

		public void AddColor(string key, Color color)
		{
			if (!_loadedBackgroundColors.ContainsKey(key))
			{
				_loadedBackgroundColors.Add(key, color);
			}
		}

		public void AddBackground(string key, Texture2D texture)
		{
			if (!_loadedBackgroundImages.ContainsKey(key))
			{
				_loadedBackgroundImages.Add(key, texture);
			}
		}

		public void RemoveBackground(string key)
		{
			if (_loadedBackgroundImages.ContainsKey(key))
			{
				_loadedBackgroundImages.Remove(key);
			}
		}
	}
}
