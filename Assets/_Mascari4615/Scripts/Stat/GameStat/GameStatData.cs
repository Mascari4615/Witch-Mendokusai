using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "GSD_", menuName = "Variable/" + nameof(GameStatData))]
	public class GameStatData : StatData<GameStatType>
	{
	}
}