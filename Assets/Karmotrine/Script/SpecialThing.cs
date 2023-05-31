using UnityEngine;
using UnityEngine.Serialization;

public abstract class SpecialThing : ScriptableObject
{
    [FormerlySerializedAs("Id")] public int ID;
    public string name;
    public string description;
    public Sprite sprite;
}

[System.Serializable]
public struct SpecialThingWithPercentage
{
    public SpecialThing specialThing;
    public float percentage;
}
