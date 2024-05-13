using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class Unit : DataSO
	{
		[field: Header("_" + nameof(Unit))]
		[PropertyOrder(10)][field: SerializeField] public GameObject Prefab { get; set; }
		[PropertyOrder(11)][field: SerializeField] public SkillData[] DefaultSkills { get; set; }
		[PropertyOrder(12)][field: SerializeField] public Material Material { get; set; }
		[PropertyOrder(13)][field: SerializeField] public StatInfos InitStatInfos { get; set; }
	}
}