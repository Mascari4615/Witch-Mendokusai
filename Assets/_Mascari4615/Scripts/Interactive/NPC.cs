using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Mascari4615
{
	public class NPC : InteractiveObject
	{
		[SerializeField] private OverlayUI overlayUI;

		public override void Interact()
		{
			UIManager.Instance.SetOverlayUI(overlayUI);
		}
	}
}