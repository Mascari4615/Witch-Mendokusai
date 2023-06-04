using UnityEngine;
[CreateAssetMenu(fileName = nameof(Recipe), menuName = "Variable/Recipe")]
public class Recipe : ScriptableObject
{
    public ItemData[] Ingredients => ingredients;
    public float Percentage => percentage;
    
    [SerializeField] private ItemData[] ingredients;
    [SerializeField] private  float percentage;
}
