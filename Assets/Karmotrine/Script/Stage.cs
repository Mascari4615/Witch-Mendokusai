using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(Stage), menuName = "Variable/Stage")]
public class Stage : SpecialThing
{
    [FormerlySerializedAs("Enemies")] public SpecialThingWithPercentage[] specialThingWithPercentages;
    public Sprite background;
}