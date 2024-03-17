using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Mascari4615
{
	public class UINPC : UIPanel
	{
		private NPC curNPC;

		public override void OnOpen()
		{
			UIManager.Instance.Chat.StartChat(curNPC, () => { });
		}

		public override void OnClose()
		{
		}

		public override void UpdateUI()
		{
		}

		public void SetNPC(NPC npc)
		{
			curNPC = npc;
		}
	}
}