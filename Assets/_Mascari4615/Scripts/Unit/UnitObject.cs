using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Mascari4615
{
	public class UnitObject : MonoBehaviour
	{
		[Header("_" + nameof(UnitObject))]
		[SerializeField] protected Unit unitData;
		public Unit UnitData => unitData;

		public UnitSkillHandler UnitSkillHandler { get; protected set; }

		[SerializeField] protected SpriteRenderer spriteRenderer;
		private Vector3 originScale;

		public int CurHp { get; protected set; }
		public int MaxHp { get; protected set; }

		public bool IsAlive => CurHp > 0;

		[SerializeField] private MMF_Player mmfPlayer;
		[SerializeField] protected Animator animator;

		protected virtual void Awake()
		{
			originScale = spriteRenderer.transform.localScale;
			if (unitData != null)
				Init(unitData);

			mmfPlayer?.StopFeedbacks();
			spriteRenderer.material.SetFloat("_Emission", 0);
		}

		public virtual void Init(Unit unitData)
		{
			this.unitData = unitData;
			UnitSkillHandler = new UnitSkillHandler(this);

			MaxHp = unitData.MaxHp;
			SetHp(unitData.MaxHp);

			for (int i = 0; i < unitData.DefaultSkills.Length; i++)
				UnitSkillHandler.SetSkill(i, unitData.DefaultSkills[i]);

			spriteRenderer.transform.localScale = originScale;
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
			CurHp = newHp;
			if (CurHp <= 0)
				Die();
		}

		public virtual void ReceiveDamage(int damage)
		{
			if (!IsAlive)
				return;

			SetHp(Mathf.Clamp(CurHp - damage, 0, int.MaxValue));
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