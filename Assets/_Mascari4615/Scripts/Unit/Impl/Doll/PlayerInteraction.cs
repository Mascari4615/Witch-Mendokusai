using UnityEngine;

namespace Mascari4615
{
	public class PlayerInteraction
	{
		public const float InteractionDistance = 1.5f;

		private readonly Transform player;
		public PlayerInteraction(Transform player)
		{
			this.player = player;
		}

		public void TryInteraction()
		{
			InteractiveObject nearest = MHelper.GetNearest(InteractiveObject.ActiveInteractives, player.position, InteractionDistance);
			if (nearest == null)
				return;
			nearest.GetComponent<InteractiveObject>().Interact();
		}
	}
}