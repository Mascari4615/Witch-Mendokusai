using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class QuestObject : MonoBehaviour
	{
		private Quest quest;

		public void Init(Quest quest)
		{
			this.quest = quest;

			foreach (GameEvent gameEvent in quest.GameEvents)
				gameEvent.AddCallback(CheckComplete);
		}

		public void CheckComplete()
		{
			foreach (Criteria criteria in quest.Criterias)
				if (criteria.HasComplete() == false)
					return;

			DataManager.Instance.QuestDic[quest.ID].SetComplete();
			gameObject.SetActive(false);
			UIManager.Instance.Popup(quest);
		}

		private void OnDisable()
		{
			if (quest != null)
				foreach (GameEvent gameEvent in quest.GameEvents)
					gameEvent.RemoveCallback(CheckComplete);
		}
	}
}