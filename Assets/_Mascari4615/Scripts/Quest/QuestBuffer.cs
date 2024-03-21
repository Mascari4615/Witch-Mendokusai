using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(QuestBuffer), menuName = "GameSystem/DataBuffer/" + nameof(Quest))]
	public class QuestBuffer : DataBuffer<Quest> { }
}