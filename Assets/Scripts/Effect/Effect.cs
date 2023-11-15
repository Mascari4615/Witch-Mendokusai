using UnityEngine;

public abstract class Effect : Artifact
{
    public abstract void OnEquip();
    public abstract void OnRemove();
}