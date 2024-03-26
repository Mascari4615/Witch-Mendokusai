using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class Unit : Artifact
	{
		[field: Header("_" + nameof(Unit))]
		[field: SerializeField] public GameObject Prefab { get; set; }
		[field: SerializeField] public Skill[] DefaultSkills { get; set; }
		[field: SerializeField] public Material Material { get; set; }
		[field: SerializeField] public StatSO InitStatSO { get; set; }
	}
}