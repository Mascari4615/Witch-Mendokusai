using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(Stage), menuName = "Variable/Stage")]
public class Stage : Artifact
{
    public ArtifactWithPercentage[] SpecialThingWithPercentages => specialThingWithPercentages;
    public Sprite Background => background;

    [SerializeField] private ArtifactWithPercentage[] specialThingWithPercentages;
    [SerializeField] private Sprite background;
}