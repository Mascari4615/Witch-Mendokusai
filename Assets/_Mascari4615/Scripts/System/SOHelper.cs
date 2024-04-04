using System;
using System.Collections.Generic;

namespace Mascari4615
{
	public static class SOHelper
	{
		public static ItemData GetItemData(int id) => GetArtifact<ItemData>(id);
		public static Doll GetDoll(int id) => GetArtifact<Doll>(id);
		public static QuestData GetQuest(int id) => GetArtifact<QuestData>(id);
		public static DungeonConstraint GetDungeonConstraint(int id) => GetArtifact<DungeonConstraint>(id);
		public static Dungeon GetDungeon(int id) => GetArtifact<Dungeon>(id);
		public static NPC GetNPC(int id) => GetArtifact<NPC>(id);

		// 아래 코드는 불가능
		// 왜 WHY : 제네릭 타입의 변환에 제한, C#의 타입 안전성을 보장하기 위한.
		// i.e. Dic<int, Artifact>를 Dic<int, ItemData>로 캐스팅하고, Artifact를 Add하려고 하면, 이는 ItemData 타입이 아니므로 문제가 발생
		// public static Dictionary<int, T> GetDictionary<T>() where T : Artifact => SOManager.Instance.Artifacts[typeof(T)] as Dictionary<int, T>;

		public static void ForEach<T>(Action<T> action) where T : Artifact
		{
			foreach (var artifact in SOManager.Instance.Artifacts[typeof(T)].Values)
			{
				action(artifact as T);
			}
		}

		public static int CountOf<T>() where T : Artifact => SOManager.Instance.Artifacts[typeof(T)].Count;

		/// <summary>
		/// 주어진 타입의 Artifact 스크립터블 오브젝트를 가져옵니다
		/// </summary>
		public static T GetArtifact<T>(int id) where T : Artifact =>
			SOManager.Instance.Artifacts[typeof(T)][id] as T;
	}
}