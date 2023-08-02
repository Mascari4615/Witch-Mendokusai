using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class UnitObject : MonoBehaviour
{
    public Unit UnitData => unitData;
    [SerializeField] protected Unit unitData;
    
    public UnitSkillHandler UnitSkillHandler => unitSkillHandler;
    protected UnitSkillHandler unitSkillHandler;
    
    [SerializeField] protected SpriteRenderer spriteRenderer;
    
    public int CurHp { get; protected set; }
    public int MaxHp { get; protected set; }

    public bool IsAlive => CurHp != 0;
    
    [SerializeField] private MMF_Player mmfPlayer;

    protected virtual void Awake()
    {
        if (unitData != null)
            Init(unitData);
        
        spriteRenderer.material.SetFloat("_Emission", 0);
    }

    public virtual void Init(Unit unitData)
    {
        this.unitData = unitData;
        unitSkillHandler = new UnitSkillHandler();
        
        Debug.Log(unitSkillHandler);
        
        MaxHp = unitData.MaxHp;
        SetHp(unitData.MaxHp);

        for (int i = 0; i < unitData.DefaultSkills.Length; i++)
            unitSkillHandler.SetSkill(i, unitData.DefaultSkills[i]);
    }

    public virtual bool UseSkill(int index)
    {
        return unitSkillHandler.UseSkill(this, index);
    }

    protected virtual void Update()
    {
        unitSkillHandler?.Tick(Time.deltaTime);
    }
    
    protected virtual void SetHp(int newHp)
    {
        CurHp = newHp;
    }
    
    public void ReceiveAttack(int damage)
    {
        SetHp(Mathf.Clamp(CurHp - damage, 0, int.MaxValue));
        OnReceiveAttack(damage);
        if (CurHp == 0)
            Die();
    }
    protected virtual void OnReceiveAttack(int damage)
    {
        mmfPlayer?.PlayFeedbacks();
        // Override 해줘
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
    private Dictionary<int, (Skill skill, SkillCoolTime skillCoolTime)> skillDic = new ();

    public void SetSkill(int skillButtonIndex, Skill skill)
    {
        skillDic[skillButtonIndex] = (skill, new SkillCoolTime(skill));
    }

    public bool UseSkill(UnitObject unitObject, int skillButtonIndex)
    {
        if (skillDic.TryGetValue(skillButtonIndex, out var value))
        {
            if (IsReady(skillButtonIndex))
            {
                value.skill.Use(unitObject);
                skillDic[skillButtonIndex].skillCoolTime.ResetCooltime();
                return true;
            }
        }

        return false;
    }

    public bool IsReady(int skillButtonIndex)
    {
        return skillDic[skillButtonIndex].skillCoolTime.IsReady;
    }

    public void Tick(float delta)
    {
        foreach (var skillSlot in skillDic.Values)
        {
            skillSlot.skillCoolTime.Tick(delta);
        }
    }
}

public class SkillCoolTime
{
    public float CurCooltime { get; private set; }
    public float Cooltime { get; private set; }
    public bool IsReady => CurCooltime == 0;

    public SkillCoolTime(Skill skill)
    {
        this.Cooltime = skill.Cooltime;
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
