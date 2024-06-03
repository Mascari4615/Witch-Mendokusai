using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static Mascari4615.SOHelper;
using Random = UnityEngine.Random;

namespace Mascari4615
{
	public class UIItemCraft : UIPanel
	{
		[SerializeField] private RecipeType recipeType;

		[SerializeField] private TextMeshProUGUI percentageText;
		[SerializeField] private TextMeshProUGUI priceText;

		[SerializeField] private UIItemSlot[] craftTableSlots;
		[SerializeField] private UIItemSlot resultSlot;
		[SerializeField] private Inventory craftTableInventory;
		[SerializeField] private UIItemGrid itemInventoryUI;
		[SerializeField] private UIItemGrid craftTableInventoryUI;

		public void TryCraft()
		{
			// Make Recipe
			List<int> recipeToList = new(craftTableSlots.Length);
			foreach (UIItemSlot slot in craftTableSlots)
			{
				if (slot.DataSO)
					recipeToList.Add(craftTableInventory.GetItem(slot.Index).Data.ID);
			}
			recipeToList.Sort();
			string recipeString = RecipeUtil.RecipeToString(recipeType, recipeToList);

			// Find Recipe
			if (DataManager.Instance.CraftDic.ContainsKey(recipeString) == false)
			{
				UIManager.Instance.PopText("조합식이 유효하지 않습니다.", TextType.Warning);
				return;
			}

			(Recipe recipe, int itemID) = DataManager.Instance.CraftDic[recipeString];
			int recipePrice = recipe.priceNyang;

			// Check Nyang
			if (recipePrice > SOManager.Instance.Nyang.RuntimeValue)
				UIManager.Instance.PopText($"제작에 필요한 냥이 부족합니다. ({recipePrice - SOManager.Instance.Nyang.RuntimeValue}냥)", TextType.Warning);

			ItemData resultItemData = GetItemData(itemID);

			// Check Result Slot
			if (resultSlot.DataSO)
			{
				if (resultSlot.DataSO.ID != resultItemData.ID)
				{
					UIManager.Instance.PopText("결과 슬롯을 비워주세요.", TextType.Warning);
					return;
				}
			}

			// Remove Ingredients
			foreach (UIItemSlot slot in craftTableSlots)
			{
				if (slot.DataSO)
					craftTableInventory.Remove(slot.Index);
			}
			SOManager.Instance.Nyang.RuntimeValue -= recipePrice;
			UIManager.Instance.PopText($"- {recipePrice}", TextType.Warning);

			// Craft
			if (Random.Range(0, 100) > recipe.Percentage)
			{
				// Fail
				UIManager.Instance.PopText("제작 실패 !", TextType.Warning);
			}
			else
			{
				// Success
				if (resultSlot.DataSO && resultSlot.DataSO.ID == resultItemData.ID)
				{
					craftTableInventory.SetItemAmount(resultSlot.Index, craftTableInventory.GetItem(resultSlot.Index).Amount + 1);
				}
				else
				{
					Item newItem = new(Guid.NewGuid(), resultItemData);
					craftTableInventory.SetItem(resultSlot.Index, newItem);
				}

				UIManager.Instance.PopText("제작 성공 !", TextType.Heal);
			}

			UpdateUI();
		}

		public override void Init()
		{
			itemInventoryUI.Init();
			craftTableInventoryUI.Init();
			resultSlot.Init();
		}

		public override void UpdateUI()
		{
			itemInventoryUI.UpdateUI();
			craftTableInventoryUI.UpdateUI();
			resultSlot.UpdateUI();
		}

		private void Update()
		{
			UpdateCraftInfo();
		}



		private void UpdateCraftInfo()
		{
			if (percentageText == null || priceText == null)
				return;

			List<int> recipeToList = new(craftTableSlots.Length);
			foreach (UIItemSlot slot in craftTableSlots)
			{
				if (slot.DataSO)
					recipeToList.Add(craftTableInventory.GetItem(slot.Index).Data.ID);
			}
			recipeToList.Sort();
			string recipeString = RecipeUtil.RecipeToString(recipeType, recipeToList);

			if (DataManager.Instance.CraftDic.ContainsKey(recipeString) == false)
			{
				percentageText.text = "_";
				priceText.text = "_";
				return;
			}

			(Recipe recipe, _) = DataManager.Instance.CraftDic[recipeString];

			percentageText.text = $"{recipe.Percentage}%";
			priceText.text = recipe.priceNyang.ToString();
		}
	}
}