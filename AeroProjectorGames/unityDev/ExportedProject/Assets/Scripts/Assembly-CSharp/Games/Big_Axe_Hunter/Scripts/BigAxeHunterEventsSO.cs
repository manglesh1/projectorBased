using UnityEngine;
using UnityEngine.Events;

namespace Games.Big_Axe_Hunter.Scripts
{
	[CreateAssetMenu(menuName = "Games/BigAxeHunter/BigAxeHunterEvents")]
	public class BigAxeHunterEventsSO : ScriptableObject
	{
		public event UnityAction<ViewPosition> OnCameraMovedTo;

		public event UnityAction<ViewPosition> OnCameraMovingAwayFrom;

		public event UnityAction<int> OnHit;

		public event UnityAction OnHitOrMiss;

		public event UnityAction<ViewPosition> OnLoadAnimalsAtPosition;

		public event UnityAction<ViewPosition, SpawnPoint> OnLoadAnimalRequest;

		public event UnityAction<Camera> OnSendGameboardCameraToGame;

		public event UnityAction OnUndo;

		public event UnityAction<ViewPosition> OnUnloadAnimalsAtPosition;

		public event UnityAction<SpawnPoint, Sprite> OnMultiDisplayLoadAnimalIconForSpawnPoint;

		public event UnityAction<SpawnPoint> OnMultiDisplaySpawnPointHit;

		public void RaiseCameraMovedTo(ViewPosition viewPosition)
		{
			this.OnCameraMovedTo?.Invoke(viewPosition);
		}

		public void RaiseCameraMovingAwayFrom(ViewPosition viewPosition)
		{
			this.OnCameraMovingAwayFrom?.Invoke(viewPosition);
		}

		public void RaiseHitOrMiss()
		{
			this.OnHitOrMiss?.Invoke();
		}

		public void RaiseLoadAnimalsForPosition(ViewPosition viewPosition)
		{
			this.OnLoadAnimalsAtPosition?.Invoke(viewPosition);
		}

		public void RaiseLoadAnimalsForPosition(ViewPosition viewPosition, SpawnPoint spawnPoint)
		{
			this.OnLoadAnimalRequest?.Invoke(viewPosition, spawnPoint);
		}

		public void RaiseMultiDisplaySpawnPointHit(SpawnPoint spawnPoint)
		{
			this.OnMultiDisplaySpawnPointHit?.Invoke(spawnPoint);
		}

		public void RaiseMultiDisplayLoadAnimalIconForSpawnPoint(SpawnPoint spawnPoint, Sprite animalIcon)
		{
			this.OnMultiDisplayLoadAnimalIconForSpawnPoint?.Invoke(spawnPoint, animalIcon);
		}

		public void RaiseOnHit(int score)
		{
			this.OnHit?.Invoke(score);
		}

		public void RaiseSendGameboardCameraToGame(Camera camera)
		{
			this.OnSendGameboardCameraToGame?.Invoke(camera);
		}

		public void RaiseUndo()
		{
			this.OnUndo?.Invoke();
		}

		public void RaiseUnloadAnimalsForPosition(ViewPosition viewPosition)
		{
			this.OnUnloadAnimalsAtPosition?.Invoke(viewPosition);
		}
	}
}
