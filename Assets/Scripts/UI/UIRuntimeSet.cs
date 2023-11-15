using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

public abstract class UIRuntimeSet<T> : MonoBehaviour
{
    [SerializeField] private RuntimeSet<T> runtimeSet;
    [HideInInspector] public List<Slot> slots = new();

    private void Awake()
    {
        slots = GetComponentsInChildren<Slot>().ToList();

        for (int i = 0; i < slots.Count; i++)
            slots[i].SetSlotIndex(i);
    }

    private void OnEnable() => UpdateUI();

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < runtimeSet.Items.Count)
            {
                slots[i].UpdateUI(runtimeSet.Items[i] as Artifact);
                // slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].UpdateUI(null);
                // slots[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetRuntimeSet(RuntimeSet<T> newRuntimeSet)
    {
        runtimeSet = newRuntimeSet;
    }
}