using UnityEngine;
[CreateAssetMenu(fileName = nameof(Recipe), menuName = "Variable/Recipe")]
public class Recipe : ScriptableObject
{
    public Item[] ingredients;
    public float percentage;
}
