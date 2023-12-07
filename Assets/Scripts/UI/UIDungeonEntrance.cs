using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mascari4615
{
	public class UIDungeonEntrance : MonoBehaviour
	{
		[SerializeField] private GameObject[] dungeonSelectButtons;
		private int _curDungeonIndex = 0;
		private List<Dungeon> _dungeonDatas;

		[SerializeField] private TextMeshProUGUI dungeonNameText;
		[SerializeField] private TextMeshProUGUI dungeonDescriptionText;

		public void OpenCanvas(List<Dungeon> dungeonDatas)
		{
			Debug.Log(nameof(OpenCanvas));
			gameObject.SetActive(true);
			for (int i = 0; i < dungeonSelectButtons.Length; i++)
				dungeonSelectButtons[i].gameObject.SetActive(dungeonDatas.Count > i);

			this._dungeonDatas = dungeonDatas;
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

		public void StartCombat()
		{
			CombatManager.Instance.StartCombat(_dungeonDatas[_curDungeonIndex]);
			gameObject.SetActive(false);
		}
	}
}