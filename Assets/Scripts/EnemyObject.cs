using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Karmotrine.Script
{
    public class EnemyObject : UnitObject, IHitable
    {
        [SerializeField] private Inventory inventory;
        public Action OnEnemyDied;

        [SerializeField] protected SpriteRenderer spriteRenderer;

        [SerializeField] private EnemyObjectVariable lastHitEnemyObject;

        public override void Init(Unit unitData)
        {
            base.Init(unitData);
            
            spriteRenderer.sprite = unitData.Thumbnail;
            gameObject.SetActive(true);
        }
        

        protected override void SetHp(int newHp)
        {
            base.SetHp(newHp);
            lastHitEnemyObject.RuntimeValue = this;
        }

        protected override void Die()
        {
            DropLoot();

            OnEnemyDied?.Invoke();
            base.Die();
        }

        protected virtual void DropLoot()
        {
            Probability<ItemData> probability = new();
            foreach (var item in (unitData as Enemy).Loots)
                probability.Add(item.Artifact as ItemData, item.Percentage);

            var newItem = probability.Get();
            inventory.Add(newItem);
        }
    }
}