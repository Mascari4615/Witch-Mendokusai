using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "STS_", menuName = "Variable/" + nameof(StatisticsData))]
	public class StatisticsData : DataSO
	{
		public StatisticsType Type;
	}
}