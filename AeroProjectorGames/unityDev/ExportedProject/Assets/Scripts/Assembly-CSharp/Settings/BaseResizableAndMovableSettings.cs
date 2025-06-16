using System;
using UnityEngine;

namespace Settings
{
	[Serializable]
	public abstract class BaseResizableAndMovableSettings : ISettings
	{
		[SerializeField]
		private float height;

		[SerializeField]
		private float width;

		[SerializeField]
		private float positionX;

		[SerializeField]
		private float positionY;

		public float Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		public float PositionX
		{
			get
			{
				return positionX;
			}
			set
			{
				positionX = value;
			}
		}

		public float PositionY
		{
			get
			{
				return positionY;
			}
			set
			{
				positionY = value;
			}
		}

		public abstract SettingsKey StorageKey { get; }

		public void Save()
		{
			SettingsStore.Set(this);
		}

		public abstract void SetDefaults();
	}
}
