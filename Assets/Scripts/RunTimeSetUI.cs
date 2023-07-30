using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

public abstract class RunTimeSetUI<T> : MonoBehaviour
{
    [SerializeField] private RunTimeSet<T> runTimeSet;
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
            if (i < runTimeSet.Items.Count)
            {
                slots[i].UpdateUI(runTimeSet.Items[i] as Artifact);
                // slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].UpdateUI(null);
                // slots[i].gameObject.SetActive(false);
            }
        }
    }
}