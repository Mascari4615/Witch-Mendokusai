using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyObject : MonoBehaviour
{
    public bool IsAlive => curHP != 0;
    [SerializeField] private Inventory inventory;
    [SerializeField] private ClickerManager clickerManager;
    private Enemy curEnemy;
    private int curHP;

    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpBarText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private MMF_Player _mmfPlayer;

    public void Init(Enemy enemy)
    {
        curEnemy = enemy;
        curHP = enemy.hp;

        nameText.text = enemy.Name;
        hpBarText.text = $"{curHP} / {curEnemy.hp}";
        spriteRenderer.sprite = enemy.sprite;
        hpBar.fillAmount = 1;
    }

    public void ReceiveAttack(int damage)
    {
        // RuntimeManager.PlayOneShot($"event:/Rock");
        curHP = Mathf.Clamp(curHP - damage, 0, int.MaxValue);

        hpBarText.text = $"{curHP} / {curEnemy.hp}";
        hpBar.fillAmount = (float)curHP / curEnemy.hp;
        
        _mmfPlayer.PlayFeedbacks();

        if (curHP != 0)
            return;

        DropLoot();
        clickerManager.SpawnEnemy();
    }

    private void DropLoot()
    {
        Probability<ItemData> probability = new();
        foreach (var item in curEnemy.Loots)
            probability.Add(item.specialThing as ItemData, item.percentage);

        var newItem = probability.Get();
        inventory.Add(newItem);
    }
}