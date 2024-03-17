using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Mascari4615
{
	public class NPC : InteractiveObject
	{
		[SerializeField] private MPanelType overlayUI;

		public override void Interact()
		{
			if (overlayUI == MPanelType.Chat)
				UIManager.Instance.Chat.SetNPC(transform);

			UIManager.Instance.SetOverlay(overlayUI);
		}
	}
}