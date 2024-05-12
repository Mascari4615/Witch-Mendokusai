using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mascari4615
{
	public class UIDungeonResult : UIPanel
	{
		// Stat
		[SerializeField] private TextMeshProUGUI playTimeText;
		[SerializeField] private TextMeshProUGUI levelText;
		[SerializeField] private TextMeshProUGUI killCountText;
		[SerializeField] private TextMeshProUGUI bossKillCountText;
		[SerializeField] private TextMeshProUGUI nyangText;

		// Info
		private UICardDataGrid cardGrid;

		public void Continue()
		{
			DungeonManager.Instance.Continue();
		}

		public override void Init()
		{
			cardGrid = GetComponentInChildren<UICardDataGrid>(true);
			cardGrid.Init();
		}

		public override void UpdateUI()
		{
			DungeonRecord record = DungeonManager.Instance.Result;

			playTimeText.text = record.PlayTime.ToString(@"mm\:ss");
			levelText.text = record.Level.ToString();
			killCountText.text = record.KillCount.ToString();
			bossKillCountText.text = record.BossKillCount.ToString();
			nyangText.text = record.Nyang.ToString();

			cardGrid.UpdateUI();
		}
	}
}