using UnityEngine;
using UnityEngine.UI;

namespace Detection.Controllers
{
	public class StreamTextureController : MonoBehaviour
	{
		[SerializeField]
		private bool showTexture;

		[SerializeField]
		private RawImage streamTexture;

		private void OnEnable()
		{
			streamTexture.enabled = showTexture;
		}
	}
}
