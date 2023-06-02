using UnityEngine;
using UnityEngine.Serialization;

public abstract class SpecialThing : ScriptableObject
{
    [FormerlySerializedAs("Id")] public int ID;
    [FormerlySerializedAs("name")] public string Name;
    public string description;
    public Sprite sprite;
}

[System.Serializable]
public struct SpecialThingWithPercentage
{
    public SpecialThing specialThing;
    public float percentage;
}
