using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;

namespace Mascari4615
{
	public abstract class UnitObject : MonoBehaviour
	{
		[field: SerializeField] public Unit UnitData { get; private set; }
		public Stat Stat { get; private set; }
		public SkillHandler SkillHandler { get; protected set; }
		[field: SerializeField] public SpriteRenderer SpriteRenderer { get; protected set; }
		public NavMeshAgent NavMeshAgent { get; protected set; }
		private Vector3 originScale;

		public bool IsAlive => Stat[StatType.HP_CUR] > 0;

		[SerializeField] private MMF_Player mmfPlayer;
		[SerializeField] protected Animator animator;

		public float stoppingDistance = 0.1f;
		public bool updateRotation = false;
		public float acceleration = 40.0f;
		public float tolerance = 1.0f;

		protected virtual void Awake()
		{
			originScale = SpriteRenderer.transform.localScale;
			mmfPlayer?.StopFeedbacks();
			SpriteRenderer.material.SetFloat("_Emission", 0);
			NavMeshAgent = GetComponent<NavMeshAgent>();

			if (UnitData != null)
				Init(UnitData);
		}

		public virtual void Init(Unit unitData)
		{
			if (Stat == null)
				Stat = new Stat();

			UnitData = unitData;
			
			if (SkillHandler != null)
				TimeManager.Instance.RemoveCallback(SkillHandler.Tick);
			SkillHandler = new(this);
			TimeManager.Instance.RegisterCallback(SkillHandler.Tick);

			Stat.Init(UnitData.InitStatSO.Stats);
			Stat[StatType.HP_MAX] = unitData.InitStatSO.Stats[StatType.HP_MAX];
			SetHp(Stat[StatType.HP_MAX]);

			SpriteRenderer.transform.localScale = originScale;

			if (NavMeshAgent)
			{
				NavMeshAgent.stoppingDistance = stoppingDistance;
				NavMeshAgent.speed = Stat[StatType.MOVEMENT_SPEED];
				// agent.destination = moveDest;
				NavMeshAgent.updateRotation = updateRotation;
				NavMeshAgent.acceleration = acceleration;
			}
		}

		public virtual bool UseSkill(int index)
		{
			return SkillHandler.UseSkill(index);
		}

		protected virtual void SetHp(int newHp)
		{
			Stat[StatType.HP_CUR] = newHp;
			if (Stat[StatType.HP_CUR] <= 0)
				Die();
		}

		public virtual void ReceiveDamage(int damage)
		{
			if (!IsAlive)
				return;

			SetHp(Mathf.Clamp(Stat[StatType.HP_CUR] - damage, 0, int.MaxValue));
			mmfPlayer?.PlayFeedbacks();
		}

		protected virtual void Die()
		{
			OnDied();
		}
		protected virtual void OnDied()
		{
		}
	}
}