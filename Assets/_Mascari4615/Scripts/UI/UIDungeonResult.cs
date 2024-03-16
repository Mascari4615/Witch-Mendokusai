using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIDungeonResult : UIPanel
	{
		public void Continue()
		{
			DungeonManager.Instance.Continue();
		}

		public override void UpdateUI(int[] someData = null)
		{
		}
	}
}