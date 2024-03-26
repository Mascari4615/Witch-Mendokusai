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
		
		public UnitSkillHandler UnitSkillHandler { get; protected set; }
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
			if (UnitData != null)
				Init(UnitData);

			mmfPlayer?.StopFeedbacks();
			SpriteRenderer.material.SetFloat("_Emission", 0);

			NavMeshAgent = GetComponent<NavMeshAgent>();
		}

		public virtual void Init(Unit unitData)
		{
			UnitData = unitData;
			UnitSkillHandler = new UnitSkillHandler(this);

			if (Stat == null)
				Stat = new Stat();

			Stat.Init(UnitData.InitStatSO.Stats);
			Stat[StatType.HP_MAX] = unitData.InitStatSO.Stats[StatType.HP_MAX];
			SetHp(Stat[StatType.HP_MAX]);

			for (int i = 0; i < unitData.DefaultSkills.Length; i++)
				UnitSkillHandler.SetSkill(i, unitData.DefaultSkills[i]);

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
			return UnitSkillHandler.UseSkill(this, index);
		}

		protected virtual void Update()
		{
			UnitSkillHandler?.Tick(Time.deltaTime);
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
			OnDie();
		}
		protected virtual void OnDie()
		{
		}
	}

	public class UnitSkillHandler
	{
		public Dictionary<int, (Skill skill, SkillCoolTime skillCoolTime)> SkillDic { get; private set; } = new();
		private UnitObject unitObject;
		private float coolTimeBonus = 0;

		public UnitSkillHandler(UnitObject unitObject)
		{
			this.unitObject = unitObject;
		}

		private float CalcSkillCoolTime(float skillCoolTime) => skillCoolTime * (1f - (coolTimeBonus / 100f));

		public void SetSkill(int skillIndex, Skill skill)
		{
			SkillDic[skillIndex] = (skill, new SkillCoolTime(CalcSkillCoolTime(skill.Cooltime)));
		}

		public bool UseSkill(UnitObject unitObject, int skillButtonIndex)
		{
			if (SkillDic.TryGetValue(skillButtonIndex, out var value))
			{
				if (IsReady(skillButtonIndex))
				{
					value.skill.Use(unitObject);
					SkillDic[skillButtonIndex].skillCoolTime.ResetCooltime();
					return true;
				}
			}

			return false;
		}

		public void SetCoolTimeBonus(float coolTimeBonus)
		{
			this.coolTimeBonus = coolTimeBonus;

			foreach (var skillSlot in SkillDic.Values)
				skillSlot.skillCoolTime.SetCoolTime(CalcSkillCoolTime(skillSlot.skill.Cooltime));
		}

		public bool IsReady(int skillButtonIndex)
		{
			if (SkillDic.TryGetValue(skillButtonIndex, out var value) == false)
			{
				Debug.LogError($"{unitObject.gameObject.name}, Invalid skillIndex");
				return false;
			}

			return value.skillCoolTime.IsReady;
		}

		public void Tick(float delta)
		{
			foreach (var skillSlot in SkillDic.Values)
			{
				skillSlot.skillCoolTime.Tick(delta);

				if (skillSlot.skillCoolTime.IsReady &&
					skillSlot.skill.AutoUse)
				{
					skillSlot.skill.Use(unitObject);
					skillSlot.skillCoolTime.ResetCooltime();
				}
			}
		}
	}

	public class SkillCoolTime
	{
		public float CurCooltime { get; private set; }
		public float Cooltime { get; private set; }
		public bool IsReady => CurCooltime == 0;

		public SkillCoolTime(float coolTime)
		{
			this.Cooltime = coolTime;
		}

		public void SetCoolTime(float coolTime)
		{
			this.Cooltime = coolTime;
		}

		public void Tick(float delta)
		{
			if (IsReady)
				return;

			CurCooltime -= delta;

			if (CurCooltime <= 0)
				CurCooltime = 0;
		}

		public void ResetCooltime()
		{
			CurCooltime = Cooltime;
		}
	}
}