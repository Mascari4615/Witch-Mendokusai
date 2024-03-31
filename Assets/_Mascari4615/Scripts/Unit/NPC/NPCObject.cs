using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Mascari4615
{
	public class NPCObject : UnitObject, IInteractable
	{
		public void OnInteract()
		{
			UIManager.Instance.Npc.SetNPC(this);
			UIManager.Instance.SetOverlay(MPanelType.NPC);
		}
	}
}