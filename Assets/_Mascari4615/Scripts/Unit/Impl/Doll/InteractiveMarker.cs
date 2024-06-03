using UnityEngine;

namespace Mascari4615

{
	public class InteractiveMarker : MonoBehaviour
	{
		private Animator animator;
		private InteractiveObject lastNearest;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Update()
		{
			InteractiveObject nearest = MHelper.GetNearest(InteractiveObject.ActiveInteractives, Player.Instance.transform.position, PlayerInteraction.InteractionDistance);
			
			if (nearest == null)
			{
				animator.SetBool("ON", false);
				return;
			}

			if (lastNearest != nearest)
			{
				lastNearest = nearest;
				animator.SetTrigger("RESET");
			}

			animator.SetBool("ON", true);
			transform.position = nearest.transform.position;
		}
	}
}