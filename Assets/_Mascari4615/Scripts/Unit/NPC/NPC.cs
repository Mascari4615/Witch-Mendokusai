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

		public List<QuestData> QuestData => GetAllArtifacts(NPCPanelType.Quest).Cast<QuestData>().ToList();
		public List<Dungeon> Dungeons => GetAllArtifacts(NPCPanelType.DungeonEntrance).Cast<Dungeon>().ToList();

		private List<Artifact> GetAllArtifacts(NPCPanelType panelType)
		{
			return PanelInfos
					.Where(i => i.Type == panelType)
					.SelectMany(i => i.Artifacts)
					.ToList();
		}

		public NPCPanelType AllPanelTypes => PanelInfos
				.Select(i => i.Type)
				.Aggregate((a, b) => a | b);
	}
}