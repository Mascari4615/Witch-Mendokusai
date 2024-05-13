using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class SaveManager
	{
		public bool IsDataLoaded { get; private set; }

		private SOManager SOManager => SOManager.Instance;
		private DataManager DataManager => DataManager.Instance;

		public void CreateNewGameData()
		{
			GameData newGameData = new()
			{
				curDollIndex = 0,
				dummyDollCount = 1,
				nyang = 100,
				inventoryItems = new(),
				dolls = new()
				{
					new(0, 1, 0, new(){})
				},
				works = new()
				{
					{ WorkListType.DollWork, new() },
					{ WorkListType.DummyWork, new() },
					{ WorkListType.VQuestWork, new() }
				},
				quests = new(),
				runtimeQuests = new(),
				statistics = new()
			};

			// 아이템(장비) 초기화
			Doll defaultDoll = GetDoll(0);
			defaultDoll.EquipmentGuids.Clear();

			Inventory inventory = SOManager.ItemInventory;
			inventory.Load(newGameData.inventoryItems);
			foreach (EquipmentData equipmentData in defaultDoll.DefaultEquipments)
			{
				inventory.Add(equipmentData);
				Guid? guid = inventory.GetItem(inventory.FindItemIndex(equipmentData)).Guid;
				newGameData.dolls[0].EquipmentGuids.Add(guid);
				defaultDoll.EquipmentGuids.Add(guid);
			}
			defaultDoll.EquipmentGuids.Add(null);
			newGameData.inventoryItems = inventory.Save();

			// 장비 초기화 이후 저장
			ForEach<Doll>(doll =>
			{
				if (doll.ID != 0)
					newGameData.dolls.Add(doll.Save());
			});
			ForEach<QuestSO>(questData => newGameData.quests.Add(questData.Save()));

			SaveData();
			LoadData(newGameData);
		}

		public void LoadData(GameData saveData)
		{
			DataManager.CurDollID = saveData.curDollIndex;
			DataManager.DummyDollCount = saveData.dummyDollCount;
			SOManager.Nyang.RuntimeValue = saveData.nyang;

			// 통계 초기화
			SOManager.Statistics.Load(saveData.statistics);

			// 아이템 초기화
			SOManager.ItemInventory.Load(saveData.inventoryItems);

			// 인형 초기화
			SOManager.DollBuffer.Clear();
			foreach (DollSaveData dollData in saveData.dolls)
			{
				GetDoll(dollData.DollID).Load(dollData);
				SOManager.DollBuffer.Add(GetDoll(dollData.DollID));
			}
			for (int i = 0; i < saveData.dummyDollCount - 1; i++)
				SOManager.DollBuffer.Add(GetDoll(Doll.DUMMY_ID));

			// 퀘스트 초기화
			foreach (QuestSOSaveData questData in saveData.quests)
			{
				GetQuestSO(questData.QuestID).Load(questData);
				if (questData.State >= QuestState.Unlocked)
					SOManager.QuestDataBuffer.Add(GetQuestSO(questData.QuestID));
			}

			// 작업 초기화
			DataManager.WorkManager.Init(saveData.works);
			DataManager.QuestManager.Init(saveData.runtimeQuests.ConvertAll(questData => new RuntimeQuest(questData)));

			IsDataLoaded = true;
		}

		public void SaveData()
		{
			GameData gameData = new()
			{
				curDollIndex = DataManager.CurDollID,
				dummyDollCount = DataManager.DummyDollCount,
				nyang = SOManager.Nyang.RuntimeValue,
				inventoryItems = SOManager.ItemInventory.Save(),
				dolls = new(),
				works = DataManager.WorkManager.Works,
				quests = new(),
				runtimeQuests = DataManager.QuestManager.Quests.Datas.Where(quest => quest.Type != QuestType.Dungeon).ToList().ConvertAll(quest => quest.Save()),
				statistics = SOManager.Statistics.Save()
			};

			ForEach<Doll>(doll => gameData.dolls.Add(doll.Save()));
			ForEach<QuestSO>(questData => gameData.quests.Add(questData.Save()));

			if (GameSetting.UseLocalData)
			{
				string json = JsonConvert.SerializeObject(gameData, Formatting.Indented, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				});
				string path = Path.Combine(Application.dataPath, "WM.json");
				File.WriteAllText(path, json);
			}
			else
			{
				DataManager.PlayFabManager.SavePlayerData(gameData);
			}
		}
	}
}