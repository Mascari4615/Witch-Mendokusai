using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(QuestDataBuffer), menuName = "DataBuffer/" + nameof(QuestData))]
	public class QuestDataBuffer : DataBuffer<QuestData> { }
}