using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Unit), menuName = "Variable/Unit")]
	public class Unit : Artifact
	{
		[field: Header("_" + nameof(Unit))]
		[field: SerializeField] public GameObject Prefab { get; set; }
		[field: SerializeField] public float MoveSpeed { get; set; }
		[field: SerializeField] public int MaxHp { get; set; }
		[field: SerializeField] public Skill[] DefaultSkills { get; set; }
		[field: SerializeField] public Material Material { get; set; }
	}
}