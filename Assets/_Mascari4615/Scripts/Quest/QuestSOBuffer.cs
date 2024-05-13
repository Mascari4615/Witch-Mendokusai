using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(QuestSOBuffer), menuName = "DataBuffer/" + nameof(QuestSO))]
	public class QuestSOBuffer : DataBufferSO<QuestSO> { }
}