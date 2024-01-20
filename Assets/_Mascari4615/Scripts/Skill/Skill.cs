using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class Skill : Artifact
	{
		[field: SerializeField] public bool AutoUse { get; set; }
		[field: SerializeField] public float Cooltime { get; set; }

		public abstract bool Use(UnitObject unitObject);
	}
}