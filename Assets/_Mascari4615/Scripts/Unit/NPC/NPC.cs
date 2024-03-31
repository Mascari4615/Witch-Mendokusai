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

		// curNPCData가 가지고 있는 PanelInfos 중 Quest 타입의 Artifacts를 모두 가져온다.
		public List<QuestData> QuestDatas => PanelInfos
				.Where(i => i.Type == NPCPanelType.Quest)
				.SelectMany(i => i.Artifacts)
				.Cast<QuestData>()
				.ToList();

		public NPCPanelType AllPanelTypes => PanelInfos
				.Select(i => i.Type)
				.Aggregate((a, b) => a | b);
	}
}