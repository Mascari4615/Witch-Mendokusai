using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(ItemData), menuName = "Variable/ItemData")]
public class ItemData : SpecialThing
{
    public Grade Grade => grade;
    public ItemType Type => type;
    public Recipe[] Recipes => recipes;
    public int MaxAmount => maxAmount;
    public bool IsCountable => MaxAmount != 1;
    
    [SerializeField] private Grade grade;
    [SerializeField] private ItemType type;
    [SerializeField] private Recipe[] recipes;
    [SerializeField] private int maxAmount = 500;

    public Item CreateItem()
    {
        return new Item(this);
    }
}

public enum ItemType
{
    Loot,
    Potion
}

public enum Grade
{
    Common,
    Uncommon,
    Rare,
    Legendary
}