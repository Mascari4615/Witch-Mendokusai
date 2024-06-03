using System;
using UnityEngine;

namespace Mascari4615
{
	public abstract class StatData<T> : DataSO where T : Enum
	{
		[field: SerializeField] public T Type { get; set; }
	}
}