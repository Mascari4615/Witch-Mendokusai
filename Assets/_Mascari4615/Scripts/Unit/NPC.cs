using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(NPC), menuName = "Variable/" + nameof(Unit) + "/" + nameof(NPC))]
	public class NPC : Unit
	{
		[field: Header("_" + nameof(NPC))]
		[field: SerializeField] public MNPCPanelType PanelType { get; private set; }
		[field: SerializeField] public List<Quest> Quests { get; private set; }
	}
}