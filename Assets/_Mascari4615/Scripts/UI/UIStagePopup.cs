using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIStagePopup : MonoBehaviour
	{
		[SerializeField] private UISlot slot;
		[SerializeField] private Animator animator;

		public void Popup(Stage stage)
		{
			slot.SetSlot(stage);
			animator.SetTrigger("POP");
		}
	}
}