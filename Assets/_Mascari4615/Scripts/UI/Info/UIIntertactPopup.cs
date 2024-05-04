using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIIntertactPopup : MonoBehaviour
	{
		[SerializeField] private GameObject interactPopup;

		private void Start()
		{
			TimeManager.Instance.RegisterCallback(UpdatePopup);
		}

		public void UpdatePopup()
		{
			Vector3 playerPos = Player.Instance.transform.position;
			float interactDistance = PlayerInteraction.InteractionDistance;

			GameObject nearestInteractive = ObjectBufferManager.Instance.GetNearestObject(ObjectType.Interactive, playerPos, interactDistance);
			
			interactPopup.SetActive(nearestInteractive != null);
		}
	}
}