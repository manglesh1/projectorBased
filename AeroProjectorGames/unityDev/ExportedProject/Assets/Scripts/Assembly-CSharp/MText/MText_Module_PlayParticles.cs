using System.Collections;
using UnityEngine;

namespace MText
{
	[CreateAssetMenu(menuName = "Modular 3d Text/Modules/Play Particle")]
	public class MText_Module_PlayParticles : MText_Module
	{
		[SerializeField]
		private GameObject particlePrefab;

		[SerializeField]
		private int destroyParticleAfter = 6;

		public override IEnumerator ModuleRoutine(GameObject obj, float delay)
		{
			yield return new WaitForSeconds(delay);
			if ((bool)obj && (bool)particlePrefab)
			{
				GameObject spawnedParticle = Object.Instantiate(particlePrefab);
				spawnedParticle.transform.position = obj.transform.position;
				spawnedParticle.transform.rotation = obj.transform.rotation;
				if ((bool)spawnedParticle.GetComponent<ParticleSystem>())
				{
					spawnedParticle.GetComponent<ParticleSystem>().Play();
				}
				yield return new WaitForSeconds(destroyParticleAfter);
				if ((bool)spawnedParticle)
				{
					Object.Destroy(spawnedParticle);
				}
			}
		}
	}
}
