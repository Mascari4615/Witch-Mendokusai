using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(QuestDataBuffer), menuName = "GameSystem/DataBuffer/" + nameof(Quest))]
	public class QuestDataBuffer : DataBuffer<Quest> { }
}