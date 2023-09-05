using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(ClickerStage), menuName = "ClickerStage")]
public class ClickerStage : Artifact
{
    public ArtifactWithPercentage[] SpecialThingWithPercentages => specialThingWithPercentages;
    public Sprite Background => background;

    [SerializeField] private ArtifactWithPercentage[] specialThingWithPercentages;
    [SerializeField] private Sprite background;
}