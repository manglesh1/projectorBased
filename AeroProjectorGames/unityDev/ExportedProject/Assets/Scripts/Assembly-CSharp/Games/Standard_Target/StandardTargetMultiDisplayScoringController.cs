using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Games.Standard_Target
{
	public class StandardTargetMultiDisplayScoringController : MonoBehaviour
	{
		private bool _showKillzone;

		[Header("Scoring Ring Controllers")]
		[SerializeField]
		private List<ScoringRingController> onePointScoringRings;

		[SerializeField]
		private List<ScoringRingController> twoPointScoringRings;

		[SerializeField]
		private List<ScoringRingController> threePointScoringRings;

		[SerializeField]
		private List<ScoringRingController> fourPointScoringRings;

		[SerializeField]
		private List<ScoringRingController> fivePointScoringRings;

		[SerializeField]
		private List<ScoringRingController> sixPointScoringRings;

		[SerializeField]
		private ScoringRingController fiveRingKillzone;

		[SerializeField]
		private ScoringRingController sixRingKillzone;

		[Header("Game Ring Controllers")]
		[SerializeField]
		private List<RingController> onePointRingCollection;

		[SerializeField]
		private List<RingController> twoPointRingCollection;

		[SerializeField]
		private List<RingController> threePointRingCollection;

		[SerializeField]
		private List<RingController> fourPointRingCollection;

		[SerializeField]
		private List<RingController> fivePointRingCollection;

		[SerializeField]
		private List<RingController> sixPointRingCollection;

		[SerializeField]
		private List<RingController> killzoneRingCollection;

		[Header("Ring Sets")]
		[SerializeField]
		private GameObject fiveRingTarget;

		[SerializeField]
		private GameObject sixRingTarget;

		private void OnDisable()
		{
			onePointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered -= HandleOnePointRings;
			});
			twoPointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered -= HandleTwoPointRings;
			});
			threePointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered -= HandleThreePointRings;
			});
			fourPointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered -= HandleFourPointRings;
			});
			fivePointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered -= HandleFivePointRings;
			});
			sixPointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered -= HandleSixPointRings;
			});
			fiveRingKillzone.OnPointerDownTriggered -= HandleKillzoneRings;
			sixRingKillzone.OnPointerDownTriggered -= HandleKillzoneRings;
		}

		private void OnEnable()
		{
			onePointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered += HandleOnePointRings;
			});
			twoPointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered += HandleTwoPointRings;
			});
			threePointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered += HandleThreePointRings;
			});
			fourPointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered += HandleFourPointRings;
			});
			fivePointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered += HandleFivePointRings;
			});
			sixPointScoringRings.ForEach(delegate(ScoringRingController c)
			{
				c.OnPointerDownTriggered += HandleSixPointRings;
			});
			fiveRingKillzone.OnPointerDownTriggered += HandleKillzoneRings;
			sixRingKillzone.OnPointerDownTriggered += HandleKillzoneRings;
			fiveRingTarget.SetActive(!SettingsStore.Target.UseSixRingTarget);
			sixRingTarget.SetActive(SettingsStore.Target.UseSixRingTarget);
			fiveRingKillzone.gameObject.SetActive(value: false);
			sixRingKillzone.gameObject.SetActive(value: false);
		}

		public void SetKillzoneActive(bool isActive)
		{
			if (_showKillzone != isActive)
			{
				_showKillzone = isActive;
				if (SettingsStore.Target.UseSixRingTarget)
				{
					fiveRingKillzone.gameObject.SetActive(value: false);
					sixRingKillzone.gameObject.SetActive(isActive);
				}
				else
				{
					fiveRingKillzone.gameObject.SetActive(isActive);
					sixRingKillzone.gameObject.SetActive(value: false);
				}
			}
		}

		private void HandleOnePointRings()
		{
			TriggerRingHit(onePointRingCollection);
		}

		private void HandleSixPointRings()
		{
			TriggerRingHit(sixPointRingCollection);
		}

		private void HandleFivePointRings()
		{
			TriggerRingHit(fivePointRingCollection);
		}

		private void HandleFourPointRings()
		{
			TriggerRingHit(fourPointRingCollection);
		}

		private void HandleThreePointRings()
		{
			TriggerRingHit(threePointRingCollection);
		}

		private void HandleTwoPointRings()
		{
			TriggerRingHit(twoPointRingCollection);
		}

		private void HandleKillzoneRings()
		{
			TriggerRingHit(killzoneRingCollection);
		}

		private void TriggerRingHit(List<RingController> targetRings)
		{
			targetRings.ForEach(delegate(RingController ring)
			{
				if (ring.isActiveAndEnabled)
				{
					ring.SendMessage("Hit");
				}
			});
		}
	}
}
