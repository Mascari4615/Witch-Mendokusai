using UnityEngine;

namespace Mascari4615
{
	[RequireComponent(typeof(UICraft))]
	public class UIItemUpgrade : UIPanel
	{
		private UICraft craft;

		public override void Init()
		{
			craft = GetComponent<UICraft>();
			craft.Init();
		}

		public override void UpdateUI()
		{
			craft.UpdateUI();
		}
	}
}