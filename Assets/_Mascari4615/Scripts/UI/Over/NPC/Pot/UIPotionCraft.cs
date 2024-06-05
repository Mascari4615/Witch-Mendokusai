using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static Mascari4615.SOHelper;
using Random = UnityEngine.Random;

namespace Mascari4615
{
	public class UIPotionCraft : UIPanel
	{
		[SerializeField] private RecipeType recipeType;

		[SerializeField] private TextMeshProUGUI percentageText;
		[SerializeField] private TextMeshProUGUI priceText;

		[SerializeField] private UIItemSlot[] craftTableSlots;
		[SerializeField] private TextMeshProUGUI[] craftTableAmounts;
		[SerializeField] private UIItemSlot resultSlot;

		[SerializeField] private UIItemDataGrid recipeGrid;

		private void OnEnable()
		{
			StartCoroutine(Loop());
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		private IEnumerator Loop()
		{
			WaitForSeconds wait = new(TimeManager.TICK);

			UpdateGrid();
			recipeGrid.SelectSlot(0);
			recipeGrid.CurSlot.Select();

			while (true)
			{
				UpdateUI();
				yield return wait;
			}
		}

		public override void Init()
		{
			resultSlot.Init();
			recipeGrid.Init();
		}

		public override void UpdateUI()
		{
			resultSlot.UpdateUI();
			UpdateGrid();
			UpdateTooltip();
		}

		private void UpdateGrid()
		{
			Dictionary<int, bool> HasRecipe = DataManager.Instance.HasRecipe;
			List<ItemData> recipes = new();

			foreach (ItemData itemData in SOManager.Instance.DataSOs[typeof(ItemData)].Values.Cast<ItemData>())
			{
				if (itemData.Type != ItemType.Potion)
					continue;

				if (itemData.Recipes.Count == 0)
					continue;

				if (HasRecipe.ContainsKey(itemData.ID) && HasRecipe[itemData.ID])
					recipes.Add(itemData);
			}

			recipeGrid.SetDatas(recipes);
			recipeGrid.UpdateUI();
		}

		private void UpdateTooltip()
		{
			if (percentageText == null || priceText == null)
				return;

			if (recipeGrid.CurSlotIndex < 0 || recipeGrid.CurSlotIndex >= recipeGrid.Datas.Count)
			{
				percentageText.text = "_";
				priceText.text = "_";
				return;
			}

			ItemData itemData = recipeGrid.Datas[recipeGrid.CurSlotIndex];
			Recipe recipe = itemData.Recipes[0];

			percentageText.text = $"{recipe.Percentage}%";
			priceText.text = $"{recipe.priceNyang}냥";

			for (int i = 0; i < craftTableSlots.Length; i++)
			{
				if (recipe.Ingredients.Count > i)
				{
					IngredientInfo ingredientInfo = recipe.Ingredients[i];
					craftTableSlots[i].SetSlot(ingredientInfo.ItemData, ingredientInfo.Count);

					// 인벤토리에 있는 해당 아이템의 양
					int amount = SOManager.Instance.ItemInventory.GetItemAmount(ingredientInfo.ItemData.ID);
					craftTableAmounts[i].text = $"{(amount > ingredientInfo.Count ? "<color=white>" : "<color=red>")}{amount}</color>";
					craftTableAmounts[i].text += $"/{ingredientInfo.Count}";
				}
				else
				{
					craftTableSlots[i].SetSlot(null);
				}
			}

			resultSlot.SetSlot(itemData);
		}

		public void TryCraft()
		{
			// Check Recipe
			if (recipeGrid.CurSlot == null)
				return;

			ItemData itemData = recipeGrid.Datas[recipeGrid.CurSlotIndex];
			Recipe recipe = itemData.Recipes[0];

			// Has Ingredients

			// Check Nyang
			int recipePrice = recipe.priceNyang;
			if (recipePrice > SOManager.Instance.Nyang.RuntimeValue)
			{
				int diff = recipePrice - SOManager.Instance.Nyang.RuntimeValue;
				UIManager.Instance.PopText($"제작에 필요한 냥이 부족합니다. ({diff}냥)", TextType.Warning);
			}

			// Check Result Slot
			ItemData resultItemData = GetItemData(itemData.ID);
			if (resultSlot.DataSO)
			{
				if (resultSlot.DataSO.ID != resultItemData.ID)
				{
					UIManager.Instance.PopText("결과 슬롯을 비워주세요.", TextType.Warning);
				}
				else if (resultSlot.Amount + 1 >= (resultSlot.DataSO as ItemData).MaxAmount)
				{
					UIManager.Instance.PopText("결과 슬롯을 비워주세요.", TextType.Warning);
				}
				return;
			}

			// Craft
			// 1. Remove Ingredients
			
			SOManager.Instance.Nyang.RuntimeValue -= recipePrice;
			UIManager.Instance.PopText($"- {recipePrice}", TextType.Warning);

			// 2. Craft
			if (Random.Range(0, 100) > recipe.Percentage)
			{
				// Fail
				Reward.GetReward(recipe.FailureRewards);
				UIManager.Instance.PopText("제작 실패 !", TextType.Warning);
			}
			else
			{
				// Success
				Reward.GetReward(recipe.SuccessRewards);
				UIManager.Instance.PopText("제작 성공 !", TextType.Heal);

				if (resultSlot.DataSO && resultSlot.DataSO.ID == resultItemData.ID)
				{
					// craftTableInventory.SetItemAmount(resultSlot.Index, craftTableInventory.GetItem(resultSlot.Index).Amount + 1);
				}
				else
				{
				// 	Item newItem = new(Guid.NewGuid(), resultItemData);
					//craftTableInventory.SetItem(resultSlot.Index, newItem);
				}
			}

			UpdateUI();
		}

	}
}