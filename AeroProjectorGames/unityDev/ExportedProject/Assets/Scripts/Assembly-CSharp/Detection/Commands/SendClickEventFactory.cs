using System;
using Detection.Models;
using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/SendClickEventFactory")]
	public class SendClickEventFactory : ScriptableObject
	{
		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private SendClickEventToButtonStrategy buttonStrategy;

		[SerializeField]
		private SendClickEventToColliderStrategy colliderStrategy;

		[SerializeField]
		private SendClickEventToEventTriggerStrategy eventTriggerStrategy;

		[SerializeField]
		private SendClickEventToSpriteStrategy spriteStrategy;

		[SerializeField]
		private SendClickEventToOnPointerClickUpwardsStrategy onPointerClickUpwardsStrategy;

		public void SendClick(GameObject proxiedObject, PointerEventData pointerData)
		{
			switch (gameState.ClickableObjectType)
			{
			case GameClickableObjectTypeEnum.Button:
				buttonStrategy.SendClick(proxiedObject, pointerData);
				break;
			case GameClickableObjectTypeEnum.EventTrigger:
				eventTriggerStrategy.SendClick(proxiedObject, pointerData);
				break;
			case GameClickableObjectTypeEnum.Sprite:
				spriteStrategy.SendClick(proxiedObject, pointerData);
				break;
			case GameClickableObjectTypeEnum.Collider:
				colliderStrategy.SendClick(proxiedObject, pointerData);
				break;
			case GameClickableObjectTypeEnum.OnPointerClickUpwards:
				onPointerClickUpwardsStrategy.SendClick(proxiedObject, pointerData);
				break;
			default:
				throw new InvalidOperationException("Unknown clickable object type");
			}
		}
	}
}
