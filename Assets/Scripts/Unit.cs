using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Unit), menuName = "Variable/Unit")]
	public class Unit : Artifact
	{
		public float Speed => speed;
		public int MaxHp => maxHp;
		public Skill[] DefaultSkills => defaultSkills;
		public Material Material => material;

		[SerializeField] private float speed = 1f;
		[SerializeField] private int maxHp;
		[SerializeField] private Skill[] defaultSkills;
		[SerializeField] private Material material;
	}
}