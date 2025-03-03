using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(NPC), menuName = "Variable/" + nameof(Unit) + "/" + nameof(NPC))]
	public class NPC : Unit
	{
		[field: Header("_" + nameof(NPC))]
		[field: SerializeField] public List<NPCPanelInfo> PanelInfos { get; private set; }
		[field: SerializeField] public List<QuestSO> QuestDatas { get; private set; }

		public List<ItemDataBuffer> ItemDataBuffers => GetAllDataSOs(NPCType.Shop).Cast<ItemDataBuffer>().ToList();
		public List<Dungeon> Dungeons => GetAllDataSOs(NPCType.DungeonEntrance).Cast<Dungeon>().ToList();

		private List<DataSO> GetAllDataSOs(NPCType npcType)
		{
			return PanelInfos
					.Where(i => i.Type == npcType)
					.SelectMany(i => i.DataSOs)
					.ToList();
		}

		public List<NPCType> GetNPCTypeList()
		{
			return PanelInfos.Select(i => i.Type).ToList();
		}
	}
}