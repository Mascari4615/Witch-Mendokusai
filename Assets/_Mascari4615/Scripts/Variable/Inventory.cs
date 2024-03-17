using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "Inventory", menuName = "GameSystem/RunTimeSet/Inventory")]
	public class Inventory : DataBuffer<Item>, ISerializationCallbackReceiver
	{
		private const int NONE = -1;
		private const int DefaultCapacity = 30;
		public int Capacity { get; private set; } = DefaultCapacity;
		[NonSerialized] private readonly List<UIItemInventory> inventoryUIs = new();

		public void RegisterInventoryUI(UIItemInventory uiItemInventory)
		{
			inventoryUIs.Add(uiItemInventory);
		}

		private int FindEmptySlotIndex(int startIndex = 0)
		{
			for (int i = startIndex; i < Capacity; i++)
			{
				if (RuntimeItems[i] == null)
					return i;
			}
			return NONE;
		}

		public int FindItemSlotIndex(ItemData target, int startIndex = 0)
		{
			for (int i = startIndex; i < Capacity; i++)
			{
				Item current = RuntimeItems[i];
				if (current == null)
					continue;

				// 아이템 종류 일치, 개수 여유 확인
				if (current.Data != target)
					continue;

				if (!current.IsMax)
					return i;
			}

			return NONE;
		}

		public int FindEquipmentByGuid(Guid? guid)
		{
			if (guid == null)
				return NONE;

			for (int i = 0; i < Capacity; i++)
			{
				Item current = RuntimeItems[i];
				if (current == null)
					continue;

				if (current.Guid == guid)
					return i;
			}

			return NONE;
		}

		/// <summary> 인벤토리에 아이템 추가
		/// <para/> 넣는 데 실패한 잉여 아이템 개수 리턴
		/// <para/> 리턴이 0이면 넣는데 모두 성공했다는 의미
		/// </summary>
		public int Add(ItemData itemData, int amount = 1)
		{
			int index;

			// 1. 수량이 있는 아이템
			// if (itemData is CountableItemData ciData)
			if (itemData.IsCountable)
			{
				var findNextCountable = true;
				index = NONE;

				while (amount > 0)
				{
					// 1-1. 이미 해당 아이템이 인벤토리 내에 존재하고, 개수 여유 있는지 검사
					if (findNextCountable)
					{
						// index = FindItemSlotIndex(ciData, index + 1);
						index = FindItemSlotIndex(itemData, index + 1);

						// 개수 여유있는 기존재 슬롯이 더이상 없다고 판단될 경우, 빈 슬롯부터 탐색 시작
						if (index == NONE)
						{
							findNextCountable = false;
						}
						// 기존재 슬롯을 찾은 경우, 양 증가시키고 초과량 존재 시 amount에 초기화
						else
						{
							// CountableItem ci = Items[index] as CountableItem;
							// amount = ci.AddAmountAndGetExcess(amount);

							amount = RuntimeItems[index].AddAmountAndGetExcess(amount);

							UpdateSlot(index);
						}
					}
					// 1-2. 빈 슬롯 탐색
					else
					{
						index = FindEmptySlotIndex(index + 1);

						// 빈 슬롯조차 없는 경우 종료
						if (index == NONE)
						{
							break;
						}
						// 빈 슬롯 발견 시, 슬롯에 아이템 추가 및 잉여량 계산
						else
						{
							// 새로운 아이템 생성
							// CountableItem ci = ciData.CreateItem() as CountableItem;
							// ci.SetAmount(amount);
							var i = itemData.CreateItem();
							i.SetAmount(amount);

							// 슬롯에 추가
							// Items[index] = ci;
							RuntimeItems[index] = i;

							// 남은 개수 계산
							// amount = (amount > ciData.MaxAmount) ? (amount - ciData.MaxAmount) : 0;
							amount = (amount > itemData.MaxAmount) ? (amount - itemData.MaxAmount) : 0;

							UpdateSlot(index);
						}
					}
				}
			}
			// 2. 수량이 없는 아이템
			else
			{
				// 2-1. 1개만 넣는 경우, 간단히 수행
				if (amount == 1)
				{
					index = FindEmptySlotIndex();
					if (index != NONE)
					{
						// 아이템을 생성하여 슬롯에 추가
						RuntimeItems[index] = itemData.CreateItem();
						amount = 0;

						UpdateSlot(index);
					}
				}

				// 2-2. 2개 이상의 수량 없는 아이템을 동시에 추가하는 경우
				index = NONE;
				for (; amount > 0; amount--)
				{
					// 아이템 넣은 인덱스의 다음 인덱스부터 슬롯 탐색
					index = FindEmptySlotIndex(index + 1);

					// 다 넣지 못한 경우 루프 종료
					if (index == NONE)
					{
						break;
					}

					// 아이템을 생성하여 슬롯에 추가
					RuntimeItems[index] = itemData.CreateItem();

					UpdateSlot(index);
				}
			}

			SOManager.Instance.LastEquipedItem.RuntimeValue = itemData;
			return amount;
		}

		public void Remove(int index, int amount = 1)
		{
			if (index < 0 || index >= Capacity)
				return;

			Item item = RuntimeItems[index];
			if (item == null)
				return;

			if (item.Data.IsCountable)
			{
				item.SetAmount(item.Amount - amount);
				if (item.IsEmpty)
					RuntimeItems[index] = null;
			}
			else
			{
				RuntimeItems[index] = null;
			}

			UpdateSlot(index);
		}

		public void LoadSaveItems(List<InventorySlotData> savedItems)
		{
			RuntimeItems = Enumerable.Repeat<Item>(null, Capacity = DefaultCapacity).ToList();
			
			foreach (InventorySlotData itemData in savedItems)
			{
				RuntimeItems[itemData.slotIndex] = new Item(
					itemData.Guid,
					DataManager.Instance.ItemDic[itemData.itemID],
					itemData.itemAmount);
			}
		}

		public List<InventorySlotData> GetInventoryData()
		{
			List<InventorySlotData> InventoryData = new(Capacity);
			for (int i = 0; i < RuntimeItems.Count; i++)
			{
				if (RuntimeItems[i] == null)
					continue;
				InventoryData.Add(new InventorySlotData(i, RuntimeItems[i]));
			}
			return InventoryData;
		}

		private bool IsValidIndex(int index)
		{
			return index >= 0 && index < Capacity;
		}

		public ItemData GetItemData(int index)
		{
			if (!IsValidIndex(index)) return null;
			return RuntimeItems[index]?.Data;
		}

		public Item GetItem(int index)
		{
			if (!IsValidIndex(index)) return null;
			return RuntimeItems[index] ?? null;
		}

		public void SetItem(int index, Item item)
		{
			if (!IsValidIndex(index))
				return;
			RuntimeItems[index] = item;
			UpdateSlot(index);
		}

		public void SetItemAmount(int index, int amount)
		{
			if (!IsValidIndex(index))
				return;
			if (RuntimeItems[index] != null)
				RuntimeItems[index].SetAmount(amount);
			UpdateSlot(index);
		}

		private void UpdateSlot(params int[] indices)
		{
			foreach (int i in indices)
				UpdateSlot(i);
		}

		public void UpdateSlot(int index)
		{
			if (!IsValidIndex(index)) return;

			if (RuntimeItems[index] != null)
				if (RuntimeItems[index].Data.IsCountable)
					if (RuntimeItems[index].IsEmpty)
						RuntimeItems[index] = null;

			Item item = RuntimeItems[index];

			foreach (UIItemInventory inventoryUI in inventoryUIs)
			{
				inventoryUI.UpdateSlotUI(index, item);

				// 1. 아이템이 슬롯에 존재하는 경우
				//if (item != null)
				//  inventoryUI.UpdateSlotFilterState(index, item.Data);
			}
		}

		public override void OnAfterDeserialize()
		{
			RuntimeItems = Enumerable.Repeat<Item>(null, Capacity = DefaultCapacity).ToList();
		}
		public override void OnBeforeSerialize() { }
	}
}