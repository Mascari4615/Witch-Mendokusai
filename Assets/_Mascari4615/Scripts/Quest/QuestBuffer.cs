using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(QuestBuffer), menuName = "DataBuffer/" + nameof(RuntimeQuest))]
	public class QuestBuffer : DataBufferSO<RuntimeQuest> { }
}