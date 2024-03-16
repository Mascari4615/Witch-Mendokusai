using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mascari4615
{
	public class UIDungeonEntrance : UIPanel
	{
		[SerializeField] private GameObject[] dungeonSelectButtons;
		private int _curDungeonIndex = 0;
		private List<Dungeon> _dungeonDatas;

		[SerializeField] private TextMeshProUGUI dungeonNameText;
		[SerializeField] private TextMeshProUGUI dungeonDescriptionText;

		public override void UpdateUI(int[] someData = null)
		{
			if (someData == null)
				return;

			_dungeonDatas = new();
			for (int i = 0; i < someData.Length; i++)
			{
				if (DataManager.Instance.DungeonDic.TryGetValue(someData[i], out Dungeon dungeon))
					_dungeonDatas.Add(dungeon);
			}

			for (int i = 0; i < dungeonSelectButtons.Length; i++)
				dungeonSelectButtons[i].SetActive(_dungeonDatas.Count > i);

			SelectDungeon(0);
		}

		public void SelectDungeon(int index)
		{
			_curDungeonIndex = index;
			UpdateDungeonPanel();
		}

		private void UpdateDungeonPanel()
		{
			Dungeon curDungeon = _dungeonDatas[_curDungeonIndex];

			dungeonNameText.text = curDungeon.Name;
			dungeonDescriptionText.text = curDungeon.Description;
		}

		public void EnterTheDungeon()
		{
			DungeonManager.Instance.StartDungeon(_dungeonDatas[_curDungeonIndex]);
		}
	}
}