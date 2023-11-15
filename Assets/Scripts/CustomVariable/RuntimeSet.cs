using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
    [field: System.NonSerialized]
    public List<T> Items { get; } = new();

    public virtual void Add(T t)
    {
        // if (!Items.Contains(t))
        {
            Items.Add(t);
        }
    }

    public virtual void Remove(T t)
    {
        if (Items.Contains(t))
        {
            Items.Remove(t);
        }
    }

    public virtual void Clear()
    {
        if (Items == null) return;

        int count = Items.Count;
        for (int i = 0; i < count; i++)
        {
            Remove(Items[0]);
        }
    }
}