using UnityEngine;
[CreateAssetMenu(fileName = nameof(Item), menuName = "Variable/Item")]
public class Item : SpecialThing
{
    public Grade grade;
    public ItemType type;
    public Recipe[] recipes;
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