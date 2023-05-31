using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyObject : MonoBehaviour
{
    public bool IsAlive => curHP != 0;
    [FormerlySerializedAs("playerItemRuntimeSet")] [FormerlySerializedAs("playerItemInventory")] [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private ClickerStageManager clickerStageManager;
    private Enemy curEnemy;
    private int curHP;
    
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpBarText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Init(Enemy enemy)
    {
        curEnemy = enemy;
        curHP = enemy.hp;
        
        nameText.text = enemy.name;
        hpBarText.text = $"{curHP} / {curEnemy.hp}";
        spriteRenderer.sprite = enemy.sprite;
        hpBar.fillAmount = 1;
    }

    public void ReceiveAttack(int damage)
    {
        curHP = Mathf.Clamp(curHP - damage, 0, int.MaxValue);
        
        hpBarText.text = $"{curHP} / {curEnemy.hp}";
        hpBar.fillAmount = (float)curHP / curEnemy.hp;

        if (curHP != 0)
            return;

        DropLoot();
        clickerStageManager.SpawnEnemy();
    }

    private void DropLoot()
    {
        Probability<Item> probability = new();
        foreach (var item in curEnemy.Loots)
            probability.Add(item.specialThing as Item, item.percentage);

        var newItem = probability.Get();
        playerInventory.Add(newItem);
    }
}
