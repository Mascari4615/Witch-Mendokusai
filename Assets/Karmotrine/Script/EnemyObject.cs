using System;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Karmotrine.Script
{
    public class EnemyObject : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        public Action OnEnemyDied;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private MMF_Player mmfPlayer;

        protected Enemy curEnemy;
        private int curHP;

        public bool IsAlive => curHP != 0;

        public virtual void Init(Enemy enemy)
        {
            curEnemy = enemy;
            CanvasManager.Instance.HpBar.UpdateEnemy(enemy, curHP);
            SetHp(enemy.maxHp);
            spriteRenderer.sprite = enemy.sprite;
            
            gameObject.SetActive(true);
        }

        public void ReceiveAttack(int damage)
        {
            SetHp(Mathf.Clamp(curHP - damage, 0, int.MaxValue));
            mmfPlayer.PlayFeedbacks();
            if (curHP == 0)
                Die();
        }

        protected void SetHp(int newHp)
        {
            curHP = newHp;
            CanvasManager.Instance.HpBar.UpdateUI(curHP);
        }

        protected virtual void Die()
        {
            DropLoot();
            OnEnemyDied?.Invoke();
        }

        protected virtual void DropLoot()
        {
            Probability<ItemData> probability = new();
            foreach (var item in curEnemy.Loots)
                probability.Add(item.specialThing as ItemData, item.percentage);

            var newItem = probability.Get();
            inventory.Add(newItem);

            gameObject.SetActive(false);
        }
    }
}