using UnityEngine;
[CreateAssetMenu(fileName = nameof(Recipe), menuName = "Variable/Recipe")]
public class Recipe : ScriptableObject
{
    public ItemData[] ingredients;
    public float percentage;
}
