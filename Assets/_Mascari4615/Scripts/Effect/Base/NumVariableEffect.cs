using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class NumVariableEffect<T> : NumEffect<T>
	{
		[field: SerializeField] public CustomVariable<T> TargetStat { get; private set; }
	}
}