using UnityEngine;
using UnityEngine.Serialization;

public abstract class Artifact : ScriptableObject
{
    public int ID => id;
    public string Name => name;
    public string Description => description;
    public Sprite Thumbnail => thumbnail;
    
    [SerializeField] private int id;
    [SerializeField] private new string name;
    [SerializeField] private string description;
    [SerializeField] private Sprite thumbnail;
}

[System.Serializable]
public struct ArtifactWithPercentage
{
    public Artifact Artifact => artifact;
    public float Percentage => percentage;
    
    [SerializeField] private Artifact artifact;
    [SerializeField] private float percentage;
}
