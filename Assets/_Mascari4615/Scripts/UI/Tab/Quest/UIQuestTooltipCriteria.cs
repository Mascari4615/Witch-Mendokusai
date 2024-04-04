using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class UIQuestTooltipCriteria : MonoBehaviour
	{
		private UISlot[] slots;

		public void Init()
		{
			slots = GetComponentsInChildren<UISlot>(true);

			foreach (UISlot slot in slots)
			{
				slot.Init();
				slot.gameObject.SetActive(false);
			}
		}

		public void SetCriteria(Quest quest)
		{
			if (quest == null || quest.Criterias == null)
			{
				foreach (UISlot slot in slots)
					slot.gameObject.SetActive(false);
				return;
			}

			List<RuntimeCriteria> criterias = quest.Criterias;

			for (int i = 0; i < slots.Length; i++)
			{
				if (i < criterias.Count)
				{
					slots[i].gameObject.SetActive(true);

					Sprite criteriaSprite = null;
					string criteriaName = "";
					string criteriaDesc = "";

					switch (criterias[i].Criteria)
					{
						case IntCriteria intCriteria:
							// HACK, TODO: Refactor
							criteriaSprite = SOManager.Instance.VQExp.Sprite;
							criteriaName = intCriteria.IntVariable.Name;
							break;
						case ItemCountCriteria itemCountCriteria:
							criteriaSprite = GetItemData(itemCountCriteria.ItemID).Sprite;
							criteriaName = GetItemData(itemCountCriteria.ItemID).Name;
							break;
						case StatCriteria statCriteria:
							// HACK
							criteriaSprite = SOManager.Instance.VQExp.Sprite;
							criteriaName = statCriteria.Type.ToString();
							break;
						case StatisticsCriteria statisticsCriteria:
							// HACK
							criteriaSprite = SOManager.Instance.VQExp.Sprite;
							criteriaName = statisticsCriteria.Type.ToString();
							break;
					}

					if (criterias[i].Criteria is NumCriteria numCriteria)
					{
						criteriaDesc = $"{numCriteria.GetCurValue()}/{numCriteria.GetTargetValue()} ({numCriteria.GetProgress() * 100}%)";
					}

					slots[i].SetSlot(criteriaSprite, criteriaName, criteriaDesc);
				}
				else
				{
					slots[i].gameObject.SetActive(false);
				}
			}
		}
	}
}