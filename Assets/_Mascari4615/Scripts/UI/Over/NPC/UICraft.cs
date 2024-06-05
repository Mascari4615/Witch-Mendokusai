using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mascari4615
{
	public class UICraft : MonoBehaviour, IUI
	{
		[SerializeField] private ItemType itemType;
		[SerializeField] private RecipeType recipeType;

		[SerializeField] private TextMeshProUGUI percentageText;
		[SerializeField] private TextMeshProUGUI priceText;

		[SerializeField] private UIItemSlot[] craftTableSlots;
		[SerializeField] private TextMeshProUGUI[] craftTableAmounts;
		[SerializeField] private UIItemSlot resultSlot;

		[SerializeField] private UIItemDataGrid recipeGrid;

		private void OnEnable()
		{
			// Debug.Log($"{nameof(UICraft)} {nameof(OnEnable)}");
			StartCoroutine(Loop());
		}

		private void OnDisable()
		{
			// Debug.Log($"{nameof(UICraft)} {nameof(OnDisable)}");
			StopAllCoroutines();
		}

		private IEnumerator Loop()
		{
			WaitForSeconds wait = new(TimeManager.TICK);

			UpdateGrid();
			recipeGrid.SelectSlot(0);
			recipeGrid.CurSlot.OnSelect(null);

			while (true)
			{
				// Debug.Log($"{nameof(UICraft)} {nameof(Loop)}");
				UpdateUI();
				yield return wait;
			}
		}

		public void Init()
		{
			resultSlot.Init();
			recipeGrid.Init();
		}

		public void UpdateUI()
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
				if (itemData.Type != itemType)
					continue;

				if (itemData.Recipes.Count == 0)
					continue;

				if (itemData.Recipes[0].Type != recipeType)
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
					craftTableSlots[i].SetSlot(ingredientInfo.ItemData, ingredientInfo.Amount);

					// 인벤토리에 있는 해당 아이템의 양
					int amount = SOManager.Instance.ItemInventory.GetItemAmount(ingredientInfo.ItemData.ID);
					craftTableAmounts[i].text = $"{(amount > ingredientInfo.Amount ? "<color=white>" : "<color=red>")}{amount}</color>";
					craftTableAmounts[i].text += $"/{ingredientInfo.Amount}";
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
			{
				UIManager.Instance.PopText("레시피를 선택해주세요.", TextType.Warning);
				return;
			}

			ItemData itemData = recipeGrid.Datas[recipeGrid.CurSlotIndex];
			Recipe recipe = itemData.Recipes[0];

			// Has Ingredients
			foreach (IngredientInfo ingredientInfo in recipe.Ingredients)
			{
				int inventoryAmount = SOManager.Instance.ItemInventory.GetItemAmount(ingredientInfo.ItemData.ID);
				if (inventoryAmount < ingredientInfo.Amount)
				{
					UIManager.Instance.PopText($"제작에 필요한 재료가 부족합니다. ({ingredientInfo.ItemData.Name})", TextType.Warning);
					return;
				}
			}

			// Check Nyang
			int recipePrice = recipe.priceNyang;
			if (recipePrice > SOManager.Instance.Nyang.RuntimeValue)
			{
				int diff = recipePrice - SOManager.Instance.Nyang.RuntimeValue;
				UIManager.Instance.PopText($"제작에 필요한 냥이 부족합니다. ({diff}냥)", TextType.Warning);
			}

			// Craft
			// 1. Remove Ingredients
			foreach (IngredientInfo ingredientInfo in recipe.Ingredients)
			{
				int remain = ingredientInfo.Amount;

				while (remain > 0)
				{
					int slotIndex = SOManager.Instance.ItemInventory.FindItemIndex(ingredientInfo.ItemData);

					Item item = SOManager.Instance.ItemInventory.GetItem(slotIndex);
					int slotAmount = item.Amount;

					if (slotAmount > remain)
					{
						SOManager.Instance.ItemInventory.SetItemAmount(slotIndex, slotAmount - remain);
						break;
					}
					else
					{
						SOManager.Instance.ItemInventory.Remove(slotIndex);
						remain -= slotAmount;
					}
				}
			}
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
				SOManager.Instance.ItemInventory.Add(itemData, 1);
			}

			UpdateUI();
		}
	}
}