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

        protected Enemy CurEnemy;
        private int _curHp;

        public bool IsAlive => _curHp != 0;

        public virtual void Init(Enemy enemy)
        {
            CurEnemy = enemy;
            CanvasManager.Instance.HpBar.UpdateEnemy(enemy, _curHp);
            SetHp(enemy.MaxHp);
            spriteRenderer.sprite = enemy.Thumbnail;
            
            gameObject.SetActive(true);
        }

        public void ReceiveAttack(int damage)
        {
            SetHp(Mathf.Clamp(_curHp - damage, 0, int.MaxValue));
            mmfPlayer.PlayFeedbacks();
            if (_curHp == 0)
                Die();
        }

        protected void SetHp(int newHp)
        {
            _curHp = newHp;
            CanvasManager.Instance.HpBar.UpdateUI(_curHp);
        }

        protected virtual void Die()
        {
            DropLoot();
            OnEnemyDied?.Invoke();
        }

        protected virtual void DropLoot()
        {
            Probability<ItemData> probability = new();
            foreach (var item in CurEnemy.Loots)
                probability.Add(item.Artifact as ItemData, item.Percentage);

            var newItem = probability.Get();
            inventory.Add(newItem);

            gameObject.SetActive(false);
        }
    }
}