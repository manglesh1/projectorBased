using UnityEngine;
using UnityEngine.UI;

namespace Helpers
{
	public static class RatioSizeHelpersr
	{
		public static Vector2 GetImageSizeToRetainSpriteProportions(Image imageObj, Sprite spriteObj)
		{
			float height = imageObj.rectTransform.rect.height;
			float width = imageObj.rectTransform.rect.width;
			float height2 = spriteObj.rect.height;
			float width2 = spriteObj.rect.width;
			float num = height2 - height;
			float num3;
			float num4;
			if (width2 - width > num)
			{
				float num2 = width2 / width;
				num3 = height2 / num2;
				if (num3 > height)
				{
					num4 = width - (num3 - height);
					num3 = height;
				}
				else
				{
					num4 = width;
				}
			}
			else
			{
				float num5 = height2 / height;
				num4 = width2 / num5;
				if (num4 > width)
				{
					num3 = height - (num4 - width);
					num4 = width;
				}
				else
				{
					num3 = height;
				}
			}
			return new Vector2(num4, num3);
		}
	}
}
