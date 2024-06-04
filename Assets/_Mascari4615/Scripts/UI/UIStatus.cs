using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class UIStatus : MonoBehaviour, IUI
	{
		private List<UISlot> statusSlots;
		private bool isInit = false;

		public void Init()
		{
			statusSlots = GetComponentsInChildren<UISlot>(true).ToList();
			isInit = true;
		}

		public void UpdateUI()
		{
			List<UnitStatType> stats = Enum.GetValues(typeof(UnitStatType)).Cast<UnitStatType>().ToList();

			for (int i = 0; i < statusSlots.Count; i++)
			{
				if (i >= CountOf<UnitStatData>())
				{
					statusSlots[i].gameObject.SetActive(false);
				}
				else
				{
					statusSlots[i].gameObject.SetActive(true);

					UnitStatType targetType = stats[i];
					int curValue = Player.Instance.UnitStat[targetType];

					// Debug.Log($"UpdateUI: {targetType} - {curValue}");
					statusSlots[i].SetSlot(Get<UnitStatData>((int)targetType), curValue);
				}
			}
		}

		private IEnumerator Loop()
		{
			while (true)
			{
				UpdateUI();
				yield return new WaitForSeconds(TimeManager.TICK);
			}
		}

		private void OnEnable()
		{
			if (isInit)
				StartCoroutine(Loop());
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}