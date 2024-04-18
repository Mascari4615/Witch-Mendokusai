using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class UILoading : Singleton<UILoading>
	{
		[SerializeField] private Image progressBar;
		private readonly List<AsyncOperationHandle> handles = new();

		public IEnumerator Loading()
		{
			gameObject.SetActive(true);
			progressBar.fillAmount = 0f;

			LoadAssetsAsync();

			while (true)
			{
				float totalPercent = 0;
				foreach (var handle in handles)
					totalPercent += handle.PercentComplete;
				progressBar.fillAmount = totalPercent / handles.Count;

				Debug.Log($"Loading... {progressBar.fillAmount * 100}%");

				if (handles.All(handle => handle.IsDone))
					break;

				yield return null;
			}
			Debug.Log($"Loading... {progressBar.fillAmount * 100}%");

			//foreach (var handle in handles)
			//	Addressables.Release(handle);

			progressBar.fillAmount = 1f;
			gameObject.SetActive(false);
		}

		private void LoadAssetsAsync()
		{
			SOManager.Instance.DataSOs.Clear();

			LoadAsset<QuestData>("QUEST_DATA");
			LoadAsset<CardData>("CARD_DATA");
			LoadAsset<Effect>("EFFECT");
			LoadAsset<ItemData>("ITEM_DATA");
			LoadAsset<MonsterWave>("MONSTER_WAVE");
			LoadAsset<SkillData>("SKILL_DATA");
			LoadAsset<WorldStage>("WORLD_STAGE");
			LoadAsset<Dungeon>("DUNGEON");
			LoadAsset<DungeonStage>("DUNGEON_STAGE");
			LoadAsset<DungeonConstraint>("DUNGEON_CONSTRAINT");
			LoadAsset<Doll>("DOLL");
			LoadAsset<NPC>("NPC");
			LoadAsset<Monster>("MONSTER");

			void LoadAsset<T>(string label) where T : DataSO
			{
				var handle = Addressables.LoadAssetsAsync<T>(label, null);
				handle.Completed += OnAssetsLoaded;
				handles.Add(handle);
			}
		}

		private void OnAssetsLoaded<T>(AsyncOperationHandle<IList<T>> obj) where T : DataSO
		{
			if (obj.Status == AsyncOperationStatus.Succeeded)
			{
				List<T> assets = obj.Result.ToList();
				SOManager.Instance.DataSOs[typeof(T)] = new();

				foreach (T asset in assets)
				{
					Debug.Log($"Loaded {asset.name}");
					SOManager.Instance.DataSOs[typeof(T)].Add(asset.ID, asset);
				}
			}
		}
	}
}