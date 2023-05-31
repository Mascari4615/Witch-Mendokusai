using UnityEngine;
public abstract class SpecialThing : ScriptableObject
{
    public int Id;
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
