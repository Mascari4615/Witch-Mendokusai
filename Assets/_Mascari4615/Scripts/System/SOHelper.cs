using System;
using System.Collections.Generic;

namespace Mascari4615
{
	public static class SOHelper
	{
		public static ItemData GetItemData(int id) => Get<ItemData>(id);
		public static Doll GetDoll(int id) => Get<Doll>(id);
		public static QuestData GetQuest(int id) => Get<QuestData>(id);
		public static DungeonConstraint GetDungeonConstraint(int id) => Get<DungeonConstraint>(id);
		public static Dungeon GetDungeon(int id) => Get<Dungeon>(id);
		public static NPC GetNPC(int id) => Get<NPC>(id);

		// 아래 코드는 불가능
		// 왜 WHY : 제네릭 타입의 변환에 제한, C#의 타입 안전성을 보장하기 위한.
		// i.e. Dic<int, DataSO>를 Dic<int, ItemData>로 캐스팅하고, DataSO를 Add하려고 하면, 이는 ItemData 타입이 아니므로 문제가 발생
		// public static Dictionary<int, T> GetDictionary<T>() where T : DataSO => SOManager.Instance.DataSOs[typeof(T)] as Dictionary<int, T>;

		public static void ForEach<T>(Action<T> action) where T : DataSO
		{
			foreach (var dataSO in SOManager.Instance.DataSOs[typeof(T)].Values)
			{
				action(dataSO as T);
			}
		}

		public static int CountOf<T>() where T : DataSO => SOManager.Instance.DataSOs[typeof(T)].Count;

		/// <summary>
		/// 주어진 타입의 DataSO 스크립터블 오브젝트를 가져옵니다
		/// </summary>
		public static T Get<T>(int id) where T : DataSO => SOManager.Instance.DataSOs[typeof(T)][id] as T;
	}
}